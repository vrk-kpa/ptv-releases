using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Framework.Logging;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTV.Framework.Attributes.Next;

namespace PTV.Application.Api.Controllers.Next
{
    [ServiceFilter(typeof(UnhandledExceptionFilterAttribute))]
    public class BaseController: Controller
    {
        private readonly ILogger logger;

        public BaseController(ILogger logger)
        {
            this.logger = logger;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            logger.LogRequestEntry(new VmRequestLogEntry
            {
                Method = Request.Method,
                Action = Request.Path,
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                QueryString = Request.QueryString.Value,
                UserName = string.Empty
            });
            var modelState = context.ModelState.FirstOrDefault();
            var jsonFailed = modelState.Value?.Errors?.Any(i => i.Exception.GetType() == typeof(JsonReaderException)) == true;
            if (jsonFailed)
            {
                logger.LogError(CoreMessages.WrongRequestFormat);
                ResponseErrorMessage(CoreMessages.WrongRequestFormat);
                return Task.FromResult(400);
            }

            return base.OnActionExecutionAsync(context, next);
        }

        private void ResponseErrorMessage(string message)
        {
            var jsonString = @"{ error: """+ message + @""" }";
            Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            Response.StatusCode = 400;
            Response.WriteAsync(jsonString, System.Text.Encoding.UTF8);
        }
    }
}
