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
using System.Data.Common;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Services.Providers;
using PTV.Database.Migrations.Migrations;
using PTV.Domain.Model.Models.Configuration;

namespace PTV.DataImport.Console.Tasks
{
    public class MaintenanceCleanDatabaseTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly IConfigurationRoot applicationConfiguration;

        public MaintenanceCleanDatabaseTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            applicationConfiguration = Program.Configuration;
            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<MaintenanceCleanDatabaseTask>();
            logger.LogDebug("MaintenanceCleanDatabaseTask .ctor");
        }

        public void MaintenanceCleanDatabaseWithUpdatedDump()
        {
            var configuration = GetUpdatedDatabaseConfiguration();
            MaintenanceCleanDatabaseData(configuration);
        }

        public void MaintenanceCleanDatabaseWithPreviousDump()
        {
            var configuration = GetPreviousDatabaseConfiguration();
            MaintenanceCleanDatabaseData(configuration);
        }

        public void DeployAndUpgradeTestDatabase()
        {
            DeployOcpDatabaseData(GetOcpDeploymentDatabaseConfiguration());
        }

        private void MaintenanceCleanDatabaseData(MaintenanceDatabaseConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IDatabaseRawContext>();
                var databaseMaintenanceServiceProvider = serviceScope.ServiceProvider.GetService<DatabaseMaintenanceServiceProvider>();

                scopedCtxMgr.ExecuteWriter(unitOfDbWork =>
                {
                    LogInfoData(logger, "Remove and create public schema is starting...");
                    databaseMaintenanceServiceProvider.RemoveAndCreateDatabaseSchema(unitOfDbWork, configuration);
                    LogInfoData(logger, "Remove and create public schema is done.");
                });

                LogInfoData(logger, "Restore DB is starting...");
                if (databaseMaintenanceServiceProvider.RestoreDatabase(configuration))
                {
                    LogInfoData(logger, "Restore DB is done.");
                }
                else
                {
                    LogInfoData(logger, "Restore DB FAILED with errors.");
                    return;
                }

                LogInfoData(logger, "Migration DB is starting...");
                MigrationManager.DoPtvMainMigrations(serviceProvider);
                LogInfoData(logger, "Migration DB is done.");

                LogInfoData(logger, "Importing test accounts...");
                var importTestAccountsTask = new ImportTestAccountsTask(serviceProvider);
                importTestAccountsTask.ImportAndUpdateUsers();
                LogInfoData(logger, "Importing of test accounts done.");

                LogInfoData(logger, "Backup DB is starting...");
                databaseMaintenanceServiceProvider.BackupDatabase(configuration);
                LogInfoData(logger, "Backup DB is done.");
            }
        }

        private void DeployOcpDatabaseData(MaintenanceDatabaseConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IDatabaseRawContext>();
                var databaseMaintenanceServiceProvider = serviceScope.ServiceProvider.GetService<DatabaseMaintenanceServiceProvider>();

                var dbExists = false;
                try
                {
                    dbExists = scopedCtxMgr.ExecuteReader(i => i.SelectOne<int>(@"SELECT 1 FROM ""Language"" LIMIT 1;", null)) == 1;
                }
                catch
                {
                    // ignored
                }

                if (!dbExists)
                {
                    LogInfoData(logger, "Restoring new DB...");
                    scopedCtxMgr.ExecuteWriter(unitOfDbWork =>
                    {
                        databaseMaintenanceServiceProvider.RemoveAndCreateDatabaseSchema(unitOfDbWork, configuration);
                    });
                    databaseMaintenanceServiceProvider.RestoreDatabase(configuration);
                    LogInfoData(logger, "New DB created.");
                }
                LogInfoData(logger, "Migration DB is starting...");
                MigrationManager.DoPtvMainMigrations(serviceProvider);
                LogInfoData(logger, "Migration DB is done.");
                LogInfoData(logger, "Importing test accounts...");
                if (!dbExists)
                {
                    var importTestAccountsTask = new ImportTestAccountsTask(serviceProvider);
                    importTestAccountsTask.ImportAndUpdateUsers();
                    LogInfoData(logger, "Importing of test accounts done.");
                }
            }
        }


        private MaintenanceDatabaseConfiguration GetPreviousDatabaseConfiguration()
        {
            return GetDatabaseConfiguration("PreviousData");
        }

        private MaintenanceDatabaseConfiguration GetUpdatedDatabaseConfiguration()
        {
            return GetDatabaseConfiguration("UpdatedOrNewData");
        }

        private MaintenanceDatabaseConfiguration GetOcpDeploymentDatabaseConfiguration()
        {
            return GetDatabaseConfiguration("OcpDeployment");
        }

        private MaintenanceDatabaseConfiguration GetDatabaseConfiguration(string section)
        {
            var configuration = GetBaseConfiguration();

            if (configuration != null)
            {
                applicationConfiguration?.GetSection(configuration.ConfigurationName +":"+section).Bind(configuration);

                if (!ValidateConfiguration(configuration))
                    return null;
            }

            return configuration;
        }


        private MaintenanceDatabaseConfiguration GetBaseConfiguration()
        {
            var configuration = new MaintenanceDatabaseConfiguration();

            if (!applicationConfiguration?.GetSection(configuration.ConfigurationName).Exists() ?? false)
            {
                return null;  //section is no set
            }

            applicationConfiguration?.GetSection(configuration.ConfigurationName).Bind(configuration);

            applicationConfiguration?.GetSection(configuration.ConfigurationName +":DumpsUpdate").Bind(configuration);

            var connectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = applicationConfiguration?[ConfigKeys.PTVConnectionString] ??
                                   throw new NullReferenceException(nameof(applicationConfiguration))
            };

            configuration.UserName = connectionStringBuilder.ContainsKey("Username") ? connectionStringBuilder["Username"] as string : null;
            configuration.Password = connectionStringBuilder.ContainsKey("Password") ? connectionStringBuilder["Password"] as string : null;
            configuration.Host = connectionStringBuilder.ContainsKey("Host") ? connectionStringBuilder["Host"] as string : null;
            configuration.Database = connectionStringBuilder.ContainsKey("Database") ? connectionStringBuilder["Database"] as string : null;
            configuration.Port = connectionStringBuilder.ContainsKey("Port") ? connectionStringBuilder["Port"] as string : "5432";

            return configuration;
        }

        private bool ValidateConfiguration(MaintenanceDatabaseConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.UserName) ||
                string.IsNullOrEmpty(configuration.Password) ||
                string.IsNullOrEmpty(configuration.Host) ||
                string.IsNullOrEmpty(configuration.Database) ||
                string.IsNullOrEmpty(configuration.Port) ||
                string.IsNullOrEmpty(configuration.DumpName) ||
                string.IsNullOrEmpty(configuration.RestoreDumpFolderName) ||
                string.IsNullOrEmpty(configuration.BackupDumpFolderName) ||
                string.IsNullOrEmpty(configuration.DumpSchema) ||
                string.IsNullOrEmpty(configuration.CommandsPath)
            )
            {
                logger.LogError("Some of input params aren't set.");
                return false;
            }

            //Check existing path
            if (Directory.Exists(configuration.CommandsPath))
            {
                LogInfoData(logger, $"Commands pg_restore and pg_dump are set in path: {configuration.CommandsPath}");
            }
            else
            {
                logger.LogError($"The path of DB commands(pg_restore, pg_dump) location does not exist, check wrong path:{configuration.CommandsPath}");
                return false;
            }
            
            var restoreDumpFilePath = Path.Combine(Environment.CurrentDirectory, configuration.RestoreDumpFolderName, configuration.DumpName + ".dump");
            if (!File.Exists(restoreDumpFilePath))
            {
                logger.LogError($"Restoring file {configuration.DumpName} does not exist in path {restoreDumpFilePath}");
                return false;
            }
            
            var backupDumpFolderPath = Path.Combine(Environment.CurrentDirectory, configuration.BackupDumpFolderName);
            if (!Directory.Exists(backupDumpFolderPath))
            {
                logger.LogError($"Restoring file {configuration.BackupDumpFolderName} does not exist in path {backupDumpFolderPath}");
                return false;
            }

            return true;
        }

        private void LogInfoData(ILogger customLogger, string logInformation)
        {
            if (customLogger == null)
            {
                return;
            }

            customLogger.LogInformation(logInformation);
            System.Console.WriteLine(logInformation);
        }
    }
}
