using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IUserCreationInfo : IUserInfo
    {
        string Password { get; }
    }
}
