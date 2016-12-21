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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Localization;
using PTV.Framework.Extensions;

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

    public class Startup
    {
        const string ProxyForwardedProtocolHeader = "X-Forwarded-Proto";

        /// <summary>
        /// Startup method to initiate environment
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("project.json", optional: true);

            // Read your project.json file
            //var project = new ConfigurationBuilder().AddJsonFile("project.json").Build();
            // Pull the version from it
            //var version = project["version"]; // yields "1.0.0-*" or something similar...
//
//            if (env.IsDevelopment())
//            {
//                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709user.
//                builder.AddUserSecrets();
//            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            PtvConfiguration = new ApplicationConfiguration(Configuration);
        }

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }
        /// <summary>
        /// PTV type of configuration
        /// </summary>
        public ApplicationConfiguration PtvConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMvcCore().AddJsonFormatters();
            //services.AddMvcCore();
            //services.AddAuthorization();
            services.AddSingleton(PtvConfiguration);
            services.AddOptions();
            //services.AddCors();
            services.Configure<CookieAuthenticationOptions>(options => Configuration.GetSection("Data:CookieAuthenticationConfigurations").Bind(options));
            services.Configure<OpenIdConnectOptions>(options => Configuration.GetSection("Data:OpenIDConnectConfigurations").Bind(options));
            //services.Configure<OAuthOptions>(options => Configuration.GetSection("Data:OpenIDConnectConfigurations").Bind(options));
            services.Configure<IdentityServerAuthenticationOptions>(options => Configuration.GetSection("Data:IdentityServerAuthenticationConfigurations").Bind(options));
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("Data:ProxyServerSettings").Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterServiceManager.RegisterFromAllAssemblies(services);
            // Add framework services.
            FrameworksInitializer.RegisterEntityFramework(services, Configuration["Data:DefaultConnection:ConnectionString"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();
            app.UseCompression();
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
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

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

            app.UseStaticFiles();
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
        }
    }
}
