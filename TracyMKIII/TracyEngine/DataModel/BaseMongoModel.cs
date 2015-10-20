using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    [DataContract]
    public abstract class BaseMongoModel
    {
        public ObjectId Id { get; set; }

        [DataMember(Name = "id")]
        public string IdString
        {
            get
            {
                return Id.ToString();
            }
            set
            {
                Id = new ObjectId(value);
            }
        }
    }
}
