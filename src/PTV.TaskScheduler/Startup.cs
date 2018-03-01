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
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Extensions.Logging;
using NLog.LayoutRenderers;
using NLog.Web;
using PTV.Database.DataAccess;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.TaskScheduler.Configuration;

namespace PTV.TaskScheduler
{
    /// <summary>
    /// Initilizer of app
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// AppSettings key 
        /// </summary>
        public static string AppSettingsKey = "";
        public static string BindingUrl = "http://localhost:5000";
        /// <summary>
        /// Constructor of initializer
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFileEncrypted($"appsettings.{env.EnvironmentName}.crypt", AppSettingsKey, optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            PtvConfiguration = new ApplicationConfiguration(Configuration) {BindingUrl = BindingUrl};
            // NLog renderers
            LayoutRenderer.Register("json-message", logEvent => $"\"{logEvent.Message}\"");
            LayoutRenderer.Register("job-type", logEvent => logEvent.Properties.ContainsKey("JobType") ? logEvent.Properties["JobType"] : null);
            LayoutRenderer.Register("job-status", logEvent => logEvent.Properties.ContainsKey("JobStatus") ? logEvent.Properties["JobStatus"] : null);
            LayoutRenderer.Register("execution-type", logEvent => logEvent.Properties.ContainsKey("ExecutionType") ? logEvent.Properties["ExecutionType"] : null);
            LayoutRenderer.Register("operation-id", logEvent => logEvent.Properties.ContainsKey("OperationId") ? logEvent.Properties["OperationId"] : null);

            var environmentLogConfigurationName = $"nlog.{env.EnvironmentName}.config";
            env.ConfigureNLog(File.Exists(environmentLogConfigurationName) ? environmentLogConfigurationName : "nlog.config");
            //if (env.IsDevelopment()) TaskSchedulerLogger.ClearAllLogs();
        }

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// PTV type of configuration
        /// </summary>
       public ApplicationConfiguration PtvConfiguration { get; set; }

        /// <summary>
        /// Configure and register all services needed for application
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(PtvConfiguration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);
            RegisterServiceManager.RegisterFromAllAssemblies(services);
            RegisterDataProviderServices.RegisterFromAssembly(services);
            FrameworksInitializer.RegisterEntityFramework(services, Configuration.GetConnectionString("QuartzConnection"));
            BaseEntityCloners.RegisterBaseEntityCloners(services);
            services.Configure<RelayingTranslationOrderConfiguration>(options => Configuration.GetSection("RelayingTranslationOrderConfiguration").Bind(options));
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("ProxyServerSettings").Bind(options));
            // Add framework services.
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.Culture = CultureInfo.InvariantCulture;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        /// <summary>
        /// Configure additional runtime pipeline middlewares
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="lifetime"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();

            var serviceProvider = app.ApplicationServices as IServiceProvider;
            QuartzScheduler.Initialize(serviceProvider);

            lifetime.ApplicationStarted.Register(QuartzScheduler.Start);
            lifetime.ApplicationStopping.Register(QuartzScheduler.Stop);

            app.UseMvc();
        }
    }
}
