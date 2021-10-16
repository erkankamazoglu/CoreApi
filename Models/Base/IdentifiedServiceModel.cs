using System.Runtime.Serialization;

namespace CoreApi.Models.Base
{
    [DataContract]
    public abstract class IdentifiedServiceModel : IBaseServiceModel
    {
        [DataMember]
        public virtual int Id { get; set; }   

        [DataMember]
        public virtual string UniqueId { get; set; } 
        
        public abstract string GetDomainName();
    }
}