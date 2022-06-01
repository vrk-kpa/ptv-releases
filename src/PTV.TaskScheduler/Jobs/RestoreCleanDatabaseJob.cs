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
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Services.Providers;
using PTV.Domain.Model;
using PTV.LocalAuthentication;
using PTV.TaskScheduler.Configuration;
using Quartz;
using System.IO;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Migrations.Migrations;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class RestoreCleanDatabaseJob : BaseJob, IJob
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var configuration = GetRestoreCleanDatabaseConfiguration(serviceProvider, jobSummary);
            if (configuration == null)
            {
                return null;
            }

            var databaseRawContext = serviceProvider.GetService<IDatabaseRawContext>();
            var databaseMaintenanceServiceProvider = serviceProvider.GetService<DatabaseMaintenanceServiceProvider>();

            databaseRawContext.ExecuteWriter(unitOfDbWork =>
            {
                LogInfoData(jobSummary, "Remove and create public schema is starting...");
                databaseMaintenanceServiceProvider.RemoveAndCreateDatabaseSchema(unitOfDbWork, configuration);
                LogInfoData(jobSummary, "Remove and create public schema is done...");
            });

            LogInfoData(jobSummary, "Restore DB is starting...");
            if (databaseMaintenanceServiceProvider.RestoreDatabase(configuration))
            {
                LogInfoData(jobSummary, "Restore DB is done.");
            }
            else
            {
                LogInfoData(jobSummary, "Restore DB FAILED with errors.");
                return null;
            }

            LogInfoData(jobSummary, "Migration DB is starting...");
            MigrationManager.DoPtvMainMigrations(serviceProvider);
            LogInfoData(jobSummary, "Migration DB is done.");

            LogInfoData(jobSummary, "Backup DB is starting...");
            databaseMaintenanceServiceProvider.BackupDatabase(configuration);
            LogInfoData(jobSummary, "Backup DB is done.");

            LogInfoData(jobSummary, "Importing test accounts...");
            ImportAndUpdateUsers(serviceProvider, jobSummary);
            LogInfoData(jobSummary, "Importing of test accounts done.");

            return null;
        }

        private RestoreCleanDatabaseConfiguration GetRestoreCleanDatabaseConfiguration(IServiceProvider serviceProvider, VmJobSummary jobSummary)
        {
            var configuration = new RestoreCleanDatabaseConfiguration();
            
            var applicationConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();
            if (!applicationConfiguration?.GetSection(configuration.ConfigurationName).Exists() ?? false)
            {
                return null;  //section is only for TRN database
            }
            applicationConfiguration?.GetSection(configuration.ConfigurationName).Bind(configuration);
            applicationConfiguration?.GetSection(configuration.ConfigurationName +":PreviousData").Bind(configuration);
            
            var ptvConfiguration = new ApplicationConfiguration(applicationConfiguration); 
            
            var connectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = ptvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.QuartzConnection)
            };
            configuration.UserName = connectionStringBuilder.ContainsKey("Username") ? connectionStringBuilder["Username"] as string : null;
            configuration.Password = connectionStringBuilder.ContainsKey("Password") ? connectionStringBuilder["Password"] as string : null;
            configuration.Host = connectionStringBuilder.ContainsKey("Host") ? connectionStringBuilder["Host"] as string : null;
            configuration.Database = connectionStringBuilder.ContainsKey("Database") ? connectionStringBuilder["Database"] as string : null;
            configuration.Port = connectionStringBuilder.ContainsKey("Port") ? connectionStringBuilder["Port"] as string : null;

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
                logger.Error("Some of input params aren't set.");
                return null;
            }

            //Check exisiting path
            if (Directory.Exists(configuration.CommandsPath))
            {
                LogInfoData(jobSummary, $"Commands pg_restore and pg_dump are set in path: {configuration.CommandsPath}");
            }
            else
            {
                logger.Error($"The path of DB commands(pg_restore, pg_dump) location does not exist, check wrong path:{configuration.CommandsPath}");
                return null;
            }

            var restoreDumpFilePath = Path.Combine(Environment.CurrentDirectory, @configuration.RestoreDumpFolderName, configuration.DumpName + ".dump");
            if (!File.Exists(restoreDumpFilePath))
            {
                logger.Error($"Restoring file {configuration.DumpName} does not exist in path {restoreDumpFilePath}");
                return null;
            }

            var backupDumpFolderPath = Path.Combine(Environment.CurrentDirectory, @configuration.BackupDumpFolderName);
            if (!Directory.Exists(backupDumpFolderPath))
            {
                logger.Error($"Restoring file {configuration.BackupDumpFolderName} does not exist in path {backupDumpFolderPath}");
                return null;
            }

            return configuration;
        }

        private void ImportAndUpdateUsers(IServiceProvider serviceProvider, VmJobSummary jobSummary)
        {
            var usersList = JsonConvert.DeserializeObject<List<StsJsonUser>>(File.ReadAllText("TestTrnUsers.json") ?? string.Empty);
            if (!usersList.Any())
            {
                LogInfoData(jobSummary,"No users found in TestTrnUsers.json file.");
                return;
            }
            LogInfoData(jobSummary,$"{usersList.Count} users loaded from file.");
            var stsUserManager = serviceProvider.GetRequiredService<IStsPtvUserManager>();
            var notImported = stsUserManager.ImportUserJsonList(usersList);
            if (notImported.NoSavedUsers.Any())
            {
                LogInfoData(jobSummary,$"Not imported users because of error in definition:{Environment.NewLine}{string.Join(Environment.NewLine,notImported.NoSavedUsers)}");
            }
            if (notImported.NoSavedMappings.Any())
            {
                LogInfoData(jobSummary,$"Not imported users because of error in mappings, role or organization not found:{Environment.NewLine}{string.Join(Environment.NewLine,notImported.NoSavedMappings.Select(i => $"UserId:{i.Item1}/OrganizationId:{i.Item2}"))}");
            }
        }

        private void LogInfoData(VmJobSummary jobSummary, string logInformation)
        {
            if (logger == null)
            {
                return;
            }
            logger.Info(logInformation);
            TaskSchedulerLogger.LogJobInfo(jobSummary, logInformation, JobStatusService);
            if (Debugger.IsAttached)
            {
                Console.WriteLine(logInformation);
            }
        }
    }
}
