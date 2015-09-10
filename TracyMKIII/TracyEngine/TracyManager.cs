using Gaia.Common.Event;
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
    public class TracyManager
    {
        private TracyDB _database;
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
        private ThunderOfflineDownloadManager _downloadManager;
        private DmhyResourceSource _dmhySource;
        public TracyManager()
        {
            _database = new TracyDB();
            _entryProvider = new EntryProvider(_database);
            _resourceProvider = new ResourceProvider(_database);
            _mediaFileProvider = new MediaFileProvider(_database);

            _dmhySource = new DmhyResourceSource(_resourceProvider);
            _dmhySource.OnResourcesFound += DmhySource_OnResourcesFound;

            InitDownloadManager();
        }

        private void InitDownloadManager()
        {
            using (var sr = new StreamReader("thunder.ini"))
            {
                string userName = sr.ReadLine();
                string password = sr.ReadLine();
                _downloadManager = new ThunderOfflineDownloadManager(_database, userName, password);
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
            var entries = _entryProvider.Collection.Find(Query<Entry>.EQ(e => e.TracingEnabled, true)).ToList();
            foreach (Resource res in list)
            {
                Entry matchedEntry = null;
                foreach (Entry entry in entries)
                {
                    if (entry.IsTitleMatched(res.Title))
                    {
                        Console.WriteLine("Resource " + res.Title + " matched entry " + entry.Name);
                        matchedEntry = entry;
                        if (entry.ResourceIds == null) entry.ResourceIds = new List<ObjectId>();
                        if (!entry.ResourceIds.Contains(res.Id))
                        {
                            entry.ResourceIds.Add(res.Id);
                            _entryProvider.Collection.Save(entry);
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

        public void DownloadResource(Entry entry, Resource res)
        {
            Console.WriteLine("Create download task for resource " + res.Title);
            var task = _downloadManager.CreateTask(entry, res);
            res.Status = 1;
            _resourceProvider.Collection.Save(res);
            //_downloadManager.CheckOnGoingTasks();
        }

        public void CheckTasks()
        {
            _downloadManager.CheckOnGoingTasks();
        }

        public void Test()
        {
            var entry = _entryProvider.Collection.FindOne();
            List<Resource> resources = _resourceProvider.FindResource(entry.SearchKeywords);
            foreach (var res in resources)
            {
                if (!entry.ResourceIds.Contains(res.Id))
                {
                    entry.ResourceIds.Add(res.Id);
                }
            }
        }
    }
}
