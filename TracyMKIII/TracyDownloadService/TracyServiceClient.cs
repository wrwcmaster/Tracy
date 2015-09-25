using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using ThunderAPI;
using Tracy.DataModel;
using Tracy.ResourceSource.Dmhy;
using TracyServerPlugin;

namespace TracyDownloadService
{
    class TracyServiceClient
    {
        private ThunderAgent _thunderAgent;
        private IService _tracyAgent;


        public TracyServiceClient()
        {
            InitThunderAgent();
            InitTracyAgent();
        }

        private void InitTracyAgent()
        {
            //Create uTorrent Web API client
            CustomBinding uTorrentCustomBinding = new CustomBinding(
                new WebMessageEncodingBindingElement() { ContentTypeMapper = new JsonContentTypeMapper() },
                new HttpTransportBindingElement() { ManualAddressing = true }
                );

            WebChannelFactory<IService> factory = new WebChannelFactory<IService>();
            factory.Endpoint.Address = new EndpointAddress("http://10.0.0.7:8801");
            factory.Endpoint.Binding = uTorrentCustomBinding;
            _tracyAgent = factory.CreateChannel();
        }

        private void InitThunderAgent()
        {
            using (var sr = new StreamReader("thunder.ini"))
            {
                string userName = sr.ReadLine();
                string password = sr.ReadLine();
                _thunderAgent = new ThunderAgent();
                _thunderAgent.Login(userName, password);
            }
        }

        public List<ThunderOfflineDownloadTask> GetDownloadTasks()
        {
            return _tracyAgent.GetDownloadTasks().Result;
        }

        public void NotifyTaskStart(string taskId)
        {
            _tracyAgent.NotifyTaskStart(taskId);
        }

        public void StartTask(ThunderOfflineDownloadTask task)
        {
            Resource res = task.Resource;
            Console.WriteLine("Starting task " + res.Title);
            BtTaskCommitResponse commitResponse = null;
            string cid = null;
            if (String.Equals(res.Type, "Torrent", StringComparison.OrdinalIgnoreCase))
            {
                var torrentFileInfo = DmhyTorrentDownloader.Download(res.Link, null);
                var uploadResponse = _thunderAgent.UploadTorrent(torrentFileInfo.PreferredName, new FileStream(torrentFileInfo.TempFilePath, FileMode.Open));
                cid = uploadResponse.Cid;
                System.Threading.Thread.Sleep(1000);
                commitResponse = _thunderAgent.CommitBtTask(uploadResponse.Cid, uploadResponse.GetIndexArray());
            }
            else
            {
                Console.WriteLine("Query Url " + res.Link);
                var urlQueryResponse = _thunderAgent.QueryUrl(res.Link);
                Console.WriteLine(urlQueryResponse.Title);
                cid = urlQueryResponse.Cid;
                System.Threading.Thread.Sleep(1000);
                commitResponse = _thunderAgent.CommitBtTask(urlQueryResponse.Cid, urlQueryResponse.GetIndexArray());
            }

            task.Cid = cid;
            task.TaskId = commitResponse.TaskId;
            task.Status = 1;
            Console.WriteLine("Committed task " + res.Title);
        }
    }
}
