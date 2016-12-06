using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poco.Voter.Strategies
{
    using Core.DB;

    /// <summary>
    /// Vote for entities such that they have a score which is scoreMin + popularity * (scoreMax - scoreMin).
    /// This mode is useful for periodically rebalancing scores appropriate to their native popularity, or simulating a voter.
    /// </summary>
    [StrategyName("Rebalance")]
    class RebalanceStrategy : TimeDistributedStrategy
    {
        protected override ScoreRange GetScoreRange(IEnumerable<Entity> entities)
        {
            return new ScoreRange(
                entities.Min(e => e.Score),
                entities.Max(e => e.Score)
            );
        }
    }
}
