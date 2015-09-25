using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Tracy;
using Tracy.DataModel;
using UserManagement;

namespace TracyServerPlugin
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public partial class Service : IService
    {
        private T HandleRequest<T>(string sessionId, Action<T> operation) where T : ServiceResponse, new()
        {
            SessionManager.RestoreSession(sessionId);
            var response = new T();
            if(TracyFacade.Instance.UserManager.GetCurrentUser() == null)
            {
                response.ErrorCode = 403;
                response.ErrorMessage = "Access Denied.";
            }
            else
            {
                try
                {
                    operation(response);
                }
                catch (Exception ex)
                {
                    response.ErrorCode = 500;
                    response.ErrorMessage = ex.GetType().ToString() + ": " + ex.Message + " " + ex.StackTrace;
                }
            }
            return response;
        }

        
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

        
        public ServiceResponse CheckDownloadTasks()
        {
            TracyFacade.Instance.Manager.CheckDownloadTasks();
            return new ServiceResponse();
        }

        
        public GenericServiceResponse<Entry> AddEntry(Entry newEntry)
        {
            TracyFacade.Instance.Manager.AddEntry(newEntry);
            return new GenericServiceResponse<Entry>(newEntry);
        }

        
        public GenericServiceResponse<List<Entry>> GetEntryList(string sessionId)
        {
            return HandleRequest<GenericServiceResponse<List<Entry>>>(sessionId, (response) =>
            {
                response.Result = TracyFacade.Instance.Manager.EntryProvider.Collection.FindAll().ToList();
            });
        }

        
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

        
        public GenericServiceResponse<List<Resource>> SearchResource(string keywords)
        {
            var rtn = TracyFacade.Instance.Manager.ResourceProvider.FindResource(keywords).ToList();
            return new GenericServiceResponse<List<Resource>>(rtn);
        }

        
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

        
        public GenericServiceResponse<ThunderOfflineDownloadTask> DownloadResource(string entryId, string resourceId)
        {
            Entry entry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOneById(new ObjectId(entryId));
            Resource res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(new ObjectId(resourceId));
            var rtn = TracyFacade.Instance.Manager.DownloadResource(entry, res);
            return new GenericServiceResponse<ThunderOfflineDownloadTask>(rtn);
        }

        
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

        
        public GenericServiceResponse<string> GetDownloadUrl(string mediaFileId)
        {
            var file = TracyFacade.Instance.Manager.MediaFileProvider.Collection.FindOneById(new ObjectId(mediaFileId));
            return new GenericServiceResponse<string>(TracyFacade.Instance.Manager.GetSharedUrl(file));
        }

        
        public GenericServiceResponse<string> Test()
        {
            return new GenericServiceResponse<string>(OperationContext.Current.SessionId);
            /*var result = TracyFacade.Instance.Manager.DownloadManager.Test();
            return new GenericServiceResponse<string>(result);*/
            
            /*var cookie = WebOperationContext.Current.IncomingRequest.Headers.Get("Cookie");
            if (String.IsNullOrEmpty(cookie) || cookie.IndexOf("session=") < 0)
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Set("Set-Cookie", "session=" + Guid.NewGuid());
            }
            return new GenericServiceResponse<string>(cookie);*/

            /*var start = DateTime.Now;
            var rtn = TracyFacade.Instance.Manager.DownloadManager.TransportToBaiduPan("http://vod29.c12.lixian.vip.xunlei.com/download?fid=qgSQ/XKp21phgaeep7CK3fvHVbJgH+wAAAAAAAJJUb+IZNuFSN3MnNoTqRM7arVQ&mid=666&threshold=150&tid=18408D52ACAB564BC1F04357B664554E&srcid=4&verno=1&g=024951BF8864DB8548DDCC9CDA13A9133B6AB550&scn=c12&i=2F32F2C36990A9468D5FC59E58B71F2D356110F3&t=6&ui=425641022&ti=1062263630599744&s=15474528&m=0&n=013A66A90C55425D5B36508F3E6B6F2D7A005A81025B31315D3A76A6004A505D5B5603D40F5D2E6D705531E45F00000000&ih=2F32F2C36990A9468D5FC59E58B71F2D356110F3&fi=0&pi=1062263630534144&ff=0&co=5A9C4451DFB665FD1D2307FA8DBEDC35&cm=1&pk=lixian&ak=1:1:6:4&e=1450028022&ms=10485760&ck=AD53752A14DD905BF5070C45424E5A27&at=A4E8874DAD1826FECBDCCC55FEEF9D4A&k=1&ts=1442252035", "test1.mp4");
            var end = DateTime.Now;
            return new GenericServiceResponse<string>(rtn + " " + (end - start).TotalMilliseconds + "ms");*/
            
        }

        public GenericServiceResponse<List<ThunderOfflineDownloadTask>> GetDownloadTasks()
        {
            return new GenericServiceResponse<List<ThunderOfflineDownloadTask>>(TracyFacade.Instance.Manager.DownloadManager.GetOnGoingTasks());
        }

    }
}
