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

using System.Net;

using Microsoft.AspNetCore.Mvc.Filters;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using PTV.Framework.Exceptions;

namespace PTV.Application.OpenApi.Attributes
{
    /// <summary>
    /// Exception filter
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute" />
    [RegisterService(typeof(OpenApiExceptionFilterAttribute), RegisterType.Singleton)]
    public class OpenApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<OpenApiExceptionFilterAttribute> logger;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="logger">ILogger to be used for exception logging.</param>
        public OpenApiExceptionFilterAttribute(ILogger<OpenApiExceptionFilterAttribute> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// decorates the response, when exception happen
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // log the exception, some case we will get multiple log entries about the same issue, but main point is that we need to cover
            // the cases that are not actually handled and logged => currently we have in prod error cases but there is no log what went wrong
            var currentException = context.Exception;
            if (currentException != null)
            {
                logger.LogError(new EventId(3500, "GenericException"),
                    $"Generic exception handler caught an exception. Details: {currentException}");
            }

            // https://github.com/aspnet/Mvc/issues/5594 , if setting the ExceptionHandled to true, no response body is sent to the caller
            //context.ExceptionHandled = true;
            context.Result = new ObjectResult(new VmError { ErrorMessage = context.Exception.Message });
            if (currentException is ExternalSourceNotFoundException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            base.OnException(context);
        }
    }
}
