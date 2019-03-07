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
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.Extensions.Logging;
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
            int portNumber;
            int.TryParse(portNumberString, out portNumber);

            var bindAddressString = args?.Skip(1).FirstOrDefault();
            if (string.IsNullOrEmpty(bindAddressString))
            {
                bindAddressString = "localhost";
            }

            var hostBuilder = new WebHostBuilder()
                .UseLibuv(i => i.ThreadCount = Environment.ProcessorCount)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostContext.Configuration.GetSection("Logging"));

                    if (hostContext.HostingEnvironment.IsDevLikeEnvironment())
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
                .UseStartup<Startup>();
            if (portNumber > 0)
            {
                Startup.BindingUrl = "http://" + bindAddressString + ":" + portNumber;
                hostBuilder = hostBuilder.UseUrls(Startup.BindingUrl);
            }
            Startup.AppSettingsKey = args?.Length >= 3 ? args[2] : string.Empty;
            var host = hostBuilder.Build();
            host.Run();
        }
    }
}