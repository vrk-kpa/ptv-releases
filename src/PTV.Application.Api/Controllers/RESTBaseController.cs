/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;
using System.Linq;
using Newtonsoft.Json;
using PTV.Framework;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Base Rest controller, base class for all REST type controllers
    /// </summary>
    [Microsoft.AspNetCore.Mvc.ServiceFilter(typeof(AppExceptionFilterAttribute))]
    public abstract class RESTBaseController : Controller
    {
        private ILogger logger;

        public RESTBaseController(ILogger logger)
        {
            this.logger = logger;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelState = context.ModelState.FirstOrDefault();
            bool jsonFailed = modelState.Value?.Errors?.Any(i => i.Exception.GetType() == typeof(JsonReaderException)) == true;
            if (jsonFailed)
            {
                logger.LogError(CoreMessages.WrongRequestFormat);
                ResponseErrorMessage(CoreMessages.WrongRequestFormat);
                return Task.FromResult(400);
            }
            return base.OnActionExecutionAsync(context, next);
        }


        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        public void ResponseErrorMessage(string message)
        {
            var jsonString = @"{ error: """+ message + @""" }";
            Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            Response.StatusCode = 400;
            Response.WriteAsync(jsonString, System.Text.Encoding.UTF8);
        }
    }
}
