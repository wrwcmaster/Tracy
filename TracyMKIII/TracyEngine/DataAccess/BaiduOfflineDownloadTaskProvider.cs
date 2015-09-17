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
    public class BaiduOfflineDownloadTaskProvider : AbstractMongoDataProvider<BaiduOfflineDownloadTask>
    {
        public override string CollectionName
        {
            get
            {
                return "baiduOfflineDownloadTask";
            }
        }

        public BaiduOfflineDownloadTaskProvider(MongoDB db) : base(db) { }
    }
}
