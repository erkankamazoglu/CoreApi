using System.Collections.Generic;

namespace CoreApi.HelperCodes
{
    public class Helper
    {
        public static bool ModelCheckName(string sourceName, string targetName)
        {
            if (sourceName.ToLower() == targetName.ToLower() 
                || sourceName.ToLower() == targetName.ToLower() + "model")
            {
                return true;
            }

            return false;
        }
        public static bool ModelCheckName(List<string> sourceNameList, string targetName)
        {
            bool isCheck = false;
            foreach (string sourceName in sourceNameList)
            {
                isCheck = isCheck || ModelCheckName(sourceName, targetName);
            } 

            return isCheck;
        }
    }
}