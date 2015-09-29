using MongoDB.Bson;
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
    public class UserProfileProvider : AbstractMongoDataProvider<UserProfile>
    {
        public override string CollectionName
        {
            get
            {
                return "userProfile";
            }
        }

        public UserProfileProvider(MongoDB db) : base(db) { }
    }
}
