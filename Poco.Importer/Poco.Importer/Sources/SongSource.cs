using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Poco.Importer.Model;
using Newtonsoft.Json.Linq;

namespace Poco.Importer.Sources
{
    using Util;

    class TrackSource : ISource
    {
        public Category Category { get { return Category.Song; } }
        int ImageWidth { get; set; }
        int MaxResultsPerGenre { get; set; }   

        string[] Genres = new string[]
        {
            "Country",
            "Disco",
            "Folk",
            "Hip-Hop",
            "House",
            "Jazz",
            "Metal",
            "Pop",
            "Punk",
            "Rap",
            "Reggae",
            "Rock",
        };

        const string API_BASE_PATH = "https://api.spotify.com/v1";
        ThrottledWebGet Getter { get; set; }

        public void Initialise(Config config) 
        {
            MaxResultsPerGenre = config.SongMaxResultsPerGenre;
            ImageWidth = config.SongImageWidth;
            // Apparently we can get away with 240 requests per minute - but for now, let's just settle with 120 to be safe.
            Getter = new ThrottledWebGet(config.SongGetThrottling);
        }

        class Image
        {
            public string Url { get; set; }
            public int Width { get; set; }

            static int Parse(string val, int def)
            {
                int t;
                return int.TryParse(val, out t) ? t : def;
            }

            public static Image Parse(dynamic image)
            {
                return new Image()
                {
                    Url = image["url"].ToString(),
                    Width = Parse(image["width"].ToString(), 0),
                };
            }
        }

        string GetBestImageUrl(dynamic track)
        {
            // Album images are stored in track.album.images.

            /*
             *  "images": 
             *  [ 
             *      {
             *          "height": 640,
             *          "url": "https://i.scdn.co/image/f2798ddab0c7b76dc2d270b65c4f67ddef7f6718",
             *          "width": 640
             *      },
             *      ...
             *  ]
             */

            // We want the one that has the closest image width to this.ImageWidth.
            var images = ((IEnumerable<dynamic>)track.album.images).Select(Image.Parse);
            var bestImage = images.OrderBy(i => Math.Abs(ImageWidth - i.Width)).FirstOrDefault();
            return bestImage != null ? bestImage.Url : string.Empty;
        }

        Entity Convert(dynamic track, string genre)
        {
            string primaryArtist = track.artists.Count != 0 ? track.artists[0].name : "?";

            // For future use:
            string previewUrl = track.preview_url;

            return new Entity
            {
                Name = string.Format("{0} by {1}", track.name, primaryArtist),  // We use the artist to attempt to mitigate the issue of non-unique track names.
                Category = Category,
                SubCategory = new List<string> { genre },
                Blurb = track.album.name,
                ImageUrl = GetBestImageUrl(track),
                NativePopularity = track.popularity,
            };
        }

        string GetTracksByGenreUrl(string genre, int offset, int limit)
        {
            // The prototype is https://api.spotify.com/v1/search?query=genre%3Arock&offset=0&limit=20&type=track
            return string.Format("{0}/search?query=genre%3A{1}&offset={2}&limit={3}&type=track",
                API_BASE_PATH,
                genre,
                offset,
                limit);
        }

        int GetTotalTracksForGenre(string genre)
        {
            // First we need to know the number of tracks available - to make sure we don't request over the limit.
            string url = GetTracksByGenreUrl(genre, 0, 1);

            var tracks = Getter.GetJsonObject(url);
            return System.Convert.ToInt32(tracks.tracks.total.ToString());
        }

        class Chunk
        {
            public int Offset { get; set; }
            public int Limit { get; set; }
        }
        
        int RESULTS_PER_PAGE = 50;

        IEnumerable<Chunk> ChunkRange(int offset, int limit)
        {
            return Enumerable
                .Range(offset, limit)
                .GroupBy(i => i / RESULTS_PER_PAGE)
                .Select(i => new Chunk
                {
                    Offset = i.Min(),
                    Limit = i.Count(),
                });
        }


        /*
         * We're going to ask for a certain number of results from each hardcoded genre, using the prototype: 
         * https://api.spotify.com/v1/search?query=genre%3Arock&offset=0&limit=20&type=track
         * I would have preferred to find an endpoint that gives you genres, but it seems like the search endpoint is pretty forgiving and smart - 
         * i.e. it's searching, not using foreign keys or something like that.
         * 
         */

        void ProcessGenre(IEntities entities, string genre)
        {
            int genreTotal = 0;
            try
            {
                // First we need to know the number of tracks available - to make sure we don't request over the limit.
                genreTotal = GetTotalTracksForGenre(genre);                
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Failed to get track information for genre {0} with error: {1}", genre, e);
                return;
            }

            // Need to break up the number of results required into chunks of pages of size 50, as required by the api.
            int resultsCount = Math.Min(genreTotal, MaxResultsPerGenre);
            Trace.TraceInformation("Fetching songs for genre '{0}' using {1} results.", genre, resultsCount);

            var chunks = ChunkRange(0, resultsCount);
                        
            foreach (var chunk in chunks)
            {                
                string url = GetTracksByGenreUrl(genre, chunk.Offset, chunk.Limit);

                try
                {
                    var tracks = Getter.GetJsonObject(url);

                    for (int i = 0; i < chunk.Limit; ++i)
                    {
                        var track = tracks.tracks.items[i];

                        if (track != null)
                            entities.Add(Convert(track, genre));
                    }

                    // If tracks.tracks.next is null, then there are no further pages, so we're done here.
                    if (tracks.tracks.next == null)
                        return;
                }
                catch (Exception e)
                {
                    Trace.TraceWarning("Failed to get tracks for url {0} with error: {1}", url, e);
                }
            }
        }

        public void AddPopularEntities(IEntities entities)
        {
            try
            {
                foreach (var genre in Genres)
                    ProcessGenre(entities, genre);
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to fetch tracks with error: {0}", e);
            }
        }
    }
}
