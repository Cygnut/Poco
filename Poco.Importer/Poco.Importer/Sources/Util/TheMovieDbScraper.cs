using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Poco.Importer.Model;

namespace Poco.Importer.Sources.Util
{
    abstract class TheMovieDbScraper
    {
        public class TheMovieDbScraperSettings
        {
            public string ApiKey { get; set; } 
            public Category Category { get; set; } 
            public int MaxPages { get; set; } 
            public int ImageWidth { get; set; }
            // 100 is a safe level for api.themoviedb
            public int ThrottlingMs { get; set; }
        }

        protected TheMovieDbScraperSettings Settings { get; set; }

        const string API_BASE_PATH = "http://api.themoviedb.org/3";

        protected TheMovieDbScraper(TheMovieDbScraperSettings settings)
        {
            Settings = settings;
        }        

        protected abstract string GetGenresPath();
        protected abstract string DiscoverEntitiesPath(int page, string genres);
        protected abstract Entity Convert(dynamic entity, List<string> subCategories, string baseImageUrl);
        
        class TheMovieDbGenres
        {
            Dictionary<string, string> GenreLookup = new Dictionary<string, string>();

            public TheMovieDbGenres(dynamic genres)
            {
                foreach (var genre in genres.genres)
                    GenreLookup[genre.id.ToString()] = genre.name.ToString();
            }

            public List<string> GetGenreNames(dynamic genre_ids)
            {
                var names = new List<string>();

                foreach (var id in genre_ids)
                {
                    string name;
                    if (GenreLookup.TryGetValue(id.ToString(), out name))
                        names.Add(name);
                }

                return names;
            }
        }

        /* Another issue with images! Sigh!
         * If you just use: 
         * config["images"]["base_url"] (- leading /) + poster_path
         * E.g. http://image.tmdb.org/t/p/jIhL6mlT7AblhbHJgEoiBIOUVl1.jpg
         * You will get a "Format not Supported" - referring to the url format.
         * You need to use:
         * config["images"]["base_url"] + (an entry from config["images"]["poster_sizes"] array) + poster_path
         * E.g. http://image.tmdb.org/t/p/w500/jIhL6mlT7AblhbHJgEoiBIOUVl1.jpg
         * Which works!
        */

        class PosterSize
        {
            public string Value { get; set; }
            public int Size { get; set; }
        }

        // Contains, e.g. { "w300" => 300 }, ...
        List<PosterSize> ParsePosterSizes(dynamic posterSizes)
        {
            var ps = new List<PosterSize>();

            foreach (var i in posterSizes)
            {
                string posterSize = i.ToString();
                if (!posterSize.StartsWith("w")) continue;

                int size;
                if (!int.TryParse(posterSize.Substring(1), out size)) continue;

                ps.Add(new PosterSize()
                {
                    Value = posterSize,
                    Size = size,
                });
            }

            return ps;
        }

        string GetBestPosterSize(dynamic posterSizes, int preferredSize, string fallbackPosterSize)
        {
            // Parse the available poster sizes:
            List<PosterSize> ps = ParsePosterSizes(posterSizes);

            // Check if the preferred size is present:
            var pref = ps.FirstOrDefault(p => p.Size == preferredSize);
            if (pref != null) return pref.Value;

            // If there isn't one that's the preferred size, find the next closest.
            var closest = ps.OrderBy(p => Math.Abs(p.Size - preferredSize)).FirstOrDefault();
            if (closest != null) return closest.Value;

            // Oh. If we have no choice - fallback.
            return fallbackPosterSize;
        }

        string GetBaseImageUrl(ThrottledWebGet getter, int preferredSize, string fallbackPosterSize)
        {
            // Get the configuration object:
            var conf = getter.GetJsonObject(string.Format("{0}/configuration?api_key={1}", API_BASE_PATH, Settings.ApiKey));
            var imageConf = conf["images"];

            // Construct the base image url:
            return 
                imageConf["base_url"].ToString() + 
                GetBestPosterSize(imageConf["poster_sizes"], preferredSize, fallbackPosterSize);
        }

        public void AddPopularEntries(IEntities entities)
        {
            try
            {
                var getter = new ThrottledWebGet(Settings.ThrottlingMs);

                // Start by fetching the configuration - required to get the location where images are hosted (the base_url) which can change rarely
                // (see http://stackoverflow.com/questions/18280194/using-themoviedb-to-display-image-poster-with-php).
                // However - as long as the Poco.Importer is run periodically then we should catch those changes!

                string baseImageUrl = GetBaseImageUrl(getter, Settings.ImageWidth, "original");
                Trace.TraceInformation("Using base image url {0}", baseImageUrl);

                // Get all genres:
                var genres = getter.GetJsonObject(string.Format("{0}/{1}", API_BASE_PATH, GetGenresPath()));
                var genreLookup = new TheMovieDbGenres(genres);

                // Now iterate over each genre, getting the top $n selection of media for each genre, and store it in the result set.
                foreach (var genre in genres.genres)
                {
                    string genreName = genre.name.ToString();
                    string genreId = genre.id.ToString();

                    Trace.TraceInformation("Fetching {0} for genre '{1}' using {2} pages.", Settings.Category, genreName, Settings.MaxPages);

                    for (int page = 1; page <= Settings.MaxPages; ++page)
                    {
                        var url = string.Format("{0}/{1}", API_BASE_PATH, DiscoverEntitiesPath(page, genreId));

                        try
                        {
                            var entityPage = getter.GetJsonObject(url);

                            foreach (var result in entityPage.results)
                            {
                                entities.Add(
                                    Convert(result, genreLookup.GetGenreNames(result.genre_ids), baseImageUrl)
                                    );
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.TraceWarning("Failed to get {0} for url {1} with error: {2}", Settings.Category, url, e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to fetch {0} with error: {1}", Settings.Category, e);
            }
        }

        protected string GetImageUrl(dynamic entity, string baseImagePath)
        {
            return string.IsNullOrEmpty(entity.poster_path.ToString()) ? string.Empty : baseImagePath + entity.poster_path.ToString();
        }

        protected int? GetNativePopularity(dynamic entity)
        {
            return System.Convert.ToInt32(entity.popularity);
        }
    }
}
