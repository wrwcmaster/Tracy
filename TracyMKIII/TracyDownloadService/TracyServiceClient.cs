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
using Gaia.Common.Event;
using System.ServiceModel.Description;
using Gaia.Common.Net.Http;
using Gaia.Common.Net.Http.RequestModifier;
using Gaia.Common.Net.Http.ResponseParser;
using System.Runtime.Serialization.Json;

namespace TracyDownloadService
{
    class FileCompleteEventArgs : EventArgs
    {
        public ThunderOfflineDownloadTask Task { get; set; }
        public MediaFile File { get; set; }
    }

    class TracyServiceClient
    {
        private ThunderAgent _thunderAgent;
        private string _tracyUrl;
        //private IService _tracyAgent;

        public event EventHandler<FileCompleteEventArgs> OnFileComplete;
        public event EventHandler<GenericEventArgs<ThunderOfflineDownloadTask>> OnTaskComplete;

        public TracyServiceClient()
        {
            InitThunderAgent();
            InitTracyAgent();
        }

        private void InitTracyAgent()
        {
            using (var sr = new StreamReader("tracy.ini"))
            {
                _tracyUrl = sr.ReadLine();

                /*CustomBinding customBinding = new CustomBinding(
                    new WebMessageEncodingBindingElement() { ContentTypeMapper = new JsonContentTypeMapper() },
                    new HttpTransportBindingElement() { ManualAddressing = true },
                    
                    );
                WebChannelFactory<IService> factory = new WebChannelFactory<IService>(new WebHttpEndpoint(ContractDescription.GetContract(typeof(IService))));
                factory.Endpoint.Address = new EndpointAddress(serviceUrl);
                factory.Endpoint.Binding = customBinding;
                _tracyAgent = factory.CreateChannel();*/
            }
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
            return HttpHelper.SendRequest(new Uri(_tracyUrl + "/GetDownloadTasks"), 
                HttpMethod.GET, 
                new List<IHttpRequestModifier>(),
                new HttpResponseJSONObjectParser<GenericServiceResponse<List<ThunderOfflineDownloadTask>>>(), 
                null).Result;
            //return _tracyAgent.GetDownloadTasks().Result;
        }

        public void NotifyTaskStart(ThunderOfflineDownloadTask task)
        {
            HttpHelper.SendRequest(new Uri(_tracyUrl + "/NotifyTaskStart"),
                HttpMethod.POST,
                new List<IHttpRequestModifier>() { new HttpRequestJSONModifier<ThunderOfflineDownloadTask>(task) },
                null).GetResponse();
            //_tracyAgent.NotifyTaskStart(task);
        }
        public void NotifyTaskComplete(string taskId)
        {
            HttpHelper.SendRequest(new Uri(_tracyUrl + "/NotifyTaskComplete"),
                HttpMethod.POST,
                new List<IHttpRequestModifier>() { new HttpRequestJSONModifier<string>(taskId) },
                null).GetResponse();
            //_tracyAgent.NotifyTaskComplete(taskId);
        }
        public void NotifyFileDownloadComplete(string taskId, MediaFile file)
        {
            NotifyFileDownloadCompleteParameter param = new NotifyFileDownloadCompleteParameter() { TaskId = taskId, MediaFile = file };
            HttpHelper.SendRequest(new Uri(_tracyUrl + "/NotifyFileDownloadComplete"),
                HttpMethod.POST,
                new List<IHttpRequestModifier>() { new HttpRequestJSONModifier<NotifyFileDownloadCompleteParameter>(param) },
                null).GetResponse();
            //_tracyAgent.NotifyFileDownloadComplete(new NotifyFileDownloadCompleteParameter() { TaskId = taskId, MediaFile = file });
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
                for(int i = 1; i <= 5; i++) //TODO: move to common logic
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1000 * i);
                        commitResponse = _thunderAgent.CommitBtTask(urlQueryResponse.Cid, urlQueryResponse.GetIndexArray());
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to Commit BT Task (Attemp " + i + ") - " + ex.Message);
                        if (i == 3)
                        {
                            throw;
                        }
                    }
                }
            }

            task.Cid = cid;
            task.TaskId = commitResponse.TaskId;
            task.Status = 1;
            Console.WriteLine("Committed task " + res.Title);
        }

        public void CheckTask(ThunderOfflineDownloadTask task)
        {
            Resource res = task.Resource;
            Console.WriteLine("Checking progress for task " + res.Title);
            var detail = _thunderAgent.QueryBTDetail(task.Cid, task.TaskId, 1);
            var remoteFileList = detail.FileList;
            
            int completeCount = 0;
            foreach (var remoteFile in remoteFileList)
            {
                Console.WriteLine(string.Format("{0} - {1:F2}%", remoteFile.FileName, remoteFile.Percent));
                bool completed = (remoteFile.Percent == 100);
                if (completed)
                {
                    completeCount++;
                    MediaFile mediaFile = new MediaFile()
                    {
                        FileName = remoteFile.FileName,
                        Size = Convert.ToInt64(remoteFile.Size),
                        CreateDate = DateTime.UtcNow,
                        Type = "Thunder",
                        Status = 1,
                        CompleteDate = DateTime.UtcNow,
                        LastSharedDate = DateTime.MinValue.ToUniversalTime(),
                        PrivateUrl = remoteFile.Url //thunderUrl
                    };
                    OnFileComplete.SafeInvoke(this, new FileCompleteEventArgs() { File = mediaFile, Task = task});
                }
            }

            if (completeCount >= detail.FileCount)
            {
                OnTaskComplete.SafeInvoke(this, new GenericEventArgs<ThunderOfflineDownloadTask>(task));
            }
            
        }
    }
}
