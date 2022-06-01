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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using PTV.Framework.Enums;
using PTV.Framework.Middleware;

namespace PTV.Application.Web
{
    public class Startup
    {
        public static string AppSettingsKey = "";
        private bool noDbAccess;

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }
        /// <summary>
        /// PTV type of configuration
        /// </summary>
        public ApplicationConfiguration PtvConfiguration { get; set; }

        private ILogger<Startup> JsonLogger { get; set; }

        /// <summary>
        /// Startup method to initiate environment
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"config/appsettings.web.json", optional: true)
				.AddJsonFile("version.json", optional: true)
				.AddEnvironmentVariables();
            AppSettingsKey = string.Empty;
            Configuration = builder.Build();
            PtvConfiguration = new ApplicationConfiguration(Configuration);

            var awsLogSettings = new AWSLogSettings();
            PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.LogConfiguration).Bind(awsLogSettings);
            env.ConfigureNLogForEnvironment(awsLogSettings);
        }

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
            services.AddControllersWithViews()
                .AddNewtonsoftJson(i => i.SerializerSettings.Error += JsonError)
                .AddApplicationPart(typeof(TokenController).Assembly);
            services.AddSingleton(PtvConfiguration);
            services.AddSingleton(PtvConfiguration.RawConfiguration);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            
            services.AddOptions();
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
            services.AddDbContext<StsDbContext>(options =>
            {
                options.UseNpgsql(PtvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.StsConnectionString));
            });
            services.AddIdentity<StsUser, StsRole>().AddEntityFrameworkStores<StsDbContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
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
            services.Configure<AWSS3Settings>(options => PtvConfiguration.GetAwsConfiguration(AwsConfigurationEnum.S3).Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<MapDNSes>(options => Configuration.GetSection("ApplicationConfiguration:MapDNSNames").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            PtvAppInitilizer.BaseInit(services, Configuration, PtvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.ConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseCompression(); // must be used as first middleware!
            app.UseXssSecurity();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("fi-FI"))
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseStaticFiles(new StaticFileOptions()
            {
              ContentTypeProvider = provider
            });
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseRequestRewinding();
            app.UseAuthentication();

            var oldRoutes = new string[]
            {
                "login", "logout", "error", "channels", "generalDescription", "organization", "frontpage",
                "serviceCollection", "old"
            };
            
            app.UseEndpoints(endpoints =>
            {
                foreach (var oldRoute in oldRoutes)
                {
                    endpoints.MapControllerRoute(
                        name: oldRoute,
                        pattern: oldRoute + "/{*url}",
                        defaults: new {controller = "Home", action = "index"});
                }
            });
            JsonLogger = logger;
            if (!noDbAccess)
            {
                PtvAppInitilizer.InitCaches(app.ApplicationServices);
            }
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            
                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });
        }
    }
}
