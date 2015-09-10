using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataAccess
{
    public class TracyDB
    {
        public TracyDB()
        {
            //Pre init codes come here
            _client = new MongoClient(_connectionString);
            _server = _client.GetServer();
        }

        private string _dbName = "tracy";
        private string _connectionString = "mongodb://localhost:27017";
        private MongoClient _client;
        private MongoServer _server;

        public MongoCollection<T> GetCollection<T>(string collectionName)
        {
            var db = _server.GetDatabase(_dbName);
            return db.GetCollection<T>(collectionName);
        }
    }
}
