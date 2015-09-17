using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;
using UserManagement;

namespace Tracy
{
    public class UserManager : IUserManager<ObjectId, User>
    {
        public User GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public bool Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public User Register(IUserCreationInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}
