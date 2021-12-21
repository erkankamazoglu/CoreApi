using System;
using System.Collections.Generic;
using CoreApi.Controllers.Base;
using CoreApi.HelperCodes.Model;
using CoreApi.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            
            resultModel.Model = serviceModelList;
            resultModel.IsSuccess = true;
            resultModel.Messages = new List<string>();
            return ResReturner(resultModel);
        }
    }
}