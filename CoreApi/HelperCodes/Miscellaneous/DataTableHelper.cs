using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using CoreApi.Models.Base;

namespace CoreApi.HelperCodes.Miscellaneous
{
    public static class DataTableHelper
    {
        public static void PopulateDataTableByModelList(this DataTable dataTable, List<IdentifiedServiceModel> serviceModelList, Dictionary<string, string> serviceModelPropDictionary, Type serviceModelType)
        {
            PropertyInfo serviceModelPropertyInfo;
            foreach (IdentifiedServiceModel serviceModel in serviceModelList)
            {
                DataRow row = dataTable.NewRow();
                foreach (var propertyKeyValuePair in serviceModelPropDictionary)
                {
                    serviceModelPropertyInfo = serviceModelType.GetProperty(propertyKeyValuePair.Key);
                    if (serviceModelPropertyInfo != null)
                    {
                        object value = serviceModelPropertyInfo.GetValue(serviceModel);
                        row[propertyKeyValuePair.Value] = value;
                    }
                }

                dataTable.Rows.Add(row);
            }
        }

        public static DataTable GenerateDataTableByPropDictionary(Dictionary<string, string> serviceModelPropDictionary)
        {
            DataTable dataTable = new DataTable();

            foreach (var propItem in serviceModelPropDictionary)
            {
                dataTable.Columns.Add(propItem.Value);
            }

            return dataTable;
        }
    }
}