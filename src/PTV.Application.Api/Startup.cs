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
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using PTV.Database.DataAccess;
using PTV.Framework;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Localization;
using PTV.Application.Framework;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Framework.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using NLog.Config;
using PTV.Framework.Logging;

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
    /// Startup claass called by .net core
    /// </summary>
    public class StartupApi
    {
        /// <summary>
        /// 
        /// </summary>
        public static string AppSettingsKey = "";
        const string ProxyForwardedProtocolHeader = "X-Forwarded-Proto";
        private readonly IHostingEnvironment appEnv;
        /// <summary>
        /// Startup method to initiate environment
        /// </summary>
        /// <param name="env"></param>
        public StartupApi(IHostingEnvironment env)
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("custom-aspnet-request-ip", typeof(AspNetRequestIpLayoutRenderer));
            appEnv = env;
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("version.json", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFileEncrypted($"appsettings.{env.EnvironmentName}.crypt", AppSettingsKey, optional: true)
                .AddEnvironmentVariables();
            AppSettingsKey = string.Empty;
            Configuration = builder.Build();
            PtvConfiguration = new ApplicationConfiguration(Configuration);

            env.ConfigureNLogForEnvironment();
        }

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }
        /// <summary>
        /// PTV type of configuration
        /// </summary>
        public ApplicationConfiguration PtvConfiguration { get; set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">object responsible for registering of the assemblies</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMvcCore().AddJsonFormatters();
            //services.AddMvcCore();
            //services.AddAuthorization();
            services.AddSingleton(PtvConfiguration);
            services.AddOptions();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ui", new Info() {Title = "PTV UI API", Version = "ui", Description = "PTV UI Web API", });
                var developmentPath = string.Format(Path.Combine(appEnv.ContentRootPath, "bin", "Debug", "netcoreapp1.1"));

                var xmlCommentPath = Path.Combine(appEnv.ContentRootPath, @"PTV.Application.Api.xml");
                var modelsXmlPath = Path.Combine(appEnv.ContentRootPath, @"PTV.Domain.Model.xml");

                c.DescribeAllEnumsAsStrings();

                if (!File.Exists(xmlCommentPath) || !File.Exists(modelsXmlPath))
                {
                    xmlCommentPath = Path.Combine(developmentPath, @"PTV.Application.Api.xml");
                    modelsXmlPath = Path.Combine(developmentPath, @"PTV.Domain.Model.xml");
                }

                if (!File.Exists(xmlCommentPath) || !File.Exists(modelsXmlPath)) return;

                c.IncludeXmlComments(xmlCommentPath);
                c.IncludeXmlComments(modelsXmlPath);
            });
            //services.AddCors();
            services.Configure<CookieAuthenticationOptions>(options => Configuration.GetSection("Data:CookieAuthenticationConfigurations").Bind(options));
            services.Configure<OpenIdConnectOptions>(options => Configuration.GetSection("Data:OpenIDConnectConfigurations").Bind(options));
            //services.Configure<OAuthOptions>(options => Configuration.GetSection("Data:OpenIDConnectConfigurations").Bind(options));
            services.Configure<IdentityServerAuthenticationOptions>(options => Configuration.GetSection("Data:IdentityServerAuthenticationConfigurations").Bind(options));
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("Data:ProxyServerSettings").Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<AnnotationServiceConfiguration>(options => Configuration.GetSection("Data:AnnotationService").Bind(options));
            services.Configure<MapServiceConfiguration>(options => Configuration.GetSection("Data:MapService").Bind(options));
            services.Configure<TestAccessUrlConfiguration>(options => Configuration.GetSection("Data:TestInternetAccess").Bind(options));
            services.Configure<MapDNSes>(options => Configuration.GetSection("ApplicationConfiguration:MapDNSNames").Bind(options));
            services.Configure<RequestFilterAppSetting>(options => Configuration.GetSection("RequestFilter").Bind(options));
            services.Configure<AccessibilityRegisterSettings>(options => Configuration.GetSection("Data:AccessibilityRegister").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterServiceManager.RegisterFromAllAssemblies(services);
            BaseEntityCloners.RegisterBaseEntityCloners(services);
            RegisterDataProviderServices.RegisterFromAssembly(services);
            // Add framework services.
            FrameworksInitializer.RegisterEntityFramework(services, Configuration["Data:DefaultConnection:ConnectionString"]);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">library responsible for configuring of app</param>
        /// <param name="env">environment of application running in</param>
        /// <param name="loggerFactory">library responsible for building logs</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                // only add these in development as these have a negative impact on performance
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUi(c =>
            {
                c.RoutePrefix = "swagger/ui";
                c.SwaggerEndpoint("/swagger/ui/swagger.json", "PTV UI API");
            });
            app.UseRequestFiltering("uiapi");
            app.UseCompression();
            app.UseXssSecurity();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("fi-FI"))
            });

            var switchRequestsToHttpsString = Configuration["Data:SwitchRequestsToHttps"];
            bool switchRequestsToHttps = false;
            bool.TryParse(switchRequestsToHttpsString, out switchRequestsToHttps);
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

            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                // TODO: do automatic migrations and seed only in development
                // this might later change when we have separate seeding for the needed values for the app
                // and some demo data we want to load for development but not for production
                //FrameworksInitializer.DoMigration(app.ApplicationServices);
                //FrameworksInitializer.SeedDatabase(app.ApplicationServices);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var cookieOptions = app.ApplicationServices.GetRequiredService<IOptions<CookieAuthenticationOptions>>().Value;
            var openIdOptions = app.ApplicationServices.GetRequiredService<IOptions<OpenIdConnectOptions>>().Value;
            var identityServerOptions = app.ApplicationServices.GetRequiredService<IOptions<IdentityServerAuthenticationOptions>>().Value;
            app.UseCookieAuthentication(cookieOptions);
            openIdOptions.Authority = Configuration["STS"];
            openIdOptions.ProtocolValidator = new ProtocolValidator() { RequireState = true };
            openIdOptions.TokenValidationParameters.ValidateIssuerSigningKey = false;
            app.UseOpenIdConnectAuthentication(openIdOptions);
            identityServerOptions.Authority = Configuration["STS"];
            identityServerOptions.AutomaticAuthenticate = true;
            identityServerOptions.AutomaticChallenge = true;
            identityServerOptions.SaveToken = true;
            identityServerOptions.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Jwt;
            identityServerOptions.AdditionalScopes = new List<string>() { "email", "role", "openid", "dataEventRecords" };
            app.UseIdentityServerAuthentication(identityServerOptions);
            app.UseMvc(routes =>
                        {
                            routes.MapRoute(
                                name: "default",
                                template: "{*url}",
                                defaults: new {controller = "Home", action = "index"});

                        });
            app.ApplicationServices.GetService<IPrefilteringManager>().Initialize();
        }
    }
}
