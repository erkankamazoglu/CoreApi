
namespace CoreApi.HelperCodes.Miscellaneous
{
    public static class LookupHelper
    {
       public static string GetUniqueIds(string modelName)
       {
           return "select Distinct(UniqueId) from " + modelName + " where UniqueId is not null and LTRIM(RTRIM(UniqueId)) != '' and StatusType = 1";
       }
    }
}