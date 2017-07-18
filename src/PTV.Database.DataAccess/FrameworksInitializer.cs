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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using Microsoft.EntityFrameworkCore;

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// Startup initialization procedures for Entity Framework
    /// </summary>
    public static class FrameworksInitializer
    {
        // postgressql error codes: http://www.postgresql.org/docs/9.4/static/errcodes-appendix.html

        /// <summary>
        /// PostgreSQL invalid catalog name error code (database doesn't exist).
        /// </summary>
        public const int NpgsqlInvalidCatalogName = 0x3D000;

        /// <summary>
        /// Performs first startup registration and initialization of Entity Framework and context
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        public static void RegisterEntityFramework(IServiceCollection services, string connectionString)
        {
            services.AddEntityFrameworkNpgsql().AddDbContext<PtvDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            services.AddDbContext<PtvDbContext>(options => options.UseNpgsql(connectionString));
        }

        /// <summary>
        /// Applies all database migrations.
        /// </summary>
        /// <param name="serviceProvider">Microsoft.AspNetCore.Builder.IServiceProvider</param>
        /// <remarks>You should call this method from Application Startup.Configure method. Call this method before you call the seeding methods.</remarks>
        /// <exception cref="System.ArgumentNullException"><i>app</i> is a null reference</exception>
        public static void DoMigration(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            CoreExtensions.RunInThreadAndWait(() =>
            {
                // do EF migrations in own scope - see https://github.com/aspnet/EntityFramework/issues/3070
                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    PtvDbContext dbContext = serviceScope.ServiceProvider.GetService<PtvDbContext>();
                    // run migrations automatically
                    dbContext.Database.Migrate();
                    var seedingService = serviceScope.ServiceProvider.GetService<ISeedingService>();
                    seedingService.SeedTypes();
                }
            });
        }

        /// <summary>
        /// Seeds the database.
        /// </summary>
        /// <param name="serviceProvider">Microsoft.AspNetCore.Builder.IServiceProvider</param>
        /// <remarks>You should call this method from Application Startup.Configure method. Call this method after you have called <see cref="PTV.Database.DataAccess.FrameworksInitializer.DoMigration(IApplicationBuilder)"/></remarks>
        /// <exception cref="System.ArgumentNullException"><i>app</i> is a null reference</exception>
        public static void SeedDatabase(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            CoreExtensions.RunInThreadAndWait(() =>
            {
                // seed data in own scope - see https://github.com/aspnet/EntityFramework/issues/3070
                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    //PtvDbContext dbContext = serviceScope.ServiceProvider.GetService<PtvDbContext>();

                    try
                    {
                        // TODO : decide if we remove this try/catch BUT first read below the reason why it was added
                        // the call to dbContext.LanguageNames.Any() will throw an exception if the database hasn't been created
                        // we will handle the exception if it is about the database doesn't exist
                        // try/catch was added here because when creating the initial migration the database doesn't exist and the dnx ef command executes the code
                        // when the db doesn't exist exception is thrown and the migration creation fails
                        // we could remove this as the initial migration is now done and this shouldn't affect any other cases

                        // just a quick test to seed or not
//                        if (!dbContext.Countries.Any())
//                        {
                            // seed data
                            var seedingService = serviceScope.ServiceProvider.GetService<ISeedingService>();
                            seedingService.SeedTypes();
                            seedingService.SeedDatabaseEnums();
                            //seedingService.SeedInitialData();
//                        }
                    }
                    catch (Npgsql.NpgsqlException e)
                    {
                        if (e.HResult == FrameworksInitializer.NpgsqlInvalidCatalogName)
                        {
                            // supress the exception as this is most likely now called by the migrations
                            // TODO : log warn message
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            });
        }
    }
}
