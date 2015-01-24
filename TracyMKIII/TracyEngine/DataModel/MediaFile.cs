using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    public class MediaFile
    {
        public ObjectId Id { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public int Status { get; set; }
    }
}
