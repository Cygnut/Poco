using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poco.Voter.Strategies.Util
{
    /// <summary>
    /// This object must be stored, rather than created when needed each time,
    /// as Random() creates a time-based seeded value, so in a tight loop this
    /// would create the possibility for the same value repeating!
    /// </summary>
    class WeightedRandom
    {
        Random Generator = new Random();

        public T Choose<T>(IEnumerable<T> col, Func<T, int> weighting)
        {
            if (col.Count() == 0) return default(T);

            var totalWeighting = col.Sum(weighting);

            int r = Generator.Next(0, totalWeighting);

            foreach (var c in col)
            {
                if (r < weighting(c))
                    return c;

                r -= weighting(c);
            }

            return col.Last();
        }
    }
}
