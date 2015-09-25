using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Tracy.DataModel;

namespace TracyServerPlugin
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<Entry> AddEntry(Entry newEntry);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse CheckDownloadTasks();
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<CheckMatchedResourcesResult> CheckMatchedResources(string keywords, string regExpr, int sampleCount);
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/DownloadResource?entryId={entryId}&resourceId={resourceId}")]
        GenericServiceResponse<ThunderOfflineDownloadTask> DownloadResource(string entryId, string resourceId);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<ThunderOfflineDownloadTask>> GetDownloadTasks();
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<string> GetDownloadUrl(string mediaFileId);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<Entry>> GetEntryList(string sessionId);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<MediaFile>> GetMediaFileList(string entryId);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<Resource>> GetResourceList(string entryId);
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<string> Login(Service.LoginInfo loginInfo);
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<User> Register(Service.UserCreationInfo newUserInfo);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<Resource>> SearchResource(string keywords);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse Sync(string startPage);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<string> Test();
    }
}