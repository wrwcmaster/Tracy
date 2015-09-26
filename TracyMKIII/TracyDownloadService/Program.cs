using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracyDownloadService
{
    class Program
    {
        static TracyServiceClient client;
        static void Main(string[] args)
        {
            client = new TracyServiceClient();
            client.OnFileComplete += Client_OnFileComplete;
            client.OnTaskComplete += Client_OnTaskComplete;
            var tasks = client.GetDownloadTasks();
            foreach(var task in tasks)
            {
                //Start download
                if(task.Status == 0)
                {
                    client.StartTask(task);
                    client.NotifyTaskStart(task);
                }
                //Check download status
                client.CheckTask(task);

            }
        }

        private static void Client_OnTaskComplete(object sender, Gaia.Common.Event.GenericEventArgs<Tracy.DataModel.ThunderOfflineDownloadTask> e)
        {
            client.NotifyTaskComplete(e.Item.IdString);
        }

        private static void Client_OnFileComplete(object sender, FileCompleteEventArgs e)
        {
            client.NotifyFileDownloadComplete(e.Task.IdString, e.File);
        }
    }
}
