using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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

        [DataMember(Name = "searchKeywords")]
        public string SearchKeywords { get; set; }

        [DataMember(Name = "regExpr")]
        public string RegExpr { get; set; }

        [DataMember(Name = "tracingEnabled")]
        public bool TracingEnabled { get; set; }

        [BsonElement("ResourceIds")]
        private List<ObjectId> _resourceIds;

        [BsonIgnore]
        public List<ObjectId> ResourceIds {
            get
            {
                if (_resourceIds == null)
                {
                    _resourceIds = new List<ObjectId>();
                }
                return _resourceIds;
            }
        }

        [BsonElement("MediaFileIds")]
        private List<ObjectId> _mediaFileIds;

        [BsonIgnore]
        public List<ObjectId> MediaFileIds
        {
            get
            {
                if (_mediaFileIds == null)
                {
                    _mediaFileIds = new List<ObjectId>();
                }
                return _mediaFileIds;
            }
        }

        public bool IsTitleMatched(string title)
        {
            if (string.IsNullOrEmpty(RegExpr))
            {
                var keywords = SearchKeywords.Split(' ');
                foreach(var keyword in keywords)
                {
                    if (!title.Contains(keyword))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                Regex regEx = new Regex(RegExpr.ToUpper());
                return regEx.Match(title.ToUpper()).Success;
            }
        }
    }
}
