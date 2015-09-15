using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IUserProvider<TKey>
    {
        IUserAccount<TKey> GetUserById(TKey id);
        IUserAccount<TKey> GetUserByName(string userName);
        IUserAccount<TKey> CreateUser(IUserCreationInfo newUserInfo);
        IUserAccount<TKey> UpdateUser(TKey id, IUserInfo newUserInfo);
    }
}
