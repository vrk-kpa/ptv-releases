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
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;

namespace PTV.Application.Api
{
    /// <summary>
    /// Main class of the application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point of the application
        /// </summary>
        /// <param name="args">input parameters when app is started</param>
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
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseNLog()
                .UseStartup<StartupApi>();

            if (portNumber > 0)
            {

                hostBuilder = hostBuilder.UseUrls("http://" + bindAddressString + ":" + portNumber);
            }
            StartupApi.AppSettingsKey = args?.Length >= 3 ? args[2] : string.Empty;
            var host = hostBuilder.Build();
            host.Run();
        }
    }
}
