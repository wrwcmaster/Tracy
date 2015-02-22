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
    public class Resource : BaseMongoModel
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "publish-date")]
        public DateTime PublishDate { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }
    }
}
