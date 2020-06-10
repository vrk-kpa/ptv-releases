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
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Npgsql;
using PTV.Application.Framework;
using PTV.Database.DataAccess;
using PTV.Database.DataAccess.DataMigrations;
using PTV.Database.Migrations;
using PTV.Database.Migrations.Migrations;
using PTV.DataImport.Console.Models;
using PTV.DataImport.Console.Services;
using PTV.DataImport.Console.Tasks;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;
using PTV.LocalAuthentication;

namespace PTV.DataImport.Console
{
    public class Program
    {
        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        internal static IConfigurationRoot Configuration { get; private set; }
        /// <summary>
        /// Gets the application IServiceProvider.
        /// </summary>
        internal static IServiceProvider ServiceProvider { get; private set; }

        public static void ProcessMenuAction(ProgramMenuOption userMenuAction, ILogger logger)
        {
            System.Console.WriteLine($"Calling '{userMenuAction}'...");
            switch (userMenuAction)
            {
                case ProgramMenuOption.InitialPtvCreate:
                    throw new NotImplementedException($"Action {ProgramMenuOption.InitialPtvCreate} is no longer supported.");
                case ProgramMenuOption.CreateOrMigrateDatabase:
                    // create and apply migrations
                    System.Console.WriteLine("Applying migrations...");
                    MigrationManager.DoPtvMainMigrations(Program.ServiceProvider);
                    //FrameworksInitializer.DoMigration(Program.ServiceProvider);
                    System.Console.WriteLine("Database created.");
                    break;
                case ProgramMenuOption.MaintenanceCleanDatabaseWithUpdatedDumpTask:
                    System.Console.WriteLine("Maintenance clean database with updated dump ...");
                    var maintenanceCleanDatabaseTask = new MaintenanceCleanDatabaseTask(ServiceProvider);
                    maintenanceCleanDatabaseTask.MaintenanceCleanDatabaseWithUpdatedDump();
                    System.Console.WriteLine();
                    break;
                case ProgramMenuOption.MaintenanceCleanDatabaseWithPreviousDumpTask:
                    System.Console.WriteLine("Maintenance clean database with new dump ...");
                    var maintenanceCleanDatabaseTsk = new MaintenanceCleanDatabaseTask(ServiceProvider);
                    maintenanceCleanDatabaseTsk.MaintenanceCleanDatabaseWithPreviousDump();
                    System.Console.WriteLine();
                    break;
                case ProgramMenuOption.CreateSchemaOrMigrateTestDatabaseTask:
                    System.Console.WriteLine("Processing test database ...");
                    var migrateTestDatabaseTask = new MaintenanceCleanDatabaseTask(ServiceProvider);
                    migrateTestDatabaseTask.DeployAndUpgradeTestDatabase();
                    System.Console.WriteLine("Maintenance of test database done.");
                    System.Console.WriteLine();
                    break;
                case ProgramMenuOption.ImportHolidayTask:
                    System.Console.WriteLine("Import new holiday dates starting...");
                    var importHolidayTask = new ImportHolidayTask(ServiceProvider);
                    importHolidayTask.SeedNewHolidayDates();
                    System.Console.WriteLine("Import new holiday dates done.");
                    System.Console.WriteLine();
                    break;
                default:
                    break;
            }
        }

        public static void Main(string[] args)
        {
            ILogger logger = null;
            var userMenuAction = ProgramMenuOption.Exit;
            try
            {
                Program.BuildConfiguration(args);
                Program.BuildAndRegisterServices();

                // TODO: SFIPTV-1929 - logging does not working correctly in 3.1
                // Program.ConfigureLogging(args);

                // just logging to make a noticeable entry in a log file when running the app multiple times
                // easier to find new runs entries
                logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
                logger.LogInformation(".oOo. *** ********************************* *** .oOo.");
                logger.LogInformation(".oOo. *** PTV.DataImport.ConsoleApp started *** .oOo.");
                logger.LogInformation(".oOo. *** ********************************* *** .oOo.");

                var commandLineActions = args.Select(i => i.Trim().ParseToInt()).Where(i => i != null).Cast<int>().ToList();
                if (commandLineActions.Any())
                {
                    commandLineActions.ForEach(action =>
                    {
                        if (!Enum.IsDefined(typeof(ProgramMenuOption), action))
                        {
                            System.Console.WriteLine($"Unknown action option {action}!");
                        }
                        ProcessMenuAction((ProgramMenuOption)action, logger);
                    });
                }
                else
                {
                    do
                    {
                        userMenuAction = DisplayProgramMenu();
                        ProcessMenuAction(userMenuAction, logger);
                        //Console.Clear();

                    } while (userMenuAction != ProgramMenuOption.Exit);
                }
                System.Console.WriteLine("Exited DataImport.");
            }
            catch (Exception ex)
            {
                logger?.LogError("Something failed.", ex);

                System.Console.WriteLine(ex.ToString());
            }
            if (userMenuAction != ProgramMenuOption.Exit)
            {
                System.Console.WriteLine("Press any key...");
                System.Console.ReadKey();
            }
        }

        public static ProgramMenuOption DisplayProgramMenu()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("PTV DataImport Console. Enter the action number and press enter:");
            System.Console.WriteLine();

            var options = Enum.GetValues(typeof(ProgramMenuOption));

            foreach (var option in options)
            {
                var name = Enum.GetName(typeof(ProgramMenuOption), option);
                var value = (int)option;

                System.Console.BackgroundColor = ConsoleColor.DarkGreen;
                System.Console.ForegroundColor = ConsoleColor.DarkBlue;
                System.Console.Write("{0}. {1}", value, name);
                System.Console.ResetColor();
                System.Console.WriteLine($" - {Program.GetProgramMenuOptionDescription((ProgramMenuOption)value)}");
            }

            //System.Console.WriteLine("...OR any other key to Exit.");

            var userAction = System.Console.ReadLine()?.TrimEnd();
            Enum.TryParse(userAction, out ProgramMenuOption result);

            System.Console.WriteLine();
            return result;
        }

        private static string GetProgramMenuOptionDescription(ProgramMenuOption option)
        {
            // quick and dirty descriptions for enum values

            var desc = "No description";

            switch (option)
            {
                case ProgramMenuOption.Exit:
                    desc = "Exit the application.";
                    break;
                case ProgramMenuOption.InitialPtvCreate:
                    desc = "Use this option if you don't have PTVbase database. Creates database, seeds system data and imports data.";
                    break;
                case ProgramMenuOption.CreateOrMigrateDatabase:
                    desc = "Creates PTVbase database if it doesn't exist and applies migrations. If database exists migrations are applied.";
                    break;
                case ProgramMenuOption.MaintenanceCleanDatabaseWithUpdatedDumpTask:
                    desc = "Maintenance for TRAINING db with UPDATED dump with NEW data";
                    break;
                case ProgramMenuOption.MaintenanceCleanDatabaseWithPreviousDumpTask:
                    desc = "Maintenance for TRAINING db with PREVIOUS dump";
                    break;
                case ProgramMenuOption.ImportHolidayTask:
                    desc = "Maintenance for IMPORT new holiday dates";
                    break;
            }

            return desc;
        }

        private static string GetEnvironmentName()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        }

        /// <summary>
        /// Builds the application configuration.
        /// </summary>
        /// <param name="args">command line arguments</param>
        private static void BuildConfiguration(string[] args)
        {
            var arguments = args.Where(i => i.StartsWith("/")).ToArray();
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Custom.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("config/appsettings.console.json", optional: true);
            builder
                .AddCommandLine(arguments)
                .AddEnvironmentVariables();

            Program.Configuration = builder.Build();
        }

        /// <summary>
        /// Builds the IServiceProvider for the application and registers available services.
        /// </summary>
        private static void BuildAndRegisterServices()
        {
            var config = Program.Configuration;
            var applicationConfiguration = new ApplicationConfiguration(config);

            if (config == null)
            {
                throw new InvalidOperationException("Application Configuration is null. You must call BuildConfiguration() before calling this method.");
            }

            var connectionString = config["Data:DefaultConnection:ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not set.");
            }

            System.Console.WriteLine($"Connection string: {connectionString}");
            var csb = applicationConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.ConnectionString);
            System.Console.Write("Database=");
            System.Console.BackgroundColor = ConsoleColor.Yellow;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(csb);
            System.Console.ResetColor();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(i =>
            {
                i.SetMinimumLevel(LogLevel.Error);
                i.AddConsole();
            });

            serviceCollection.AddDbContext<StsDbContext>(options =>
            {
                options.UseNpgsql(applicationConfiguration.GetAwsConnectionString(AwsDbConnectionStringEnum.StsConnectionString));
            });
            serviceCollection.AddIdentity<StsUser, StsRole>().AddEntityFrameworkStores<StsDbContext>();

            // for now make it behave like this..
            serviceCollection.AddTransient<IHttpContextAccessor, FakeHttpContext>();
            serviceCollection.AddTransient<IWebHostEnvironment, FakeHostingEnv>(provider => new FakeHostingEnv { EnvironmentName = GetEnvironmentName()});
            serviceCollection.AddSingleton(applicationConfiguration);
            PtvAppInitilizer.BaseInit(serviceCollection, Configuration, csb);
            Program.ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureLogging(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();  // NLog: Setup NLog for Dependency injection

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();  // NLog: setup NLog for Dependency injection
    }

    public class FakeHttpContext : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }

    public class FakeHostingEnv : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; } = "Generated";
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
