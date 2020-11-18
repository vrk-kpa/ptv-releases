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
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using PTV.Framework;

namespace PTV.TaskScheduler
{
    /// <summary>
    /// Application class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// the entry point of the application
        /// </summary>
        public static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;
            System.Threading.ThreadPool.GetMaxThreads(out int _, out int completionThreads);
            System.Threading.ThreadPool.SetMinThreads(256, completionThreads);

            var portNumberString = args?.FirstOrDefault();
            int.TryParse(portNumberString, out var portNumber);

            var bindAddressString = args?.Skip(1).FirstOrDefault();
            if (string.IsNullOrEmpty(bindAddressString))
            {
                bindAddressString = "localhost";
            }

            ConfigureLoggingAndStart(portNumber, bindAddressString, args);
        }

        private static void ConfigureLoggingAndStart(int portNumber, string bindAddressString, string[] args)
        {
            var hostingEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            var envConfigFilename = $"nlog.{hostingEnvironment}.config";
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(File.Exists(envConfigFilename) ? envConfigFilename : "nlog.config").GetCurrentClassLogger();         
            try
            {
                logger.Debug("init main");
                var hostBuilder = CreateWebHostBuilder(args);
                if (portNumber > 0)
                {
                    Startup.BindingUrl = "http://" + bindAddressString + ":" + portNumber;
                    hostBuilder = hostBuilder.UseUrls(Startup.BindingUrl);
                }

                Startup.AppSettingsKey = args?.Length >= 3 ? args[2] : string.Empty;
                hostBuilder.Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(logging =>
                {
                    // logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog(); // NLog: Setup NLog for Dependency injection

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((hostContext, logging) =>
                {
                    if (!hostContext.HostingEnvironment.IsDevelopment() && !Debugger.IsAttached)
                    {
                        logging.ClearProviders();
                    }

                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog(); // NLog: setup NLog for Dependency injection
    }
}
