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

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using PTV.MapServer.Common;

namespace PTV.MapServer.ExceptionHandler
{
    public class OwsErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly MapServerConfiguration configuration;

        public OwsErrorHandlingMiddleware(RequestDelegate next, MapServerConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                // must be awaited
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            return (exception is OwsException)
                ? WriteOwsExceptionAsync(context, (OwsException)exception)
                : WriteGeneralExceptionAsync(context, exception);
        }

        private Task WriteOwsExceptionAsync(HttpContext context, OwsException exception)
        {
            if (exception == null) throw new Exception("Not supported exception type.");

            var response = context.Response;
            response.ContentType = "application/xml";
            response.StatusCode = GetStatusCode(exception.Code);
            return response.WriteAsync(SerializeException(context, exception));
        }

        private static Task WriteGeneralExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return response.WriteAsync(exception.Message + Environment.NewLine + exception.StackTrace);
        }

        private string SerializeException(HttpContext context, OwsException exception)
        {
            if (exception == null) return "Not supported exception type.";

            var version = context.Items["version"];
            var exceptionReport = new OwsExceptionReport
            {
                Version = (version == null) ? configuration.DefaultVersion : version.ToString(),
                Exceptions = new [] { new OwsExceptionReportException
                {
                    ExceptionCode = exception.Code.ToString(),
//                    Locator = (string.IsNullOrEmpty(exception.Locator)) ? context.Request.Host+context.Request.Method : exception.Locator,
                    Locator = exception.Locator, // ?? context.Request.Host+context.Request.Path,
                    ExceptionMessages = exception.Messages
                }}
            };

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("ows", Namespaces.Ows);

                    var serializer = new XmlSerializer(typeof(OwsExceptionReport));
                    serializer.Serialize(sw, exceptionReport, ns);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }

            }
        }

        private static int GetStatusCode(OwsExceptionCodeEnum code)
        {
            switch (code)
            {
                case OwsExceptionCodeEnum.OperationNotSupported: return (int)HttpStatusCode.NotImplemented;
                case OwsExceptionCodeEnum.MissingParameterValue: return (int)HttpStatusCode.BadRequest;
                case OwsExceptionCodeEnum.InvalidParameterValue: return (int)HttpStatusCode.BadRequest;
                case OwsExceptionCodeEnum.VersionNegotiationFailed: return (int)HttpStatusCode.BadRequest;
                case OwsExceptionCodeEnum.InvalidUpdateSequence: return (int)HttpStatusCode.BadRequest;
                case OwsExceptionCodeEnum.OptionNotSupported: return (int)HttpStatusCode.InternalServerError;
                case OwsExceptionCodeEnum.NoApplicableCode: return (int)HttpStatusCode.InternalServerError;
                default: throw new ArgumentOutOfRangeException(nameof(code), code, "Unknown exceptionCode type");
            }
        }
    }
}
