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
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTV.Application.Framework;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.ToolUtilities;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Responsible for providing status of components using external resources like database and services
    /// </summary>
    [AllowAnonymous]
    public class HealthCheckApiController : Controller
    {
        private readonly HealthChecker healthChecker;
        private readonly EnvironmentHelper environmentHelper;
        private readonly PerformanceStatisticsHolder performanceStatisticsHolder;
        private readonly IResolveManager resolveManager;

        /// <summary>
        /// Constructor of HealthCheckApiController
        /// </summary>
        /// <param name="healthChecker"></param>
        /// <param name="environmentHelper"></param>
        /// <param name="performanceStatisticsHolder"></param>
        /// <param name="resolveManager"></param>
        public HealthCheckApiController(HealthChecker healthChecker, EnvironmentHelper environmentHelper, PerformanceStatisticsHolder performanceStatisticsHolder, IResolveManager resolveManager)
        {
            this.healthChecker = healthChecker;
            this.environmentHelper = environmentHelper;
            this.performanceStatisticsHolder = performanceStatisticsHolder;
            this.resolveManager = resolveManager;
        }

        /// <summary>
        /// Simple test that app is responding
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Ping")]
        public IActionResult Ping()
        {
            return Content(HealthCheckResult.Ok.ToString());
        }


        /// <summary>
        /// Advanced test, provides status of each core component
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Pong")]
        public IActionResult Pong()
        {
            var result = environmentHelper.GetExecutingEnvironment() == ExecutingEnvironment.Web || environmentHelper.GetExecutingEnvironment() == ExecutingEnvironment.Unknown
                ? HealthCheckResult.Ok.ToString()
                : healthChecker.CallAllCheckers();
            return Content($"{result}{Environment.NewLine}{DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss",CultureInfo.InvariantCulture)}");
        }

        /// <summary>
        /// Measured statistics
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("StatsMon")]
        public IActionResult StatsMon()
        {
            var header = $"Statistics of requests{Environment.NewLine}-----------{Environment.NewLine}";
            var stats = performanceStatisticsHolder.GetStatistics;
            if (!stats.Any())
            {
                return Content(header+"Nothing");
            }
            return Content(header+string.Join(Environment.NewLine, stats.OrderByDescending(i => i.Timestamp).Select(FormatStatLine)));
        }

        private string FormatStatLine(RequestStatisticsMeasure i)
        {
            return $"{i.Timestamp.ToString("dd.MM.yy HH:mm:ss.fff", CultureInfo.InvariantCulture)}{Environment.NewLine}{string.Join(Environment.NewLine, i.Data.Prepend(i.Data.Last()).SkipLast(1).Select(FormatOneRow))}";
        }

        private string FormatOneRow(MeasuredRecord j)
        {
            return j == null ? string.Empty : $"{j.Origin} = {j.Time}ms";
        }

        /// <summary>
        /// THe worst measurements
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("StatsWorst")]
        public IActionResult StatsWorst()
        {
            var header = $"Worst (longest) requests{Environment.NewLine}-----------{Environment.NewLine}";
            var stats = performanceStatisticsHolder.CalculateWorstMeasurements();
            if (!stats.Any())
            {
                return Content("Nothing");
            }
            return Content(header+string.Join(Environment.NewLine, stats.Select(FormatStatLine)));
        }

        /// <summary>
        /// Measured statistics
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("StatsReqs")]
        public IActionResult StatsReqs()
        {
            var filteringMiddleware = resolveManager.Resolve<IRequestFiltering>(true);
            if (filteringMiddleware == null)
            {
                return Content("Not available");
            }
            var header = $"Statistics of requests{Environment.NewLine}-----------{Environment.NewLine}";
            var stats = filteringMiddleware.GetRequestsStatistics();
            var totalReqs = $"Current total active requests: {stats.TotalRequests}{Environment.NewLine}";
            var anonymousReqs = $"Current anonymous requests: {stats.AnonymousRequests}{Environment.NewLine}";
            var limits = string.Join(Environment.NewLine, stats.LoadedLimits.Select(i => $"Username: {i.UserName}, IP address: {i.IPAddress}, limit: {i.ConcurrentRequests}"));
            return Content(header+totalReqs+anonymousReqs+limits+Environment.NewLine+$"Limits loaded: {stats.LimitsLoadedAt}");
        }

        /// <summary>
        /// Statistics of connections
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("StatsCons")]
        public IActionResult StatsCons()
        {
            var header = $"Active TCP connections:{Environment.NewLine}-----------{Environment.NewLine}";

            var usedPorts = new List<int>();
            var tcpStateDictionary = ((TcpState[]) Enum.GetValues(typeof(TcpState))).ToDictionary(tcpState => tcpState.ToString(), tcpState => 0);
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = properties.GetActiveTcpConnections();

            var tcpStateOrderMap = new[] {12,6,13,8,7,11,5,4,3,2,10,1,9};
            var orderedConnections = connections.Select(x => new ConnectionInformation
                {
                    LocalEndPoint = x.LocalEndPoint.ToString(),
                    RemoteEndPoint = x.RemoteEndPoint.ToString(),
                    State = x.State,
                    Port = x.RemoteEndPoint.Port
                }
            ).OrderBy(x => tcpStateOrderMap[(int) x.State]).ToList();

            var activeTcpConnectionsStringBuilder = new StringBuilder();
            activeTcpConnectionsStringBuilder.AppendLine("All active connections:");
            if (!orderedConnections.Any())
            {
                activeTcpConnectionsStringBuilder.AppendLine("Nothing");
            }

            foreach (var con in orderedConnections)
            {
                activeTcpConnectionsStringBuilder.AppendLine($"{con.LocalEndPoint} <==> {con.RemoteEndPoint} - {con.State.ToString()}");
                if (tcpStateDictionary.ContainsKey(con.State.ToString()))
                {
                    tcpStateDictionary[con.State.ToString()]++;
                }
                usedPorts.Add(con.Port);
            }

            var numberOfConnections = $"Number of connections {orderedConnections.Count} {Environment.NewLine}";
            var numberOfUsedPorts = $"Number of used ports: {usedPorts.Distinct().Count()} {Environment.NewLine}";

            var numberOfTcpStatesStringBuilder = new StringBuilder();
            foreach (var tcpState in tcpStateDictionary.Where(x => x.Value > 0))
            {
                numberOfTcpStatesStringBuilder.AppendLine($"Number of tcp state {tcpState.Key}: {tcpState.Value}");
            }

            return Content(header+numberOfConnections+numberOfUsedPorts+numberOfTcpStatesStringBuilder+activeTcpConnectionsStringBuilder);
        }
    }
}
