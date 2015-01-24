﻿using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;

namespace Tracy.DataAccess
{
    public class ResourceProvider
    {
        private TracyDB _db;
        public ResourceProvider(TracyDB database)
        {
            _db = database;
        }

        public MongoCollection<Resource> Collection
        {
            get
            {
                return _db.GetCollection<Resource>("resource");
            }
        }

        public List<Resource> FindResource(string keyword)
        {
            return Collection.Find(Query<Resource>.Matches(r => r.Title, "/" + keyword + "/")).ToList();
        }

        public DateTime GetLatestPublishTime(string source)
        {
            Resource res = Collection.Find(Query<Resource>.EQ(r => r.Source, source)).SetSortOrder(SortBy.Descending("PublishDate")).SetLimit(1).FirstOrDefault();
            return (res != null) ? res.PublishDate : DateTime.MinValue;
        }

        public bool AddResource(Resource res)
        {
            if (res == null) return false;
            if (Collection.Count(Query<Resource>.EQ(r => r.Link, res.Link)) == 0)
            {
                Collection.Insert(res);
                return true;
            }
            return false;
        }
    }
}