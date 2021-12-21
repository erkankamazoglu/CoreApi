using System.Runtime.Serialization;
using CoreApi.Models.Base;

namespace CoreApi.Models
{
    [DataContract]
    public class ProductModel : IdentifiedServiceModel
    {
        [DataMember]
        public string CompanyUniqueId { get; set; }

        [DataMember]
        public string BatchNumber { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember]
        public decimal Price { get; set; } 
 
        public override string GetDomainName()
        {
            return "Product";
        }
    }
}