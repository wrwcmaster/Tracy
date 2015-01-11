using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    public class Resource
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PublishDate { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
    }
}
