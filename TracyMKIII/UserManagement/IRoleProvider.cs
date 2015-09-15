using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IRoleProvider<TKey>
    {
        List<IRole<TKey>> GetRoles();
        IRole<TKey> GetRoleById(TKey id);
    }
}
