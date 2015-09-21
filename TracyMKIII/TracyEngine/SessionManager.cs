
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;

namespace Tracy
{
    public class SessionManager
    {
        public class Session : Dictionary<string, string>
        {
            public string Id
            {
                get
                {
                    return this["id"];
                }

                set
                {
                    this["id"] = value;
                }
            }

            public Session(string newId)
            {
                Id = newId;
            }
        }

        [ThreadStatic]
        private static string _currentSessionId = null;

        private static Dictionary<string, Session> _sessionDict = new Dictionary<string, Session>();

        private static Session CreateNewSession()
        {
            string newId = null;
            do
            {
                newId = Guid.NewGuid().ToString();
            } while (_sessionDict.ContainsKey(newId));
            var rtn = new Session(newId);
            _sessionDict[newId] = rtn;
            return rtn;
        }

        public static Session BuildSession()
        {
            var rtn = CreateNewSession();
            _currentSessionId = rtn.Id;
            CookieHelper.SetResponseCookie("session-id", rtn.Id);
            return rtn;
        }

        public static Session CurrentSession
        {
            get
            {
                if(_currentSessionId == null)
                {
                    _currentSessionId = CookieHelper.GetRequestCookie("session-id");
                }
                if(_currentSessionId != null && _sessionDict.ContainsKey(_currentSessionId))
                {
                    return _sessionDict[_currentSessionId];
                }
                return null;
            }
        }
        
    }
}
