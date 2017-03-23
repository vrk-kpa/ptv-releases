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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Attributes;
using PTV.Application.OpenApi.Models;
using PTV.Framework.Attributes;
using PTV.Framework.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Open Api base controller.
    /// </summary>
    [Microsoft.AspNetCore.Mvc.ServiceFilter(typeof(OpenApiExceptionFilterAttribute))]
    public class BaseController : Controller
    {
        private readonly AppSettings settings;
        private ILogger logger;

        /// <summary>
        /// Gets the reference to AppSettings.
        /// </summary>
        protected AppSettings Settings { get { return settings; } private set { } }

        /// <summary>
        /// BaseController constructor.
        /// </summary>
        public BaseController(IOptions<AppSettings> settings, ILogger logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Handle logs
        /// </summary>
        /// <param name="context">context of called action</param>
        /// <param name="next">next action</param>
        /// <returns></returns>
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email");
            logger.LogRequestEntry(new VmRequestLogEntry()
            {
                Method = Request.Method,
                Action = Request.Path,
                Scheme = Request.Scheme,
                Host = Request.Host.Value,
                QueryString = Request.QueryString.Value,
                UserName = user?.Value
            });
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
