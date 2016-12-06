using System.Collections.Generic;
using Poco.Importer.Model;

namespace Poco.Importer.Sources
{
    using Util;

    class FilmSource : ISource
    {
        public Category Category { get { return Category.Film; } }

        TheMovieDbScraper Scraper;

        public void Initialise(Config config)
        {
            Scraper = new TheMovieDbFilmScraper(Category, config, false);
        }

        class TheMovieDbFilmScraper : TheMovieDbScraper
        {
            bool IncludeAdult;

            public TheMovieDbFilmScraper(Category category, Config c, bool includeAdult)
                : base(new TheMovieDbScraperSettings
                {
                    ApiKey = c.TheMovieDbApiKey,
                    Category = category,
                    MaxPages = c.FilmMaxPages,
                    ImageWidth = c.FilmImageWidth,
                    ThrottlingMs = c.FilmGetThrottling
                })
            {
                IncludeAdult = includeAdult;
            }

            protected override string GetGenresPath()
            {
                return string.Format("genre/movie/list?api_key={0}", Settings.ApiKey);
            }

            protected override string DiscoverEntitiesPath(int page, string genres)
            {
                return string.Format("discover/movie?page={0}&with_genres={1}&sort_by=popularity.desc&include_adult={2}&api_key={3}",
                    page,
                    genres,
                    IncludeAdult.ToString().ToLower(),
                    Settings.ApiKey);
            }

            protected override Entity Convert(dynamic entity, List<string> subCategories, string baseImagePath)
            {
                return new Entity
                {
                    Name = entity.title.ToString(),
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
            // I can't find a way to just request a lit of all movies.
            // So instead, we'll get the list of movie genres, and get a list of movies for each genre.
            // That should give us a sufficient amount of data.
            
            // So we go:
            // genre/movie/list
            // discover/movie?page=&with_genres=&sort_by=popularity.desc&include_adult

            Scraper.AddPopularEntries(entities);
        }
    }
}
