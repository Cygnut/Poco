using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace Poco.Importer
{
    using Core.Configuration;
    using Model;

    class Config
    {
        [ConfigurationEntry("DatabaseConnectionString", "Connection string to the MySQL database to insert into.")]
        public string DatabaseConnectionString = null;

        [ConfigurationEntry("TheMovieDbApiKey", "Api key to TheMovieDb api.")]
        public string TheMovieDbApiKey = null;

        [ConfigurationEntry("CategoryFilter", "Filter the categories to process.")]
        public Category? CategoryFilter = null;

        [ConfigurationEntry("NativePopularityFallback", "Fallback value from 0-100 to use for NativePopularity if not supplied")]
        public int NativePopularityFallback = 45;

        // Production defaults.
        [ConfigurationEntry("FilmMaxPages", "Max pages to collect in the Film source.")]
        public int FilmMaxPages = 5;
        [ConfigurationEntry("FilmImageWidth", "Target image width to use for the Film source.")]
        public int FilmImageWidth = 300;
        [ConfigurationEntry("FilmGetThrottling", "Throttling to use for GETs in ms for the Film source.")]
        public int FilmGetThrottling = 100;

        [ConfigurationEntry("TVMaxPages", "Max pages to collect in the TV source.")]
        public int TVMaxPages = 5;
        [ConfigurationEntry("TVImageWidth", "Target image width to use for the TV source.")]
        public int TVImageWidth = 300;
        [ConfigurationEntry("TVGetThrottling", "Throttling to use for GETs in ms for the TV source.")]
        public int TVGetThrottling = 100;


        [ConfigurationEntry("SongMaxResultsPerGenre", "Max results per genre to collect in the Song source.")]
        public int SongMaxResultsPerGenre = 50;
        [ConfigurationEntry("SongImageWidth", "Target image width to use for the Song source.")]
        public int SongImageWidth = 300;
        [ConfigurationEntry("SongGetThrottling", "Throttling to use for GETs in ms for the Song source.")]
        public int SongGetThrottling = 500;
    }
}
