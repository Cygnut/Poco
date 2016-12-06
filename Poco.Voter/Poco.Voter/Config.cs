using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poco.Voter
{
    using Core.Configuration;

    public class Config
    {
        [ConfigurationEntry("DatabaseConnectionString", "Connection string to the MySQL database to insert into.")]
        public string DatabaseConnectionString = null;

        [ConfigurationEntry("CacheRefreshRate", "Time between successive cache refreshes, in ms.")]
        public int CacheRefreshRate = (int)TimeSpan.FromMinutes(15).TotalMilliseconds;
        [ConfigurationEntry("VoterRate", "Time between each successive vote, in ms.")]
        public int VoterRate = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;

        [ConfigurationEntry("VoterStrategy", "Options include SlowSeed, FastSeed, Rebalance.")]
        public string VoterStrategy = "Rebalance";
        [ConfigurationEntry("SeedMin", "For VoteMode=*Seed: The minimum that scores will be adjusted to.")]
        public int SeedMin = -1000;
        [ConfigurationEntry("SeedMax", "For VoteMode=*Seed: The maximum that scores will be adjusted to.")]
        public int SeedMax = 1000;
        [ConfigurationEntry("SeedWaverRatio", "For VoteMode=*Seed: The ratio of votes to the delta made to simulate real voting.")]
        public double SeedWaverRatio = 0.05;    // 5%
        [ConfigurationEntry("SeedWaverMin", "For VoteMode=*Seed: The minimum number of votes made to simulate real voting.")]
        public int SeedWaverMin = 20;
    }
}
