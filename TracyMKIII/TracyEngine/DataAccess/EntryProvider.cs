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
    public class EntryProvider
    {
        private TracyDB _db;
        public EntryProvider(TracyDB database)
        {
            _db = database;
        }

        public MongoCollection<Entry> Collection
        {
            get
            {
                return _db.GetCollection<Entry>("entry");
            }
        }


    }
}
