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
    public class MediaFileProvider : AbstractMongoDataProvider<MediaFile>
    {
        public override string CollectionName
        {
            get
            {
                return "mediaFile";
            }
        }

        public MediaFileProvider(MongoDB db) : base(db) { }

    }
}
