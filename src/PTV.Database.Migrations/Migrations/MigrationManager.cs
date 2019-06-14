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
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Import;
using PTV.Database.Migrations.Migrations.Base;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Framework;
using CoreExtensions = PTV.Framework.CoreExtensions;

namespace PTV.Database.Migrations.Migrations
{
    /// <summary>
    /// Responsible for calling DB migrations and related C# actions
    /// </summary>
    public static class MigrationManager
    {
        /// <summary>
        /// List of defined actions that should be called once migration applied
        /// </summary>
        private static readonly List<Action<IServiceProvider>> DataMigrationActions = new List<Action<IServiceProvider>>();
        
        /// <summary>
        /// C# conversion action that should be called
        /// </summary>
        /// <param name="migration"></param>
        /// <param name="migrationAction"></param>
        internal static void AddMigrationAction(this Migration migration, Action<IServiceProvider> migrationAction)
        {
            DataMigrationActions.Add(migrationAction);
        }
        
        /// <summary>
        /// C# conversion action that should be called
        /// </summary>
        /// <param name="migration"></param>
        /// <param name="migrationAction"></param>
        internal static void AddMigrationAction(this IPartialMigration migration, Action<IServiceProvider> migrationAction)
        {
            DataMigrationActions.Add(migrationAction);
        }
        
        private static void PerformActions(IServiceProvider serviceProvider)
        {
            DataMigrationActions.ForEach(i => i(serviceProvider));
        }

        private static void CallAndFillMigrations<T>(IServiceProvider serviceProvider) where T : DbContext
        {
            T dbContext = serviceProvider.GetService<T>();
            dbContext.Database.Migrate();
            var migrationClasses = typeof(MigrationManager).Assembly.DefinedTypes.Select(i => new {MigrationClass = i, Attribute = i.GetCustomAttribute<MigrationAttribute>()}).Where(i => i.Attribute != null)
                .Select(i => new {AppVersion = i.MigrationClass.Namespace.Split('.').Last(), Migration = i.Attribute.Id}).ToList();

            var badMigrations = migrationClasses.Where(i => i.AppVersion.Contains("Migrations"));
            if (badMigrations.Any())
            {
                throw new Exception($"Invalid migration! Fix namespaces! {string.Join(',',badMigrations.Select(i => i.Migration))}");
            }
            
            var migrationDefinitions = migrationClasses.Where(i => i.AppVersion.Contains("Release_")).ToDictionary(i => i.Migration, i => i.AppVersion);
            var currentTime = DateTime.UtcNow;
            dbContext.Set<EFMigrationsHistory>().Where(i => i.AppVersion == null || i.AppVersion == string.Empty).ToList().ForEach(m =>
            {
                if (migrationDefinitions.TryGetValue(m.MigrationId, out var row))
                {
                    m.AppVersion = row;
                    m.Applied = currentTime;
                }
            });
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Start migration process on database
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <typeparam name="T"></typeparam>
        public static void DoMigrations<T>(IServiceProvider serviceProvider) where T : DbContext
        {
            CallAndFillMigrations<T>(serviceProvider);
            PerformActions(serviceProvider);
            serviceProvider.GetService<ISeedingService>().SeedTypes();
            serviceProvider.GetService<IUserRolesImport>().UpdateUserRoles();
        }
        
        /// <summary>
        /// Start migration process on database
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void DoPtvMainMigrations(IServiceProvider serviceProvider)
        {
            DoMigrations<PtvDbContext>(serviceProvider);
        }
    }
}