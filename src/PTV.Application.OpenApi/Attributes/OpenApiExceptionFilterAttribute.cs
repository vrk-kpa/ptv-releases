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

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using Microsoft.Extensions.Logging;
using System;

namespace PTV.Application.OpenApi.Attributes
{
    /// <summary>
    /// Excpetion filter
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute" />
    [RegisterService(typeof(OpenApiExceptionFilterAttribute), RegisterType.Singleton)]
    public class OpenApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<OpenApiExceptionFilterAttribute> Logger;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="logger">ILogger to be used for exception logging.</param>
        public OpenApiExceptionFilterAttribute(ILogger<OpenApiExceptionFilterAttribute> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// decorates the response, when exception happen
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            // log the exception, some case we will get multiple log entries about the same issue, but main point is that we need to cover
            // the cases that are not actually handled and logged => currently we have in prod error cases but there is no log what went wrong
            Exception currentException = context?.Exception;
            if (currentException != null)
            {
                Logger.LogError(new EventId(3500, "GenericException"), $"Generic exception handler catched an exception. Details: {currentException.ToString()}");
            }

            // https://github.com/aspnet/Mvc/issues/5594 , if setting the ExceptionHandled to true, no response body is sent to the caller
            //context.ExceptionHandled = true;
            context.Result = new ObjectResult(new VmError() { ErrorMessage = context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            base.OnException(context);
        }
    }
}
