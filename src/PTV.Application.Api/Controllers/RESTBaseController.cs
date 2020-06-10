/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PTV.Framework.Attributes;
using System.Linq;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Framework.Logging;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Base Rest controller, base class for all REST type controllers
    /// </summary>
    [ServiceFilter(typeof(AppExceptionFilterAttribute))]
    public abstract class RESTBaseController : Controller
    {
        /// <summary>
        /// Global message for entity saved
        /// </summary>
        protected const string EntityMessageSaved = "Entity.MessageSaved";
        /// <summary>
        /// Global message for entity published
        /// </summary>
        protected const string EntityMessagePublished = "Entity.MessagePublished";
        /// <summary>
        /// Global message for entity schedule publish
        /// </summary>
        protected const string EntityMessageSchedulePublish = "Entity.MessageSchedulePublish";
        /// <summary>
        /// Global message for entity schedule publish
        /// </summary>
        protected const string EntityMessageScheduleArchive = "Entity.MessageScheduleArchive";
        /// <summary>
        /// Global message for entity withdrawn
        /// </summary>
        protected const string EntityMessageWithdrawn = "Entity.MessageWithdrawn";
        /// <summary>
        /// Global message for entity restored
        /// </summary>
        protected const string EntityMessageRestored = "Entity.MessageRestored";
        /// <summary>
        /// Global message for entity archived
        /// </summary>
        protected const string EntityMessageArchived = "Entity.MessageArchived";
        /// <summary>
        /// Global message for entity cannot be edited
        /// </summary>
        protected const string EntityMessageCannotBeEdited = "Entity.MessageCannotBeEdited";
        /// <summary>
        /// Global message for entity can not be published
        /// </summary>
        protected const string EntityMessageCannotBePublished = "Entity.MessageCannotBePublished";

        private readonly ILogger logger;

        /// <summary>
        /// Base controller of all UI API controllers
        /// </summary>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        protected RESTBaseController(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Handling of errors when actions are called (JSON file is attacked), in case of error the error message is returned
        /// </summary>
        /// <param name="context">context of called action</param>
        /// <param name="next">next action</param>
        /// <returns></returns>
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
//            var user = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email");
            logger.LogRequestEntry(new VmRequestLogEntry
            {
                    Method = Request.Method,
                    Action = Request.Path,
                    Scheme = Request.Scheme,
                    Host = Request.Host.Value,
                    QueryString = Request.QueryString.Value,
                    UserName = string.Empty});
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

        /// <summary>
        /// Writes the error message to the http response
        /// </summary>
        /// <param name="message">message returned in response</param>
        private void ResponseErrorMessage(string message)
        {
            var jsonString = @"{ error: """+ message + @""" }";
            Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            Response.StatusCode = 400;
            Response.WriteAsync(jsonString, System.Text.Encoding.UTF8);
        }
    }
}
