using Gaia.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace TracyServerPlugin
{
    public class PluginInterface : IPlugin
    {
        public void Initiate()
        {
            WebServiceHost host = new WebServiceHost(typeof(Service), new Uri("http://localhost:8801/"));
            host.Open();
        }
    }
}
