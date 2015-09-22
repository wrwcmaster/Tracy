using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Gaia.Common.Serialization;
namespace Tracy
{
    public class CookieHelper
    {
        [ThreadStatic]
        private static Dictionary<string, string> _requestCookieStore = null;

        public static string GetRequestCookie(string key)
        {
            if(_requestCookieStore == null)
            {
                _requestCookieStore = ParseRequestCookie();
            }
            if (!_requestCookieStore.ContainsKey(key))
            {
                return null;
            }
            return _requestCookieStore[key];
        }

        private static Dictionary<string, string> ParseRequestCookie()
        {
            Dictionary<string, string> rtn = new Dictionary<string, string>();
            var cookieStr = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Cookie];
            if (!string.IsNullOrEmpty(cookieStr))
            {
                rtn = KeyValuePairParser.Parse(cookieStr, rtn, ';');
            }
            _requestCookieStore = rtn;
            return _requestCookieStore;
        }

        public static void SetResponseCookie(string key, string value)
        {
            var cookieStr = KeyValuePairParser.Compose<string, string>(new Dictionary<string, string>()
            {
                { key, value }
            }, null, ";");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Set-Cookie", cookieStr);
        }



    }
}
