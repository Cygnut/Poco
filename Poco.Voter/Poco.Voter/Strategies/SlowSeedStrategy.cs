using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poco.Voter.Strategies
{
    /// <summary>
    /// Vote for entities such that they have a score which is seedMin + popularity * (seedMax - seedMin)
    /// This mode is useful for initially seeding scores over a range [seedMin, seedMax] proportional to their native popularity.
    /// </summary>
    [StrategyName("SlowSeed")]
    class SlowSeedStrategy : TimeDistributedStrategy
    {
        Random WaverGenerator = new Random();

        protected override void DoAdditionalVotes(EntityInfo e)
        {
            // Slowly waver votes (i.e. 1 call to SP per vote).
            for (int i = 0; i < GetWaverVotes(e); ++i)
                Vote(e, WaverGenerator.NextBoolean(), 1);
        }
    }
}
