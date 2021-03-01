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
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.Migrations
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(EntityNavigationsMap));
            services.AddTransient<ICacheBuilder, CacheBuilder>();
            services.AddTransient<IResolveManager, ResolveManager>();
            services.AddTransient<IUserIdentification, UserIdentification>();
            services.AddTransient<IEntityNavigationsMap, EntityNavigationsMap>();
            services.AddDbContext<PtvDbContext>(
                options => options.UseNpgsql(
                    Configuration["Data:DefaultConnection:ConnectionString"],
                    sqlServerOptions => sqlServerOptions
                        .MigrationsAssembly("PTV.Database.Migrations")
                        .UseNetTopologySuite()
                   ));
        }

        public void Configure(IApplicationBuilder app)
        {

        }

        class UserIdentification : IUserIdentification
        {
            public string UserName { get; }
            public Guid UserOrganization { get; }
            public int UserRole { get; }
        }
    }
}
