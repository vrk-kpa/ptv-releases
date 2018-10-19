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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PTV.Framework.Converters;
using PTV.Framework.Interfaces;

namespace PTV.Framework
{
    public static class PerformanceMonitoringExtensions
    {
        public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceMonitoringMiddleware>();
        }
    }
    
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate nextDelegate;
        private readonly PerformanceStatisticsHolder performanceStatisticsHolder;

        public PerformanceMonitoringMiddleware(RequestDelegate next, PerformanceStatisticsHolder performanceStatisticsHolder)
        {
            nextDelegate = next;
            this.performanceStatisticsHolder = performanceStatisticsHolder;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var performanceMonitorManager = (IPerformanceMonitorManager)httpContext.RequestServices.GetService(typeof(IPerformanceMonitorManager));
            var measuring = performanceMonitorManager.StartMeasuring($"{httpContext.Request.Method} {httpContext.Request.Path}");
            await nextDelegate(httpContext);
            performanceMonitorManager.StopMeasuring(measuring);
            performanceStatisticsHolder.Add(DateTime.UtcNow, performanceMonitorManager.GetMeasures());
        }
    }
}