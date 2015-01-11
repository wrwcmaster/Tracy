using BaiduPanApi.Agent;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataAccess;
using Tracy.DataModel;
using Tracy.ResourceSource.Dmhy;

namespace Tracy
{
    public class BaiduOfflineDownloadManager
    {
        private const string BDUSS = "DM4aTBzTTlrS29uUkFMNkY5YmpTMEJlVlVSZn5TY2dWY0t0cm5KbEpJRWE1dFJVQVFBQUFBJCQAAAAAAAAAAAEAAABIEo1NU2NvdHRUZXN0MDgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpZrVQaWa1UR";
        private BaiduPanAgent _agent;
        private BaiduOfflineDownloadTaskProvider _provider;
        public BaiduOfflineDownloadManager(TracyDB db)
        {
            _provider = new BaiduOfflineDownloadTaskProvider(db);
            _agent = new BaiduPanAgent(BDUSS);
            _agent.RefreshToken();
        }

        public BaiduOfflineDownloadTask CreateTask(Entry entry, Resource res)
        {
            var torrentFileInfo = DmhyTorrentDownloader.Download(res.Link, null);
            //todo: rename file
            BaiduOfflineDownloadTask task = new BaiduOfflineDownloadTask() { Status = 0, ResourceId = res.Id, LocalTorrentPath = torrentFileInfo.TempFilePath };
            _provider.Collection.Insert(task);
            return task;
        }

        public void StartTask(BaiduOfflineDownloadTask task)
        {
            Resource res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(task.ResourceId);
            if (task.Status == 0)
            {
                FileStream fs = new FileStream(task.LocalTorrentPath, FileMode.Open);
                long size = fs.Length;
                var pcsResponse = _agent.UploadTempFile(fs);
                task.TorrentMD5 = pcsResponse.MD5;
                var createResponse = _agent.CreateCloudFile("/" + res.Title + ".torrent", size, task.TorrentMD5);
                if (createResponse.ErrorNumber == 0)
                {
                    task.RemoteTorrentPath = createResponse.Path;
                    task.Status = 1;
                    _provider.Collection.Save(task);
                }
            }

            if (task.Status == 1)
            {
                Entry entry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOneById(task.EntryId);
                var torrentQueryResponse = _agent.QueryTorrentInfo(task.RemoteTorrentPath);
                task.TorrentSHA1 = torrentQueryResponse.Info.SHA1;
                var fileList = torrentQueryResponse.Info.FileList;
                int[] filesToDownload = new int[fileList.Count];
                for (int i = 0; i < fileList.Count; i++)
                {
                    filesToDownload[i] = i + 1;
                }
                var offlineDownloadResponse = _agent.OfflineDownload(task.RemoteTorrentPath, task.TorrentSHA1, "/", filesToDownload);
                
                for (int i = 0; i < fileList.Count; i++)
                {
                    var file = fileList[i];
                    MediaFile mediaFile = new MediaFile() { FileName = file.FileName, Size = Convert.ToInt64(file.Size), CreateDate = DateTime.Now, Type = "BaiduPan", Status = 0 };
                    TracyFacade.Instance.Manager.MediaFileProvider.Collection.Insert(mediaFile);
                    if (entry.MediaFileIds == null) entry.MediaFileIds = new List<ObjectId>();
                    entry.MediaFileIds.Add(mediaFile.Id);
                    if (task.FileIds == null) task.FileIds = new List<ObjectId>();
                    task.FileIds.Add(mediaFile.Id);
                }
                TracyFacade.Instance.Manager.EntryProvider.Collection.Save(entry);

                task.Status = 2;
                if (offlineDownloadResponse.RapidDownload == 1)
                {
                    MonitorOnGoingTasks();
                }
                _provider.Collection.Save(task);
            }

        }

        public void MonitorOnGoingTasks()
        {
            var onGoingTasks = _provider.Collection.Find(Query<BaiduOfflineDownloadTask>.EQ(t => t.Status, 2)).ToList();
            if (onGoingTasks.Count > 0)
            {
                foreach (var task in onGoingTasks)
                {
                    foreach (var fileId in task.FileIds)
                    {
                        var file = TracyFacade.Instance.Manager.MediaFileProvider.Collection.FindOneById(fileId);
                        var searchResponse = _agent.Search(file.FileName, 1, onGoingTasks.Count);
                        if (searchResponse.ResultList != null && searchResponse.ResultList.Count > 0)
                        {
                            var result = searchResponse.ResultList[0];
                            if (String.Equals(result.ServerFileName, file.FileName, StringComparison.OrdinalIgnoreCase))
                            {
                                long serverFileId = result.FileId;
                                var shareResponse = _agent.Share(serverFileId);
                                file.Url = shareResponse.Link;
                                file.Status = 1;
                                TracyFacade.Instance.Manager.MediaFileProvider.Collection.Save(file);
                            }
                        }
                    }
                }
            }
        }
    }
}
