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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PTV.Framework;
using PTV.Database.DataAccess.Services;
using PTV.Framework.ServiceManager;
// ReSharper disable StringLiteralTypo

namespace PTV.Application.Framework
{
    public class ThreadSafeCounter<TKey>
    {
        private readonly Dictionary<TKey, StatHolder> data = new Dictionary<TKey, StatHolder>();

        private class StatHolder
        {
            internal TKey Key { get; }
            internal readonly List<Guid> RequestIds = new List<Guid>();
            internal int Counter;

            internal StatHolder(TKey key)
            {
                Key = key;
            }
        }

        private long globalCounter;

        public bool Increase(TKey key, int limit, Guid requestId, int totalLimit = Int32.MaxValue)
        {
            lock (this)
            {
                if (globalCounter >= totalLimit)
                {
                    return false;
                }
                var stat = data[key] = data.TryGetOrDefault(key, new StatHolder(key));
                if (stat.Counter >= limit)
                {
                    return false;
                }
                stat.RequestIds.Add(requestId);
                stat.Counter++;
                globalCounter++;
                return true;
            }
        }

        public void Decrease(TKey key, Guid requestId)
        {
            lock (this)
            {
                var value = data.TryGetOrDefault(key);
                if (value == null)
                {
                    return;
                }

                if (!value.RequestIds.Remove(requestId))
                {
                    return;
                }
                if (--value.Counter <= 0)
                {
                    data.Remove(key);
                }
                globalCounter--;
                if (globalCounter < 0) globalCounter = 0;
            }
        }

        public void Decrease(Guid requestId)
        {
            lock (this)
            {
                var value = data.Values.FirstOrDefault(i => i.RequestIds.Contains(requestId));
                if (value == null) return;
                value.RequestIds.Remove(requestId);
                if (--value.Counter <= 0)
                {
                    data.Remove(value.Key);
                }
                globalCounter--;
                if (globalCounter < 0) globalCounter = 0;
            }
        }

        // ReSharper disable once InconsistentlySynchronizedField
        public long GlobalCount => globalCounter;

        public int GetCountForKey(TKey key)
        {
            try
            {
                lock (this)
                {
                    return data.TryGetOrDefault(key, new StatHolder(key)).Counter;
                }
            }
            catch
            {
                return 0;
            }
        }
    }

    public interface IRequestFiltering
    {
        RequestsStatistics GetRequestsStatistics();
    }

    public interface IRequestFilteringManager : IRequestFiltering
    {
    }


    public static class RequestFilteringMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestFiltering(this IApplicationBuilder builder, string tag)
        {
            return builder.UseMiddleware<RequestFilteringMiddleware>(tag);
        }
    }

    [RegisterService(typeof(IRequestFiltering), RegisterType.Singleton)]
    public class RequestFilteringManager : IRequestFilteringManager
    {
        internal const string AnonymousKey = "none";
        internal const string AnyUserKey = "any";

        internal static readonly List<string> UnlimitedActions = new List<string> { "statsreqs", "statsmon", "statsworst", "ping", "pong" };

        internal readonly ThreadSafeCounter<string> RequestsPerIp = new ThreadSafeCounter<string>();
        internal readonly ThreadSafeCounter<string> RequestsPerUser = new ThreadSafeCounter<string>();

        internal Dictionary<string, List<RequestFilterConfigurationData>> IPAddressLimits = new Dictionary<string, List<RequestFilterConfigurationData>>();
        internal Dictionary<string, List<RequestFilterConfigurationData>> UserLimits = new Dictionary<string, List<RequestFilterConfigurationData>>();

        internal DateTime RequestCountingSince = DateTime.UtcNow;
        internal DateTime LimitsLoadedAt = DateTime.MinValue;

        public RequestsStatistics GetRequestsStatistics()
        {
            return new RequestsStatistics(RequestsPerIp.GlobalCount, RequestsPerUser.GetCountForKey(AnonymousKey),
                IPAddressLimits.Values.SelectMany(i => i).Union(UserLimits.Values.SelectMany(i => i)).Distinct().ToList(), LimitsLoadedAt);
        }
    }

    public class RequestFilteringMiddleware
    {
        private readonly TimeSpan reloadLimitsInInterval = TimeSpan.FromHours(1);

        private readonly RequestFilteringManager requestFilteringManager;

        private readonly int perIpAddressLimit = 64;
        private readonly int perUserLimit = 64;
        private readonly int anonymousLimit = 128;
        private readonly int ipAddressLimitGlobal = 1024;

        private readonly RequestDelegate nextDelegate;
        private readonly IConfigurationService configurationService;
        private readonly string tag;

        private readonly RequestFilterConfigurationData maxUserNameLimit;

        private readonly RequestFilterConfigurationData maxIpAddressLimit;

        private Thread reloadingThread;

        private bool UnlimitedAccess(string request)
        {
            if (request.Length <= 1) return false;
            return RequestFilteringManager.UnlimitedActions.Contains(request.Substring(1).ToLowerInvariant());
        }

        public RequestFilteringMiddleware(RequestDelegate next, string tag, IConfigurationService configurationService, IOptions<RequestFilterAppSetting> appSettings, IRequestFiltering requestFiltering)
        {
            requestFilteringManager = requestFiltering as RequestFilteringManager;
            this.nextDelegate = next;
            this.tag = tag;
            this.configurationService = configurationService;
            var appSettingsLimits = appSettings.Value;
            if (appSettingsLimits != null)
            {
                perIpAddressLimit = appSettingsLimits.MaxRequestsPerIp.ValueOrDefaultIfZero(perIpAddressLimit);
                perUserLimit = appSettingsLimits.MaxRequestsPerUser.ValueOrDefaultIfZero(perUserLimit);
                ipAddressLimitGlobal = appSettingsLimits.MaxRequestsTotal.ValueOrDefaultIfZero(ipAddressLimitGlobal);
                anonymousLimit = appSettingsLimits.MaxAnonymousRequestsPerIp.ValueOrDefaultIfZero(anonymousLimit);
            }
            if (perIpAddressLimit <= 0) perIpAddressLimit = int.MaxValue;
            if (perUserLimit <= 0) perUserLimit = int.MaxValue;
            if (ipAddressLimitGlobal <= 0) ipAddressLimitGlobal = int.MaxValue;
            if (anonymousLimit <= 0) anonymousLimit = int.MaxValue;
            maxUserNameLimit = new RequestFilterConfigurationData
            {
                UserName = string.Empty,
                IPAddress = string.Empty,
                ConcurrentRequests = perUserLimit
            };
            maxIpAddressLimit = new RequestFilterConfigurationData
            {
                UserName = string.Empty,
                IPAddress = string.Empty,
                ConcurrentRequests = perIpAddressLimit
            };
            ReloadLimits();
            StartLimitsReloader();
        }

        private void ReloadLimits()
        {
            var configurationData = configurationService.GetRequestFilterConfiguration(tag);
            if (!configurationData.Any(i => i.UserName.ToLowerInvariant() == RequestFilteringManager.AnonymousKey && string.IsNullOrEmpty(i.IPAddress)))
            {
                configurationData.Add(new RequestFilterConfigurationData
                {
                    UserName = RequestFilteringManager.AnonymousKey,
                    IPAddress = string.Empty,
                    ConcurrentRequests = anonymousLimit
                });
            }
            configurationData.Where(i => string.IsNullOrEmpty(i.UserName)).ForEach(i => i.UserName = RequestFilteringManager.AnyUserKey);
            if (!configurationData.Any(i => i.UserName.ToLowerInvariant() == RequestFilteringManager.AnyUserKey && string.IsNullOrEmpty(i.IPAddress)))
            {
                configurationData.Add(new RequestFilterConfigurationData
                {
                    UserName = RequestFilteringManager.AnyUserKey,
                    IPAddress = string.Empty,
                    ConcurrentRequests = perUserLimit
                });
            }
            requestFilteringManager.IPAddressLimits = configurationData.GroupBy(i => i.IPAddress).ToDictionary(i => i.Key, i => i.Select(j => j).ToList());
            requestFilteringManager.UserLimits = configurationData.GroupBy(i => i.UserName).ToDictionary(i => i.Key, i => i.Select(j => j).ToList());
            requestFilteringManager.LimitsLoadedAt = DateTime.UtcNow;
        }

        private void StartLimitsReloader()
        {
            reloadingThread = new Thread(() =>
            {
                while (Thread.CurrentThread.ThreadState == ThreadState.Background)
                {
                    try
                    {
                        Thread.Sleep(reloadLimitsInInterval);
                        ReloadLimits();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }){ IsBackground = true};
            reloadingThread.Start();
        }

        private void ReturnTooManyRequestsNotification(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 429;
            httpContext.Response.WriteAsync(CoreMessages.TooManyRequests, Encoding.UTF8).Wait();
        }

        private bool IsRequestAllowed(string userName, string ipAddress, Guid requestId)
        {
            var userNameLimit = requestFilteringManager.UserLimits.TryGetOrCallDefault(userName, () =>
                requestFilteringManager.UserLimits.TryGetOrCallDefault(RequestFilteringManager.AnyUserKey, () =>
                    new List<RequestFilterConfigurationData> {maxUserNameLimit}));
            var ipAddressLimit = requestFilteringManager.IPAddressLimits.TryGetOrCallDefault(ipAddress, () => requestFilteringManager.IPAddressLimits.TryGetOrCallDefault(string.Empty, () => new List<RequestFilterConfigurationData> {maxIpAddressLimit}));
            if (requestFilteringManager.RequestsPerUser.Increase(userName, (userNameLimit.FirstOrDefault(i => i.IPAddress == ipAddress) ?? userNameLimit.FirstOrDefault(i => string.IsNullOrEmpty(i.IPAddress)) ?? userNameLimit.First()).ConcurrentRequests, requestId, ipAddressLimitGlobal))
            {
                if (requestFilteringManager.RequestsPerIp.Increase(ipAddress, (ipAddressLimit.FirstOrDefault(i => i.UserName == userName) ?? ipAddressLimit.FirstOrDefault(i => i.UserName == RequestFilteringManager.AnyUserKey) ?? ipAddressLimit.First()).ConcurrentRequests, requestId))
                {
                    return true;
                }
                requestFilteringManager.RequestsPerUser.Decrease(userName, requestId);
            }
            return false;
        }

        private void RequestFinished(string userName, string ipAddress, Guid requestId)
        {
            requestFilteringManager.RequestsPerIp.Decrease(ipAddress, requestId);
            requestFilteringManager.RequestsPerUser.Decrease(userName, requestId);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var requestPath = httpContext?.Request?.Path.Value ?? string.Empty;
            if (UnlimitedAccess(requestPath))
            {
                await nextDelegate(httpContext);
                return;
            }
            var requestId = Guid.NewGuid();
            //requestTotalCount++;
            var clientIp = GetRequestIP(httpContext);
            if (clientIp.IsNullOrWhitespace())
            {
                await nextDelegate(httpContext);
                return;
            }

            var userName = (httpContext?.RequestServices?.GetService(typeof(IUserIdentification)) as IUserIdentification)?.UserName;
            userName = string.IsNullOrEmpty(userName) ? RequestFilteringManager.AnonymousKey : userName;
            if (!IsRequestAllowed(userName, clientIp, requestId))
            {
                ReturnTooManyRequestsNotification(httpContext);
                return;
            }

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
                RequestFinished(userName, clientIp, requestId);
            }
        }

        private string GetRequestIP(HttpContext httpContext)
        {
            if (httpContext == null) return string.Empty;
            var ip = GetHeaderValueAs<string>(httpContext,"X-Forwarded-For").SplitCsv().FirstOrDefault();
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
                var rawValues = values.ToString();

                if (!rawValues.IsNullOrEmpty())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }


    }

    public class RequestsStatistics
    {
        public long TotalRequests { get; }
        public long AnonymousRequests { get; }

        public List<RequestFilterConfigurationData> LoadedLimits { get; }
        public DateTime LimitsLoadedAt { get; }

        public RequestsStatistics(long totalRequests, long anonymousRequests, List<RequestFilterConfigurationData> limits, DateTime loadedAt)
        {
            TotalRequests = totalRequests;
            AnonymousRequests = anonymousRequests;
            LoadedLimits = limits;
            LimitsLoadedAt = loadedAt;
        }


    }
}
