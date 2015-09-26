using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Tracy.DataModel
{
    [DataContract]
    public class ThunderOfflineDownloadTask : BaseMongoModel
    {
        public ObjectId ResourceId { get; set; }

        public ObjectId EntryId { get; set; }
        [DataMember(Name = "thunderCid")]
        public string Cid { get; set; }
        [DataMember(Name = "thunderTaskId")]
        public long TaskId { get; set; }
        public List<ObjectId> FileIds { get; set; }
        [DataMember(Name = "status")]
        public int Status { get; set; }
        [DataMember(Name = "failCount")]
        public int FailCount { get; set; }

        [BsonIgnore]
        private Resource _res = null;
        [BsonIgnore]
        [DataMember(Name = "resource")]
        public Resource Resource
        {
            get
            {
                if (_res == null)
                {
                    _res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(ResourceId); //TODO: manually init this
                }
                return _res;
            }
            set
            {
                _res = value;
            }
        }
    }
}
