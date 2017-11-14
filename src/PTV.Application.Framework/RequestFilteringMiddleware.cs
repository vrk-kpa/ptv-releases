using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly IHttpContextAccessor httpContextAccessor;
        private static readonly ThreadSafeCounter<string> RequestsPerIp = new ThreadSafeCounter<string>();
        private static readonly ThreadSafeCounter<string> RequestsPerUser = new ThreadSafeCounter<string>();

        private static Dictionary<string, int> ipAddressLimits = new Dictionary<string, int>();
        private static Dictionary<string, int> userLimits = new Dictionary<string, int>();
        private int ipAddressLimitGlobal = 64;
        private int userLimitGlobal = 64;

        private long requestTotalCount = 0;
        private DateTime requestCountingSince = DateTime.UtcNow;

        private readonly RequestDelegate nextDelegate;
        private readonly string tag;
        private readonly IUserIdentification userIdentification;
        private readonly IConfigurationService configurationService;
        private RequestFilterAppSetting appSettings;

        public RequestFilteringMiddleware(RequestDelegate next, string tag, IHttpContextAccessor httpContextAccessor, IUserIdentification userIdentification, IConfigurationService configurationService, IOptions<RequestFilterAppSetting> appSettings)
        {
            this.nextDelegate = next;
            this.httpContextAccessor = httpContextAccessor;
            this.tag = tag;
            this.userIdentification = userIdentification;
            this.configurationService = configurationService;
            this.appSettings = appSettings.Value;
            if (this.appSettings != null)
            {
                ipAddressLimitGlobal = this.appSettings.MaxRequestsPerIp;
                userLimitGlobal = this.appSettings.MaxRequestsPerUser;
            }
            if (ipAddressLimitGlobal <= 0) ipAddressLimitGlobal = int.MaxValue;
            if (userLimitGlobal <= 0) userLimitGlobal = int.MaxValue;
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
                ipAddressLimitGlobal = newGlobalIpLimit.Value;
            }
            if (newGlobalUserLimit != null)
            {
                userLimitGlobal = newGlobalUserLimit.Value;
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
            var clientIp = GetRequestIP();
            if (clientIp.IsNullOrWhitespace())
            {
                await nextDelegate(httpContext);
                return;
            }
            var requestLimit = ipAddressLimits.TryGetOrDefault(clientIp, ipAddressLimitGlobal);
            var requestsCurrent = RequestsPerIp.GetStatus(clientIp);
            if (requestsCurrent >= requestLimit || RequestsPerIp.GetTotalCounts >= ipAddressLimitGlobal)
            {
                // request denied
                ReturnTooManyRequestsNotification(httpContext);
                return;
            }

            var userName = userIdentification.UserName;
            requestLimit = userLimits.TryGetOrDefault(userName, userLimitGlobal);
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

        private string GetRequestIP()
        {
            string ip = GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();
            if (ip.IsNullOrWhitespace() && httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
                ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace())
                ip = GetHeaderValueAs<string>("REMOTE_ADDR");

            ip = (ip ?? string.Empty).Trim();
            return ip;
        }

        private T GetHeaderValueAs<T>(string headerName)
        {
            if (httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
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