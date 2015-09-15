using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IUserAccount<TKey> : IUserInfo
    { 
        [DataMember(Name = "id")]
        TKey Id { get; set; }
   
        string PasswordHash { get; set; }

        List<IRole<TKey>> RoleList { get; set; }
    }
}
