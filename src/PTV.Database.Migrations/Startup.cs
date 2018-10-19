using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
                    sqlServerOptions => sqlServerOptions.MigrationsAssembly("PTV.Database.Migrations")
                   ));
        }

        public void Configure(IApplicationBuilder app)
        {
        }

        class UserIdentification : IUserIdentification
        {
            public string UserName { get; }
        }
    }
}