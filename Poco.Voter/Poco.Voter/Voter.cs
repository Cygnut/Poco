using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Poco.Voter
{
    using Core;
    using Core.DB;
    using Strategies;
    
    class Voter
    {
        public void Execute(Config configuration, CancellationTokenSource cancel)
        {
            Strategy.GetStrategy(
                configuration.VoterStrategy,
                new DBClient(configuration.DatabaseConnectionString),
                configuration)
                .Execute(cancel);
        }
    }
}
