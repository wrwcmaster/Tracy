using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    public class UserBrowseHistory : BaseMongoModel
    {
        public ObjectId UserId { get; set; }
        public ObjectId MediaFileId { get; set; }
        public DateTime BrowseDate { get; set; }
    }
}
