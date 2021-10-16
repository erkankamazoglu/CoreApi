using System.Runtime.Serialization;

namespace CoreApi.Models.Base
{
    [DataContract]
    public abstract class BaseServiceModel : IBaseServiceModel
    {
        public abstract string GetDomainName();
    }
}