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
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.TaskScheduler.Configuration;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.Cloud;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class DatabaseBackupJob : BaseJob
    {
        protected override string CallExecute(IJobExecutionContext context, 
            IServiceProvider serviceProvider, 
            IContextManager contextManager, 
            VmJobSummary jobSummary)
        {
            var stopwatch = Stopwatch.StartNew();
            var jobData = QuartzScheduler.GetJobDataConfiguration<DatabaseBackupConfiguration>(context.JobDetail);
            var applicationConfiguration = serviceProvider.GetRequiredService<IConfigurationRoot>();
            var ptvConfiguration = new ApplicationConfiguration(applicationConfiguration);
            var connectionString = ptvConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.QuartzConnection);
            if (connectionString.IsNullOrEmpty())
            {
                TaskSchedulerLogger.LogJobError(jobSummary, $"Connection string for {JobKey} not found.", JobStatusService);
                return null;
            }

            // parse connection string
            var connectionStringBuilder = new DbConnectionStringBuilder {ConnectionString =  connectionString};
            var userName = connectionStringBuilder.ContainsKey("Username") ? connectionStringBuilder["Username"] as string : null;
            var password = connectionStringBuilder.ContainsKey("Password") ? connectionStringBuilder["Password"] as string : null;
            var host = connectionStringBuilder.ContainsKey("Host") ? connectionStringBuilder["Host"] as string : null;
            var port = connectionStringBuilder.ContainsKey("Port") ? connectionStringBuilder["Port"] as string : null;
            var database = connectionStringBuilder.ContainsKey("Database") ? connectionStringBuilder["Database"] as string : null;
            
            // prepare output file name
            var tempPath = Path.GetTempPath();
            var backupFileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm}.dump";
            var backupFilePath = Path.Combine(tempPath, backupFileName);
            
            // prepare arguments
            var dbName = string.Format(jobData.DatabaseConnection, userName, password, host, port, database);
            var arguments = string.Format(jobData.Arguments, dbName, backupFilePath);
            
            // create backup
            stopwatch.Start();
            string errors = null;
            using var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    FileName = jobData.Command,
                    Arguments = arguments
                }
            };
            process.ErrorDataReceived += (sender, e) => errors += e.Data;
            process.Start();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                TaskSchedulerLogger.LogJobError(jobSummary, $"pg_dump command failed: {errors}", JobStatusService);
                return null;
            }
            stopwatch.Stop();
            var backupElapsed = stopwatch.Elapsed;
            
            // ensure that backup file exists
            if (!File.Exists(backupFilePath))
            {
                TaskSchedulerLogger.LogJobError(jobSummary, $"Backup file not found: {backupFilePath}", JobStatusService);
                return null;
            }
            
            // copy file to s3 storage
            stopwatch.Reset();
            stopwatch.Start();
            var storage = serviceProvider.GetService<IStorageService>();
            using var client = storage.GetClient();
            try
            {
                client.UploadFile(jobData.Folder, backupFileName, backupFilePath);
            }
            catch (Exception ex)
            {
                TaskSchedulerLogger.LogJobError(jobSummary, $"Exception during uploading of '{backupFilePath}': {ex}", JobStatusService);
                return null;
            }
            stopwatch.Stop();
            var uploadElapsed = stopwatch.Elapsed;
            
            // delete backup from temp
            File.Delete(backupFilePath);
            
            // show job info
            TaskSchedulerLogger.LogJobInfo(jobSummary, $"Backup '{backupFileName}' was created in '{backupElapsed}'; uploaded to storage in '{uploadElapsed}'; total job time: '{backupElapsed+uploadElapsed}'", JobStatusService);
            return null;
        }
    }
}
