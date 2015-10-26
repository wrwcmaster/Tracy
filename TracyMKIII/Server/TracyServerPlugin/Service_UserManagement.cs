using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Tracy;
using Tracy.DataModel;
using UserManagement;

namespace TracyServerPlugin
{
    public partial class Service
    {
        #region UserManagement

        
        public GenericServiceResponse<string> Register(UserCreationInfo newUserInfo)
        {
            return HandleRequest<GenericServiceResponse<string>>((response) =>
            {
                var newUser = TracyFacade.Instance.UserManager.Register(newUserInfo);
                var loginResponse = Login(new LoginInfo() { UserName = newUserInfo.UserName, Password = newUserInfo.Password });
                response.Result = loginResponse.Result;
                if(response.ErrorCode == 0)
                {
                    response.ErrorCode = loginResponse.ErrorCode;
                    response.ErrorMessage = loginResponse.ErrorMessage;
                }
            });
        }

        [DataContract]
        public class UserCreationInfo : IUserCreationInfo
        {
            [DataMember(Name = "displayName")]
            public string DisplayName { get; set; }
            [DataMember(Name = "email")]
            public string Email { get; set; }
            [DataMember(Name = "password")]
            public string Password { get; set; }
            [DataMember(Name = "userName")]
            public string UserName { get; set; }
        }

        [DataContract]
        public class LoginInfo
        {
            [DataMember(Name = "userName")]
            public string UserName { get; set; }
            [DataMember(Name = "password")]
            public string Password { get; set; }
        }

        public GenericServiceResponse<string> Login(LoginInfo loginInfo)
        {
            TracyFacade.Instance.UserManager.Login(loginInfo.UserName, loginInfo.Password);
            if (SessionManager.CurrentSession == null)
            {
                var rtn = new GenericServiceResponse<string>(null);
                rtn.ErrorCode = 403;
                rtn.ErrorMessage = "Username and password not match.";
                return rtn;
            }
            return new GenericServiceResponse<string>(SessionManager.CurrentSession.Id);
        }

        #endregion
    }
}
