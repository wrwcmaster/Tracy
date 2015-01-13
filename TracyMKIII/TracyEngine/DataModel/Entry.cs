using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    public class Entry
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string SearchKeywords { get; set; }
        public string RegExpr { get; set; }
        public bool TracingEnabled { get; set; }
        public List<ObjectId> ResourceIds { get; set; }
        public List<ObjectId> MediaFileIds { get; set; }
        public bool IsTitleMatched(string title)
        {
            Regex regEx = new Regex(RegExpr.ToUpper());
            return regEx.Match(title.ToUpper()).Success;
        }
    }
}
