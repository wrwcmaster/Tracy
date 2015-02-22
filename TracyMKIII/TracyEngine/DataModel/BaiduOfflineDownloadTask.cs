using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataModel
{
    public class BaiduOfflineDownloadTask : BaseMongoModel
    {
        public ObjectId ResourceId { get; set; }
        public ObjectId EntryId { get; set; }
        public string LocalTorrentPath { get; set; }
        public string TorrentMD5 { get; set; }
        public string TorrentSHA1 { get; set; }
        public string RemoteTorrentPath { get; set; }
        public long RemoteTaskId { get; set; }
        public int Status { get; set; }
        public List<ObjectId> FileIds { get; set; }
    }
}
