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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Attributes
{
    [RegisterService(typeof(AppExceptionFilterAttribute), RegisterType.Singleton)]
    public class AppExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        public AppExceptionFilterAttribute(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AppExceptionFilterAttribute>();
        }
        public override void OnException(ExceptionContext context)
        {
            _logger.LogInformation("OnActionExecuting");
            if (context.Exception != null)
            {
                _logger.LogError("Server error :", context.Exception);

                IError errorToReturn = context.Exception is PtvDbTooManyConnectionsException
                    ? new Error("Rejected: Too many connections to database. Try again later.", null)
                    : new Error("Server error: " + context.Exception.Message, null, context.Exception.StackTrace);

                context.Result = new JsonResult(new ServiceResultWrap
                {
                    Messages = new VmMessages
                    {
                        Errors = new List<IError> { errorToReturn },
                    }
                });
            }
            base.OnException(context);
        }
    }
}
