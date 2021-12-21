using System.Collections.Generic;
using CoreApi.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoreApi.Controllers.Base
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    { 
        public static string ErrorRes()
        {
            ResultModel resultModel = new ResultModel{
                IsSuccess = false,
                Messages = new List<string>
                {
                    "An error occurred during operation."
                }
            }; 

            return JsonConvert.SerializeObject(resultModel, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None
                }); 
        }

        protected string ResReturner(ResultModel resultModel)
        {
            if (resultModel.IsSuccess)
            {
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        DateFormatString = "dd.MM.yyy hh:mm:ss"
                    });
            }
            else
            {
                resultModel.Messages = new List<string>
                {
                    "An error occurred during operation."
                };
            } 

            return JsonConvert.SerializeObject(resultModel, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    DateFormatString = "dd.MM.yyy hh:mm:ss"
                });
        }
    }
}