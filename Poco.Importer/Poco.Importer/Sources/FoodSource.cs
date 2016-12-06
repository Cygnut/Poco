using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Poco.Importer.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Poco.Importer.Sources
{
    using Util;

    class FoodSource : ISource
    {
        public Category Category { get { return Category.Food; } }

        public void Initialise(Config config)
        {   
        }

        JObject DeserializeJsonStream(Stream s)
        {
            using (var sr = new StreamReader(s))
            using (var json = new JsonTextReader(sr))
            {
                return JObject.Load(json);
                //return new JsonSerializer().Deserialize(json);
            }
        }

        public void AddPopularEntities(IEntities entities)
        {
            // Parse the embedded json file listing all foodstuffs.
            dynamic json = DeserializeJsonStream(Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("Poco.Importer.Sources.Food.json")
                );

            // Return them all translated into our object model.
            foreach (var item in json.items)
            {
                entities.Add(new Entity()
                {
                    Name = item.name.ToString(),
                    Category = Category,
                    SubCategory = new List<string> { item.sub_category.ToString() },
                    Blurb = item.blurb.ToString(),
                    ImageUrl = item.image_url.ToString(),
                    NativePopularity = item.popularity,
                });
            }
        }
    }
}
