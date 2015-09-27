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

        public void AddBrowseHistory(ObjectId userId, ObjectId mediaFileId, DateTime browseDate)
        {
            var existingHistory = GetBrowseHistory(userId, mediaFileId);
            if( existingHistory != null)
            {
                existingHistory.BrowseDate = browseDate;
                Collection.Save(existingHistory);
            }
            else
            {
                var newHistory = new UserBrowseHistory() { UserId = userId, MediaFileId = mediaFileId, BrowseDate = browseDate };
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
    }
}
