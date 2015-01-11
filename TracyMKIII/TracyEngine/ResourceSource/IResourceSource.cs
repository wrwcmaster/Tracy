using Gaia.Common.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;

namespace Tracy.ResourceSource
{
    interface IResourceSource
    {
        event EventHandler<GenericEventArgs<List<Resource>>> OnResourcesFound;
        string Name { get; }
        
    }
}
