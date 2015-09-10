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
        [WebGet]
        public void Sync(string startPage)
        {
            if (String.IsNullOrEmpty(startPage))
            {
                TracyFacade.Instance.Manager.SyncResource();
            }
            else
            {
                TracyFacade.Instance.Manager.SyncResource(int.Parse(startPage));
            }
        }

        [OperationContract]
        [WebGet]
        public void CheckTasks()
        {
            TracyFacade.Instance.Manager.CheckTasks();
        }

        [OperationContract]
        [WebGet]
        public void Test()
        {
            TracyFacade.Instance.Manager.Test();
        }

        [OperationContract]
        [WebInvoke]
        public Entry AddEntry(Entry newEntry)
        {
            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            TracyFacade.Instance.Manager.EntryProvider.Collection.Insert(newEntry);
            return newEntry;
        }

        [OperationContract]
        [WebGet]
        public List<Entry> GetEntryList()
        {
            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            return TracyFacade.Instance.Manager.EntryProvider.Collection.FindAll().ToList();
        }

        [OperationContract]
        [WebGet]
        public List<Resource> GetResourceList(string entryId)
        {
            List<Resource> rtn = new List<Resource>();
            WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            Entry entry = TracyFacade.Instance.Manager.EntryProvider.Collection.FindOneById(new ObjectId(entryId));
            foreach (ObjectId resId in entry.ResourceIds)
            {
                Resource res = TracyFacade.Instance.Manager.ResourceProvider.Collection.FindOneById(resId);
                if(res != null) rtn.Add(res);
            }
            return rtn;
        }
    }
}
