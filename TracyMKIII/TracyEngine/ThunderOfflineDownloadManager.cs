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

namespace Tracy
{
    public class ThunderOfflineDownloadManager
    {
        private ThunderAgent _agent;
        private ThunderOfflineDownloadTaskProvider _provider;
        public ThunderOfflineDownloadManager(TracyDB db, string userName, string password)
        {
            _provider = new ThunderOfflineDownloadTaskProvider(db);
            _agent = new ThunderAgent();
            _agent.Login(userName, password);
        }

        public ThunderOfflineDownloadTask CreateTask(Entry entry, Resource res)
        {
            BtTaskCommitResponse commitResponse = null;
            string cid = null;
            if (String.Equals(res.Type, "Torrent", StringComparison.OrdinalIgnoreCase))
            {
                var torrentFileInfo = DmhyTorrentDownloader.Download(res.Link, null);
                var uploadResponse = _agent.UploadTorrent(torrentFileInfo.PreferredName, new FileStream(torrentFileInfo.TempFilePath, FileMode.Open));
                cid = uploadResponse.Cid;
                commitResponse = _agent.CommitBtTask(uploadResponse.Cid, uploadResponse.GetIndexArray());
            }
            else
            {
                var urlQueryResponse = _agent.QueryUrl(res.Link);
                cid = urlQueryResponse.Cid;
                commitResponse = _agent.CommitBtTask(urlQueryResponse.Cid, urlQueryResponse.GetIndexArray());
            }

            ThunderOfflineDownloadTask task = new ThunderOfflineDownloadTask() { Status = 0, ResourceId = res.Id, EntryId = entry.Id, Cid = cid, TaskId = commitResponse.TaskId };
            _provider.Collection.Insert(task);
            return task;
        }

        public void MonitorOnGoingTasks()
        {
            var onGoingTasks = _provider.Collection.Find(Query<ThunderOfflineDownloadTask>.EQ(t => t.Status, 0)).ToList();
            if (onGoingTasks.Count > 0)
            {
                foreach (var task in onGoingTasks)
                {
                    CheckTask(task);
                }
            }
        }

        private void CheckTask(ThunderOfflineDownloadTask task)
        {
            var detail = _agent.QueryBTDetail(task.Cid, task.TaskId, 1);
            var remoteFileList = detail.FileList;
            var fileList = new List<MediaFile>();
            foreach (var fileId in task.FileIds)
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
                            localFile.CompleteDate = DateTime.UtcNow;
                            localFile.Status = 1;
                            TracyFacade.Instance.Manager.MediaFileProvider.Collection.Save(localFile);
                        }
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    MediaFile mediaFile = new MediaFile() { FileName = remoteFile.FileName, Size = Convert.ToInt64(remoteFile.Size), CreateDate = DateTime.UtcNow, Type = "Thunder", Status = completed ? 1 : 0 };
                    TracyFacade.Instance.Manager.MediaFileProvider.Collection.Insert(mediaFile);
                }
            }

            if (completeCount == remoteFileList.Count)
            {
                task.Status = 1;
                _provider.Collection.Save(task);
            }
        }
    }
}
