using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Web;
using PTV.Framework.Logging;

namespace PTV.Application.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var portNumberString = args?.FirstOrDefault();
            int.TryParse(portNumberString, out var portNumber);

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
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                    if (hostContext.HostingEnvironment.IsDevelopment() || Debugger.IsAttached)
                    {
                        // if development environment then add Console and Debug
                        loggingBuilder.AddConsole();
                        loggingBuilder.AddDebug();
                        loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                        loggingBuilder.AddFilter("ContextManager", LogLevel.Warning);
                    }
                })
                .UseNLog()
                .UseStartup<Startup>();

            if (portNumber > 0)
            {

                hostBuilder = hostBuilder.UseUrls("http://" + bindAddressString + ":" + portNumber);
            }
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-request-ip", typeof(LayoutRendererCustomRequestIp));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-user-agent", typeof(LayoutRendererCustomUserAgent));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-user-organization", typeof(LayoutRendererCustomUserOrganization));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-user-role", typeof(LayoutRendererCustomUserRole));
            Startup.AppSettingsKey = args?.Length >= 3 ? args[2] : string.Empty;
            var host = hostBuilder.Build();
            host.Run();
        }
    }
}
