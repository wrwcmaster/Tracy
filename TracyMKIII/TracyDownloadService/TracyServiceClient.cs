using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using TracyServerPlugin;

namespace TracyDownloadService
{
    class TracyServiceClient
    {
        private IService _client;
        private IService CreateClient()
        {
            //Create uTorrent Web API client
            CustomBinding uTorrentCustomBinding = new CustomBinding(
                new WebMessageEncodingBindingElement() { ContentTypeMapper = new JsonContentTypeMapper() },
                new HttpTransportBindingElement() { ManualAddressing = true }
                );

            WebChannelFactory<IService> factory = new WebChannelFactory<IService>();
            factory.Endpoint.Address = new EndpointAddress("http://10.0.0.7:8801");
            factory.Endpoint.Binding = uTorrentCustomBinding;
            return factory.CreateChannel();
        }

        public TracyServiceClient()
        {
            _client = CreateClient();
        }

        public void Test()
        {
            var tasks = _client.GetDownloadTasks();
        }
    }
}
