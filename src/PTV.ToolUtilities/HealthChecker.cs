using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.ToolUtilities
{
    /// <summary>
    /// Verifying status of core components using external resources
    /// </summary>
    [RegisterService(typeof(HealthChecker), RegisterType.Singleton)]
    public class HealthChecker
    {
        private readonly ProxyServerSettings proxySettings;
        private readonly TestAccessUrlConfiguration testAccessUrlConfiguration;
        private readonly MapServiceProvider mapServiceProvider;
        private readonly AnnotationServiceProvider annotationServiceProvider;
        private readonly IContextManager contextManager;
        private readonly string proxyPureAddress = null;
        private readonly string proxyFullUri = null;
        private readonly int proxyPort = 0;
        private readonly bool proxyConfigured = false;
        private readonly string newLine = Environment.NewLine;
        private int callers;
        private const int MaxCalls = 3;
        private readonly bool initialized;
        private ILogger<HealthChecker> logger;

        public HealthChecker(IOptions<ProxyServerSettings> proxySettings, IOptions<TestAccessUrlConfiguration> testAccessUrlConfiguration, MapServiceProvider mapServiceProvider, AnnotationServiceProvider annotationServiceProvider, IContextManager contextManager, ILogger<HealthChecker> logger)
        {
            initialized = true;
            this.proxySettings = proxySettings.Value;
            this.testAccessUrlConfiguration = testAccessUrlConfiguration.Value;
            this.mapServiceProvider = mapServiceProvider;
            this.annotationServiceProvider = annotationServiceProvider;
            this.contextManager = contextManager;
            this.logger = logger;
            if (!string.IsNullOrEmpty(this.proxySettings.Address) && !string.IsNullOrEmpty(this.proxySettings.Port))
            {
                try
                {
                    string address = this.proxySettings.Address.ToLower();
                    string httpPrefix = "http://";
                    string httpsPrefix = "https://";
                    if (address.StartsWith(httpPrefix))
                    {
                        address = address.Remove(0, httpPrefix.Length);
                    }
                    else if (address.StartsWith(httpsPrefix))
                    {
                        address = address.Remove(0, httpsPrefix.Length);
                    }
                    if (address.EndsWith("/"))
                    {
                        address = address.Remove(address.Length - 1, 1);
                    }
                    proxyPureAddress = address;
                    int.TryParse(this.proxySettings.Port, out proxyPort);
                    proxyFullUri = string.Format("{0}:{1}", this.proxySettings.Address, this.proxySettings.Port);
                    var testProxy = new WebProxy(proxyFullUri);
                }
                catch (Exception e)
                {
                    initialized = false;
                    logger.LogError(e.Message);
                }
            }
            proxyConfigured = !string.IsNullOrEmpty(proxyPureAddress) && (proxyPort != 0);
        }

        /// <summary>
        /// Call all checkers at once
        /// </summary>
        /// <returns></returns>
        public string CallAllCheckers()
        {
            HealthCheckResult resultDatabase = HealthCheckResult.NotTested;
            HealthCheckResult resultProxy = HealthCheckResult.NotTested;
            HealthCheckResult resultInternet = HealthCheckResult.NotTested;
            HealthCheckResult resultMap = HealthCheckResult.NotTested;
            HealthCheckResult resultAnnotation = HealthCheckResult.NotTested;
            if (initialized)
            {
                var testingThreads = new List<Thread>()
                {
                    new Thread(() => { resultDatabase = contextManager.VerifyDatabaseConnection(); }),
                    new Thread(() => { resultProxy = VerifyProxyServer(); }),
                    new Thread(() => { resultInternet = VerifyInternetAccess(); }),
                    new Thread(() => { resultMap = VerifyMapServiceAccess(); }),
                    new Thread(() => { resultAnnotation = VerifyAnnotationServiceAccess(); })
                };
                if (callers > MaxCalls)
                {
                    return $"Another test already in progress";
                }
                lock (this)
                {
                    callers++;
                    try
                    {
                        testingThreads.ForEach(t => t.Start());
                        testingThreads.ForEach(t => t.Join());
                    }
                    catch (Exception) {}
                    callers--;
                    if (callers < 0) callers = 0;
                }
            }
            return $"{resultDatabase}{newLine}Database connection: {resultDatabase}{newLine}Proxy server: {resultProxy}{newLine}Internet acccess: {resultInternet}{newLine}Map service: {resultMap}{newLine}Annotation service: {resultAnnotation}";
        }


        /// <summary>
        /// Test connection to proxy server (not outside)
        /// </summary>
        /// <returns></returns>
        public HealthCheckResult VerifyProxyServer()
        {
            if (!proxyConfigured) return HealthCheckResult.NotConfigured;
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.ConnectAsync(proxyPureAddress, proxyPort).Wait(3000);
                return tcpClient.Connected ? HealthCheckResult.Ok :  HealthCheckResult.Failed;
            }
        }

        /// <summary>
        /// Test internet access
        /// </summary>
        /// <returns></returns>
        public HealthCheckResult VerifyInternetAccess()
        {
            if (string.IsNullOrEmpty(testAccessUrlConfiguration.Uri)) return HealthCheckResult.NotConfigured;
            return CallHttpClientWithProxy(c => (c.GetStringAsync(testAccessUrlConfiguration.Uri).Result ?? string.Empty).Contains("html") ? HealthCheckResult.Ok : HealthCheckResult.Failed);
        }

        /// <summary>
        /// Test response of map service for coordinates
        /// </summary>
        /// <returns></returns>
        public HealthCheckResult VerifyMapServiceAccess()
        {
            return mapServiceProvider.CallTestAddress() ? HealthCheckResult.Ok : HealthCheckResult.Failed;
        }

        /// <summary>
        /// Test response of annotation service
        /// </summary>
        /// <returns></returns>
        public HealthCheckResult VerifyAnnotationServiceAccess()
        {
            return annotationServiceProvider.CallTestAnnotation();
        }

        private TResult CallHttpClientWithProxy<TResult>(Func<HttpClient, TResult> action)
        {
            WebProxy proxy = null;
            if (proxyConfigured)
            {
                NetworkCredential proxyCreds = new NetworkCredential(proxySettings.UserName, proxySettings.Password);
                proxy = new WebProxy(proxyFullUri)
                {
                    Credentials = proxyCreds
                };
            }
            using (HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            })
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    return action(httpClient);
                }
            }
        }
    }
}
