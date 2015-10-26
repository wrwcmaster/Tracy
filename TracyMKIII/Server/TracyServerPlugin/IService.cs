using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        GenericServiceResponse<string> GetDownloadUrl(string sessionId, string mediaFileId);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<EntryViewModel>> GetEntryList(string sessionId);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<UserMediaFile>> GetMediaFileList(string sessionId, string entryId);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<Resource>> GetResourceList(string entryId);
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse AddBrowseHistory(AddBrowseHistoryParameter parameter);



        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<string> Login(Service.LoginInfo loginInfo);
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<string> Register(Service.UserCreationInfo newUserInfo);


        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<Resource>> SearchResource(string keywords);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse Sync(string startPage);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, UriTemplate = "/NotifyTaskStart")]
        ServiceResponse NotifyTaskStart(ThunderOfflineDownloadTask task);
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse NotifyTaskComplete(string taskId);
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ServiceResponse NotifyFileDownloadComplete(NotifyFileDownloadCompleteParameter param);
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<List<ThunderOfflineDownloadTask>> GetDownloadTasks();
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        ServiceResponse FollowEntry(FollowEntryParameter parameter);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        GenericServiceResponse<MediaFile> Test();
    }

    [DataContract]
    public class NotifyFileDownloadCompleteParameter
    {
        [DataMember(Name = "taskId")]
        public string TaskId { get; set; }
        [DataMember(Name = "mediaFile")]
        public MediaFile MediaFile { get; set; }
    }

    [DataContract]
    public class BasePostParameter
    {
        [DataMember(Name = "sessionId")]
        public string SessionId { get; set; }
    }

    [DataContract]
    public class AddBrowseHistoryParameter : BasePostParameter
    {
        [DataMember(Name = "mediaFileId")]
        public string MediaFileId { get; set; }
    }

    [DataContract]
    public class FollowEntryParameter : BasePostParameter
    {
        [DataMember(Name = "entryId")]
        public string EntryId { get; set; }
        [DataMember(Name = "followFlag")]
        public bool FollowFlag { get; set; }
    }

    [DataContract]
    public class UserMediaFile
    {
        [DataMember(Name = "mediaFile")]
        public MediaFile MediaFile { get; set; }
        [DataMember(Name = "isNew")]
        public bool IsNew { get; set; }
        [DataMember(Name = "lastBrowseDate")]
        public DateTime LastBrowsDate { get; set; }
        [DataMember(Name = "totalBrowseCount")]
        public int TotalBrowseCount { get; set; }
    }

    [DataContract]
    public class EntryViewModel
    {
        [DataMember(Name = "entry")]
        public Entry Entry { get; set; }
        [DataMember(Name = "followDate")]
        public DateTime FollowDate { get; set; }
        [DataMember(Name = "isFollowed")]
        public bool IsFollowed { get; set; }
        [DataMember(Name = "totalFollowedCount")]
        public long TotalFollowedCount { get; set; }
        [DataMember(Name = "maxBrowsedEpisode")]
        public int? MaxBrowsedEpisode { get; set; }
    }
}