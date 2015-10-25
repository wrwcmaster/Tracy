using Gaia.Common.Event;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tracy.DataAccess;
using Tracy.DataModel;
using Tracy.ResourceSource.Dmhy;

namespace Tracy
{
    public class TracyManager
    {
        //private DataAccess.MongoDB _database;
        private EntryProvider _entryProvider;
        public EntryProvider EntryProvider
        {
            get { return _entryProvider; }
        }
        private ResourceProvider _resourceProvider;
        public ResourceProvider ResourceProvider
        {
            get { return _resourceProvider; }
        }

        private MediaFileProvider _mediaFileProvider;
        public MediaFileProvider MediaFileProvider
        {
            get { return _mediaFileProvider; }
        }

        private UserBrowseHistoryProvider _userBrowseHistoryProvider;
        public UserBrowseHistoryProvider UserBrowseHistoryProvider
        {
            get { return _userBrowseHistoryProvider; }
        }

        private UserProfileProvider _userProfileProvider;
        public UserProfileProvider UserProfileProvider
        {
            get { return _userProfileProvider; }
        }

        private ThunderOfflineDownloadManager _downloadManager;
        public ThunderOfflineDownloadManager DownloadManager
        {
            get { return _downloadManager; }
        }

        private DmhyResourceSource _dmhySource;
        public TracyManager(DataAccess.MongoDB database)
        {
            _entryProvider = new EntryProvider(database);
            _resourceProvider = new ResourceProvider(database);
            _mediaFileProvider = new MediaFileProvider(database);
            _userBrowseHistoryProvider = new UserBrowseHistoryProvider(database);
            _userProfileProvider = new UserProfileProvider(database);

            _dmhySource = new DmhyResourceSource(_resourceProvider);
            _dmhySource.OnResourcesFound += DmhySource_OnResourcesFound;

            InitDownloadManager(database);
        }

        private void InitDownloadManager(DataAccess.MongoDB database)
        {
            using (var sr = new StreamReader("thunder.ini"))
            {
                string userName = sr.ReadLine();
                string password = sr.ReadLine();
                _downloadManager = new ThunderOfflineDownloadManager(database, userName, password);
            }
            
        }

        public void AddEntry(Entry newEntry)
        {
            BindResources(newEntry, false);
            EntryProvider.Collection.Insert(newEntry);
        }

        private void BindResources(Entry entry, bool willCheckDuplication)
        {
            var resourceList = TracyFacade.Instance.Manager.ResourceProvider.FindResource(entry.SearchKeywords);
            var result = TracyFacade.Instance.Manager.ResourceProvider.FilterResource(resourceList, entry.RegExpr);
            foreach (var res in result)
            {
                if (willCheckDuplication || !entry.ResourceIds.Contains(res.Id))
                {
                    entry.ResourceIds.Add(res.Id);
                }
            }
        }

        public void SyncResource()
        {
            _dmhySource.Sync(1, true);
        }

        public void SyncResource(int startPage)
        {
            _dmhySource.Sync(startPage, false);
        }

        private void DmhySource_OnResourcesFound(object sender, GenericEventArgs<List<Resource>> e)
        {
            ProcessTracingResources(e.Item);
        }

        private void ProcessTracingResources(List<Resource> list)
        {
            var entries = _entryProvider.Collection.FindAll().ToList();
            foreach (Resource res in list)
            {
                Entry matchedEntry = null;
                foreach (Entry entry in entries)
                {
                    if (entry.IsTitleMatched(res.Title))
                    {
                        Console.WriteLine("Resource " + res.Title + " matched entry " + entry.Name);
                        
                        if (!entry.ResourceIds.Contains(res.Id))
                        {
                            entry.ResourceIds.Add(res.Id);
                            _entryProvider.Collection.Save(entry);

                            if (entry.TracingEnabled)
                            {
                                matchedEntry = entry;
                            }

                            break; //TODO: multiple match
                        }
                    }
                }

                if (res.Status == 0 && matchedEntry != null)
                {
                    DownloadResource(matchedEntry, res);
                }
            }
        }

        public ThunderOfflineDownloadTask DownloadResource(Entry entry, Resource res)
        {
            Console.WriteLine("Create download task for resource " + res.Title);
            var task = _downloadManager.CreateTask(entry, res);
            res.Status = 1;
            _resourceProvider.Collection.Save(res);
            
            //Move download opperation to client side
            //_downloadManager.CheckOnGoingTasks(); 
            //task = _downloadManager.GetTaskById(task.Id);
            return task;
        }

        public void CheckDownloadTasks()
        {
            _downloadManager.CheckOnGoingTasks();
        }

        public string GetSharedUrl(MediaFile file)
        {
            if(String.IsNullOrEmpty(file.SharedUrl) || (DateTime.UtcNow - file.LastSharedDate).Days > 10)
            {
                file.SharedUrl = _downloadManager.GetSharedUrl(file.PrivateUrl, file.FileName);
                file.LastSharedDate = DateTime.UtcNow;
                MediaFileProvider.Collection.Save(file);
            }
            return file.SharedUrl;
        }
    }
}
