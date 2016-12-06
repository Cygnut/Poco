using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Poco.Voter.Strategies
{
    using Util;

    abstract class TimeDistributedStrategy : Strategy
    {
        WeightedRandom Chooser = new WeightedRandom();

        protected virtual void DoAdditionalVotes(EntityInfo e) { }

        void RandomVote()
        {
            // Pick an entity randomly.
            // The choosing algorithm should be weighted so entities with large moduli are selected more often,
            // and entities with small or zero moduli should be rarely selected.
            var e = Chooser.Choose(Cache.Entities, entity => entity.DeltaModulus);

            try
            {
                // Vote it into the direction that would put it closer to expected.
                Vote(e, e.DeltaSign > 0, 1);
                DoAdditionalVotes(e);

                // Log what we've done.
                Trace.TraceInformation("Voted {0} from score {1} towards expected {2}.",
                    e.Entity.Name,
                    e.Entity.Score,
                    e.ExpectedScore);
            }
            catch (Exception er)
            {
                Trace.TraceError("Failed to vote on entity {0} with name {1} with error {2}.", e.Entity.Id, e.Entity.Name, er);
            }
        }

        protected override void ExecuteMethod(CancellationToken cancel)
        {
            using (var disp = new TimedDispatcher())
            {
                disp.Add(
                    () => base.RefreshCache(),
                    Configuration.CacheRefreshRate,
                    Configuration.CacheRefreshRate);

                disp.Add(
                    () => RandomVote(),
                    Configuration.VoterRate,
                    Configuration.VoterRate);

                disp.Run(cancel);
            }
        }
    }
}
