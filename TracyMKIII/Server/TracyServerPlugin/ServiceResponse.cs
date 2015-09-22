using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TracyServerPlugin
{
    [DataContract]
    public class ServiceResponse
    {
        [DataMember(Name ="errorCode")]
        public int ErrorCode { get; set; }
        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public class GenericServiceResponse<T> : ServiceResponse
    {
        [DataMember(Name = "result")]
        public T Result { get; set; }

        public GenericServiceResponse(T data)
        {
            Result = data;
        }

        public GenericServiceResponse()
        {
        }
    }
}
