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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PTV.LocalAuthentication;
using PTV.Application.Framework;
using PTV.Database.DataAccess.Interfaces;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.IdentityUserManager.Views;

namespace PTV.IdentityUserManager
{
    public class Startup
    {
        public static string EnvironmentName { get; set; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            PtvConfiguration = new ApplicationConfiguration(Configuration);
        }

        public IConfiguration Configuration { get; }
        public ApplicationConfiguration PtvConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services
                .AddMvc(options => { options.EnableEndpointRouting = false; })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new CustomViewLocationExpander());
                });
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("fi-FI"),
                        new CultureInfo("sv-SE")
                    };

                    options.DefaultRequestCulture = new RequestCulture(culture: "fi-FI", uiCulture: "fi-FI");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                }
            );


            services.AddSingleton(PtvConfiguration);
            services.AddOptions();

            services.AddDbContext<StsDbContext>(options =>
            {
                options.UseNpgsql(Configuration["Data:DefaultConnection:StsConnectionString"]);
            });
            services.AddIdentity<StsUser, StsRole>(o =>
            {
                o.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<StsDbContext>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.Configure<ProxyServerSettings>(options => Configuration.GetSection("Data:ProxyServerSettings").Bind(options));
            services.Configure<DataContextOptions>(options => Configuration.GetSection("Data:DataContextOptions").Bind(options));
            services.Configure<AnnotationServiceConfiguration>(options => Configuration.GetSection("Data:AnnotationService").Bind(options));
            services.Configure<MapServiceConfiguration>(options => Configuration.GetSection("Data:MapService").Bind(options));
            services.Configure<TestAccessUrlConfiguration>(options => Configuration.GetSection("Data:TestInternetAccess").Bind(options));
            services.Configure<RequestFilterAppSetting>(options => Configuration.GetSection("RequestFilter").Bind(options));
            services.Configure<AccessibilityRegisterSettings>(options => Configuration.GetSection("Data:AccessibilityRegister").Bind(options));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            PtvAppInitilizer.BaseInit(services, Configuration, Configuration["Data:DefaultConnection:ConnectionString"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            PtvAppInitilizer.InitCaches(app.ApplicationServices);
        }
    }
}
