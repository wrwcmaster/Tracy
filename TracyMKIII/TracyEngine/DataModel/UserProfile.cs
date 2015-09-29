using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    public class UserProfile : BaseMongoModel
    {
        public ObjectId UserProfileId { get; set; }
        public ObjectId UserId { get; set; }

        [BsonElement("FollowedEntries")]
        private List<EntryFollowRecord> _followedEntries;

        [BsonIgnore]
        public List<EntryFollowRecord> FollowedEntries
        {
            get
            {
                if (_followedEntries == null)
                {
                    _followedEntries = new List<EntryFollowRecord>();
                }
                return _followedEntries;
            }
        }

        public bool IsEntryFollowed(ObjectId entryId)
        {
            foreach (var record in FollowedEntries)
            {
                if (record.EntryId == entryId)
                {
                    return true;
                }
            }
            return false;
        }

        public EntryFollowRecord GetEntryFollowRecord(ObjectId entryId)
        {
            foreach (var record in FollowedEntries)
            {
                if (record.EntryId == entryId)
                {
                    return record;
                }
            }
            return null;
        }
    }

    public class EntryFollowRecord
    {
        public ObjectId EntryId { get; set; }
        public DateTime FollowDate { get; set; }
        public bool IsActive { get; set; }
    }

    
}
