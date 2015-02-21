using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Tracy;

namespace TracyServerPlugin
{
    [ServiceContract]
    public class Service
    {


        [OperationContract]
        [WebGet]
        void Sync()
        {
            TracyFacade.Instance.Manager.SyncResource();
        }

        [OperationContract]
        [WebGet]
        void CheckTasks()
        {
            TracyFacade.Instance.Manager.CheckTasks();
        }

        [OperationContract]
        [WebGet]
        void Test()
        {
            TracyFacade.Instance.Manager.Test();
        }
    }
}
