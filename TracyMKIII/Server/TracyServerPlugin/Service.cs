using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Tracy;
using Tracy.DataModel;

namespace TracyServerPlugin
{
    [ServiceContract]
    public class Service
    {
        [OperationContract]
        [WebGet(ResponseFormat =WebMessageFormat.Json)]
        public ServiceResponse Sync(string startPage)
        {
            if (String.IsNullOrEmpty(startPage))
            {
                TracyFacade.Instance.Manager.SyncResource();
            }
            else
            {
                TracyFacade.Instance.Manager.SyncResource(int.Parse(startPage));
            }
            return new ServiceResponse();
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public ServiceResponse CheckDownloadTasks()
        {
            TracyFacade.Instance.Manager.CheckDownloadTasks();
            return new ServiceResponse();
        }

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<Entry> AddEntry(Entry newEntry)
        {
            TracyFacade.Instance.Manager.AddEntry(newEntry);
            return new GenericServiceResponse<Entry>(newEntry);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<List<Entry>> GetEntryList()
        {
            var rtn = TracyFacade.Instance.Manager.EntryProvider.Collection.FindAll().ToList();
            return new GenericServiceResponse<List<Entry>>(rtn);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<List<Resource>> GetResourceList(string entryId)
        {
            List<Resource> rtn = new List<Resource>();
            Entry entry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOneById(new ObjectId(entryId));
            foreach (ObjectId resId in entry.ResourceIds)
            {
                Resource res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(resId);
                if(res != null) rtn.Add(res);
            }
            return new GenericServiceResponse<List<Resource>>(rtn);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<List<Resource>> SearchResource(string keywords)
        {
            var rtn = TracyFacade.Instance.Manager.ResourceProvider.FindResource(keywords).ToList();
            return new GenericServiceResponse<List<Resource>>(rtn);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<CheckMatchedResourcesResult> CheckMatchedResources(string keywords, string regExpr, int sampleCount)
        {
            var resourceList = TracyFacade.Instance.Manager.ResourceProvider.FindResource(keywords);
            var result = TracyFacade.Instance.Manager.ResourceProvider.FilterResource(resourceList, regExpr);
            return new GenericServiceResponse<CheckMatchedResourcesResult>(new CheckMatchedResourcesResult()
            {
                FoundCount = resourceList.Count(),
                MatchedCount = result.Count(),
                SampleList = result.Take(sampleCount).Select((res) => res.Title).ToList()
            });
        }

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate= "/DownloadResource?entryId={entryId}&resourceId={resourceId}")]
        public GenericServiceResponse<ThunderOfflineDownloadTask> DownloadResource(string entryId, string resourceId)
        {
            Entry entry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOneById(new ObjectId(entryId));
            Resource res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(new ObjectId(resourceId));
            var rtn = TracyFacade.Instance.Manager.DownloadResource(entry, res);
            return new GenericServiceResponse<ThunderOfflineDownloadTask>(rtn);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<List<MediaFile>> GetMediaFileList(string entryId)
        {
            List<MediaFile> rtn = new List<MediaFile>();
            Entry entry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOneById(new ObjectId(entryId));
            foreach (ObjectId fileId in entry.MediaFileIds)
            {
                MediaFile file = TracyFacade.Instance.Manager.MediaFileProvider.Collection.FindOneById(fileId);
                if (file != null) rtn.Add(file);
            }
            return new GenericServiceResponse<List<MediaFile>>(rtn);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<string> GetDownloadUrl(string mediaFileId)
        {
            var file = TracyFacade.Instance.Manager.MediaFileProvider.Collection.FindOneById(new ObjectId(mediaFileId));
            return new GenericServiceResponse<string>(TracyFacade.Instance.Manager.GetSharedUrl(file));
        }
    }
}
