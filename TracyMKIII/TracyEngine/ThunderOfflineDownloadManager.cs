using BaiduPanApi.Agent;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderAPI;
using Tracy.DataAccess;
using Tracy.DataModel;
using Tracy.ResourceSource.Dmhy;
using Gaia.Common.Collections;

namespace Tracy
{
    public class ThunderOfflineDownloadManager
    {
        //Only for test
        private BaiduPanAgent _baiduAgent;
        public string Test()
        {
            var result = _agent.QueryUrl("magnet:?xt=urn:btih:W4YJLYYQJ25IUVDATJ42ZO3K6W4UZBDC&dn=&tr=http%3A%2F%2F208.67.16.113%3A8000%2Fannounce&tr=udp%3A%2F%2F208.67.16.113%3A8000%2Fannounce&tr=http%3A%2F%2Ftracker.openbittorrent.com%3A80%2Fannounce&tr=http%3A%2F%2Ftracker.publicbt.com%3A80%2Fannounce&tr=http%3A%2F%2Ftracker.prq.to%2Fannounce&tr=http%3A%2F%2Fopen.acgtracker.com%3A1096%2Fannounce&tr=http%3A%2F%2Ftr.bangumi.moe%3A6969%2Fannounce&tr=https%3A%2F%2Ft-115.rhcloud.com%2Fonly_for_ylbud&tr=http%3A%2F%2Fbtfile.sdo.com%3A6961%2Fannounce&tr=http%3A%2F%2Fexodus.desync.com%3A6969%2Fannounce&tr=https%3A%2F%2Ftr.bangumi.moe%3A9696%2Fannounce&tr=http%3A%2F%2F173.254.204.71%3A1096%2Fannounce&tr=http%3A%2F%2F188.190.120.74%3A80%2Fannounce&tr=http%3A%2F%2F95.68.246.30%3A80%2Fannounce&tr=http%3A%2F%2Fmgtracker.org%3A2710%2Fannounce&tr=http%3A%2F%2Ft2.popgo.org%3A7456%2Fannonce&tr=http%3A%2F%2Fshare.camoe.cn%3A8080%2Fannounce&tr=http%3A%2F%2Fretracker.adminko.org%3A80%2Fannounce&tr=http%3A%2F%2Ftracker.xelion.fr%3A6969%2Fannounce");
            return result.Title;
        }

        private ThunderAgent _agent;
        
        private ThunderOfflineDownloadTaskProvider _provider;
        public ThunderOfflineDownloadManager(DataAccess.MongoDB db, string userName, string password)
        {
            _provider = new ThunderOfflineDownloadTaskProvider(db);
            _agent = new ThunderAgent();
            _agent.Login(userName, password);

            //Only for test
            _baiduAgent = new BaiduPanAgent("nA2MXZmZEwzZWV0LVBvSy1NMkIzR35wclR3dDJ5UEZWRzFRNm1UbGRadFJkUjVXQVFBQUFBJCQAAAAAAAAAAAEAAABIEo1NU2NvdHRUZXN0MDgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFHo9lVR6PZVT");
        }

        public ThunderOfflineDownloadTask CreateTask(Entry entry, Resource res)
        {
            ThunderOfflineDownloadTask task = new ThunderOfflineDownloadTask() { Status = 0, ResourceId = res.Id, EntryId = entry.Id, FailCount = 0 };
            _provider.Collection.Insert(task);
            return task;
        }

        public ThunderOfflineDownloadTask GetTaskById(ObjectId id)
        {
            return _provider.Collection.FindOneById(id);    
        }

        public ThunderOfflineDownloadTask SaveTask(ThunderOfflineDownloadTask task) //Temp
        {
            _provider.Collection.Save(task);
            return task;
        }

        public List<ThunderOfflineDownloadTask> GetOnGoingTasks()
        {
            return _provider.Collection.Find(Query<ThunderOfflineDownloadTask>.LT(t => t.Status, 2)).ToList();
        }

        public void AcceptTask(string taskId)
        {
            var task = _provider.Collection.FindOneById(new ObjectId(taskId));
            if (task != null && task.Status == 0)
            {
                task.Status = 1;
                _provider.Collection.Save(task);
            }
        }

        public void CheckOnGoingTasks()
        {
            var onGoingTasks = _provider.Collection.Find(Query<ThunderOfflineDownloadTask>.LT(t => t.Status, 2)).ToList();
            if (onGoingTasks.Count > 0)
            {
                foreach (var task in onGoingTasks)
                {
                    try
                    {
                        if (task.Status == 0)
                        {
                            StartTask(task);
                            CheckTask(task);
                        }
                        else if (task.Status == 1)
                        {
                            CheckTask(task);
                        }  
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR] " + ex.GetType().Name + ": " + ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        if(ex.InnerException!= null)
                        {
                            Console.WriteLine(ex.InnerException.GetType().Name + ": " + ex.InnerException.Message);
                            Console.WriteLine(ex.InnerException.StackTrace);
                        }
                        task.FailCount = task.FailCount + 1;
                        _provider.Collection.Save(task);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public void StartTask(ThunderOfflineDownloadTask task)
        {
            Resource res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(task.ResourceId);
            Console.WriteLine("Starting task " + res.Title);
            BtTaskCommitResponse commitResponse = null;
            string cid = null;
            if (String.Equals(res.Type, "Torrent", StringComparison.OrdinalIgnoreCase))
            {
                var torrentFileInfo = DmhyTorrentDownloader.Download(res.Link, null);
                var uploadResponse = _agent.UploadTorrent(torrentFileInfo.PreferredName, new FileStream(torrentFileInfo.TempFilePath, FileMode.Open));
                cid = uploadResponse.Cid;
                System.Threading.Thread.Sleep(1000);
                commitResponse = _agent.CommitBtTask(uploadResponse.Cid, uploadResponse.GetIndexArray());
            }
            else
            {
                Console.WriteLine("Query Url " + res.Link);
                var urlQueryResponse = _agent.QueryUrl(res.Link);
                Console.WriteLine(urlQueryResponse.Title);
                cid = urlQueryResponse.Cid;
                System.Threading.Thread.Sleep(1000);
                commitResponse = _agent.CommitBtTask(urlQueryResponse.Cid, urlQueryResponse.GetIndexArray());
            }

            task.Cid = cid;
            task.TaskId = commitResponse.TaskId;
            task.Status = 1;
            _provider.Collection.Save(task);
            Console.WriteLine("Committed task " + res.Title);
        }

        private void CheckTask(ThunderOfflineDownloadTask task)
        {
            Entry entry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOneById(task.EntryId);
            Resource res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(task.ResourceId);
            Console.WriteLine("Checking progress for task " + res.Title);
            var detail = _agent.QueryBTDetail(task.Cid, task.TaskId, 1);
            var remoteFileList = detail.FileList;
            var fileList = new List<MediaFile>();
            foreach (var fileId in task.FileIds.GetSafeEnumerable())
            {
                var file = TracyFacade.Instance.Manager.MediaFileProvider.Collection.FindOneById(fileId);
                fileList.Add(file);
            }
            int completeCount = 0;
            foreach (var remoteFile in remoteFileList)
            {
                bool completed = (remoteFile.Percent == 100);
                if (completed)
                {
                    completeCount++;
                }
                bool exists = false;
                foreach (var localFile in fileList)
                {
                    if (String.Equals(remoteFile.FileName, localFile.FileName))
                    {
                        if (localFile.Status == 0 && completed)
                        {
                            MarkMediaFileAsCompleted(localFile, remoteFile.DownloadUrl);
                            TracyFacade.Instance.Manager.MediaFileProvider.Collection.Save(localFile);
                        }
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    MediaFile mediaFile = new MediaFile() {
                        FileName = remoteFile.FileName,
                        Size = Convert.ToInt64(remoteFile.Size),
                        CreateDate = DateTime.UtcNow,
                        LastSharedDate= DateTime.MinValue,
                        Type = "Thunder",
                        Status = 0
                    };

                    if (completed)
                    {
                        MarkMediaFileAsCompleted(mediaFile, remoteFile.DownloadUrl);
                    }

                    TracyFacade.Instance.Manager.MediaFileProvider.Collection.Insert(mediaFile);
                    //Link to entry
                    entry.MediaFileIds.Add(mediaFile.Id);
                    TracyFacade.Instance.Manager.EntryProvider.Collection.Save(entry);
                }
            }

            if (completeCount == remoteFileList.Count)
            {
                Console.WriteLine("Download completed for task " + res.Title);
                task.Status = 2;
                _provider.Collection.Save(task);
            }
        }

        private void MarkMediaFileAsCompleted(MediaFile file, string privateUrl)
        {
            file.CompleteDate = DateTime.UtcNow;
            file.PrivateUrl = privateUrl; //thunderUrl
            file.Status = 1;
        }

        public string GetSharedUrl(string thunderUrl, string fileName)
        {
            var analysisResult = _agent.KuaiAnalyzeUrl(thunderUrl);
            var forwardResponse = _agent.KuaiForwardOfflineDownloadTask(analysisResult.Result.Cid, analysisResult.Result.FileSize, analysisResult.Result.Gcid, fileName, analysisResult.Result.Url, analysisResult.Result.Section);
            var shortUrlResponse = _agent.KuaiGetShortUrl(forwardResponse.ForwardTaskId);
            //var url = _agent.KuaiGetActualUrl(shortUrlResponse.Url); //This url needs cookie, it is not public to all
            return shortUrlResponse.Url;
        }

        //Only for test, move to stand alone plugin in the future
        public string TransportToBaiduPan(string privateUrl, string fileName)
        {
            var tmpFileName = Path.GetTempFileName();
            try
            {
                long fileSize = 0;
                using(var outputFileStream = new FileStream(tmpFileName, FileMode.Create))
                {
                    _agent.PrivateDownload(privateUrl, outputFileStream);
                    fileSize = outputFileStream.Length;
                }
                using(var inputFileStream = new FileStream(tmpFileName, FileMode.Open))
                {
                    var pcsResponse = _baiduAgent.UploadTempFile(fileName, inputFileStream);
                    var createResponse = _baiduAgent.CreateCloudFile("/" + fileName, fileSize, pcsResponse.MD5);
                    return createResponse.Path;
                }
            }
            finally
            {
                File.Delete(tmpFileName);
            }
            

            
        }
    }
}
