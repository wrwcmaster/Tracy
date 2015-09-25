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
        static void Main(string[] args)
        {
            
            var client = new TracyServiceClient();
            var tasks = client.GetDownloadTasks();
            foreach(var task in tasks)
            {
                if(task.Status == 0)
                {
                    client.StartTask(task);
                    client.NotifyTaskStart(task.IdString);
                }

            }
        }


    }
}
