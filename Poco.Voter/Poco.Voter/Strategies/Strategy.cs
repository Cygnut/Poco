using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;

namespace Poco.Voter.Strategies
{
    using Core;
    using Core.DB;

    public class StrategyNameAttribute : Attribute
    {
        public string Name { get; set; }

        public StrategyNameAttribute(string name)
        {
            Name = name;
        }
    }

    abstract class Strategy
    {
        DBClient Client { get; set; }
        protected Config Configuration { get; set; }

        protected void Vote(EntityInfo e, bool up, int by)
        {
            Client.VoteEntity(e.Entity.Id, up, by);
        }

        public void Initialize(DBClient client, Config configuration)
        {
            Client = client;
            Configuration = configuration;
        }

        protected class ScoreRange
        {
            public int Min { get; set; }
            public int Max { get; set; }

            public ScoreRange(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public int GetExpectedScore(double p)
            {
                return Min + (int)(p * (Max - Min));
            }
        }

        protected virtual ScoreRange GetScoreRange(IEnumerable<Entity> entities)
        {
            return new ScoreRange(
                Configuration.SeedMin,
                Configuration.SeedMax
                );
        }

        protected class EntityInfo
        {
            public Entity Entity { get; set; }

            public int ExpectedScore { get; set; }

            // Don't store these values, make them dynamic so we can update either base values and the below will stay correct.
            int Delta { get { return ExpectedScore - Entity.Score; } }
            public int DeltaModulus { get { return Math.Abs(Delta); } }
            public int DeltaSign { get { return Math.Sign(Delta); } }
        }

        protected class StrategyCache
        {
            public List<EntityInfo> Entities { get; set; }
        }

        protected StrategyCache Cache = new StrategyCache();

        protected void RefreshCache()
        {
            // 1. Get the set of all entities, with their score, popularity, category.
            // This needs to be periodically refreshed.
            var entities = Client.GetEntities();

            // 3. Get the max and min global score.
            var scoreRange = GetScoreRange(entities);

            // 2. Get the max and min popularity within each category, and normalise to double [0-1]
            Cache.Entities = entities
                .GroupBy(e => e.Category)
                .SelectMany(category =>
                {
                    // Renormalize popularity within each category, so now across all entities
                    var min = category.Min(e => e.NativePopularity);
                    var max = category.Max(e => e.NativePopularity);
                    var range = max - min;

                    return category.Select(e =>
                    {
                        // Get a global popularity - normalized in [0,1].
                        var normalizedPopularity = 
                            range != 0
                            ?
                            ((double) (e.NativePopularity - min)) / range
                            :
                            0.5;

                        // Now all entities have a normalized popularity from [0-1].
                        // Use this to get the expected score for each entity, which is our guideline.
                        return new EntityInfo()
                        {
                            Entity = e,
                            ExpectedScore = scoreRange.GetExpectedScore(normalizedPopularity),
                        };
                    });
                })
                .ToList();
        }

        protected abstract void ExecuteMethod(CancellationToken cancel);

        Thread _thread;

        public void Execute(CancellationTokenSource cancel)
        {
            _thread = new Thread(() =>
                {
                    // Ensure the cache is initialized.
                    RefreshCache();

                    ExecuteMethod(cancel.Token);
                });

            _thread.Start();
            _thread.Join();
        }

        protected int GetWaverVotes(EntityInfo e)
        {
            return (int)Math.Max(Configuration.SeedWaverMin, Configuration.SeedWaverRatio * e.DeltaModulus);
        }

        static Dictionary<string, Type> GetStrategies()
        {
            // Get all concrete implementations of Strategy.
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(
                    t => 
                        typeof(Strategy).IsAssignableFrom(t) 
                        && 
                        !t.IsAbstract
                        &&
                        t.GetCustomAttributes(typeof(StrategyNameAttribute), true).Any()
                )
                .ToDictionary(t => 
                    {
                        var attr = t.GetCustomAttributes(typeof(StrategyNameAttribute), true).FirstOrDefault() as StrategyNameAttribute;
                        return attr.Name;
                    });
        }

        /// <summary>
        /// Create an instance of the concrete Strategy with matching StrategyName.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Strategy GetStrategy(string strategyName, DBClient client, Config configuration)
        {
            var strategies = GetStrategies();

            Type type;
            if (!strategies.TryGetValue(strategyName, out type))
                throw new Exception(string.Format("No such strategy with name {0}. Currently supported: {1}", strategyName, GetStrategyNames()));

            // Instantiate it already.
            var strategy = (Strategy) type.GetConstructor(new Type[] { }).Invoke(new Type[] { });
            strategy.Initialize(client, configuration);
            return strategy;
        }

        public static string[] GetStrategyNames()
        {
            return GetStrategies().Keys.ToArray();
        }
    }

    public static class RandomExtensions
    {
        public static bool NextBoolean(this Random r)
        {
            return r.Next(0, 1) == 0;
        }
    }
}
