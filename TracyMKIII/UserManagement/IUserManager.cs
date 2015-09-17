using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IUserManager<TKey, TUser> where TUser : IUserAccount<TKey>
    {
        bool Login(string userName, string password);
        TUser Register(IUserCreationInfo userInfo);
        TUser GetCurrentUser();
    }
}
