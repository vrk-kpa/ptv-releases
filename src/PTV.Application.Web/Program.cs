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
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NLog.Web;

namespace PTV.Application.Web
{
    /// <summary>
    /// Application.Web Entry point
    /// </summary>
    public class Program
    {
        // entry point update 4
        public static void Main(string[] args)
        {
            var portNumberString = args?.FirstOrDefault();
            int portNumber;
            int.TryParse(portNumberString, out portNumber);

            var bindAddressString = args?.Skip(1).FirstOrDefault();
            if (string.IsNullOrEmpty(bindAddressString))
            {
                bindAddressString = "localhost";
            }

            var hostBuilder = new WebHostBuilder()
                .UseKestrel(o =>
                {
                    o.Limits.MinRequestBodyDataRate = null;
                    o.Limits.MinResponseDataRate = null;
                    o.Limits.MaxResponseBufferSize = null;
                    //                    o.NoDelay = true; // dont use nagle
                    //                    o.ShutdownTimeout = new TimeSpan(0,0,0,60); // wait 60s before shutdown if any request is ongoing
                    //                    o.ThreadCount = 16; // I/O threads
                    //                    o.Limits.KeepAliveTimeout = new TimeSpan(0,0,10,0); // 10min timeout for request
                    //                    o.Limits.MaxResponseBufferSize = null; // max response size, null = unlimited
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseNLog()
                .UseStartup<StartupWeb>();

            if (portNumber > 0)
            {

                hostBuilder = hostBuilder.UseUrls("http://" + bindAddressString + ":" + portNumber);
            }
            StartupWeb.AppSettingsKey = args?.Length >= 3 ? args[2] : string.Empty;
            var host = hostBuilder.Build();
            host.Run();
        }
    }
}
