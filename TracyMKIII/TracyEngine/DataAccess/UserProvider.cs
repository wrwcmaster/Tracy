using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;
using UserManagement;
using Gaia.Common.Cryptography;

namespace Tracy.DataAccess
{
    public class UserProvider : AbstractMongoDataProvider<User>, IUserProvider<ObjectId, User>
    {
        public override string CollectionName
        {
            get
            {
                return "user";
            }
        }

        public UserProvider(MongoDB db) : base(db) { }

        public User GetUserById(ObjectId id)
        {
            return Collection.FindOneById(id);
        }

        public User GetUserByName(string userName)
        {
            return Collection.FindOne(Query<User>.EQ(user => user.UserName, userName));
        }

        public User CreateUser(IUserCreationInfo newUserInfo)
        {
            var newUser = new User(newUserInfo.UserName, GeneratePasswordHash(newUserInfo.UserName, newUserInfo.Password));
            newUser.Email = newUserInfo.Email;
            newUser.DisplayName = newUserInfo.DisplayName;
            Collection.Insert(newUser);
            return newUser;
        }

        //TODO: move to parent class
        private string PrepareHashData(string userName, string password)
        {
            return userName + "|salt|" + password;
        }

        private System.Security.Cryptography.HashAlgorithm HashAlgorithm
        {
            get
            {
                return System.Security.Cryptography.SHA1.Create();
            }
        }

        public string GeneratePasswordHash(string userName, string password)
        {
            var input = PrepareHashData(userName, password);
            return HashAlgorithm.GetHashedString(input);
        }

        public bool VerifyPassword(User user, string password)
        {
            string toVerify = PrepareHashData(user.UserName, password);
            return HashAlgorithm.VerifyHashString(toVerify, user.PasswordHash);
        }

        public User UpdateUser(ObjectId id, IUserInfo newUserInfo)
        {
            var user = GetUserById(id);
            user.DisplayName = newUserInfo.DisplayName;
            user.Email = newUserInfo.Email;
            Collection.Save(user);
            return user;
        }
    }
}
