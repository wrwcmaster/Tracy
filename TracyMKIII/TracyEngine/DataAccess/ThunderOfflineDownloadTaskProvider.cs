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
    public class ThunderOfflineDownloadTaskProvider
    {
        private TracyDB _db;
        public ThunderOfflineDownloadTaskProvider(TracyDB database)
        {
            _db = database;
        }

        public MongoCollection<ThunderOfflineDownloadTask> Collection
        {
            get
            {
                return _db.GetCollection<ThunderOfflineDownloadTask>("thunder_offline_download_task");
            }
        }
    }
}
