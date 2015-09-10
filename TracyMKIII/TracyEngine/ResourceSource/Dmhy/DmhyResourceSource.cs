using Gaia.Common.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataAccess;
using Tracy.DataModel;

namespace Tracy.ResourceSource.Dmhy
{
    public class DmhyResourceSource : IResourceSource
    {
        public event EventHandler<GenericEventArgs<List<Resource>>> OnResourcesFound;

        public string Name
        {
            get { return "DMHY"; }
        }

        private ResourceProvider Provider { get; set; }

        public DmhyResourceSource(ResourceProvider provider)
        {
            Provider = provider;
        }

        public void Sync(int startPage, bool incrementalMode)
        {
            DateTime lastPublishDate = incrementalMode ? Provider.GetLatestPublishTime(Name) : DateTime.MinValue;
            int i = startPage;
            while (true)
            {
                Console.WriteLine("Syncing page " + i);
                List<Resource> list = DmhyHtmlParser.Parse(i, Name);
                if (list.Count == 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Retrieved " + list.Count + " items - " + list[0].PublishDate.ToString());
                    if (!SaveResourceList(list, lastPublishDate)) break;
                    i++;
                }
            }
            Console.WriteLine("Sync Completed.");
        }

        public bool SaveResourceList(List<Resource> list, DateTime lastPublishDate)
        {
            bool haveNew = true;
            List<Resource> newResources = new List<Resource>();
            foreach (Resource res in list)
            {
                if (res.PublishDate > lastPublishDate)
                {
                    if (Provider.AddResource(res))
                    {
                        newResources.Add(res);
                    }
                }
                else
                {
                    haveNew = false;
                    break;
                }
            }
            if (newResources.Count > 0)
            {
                Console.WriteLine(newResources.Count + " new items found.");
                OnResourcesFound.SafeInvoke(this, new GenericEventArgs<List<Resource>>(newResources));
            }
            return haveNew;
        }
    }
}
