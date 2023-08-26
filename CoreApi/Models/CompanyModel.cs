using System.Runtime.Serialization;
using CoreApi.Models.Base;

namespace CoreApi.Models
{
    [DataContract]
    public class CompanyModel : IdentifiedServiceModel
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public string TaxNumber { get; set; }

        [DataMember]
        public string TaxOffice { get; set; }  

        public override string GetDomainName()
        {
            return "Company";
        } 
    }
}