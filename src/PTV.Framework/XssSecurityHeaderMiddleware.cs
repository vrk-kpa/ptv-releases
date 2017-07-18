using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PTV.Framework
{
    public static class XssSecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseXssSecurity(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<XssSecurityMiddleware>();
        }
    }

    public class XssSecurityMiddleware
    {
        private readonly RequestDelegate nextDelegate;

        public XssSecurityMiddleware(RequestDelegate next)
        {
            nextDelegate = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            await nextDelegate(httpContext);
        }
    }
}
