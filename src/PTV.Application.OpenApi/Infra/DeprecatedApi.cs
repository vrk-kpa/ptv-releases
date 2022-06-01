using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PTV.Application.OpenApi.Infra
{
    public class DeprecatedApi
    {
        public static ObjectResult CreateResponse()
        {
            var result = new ObjectResult("This API has been deprecated and it is no longer available.");
            result.StatusCode = StatusCodes.Status410Gone;
            return result;
        }
    }    
}

