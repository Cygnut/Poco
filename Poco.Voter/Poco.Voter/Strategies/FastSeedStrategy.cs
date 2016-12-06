using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Poco.Voter.Strategies
{
    /// <summary>
    /// Same as SlowSeed, but instead of spreading out votes over time, does all at once.
    /// </summary>
    [StrategyName("FastSeed")]
    class FastSeedStrategy : Strategy
    {
        Random Generator = new Random();

        void FastWaverVote(EntityInfo e)
        {
            // Faster version of SlowSeedStrategy wavering.
            int waverVotes = GetWaverVotes(e);

            // Work out how many of the votes are upvotes, how many are downvotes.
            int upVotes = Generator.Next(0, waverVotes);
            int downVotes = waverVotes - upVotes;
            int netVotes = upVotes - downVotes;

            // Then enact that.
            Vote(e, netVotes > 0, Math.Abs(netVotes));
        }

        protected override void ExecuteMethod(CancellationToken cancel)
        {
            try
            {
                for (int i = 0; i < Cache.Entities.Count; ++i)
                {
                    cancel.ThrowIfCancellationRequested();

                    var e = Cache.Entities[i];

                    try
                    {
                        Vote(e, e.DeltaSign > 0, e.DeltaModulus);
                        FastWaverVote(e);
                        Trace.TraceInformation("{0}/{1}: Finished voting {2} to {3}.", i + 1, Cache.Entities.Count, e.Entity.Name, e.ExpectedScore);
                    }
                    catch (Exception er)
                    {
                        Trace.TraceError("Failed to vote on entity {0} with name {1} with error {2}.", e.Entity.Id, e.Entity.Name, er);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
