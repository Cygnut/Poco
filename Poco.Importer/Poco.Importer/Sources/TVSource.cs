using System.Collections.Generic;
using Poco.Importer.Model;

namespace Poco.Importer.Sources
{
    using Util;

    class TVSource : ISource
    {
        public Category Category { get { return Category.TV; } }

        TheMovieDbTVScraper Scraper;

        public void Initialise(Config config)
        {
            Scraper = new TheMovieDbTVScraper(Category, config);
        }

        class TheMovieDbTVScraper : TheMovieDbScraper
        {
            public TheMovieDbTVScraper(Category category, Config c)
                : base(new TheMovieDbScraperSettings
                {
                    ApiKey = c.TheMovieDbApiKey,
                    Category = category,
                    MaxPages = c.TVMaxPages,
                    ImageWidth = c.TVImageWidth,
                    ThrottlingMs = c.TVGetThrottling
                })
            {
            }

            protected override string GetGenresPath()
            {
                return string.Format("genre/tv/list?api_key={0}", Settings.ApiKey);
            }

            protected override string DiscoverEntitiesPath(int page, string genres)
            {
                return string.Format("discover/tv?page={0}&with_genres={1}&sort_by=popularity.desc&api_key={2}",
                    page,
                    genres,
                    Settings.ApiKey);
            }

            protected override Entity Convert(dynamic entity, List<string> subCategories, string baseImagePath)
            {
                return new Entity()
                {
                    Name = entity.name.ToString(),
                    Category = Settings.Category,
                    SubCategory = subCategories,
                    Blurb = entity.overview,
                    ImageUrl = GetImageUrl(entity, baseImagePath),
                    NativePopularity = GetNativePopularity(entity),

                };
            }
        }

        public void AddPopularEntities(IEntities entities)
        {
            // I can't find a way to just request a lit of all tv.
            // So instead, we'll get the list of movie genres, and get a list of tv for each genre.
            // That should give us a sufficient amount of data.

            // There is no equivalent genre/{tv genre id}/tv endpoint, so we'll have to use discovery instead.

            // So we go:
            // genre/tv/list
            // discover/tv?page=&with_genres=&sort_by=popularity.desc

            Scraper.AddPopularEntries(entities);
        }
    }
}
