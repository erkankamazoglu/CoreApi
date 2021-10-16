using System.ComponentModel;

namespace CoreApi.HelperCodes.Enumerate
{
    public class Enums
    {
        public enum StatusTypeEnum 
        {
            [Description("Not Determined")]
            Na = -1,
            
            [Description("Active")]
            Active = 1, 
            
            [Description("Deleted")]
            Deleted = 2
        }
    }
}