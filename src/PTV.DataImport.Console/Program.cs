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
using PTV.DataImport.ConsoleApp.Services;
using System.Linq;
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
using PTV.Framework.Extensions;
using System.IO;
using Npgsql;
using PTV.Application.Framework;
using PTV.Database.DataAccess.DataMigrations;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Services;
using PTV.Database.Migrations.Migrations;
using PTV.LocalAuthentication;

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
                    MigrationManager.DoPtvMainMigrations(Program.ServiceProvider);
                    //FrameworksInitializer.DoMigration(Program.ServiceProvider);
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
                    MigrationManager.DoPtvMainMigrations(Program.ServiceProvider);
                    //FrameworksInitializer.DoMigration(Program.ServiceProvider);
                    System.Console.WriteLine("Database created.");
                    break;
//                case ProgramMenuOption.SeedSystemData:
//                    // seed system data
//                    System.Console.WriteLine("Seeding system data to database..");
//                    FrameworksInitializer.SeedDatabase(Program.ServiceProvider);
//                    System.Console.WriteLine("System data seeded to database.");
//                    break;
//                case ProgramMenuOption.DownloadGeneralDescription:
//                    // import general descriptions
//                    System.Console.WriteLine("Downloading general descriptions from DB to file..");
//                    sw = new Stopwatch();
//                    sw.Restart();
//                    UpdateCreateGeneralDescriptionsTask downloadDescTask = new UpdateCreateGeneralDescriptionsTask(Program.ServiceProvider);
//                    downloadDescTask.DownloadFromDatabase();
//                    sw.Stop();
//
//                    System.Console.WriteLine();
//                    msg = $"General descriptions downloaded in {sw.Elapsed}.";
//                    System.Console.WriteLine(msg);
//                    logger.LogInformation(msg);
//                    System.Console.WriteLine();
//                    break;
//                case ProgramMenuOption.UpdateGeneralDescription:
//                    // import general descriptions
//                    System.Console.WriteLine("Importing general descriptions from JSON file..");
//                    sw = new Stopwatch();
//                    sw.Restart();
//                    UpdateCreateGeneralDescriptionsTask updateDescTask = new UpdateCreateGeneralDescriptionsTask(Program.ServiceProvider);
//                    updateDescTask.ImportDataFromJSON();
//                    sw.Stop();
//
//                    System.Console.WriteLine();
//                    msg = $"General descriptions imported in {sw.Elapsed}.";
//                    System.Console.WriteLine(msg);
//                    logger.LogInformation(msg);
//                    System.Console.WriteLine();
//                    break;
//                case ProgramMenuOption.CreateMunicipalityOrganizations:
//                    System.Console.WriteLine("Creating Organizations for Municipalities.");
//                    CreateMunicipalityOrganizationsTask municipalityOrganizationsTask = new CreateMunicipalityOrganizationsTask(Program.ServiceProvider);
//                    municipalityOrganizationsTask.Create();
//                    break;
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
//                case ProgramMenuOption.DumpFakePtv:
//                    System.Console.WriteLine("Starting to dump fake PTV to JSON files..");
//                    DumpFakePtvToFilesTask task = new DumpFakePtvToFilesTask(Program.ServiceProvider);
//                    task.WriteToFiles();
//                    System.Console.WriteLine("Fake PTV dumped to JSON files.");
//                    break;
//                case ProgramMenuOption.ValidateFakePtvJson:
//                    System.Console.WriteLine("Starting to validate fake PTV generated JSON files to model..");
//
//                    try
//                    {
//                        DumpFakePtvToFilesTask validationTask = new DumpFakePtvToFilesTask(Program.ServiceProvider);
//                        validationTask.ValidateJson();
//                        System.Console.WriteLine("Validation success.");
//                    }
//                    catch (Exception ex)
//                    {
//                        System.Console.WriteLine(ex.ToString());
//
//                        System.Console.BackgroundColor = ConsoleColor.Red;
//                        System.Console.Write("Error:");
//                        System.Console.ResetColor();
//                        System.Console.WriteLine($" check the SourceXXX model, {ex.Message}");
//                    }
//                    break;
//                case ProgramMenuOption.CreatePostalCodesJson:
//                    System.Console.WriteLine("Generating postal codes JSON..");
//                    CreatePostalCodesJsonTask pct = new CreatePostalCodesJsonTask(Program.ServiceProvider);
//                    pct.Generate();
//                    System.Console.WriteLine("Postal codes JSON generation complete.");
//                    break;
//
//                case ProgramMenuOption.ImportOrganizations:
//                    System.Console.WriteLine("Importing organizations JSON..");
//                    CreateOrganizationsJsonTask organizationTask = new CreateOrganizationsJsonTask(Program.ServiceProvider);
//                    organizationTask.ImportDataFromJSON();
//                    System.Console.WriteLine("Importing organizations complete.");
//                    break;
//                case ProgramMenuOption.UpdateCoordinatesForAddress:
//                    System.Console.WriteLine("Updating addresses..");
//                    var addressTask = new UpdateCoordinatesForAddressesTask(Program.ServiceProvider);
//                    addressTask.UpdateAddresses();
//                    System.Console.WriteLine("Updating addresses complete.");
//                    break;
//                case ProgramMenuOption.ImportOrUpdateFinto:
//                    System.Console.WriteLine("Importing finto data..");
//                    var fintoUpdateTask = new ImportFintoDataTask(Program.ServiceProvider);
//                    fintoUpdateTask.ImportData();
//                    System.Console.WriteLine("Importing finto data complete.");
//                    break;
                //                        case ProgramMenuOption.SwitchCoordinates:
                //                            System.Console.WriteLine("Switching coordinates..");
                //                            var addressSwitchTask = new UpdateCoordinatesForAddressesTask(Program.ServiceProvider);
                //                            addressSwitchTask.SwitchCoordinates();
                //                            System.Console.WriteLine("Switching coordinates complete.");
                //                            break;
//                case ProgramMenuOption.UpdateLanguageAvailabilitiesAndVersions:
//                    var languagesAndVersions = new UpdateLangaugeAvailabilitiesAndVersions(Program.ServiceProvider);
//                    languagesAndVersions.CheckAndUpdateLangaugeAvailabilitiesAndVersions();
//                    break;
//                case ProgramMenuOption.UpdateTextDescription:
//                    var updateTextDescription = new UpdateTextDescriptionToJsonTask(Program.ServiceProvider);
//                    updateTextDescription.CheckAndUpdateTextDescriptionToJson();
//                    break;
//                case ProgramMenuOption.UpdateServiceDescriptionByGeneralDescription:
//                    Console.WriteLine($"Update operation can take few minutes...");
//                    var updateBackgroundGeneralDescription = new UpdateServiceDescriptionByGeneralDescriptionTask(Program.ServiceProvider);
//                    updateBackgroundGeneralDescription.CheckAndUpdateServiceDescriptionByGeneralDescription();
//                    Console.WriteLine("Update operation is complete.");
//                    break;
//                case ProgramMenuOption.FindCyclicOrganizations:
//                    var findCylicOrganizationsTask = new FindCyclicOrganizationsTask(Program.ServiceProvider);
//                    findCylicOrganizationsTask.FindCyclicOrganizations();
//                    break;
//                case ProgramMenuOption.DeleteOldData:
//                    var deleteOldDataTask = new DeleteOldDataTask(Program.ServiceProvider);
//                    deleteOldDataTask.DeleteOldData();
//                    break;
//                case ProgramMenuOption.DownloadAndImportDigitalAuthorizations:
//                    var digitalAuthorizationsTask = new DownloadAndImportDigitalAuthorizationsTask(ServiceProvider);
//                    digitalAuthorizationsTask.DownloadAndImportDigitalAuthorizations();
//                    break;
//                case ProgramMenuOption.FixFintoMerge:
//                    var fixFintoTask = new FixFintoTask(ServiceProvider);
//                    fixFintoTask.Apply();
//                    break;
//                case ProgramMenuOption.DownloadPostalCodesFromCodeServiceTask:
//                    DownloadPostalCodesFromCodeServiceTask dpcTask = new DownloadPostalCodesFromCodeServiceTask(ServiceProvider);
//                    dpcTask.Download();
//                    break;
//                case ProgramMenuOption.FixMultiPublishedEntities:
//                    FixMultiplePublishedEntitiesTask fixMultipleTask = new FixMultiplePublishedEntitiesTask(ServiceProvider);
//                    fixMultipleTask.Apply();
//                    break;
//                case ProgramMenuOption.PrintGDwithoutLaw:
//                    PrintGdWithoutLaw printGd = new PrintGdWithoutLaw(ServiceProvider);
//                    printGd.Apply();
//                    break;
//                case ProgramMenuOption.CopyLawsFromArchivedToPublishedGeneralDescs:
//                    CopyLawsOfGeneralDescs copyLaws = new CopyLawsOfGeneralDescs(ServiceProvider);
//                    copyLaws.Apply();
//                    break;
                //                case ProgramMenuOption.PrintMissingGDSWELaws:
                //                    CopyLawsOfGeneralDescs copyLaws2 = new CopyLawsOfGeneralDescs(ServiceProvider);
                //                    copyLaws2.PrintMissingSweLaws();
                //                    break;
//                case ProgramMenuOption.ImportLanguageStateCultureToLanguageTask:
//                    var importLanguageStateCultureToLanguageTask = new ImportLanguageStateCultureToLanguageTask(Program.ServiceProvider);
//                    importLanguageStateCultureToLanguageTask.ImportData();
//                    break;
//                case ProgramMenuOption.ImportMissingLanguageAvailabilityForTranslationOrderTask:
//                    var importMissingLanguageAvailabilityForTranslationOrderTask = new ImportMissingLanguageAvailabilityForTranslationOrderTask(Program.ServiceProvider);
//                    importMissingLanguageAvailabilityForTranslationOrderTask.UpdateData();
//                    break;
//                case ProgramMenuOption.SeedOrganizationTypeRestrictions:
//                    // import general descriptions restrictions
//                    System.Console.WriteLine("Importing Organization type Restrictions..");
//                    var updateOrganizationTypeRestriciton = new OrganizationTypeRestrictionTask(ServiceProvider);
//                    updateOrganizationTypeRestriciton.UpdateOrganizationTypeRestrictions();
//                    System.Console.WriteLine();
//                    break;
//                case ProgramMenuOption.CreateOrganizationUserList:
//                    // icreate organization user list
//                    System.Console.WriteLine("Creating user organization list...");
//                    var generateOU = new CreateUserOrganizationList(ServiceProvider);
//                    generateOU.Generate();
//                    System.Console.WriteLine();
//                    break;
//                case ProgramMenuOption.GroupVersioning:
//                    System.Console.WriteLine("Grouping versionings by UnificRootIds of versioned entity...");
//                    new GroupVersionsTask(ServiceProvider).GroupVersions();
//                    System.Console.WriteLine("Grouping of versionings finished");
//                    break;
//                case ProgramMenuOption.MapSahaGuids:
//                    // icreate organization user list
//                    System.Console.WriteLine("Map Saha guids ...");
//                    var mapSahaGuids = new MapSahaGuids(ServiceProvider);
//                    mapSahaGuids.MapGuids();
//                    System.Console.WriteLine();
//                    break;
//                case ProgramMenuOption.UpdateInheritedServiceNameByGeneralDescription:
//                    Console.WriteLine($"Update operation can take few minutes...");
//                    var updateServiceDescriptionByGeneralDescriptionTask = new UpdateInheritedForServiceNamesByGeneralDescriptionTask(ServiceProvider);
//                    updateServiceDescriptionByGeneralDescriptionTask.UpdateData();
//                    System.Console.WriteLine();
//                    break;
//                case ProgramMenuOption.ImportMissingNamesAfterTranslationOrderTask:
//                    System.Console.WriteLine("Import missing names after translation order ...");
//                    var importMissingNamesAfterTranslationOrderTask = new ImportMissingNamesAfterTranslationOrderTask(ServiceProvider);
//                    importMissingNamesAfterTranslationOrderTask.UpdateData();
//                    System.Console.WriteLine();
//                    break;
//                case ProgramMenuOption.ImportTestAccounts:
//                    System.Console.WriteLine("Import test accounts ...");
//                    var importTestAccountsTask = new ImportTestAccountsTask(ServiceProvider);
//                    importTestAccountsTask.ImportAndUpdateUsers();
//                    System.Console.WriteLine("Done.");
//                    break;
//                case ProgramMenuOption.ImportUiAccessRights:
//                    System.Console.WriteLine("Import UI access rights ...");
//                    var importAccessRightsTask = new ImportUserRolesTask(ServiceProvider);
//                    importAccessRightsTask.ImportUserRoles();
//                    System.Console.WriteLine("Done.");
//                    break;
//                case ProgramMenuOption.SetSpecialAccessRightsBasedOnRestrictions:
//                    System.Console.WriteLine("Import UI access rights ...");
//                    var setSpecialAccessRightsBasedOnRestrictionsTask = new ImportUserRolesTask.SetSpecialAccessRightsBasedOnRestrictionsTask(ServiceProvider);
//                    setSpecialAccessRightsBasedOnRestrictionsTask.SetAccessRights();
//                    System.Console.WriteLine("Done.");
//                    break;
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
                
//                case ProgramMenuOption.ClearAllClsAddresses:
//                    System.Console.WriteLine("Converting addresses ...");
//                    var streetDataService = new OldAddressToClsStructConverter(ServiceProvider);
//                    streetDataService.ClearAllClsAddresses();
//                    System.Console.WriteLine("Done.");
//                    System.Console.WriteLine();
//                    break;
                
                case ProgramMenuOption.ImportAddresses:
                    System.Console.WriteLine("Importing addresses ...");
                    var proxyServerSettings = new ProxyServerSettings();
                    var clsStreetDownloadSettings = new ClsStreetDownloadSettings();
                    var applicationConfiguration = ServiceProvider.GetService<ApplicationConfiguration>();
                    applicationConfiguration.RawConfiguration.GetSection("ProxyServerSettings").Bind(proxyServerSettings);
                    applicationConfiguration.RawConfiguration.GetSection("ClsStreetDownloadSettings").Bind(clsStreetDownloadSettings);
                    var clsStreetDownloadTask = new ClsStreetDownload(ServiceProvider, proxyServerSettings, clsStreetDownloadSettings);
                    clsStreetDownloadTask.DownloadAddresses();
                    System.Console.WriteLine("Done.");
                    System.Console.WriteLine();
                    break;
                
                case ProgramMenuOption.ConvertAddresses:
                    System.Console.WriteLine("Converting addresses ...");
                    var convertAddressesTask = new OldAddressToClsStructConverter(ServiceProvider);
                    convertAddressesTask.ConvertAddresses();
                    System.Console.WriteLine("Done.");
                    System.Console.WriteLine();
                    break;
                case ProgramMenuOption.RemoveOldSahaGuids:
                    // icreate organization user list
                    System.Console.WriteLine("Remove old saha guids ...");
                    var mapSahaGuids = new MapSahaGuids(ServiceProvider);
                    mapSahaGuids.RemoveOldMappings();
                    System.Console.WriteLine();
                    break;
                //case ProgramMenuOption.CreateImportAddressScript:
//                    var oldProcessing = ServiceProvider.GetService<IOldArchiveProcessingService>();
//                    oldProcessing.ProcessOldContentForMainEntities();
//                    System.Console.WriteLine("Creating import scripts ...");
//                    var importAddressesTask = new OldAddressToClsStructConverter(ServiceProvider);
//                    importAddressesTask.CreateInsertScriptForBrokenStreetAddresses();
//                    System.Console.WriteLine("Done.");
//                    System.Console.WriteLine();
                //    break;
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
//                case ProgramMenuOption.SeedSystemData:
//                    desc = "Seeds system data to existing database. See FrameworksInitializer.SeedDatabase method for implementation.";
//                    break;
//                case ProgramMenuOption.CreateMunicipalityOrganizations:
//                    desc = "Create Organization and Business for each Municipality.";
//                    break;
//                case ProgramMenuOption.ImportGeneralDescriptions:
//                    desc = "Imports general descriptions from JSON file to PTV (requires municipality organizations for service creation).";
//                    break;
//                case ProgramMenuOption.ImportFakePtv:
//                    desc = "Imports fake PTV data to PTV.";
//                    break;
//                case ProgramMenuOption.DumpFakePtv:
//                    desc = "Generates JSON files to /Generated/FakePtv folder in the app from fake PTV database. Requires the fake PTV database.";
//                    break;
//                case ProgramMenuOption.ValidateFakePtvJson:
//                    desc = "Reads fake PTV data from DB and tries to deserialize to objects. Throws exception if our models are missing members.";
//                    break;
//                case ProgramMenuOption.CreatePostalCodesJson:
//                    desc = "Generates postal code JSON file from Finnish postal service data file.";
//                    break;
//                case ProgramMenuOption.UpdateCoordinatesForAddress:
//                    desc = "Update coordinates for addresses.";
//                    break;
//                case ProgramMenuOption.ImportOrganizations:
//                    desc = "Imports organizations from JSON file (created by WinExcelToJSON tool) to DB.";
//                    break;
//                case ProgramMenuOption.ImportOrUpdateFinto:
//                    desc = "Creates or updates finto data from json to DB.";
//                    break;
//                case ProgramMenuOption.UpdateGeneralDescription:
//                    desc = "Creates or updates general description data in DB.";
//                    break;
//                case ProgramMenuOption.UpdateLanguageAvailabilitiesAndVersions:
//                    desc = "Check and update langauge availabilities and versioning of main entities.";
//                    break;
//                case ProgramMenuOption.UpdateTextDescription:
//                    desc = "Check and update text descriptions to JSON.";
//                    break;
//                case ProgramMenuOption.UpdateServiceDescriptionByGeneralDescription:
//                    desc = "Check and update empty or missing service description by general description after background description changes.";
//                    break;
//				case ProgramMenuOption.FindCyclicOrganizations:
//					desc = "Find cyclic dependencies in organization structure";
//					break;
//                case ProgramMenuOption.DownloadAndImportDigitalAuthorizations:
//                    desc = "Download and import digital authorizations";
//                    break;
//                case ProgramMenuOption.FixFintoMerge:
//                    desc = "Fix duplicated items for finto";
//                    break;
//                case ProgramMenuOption.DownloadPostalCodesFromCodeServiceTask:
//                    desc = "Download postal codes from code service and write to PostalCode.json file under Generated folder.";
//                    break;
//                case ProgramMenuOption.FixMultiPublishedEntities:
//                    desc = "Find and fix multiple published versions of the same data";
//                    break;
//                case ProgramMenuOption.ImportLanguageStateCultureToLanguageTask:
//                    desc = "Import language state cultures to languages.";
//                    break;
//                case ProgramMenuOption.ImportMissingLanguageAvailabilityForTranslationOrderTask:
//                    desc = "Import missing language version for entities in Translation.";
//                    break;
//                case ProgramMenuOption.GroupVersioning:
//                    desc = "Group versionings by UnificRootId of versioned entity (this operation is memory intensive)";
//                    break;
//
//                case ProgramMenuOption.UpdateInheritedServiceNameByGeneralDescription:
//                    desc = "Update inherited fields on service name by general description.";
//                    break;
//                case ProgramMenuOption.ImportMissingNamesAfterTranslationOrderTask:
//                    desc = "Import missing entities names after transaltion order.";
//                    break;
                case ProgramMenuOption.MaintenanceCleanDatabaseWithUpdatedDumpTask:
                    desc = "Maintennace for TRAINING db with UPDATED dump with NEW data";
                    break;
                case ProgramMenuOption.MaintenanceCleanDatabaseWithPreviousDumpTask:
                    desc = "Maintennace for TRAINING db with PREVIOUS dump";
                    break;
                default:
                    break;
            }

            return desc;
        }

        private static string GetEnviromentName()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        }

        /// <summary>
        /// Builds the application configuration.
        /// </summary>
        /// <param name="args">command line arguments</param>
        private static void BuildConfiguration(string[] args)
        {
            string environment = GetEnviromentName();
           
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

            var connectionString = config["Data:ptvdb:ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not set.");
            }
            
            Console.WriteLine($"Connection string: {connectionString}");
            var csb = new NpgsqlConnectionStringBuilder(connectionString);
            Console.Write("Database=");
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(csb.Database);
            Console.ResetColor();

            var srvcol = new ServiceCollection();
            srvcol.AddLogging(i =>
            {
                i.SetMinimumLevel(LogLevel.Error);
                i.AddConsole();
            });

            srvcol.AddDbContext<StsDbContext>(options =>
            {
                options.UseNpgsql(Configuration["Data:ptvdb:StsConnectionString"]);
            });
            srvcol.AddIdentity<StsUser, StsRole>().AddEntityFrameworkStores<StsDbContext>();

            // for now make it behave like this..
            srvcol.AddTransient<ISourceRepository, SourceRepository>();
            srvcol.AddTransient<IFakePtvRepository, FakePtvJsonFileRepository>();
            srvcol.AddTransient<IHttpContextAccessor, FakeHttpContext>();
            srvcol.AddTransient<IHostingEnvironment, FakeHostingEnv>(provider => new FakeHostingEnv { EnvironmentName = GetEnviromentName()});
            srvcol.AddSingleton(new ApplicationConfiguration(config));
            PtvAppInitilizer.BaseInit(srvcol, Configuration, Configuration[ConfigKeys.PTVConnectionString]);
            Program.ServiceProvider = srvcol.BuildServiceProvider();
        }

        private static void ConfigureLogging()
        {
            ILoggerFactory logFactory = Program.ServiceProvider.GetService<ILoggerFactory>();
            //logFactory.AddConsole(minLevel: LogLevel.Error);

            // uncomment to log to debug output window
            //logFactory.AddDebug(minLevel: LogLevel.Verbose);

            // Configure NLog
            logFactory.AddNLog();

            try
            {
                NLog.LogManager.LoadConfiguration("nlog.config");
                //logFactory.ConfigureNLog("nlog.config");
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
