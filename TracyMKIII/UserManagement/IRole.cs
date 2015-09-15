using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public interface IRole<TKey>
    {
        [DataMember(Name = "id")]
        TKey Id { get; set; }

        [DataMember(Name = "name")]
        string Name { get; set; }
    }
}
