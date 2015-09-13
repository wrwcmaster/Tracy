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
    public class BaiduOfflineDownloadTaskProvider
    {
        private TracyDB _db;
        public BaiduOfflineDownloadTaskProvider(TracyDB database)
        {
            _db = database;
        }

        public MongoCollection<BaiduOfflineDownloadTask> Collection
        {
            get
            {
                return _db.GetCollection<BaiduOfflineDownloadTask>("baiduOfflineDownloadTask");
            }
        }
    }
}
