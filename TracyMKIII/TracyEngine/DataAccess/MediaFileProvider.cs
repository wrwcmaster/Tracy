using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;

namespace Tracy.DataAccess
{
    public class MediaFileProvider
    {
        private TracyDB _db;
        public MediaFileProvider(TracyDB database)
        {
            _db = database;
        }

        public MongoCollection<MediaFile> Collection
        {
            get
            {
                return _db.GetCollection<MediaFile>("media_file");
            }
        }
    }
}
