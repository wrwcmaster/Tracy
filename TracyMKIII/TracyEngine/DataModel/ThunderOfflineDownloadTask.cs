using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Tracy.DataModel
{
    [DataContract]
    public class ThunderOfflineDownloadTask : BaseMongoModel
    {
        public ObjectId ResourceId { get; set; }
        public ObjectId EntryId { get; set; }
        public string Cid { get; set; }
        public long TaskId { get; set; }
        public List<ObjectId> FileIds { get; set; }
        public int Status { get; set; }
        public int FailCount { get; set; }
    }
}
