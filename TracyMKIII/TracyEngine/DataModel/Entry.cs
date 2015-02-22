using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    [DataContract]
    public class Entry : BaseMongoModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "search-keywords")]
        public string SearchKeywords { get; set; }

        [DataMember(Name = "reg-expr")]
        public string RegExpr { get; set; }

        [DataMember(Name = "tracing-enabled")]
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
