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
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Introspection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using PTV.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using PTV.Application.Framework;
using PTV.Framework.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using PTV.Framework.Logging;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication.Cookies;
using PTV.Database.DataAccess.Interfaces;
using PTV.SignalR.Hubs;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog.AWS.Logger;
using PTV.Framework.Enums;

namespace PTV.Application.Api
{
    class ProtocolValidator : OpenIdConnectProtocolValidator
    {
        protected override void ValidateState(OpenIdConnectProtocolValidationContext validationContext)
        {
        }
        protected override void ValidateAtHash(OpenIdConnectProtocolValidationContext validationContext)
        {

        }
    }

    /// <summary>
    /// Startup class called by .net core
    /// </summary>
    public class StartupApi
    {
        /// <summary>
        ///
        /// </summary>
        public static string AppSettingsKey = "";
        const string ProxyForwardedProtocolHeader = "X-Forwarded-Proto";
        private readonly IWebHostEnvironment appEnv;
        /// <summary>
        /// Startup method to initiate environment
        /// </summary>
        /// <param name="env"></param>
        public StartupApi(IWebHostEnvironment env)
        {
            appEnv = env;
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("version.json", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"config/appsettings.uiapi.json", optional: true)
                //.AddJsonFileEncrypted($"appsettings.{env.EnvironmentName}.crypt", AppSettingsKey, optional: true)
                .AddEnvironmentVariables();
            AppSettingsKey = string.Empty;
            Configuration = builder.Build();
            PtvConfiguration = new ApplicationConfiguration(Configuration);

            var awsLogSettings = new AWSLogSettings();
            PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.LogConfiguration).Bind(awsLogSettings);
            env.ConfigureNLogForEnvironment(awsLogSettings);
        }

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }
        /// <summary>
        /// PTV type of configuration
        /// </summary>
        public ApplicationConfiguration PtvConfiguration { get; set; }


        private ILogger<StartupApi> JsonLogger { get; set; }
        void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            var data = e.ErrorContext.OriginalObject?.ToString();
            JsonLogger.LogError($"{e.ErrorContext.Error.Message}{Environment.NewLine}{data ?? string.Empty}");
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">object responsible for registering of the assemblies</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson(i => i.SerializerSettings.Error += JsonError);
            services.AddSingleton(PtvConfiguration);
            services.AddOptions();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
            var tokenUrl = Configuration["ApplicationConfiguration:TokenServiceUrl"];
            Console.WriteLine(tokenUrl);

            var allowedCorsOrigins = Configuration.GetSection("AllowedCorsOrigins")
                .AsEnumerable()
                .Where(x => x.Value != null)
                .Select(x => x.Value)
                .ToArray();
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.WithOrigins(allowedCorsOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials().Build();
                }));

	            services.AddAuthentication(options =>
            	{
        	        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    	            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	            })
                .AddOAuthIntrospection(options =>
                {
                    options.Events = new OAuthIntrospectionEvents
                    {
                        OnRetrieveToken = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/massToolHub")))
                            {
                                context.Request.Query = new QueryCollection();
                                // Read the token out of the query string
                                // context.Token = accessToken;
                                context.Request.Headers.Add("Authorization", "Bearer " + accessToken);
                            }

                            return Task.CompletedTask;
                        }
                    };

                    options.CachingPolicy = null;
                    options.SaveToken = true;
                    options.Authority = new Uri(tokenUrl);
                    options.ClientId = "PtvApp";
                    options.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";
                    options.RequireHttpsMetadata = false;
                    options.NameClaimType = PahaClaims.UserName;
                });

            services.AddSignalR().AddNewtonsoftJsonProtocol();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ui", new OpenApiInfo {Title = "PTV UI API", Version = "ui", Description = "PTV UI Web API", });
                var developmentPath = string.Format(Path.Combine(appEnv.ContentRootPath, "bin", "Debug", "netcoreapp2.0"));

                var xmlCommentPath = Path.Combine(appEnv.ContentRootPath, @"PTV.Application.Api.xml");
                var modelsXmlPath = Path.Combine(appEnv.ContentRootPath, @"PTV.Domain.Model.xml");

                if (!File.Exists(xmlCommentPath) || !File.Exists(modelsXmlPath))
                {
                    xmlCommentPath = Path.Combine(developmentPath, @"PTV.Application.Api.xml");
                    modelsXmlPath = Path.Combine(developmentPath, @"PTV.Domain.Model.xml");
                }

                if (!File.Exists(xmlCommentPath) || !File.Exists(modelsXmlPath)) return;

                c.IncludeXmlComments(xmlCommentPath);
                c.IncludeXmlComments(modelsXmlPath);
            });
            services.AddSingleton(new OAuthSettings { Authority = tokenUrl });
            //services.AddCors();
            services.Configure<AWSS3Settings>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.S3).Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<AnnotationServiceConfiguration>(options => Configuration.GetSection("Data:AnnotationService").Bind(options));
            services.Configure<MapServiceConfiguration>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.MapService).Bind(options));
            services.Configure<TestAccessUrlConfiguration>(options => Configuration.GetSection("Data:TestInternetAccess").Bind(options));
            services.Configure<RequestFilterAppSetting>(options => Configuration.GetSection("RequestFilter").Bind(options));
            services.Configure<AccessibilityRegisterSettings>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.AccessibilityRegister).Bind(options));
            services.Configure<MassToolConfiguration>(options => Configuration.GetSection("MassToolConfiguration").Bind(options));
            services.Configure<QualityAgentConfiguration>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.QualityAgentService).Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<CachePeriodicRefresher, CachePeriodicRefresher>();
            PtvAppInitilizer.BaseInit(services, Configuration, PtvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.ConnectionString));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">library responsible for configuring of app</param>
        /// <param name="env">environment of application running in</param>
        /// <param name="logger">library responsible for building logs</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<StartupApi> logger)
        {
//            if (env.IsDevelopment() || env.EnvironmentName == "dev" || env.EnvironmentName == "test")
//            {
//                // only add these in development as these have a negative impact on performance
//                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
//                loggerFactory.AddDebug();
//            }
            JsonLogger = logger;
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSwagger();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger/ui";
                c.SwaggerEndpoint("/swagger/ui/swagger.json", "PTV UI API");
            });
            app.UseRequestFiltering("uiapi");
            app.UsePerformanceMonitoring();
            app.UseCompression();
            app.UseXssSecurity();
//            app.UseRequestLocalization(new RequestLocalizationOptions
//            {
//                DefaultRequestCulture = new RequestCulture(new CultureInfo("fi-FI"))
//            });

            var switchRequestsToHttpsString = Configuration["Data:SwitchRequestsToHttps"];
            bool.TryParse(switchRequestsToHttpsString, out var switchRequestsToHttps);
            if (switchRequestsToHttps)
            {
                Console.WriteLine("Incoming requests switched to HTTPS");
                app.Use(async (context, next) =>
                {
                    var prefProto = string.Empty;
                    if (context.Request.Headers.ContainsKey(ProxyForwardedProtocolHeader))
                    {
                        prefProto = context.Request.Headers[ProxyForwardedProtocolHeader];
                        context.Request.Headers[ProxyForwardedProtocolHeader] = "https";
                    }
                    context.Request.IsHttps = true;
                    await next();
                    if (!string.IsNullOrEmpty(prefProto))
                    {
                        context.Request.Headers[ProxyForwardedProtocolHeader] = prefProto;
                    }
                });
            }

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MassToolHub>("/massToolHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{*url}",
                    defaults: new {controller = "Home", action = "index"});
            });
            PtvAppInitilizer.InitCaches(app.ApplicationServices);
        }
    }
}
