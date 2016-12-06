using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Poco.Importer.Model;

namespace Poco.Importer.Sources
{
    class Entities : IEntities
    {
        List<Entity> Items = new List<Entity>();
        bool Log;

        public Entities(bool log = false)
        {
            Log = log;
        }

        List<Entity> ReduceByCategory(List<Entity> entities)
        {
            return entities
                .GroupBy(e => e.Category)
                .SelectMany(
                    c => c.GroupBy(g => g.Name).Select(n => n.First())
                        )
                .ToList();
        }

        public List<Entity> Get()
        {
            return ReduceByCategory(Items);
        }

        public void Add(Entity entity)
        {
            if (Log)
                Trace.TraceInformation("Adding {0}.{1}", entity.Category, entity.Name);

            Items.Add(entity);
        }
    }
}
