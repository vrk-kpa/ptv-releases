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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess
{
    public class Startup
    {
        /// <summary>
        /// Startup method to initiate environment
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
//            Console.WriteLine(env.ContentRootPath);
//
//            var filePath = env.ContentRootPath.Split('\\');
//            var jsonPath = string.Join("\\", filePath, 0, filePath.Length - 1);
//            Console.WriteLine(jsonPath);
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(@"appsettings.json")
                .AddJsonFile($@"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("project.json");
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(EntityNavigationsMap));
            services.AddTransient<ICacheBuilder, CacheBuilder>();
            services.AddTransient<IResolveManager, ResolveManager>();
            services.AddTransient<IUserIdentification, UserIdentification>();
            services.AddTransient<IEntityNavigationsMap, EntityNavigationsMap>();
            FrameworksInitializer.RegisterEntityFramework(services, Configuration["Data:DefaultConnection:ConnectionString"]);
        }

        /// <summary>
        /// Fake implementation for migration purposes
        /// </summary>
        public class UserIdentification : IUserIdentification
        {
            public string UserName { get; }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }
    }
}
