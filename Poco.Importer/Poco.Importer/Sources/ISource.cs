using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poco.Importer.Model;

namespace Poco.Importer.Sources
{
    interface ISource
    {
        Category Category { get; }

        void Initialise(Config config);

        void AddPopularEntities(IEntities entities);
    }
}
