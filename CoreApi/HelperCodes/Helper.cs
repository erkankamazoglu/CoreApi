using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AutoMapper.Configuration.Conventions;

namespace CoreApi.HelperCodes
{
    public static class Helper
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

        public static Dictionary<string, string> IdentifiedBaseServiceModelMappedProps(Type serviceModelType)
        {
            Dictionary<string, string> propDictionary = new Dictionary<string, string>();
            PropertyInfo[] props = serviceModelType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;

                if (propName == "Id")
                {
                    continue;
                }

                object[] attrs = prop.GetCustomAttributes(true);
                bool isMapto = false;
                foreach (object attr in attrs)
                {
                    //MapTo Attribute Check 
                    MapToAttribute mapToAttr = attr as MapToAttribute;
                    if (mapToAttr != null)
                    {
                        string maptoName = mapToAttr.MatchingName;
                        propDictionary.Add(propName, maptoName);
                        isMapto = true;
                    }
                }

                if (!isMapto)
                {
                    propDictionary.Add(propName, propName);
                }
            }

            return propDictionary;
        }

        public static string GetUpdateTempTableQuery(Dictionary<string, string> serviceModelPropDictionary)
        {
            //To Mapped a Foreign Key 
            StringBuilder alterTempQuerySb = new StringBuilder("");
            StringBuilder updateTempQuerySb = new StringBuilder("");

            foreach (var propDictItem in serviceModelPropDictionary)
            { 
                if (propDictItem.Value == "CompanyUniqueId")
                {
                    alterTempQuerySb.Append("ALTER TABLE #tempTableName# ADD CompanyId INT; ");
                    updateTempQuerySb.Append("T.CompanyId = (select top 1 C.Id from Company C where C.UniqueId = T.CompanyUniqueId and C.StatusType = 1) ");
                } 
                //else if more columns to map etc.
                //do not forget to separate it with commas
            }

            if(!string.IsNullOrEmpty(updateTempQuerySb.ToString()))
            {
                updateTempQuerySb.Insert(0, "UPDATE T SET ");
                updateTempQuerySb.Append(" FROM #tempTableName# T;");

                updateTempQuerySb.Insert(0, alterTempQuerySb.ToString());
            } 
            
            return updateTempQuerySb.ToString();
        }

        public static List<string> ColumnListToExtract()
        {
            return new List<string> {
                "CompanyUniqueId"
            }; 
        }
    }
}