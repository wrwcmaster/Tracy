﻿using Gaia.Common.Event;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
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

            _downloadManager = new ThunderOfflineDownloadManager(_database, "username", "password");
        }

        public void SyncResource()
        {
            _dmhySource.Sync();
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
            var task = _downloadManager.CreateTask(entry, res);
            res.Status = 1;
            _resourceProvider.Collection.Save(res);
            _downloadManager.MonitorOnGoingTasks();
        }
    }
}