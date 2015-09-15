using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IUserManager<TKey> : IRoleProvider<TKey>, IUserProvider<TKey>
    {
        bool Login(string userName, string password);
        IUserAccount<TKey> GetCurrentUser();
    }
}
