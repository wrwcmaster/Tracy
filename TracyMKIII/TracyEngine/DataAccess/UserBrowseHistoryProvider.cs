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

        public void LogBrowseHistory(ObjectId userId, MediaFile mediaFile, DateTime browseDate)
        {
            var existingHistory = GetBrowseHistory(userId, mediaFile.Id);
            if( existingHistory != null)
            {
                existingHistory.LastBrowseDate = browseDate;
                existingHistory.BrowseCount++;
                Collection.Save(existingHistory);
            }
            else
            {
                var newHistory = new UserBrowseHistory() { UserId = userId, MediaFileId = mediaFile.Id, LastBrowseDate = browseDate, BrowseCount = 1 };
                Collection.Insert(newHistory);
            }

            if(mediaFile.Episode != null)
            {
                //TODO: move logic to manager level
                //Update user profile for max browsed episode of the entry
                var relatedEntry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOne(Query.EQ("MediaFileIds", mediaFile.Id));
                if (relatedEntry != null)
                {
                    var profile = TracyFacade.Instance.Manager.UserProfileProvider.GetUserProfileByUserId(userId);
                    if (profile != null)
                    {
                        var entryFollowRecord = profile.GetEntryFollowRecord(relatedEntry.Id);
                        if (entryFollowRecord != null)
                        {
                            if (entryFollowRecord.MaxBrowsedEpisode == null || mediaFile.Episode > entryFollowRecord.MaxBrowsedEpisode)
                            {
                                entryFollowRecord.MaxBrowsedEpisode = mediaFile.Episode;
                                TracyFacade.Instance.Manager.UserProfileProvider.Collection.Save(profile);
                            }
                        }
                    }
                }
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
