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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PTV.Framework;
using PTV.Database.DataAccess.Services;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Framework
{
    public class ThreadSafeCounter<TKey> : Dictionary<TKey, int>
    {
        private long globalCounter = 0;

        public void Increase(TKey key)
        {
            lock (this)
            {
                this[key] = this.TryGetOrDefault(key, 0) + 1;
                globalCounter++;
            }
        }

        public void Decrease(TKey key)
        {
            lock (this)
            {
                var value = this.TryGetOrDefault(key, 0) - 1;
                if (value <= 0)
                {
                    this.Remove(key);
                }
                else
                {
                    this[key] = value;
                }
                globalCounter--;
                if (globalCounter < 0) globalCounter = 0;
            }
        }

        public int GetStatus(TKey key)
        {
            return this.TryGetOrDefault(key, 0);
        }

        public long GetTotalCounts => globalCounter;

    }

    public interface IRequestFiltering
    {
        Dictionary<string, int> GetStatistic();
    }

    public static class RequestFilteringMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestFiltering(this IApplicationBuilder builder, string tag)
        {
            return builder.UseMiddleware<RequestFilteringMiddleware>(tag);
        }
    }


    [RegisterService(typeof(IRequestFiltering), RegisterType.Singleton)]
    public class RequestFilteringMiddleware : IRequestFiltering
    {
        private static readonly ThreadSafeCounter<string> RequestsPerIp = new ThreadSafeCounter<string>();
        private static readonly ThreadSafeCounter<string> RequestsPerUser = new ThreadSafeCounter<string>();

        private static Dictionary<string, int> ipAddressLimits = new Dictionary<string, int>();
        private static Dictionary<string, int> userLimits = new Dictionary<string, int>();
        private int perIpAddressLimit = 64;
        private int ipAddressLimitGlobal = 128;
        private int perUserLimit = 64;

        private long requestTotalCount = 0;
        private DateTime requestCountingSince = DateTime.UtcNow;

        private readonly RequestDelegate nextDelegate;
        private readonly IConfigurationService configurationService;
        private readonly string tag;

        public RequestFilteringMiddleware(RequestDelegate next, string tag, IConfigurationService configurationService, IOptions<RequestFilterAppSetting> appSettings)
        {
            this.nextDelegate = next;
            this.tag = tag;
            this.configurationService = configurationService;
            var appSettingsLimits = appSettings.Value;
            if (appSettingsLimits != null)
            {
                perIpAddressLimit = appSettingsLimits.MaxRequestsPerIp;
                perUserLimit = appSettingsLimits.MaxRequestsPerUser;
                ipAddressLimitGlobal = appSettingsLimits.MaxRequestsTotal;
            }
            if (perIpAddressLimit <= 0) perIpAddressLimit = int.MaxValue;
            if (perUserLimit <= 0) perUserLimit = int.MaxValue;
            if (ipAddressLimitGlobal <= 0) ipAddressLimitGlobal = int.MaxValue;
        }

        public void ReloadLimits()
        {
            var configurationData = configurationService.GetRequestFilterConfiguration(tag);
            ipAddressLimits = configurationData.Where(i => !i.IPAddress.IsNullOrEmpty()).DistinctBy(i => i.IPAddress).ToDictionary(i => i.IPAddress, i => i.ConcurrentRequests);
            userLimits = configurationData.Where(i => !i.UserName.IsNullOrEmpty()).DistinctBy(i => i.UserName).ToDictionary(i => i.UserName, i => i.ConcurrentRequests);
            var newGlobalIpLimit = configurationData.FirstOrDefault(i => i.IPAddress.IsNullOrEmpty())?.ConcurrentRequests;
            var newGlobalUserLimit = configurationData.FirstOrDefault(i => i.UserName.IsNullOrEmpty())?.ConcurrentRequests;
            if (newGlobalIpLimit != null)
            {
                perIpAddressLimit = newGlobalIpLimit.Value;
            }
            if (newGlobalUserLimit != null)
            {
                perUserLimit = newGlobalUserLimit.Value;
            }
        }

        private void ReturnTooManyRequestsNotification(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 429;
            httpContext.Response.WriteAsync(CoreMessages.TooManyRequests, Encoding.UTF8).Wait();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            requestTotalCount++;
            var clientIp = GetRequestIP(httpContext);
            if (clientIp.IsNullOrWhitespace())
            {
                await nextDelegate(httpContext);
                return;
            }
            var requestLimit = ipAddressLimits.TryGetOrDefault(clientIp, perIpAddressLimit);
            var requestsCurrent = RequestsPerIp.GetStatus(clientIp);
            if (requestsCurrent >= requestLimit || RequestsPerIp.GetTotalCounts >= ipAddressLimitGlobal)
            {
                // request denied
                ReturnTooManyRequestsNotification(httpContext);
                return;
            }

            var userName = (httpContext.RequestServices.GetService(typeof(IUserIdentification)) as IUserIdentification)?.UserName ?? string.Empty;
            requestLimit = userLimits.TryGetOrDefault(userName, perUserLimit);
            requestsCurrent = RequestsPerUser.GetStatus(userName);
            if (requestsCurrent >= requestLimit)
            {
                // request denied
                ReturnTooManyRequestsNotification(httpContext);
                return;
            }

            RequestsPerIp.Increase(clientIp);
            RequestsPerUser.Increase(userName);
            try
            {
                await nextDelegate(httpContext);
            }
            catch (PtvDbTooManyConnectionsException)
            {
                ReturnTooManyRequestsNotification(httpContext);
            }
            finally
            {
                RequestsPerIp.Decrease(clientIp);
                RequestsPerUser.Decrease(userName);
            }
        }

        private string GetRequestIP(HttpContext httpContext)
        {
            if (httpContext == null) return string.Empty;
            string ip = GetHeaderValueAs<string>(httpContext,"X-Forwarded-For").SplitCsv().FirstOrDefault();
            if (ip.IsNullOrWhitespace() && httpContext.Connection?.RemoteIpAddress != null)
                ip = httpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace())
                ip = GetHeaderValueAs<string>(httpContext, "REMOTE_ADDR");

            ip = (ip ?? string.Empty).Trim();
            return ip;
        }

        private T GetHeaderValueAs<T>(HttpContext httpContext, string headerName)
        {
            if (httpContext == null) return default(T);
            if (httpContext.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                string rawValues = values.ToString();

                if (!rawValues.IsNullOrEmpty())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }

        public Dictionary<string, int> GetStatistic()
        {
            return new Dictionary<string, int>(RequestsPerIp);
        }
    }
}