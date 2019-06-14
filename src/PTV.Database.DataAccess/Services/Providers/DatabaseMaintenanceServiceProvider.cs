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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Configuration;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Providers
{
    [RegisterService(typeof(DatabaseMaintenanceServiceProvider), RegisterType.Singleton)]
    public class DatabaseMaintenanceServiceProvider
    {
        private ILogger logger;

        public DatabaseMaintenanceServiceProvider(ILogger<DatabaseMaintenanceServiceProvider> logger)
        {
            this.logger = logger;
        }

        public void RemoveAndCreateDatabaseSchema(IDatabaseRawAccessor unitOfDbWork, MaintenanceDatabaseConfiguration configuration)
        {
            try
            {
                //1. REMOVE ALL ACTIVE PROCESSES
                //TODO USE after set re-join DB, otherwise DB is throw exception
                //var cleanProcessesCommand = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE pid <> pg_backend_pid() AND datname = \'{configuration.Database}\'"; //TODO USE
                //unitOfDbWork.Command(cleanProcessesCommand, new { });  

                //2. DROP, CREATE SCHEMA, ADD EXTENSION - permissions by taskScheduller setting
                unitOfDbWork.Command($"DROP SCHEMA IF EXISTS \"{configuration.DumpSchema}\" CASCADE;", new { });
                unitOfDbWork.Command($"CREATE SCHEMA \"{configuration.DumpSchema}\";", new { });
                unitOfDbWork.Command($"CREATE EXTENSION \"uuid-ossp\" SCHEMA \"{configuration.DumpSchema}\";", new { });
                unitOfDbWork.Command($"CREATE EXTENSION \"postgis\" SCHEMA \"{configuration.DumpSchema}\";", new { });
                unitOfDbWork.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //ADD to log and console
                logger.LogError($"Execute RemoveAndCreateDatabaseSchema thrown error with exception: {e.Message}");
                throw;
            }
        }

        public void BackupDatabase(MaintenanceDatabaseConfiguration conf)
        {
            var commandType = DatabaseCommandTypeEnum.pg_dump.ToString();

            var backupDumpPath = Path.Combine(Environment.CurrentDirectory, @conf.BackupDumpFolderName, conf.DumpName + ".dump"); 

            var compositeDatabaseName = $"postgresql://{conf.UserName}:{conf.Password}@{conf.Host}:{conf.Port}/{conf.Database}";
            var command = $"{commandType} --dbname {compositeDatabaseName} -n {conf.DumpSchema} -c -F c > {backupDumpPath}";

            ExecuteCommand(commandType, command, conf);
        }

        public bool RestoreDatabase(MaintenanceDatabaseConfiguration conf)
        {
            var commandType = DatabaseCommandTypeEnum.pg_restore.ToString();

            var restoreDumpPath = Path.Combine(Environment.CurrentDirectory, @conf.RestoreDumpFolderName, conf.DumpName + ".dump");

            if (File.Exists(restoreDumpPath))
            {
                Console.WriteLine("Dump database file exist");
            }
            else
            {
                throw new IOException($"File {restoreDumpPath} does not exist!");
            }
            
            var compositeDatabaseName = $"postgresql://{conf.UserName}:{conf.Password}@{conf.Host}:{conf.Port}/{conf.Database}";

            var command = $"{commandType} --dbname {compositeDatabaseName} -n {conf.DumpSchema} --verbose {restoreDumpPath}";
            
            Console.WriteLine($"Command to be executed: {command}");
            
            return ExecuteCommand(commandType, command, conf);
        }

        private bool ExecuteCommand(string commandType, string command, MaintenanceDatabaseConfiguration conf)
        {
            try
            {
                string shellCommand = null;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    shellCommand = "/bin/bash";
                    command = $"-c \"{command}\"";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    shellCommand = "cmd.exe";
                    command = $"/c \"{command}\"";
                }
                else
                {
                    logger.LogError($"Execute command: '{commandType}' isn't supported for enviroment {Environment.OSVersion.Platform}");
                }

                var watch = System.Diagnostics.Stopwatch.StartNew();
                var info = new System.Diagnostics.ProcessStartInfo(shellCommand, command)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = conf.CommandsPath
                };

                using (var proc = System.Diagnostics.Process.Start(info))
                {
                    if (proc != null)
                    {
                        proc.EnableRaisingEvents = true;
                        string errors = proc.StandardError.ReadToEnd();
                        var commandInfo = $"Execute command: '{commandType}' - info: {errors}";
                        logger.LogInformation(commandInfo);
                        Console.WriteLine(commandInfo);

                        while (!proc.HasExited)
                            System.Threading.Thread.Sleep(1000);
                        proc.WaitForExit();
                        
                        if (proc.ExitCode == 0) //Ok
                        {
                            proc.Close();
                            return true;
                        }

                        proc.Close(); 
                        return false; //Error 
                    }
                }

                watch.Stop();
                var executionTime = $"Execution time of command '{commandType}': {watch.Elapsed}. ";
                logger.LogInformation(executionTime);
                Console.WriteLine(executionTime);
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format($"Database command: '{command}' throw this error: {ex.Message}");
                logger.LogError(errorMsg + " " + ex.StackTrace);
                return false;
            }
            
            return true;
        }
    }
}
