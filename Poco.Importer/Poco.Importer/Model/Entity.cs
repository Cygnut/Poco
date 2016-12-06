using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Poco.Importer.Model
{
    [DebuggerDisplay("{Name}")]
    public class Entity
    {
        public string Name { get; set; }
        public Category Category { get; set; }
        public List<string> SubCategory { get; set; }
        public string Blurb { get; set; }
        public string ImageUrl { get; set; }
        // A score which represents the rough popularity of that item within that category.
        // A larger number indicates a higher popularity. Does not need to be in any numeric range which
        // matches any other category - just consistent within that category.
        // E.g. For music, could be 1-10, for film 1-100.
        public int? NativePopularity { get; set; }
    }
}
