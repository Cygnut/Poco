using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poco.Importer.Model;

namespace Poco.Importer.Sources
{
    interface IEntities
    {
        void Add(Entity entity);
    }

    static class IEntitiesExtensions
    {
        public static void AddRange(this IEntities e, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                e.Add(entity);
        }
    }
}
