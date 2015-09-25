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

        
        public GenericServiceResponse<User> Register(UserCreationInfo newUserInfo)
        {
            var newUser = TracyFacade.Instance.UserManager.Register(newUserInfo);
            return new GenericServiceResponse<User>(newUser);
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
            return new GenericServiceResponse<string>(SessionManager.CurrentSession.Id);
        }

        #endregion
    }
}
