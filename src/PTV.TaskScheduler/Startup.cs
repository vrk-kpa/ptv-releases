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
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.LayoutRenderers;
using NLog.Web;
using PTV.Application.Framework;
using PTV.Database.DataAccess;
using PTV.Database.DataAccess.Interfaces;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;
using PTV.Framework.Logging;
using PTV.LocalAuthentication;
using PTV.Localization.Services.Model;
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
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"config/appsettings.scheduler.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            PtvConfiguration = new ApplicationConfiguration(Configuration) {BindingUrl = BindingUrl};

            var jobConfigurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("jobsettings.json", optional: false, reloadOnChange: true);
            JobConfiguration = jobConfigurationBuilder.Build();

            // NLog renderers
            LayoutRenderer.Register("json-message", logEvent => $"\"{logEvent.Message}\"");
            LayoutRenderer.Register("job-type", logEvent => logEvent.Properties.ContainsKey("JobType") ? logEvent.Properties["JobType"] : null);
            LayoutRenderer.Register("job-status", logEvent => logEvent.Properties.ContainsKey("JobStatus") ? logEvent.Properties["JobStatus"] : null);
            LayoutRenderer.Register("execution-type", logEvent => logEvent.Properties.ContainsKey("ExecutionType") ? logEvent.Properties["ExecutionType"] : null);
            LayoutRenderer.Register("operation-id", logEvent => logEvent.Properties.ContainsKey("OperationId") ? logEvent.Properties["OperationId"] : null);

            var awsLogSettings = new AWSLogSettings();
            PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.LogConfiguration).Bind(awsLogSettings);
            env.ConfigureNLogForEnvironment(awsLogSettings);
            //if (env.IsDevelopment()) TaskSchedulerLogger.ClearAllLogs();
        }

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        private IConfigurationRoot Configuration { get; }

        /// <summary>
        /// PTV type of configuration
        /// </summary>
        private ApplicationConfiguration PtvConfiguration { get; set; }

        private IConfiguration JobConfiguration { get; set; }

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
            services.AddSingleton(JobConfiguration);
            RegisterServiceManager.RegisterFromAllAssemblies(services, Configuration);
            RegisterDataProviderServices.RegisterFromAssembly(services);
            FrameworksInitializer.RegisterEntityFramework(services, PtvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.QuartzConnection));
            BaseEntityCloners.RegisterBaseEntityCloners(services);

            services.AddDbContext<StsDbContext>(options =>
            {
                options.UseNpgsql(PtvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.StsConnectionString));
            });
            services.AddIdentity<StsUser, StsRole>().AddEntityFrameworkStores<StsDbContext>();

            services.Configure<RelayingTranslationOrderConfiguration>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.RelayingTranslationOrderConfiguration).Bind(options));
            services.Configure<AWSS3Settings>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.S3).Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("DataContextOptions").Bind(options));
            services.Configure<TransifexConfiguration>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.TransifexLocalizationConfiguration).Bind(options));
            // Add framework services.
            services.AddMvc(options => { options.EnableEndpointRouting = false; }).AddNewtonsoftJson(options =>
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
        /// <param name="lifetime"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
//            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
//            loggerFactory.AddDebug();

            var serviceProvider = app.ApplicationServices;
            QuartzScheduler.Initialize(serviceProvider);

            lifetime.ApplicationStarted.Register(QuartzScheduler.Start);
            lifetime.ApplicationStopping.Register(QuartzScheduler.Stop);

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
