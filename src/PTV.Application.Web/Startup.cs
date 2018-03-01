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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using PTV.Database.DataAccess;
using PTV.Framework;
using Webpack;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Framework.Extensions;
using PTV.Framework.Logging;

namespace PTV.Application.Web
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

    public class StartupWeb
    {
        public static string AppSettingsKey = "";

        /// <summary>
        /// Startup method to initiate environment
        /// </summary>
        /// <param name="env"></param>
        public StartupWeb(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFileEncrypted($"appsettings.{env.EnvironmentName}.crypt", AppSettingsKey, optional: true)
				.AddJsonFile("version.json", optional: true)
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

        private ILogger<StartupWeb> JsonLogger { get; set; }


        void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            JsonLogger.LogError(e.ErrorContext.Error.Message);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(i => i.SerializerSettings.Error += JsonError);
            services.AddWebpack();
            services.AddSingleton(PtvConfiguration);
            services.AddOptions();
            services.Configure<CookieAuthenticationOptions>(options => Configuration.GetSection("Data:CookieAuthenticationConfigurations").Bind(options));
            services.Configure<OpenIdConnectOptions>(options => Configuration.GetSection("Data:OpenIDConnectConfigurations").Bind(options));
            services.Configure<IdentityServerAuthenticationOptions>(options => Configuration.GetSection("Data:IdentityServerAuthenticationConfigurations").Bind(options));
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("Data:ProxyServerSettings").Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<AnnotationServiceConfiguration>(options => Configuration.GetSection("Data:AnnotationService").Bind(options));
            services.Configure<MapServiceConfiguration>(options => Configuration.GetSection("Data:MapService").Bind(options));
            services.Configure<MapDNSes>(options => Configuration.GetSection("ApplicationConfiguration:MapDNSNames").Bind(options));
            services.Configure<AccessibilityRegisterSettings>(options => Configuration.GetSection("Data:AccessibilityRegister").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterServiceManager.RegisterFromAllAssemblies(services);
            BaseEntityCloners.RegisterBaseEntityCloners(services);
            RegisterDataProviderServices.RegisterFromAssembly(services);
            FrameworksInitializer.RegisterEntityFramework(services, Configuration["Data:DefaultConnection:ConnectionString"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                // only add these in development as these have a negative impact on performance
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            loggerFactory.AddNLog();
            //app.UseRequestFilteringMiddleware("webapp");
            app.UseCompression(); // must be used as first middleware!
            app.AddNLogWeb();
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
                app.UseHttpsSwitch();
            }

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                if (!PtvConfiguration.IsWebpackDisabled())
                {
                    app.UseWebpack("webpack.config.js", "ptv.bundle.js", new WebpackDevServerOptions("localhost", 4000));
                }
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
            cookieOptions.TicketDataFormat = new TicketDataFormat(new DataProtector());
            app.UseCookieAuthentication(cookieOptions);
            openIdOptions.Authority = Configuration["STS"];

            // default Nonce cookie life time is 60 minutes and this can cause sometimes issues when there are multiple failed login attemps
            // so client will have tens of .AspNetCore.OpenIdConnect.Nonce.xxxxxxxxx named cookies and when there is enough big payload in cookies
            // client will get 431 or 400 http status response, request too long
            // setting low lifetime for nonce cookie makes it less likely to happen
            // but this has also the drawback that if you stay on the loging screen longer than 10 minutes the nonce cookie has expired and when redirected to /signin-oidc
            // it will return http status 500, but if you just reload the /-root page of app, it will successfully login automatically
            // idsvr.SignInRequest.xxxxxx named cookies can also have this same effect as those are session cookies and by default have about double the size payload
            // that cookie is configured elsewhere (Note: seems like there will be at maximum 5 idsvr.SignInRequest.xxxxxx cookies at eany time, so this will not be an issue)
            openIdOptions.ProtocolValidator = new ProtocolValidator() { RequireState = true, NonceLifetime = TimeSpan.FromMinutes(PtvConfiguration.GetOpenIdNonceLifetimeMinutes()) };

            openIdOptions.TokenValidationParameters.ValidateIssuerSigningKey = false;
            openIdOptions.AutomaticAuthenticate = true;
            openIdOptions.AutomaticChallenge = true;
            openIdOptions.GetClaimsFromUserInfoEndpoint = true;
            openIdOptions.UseTokenLifetime = true;
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
            JsonLogger = loggerFactory.CreateLogger<StartupWeb>();
            if (env.IsDevelopment())
            {
                app.ApplicationServices.GetService<IPrefilteringManager>().Initialize();
            }
        }
    }

    public class DataProtector : IDataProtector
    {
        public IDataProtector CreateProtector(string purpose)
        {
            return new DataProtector();
        }

        public byte[] Protect(byte[] plaintext)
        {
            return plaintext;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}
