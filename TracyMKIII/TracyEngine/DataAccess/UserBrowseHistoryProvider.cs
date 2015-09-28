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
    public class UserBrowseHistoryProvider : AbstractMongoDataProvider<UserBrowseHistory>
    {
        public override string CollectionName
        {
            get
            {
                return "userBrowseHistory";
            }
        }

        public UserBrowseHistoryProvider(MongoDB db) : base(db) { }

        public void LogBrowseHistory(ObjectId userId, ObjectId mediaFileId, DateTime browseDate)
        {
            var existingHistory = GetBrowseHistory(userId, mediaFileId);
            if( existingHistory != null)
            {
                existingHistory.LastBrowseDate = browseDate;
                existingHistory.BrowseCount++;
                Collection.Save(existingHistory);
            }
            else
            {
                var newHistory = new UserBrowseHistory() { UserId = userId, MediaFileId = mediaFileId, LastBrowseDate = browseDate, BrowseCount = 1 };
                Collection.Insert(newHistory);
            }
        }

        public UserBrowseHistory GetBrowseHistory(ObjectId userId, ObjectId mediaFileId)
        {
            var queryBuilder = new QueryBuilder<UserBrowseHistory>();
            var query = queryBuilder.And(Query<UserBrowseHistory>.EQ(history => history.UserId, userId), Query<UserBrowseHistory>.EQ(history => history.MediaFileId, mediaFileId));
            var existingHistory = Collection.FindOne(query);
            return existingHistory;
        }

        public int GetBrowseCount(ObjectId mediaFileId)
        {
            var pipeLine = new[]
            {
                new BsonDocument("$match", new BsonDocument("MediaFileId", mediaFileId)),
                new BsonDocument("$group", new BsonDocument() {
                    { "_id", 1 },
                    {
                        "Total", new BsonDocument()
                        {
                            { "$sum", "$BrowseCount" }
                        }
                    }
                })
            };
            
            var result = Collection.Aggregate(new AggregateArgs() { Pipeline = pipeLine }).ToList();
            if (result.Count == 0) return 0;
            return result[0]["Total"].AsInt32;
        }
    }
}
