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
        [DataMember(Name = "userName")]
        string UserName { get; }

        [DataMember(Name = "displayName")]
        string DisplayName { get; }

        [DataMember(Name = "email")]
        string Email { get; }
    }
}
