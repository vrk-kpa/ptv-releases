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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTV.DataImport.ConsoleApp.DataAccess;
using PTV.DataImport.ConsoleApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PTV.Framework;
using PTV.Database.DataAccess;
using PTV.DataImport.ConsoleApp.Models;
using PTV.DataImport.ConsoleApp.Tasks;
using NLog.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PTV.Framework.Extensions;

namespace PTV.DataImport.ConsoleApp
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

        public static void Main(string[] args)
        {
            ILogger logger = null;

            try
            {
                Program.BuildConfiguration(args);
                Program.BuildAndRegisterServices();
                Program.ConfigureLogging();

                // just logging to make a noticable entry in a log file when running the app multiple times
                // easier to find new runs entries
                logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
                logger.LogInformation(".oOo. *** ********************************* *** .oOo.");
                logger.LogInformation(".oOo. *** PTV.DataImport.ConsoleApp started *** .oOo.");
                logger.LogInformation(".oOo. *** ********************************* *** .oOo.");

                var userMenuAction = ProgramMenuOption.Exit;

                do
                {
                    userMenuAction = DisplayProgramMenu();
                    //Console.Clear();
                    switch (userMenuAction)
                    {
                        case ProgramMenuOption.InitialPtvCreate:
                            Stopwatch sw = new Stopwatch();
                            Stopwatch swTotal = new Stopwatch();
                            swTotal.Start();

                            // create and apply migrations
                            Console.WriteLine("Creating new database..");
                            sw.Start();
                            FrameworksInitializer.DoMigration(Program.ServiceProvider);
                            sw.Stop();
                            string msg = $"Database created in {sw.Elapsed}.";
                            Console.WriteLine(msg);
                            logger.LogInformation(msg);
                            Console.WriteLine();

                            // seed system data
                            Console.WriteLine("Seeding system data to database..");
                            sw.Restart();
                            FrameworksInitializer.SeedDatabase(Program.ServiceProvider);
                            sw.Stop();
                            msg = $"System data seeded to database in {sw.Elapsed}.";
                            Console.WriteLine(msg);
                            logger.LogInformation(msg);
                            Console.WriteLine();

                            // Create organizations for municipalities
                            Console.WriteLine("Creating organizations for municipalities..");
                            sw.Restart();
                            CreateMunicipalityOrganizationsTask munOrgTask = new CreateMunicipalityOrganizationsTask(Program.ServiceProvider);
                            munOrgTask.Create();
                            sw.Stop();
                            Console.WriteLine();
                            msg = $"Organizations for municipalities created in {sw.Elapsed}.";
                            Console.WriteLine(msg);
                            logger.LogInformation(msg);
                            Console.WriteLine();

                            // import general descriptions
                            Console.WriteLine("Importing general descriptions from JSON file..");
                            sw.Restart();
                            CreateGeneralDescriptionsJsonTask genDescTask = new CreateGeneralDescriptionsJsonTask(Program.ServiceProvider);
                            genDescTask.ImportDataFromJSON();
                            sw.Stop();
                            Console.WriteLine();
                            msg = $"General descriptions imported in {sw.Elapsed}.";
                            Console.WriteLine(msg);
                            logger.LogInformation(msg);
                            Console.WriteLine();

                            // import fake ptv data from json
                            Console.WriteLine("Starting fake PTV import..");
                            sw.Restart();
                            ImportTask fakeit = new ImportTask(Program.ServiceProvider);
                            fakeit.ImportFakePtv();
                            sw.Stop();
                            msg = $"Fake PTV import complete in {sw.Elapsed}.";
                            Console.WriteLine(msg);
                            logger.LogInformation(msg);
                            Console.WriteLine();

                            swTotal.Stop();
                            msg = $"Create, seed and import data, total time: {swTotal.Elapsed}";
                            Console.WriteLine(msg);
                            logger.LogInformation(msg);
                            Console.WriteLine();

                            Console.WriteLine("See log files in /logs subfolder for details.");
                            break;
                        case ProgramMenuOption.CreateOrMigrateDatabase:
                            // create and apply migrations
                            Console.WriteLine("Creating new database..");
                            FrameworksInitializer.DoMigration(Program.ServiceProvider);
                            Console.WriteLine("Database created.");
                            break;
                        case ProgramMenuOption.SeedSystemData:
                            // seed system data
                            Console.WriteLine("Seeding system data to database..");
                            FrameworksInitializer.SeedDatabase(Program.ServiceProvider);
                            Console.WriteLine("System data seeded to database.");
                            // import general descriptions
                            Console.WriteLine("Importing general descriptions from JSON file..");
                            sw = new Stopwatch();
                            sw.Restart();
                            UpdateCreateGeneralDescriptionsTask updateDescTask = new UpdateCreateGeneralDescriptionsTask(Program.ServiceProvider);
                            updateDescTask.ImportDataFromJSON();
                            sw.Stop();

                            Console.WriteLine();
                            msg = $"General descriptions imported in {sw.Elapsed}.";
                            Console.WriteLine(msg);
                            logger.LogInformation(msg);
                            Console.WriteLine();
                            break;
                        case ProgramMenuOption.CreateMunicipalityOrganizations:
                            Console.WriteLine("Creating Organizations for Municipalities.");
                            CreateMunicipalityOrganizationsTask municipalityOrganizationsTask = new CreateMunicipalityOrganizationsTask(Program.ServiceProvider);
                            municipalityOrganizationsTask.Create();
                            break;
                        //case ProgramMenuOption.ImportGeneralDescriptions:
                        //    Console.WriteLine("Generating general descriptions JSON...");
                        //    CreateGeneralDescriptionsJsonTask generalDescriptionTask = new CreateGeneralDescriptionsJsonTask(Program.ServiceProvider);
                        //    generalDescriptionTask.Generate();
                        //    Console.WriteLine("General descriptions JSON generation complete.");
                        //    break;
                        //case ProgramMenuOption.ImportFakePtv:
                        //    Console.WriteLine("Starting fake PTV import..");
                        //    ImportTask it = new ImportTask(Program.ServiceProvider);
                        //    it.ImportFakePtv();
                        //    Console.WriteLine("Fake PTV import complete.");
                        //    break;
                        case ProgramMenuOption.DumpFakePtv:
                            Console.WriteLine("Starting to dump fake PTV to JSON files..");
                            DumpFakePtvToFilesTask task = new DumpFakePtvToFilesTask(Program.ServiceProvider);
                            task.WriteToFiles();
                            Console.WriteLine("Fake PTV dumped to JSON files.");
                            break;
                        case ProgramMenuOption.ValidateFakePtvJson:
                            Console.WriteLine("Starting to validate fake PTV generated JSON files to model..");

                            try
                            {
                                DumpFakePtvToFilesTask validationTask = new DumpFakePtvToFilesTask(Program.ServiceProvider);
                                validationTask.ValidateJson();
                                Console.WriteLine("Validation success.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());

                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.Write("Error:");
                                Console.ResetColor();
                                Console.WriteLine($" check the SourceXXX model, {ex.Message}");
                            }
                            break;
                        case ProgramMenuOption.CreatePostalCodesJson:
                            Console.WriteLine("Generating postal codes JSON..");
                            CreatePostalCodesJsonTask pct = new CreatePostalCodesJsonTask(Program.ServiceProvider);
                            pct.Generate();
                            Console.WriteLine("Postal codes JSON generation complete.");
                            break;

                        case ProgramMenuOption.ImportOrganizations:
                            Console.WriteLine("Importing organizations JSON..");
                            CreateOrganizationsJsonTask organizationTask = new CreateOrganizationsJsonTask(Program.ServiceProvider);
                            organizationTask.ImportDataFromJSON();
                            Console.WriteLine("Importing organizations complete.");
                            break;
                        case ProgramMenuOption.UpdateCoordinatesForAddress:
                            Console.WriteLine("Updating addresses..");
                            var addressTask = new UpdateCoordinatesForAddressesTask(Program.ServiceProvider);
                            addressTask.UpdateAddresses();
                            Console.WriteLine("Updating addresses complete.");
                            break;
                        case ProgramMenuOption.SwitchCoordinates:
                            Console.WriteLine("Switching coordinates..");
                            var addressSwitchTask = new UpdateCoordinatesForAddressesTask(Program.ServiceProvider);
                            addressSwitchTask.SwitchCoordinates();
                            Console.WriteLine("Switching coordinates complete.");
                            break;
                        default:
                            break;
                    }
                } while (userMenuAction != ProgramMenuOption.Exit);

                Console.WriteLine("Exited DataImport.");
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogError("Something failed.", ex);
                }

                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }

        public static ProgramMenuOption DisplayProgramMenu()
        {
            Console.WriteLine();
            Console.WriteLine("PTV DataImport Console. Enter the action number and press enter:");
            Console.WriteLine();

            var options = Enum.GetValues(typeof(ProgramMenuOption));

            foreach (var option in options)
            {
                var name = Enum.GetName(typeof(ProgramMenuOption), option);
                var value = (int)option;

                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write("{0}. {1}", value, name);
                Console.ResetColor();
                Console.WriteLine($" - {Program.GetProgramMenuOptionDescription((ProgramMenuOption)value)}");
            }

            //Console.WriteLine("...OR any other key to Exit.");

            ProgramMenuOption result = ProgramMenuOption.Exit;
            var userAction = Console.ReadLine().TrimEnd().ToString();
            Enum.TryParse(userAction, out result);

            Console.WriteLine();
            return result;
        }

        private static string GetProgramMenuOptionDescription(ProgramMenuOption option)
        {
            // quick and dirty descriptions for enum values

            string desc = "No description";

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
                case ProgramMenuOption.SeedSystemData:
                    desc = "Seeds system data to existing database. See FrameworksInitializer.SeedDatabase method for implementation.";
                    break;
                case ProgramMenuOption.CreateMunicipalityOrganizations:
                    desc = "Create Organization and Business for each Municipality.";
                    break;
                //case ProgramMenuOption.ImportGeneralDescriptions:
                //    desc = "Imports general descriptions from JSON file to PTV (requires municipality organizations for service creation).";
                //    break;
                //case ProgramMenuOption.ImportFakePtv:
                //    desc = "Imports fake PTV data to PTV.";
                //    break;
                case ProgramMenuOption.DumpFakePtv:
                    desc = "Generates JSON files to /Generated/FakePtv folder in the app from fake PTV database. Requires the fake PTV database.";
                    break;
                case ProgramMenuOption.ValidateFakePtvJson:
                    desc = "Reads fake PTV data from DB and tries to deserialize to objects. Throws exception if our models are missing members.";
                    break;
                case ProgramMenuOption.CreatePostalCodesJson:
                    desc = "Generates postal code JSON file from Finnish postal service data file.";
                    break;
                case ProgramMenuOption.UpdateCoordinatesForAddress:
                    desc = "Update coordinates for addresses.";
                    break;
                case ProgramMenuOption.ImportOrganizations:
                    desc = "Imports organizations from JSON file (created by WinExcelToJSON tool) to DB.";
                    break;
                default:
                    break;
            }

            return desc;
        }

        /// <summary>
        /// Builds the application configuration.
        /// </summary>
        /// <param name="args">command line arguments</param>
        private static void BuildConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(AppContext.BaseDirectory);
            builder.AddJsonFile("appsettings.json").AddCommandLine(args);

            Program.Configuration = builder.Build();
        }

        /// <summary>
        /// Builds the IServiceProvider for the application and registers available services.
        /// </summary>
        private static void BuildAndRegisterServices()
        {
            IConfigurationRoot config = Program.Configuration;

            if (config == null)
            {
                throw new InvalidOperationException("Application Configuration is null. You must call BuildConfiguration() before calling this method.");
            }
            Console.WriteLine($"Connection string: {config["Data:ptvdb:ConnectionString"]}");

            var srvcol = new ServiceCollection();
            srvcol.AddLogging();

            // register console apps db context
            srvcol.AddEntityFrameworkNpgsql().AddDbContext<SourceDbContext>(options => options.UseNpgsql(Program.Configuration[ConfigKeys.SourceConnectionString]));
            srvcol.AddDbContext<SourceDbContext>(options => options.UseNpgsql(Program.Configuration[ConfigKeys.SourceConnectionString]));
            // register ptv db context and PTV services
            RegisterServiceManager.RegisterFromAllAssemblies(srvcol);
            FrameworksInitializer.RegisterEntityFramework(srvcol, Program.Configuration[ConfigKeys.PTVConnectionString]);

            // for now make it behave like this..
            srvcol.AddTransient<ISourceRepository, SourceRepository>();
            srvcol.AddTransient<IFakePtvRepository, FakePtvJsonFileRepository>();
            srvcol.AddTransient<IHttpContextAccessor, FakeHttpContext>();
            srvcol.AddTransient<IHostingEnvironment, FakeHostingEnv>();
            srvcol.AddSingleton(new ApplicationConfiguration(config));
            srvcol.AddSingleton(new MigrationTools(srvcol.BuildServiceProvider()));

            Program.ServiceProvider = srvcol.BuildServiceProvider();
        }

        private static void ConfigureLogging()
        {
            ILoggerFactory logFactory = Program.ServiceProvider.GetService<ILoggerFactory>();
            logFactory.AddConsole(minLevel: LogLevel.Error);
            logFactory.AddNLog();

            // Configure NLog
            //PlatformServices.Default.Application.ConfigureNLog("nlog.config");

            // uncomment to log to debug output window
            // just for now to see the sql warnings
            //logFactory.AddDebug(minLevel: LogLevel.Verbose);
        }
    }

    public class FakeHttpContext : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }

    public class FakeHostingEnv : IHostingEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; } = "Generated\\";
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
