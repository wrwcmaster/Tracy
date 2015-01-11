using Gaia.Common.Execute.Control;
using Gaia.Common.Net.Http;
using Gaia.Common.Net.Http.RequestModifier;
using Gaia.Common.Net.Http.ResponseParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.ResourceSource.Dmhy
{
    public class DmhyTorrentDownloader
    {
        public static HttpResponseFileParser.TempFileInfo Download(string fileUrl, IExecutionControl control)
        {
            var result = HttpHelper.SendRequest(new Uri(fileUrl),
                HttpMethod.GET,
                new List<IHttpRequestModifier>(){
                    new HttpRequestSimpleHeaderModifier("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36"),
                    new HttpRequestSimpleHeaderModifier("Cookie", "rsspass=59d8de300951337103b3b54a73; uid=62326")
                },
                new HttpResponseFileParser(),
                control);
            return result;
        }
    }
}
