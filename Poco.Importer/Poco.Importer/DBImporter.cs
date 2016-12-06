using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Poco.Importer.Model;

namespace Poco.Importer
{
    using Core;

    class DBImporter
    {
        Core.DB.DBClient Client;
        int NativePopularityFallback;

        public DBImporter(string connectionString, int nativePopularityFallback)
        {
            Client = new Core.DB.DBClient(connectionString);
            NativePopularityFallback = nativePopularityFallback;
        }
        
        class Categories
        {
            Dictionary<Category, int> Lookup = new Dictionary<Category, int>();

            public void Add(Category category, int id)
            {
                Lookup[category] = id;
            }

            public int Get(Category category)
            {
                return Lookup[category];
            }
        }

        Categories UpdateEntityCategory(IEnumerable<Entity> entities)
        {
            var categories = entities.GroupBy(e => e.Category).Select(c => c.Key);

            var l = new Categories();

            foreach (var category in categories)
            {
                l.Add(
                    category,
                    Client.UpdateEntityCategory(category.ToString())
                    );
            }

            return l;
        }

        class ImportProgress
        {
            public int Total { get; set; }
            public int Done { get; set; }
                        
            class CategoriesInfo
            {
                public class CategoryInfo
                {
                    public int Inserts { get; set; }
                    public int Updates { get; set; }
                }

                Dictionary<Category, CategoryInfo> Categories = new Dictionary<Category, CategoryInfo>();

                public CategoryInfo GetCategoryInfo(Category category)
                {
                    CategoryInfo ci;
                    return
                        Categories.TryGetValue(category, out ci)
                        ?
                        ci
                        :
                        Categories[category] = new CategoryInfo();
                }

                public int GetTotalInserts()
                {
                    return Categories.Values.Sum(c => c.Inserts);
                }

                public int GetTotalUpdates()
                {
                    return Categories.Values.Sum(c => c.Updates);
                }

                public void LogCategoryBreakdown()
                {
                    foreach (var c in Categories)
                        Trace.TraceInformation("{0}: Total of {1} inserts, {2} updates.", c.Key, c.Value.Inserts, c.Value.Updates);
                }
            }

            CategoriesInfo Categories = new CategoriesInfo();

            const int REPORT_RATE = 20;

            public ImportProgress(int total)
            {
                Total = total;
                Done = 0;
                Trace.TraceInformation("Importing {0} entities into entity table.", Total);
            }

            public void OnImported()
            {
                ++Done;

                if (Done % REPORT_RATE == 0 || Done == Total)
                    Trace.TraceInformation("{0:0.0}% {1} inserts, {2} updates.", 
                        (Done * 100.0f / Total), 
                        Categories.GetTotalInserts(),
                        Categories.GetTotalUpdates()
                        );
            }

            public void OnInsert(Category category)
            {
                Categories.GetCategoryInfo(category).Inserts++;
            }

            public void OnUpdate(Category category)
            {
                Categories.GetCategoryInfo(category).Updates++;
            }

            public void Complete()
            {
                Trace.TraceInformation("Imported {0} entities into entity table.", Total);

                Categories.LogCategoryBreakdown();

                Trace.TraceInformation("Total of {0} inserts, {1} updates.", 
                    Categories.GetTotalInserts(), 
                    Categories.GetTotalUpdates()
                    );
            }
        }

        void UpdateEntities(Categories categories, IEnumerable<Entity> entities)
        {
            if (!entities.Any()) return;

            Trace.TraceInformation("Inserting {0} entities into entity table.", entities.Count());

            var prog = new ImportProgress(entities.Count());

            foreach (var e in entities)
            {
                var result = Client.UpdateEntity(
                    e.Name,
                    categories.Get(e.Category),
                    string.Join(", ", e.SubCategory),
                    e.Blurb,
                    e.ImageUrl,
                    e.NativePopularity ?? NativePopularityFallback
                    );

                if (result.InsertOccurred) prog.OnInsert(e.Category);
                if (result.UpdateOccurred) prog.OnUpdate(e.Category);

                prog.OnImported();
            }

            prog.Complete();
        }
        
        void LogPostImportStats()
        {
            Trace.TraceInformation("Post import stats:");

            var result = Client.GetPostImportStats();

            foreach (var i in result.Items)
                Trace.TraceInformation("{0}:{1} exist", i.Category, i.Count);
        }

        public void Import(IEnumerable<Entity> entities)
        {
            var categories = UpdateEntityCategory(entities);
            UpdateEntities(categories, entities);
            LogPostImportStats();
        }
    }
}
