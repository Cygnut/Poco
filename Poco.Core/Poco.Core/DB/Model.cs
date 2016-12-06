using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poco.Core.DB
{
    public class UpdateEntityResult
    {
        public bool InsertOccurred { get; set; }
        public bool UpdateOccurred { get; set; }
    }

    public class GetPostImportStatsResult
    {
        public class Item
        {
            public string Category { get; set; }
            public int Count { get; set; }
        }

        public List<Item> Items { get; set; }

        public GetPostImportStatsResult()
        {
            Items = new List<Item>();
        }
    }

    public class Entity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        // Don't need these for now - would just take up extra memory.
        //public string SubCategory { get; set; }
        //public string Blurb { get; set; }
        //public string ImageUrl { get; set; }
        public int NativePopularity { get; set; }
        public int Score { get; set; }
        public int TotalVotes { get; set; }
    }
}
