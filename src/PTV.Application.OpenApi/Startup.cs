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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess;
using PTV.Framework;
using PTV.Framework.Extensions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PTV.Application.Framework;
using Swashbuckle.AspNetCore.Swagger;
using PTV.Application.OpenApi.Swagger;
using PTV.Database.DataAccess.Interfaces;
using PTV.Framework.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace PTV.Application.OpenApi
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
    /// Initilizer of app
    /// </summary>
    public class StartupOpenApi
    {
        private readonly IHostingEnvironment appEnv;

        /// <summary>
        /// Constructor of initializer
        /// </summary>
        /// <param name="env">IHostingEnvironment</param>
        /// <param name="configuration">IConfiguration</param>
        public StartupOpenApi(IHostingEnvironment env, IConfiguration configuration)
        {
            appEnv = env;
            Configuration = configuration;
            PtvConfiguration = new ApplicationConfiguration(Configuration);
        }

        /// <summary>
        /// Loaded configuration of application
        /// </summary>
        public IConfiguration Configuration { get; private set; }

        /// <summary>
        /// PTV type of configuration
        /// </summary>
        public ApplicationConfiguration PtvConfiguration { get; private set; }

        /// <summary>
        /// Configure and register all services needed for application
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
            var tokenUrl = Configuration["ApplicationConfiguration:TokenServiceUrl"];
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ClientId = "";
                    options.ClientSecret = "";
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.ResponseType = OpenIdConnectResponseType.IdTokenToken;
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.Authority = tokenUrl;
                    options.Scope.Add("email");
                    options.Scope.Add("roles");
                    options.Scope.Add("profile");
                    options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                    options.TokenValidationParameters.RequireSignedTokens = false;
                    options.SecurityTokenValidator = new JwtSecurityTokenHandler
                    {
                        // Disable the built-in JWT claims mapping feature.
                        InboundClaimTypeMap = new Dictionary<string, string>(),
                    };

                    options.TokenValidationParameters.NameClaimType = PahaClaims.UserName;
                })
                .AddOAuthIntrospection(options =>
                {
                    options.CachingPolicy = null;
                    options.SaveToken = true;
                    options.Authority = new Uri(tokenUrl);
                    options.ClientId = "";
                    options.ClientSecret = "";
                    options.RequireHttpsMetadata = false;
                    options.NameClaimType = PahaClaims.UserName;
                });
            
            // Add framework services.
            services.AddMvc(c =>
                {
                    c.AllowEmptyInputInBodyModelBinding = true;
                    c.Conventions.Add(new ControllerModelRemoveVersionPrefixConvention());
                }
            );
            services.AddMvcCore(o => o.AllowEmptyInputInBodyModelBinding = true).AddJsonFormatters();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IConfigurationRoot>(Configuration);
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
            services.AddSingleton(PtvConfiguration);
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("Data:ProxyServerSettings").Bind(options));
            services.Configure<PerformanceMeasuringSettings>(options => Configuration.GetSection("Data:PerformanceMeasuringSettings").Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<AnnotationServiceConfiguration>(options => Configuration.GetSection("Data:AnnotationService").Bind(options));
            services.Configure<MapServiceConfiguration>(options => Configuration.GetSection("Data:MapService").Bind(options));
            services.Configure<TestAccessUrlConfiguration>(options => Configuration.GetSection("Data:TestInternetAccess").Bind(options));
            services.Configure<MapDNSes>(options => Configuration.GetSection("ApplicationConfiguration:MapDNSNames").Bind(options));
            services.Configure<RequestFilterAppSetting>(options => Configuration.GetSection("RequestFilter").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            PtvAppInitilizer.BaseInit(services, Configuration, Configuration["Data:DefaultConnection:ConnectionString"]);

            services.AddSwaggerGen(
                c => {
                    c.SwaggerDoc("v6", new Info
                    {
                        Version = "v6",
                        Title = "PTV Open Api version 6",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });
                    c.SwaggerDoc("v7", new Info
                    {
                        Version = "v7",
                        Title = "PTV Open Api version 7",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });
                    c.SwaggerDoc("v8", new Info
                    {
                        Version = "v8",
                        Title = "PTV Open Api version 8",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });
                    c.SwaggerDoc("v9", new Info
                    {
                        Version = "v9",
                        Title = "PTV Open Api version 9",
                        Description = "Here you can see listed all the PTV Open Api methods."
                    });

                    c.OperationFilter<SecurityRequirementsOperationFilter>(false);
                    c.DocInclusionPredicate((version, apiDescription) => ResolveVersionSupportByRouteConstraint(apiDescription, version));
                    c.SchemaFilter<SwaggerSchemaFilter>();

                    // TODO: PTV-1556:
                    //c.DocumentFilter<SwaggerDocumentFilter>();

                    // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)    
                    // Let's add authorization scheme according to environment
                    if (appEnv.IsDevelopment() || appEnv.EnvironmentName == "dev" || appEnv.EnvironmentName == "test" ||appEnv.EnvironmentName == "trn")
                    {
                        c.AddSecurityDefinition("oauth2", new ApiKeyScheme // The security definition name has to be "oauth2" to make the swagger authetication to work!
                        {
                            Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                            In = "header",
                            Name = "Authorization",
                            Type = "apiKey"
                        });
                    }
                    else
                    {
                        c.AddSecurityDefinition("oauth2", new OAuth2Scheme // The security definition name has to be "oauth2" to make the swagger authetication to work!
                        {
                            Type = "oauth2",
                            Flow = "implicit",
                            AuthorizationUrl = $"{tokenUrl}/connect/authorize",
                            Scopes = new Dictionary<string, string>
                        {
                            { "dataEventRecords", "Access event records" }
                        }
                        });
                    }
                    
                });

            var path = appEnv.IsDevelopment() ? Path.Combine(appEnv.ContentRootPath, "bin", "Debug", @"netcoreapp2.1") : appEnv.ContentRootPath;
            var xmlCommentPath = Path.Combine(path, @"PTV.Application.OpenApi.xml");
            var modelsXmlPath = Path.Combine(path, @"PTV.Domain.Model.xml");
            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.IncludeXmlComments(xmlCommentPath);
                if (File.Exists(modelsXmlPath))
                {
                    options.IncludeXmlComments(modelsXmlPath);
                }
            });
        }

        private static bool ResolveVersionSupportByRouteConstraint(ApiDescription apiDesc, string targetApiVersion)
        {
            var path = apiDesc.RelativePath;
            if (targetApiVersion == "v1")
            {
                return !path.Contains("api/v");
            }

            return path.Contains(targetApiVersion);
        }

        /// <summary>
        /// Configure additional runtime pipeline middlewares
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment() || env.EnvironmentName == "dev" || env.EnvironmentName == "test")
            {
                // only add these in development as these have a negative impact on performance
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            
            var switchRequestsToHttpsString = Configuration["Data:SwitchRequestsToHttps"];
            bool switchRequestsToHttps = false;
            bool.TryParse(switchRequestsToHttpsString, out switchRequestsToHttps);
            if (switchRequestsToHttps)
            {
                app.UseHttpsSwitch();
            }
            app.UseRequestFiltering("openapi");
            app.UsePerformanceMonitoring();
            //app.UseCompression(); // lower bandwidth but higher cpu usage
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseMvc();
            app.UseSwagger();

            string swaggerEnvTitle = null;

            try
            {
                // get environment
                EnvironmentTypeEnum envType = PtvConfiguration.GetEnvironmentType();

                if (envType != 0)
                {
                    // the environment is defined in the config
                    swaggerEnvTitle = $" - {envType.ToString().ToUpper()}";
                }
            }
            catch {}

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v9/swagger.json", "Version 9");
                c.SwaggerEndpoint("/swagger/v8/swagger.json", "Version 8");
                c.SwaggerEndpoint("/swagger/v7/swagger.json", "Version 7");
                c.SwaggerEndpoint("/swagger/v6/swagger.json", "Version 6");

                c.EnableValidator(null);
                c.DocExpansion(DocExpansion.None);
                c.RoutePrefix = "swagger/ui";
                c.DocumentTitle = $"PTV Open Api Documentation{swaggerEnvTitle}";

                //c.ConfigureOAuth2(openIdOptions.ClientId, openIdOptions.ClientSecret, "swagger-ui-realm", openIdOptions.ClientId);
            });
            PtvAppInitilizer.InitCaches(app.ApplicationServices);
        }
    }
}
