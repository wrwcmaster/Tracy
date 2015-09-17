using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataAccess
{
    public abstract class MongoDB
    {
        public abstract string DBName { get; }
        public abstract string ConnectionString { get; }

        public MongoDB()
        {
            //Pre init codes come here
        }

        private MongoClient _client;
        protected MongoClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new MongoClient(ConnectionString);
                }
                return _client;
            }
        }
        private MongoServer _server;
        protected MongoServer Server
        {
            get
            {
                if (_server == null)
                {
                    _server = Client.GetServer();
                }
                return _server;
            }
        }

        public MongoCollection<T> GetCollection<T>(string collectionName)
        {
            var db = Server.GetDatabase(DBName);
            return db.GetCollection<T>(collectionName);
        }
    }
}
