using CoreApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.Controllers
{
    [Route("[controller]")]
    public class ApiController : ApiBaseController
    {
        [Route("{text}/Bulk")]
        [HttpPost]
        public string Bulk(string text, [FromBody] object jsonObject)
        { 
            return "Test";
        }
    }
}