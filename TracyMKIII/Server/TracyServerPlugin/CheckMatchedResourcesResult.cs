using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TracyServerPlugin
{
    [DataContract]
    public class CheckMatchedResourcesResult
    {
        [DataMember(Name = "foundCount")]
        public int FoundCount { get; set; }
        [DataMember(Name = "matchedCount")]
        public int MatchedCount { get; set; }
        [DataMember(Name = "sampleList")]
        public List<string> SampleList { get; set; }
    }
}
