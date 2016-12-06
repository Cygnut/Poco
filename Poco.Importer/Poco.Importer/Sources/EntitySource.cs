using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Poco.Importer.Model;

namespace Poco.Importer.Sources
{
    class EntitySource
    {
        List<Type> GetTypesImplementing<TInterface>()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(
                    t => typeof(TInterface).IsAssignableFrom(t) && !t.IsAbstract
                    )
                .ToList();
        }

        List<ISource> GetSources(Config config)
        {
            // Create an instance of each source and initialize it.
            return GetTypesImplementing<ISource>().Select(t =>
            {
                var s = (ISource)t.GetConstructor(new Type[] { }).Invoke(new Type[] { });
                s.Initialise(config);
                return s;
            }).ToList();
        }

        public List<Entity> GetPopularEntities(Config config)
        {
            // Create an instance of each source and initialize it.
            var sources = GetSources(config);

            // If we only want one category to be done, then do this.
            if (config.CategoryFilter.HasValue)
            {
                Trace.TraceWarning("CATEGORY FILTER HAS BEEN SET TO '{0}'", config.CategoryFilter);
                sources = sources.Where(s => s.Category == config.CategoryFilter).ToList();
            }

            var e = new Entities();

            // Source our entities.
            sources
                .OrderBy(s => s.Category)   // For repeatability
                .ToList()
                .ForEach(s => 
                    {
                        try
                        {
                            s.AddPopularEntities(e);
                        }
                        catch (Exception er)
                        {
                            Trace.TraceError("Source {0} failed with error {1}.", s.Category, er);
                        }
                    });

            // Remove duplicates within each category.
            return e.Get();
        }
    }
}
