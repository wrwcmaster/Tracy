using System;
using System.Collections.Generic;
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
            client.Test();
        }
    }
}
