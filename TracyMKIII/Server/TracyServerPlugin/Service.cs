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
        public ServiceResponse CheckTasks()
        {
            TracyFacade.Instance.Manager.CheckTasks();
            return new ServiceResponse();
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public ServiceResponse Test()
        {
            TracyFacade.Instance.Manager.Test();
            return new ServiceResponse();
        }

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        public GenericServiceResponse<Entry> AddEntry(Entry newEntry)
        {
            TracyFacade.Instance.Manager.EntryProvider.Collection.Insert(newEntry);
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
            var rtn = TracyFacade.Instance.Manager.ResourceProvider.FindResource(keywords);
            return new GenericServiceResponse<List<Resource>>(rtn);
        }
    }
}
