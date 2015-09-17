using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;

namespace Tracy.DataAccess
{
    public abstract class AbstractMongoDataProvider<T> where T : BaseMongoModel
    {
        public abstract string CollectionName { get; }

        private MongoDB _db;
        public AbstractMongoDataProvider(MongoDB database)
        {
            _db = database;
        }

        public MongoCollection<T> Collection
        {
            get
            {
                return _db.GetCollection<T>(CollectionName);
            }
        }
    }
}
