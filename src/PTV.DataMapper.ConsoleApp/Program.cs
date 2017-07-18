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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using PTV.DataMapper.ConsoleApp.Services;
using PTV.DataMapper.ConsoleApp.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Application.Framework;
using PTV.Database.DataAccess;
using Microsoft.AspNetCore.Http;
using PTV.DataMapper.ConsoleApp.Services.Interfaces;

namespace PTV.DataMapper.ConsoleApp
{
    public class Program
    {
        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        internal static IConfigurationRoot Configuration { get; private set; }

        /// <summary>
        /// Gets the application IServiceProvider.
        /// </summary>
        internal static IServiceProvider ServiceProvider { get; private set; }

        public static void Main(string[] args)
        {
            ILogger logger = null;
            IDataHandler dataHandler;
            try
            {
                BuildConfiguration();
                BuildAndRegisterServices();
                ConfigureLogging();
                logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
                logger.LogDebug("Import start");
                dataHandler = ServiceProvider.GetService<IDataHandler>();
                dataHandler.ProcessAllData(ServiceProvider);
                //var reader = new DataReader();
            }
            catch(Exception ex)
            {
                if (logger != null)
                {
                    logger.LogError("Error occured.", ex.ToString());
                }

                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Builds the application configuration.
        /// </summary>
        /// <param name="args">command line arguments</param>
        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(AppContext.BaseDirectory);
            builder.AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }
        /// <summary>
        /// Builds the IServiceProvider for the application and registers available services.
        /// </summary>
        private static void BuildAndRegisterServices()
        {
            IConfigurationRoot config = Configuration;

            if (config == null)
            {
                throw new InvalidOperationException("Application Configuration is null. You must call BuildConfiguration() before calling this method.");
            }
            Console.WriteLine($"Connection string: {config["Data:ptvdb:ConnectionString"]}");

            var services = new ServiceCollection();
            services.AddLogging();

            // register ptv db context and PTV services
            RegisterServiceManager.RegisterFromAllAssemblies(services);
            FrameworksInitializer.RegisterEntityFramework(services, Configuration["Data:ptvdb:ConnectionString"]);
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddOptions();
            services.AddTransient<IDataHandler, DataHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserIdentification, UserIdentification>();
            //services.AddTransient<IOrganizationService, OrganizationService>();
            //services.AddTransient<IServiceService, ServiceService>();
            //services.AddTransient<IChannelService, ChannelService>();
            //services.AddTransient<IGeneralDescriptionService, GeneralDescriptionService>();
            services.AddSingleton(new ApplicationConfiguration(config));
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureLogging()
        {
            ILoggerFactory logFactory = ServiceProvider.GetService<ILoggerFactory>();
            logFactory.AddConsole(minLevel: LogLevel.Error);
            logFactory.AddDebug();
            logFactory.AddNLog();

            // uncomment to log to debug output window
            // just for now to see the sql warnings
            //logFactory.AddDebug(minLevel: LogLevel.Verbose);
        }
    }
}
