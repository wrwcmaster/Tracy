using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataAccess;
using Tracy.DataModel;
using UserManagement;

namespace Tracy
{
    public class UserManager : IUserManager<ObjectId, User>
    {
        private UserProvider _provider;
        public UserManager(DataAccess.MongoDB database)
        {
            _provider = new UserProvider(database);
        }

        public User GetCurrentUser()
        {
            if (!SessionManager.CurrentSession.ContainsKey("userId"))
            {
                return null;
            }
            var userId = SessionManager.CurrentSession["userId"];
            return _provider.GetUserById(new ObjectId(userId));
        }

        public bool Login(string userName, string password)
        {
            var user = _provider.GetUserByName(userName);
            var result = (user != null && _provider.VerifyPassword(user, password));
            if (result)
            {
                SessionManager.BuildSession();
                SessionManager.CurrentSession["userId"] = user.Id.ToString();
                SessionManager.CurrentSession["userName"] = user.UserName;
            }
            return result;
        }

        public User Register(IUserCreationInfo userInfo)
        {
            if(_provider.GetUserByName(userInfo.UserName) != null)
            {
                throw new ArgumentException("User '" + userInfo.UserName + "' already exists.");
            }
            return _provider.CreateUser(userInfo);
        }
    }
}
