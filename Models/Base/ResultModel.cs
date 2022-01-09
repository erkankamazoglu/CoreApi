using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoreApi.Models.Base
{
    [DataContract]
    public class ResultModel
    {
        public ResultModel()
        {
            Messages = new List<string>();
        }

        [DataMember]
        public List<string> Messages { get; set; }  

        [DataMember]
        public bool IsSuccess { get; set; } 

        [DataMember]
        public object Model { get; set; } 
    }
}