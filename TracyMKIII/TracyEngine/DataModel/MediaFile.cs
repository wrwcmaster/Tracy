using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    [DataContract]
    public class MediaFile : BaseMongoModel
    {
        [DataMember(Name = "file-name")]
        public string FileName { get; set; }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "create-date")]
        public DateTime CreateDate { get; set; }

        [DataMember(Name = "complete-date")]
        public DateTime CompleteDate { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }
    }
}
