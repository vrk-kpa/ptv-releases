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
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using PTV.Framework.Extensions;

// ReSharper disable EmptyGeneralCatchClause

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
        private readonly IQualityCheckService qualityCheckService;

        private readonly string proxyPureAddress = null;
        private readonly int proxyPort = 0;
        private readonly bool proxyConfigured = false;
        private readonly string newLine = Environment.NewLine;
        private int callers;
        private const int MaxCalls = 3;
        private readonly bool initialized;


        public HealthChecker(IOptions<ProxyServerSettings> proxySettings, IOptions<TestAccessUrlConfiguration> testAccessUrlConfiguration, MapServiceProvider mapServiceProvider, AnnotationServiceProvider annotationServiceProvider, IContextManager contextManager, ILogger<HealthChecker> logger, IQualityCheckService qualityCheckService)
        {
            initialized = true;
            this.proxySettings = proxySettings.Value;
            this.testAccessUrlConfiguration = testAccessUrlConfiguration.Value;
            this.mapServiceProvider = mapServiceProvider;
            this.annotationServiceProvider = annotationServiceProvider;
            this.contextManager = contextManager;
            this.qualityCheckService = qualityCheckService;

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
                    var proxyFullUri = string.Format("{0}:{1}", this.proxySettings.Address, this.proxySettings.Port);
                    // ReSharper disable once UnusedVariable
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

        private HealthCheckResult SetStatusWithExceptionHandling(Func<HealthCheckResult> action)
        {
            try
            {
                return action();
            }
            catch{}
            return HealthCheckResult.Failed;
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
            HealthCheckResult resultQualityAgent = HealthCheckResult.NotTested;
            if (initialized)
            {
                var testingThreads = new List<Thread>
                {
                    new Thread(() => { resultDatabase = SetStatusWithExceptionHandling(() => contextManager.VerifyDatabaseConnection()); }),
                    new Thread(() => { resultProxy = SetStatusWithExceptionHandling(VerifyProxyServer); }),
                    new Thread(() => { resultInternet = SetStatusWithExceptionHandling(VerifyInternetAccess); }),
                    new Thread(() => { resultMap = SetStatusWithExceptionHandling(VerifyMapServiceAccess); }),
                    new Thread(() => { resultAnnotation = SetStatusWithExceptionHandling(VerifyAnnotationServiceAccess); }),
                    new Thread(() => { resultQualityAgent = SetStatusWithExceptionHandling(VerifyQualityAgentServiceAccess); })
                };
                if (callers > MaxCalls)
                {
                    return "Another test already in progress";
                }
                lock (this)
                {
                    callers++;
                    try
                    {
                        testingThreads.ForEach(t => t.Start());
                        testingThreads.ForEach(t => t.Join(TimeSpan.FromMinutes(1)));
                    }
                    catch (Exception) {}
                    callers--;
                    if (callers < 0) callers = 0;
                }
            }

            var process = Process.GetCurrentProcess();
            return $"{resultDatabase}{newLine}Database connection: {resultDatabase}{newLine}Proxy server: {resultProxy}{newLine}Internet access: {resultInternet}{newLine}Map service: {resultMap}{newLine}Annotation service: {resultAnnotation}{newLine}Quality agent service: {resultQualityAgent}{newLine}Processor count: {Environment.ProcessorCount}{newLine}Process name: {process.ProcessName}{newLine}Private memory size: {process.PrivateMemorySize64 / 1024} kb{newLine}Virtual memory size: {process.VirtualMemorySize64 / 1024} kb{newLine}Working set: {process.WorkingSet64 / 1024} kb{newLine}Start time: {process.StartTime}{newLine}Total processor time: {process.TotalProcessorTime}{newLine}";
        }


        /// <summary>
        /// Test connection to proxy server (not outside)
        /// </summary>
        /// <returns></returns>
        private HealthCheckResult VerifyProxyServer()
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
        private HealthCheckResult VerifyInternetAccess()
        {
            if (string.IsNullOrEmpty(testAccessUrlConfiguration.Uri)) return HealthCheckResult.NotConfigured;
            return HttpClientWithProxy.Use(proxySettings, c => (c.GetStringAsync(testAccessUrlConfiguration.Uri).Result ?? string.Empty).Contains("html") ? HealthCheckResult.Ok : HealthCheckResult.Failed, 20);
        }

        /// <summary>
        /// Test response of map service for coordinates
        /// </summary>
        /// <returns></returns>
        private HealthCheckResult VerifyMapServiceAccess()
        {
            return mapServiceProvider.CallTestAddress() ? HealthCheckResult.Ok : HealthCheckResult.Failed;
        }

        /// <summary>
        /// Test response of annotation service
        /// </summary>
        /// <returns></returns>
        private HealthCheckResult VerifyAnnotationServiceAccess()
        {
            return annotationServiceProvider.CallTestAnnotation();
        }

        /// <summary>
        /// Test response of quality agent service
        /// </summary>
        /// <returns></returns>
        private HealthCheckResult VerifyQualityAgentServiceAccess()
        {
            return qualityCheckService.CallTest();
        }
    }
}
