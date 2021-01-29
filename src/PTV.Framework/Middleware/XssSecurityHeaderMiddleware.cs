﻿/**
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PTV.Framework.Middleware
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

    public static class RequestInspectionMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestInspection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<XssSecurityMiddleware>();
        }
    }

    public class RequestInspectionMiddleware
    {
        private readonly RequestDelegate nextDelegate;

        public RequestInspectionMiddleware(RequestDelegate next)
        {
            nextDelegate = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var req = httpContext.Request;
            var abc = req;
            await nextDelegate(httpContext);
        }
    }
}
