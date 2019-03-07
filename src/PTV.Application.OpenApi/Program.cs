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
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Web;
using PTV.Framework;
using PTV.Framework.Logging;

namespace PTV.Application.OpenApi
{
    /// <summary>
    /// Application class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// the entry point of the application
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;
            System.Threading.ThreadPool.GetMaxThreads(out int _, out int completionThreads);
            System.Threading.ThreadPool.SetMinThreads(256, completionThreads);
            
            var portNumberString = args?.FirstOrDefault();
            int portNumber;
            int.TryParse(portNumberString, out portNumber);

            var bindAddressString = args?.Skip(1).FirstOrDefault();

            if (string.IsNullOrEmpty(bindAddressString))
            {
                bindAddressString = "localhost";
            }

            string listenUrl = null;

            if (portNumber > 0)
            {
                listenUrl = $"http://{bindAddressString}:{portNumber}";
            }

            string decryptKey = args?.Length >= 3 ? args[2] : string.Empty;
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-request-ip", typeof(LayoutRendererCustomRequestIp));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-request-duration", typeof(LayoutRendererCustomRequestDuration));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-request-statuscode", typeof(LayoutRendererCustomRequestStatusCode));
            BuildWebHost(decryptKey, listenUrl).Run();
        }

        /// <summary>
        /// Creates the application IWebHost.
        /// </summary>
        /// <param name="appSettingsDecryptKey">encrypted appsettings decrypting key</param>
        /// <param name="urls">array of urls that the host will listen to</param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string appSettingsDecryptKey, params string[] urls)
        {
            var builder = new WebHostBuilder()
                .UseLibuv(i => i.ThreadCount = Environment.ProcessorCount)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostBuilder, configBuilder) =>
                {
                    var env = hostBuilder.HostingEnvironment;

                    configBuilder.AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("version.json", optional: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddJsonFileEncrypted($"appsettings.{env.EnvironmentName}.crypt", appSettingsDecryptKey, optional: true)
                    .AddEnvironmentVariables();

                    // configure NLog
                    env.ConfigureNLogForEnvironment();
                })
                .ConfigureLogging((hostBuilder, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostBuilder.Configuration.GetSection("Logging"));

                    if (hostBuilder.HostingEnvironment.IsDevLikeEnvironment())
                    {
                        // if development environment only then add Console and Debug
                        loggingBuilder.AddConsole();
                        loggingBuilder.AddDebug();

                        // you could also call to set the minimum level to trace BUT appsettings Logging -> LogLevel -> Default will override this
                        // framework default minimumlevel is LogLevel.Information
                        // So we use the appsettings to control this
                        //loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    }
                })
                .UseNLog()
                .UseStartup<StartupOpenApi>();

            if (urls != null && urls.Length > 0)
            {
                builder = builder.UseUrls(urls);
            }

            var host = builder.Build();

            return host;
        }
    }
}
