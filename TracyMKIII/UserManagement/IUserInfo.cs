using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace UserManagement
{
    public interface IUserInfo
    {
        string UserName { get; }
        
        string DisplayName { get; }

        string Email { get; }
    }
}
