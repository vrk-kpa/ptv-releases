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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTV.DataImport.ConsoleApp.DataAccess;
using PTV.DataImport.ConsoleApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PTV.Framework;
using PTV.Database.DataAccess;
using PTV.DataImport.ConsoleApp.Models;
using PTV.DataImport.ConsoleApp.Tasks;
using NLog.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PTV.DataImport.ConsoleApp;
using PTV.Framework.Extensions;
using System.IO;
using PTV.Database.DataAccess.EntityCloners;

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

        public static void ProcessMenuAction(ProgramMenuOption userMenuAction, ILogger logger)
        {
            Console.WriteLine($"Calling '{userMenuAction}'...");
            switch (userMenuAction)
            {
                case ProgramMenuOption.InitialPtvCreate:
                    Stopwatch sw = new Stopwatch();
                    Stopwatch swTotal = new Stopwatch();
                    swTotal.Start();

                    // create and apply migrations
                    System.Console.WriteLine("Creating new database...");
                    sw.Start();
                    FrameworksInitializer.DoMigration(Program.ServiceProvider);
                    sw.Stop();
                    string msg = $"Database created in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();

                    // seed system data
                    System.Console.WriteLine("Seeding system data to database..");
                    sw.Restart();
                    FrameworksInitializer.SeedDatabase(Program.ServiceProvider);
                    sw.Stop();
                    msg = $"System data seeded to database in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();

                    // Seed finto data
                    System.Console.WriteLine("Seeding finto data..");
                    sw.Restart();
                    ImportFintoDataTask fintoTask = new ImportFintoDataTask(Program.ServiceProvider);
                    fintoTask.ImportData();
                    sw.Stop();
                    System.Console.WriteLine();
                    msg = $"Finto items imported in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();

                    // Seed digital authorizations
                    System.Console.WriteLine("Seeding digital authorizations..");
                    sw.Restart();
                    var importDigitalAuthorizations = new DownloadAndImportDigitalAuthorizationsTask(ServiceProvider);
                    importDigitalAuthorizations.ImportDigitalAuthorizations();
                    sw.Stop();
                    System.Console.WriteLine();
                    msg = $"Digital authorization items imported in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();

                    // Create organizations for municipalities
                    System.Console.WriteLine("Creating organizations for municipalities..");
                    sw.Restart();
                    CreateMunicipalityOrganizationsTask munOrgTask = new CreateMunicipalityOrganizationsTask(Program.ServiceProvider);
                    munOrgTask.Create();
                    sw.Stop();
                    System.Console.WriteLine();
                    msg = $"Organizations for municipalities created in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();

                    // import general descriptions
                    System.Console.WriteLine("Importing general descriptions from JSON file..");
                    sw.Restart();
                    UpdateCreateGeneralDescriptionsTask generalDescriptionsTask = new UpdateCreateGeneralDescriptionsTask(ServiceProvider);
                    generalDescriptionsTask.ImportDataFromJSON();
                    CreateServiceDataForGeneralDescriptionsJsonTask createServicesDescTask = new CreateServiceDataForGeneralDescriptionsJsonTask(Program.ServiceProvider);
                    createServicesDescTask.ImportDataFromJSON();
                    sw.Stop();
                    System.Console.WriteLine();
                    msg = $"General descriptions imported in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();

                    // import fake ptv data from json
                    System.Console.WriteLine("Starting fake PTV import..");
                    sw.Restart();
                    ImportTask fakeit = new ImportTask(Program.ServiceProvider);
                    fakeit.ImportFakePtv();
                    sw.Stop();
                    msg = $"Fake PTV import complete in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();
                    var languagesAndVersionsTask = new UpdateLangaugeAvailabilitiesAndVersions(Program.ServiceProvider);
                    languagesAndVersionsTask.CheckAndUpdateLangaugeAvailabilitiesAndVersions();
                    //Update text description to Json
                    var updateTextDescriptionTask = new UpdateTextDescriptionToJsonTask(Program.ServiceProvider);
                    updateTextDescriptionTask.CheckAndUpdateTextDescriptionToJson();

                    swTotal.Stop();
                    msg = $"Create, seed and import data, total time: {swTotal.Elapsed}";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();

                    System.Console.WriteLine("See log files in /logs subfolder for details.");
                    break;
                case ProgramMenuOption.CreateOrMigrateDatabase:
                    // create and apply migrations
                    System.Console.WriteLine("Applying migrations...");
                    FrameworksInitializer.DoMigration(Program.ServiceProvider);
                    System.Console.WriteLine("Database created.");
                    break;
                case ProgramMenuOption.SeedSystemData:
                    // seed system data
                    System.Console.WriteLine("Seeding system data to database..");
                    FrameworksInitializer.SeedDatabase(Program.ServiceProvider);
                    System.Console.WriteLine("System data seeded to database.");
                    break;
                case ProgramMenuOption.DownloadGeneralDescription:
                    // import general descriptions
                    System.Console.WriteLine("Downloading general descriptions from DB to file..");
                    sw = new Stopwatch();
                    sw.Restart();
                    UpdateCreateGeneralDescriptionsTask downloadDescTask = new UpdateCreateGeneralDescriptionsTask(Program.ServiceProvider);
                    downloadDescTask.DownloadFromDatabase();
                    sw.Stop();

                    System.Console.WriteLine();
                    msg = $"General descriptions downloaded in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();
                    break;
                case ProgramMenuOption.UpdateGeneralDescription:
                    // import general descriptions
                    System.Console.WriteLine("Importing general descriptions from JSON file..");
                    sw = new Stopwatch();
                    sw.Restart();
                    UpdateCreateGeneralDescriptionsTask updateDescTask = new UpdateCreateGeneralDescriptionsTask(Program.ServiceProvider);
                    updateDescTask.ImportDataFromJSON();
                    sw.Stop();

                    System.Console.WriteLine();
                    msg = $"General descriptions imported in {sw.Elapsed}.";
                    System.Console.WriteLine(msg);
                    logger.LogInformation(msg);
                    System.Console.WriteLine();
                    break;
                case ProgramMenuOption.CreateMunicipalityOrganizations:
                    System.Console.WriteLine("Creating Organizations for Municipalities.");
                    CreateMunicipalityOrganizationsTask municipalityOrganizationsTask = new CreateMunicipalityOrganizationsTask(Program.ServiceProvider);
                    municipalityOrganizationsTask.Create();
                    break;
                //case ProgramMenuOption.ImportGeneralDescriptions:
                //    System.Console.WriteLine("Generating general descriptions JSON...");
                //    CreateGeneralDescriptionsJsonTask generalDescriptionTask = new CreateGeneralDescriptionsJsonTask(Program.ServiceProvider);
                //    generalDescriptionTask.Generate();
                //    System.Console.WriteLine("General descriptions JSON generation complete.");
                //    break;
                //case ProgramMenuOption.ImportFakePtv:
                //    System.Console.WriteLine("Starting fake PTV import..");
                //    ImportTask it = new ImportTask(Program.ServiceProvider);
                //    it.ImportFakePtv();
                //    System.Console.WriteLine("Fake PTV import complete.");
                //    break;
                case ProgramMenuOption.DumpFakePtv:
                    System.Console.WriteLine("Starting to dump fake PTV to JSON files..");
                    DumpFakePtvToFilesTask task = new DumpFakePtvToFilesTask(Program.ServiceProvider);
                    task.WriteToFiles();
                    System.Console.WriteLine("Fake PTV dumped to JSON files.");
                    break;
                case ProgramMenuOption.ValidateFakePtvJson:
                    System.Console.WriteLine("Starting to validate fake PTV generated JSON files to model..");

                    try
                    {
                        DumpFakePtvToFilesTask validationTask = new DumpFakePtvToFilesTask(Program.ServiceProvider);
                        validationTask.ValidateJson();
                        System.Console.WriteLine("Validation success.");
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.ToString());

                        System.Console.BackgroundColor = ConsoleColor.Red;
                        System.Console.Write("Error:");
                        System.Console.ResetColor();
                        System.Console.WriteLine($" check the SourceXXX model, {ex.Message}");
                    }
                    break;
                case ProgramMenuOption.CreatePostalCodesJson:
                    System.Console.WriteLine("Generating postal codes JSON..");
                    CreatePostalCodesJsonTask pct = new CreatePostalCodesJsonTask(Program.ServiceProvider);
                    pct.Generate();
                    System.Console.WriteLine("Postal codes JSON generation complete.");
                    break;

                case ProgramMenuOption.ImportOrganizations:
                    System.Console.WriteLine("Importing organizations JSON..");
                    CreateOrganizationsJsonTask organizationTask = new CreateOrganizationsJsonTask(Program.ServiceProvider);
                    organizationTask.ImportDataFromJSON();
                    System.Console.WriteLine("Importing organizations complete.");
                    break;
                case ProgramMenuOption.UpdateCoordinatesForAddress:
                    System.Console.WriteLine("Updating addresses..");
                    var addressTask = new UpdateCoordinatesForAddressesTask(Program.ServiceProvider);
                    addressTask.UpdateAddresses();
                    System.Console.WriteLine("Updating addresses complete.");
                    break;
                case ProgramMenuOption.ImportOrUpdateFinto:
                    System.Console.WriteLine("Importing finto data..");
                    var fintoUpdateTask = new ImportFintoDataTask(Program.ServiceProvider);
                    fintoUpdateTask.ImportData();
                    System.Console.WriteLine("Importing finto data complete.");
                    break;
                case ProgramMenuOption.DownloadFinto:
                    System.Console.WriteLine("Importing finto data..");
                    var downloadFintoTask = new DownloadFintoDataTask();
                    downloadFintoTask.GetAllFintoData();
                    System.Console.WriteLine("Importing finto data complete.");
                    break;
                //                        case ProgramMenuOption.SwitchCoordinates:
                //                            System.Console.WriteLine("Switching coordinates..");
                //                            var addressSwitchTask = new UpdateCoordinatesForAddressesTask(Program.ServiceProvider);
                //                            addressSwitchTask.SwitchCoordinates();
                //                            System.Console.WriteLine("Switching coordinates complete.");
                //                            break;
                case ProgramMenuOption.UpdateLanguageAvailabilitiesAndVersions:
                    var languagesAndVersions = new UpdateLangaugeAvailabilitiesAndVersions(Program.ServiceProvider);
                    languagesAndVersions.CheckAndUpdateLangaugeAvailabilitiesAndVersions();
                    break;
                case ProgramMenuOption.UpdateTextDescription:
                    var updateTextDescription = new UpdateTextDescriptionToJsonTask(Program.ServiceProvider);
                    updateTextDescription.CheckAndUpdateTextDescriptionToJson();
                    break;
                case ProgramMenuOption.UpdateServiceDescriptionByGeneralDescription:
                    Console.WriteLine($"Update operation can take few minutes...");
                    var updateBackgroundGeneralDescription = new UpdateServiceDescriptionByGeneralDescriptionTask(Program.ServiceProvider);
                    updateBackgroundGeneralDescription.CheckAndUpdateServiceDescriptionByGeneralDescription();
                    Console.WriteLine("Update operation is complete.");
                    break;
                case ProgramMenuOption.FindCyclicOrganizations:
                    var findCylicOrganizationsTask = new FindCyclicOrganizationsTask(Program.ServiceProvider);
                    findCylicOrganizationsTask.FindCyclicOrganizations();
                    break;
                case ProgramMenuOption.DeleteOldData:
                    var deleteOldDataTask = new DeleteOldDataTask(Program.ServiceProvider);
                    deleteOldDataTask.DeleteOldData();
                    break;
                case ProgramMenuOption.DownloadAndImportDigitalAuthorizations:
                    var digitalAuthorizationsTask = new DownloadAndImportDigitalAuthorizationsTask(ServiceProvider);
                    digitalAuthorizationsTask.DownloadAndImportDigitalAuthorizations();
                    break;
                case ProgramMenuOption.FixFintoMerge:
                    var fixFintoTask = new FixFintoTask(ServiceProvider);
                    fixFintoTask.Apply();
                    break;
                case ProgramMenuOption.DownloadPostalCodesFromCodeServiceTask:
                    DownloadPostalCodesFromCodeServiceTask dpcTask = new DownloadPostalCodesFromCodeServiceTask(ServiceProvider);
                    dpcTask.Download();
                    break;
                case ProgramMenuOption.FixMultiPublishedEntities:
                    FixMultiplePublishedEntitiesTask fixMultipleTask = new FixMultiplePublishedEntitiesTask(ServiceProvider);
                    fixMultipleTask.Apply();
                    break;
//                case ProgramMenuOption.PrintGDwithoutLaw:
//                    PrintGdWithoutLaw printGd = new PrintGdWithoutLaw(ServiceProvider);
//                    printGd.Apply();
//                    break;
                case ProgramMenuOption.CopyLawsFromArchivedToPublishedGeneralDescs:
                    CopyLawsOfGeneralDescs copyLaws = new CopyLawsOfGeneralDescs(ServiceProvider);
                    copyLaws.Apply();
                    break;
//                case ProgramMenuOption.PrintMissingGDSWELaws:
//                    CopyLawsOfGeneralDescs copyLaws2 = new CopyLawsOfGeneralDescs(ServiceProvider);
//                    copyLaws2.PrintMissingSweLaws();
//                    break;
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
                Program.ConfigureLogging();

                // just logging to make a noticable entry in a log file when running the app multiple times
                // easier to find new runs entries
                logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
                logger.LogInformation(".oOo. *** ********************************* *** .oOo.");
                logger.LogInformation(".oOo. *** PTV.DataImport.ConsoleApp started *** .oOo.");
                logger.LogInformation(".oOo. *** ********************************* *** .oOo.");


                var commandLineActions = (Configuration["Actions"] ?? string.Empty).Split(',').Select(i => i.Trim().ParseToInt()).Where(i => i != null).Cast<int>().ToList();
                if (commandLineActions.Any())
                {
                    commandLineActions.ForEach(action =>
                    {
                        if (!Enum.IsDefined(typeof(ProgramMenuOption), action))
                        {
                            Console.WriteLine($"Unknown action option {action}!");
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

            ProgramMenuOption result = ProgramMenuOption.Exit;
            var userAction = System.Console.ReadLine().TrimEnd().ToString();
            Enum.TryParse(userAction, out result);

            System.Console.WriteLine();
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
                case ProgramMenuOption.ImportOrUpdateFinto:
                    desc = "Creates or updates finto data from json to DB.";
                    break;
                case ProgramMenuOption.DownloadFinto:
                    desc = "Downlopad new json files from service views for Finto data.";
                    break;
                case ProgramMenuOption.UpdateGeneralDescription:
                    desc = "Creates or updates general description data in DB.";
                    break;
                case ProgramMenuOption.UpdateLanguageAvailabilitiesAndVersions:
                    desc = "Check and update langauge availabilities and versioning of main entities.";
                    break;
                case ProgramMenuOption.UpdateTextDescription:
                    desc = "Check and update text descriptions to JSON.";
                    break;
                case ProgramMenuOption.UpdateServiceDescriptionByGeneralDescription:
                    desc = "Check and update empty or missing service description by general description after background description changes.";
                    break;
				case ProgramMenuOption.FindCyclicOrganizations:
					desc = "Find cyclic dependencies in organization structure";
					break;
                case ProgramMenuOption.DownloadAndImportDigitalAuthorizations:
                    desc = "Download and import digital authorizations";
                    break;
                case ProgramMenuOption.FixFintoMerge:
                    desc = "Fix duplicated items for finto";
                    break;
                case ProgramMenuOption.DownloadPostalCodesFromCodeServiceTask:
                    desc = "Download postal codes from code service and write to PostalCode.json file under Generated folder.";
                    break;
                case ProgramMenuOption.FixMultiPublishedEntities:
                    desc = "Find and fix multiple published versions of the same data";
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
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            var appSettingsKey = args.Length > 0 ? args[0] : string.Empty;
            if (appSettingsKey.StartsWith("/"))
            {
                appSettingsKey = string.Empty;
            }

            var arguments = args.Where(i => i.StartsWith("/")).ToArray();
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Custom.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true);
            if (!string.IsNullOrEmpty(environment))
            {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true);
                if (!string.IsNullOrEmpty(appSettingsKey))
                {
                    builder.AddJsonFileEncrypted($"appsettings.{environment}.crypt", appSettingsKey, optional: true);
                }
            }
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
            IConfigurationRoot config = Program.Configuration;

            if (config == null)
            {
                throw new InvalidOperationException("Application Configuration is null. You must call BuildConfiguration() before calling this method.");
            }
            System.Console.WriteLine($"Connection string: {config["Data:ptvdb:ConnectionString"]}");

            var srvcol = new ServiceCollection();
            srvcol.AddLogging();

            // register console apps db context
            srvcol.AddEntityFrameworkNpgsql().AddDbContext<SourceDbContext>(options => options.UseNpgsql(Program.Configuration[ConfigKeys.SourceConnectionString]));
            srvcol.AddDbContext<SourceDbContext>(options => options.UseNpgsql(Program.Configuration[ConfigKeys.SourceConnectionString]));
            // register ptv db context and PTV services
            RegisterServiceManager.RegisterFromAllAssemblies(srvcol);
            RegisterDataProviderServices.RegisterFromAssembly(srvcol);
            BaseEntityCloners.RegisterBaseEntityCloners(srvcol);
            FrameworksInitializer.RegisterEntityFramework(srvcol, Program.Configuration[ConfigKeys.PTVConnectionString]);

            // for now make it behave like this..
            srvcol.AddTransient<ISourceRepository, SourceRepository>();
            srvcol.AddTransient<IFakePtvRepository, FakePtvJsonFileRepository>();
            srvcol.AddTransient<IHttpContextAccessor, FakeHttpContext>();
            srvcol.AddTransient<IHostingEnvironment, FakeHostingEnv>();
            srvcol.AddSingleton(new ApplicationConfiguration(config));

            Program.ServiceProvider = srvcol.BuildServiceProvider();
        }

        private static void ConfigureLogging()
        {
            ILoggerFactory logFactory = Program.ServiceProvider.GetService<ILoggerFactory>();
            logFactory.AddConsole(minLevel: LogLevel.Error);

            // uncomment to log to debug output window
            //logFactory.AddDebug(minLevel: LogLevel.Verbose);

            // Configure NLog
            logFactory.AddNLog();

            try
            {
                logFactory.ConfigureNLog("nlog.config");
            }
            catch (FileNotFoundException)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING! nlog.config file not found.");
                System.Console.ResetColor();
            }
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
        public string ContentRootPath { get; set; } = "Generated";
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
