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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Services.Providers;
using PTV.Database.Migrations.Migrations;
using PTV.DataImport.ConsoleApp.Models;
using PTV.Domain.Model.Models.Configuration;
using PTV.Framework;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class MaintenanceCleanDatabaseTask
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _applicationConfiguration;

        public MaintenanceCleanDatabaseTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _applicationConfiguration = Program.Configuration;
            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<MaintenanceCleanDatabaseTask>();
            _logger.LogDebug("MaintenanceCleanDatabaseTask .ctor");
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

        private void MaintenanceCleanDatabaseData(MaintenanceDatabaseConfiguration configuration)
        {
            if (configuration == null)
            {
                return;
            }

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IDatabaseRawContext>();
                var databaseMaintenanceServiceProvider = serviceScope.ServiceProvider.GetService<DatabaseMaintenanceServiceProvider>();
                
                scopedCtxMgr.ExecuteWriter(unitOfDbWork =>
                {
                    LogInfoData(_logger, "Remove and create public schema is starting...");
                    databaseMaintenanceServiceProvider.RemoveAndCreateDatabaseSchema(unitOfDbWork, configuration);
                    LogInfoData(_logger, "Remove and create public schema is done.");
                });

                LogInfoData(_logger, "Restore DB is starting...");
                if (databaseMaintenanceServiceProvider.RestoreDatabase(configuration))
                {
                    LogInfoData(_logger, "Restore DB is done.");
                }
                else
                {
                    LogInfoData(_logger, "Restore DB FAILED with errors.");
                    return;
                }

                LogInfoData(_logger, "Migration DB is starting...");
                MigrationManager.DoPtvMainMigrations(_serviceProvider);
                LogInfoData(_logger, "Migration DB is done.");

                LogInfoData(_logger, "Importing test accounts...");
                var importTestAccountsTask = new ImportTestAccountsTask(_serviceProvider);
                importTestAccountsTask.ImportAndUpdateUsers();
                LogInfoData(_logger, "Importing of test accounts done.");

                LogInfoData(_logger, "Backup DB is starting...");
                databaseMaintenanceServiceProvider.BackupDatabase(configuration);
                LogInfoData(_logger, "Backup DB is done.");
            }
        }
        
        
        private MaintenanceDatabaseConfiguration GetPreviousDatabaseConfiguration()
        {
            var configuration = GetBaseConfiguration();

            if (configuration != null)
            {
                _applicationConfiguration?.GetSection(configuration.ConfigurationName +":PreviousData").Bind(configuration);
                
                if (!ValidateConfiguration(configuration))
                    return null;
            }

            return configuration;
        }

        private MaintenanceDatabaseConfiguration GetUpdatedDatabaseConfiguration()
        {
            var configuration = GetBaseConfiguration();

            if (configuration != null)
            {
                _applicationConfiguration?.GetSection(configuration.ConfigurationName +":UpdatedOrNewData").Bind(configuration);
                
                if (!ValidateConfiguration(configuration))
                    return null;
            }

            return configuration;
        }
        
        private MaintenanceDatabaseConfiguration GetBaseConfiguration()
        {
            var configuration = new MaintenanceDatabaseConfiguration();

            if (!_applicationConfiguration?.GetSection(configuration.ConfigurationName).Exists() ?? false)
            {
                return null;  //section is no set 
            }

            _applicationConfiguration?.GetSection(configuration.ConfigurationName).Bind(configuration);
            
            _applicationConfiguration?.GetSection(configuration.ConfigurationName +":DumpsUpdate").Bind(configuration);
            
            var connectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = _applicationConfiguration[ConfigKeys.PTVConnectionString]
            };

            configuration.UserName = connectionStringBuilder.ContainsKey("Username") ? connectionStringBuilder["Username"] as string : null;
            configuration.Password = connectionStringBuilder.ContainsKey("Password") ? connectionStringBuilder["Password"] as string : null;
            configuration.Host = connectionStringBuilder.ContainsKey("Host") ? connectionStringBuilder["Host"] as string : null;
            configuration.Database = connectionStringBuilder.ContainsKey("Database") ? connectionStringBuilder["Database"] as string : null;
            configuration.Port = connectionStringBuilder.ContainsKey("Port") ? connectionStringBuilder["Port"] as string : null;

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
                _logger.LogError("Some of input params aren't set.");
                return false;
            }
            
            //Check exisiting path
            if (Directory.Exists(configuration.CommandsPath))
            {
                LogInfoData(_logger, $"Commands pg_restore and pg_dump are set in path: {configuration.CommandsPath}");
            }
            else
            {
                _logger.LogError($"The path of DB commands(pg_restore, pg_dump) location does not exist, check wrong path:{configuration.CommandsPath}");
                return false;
            }
            
            var restoreDumpFilePath = Path.Combine(Environment.CurrentDirectory, @configuration.RestoreDumpFolderName, configuration.DumpName + ".dump"); 
            if (!File.Exists(restoreDumpFilePath))
            {
                _logger.LogError($"Restoring file {configuration.DumpName} does not exist in path {restoreDumpFilePath}");
                return false;
            }
            
            var backupDumpFolderPath = Path.Combine(Environment.CurrentDirectory, @configuration.BackupDumpFolderName); 
            if (!Directory.Exists(backupDumpFolderPath))
            {
                _logger.LogError($"Restoring file {configuration.BackupDumpFolderName} does not exist in path {backupDumpFolderPath}");
                return false;
            }

            return true;
        }

        private void LogInfoData(ILogger logger, string logInformation)
        {
            if (logger == null)
            {
                return;
            }

            logger.LogInformation(logInformation);
            Console.WriteLine(logInformation);
        }
    }
}
