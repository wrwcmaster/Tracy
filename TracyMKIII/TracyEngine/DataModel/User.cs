using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UserManagement;

namespace Tracy.DataModel
{
    [DataContract]
    public class User : BaseMongoModel, IUserAccount<ObjectId>
    {
        [DataMember(Name = "displayName")]
        public string DisplayName
        {
            get; set;
        }
        [DataMember(Name = "email")]
        public string Email
        {
            get; set;
        }

        public string PasswordHash
        {
            get; private set;
        }

        //public List<IRole<ObjectId>> RoleList
        //{
        //    get;
        //}

        //[BsonElement("RoleIds")]
        //private List<ObjectId> _roleIds;
        [DataMember(Name = "userName")]
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
