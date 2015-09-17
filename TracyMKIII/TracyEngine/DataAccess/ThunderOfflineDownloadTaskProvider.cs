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
    public class ThunderOfflineDownloadTaskProvider : AbstractMongoDataProvider<ThunderOfflineDownloadTask>
    {
        public override string CollectionName
        {
            get
            {
                return "thunderOfflineDownloadTask";
            }
        }

        public ThunderOfflineDownloadTaskProvider(MongoDB db) : base(db) { }
    }
}
