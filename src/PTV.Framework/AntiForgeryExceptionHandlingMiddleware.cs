using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PTV.Framework
{
    public class AntiForgeryExceptionHandlingMiddleware
    {
        private readonly RequestDelegate nextDelegate;

        public AntiForgeryExceptionHandlingMiddleware(RequestDelegate next)
        {
            nextDelegate = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await nextDelegate(httpContext);
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is CryptographicException)
                {
                    httpContext.Response.Redirect("/");
                }
            }
        }
    }

    public static class AntiForgeryExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseCatchingAntiForgery(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiForgeryExceptionHandlingMiddleware>();
        }
    }
}