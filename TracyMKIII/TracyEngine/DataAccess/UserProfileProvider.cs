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

        public UserProfile GetUserProfileByUserId(ObjectId userId)
        {
            return Collection.FindOne(Query<UserProfile>.EQ(profile => profile.UserId, userId));
        }

        public void FollowEntry(ObjectId userId, ObjectId entryId, DateTime followDate)
        {
            var profile = TracyFacade.Instance.Manager.UserProfileProvider.GetUserProfileByUserId(userId);
            if (profile == null)
            {
                profile = new UserProfile() { UserId = userId };
            }
            var record = profile.GetEntryFollowRecord(entryId);
            if (record == null)
            {
                record = new EntryFollowRecord() { EntryId = entryId };
                profile.FollowedEntries.Add(record);
            }
            record.FollowDate = followDate;
            record.IsActive = true;
            
            TracyFacade.Instance.Manager.UserProfileProvider.Collection.Save(profile);
        }

        public void UnfollowEntry(ObjectId userId, ObjectId entryId)
        {
            var profile = TracyFacade.Instance.Manager.UserProfileProvider.GetUserProfileByUserId(userId);
            if (profile == null)
            {
                return;
            }
            var record = profile.GetEntryFollowRecord(entryId);
            if (record == null)
            {
                return;
            }
            record.IsActive = false;
            TracyFacade.Instance.Manager.UserProfileProvider.Collection.Save(profile);
        }

        public long GetFollowCount(ObjectId entryId)
        {
            return Collection.Count(
                Query.ElemMatch("FollowedEntries", 
                    Query.And(
                        Query.EQ("EntryId", entryId), 
                        Query.EQ("IsActive", true)
                    )
                )
            );
        }
    }
}
