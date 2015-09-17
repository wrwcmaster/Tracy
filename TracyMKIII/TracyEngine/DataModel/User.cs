using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement;

namespace Tracy.DataModel
{
    public class User : BaseMongoModel, IUserAccount<ObjectId>
    {
        public string DisplayName
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string PasswordHash
        {
            get; private set;
        }

        public List<IRole<ObjectId>> RoleList
        {
            get;
        }

        [BsonElement("RoleIds")]
        private List<ObjectId> _roleIds;

        public string UserName
        {
            get; private set;
        }

        public User(string userName, string passwordHash)
        {
            UserName = userName;
            PasswordHash = passwordHash;
        }
    }
}
