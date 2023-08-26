using System;
using System.Collections.Generic;
using CoreApi.Controllers.Base;
using CoreApi.HelperCodes.Model;
using CoreApi.Models.Base;
using CoreApi.HelperCodes.AdoNet;
using CoreApi.HelperCodes.Miscellaneous;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using CoreApi.HelperCodes; 
using System.Dynamic;

namespace CoreApi.Controllers
{
    [Route("[controller]")]
    public class ApiController : ApiBaseController
    {
        [Route("{text}/Bulk")]
        [HttpPost]
        public string Bulk(string text, [FromBody] object jsonObject)
        {
            ResultModel resultModel = new ResultModel();
            dynamic messageModel = new ExpandoObject();

            IdentifiedServiceModel mockServiceModel = ModelCreator.GetModelFromName(text);
            string baseModelName = mockServiceModel.GetDomainName();

            Type serviceModelType = mockServiceModel.GetType();
            Type serviceModelListType = typeof(List<>).MakeGenericType(serviceModelType);
            dynamic deserializeObject = JsonConvert.DeserializeObject(jsonObject.ToString(), serviceModelListType);
            List<IdentifiedServiceModel> serviceModelList = new List<IdentifiedServiceModel>();
            for (int i = 0; i < deserializeObject.Count; i++)
            {
                serviceModelList.Add(deserializeObject[i]);
            }

            string baseModelUniqueIdsQuery = LookupHelper.GetUniqueIds(baseModelName);
            DataTable baseModelUniqueIdsDt = DatabaseHelper.PullData(baseModelUniqueIdsQuery);
            HashSet<string> baseModelUniqueIdList = baseModelUniqueIdsDt.AsEnumerable().Select(row => row.Field<string>("UniqueId")).ToHashSet();

            HashSet<string> serviceModelUniqueIdList = serviceModelList.Select(i => i.UniqueId).Where(i => i != null && i != string.Empty).ToHashSet();

            HashSet<string> addUniqueIdList = serviceModelUniqueIdList.Except(baseModelUniqueIdList).ToHashSet();
            HashSet<string> deleteUniqueIdList = baseModelUniqueIdList.Except(serviceModelUniqueIdList).ToHashSet();
            HashSet<string> updateUniqueIdList = baseModelUniqueIdList.Intersect(serviceModelUniqueIdList).ToHashSet();

            Dictionary<string, string> serviceModelPropDictionary = Helper.IdentifiedBaseServiceModelMappedProps(serviceModelType); 
            string updateTempTableQuery = Helper.GetUpdateTempTableQuery(serviceModelPropDictionary);

            bool isOk = true;

            if (addUniqueIdList.Any())
            {
                List<IdentifiedServiceModel> addServiceModelList = serviceModelList.Where(i => addUniqueIdList.Contains(i.UniqueId)).ToList();

                DataTable addDt = DataTableHelper.GenerateDataTableByPropDictionary(serviceModelPropDictionary);
                addDt.PopulateDataTableByModelList(addServiceModelList, serviceModelPropDictionary, serviceModelType);

                isOk = DatabaseHelper.BulkInsertData(addDt, baseModelName, updateTempTableQuery);
            }

            if (updateUniqueIdList.Any())
            {
                List<IdentifiedServiceModel> updateServiceModelList = serviceModelList.Where(i => updateUniqueIdList.Contains(i.UniqueId)).ToList();

                DataTable updateDt = DataTableHelper.GenerateDataTableByPropDictionary(serviceModelPropDictionary);
                updateDt.PopulateDataTableByModelList(updateServiceModelList, serviceModelPropDictionary, serviceModelType);

                isOk = isOk && DatabaseHelper.BulkUpdateData(updateDt, baseModelName, updateTempTableQuery);
            }

            if (deleteUniqueIdList.Any())
            {
                DataTable deleteDt = new DataTable();
                deleteDt.Columns.Add("UniqueId");

                foreach (string deleteUniqueId in deleteUniqueIdList)
                {
                    deleteDt.Rows.Add(deleteUniqueId);
                }

                isOk = isOk && DatabaseHelper.BulkDeleteData(deleteDt, baseModelName);
            }

            resultModel.IsSuccess = isOk;
            messageModel.AddUniqueIds = addUniqueIdList;
            messageModel.UpdateUniqueIds = updateUniqueIdList;
            messageModel.DeleteUniqueIds = deleteUniqueIdList;
            resultModel.Model = messageModel;
 
            resultModel.Messages.Add(isOk ? "Operation Successful" : "Operation Failed");
            return ResReturner(resultModel);
        }
    }
}