using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IUserProvider<TKey, TUser> where TUser : IUserAccount<TKey>
    {
        TUser GetUserById(TKey id);
        TUser GetUserByName(string userName);
        TUser CreateUser(IUserCreationInfo newUserInfo);
        TUser UpdateUser(TKey id, IUserInfo newUserInfo);
    }
}
