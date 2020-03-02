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
using System.IdentityModel.Tokens.Jwt;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Webpack;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using PTV.Application.Framework;
using PTV.Database.DataAccess.Interfaces;
using PTV.Framework.Extensions;
using PTV.Framework.Logging;
using PTV.Framework;
using PTV.LocalAuthentication;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;

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

        public override void ValidateTokenResponse(OpenIdConnectProtocolValidationContext validationContext)
        {

        }

        protected override void ValidateCHash(OpenIdConnectProtocolValidationContext validationContext)
        {

        }

        protected override void ValidateIdToken(OpenIdConnectProtocolValidationContext validationContext)
        {

        }

        public override void ValidateAuthenticationResponse(OpenIdConnectProtocolValidationContext validationContext)
        {

        }
    }

    public class StartupWeb
    {
        public static string AppSettingsKey = "";
        private bool noDbAccess;

        /// <summary>
        /// Startup method to initiate environment
        /// </summary>
        /// <param name="env"></param>
        public StartupWeb(IWebHostEnvironment env)
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
            var usePahaAuth = Configuration["ApplicationConfiguration:UsePAHAAuthentication"].Is("true");
            var tokenUrl = usePahaAuth ? CoreExtensions.CombineUris(Configuration["ApplicationConfiguration:PAHARedirectUrl"],"/api/auth") : Configuration["ApplicationConfiguration:TokenServiceUrl"];
            services.AddSignalR();
            services.AddMvc().AddNewtonsoftJson(i => i.SerializerSettings.Error += JsonError)
                .AddApplicationPart(typeof(TokenController).Assembly);
            services.AddWebpack();
            services.AddSingleton(PtvConfiguration);
            services.AddSingleton(PtvConfiguration.RawConfiguration);
            services.AddOptions();
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
            services.AddDbContext<StsDbContext>(options =>
            {
                options.UseNpgsql(Configuration["Data:DefaultConnection:StsConnectionString"]);
            });
            services.AddIdentity<StsUser, StsRole>().AddEntityFrameworkStores<StsDbContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
//                .AddOpenIdConnect(options =>
//                {
//                    options.ClientId = "PtvApp";
//                    options.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";
//                    options.RequireHttpsMetadata = false;
//                    options.GetClaimsFromUserInfoEndpoint = true;
//                    options.SaveTokens = true;
//
//                    options.ResponseType = OpenIdConnectResponseType.IdTokenToken;
//                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
//                    options.Authority = tokenUrl;
//                    options.Scope.Add("email");
//                    options.Scope.Add("roles");
//                    options.Scope.Add("profile");
//                    options.TokenValidationParameters.ValidateIssuerSigningKey = false;
//                    options.TokenValidationParameters.RequireSignedTokens = false;
//                    options.SecurityTokenValidator = new JwtSecurityTokenHandler
//                    {
//                        // Disable the built-in JWT claims mapping feature.
//                        InboundClaimTypeMap = new Dictionary<string, string>(),
//                    };
//
//                    options.TokenValidationParameters.NameClaimType = PahaClaims.UserName;
//                })
                .AddOAuthIntrospection(options =>
                {
                    options.CachingPolicy = null;
                    options.SaveToken = true;
                    options.Authority = new Uri(tokenUrl);
                    //options.Audiences.Add("PtvApp");
                    options.ClientId = "PtvApp";
                    options.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";
                    options.RequireHttpsMetadata = false;
                    options.NameClaimType = PahaClaims.UserName;
                    // options.RoleClaimType = "custom_role_claim";
                });
            services.AddSingleton(new OAuthSettings { Authority = tokenUrl });
            noDbAccess = Configuration["Data:DefaultConnection:NoDbAccess"]?.ToLower() == "true";
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("Data:ProxyServerSettings").Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<MapDNSes>(options => Configuration.GetSection("ApplicationConfiguration:MapDNSNames").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            PtvAppInitilizer.BaseInit(services, Configuration, Configuration["Data:DefaultConnection:ConnectionString"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<StartupWeb> logger)
        {
//            if (env.IsDevelopment() || env.EnvironmentName == "dev" || env.EnvironmentName == "test")
//            {
//                // only add these in development as these have a negative impact on performance
//                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
//                loggerFactory.AddDebug();
//            }

            app.UseCompression(); // must be used as first middleware!
            app.UseXssSecurity();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("fi-FI"))
            });

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseStaticFiles(new StaticFileOptions()
            {
              ContentTypeProvider = provider
            });
            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{*url}",
                    defaults: new {controller = "Home", action = "index"});
            });
            JsonLogger = logger;
            if (!noDbAccess)
            {
                PtvAppInitilizer.InitCaches(app.ApplicationServices);
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
