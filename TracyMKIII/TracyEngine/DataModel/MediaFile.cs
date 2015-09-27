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
        [DataMember(Name = "fileName")]
        public string FileName { get; set; }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        //TODO: move to resource
        [DataMember(Name = "privateUrl")]
        public string PrivateUrl { get; set; }

        [DataMember(Name = "sharedUrl")]
        public string SharedUrl { get; set; }

        [DataMember(Name = "lastSharedDate")]
        public DateTime LastSharedDate { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "createDate")]
        public DateTime CreateDate { get; set; }

        [DataMember(Name = "completeDate")]
        public DateTime CompleteDate { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        public ObjectId ResourceId
        {
            get; set;
        }

        [DataMember(Name = "resourceId")]
        public string ResourceIdString
        {
            get
            {
                return ResourceId != null ? ResourceId.ToString() : null;
            }
            set
            {
                ResourceId = new ObjectId(value);
            }
        }
    }
}
