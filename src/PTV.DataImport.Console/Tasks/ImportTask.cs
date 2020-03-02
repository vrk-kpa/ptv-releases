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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.DataImport.Console.Models;
using PTV.DataImport.Console.Services;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;

namespace PTV.DataImport.Console.Tasks
{
    public class ImportTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly IStreetService streetService;

        /// <summary>
        /// Default CreatedBy "username".
        /// </summary>
        private const string DefaultCreatedBy = "ImportTask";

        public ImportTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<ImportTask>();
            streetService = serviceProvider.GetService<IStreetService>();

            logger.LogDebug("ImportTask .ctor");
        }

        /// <summary>
        /// Just a helper to avoid repeat the Console.WriteLine(). Writes the <i>msg</i> to console and to log.
        /// </summary>
        /// <param name="msg"></param>
        private void ConsoleWriteEndOfAction(string msg)
        {
            System.Console.WriteLine();
            System.Console.WriteLine(msg);
            System.Console.WriteLine();

            logger.LogInformation(msg);
        }

        public void ImportFakePtv()
        {
            var sw = new Stopwatch();

            // all import methods return List<Tuple<int, Guid>>
            // Tuples int and guid properties (item1 and item2) contain the original id and PTV created id
            // Item1 (int) the id used in the source json for an entity
            // Item2 (Guid) the Guid given to the PTV created corresponding entity

            // import organizations from fake PTV data (only imports the organizations but doesn't connect with any services)
            System.Console.WriteLine("Import all organizations..");
            sw.Restart();
            var importedOrganizations = ImportAllOrganizations();
            sw.Stop();
            ConsoleWriteEndOfAction($"Imported organizations in : {sw.Elapsed}.");


            // COMMENTED OUT BECAUSE NINNI SAID THAT THESE SHOULDN'T BE IMPORTED BUT CONNECT USING NAME OF THE GENERAL DESCRIPTION IN FAKE PTV
            // TO OUR STATUTORY GENERAL DESCRIPTIONS
            // import fake PTV general descriptions
            //var importedStatutoryGeneralDescriptions = ImportStatutoryGeneralDescriptions(systemValues);

            // import webpage channels
            System.Console.WriteLine("Import webpage channels..");
            sw.Restart();
            var importedWebpageChannels = ImportWebpageChannels(importedOrganizations);
            sw.Stop();
            ConsoleWriteEndOfAction($"Imported webpage channels in : {sw.Elapsed}.");

            // import phone channels
            System.Console.WriteLine("Import phone channels..");
            sw.Restart();
            var importedPhoneChannels = ImportPhoneChannels(importedOrganizations);
            sw.Stop();
            ConsoleWriteEndOfAction($"Imported phone channels in : {sw.Elapsed}.");

            // import electronic channels
            System.Console.WriteLine("Import electronic channels..");
            sw.Restart();
            var importedEChannels = ImportElectronicChannels(importedOrganizations);
            sw.Stop();
            ConsoleWriteEndOfAction($"Imported electronic channels in : {sw.Elapsed}.");

            // import service location channels
            System.Console.WriteLine("Import service locations channels..");
            sw.Restart();
            var importedServiceLocationChannels = ImportServiceLocationChannels(importedOrganizations);
            sw.Stop();
            ConsoleWriteEndOfAction($"Imported service location channels in : {sw.Elapsed}.");

            // import printable forms
            System.Console.WriteLine("Import printable forms..");
            sw.Restart();
            var importedPrintableForms = ImportPrintableFormChannels(importedOrganizations);
            sw.Stop();
            ConsoleWriteEndOfAction($"Imported printable forms in : {sw.Elapsed}.");

            // import services
            System.Console.WriteLine("Import services..");
            sw.Restart();
            var importedServices = ImportServices(importedPhoneChannels, importedWebpageChannels, importedEChannels,
                importedServiceLocationChannels, importedPrintableForms, importedOrganizations);
            sw.Stop();
            ConsoleWriteEndOfAction($"Imported services in : {sw.Elapsed}.");

            // connect services to organization
            System.Console.WriteLine("Connect services to organizations..");
            sw.Restart();
            ImportServiceToOrganizationConnection(importedOrganizations, importedServices);
            sw.Stop();
            ConsoleWriteEndOfAction($"Connected services to organizations in : {sw.Elapsed}.");
        }

        private void ImportServiceToOrganizationConnection(List<Tuple<int, Guid>> organizationIdsMap, List<Tuple<int, Guid>> serviceIdsMap)
        {
            if (organizationIdsMap == null)
            {
                throw new ArgumentNullException(nameof(organizationIdsMap));
            }

            if (serviceIdsMap == null)
            {
                throw new ArgumentNullException(nameof(serviceIdsMap));
            }

            // TODO : implement provision type, the data is in services.json

            var repo = serviceProvider.GetService<IFakePtvRepository>();
            var sourceOrganizations = repo.GetOrganizations();
            var sourceServices = repo.GetServices();

            if (sourceOrganizations == null || sourceOrganizations.Count == 0)
            {
                throw new Exception("No organizations found from source repository.");
            }

            if (organizationIdsMap == null || organizationIdsMap.Count == 0)
            {
                throw new ArgumentException("Cannot connect services to organizations because the argument organizationIdsMap doesn't contain any entries.", nameof(organizationIdsMap));
            }

            if (serviceIdsMap == null || serviceIdsMap.Count == 0)
            {
                throw new ArgumentException("Cannot connect services to organizations because the argument serviceIdsMap doesn't contain any entries.", nameof(organizationIdsMap));
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
//                    GetSystemValues(unitOfWork);
                    var orgServiceRepo = unitOfWork.CreateRepository<IOrganizationServiceRepository>();

                    foreach (var srcOrg in sourceOrganizations)
                    {
                        // console progressbar
                        System.Console.Write("#");

                        if (srcOrg.Services == null)
                        {
                            logger.LogWarning($"ImportServiceToOrganizationConnection, source organization with id '{srcOrg.Id}' doesn't have any services.");
                            continue;
                        }

                        // find the system guid for the organization that has been added in previous import steps
                        var matchedOrg = organizationIdsMap.Find(x => x.Item1 == srcOrg.Id);

                        if (matchedOrg == null)
                        {
                            logger.LogWarning($"ImportServiceToOrganizationConnection, organization id map didn't contain organization with source organization id: '{srcOrg.Id}'. Cannot connect with services.");
                            continue;
                        }

                        if (srcOrg.Services.Producing == null)
                        {
                            logger.LogWarning($"ImportServiceToOrganizationConnection, source organization with id '{srcOrg.Id}' doesn't have any producing services.");
                        }
                        else
                        {
                            ConnectServiceToOrganization(srcOrg.Services.Producing, serviceIdsMap, /*ProducerInfo systemValues.RoleTypes.Producer, */sourceServices, matchedOrg, orgServiceRepo);
                        }

                        logger.LogWarning(srcOrg.Services.Responsible == null
                            ? $"ImportServiceToOrganizationConnection, source organization with id '{srcOrg.Id}' doesn't have any responsible services."
                            : "Importing of service producers is not supported just now .. will be fixed soon");
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }
        }

        /// <summary>
        /// Tries to import all organizations from source database to PTV database.
        /// </summary>
        /// <returns>list of tuple where first value is the source id for the organization and second value is the guid for the created organization</returns>
        private List<Tuple<int, Guid>> ImportAllOrganizations()
        {
            // get all source organizations to memory
            var repo = serviceProvider.GetService<IFakePtvRepository>();
            var sourceOrgs = repo.GetOrganizations();

            if (sourceOrgs == null || sourceOrgs.Count == 0)
            {
                throw new Exception("No organizations found from source repository.");
            }

            var createdOrganizations = new List<Tuple<int, Guid>>(sourceOrgs.Count);
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var systemValues = GetSystemValues(unitOfWork);
                    var postalCodes = unitOfWork.CreateRepository<IPostalCodeRepository>().All().ToList();
                    foreach (var org in sourceOrgs)
                    {
                        try
                        {
                            createdOrganizations.Add(new Tuple<int, Guid>(org.Id, ImportOrganization(unitOfWork, org, systemValues, postalCodes)));
                            // console progressbar
                            System.Console.Write("#");
                        }
                        catch (Exception ex)
                        {
                            // Exception thrown if the organization already exists in the PTV database or organization type is missing
                            logger.LogError(ex.Message);
                        }
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }
            return createdOrganizations;
        }

        /// <summary>
        /// Imports organization and other things related to it.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="srcOrg"></param>
        /// <param name="systemValues">system values</param>
        /// <param name="postalCodes"></param>
        /// <exception cref="System.ArgumentNullException"><i>srcOrg</i> or <i>sysValues</i> is null</exception>
        /// <exception cref="System.Exception">Organization with <i>organizationId</i> already exists in the PTV database.</exception>
        private Guid ImportOrganization(IUnitOfWorkWritable unitOfWork, SourceOrganizationEntity srcOrg, ImportTaskSystemValues systemValues, List<PostalCode> postalCodes)
        {
            if (srcOrg == null)
            {
                throw new ArgumentNullException(nameof(srcOrg), "Organization can not be null.");
            }

            if (systemValues == null)
            {
                throw new ArgumentNullException(nameof(systemValues));
            }
            // guid of the created organization
            var newOrganizationVersionedId = Guid.NewGuid();
            var newOrganizationRootId = Guid.NewGuid();
            var nameTypeName = NameTypeEnum.Name.ToString();

            var orgRepo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            // try to get current organizations municipality
            var currentMunicipality = unitOfWork.CreateRepository<IMunicipalityRepository>().All().FirstOrDefault(m => m.Code == srcOrg.MunicipalityCode);

            // repositories
            var webPageRepo = unitOfWork.CreateRepository<IWebPageRepository>();
            var businessRepo = unitOfWork.CreateRepository<IBusinessRepository>();

            var orgWebPageRepo = unitOfWork.CreateRepository<IOrganizationWebPageRepository>();
            var orgNameRepo = unitOfWork.CreateRepository<IOrganizationNameRepository>();
            var orgDisplayNameTypeRepo = unitOfWork.CreateRepository<IOrganizationDisplayNameTypeRepository>();
            var orgDescriptionRepo = unitOfWork.CreateRepository<IOrganizationDescriptionRepository>();
            var orgEmailRepo = unitOfWork.CreateRepository<IOrganizationEmailRepository>();
            var orgPhoneNumberRepo = unitOfWork.CreateRepository<IOrganizationPhoneRepository>();

            var defaultNameType = unitOfWork.CreateRepository<INameTypeRepository>().All().FirstOrDefault(x => x.Code == nameTypeName);
            //var orgServiceRepo = unitOfWork.CreateRepository<IOrganizationServiceRepository>();

            //var orgTypeRepo = unitOfWork.CreateRepository<IOrganizationTypeRepository>();

            var orgType = GetOrganizationType(systemValues.OrganizationTypes, srcOrg);

            if (orgType == null)
            {
                // we cannot add an organization without a valid OrganizationType
                throw new Exception(
                    $"ImportOrganization, cannot add organization with source organization id '{srcOrg.Id}' because the organizations type '{srcOrg.OrganizationType}' cannot be matched to system OrganizationType.");
            }


            // if current organizations municipality doesn't exist in the db, add the municipality using source organization
            if (currentMunicipality == null)
            {
                if (string.IsNullOrWhiteSpace(srcOrg.MunicipalityCode) || string.IsNullOrWhiteSpace(srcOrg.MunicipalityName))
                {
                    logger.LogInformation(
                        $"Fake PTV source organization with id: '{srcOrg.Id}' has no municipality code and/or name defined (name: '{srcOrg.MunicipalityName}', code: '{srcOrg.MunicipalityCode}').");
                }
                else
                {
                    logger.LogWarning(
                        $"Municipality with code {srcOrg.MunicipalityCode} doesn't exist in the PTV database. Can not connect Fake PTV source organization with id: '{srcOrg.Id}' to municipality '{srcOrg.MunicipalityName}' (code: '{srcOrg.MunicipalityCode}').");

                    // don't create new municipality based on the organization information
                    // this code is called in a loop and municipality is not saved and for fake ptv data we end up creating mikkeli multiple times!

                    //currentMunicipality = new Municipality()
                    //{
                    //    Code = srcOrg.MunicipalityCode,
                    //    Id = Guid.NewGuid(),
                    //    Description = srcOrg.MunicipalityName,
                    //    Name = srcOrg.MunicipalityName,
                    //    //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType
                    //};
                    //ImportTask.SetCreatedInfo(currentMunicipality);
                    //municipalityRepo.Add(currentMunicipality);
                }
            }



            // create business entity out of the new organization
            // should we implement a check that the business doesn't already exists in DB?
            var newBusiness = new Business
            {
                Id = Guid.NewGuid(),
                Code = srcOrg.BusinessId?.Trim().Replace(" ", string.Empty),
                Description = srcOrg.AlternateName,
                //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                Name = srcOrg.Name
            };
            SetCreatedInfo(newBusiness);
            businessRepo.Add(newBusiness);


            // create organization
            var org = new OrganizationVersioned
            {
                Business = newBusiness,
                Id = newOrganizationVersionedId,
                Municipality = currentMunicipality,
                PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                Type = orgType,
                AreaInformationType = systemValues.AreaInformationTypes.WholeCountry,
                //DisplayNameType = defaultNameType,
                UnificRoot = new Organization { Id = newOrganizationRootId }
            };
            SetCreatedInfo(org);
            orgRepo.Add(org);

            // create web pages and connect them with the organization
            if (srcOrg.Webpages != null && srcOrg.Webpages.Count > 0)
            {
                foreach (var sourceWebPage in srcOrg.Webpages)
                {
                    // source data contains bogus entries, simple check to skip those (http:// == 7 chars => len 7)
                    if (string.IsNullOrWhiteSpace(sourceWebPage.Url) || sourceWebPage.Url.Length < 8)
                    {
                        continue;
                    }

                    var wp = new WebPage
                    {
                        Id = Guid.NewGuid(),
                        //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                        Url = sourceWebPage.Url
                    };
                    SetCreatedInfo(wp);
                    webPageRepo.Add(wp);

                    orgWebPageRepo.Add(new OrganizationWebPage
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        OrganizationVersioned = org,
                        Type = systemValues.WebpageTypes.Home,
                        Localization = systemValues.DefaultLanguage,
                        Name = sourceWebPage.GetWebpageName(),
                        WebPage = wp
                    });
                }
            }

            // organization email
            if (!string.IsNullOrWhiteSpace(srcOrg.Email))
            {


                orgEmailRepo.Add(new OrganizationEmail
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = DefaultCreatedBy,
                    Email = new Email
                    {
                        Value = srcOrg.Email,
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        Localization = systemValues.DefaultLanguage,
                    },
                    OrganizationVersioned = org
                });
            }

            // organization phoneNumber
            if (!string.IsNullOrWhiteSpace(srcOrg.Phone))
            {
                orgPhoneNumberRepo.Add(new OrganizationPhone
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = DefaultCreatedBy,
                    Phone = new Phone
                    {
                        Id = Guid.NewGuid(),
                        Number = GetTrimmedText(srcOrg.Phone.Replace(" ", string.Empty).Trim(), 20),
                        Type = systemValues.PhoneNumberTypes.Phone,
                        ChargeType = systemValues.ServiceChargeTypes.Charged,
                        Localization = systemValues.DefaultLanguage,
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                    },
                    OrganizationVersioned = org,
                });
            }

            // organization names
            if (!string.IsNullOrWhiteSpace(srcOrg.Name))
            {
                orgNameRepo.Add(new OrganizationName
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = DefaultCreatedBy,
                    Localization = systemValues.DefaultLanguage,
                    Name = srcOrg.Name.Trim(),
                    OrganizationVersioned = org,
                    Type = systemValues.NameTypes.Name
                });
            }

            if (!string.IsNullOrWhiteSpace(srcOrg.AlternateName))
            {
                orgNameRepo.Add(new OrganizationName
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = DefaultCreatedBy,
                    Localization = systemValues.DefaultLanguage,
                    Name = srcOrg.AlternateName.Trim(),
                    OrganizationVersioned = org,
                    Type = systemValues.NameTypes.AlternateName
                });
            }

            orgDisplayNameTypeRepo.Add(new OrganizationDisplayNameType
            {
                Created = DateTime.UtcNow,
                CreatedBy = DefaultCreatedBy,
                Localization = systemValues.DefaultLanguage,
                OrganizationVersioned = org,
                DisplayNameType = defaultNameType
            });

            // organization description
            if (!string.IsNullOrWhiteSpace(srcOrg.Description))
            {
                orgDescriptionRepo.Add(new OrganizationDescription
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = DefaultCreatedBy,
                    Description = srcOrg.Description,
                    Localization = systemValues.DefaultLanguage,
                    OrganizationVersioned = org,
                    Type = systemValues.DescriptionTypes.Description
                });
            }

            // create postal addresses
            AddOrganizationPostalAddress(org, srcOrg.PostalAddresses, postalCodes, systemValues, unitOfWork);

            // create visiting addresses
            AddOrganizationVisitingAddress(org, srcOrg.VisitAddresses, postalCodes, systemValues, unitOfWork);

            logger.LogInformation($"Saving organization '{srcOrg.Name}' (id: {srcOrg.Id}).");
            //unitOfWork.Save(SaveMode.AllowAnonymous);
            logger.LogInformation($"Organization '{srcOrg.Name}' (id: {srcOrg.Id}) saved.");

            return newOrganizationRootId;
        }

        private List<Tuple<int, Guid>> ImportPhoneChannels(List<Tuple<int, Guid>> organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            if (organizations.Count == 0)
            {
                throw new ArgumentException("There are no organizations provided. The webpage channels cannot be imported without organizations.", nameof(organizations));
            }

            var repo = serviceProvider.GetService<IFakePtvRepository>();

            var srcPhoneChannels = repo.GetPhoneEntities();

            logger.LogDebug($"Loaded {srcPhoneChannels.Count} phone channels from fake ptv.");

            var addedPhoneChannels = new List<Tuple<int, Guid>>();

            if (srcPhoneChannels.Count == 0)
            {
                logger.LogWarning("Cannot import phone channels as there are no entries in the fake ptv.");
                return addedPhoneChannels;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var systemValues = GetSystemValues(unitOfWork);
                    // existing phone channel names

                    // target groups
                    var targetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().ToList();

                    // get existing service channel names for channels that are phone channels
                    // get every channels channelNames in the last query (many channels and each can have many names)
                    var existingPhoneChannelNames = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(sc => sc.TypeId == systemValues.ServiceChannelTypes.Phone.Id),
                        includes => includes.Include(sc => sc.ServiceChannelNames)).ToList().SelectMany(channel => channel.ServiceChannelNames.Select(scn => scn.Name)).ToList();

                    var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var phoneRepo = unitOfWork.CreateRepository<IPhoneRepository>();
                    var serviceChannelPhoneRepo = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();

                    var emailRepo = unitOfWork.CreateRepository<IEmailRepository>();
                    var serviceChannelEmailRepo = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();

                    var nameRepo = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                    var descRepo = unitOfWork.CreateRepository<IServiceChannelDescriptionRepository>();

                    var channelWebpageRepo = unitOfWork.CreateRepository<IServiceChannelWebPageRepository>();
                    var webpageRepo = unitOfWork.CreateRepository<IWebPageRepository>();

                    var scServiceHoursRepo = unitOfWork.CreateRepository<IServiceChannelServiceHoursRepository>();
                    var serviceHoursRepo = unitOfWork.CreateRepository<IServiceHoursRepository>();
                    var serviceHoursAdditionalInfoRepo = unitOfWork.CreateRepository<IServiceHoursAdditionalInformationRepository>();

                    var channelLangRepo = unitOfWork.CreateRepository<IServiceChannelLanguageRepository>();

                    var scTargetGroupRepo = unitOfWork.CreateRepository<IServiceChannelTargetGroupRepository>();

                    var defaultConnectionType = unitOfWork.CreateRepository<IServiceChannelConnectionTypeRepository>()
                        .All().FirstOrDefault(x => x.Code == ServiceChannelConnectionTypeEnum.NotCommon.ToString());

                    foreach (var fakePhoneChannel in srcPhoneChannels)
                    {
                        // console progressbar
                        System.Console.Write("#");
                        if (existingPhoneChannelNames.Contains(fakePhoneChannel.Name))
                        {
                            logger.LogWarning($"Phone channel name {fakePhoneChannel.Name} already exists in the database. Possible duplicate from fake PTV data.");
                        }

                        if (string.IsNullOrWhiteSpace(fakePhoneChannel.Phone))
                        {
                            logger.LogWarning($"Fake PTV data phone channel entry with id:'{fakePhoneChannel.Id}' doesn't contain phone number. Skipping the entry.");
                            continue;
                        }

                        var orgTuple = organizations.Find(x => x.Item1 == fakePhoneChannel.Organization);

                        if (orgTuple == null)
                        {
                            logger.LogError($"Fake PTV phone channel, organization with id '{fakePhoneChannel.Organization}' not found from organizations list. Cannot add the phone channel entry with id: {fakePhoneChannel.Id}.");
                            continue;
                        }

                        // create new service channel for phone type
                        var sc = new ServiceChannelVersioned
                        {
                            Id = Guid.NewGuid(),
                            Type = systemValues.ServiceChannelTypes.Phone,
                            Charge = true,
                            OrganizationId = orgTuple.Item2,
                            PublishingStatus = systemValues.PublishingStatuses.DraftStatusType,
                            UnificRoot = new ServiceChannel {  Id = Guid.NewGuid() },
                            ConnectionType = defaultConnectionType
                        };
                        SetCreatedInfo(sc);
                        channelRepo.Add(sc);

                        // create new phone channel
                        var phone = new Phone
                        {
                            Id = Guid.NewGuid(),
                            Type = GetPhoneNumberType(fakePhoneChannel, systemValues.PhoneNumberTypes),
                            Localization = systemValues.DefaultLanguage,
                            Number = GetTrimmedText(fakePhoneChannel.Phone, 20),
                            ChargeType = systemValues.ServiceChargeTypes.Charged,
                            AdditionalInformation = fakePhoneChannel.PhoneCallFeeAdditionalInformation
                        };
                        SetCreatedInfo(phone);
                        phoneRepo.Add(phone);

                        var serviceChannelPhone = new ServiceChannelPhone
                        {
                            ServiceChannelVersioned = sc,
                            Phone = phone
                        };
                        serviceChannelPhoneRepo.Add(serviceChannelPhone);

                        // add channels phone number
                        // channel name
                        nameRepo.Add(new ServiceChannelName
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
                            Name = fakePhoneChannel.Name,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.NameTypes.Name
                        });

                        // channel description, add with default description type as the source data doesn't have short description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(fakePhoneChannel.Description),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.Description
                        });

                        // add channel support info if the source has email
                        if (!string.IsNullOrWhiteSpace(fakePhoneChannel.Email))
                        {
                            var email = new Email
                            {
                                Value = fakePhoneChannel.Email,
                                Localization = systemValues.DefaultLanguage
                            };

                            SetCreatedInfo(email);
                            emailRepo.Add(email);

                            serviceChannelEmailRepo.Add(new ServiceChannelEmail
                            {
                                Email = email,
                                ServiceChannelVersioned = sc,
                                Created = DateTime.UtcNow,
                                CreatedBy = DefaultCreatedBy
                            });
                        }

                        // add web page if source has it
                        if (!string.IsNullOrWhiteSpace(fakePhoneChannel.WebpageUrl))
                        {
                            var wp = new WebPage
                            {
                                Id = Guid.NewGuid(),
                                //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                                Url = fakePhoneChannel.WebpageUrl
                            };
                            SetCreatedInfo(wp);
                            webpageRepo.Add(wp);

                            channelWebpageRepo.Add(new ServiceChannelWebPage
                            {
                                Created = DateTime.UtcNow,
                                CreatedBy = DefaultCreatedBy,
                                ServiceChannelVersioned = sc,
                                Type = systemValues.WebpageTypes.Home,
                                Localization = systemValues.DefaultLanguage,
                                Name = $"{fakePhoneChannel.Name} kotisivu",
                                WebPage = wp
                            });
                        }

                        var channelIdentifier = $"Phone channel source id: {fakePhoneChannel.Id}.";

                        // add channels service hours
                        if (fakePhoneChannel.OpeningTimes != null && fakePhoneChannel.OpeningTimes.Count > 0)
                        {
                            AddServiceChannelServiceHours(fakePhoneChannel.OpeningTimes, systemValues, sc, scServiceHoursRepo, serviceHoursRepo, serviceHoursAdditionalInfoRepo);
                        }

                        // channel languages
                        if (fakePhoneChannel.Languages != null && fakePhoneChannel.Languages.Count > 0)
                        {
                            AddChannelLanguages(systemValues.Languages, fakePhoneChannel.Languages, sc, channelLangRepo, $"Phone channel, source entity id: '{fakePhoneChannel.Id}'");
                        }

                        // link target group
                        ConnectChannelTargetGroups(sc, scTargetGroupRepo, targetGroups, fakePhoneChannel.TargetGroups, channelIdentifier);

                        // link service class / currently source has no definition to these
                        //ConnectChannelServiceClasses(sc, scServiceClassRepo, serviceClasses, fakePhoneChannel.ServiceClasses, channelIdentifier);

                        // link ontology term / currently source has no definition to these
                        //ConnectChannelOntologyTerms(sc, scOntologyRepo, ontologyTerms, fakePhoneChannel.OntologyTerms, channelIdentifier);

                        // link or create new keyword / currently source has no definition to these
                        //ConnectOrCreateChannelKeywords(sc, scKeywordRepo, kwRepo, keywords, fakePhoneChannel.Keywords, channelIdentifier);

                        addedPhoneChannels.Add(Tuple.Create(fakePhoneChannel.Id, sc.UnificRoot.Id));
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }

            return addedPhoneChannels;
        }

        private List<Tuple<int, Guid>> ImportWebpageChannels(List<Tuple<int, Guid>> organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            if (organizations.Count == 0)
            {
                throw new ArgumentException("There are no organizations provided. The webpage channels cannot be imported without organizations.", nameof(organizations));
            }

            var srcWebpageChannels = serviceProvider.GetService<IFakePtvRepository>().GetElectronicInformationServices();

            var addedWebpageChannels = new List<Tuple<int, Guid>>();

            if (srcWebpageChannels == null || srcWebpageChannels.Count == 0)
            {
                logger.LogWarning("There are no Webpage channels to import from fake PTV repository.");
                return addedWebpageChannels;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var systemValues = GetSystemValues(unitOfWork);
                    // target groups
                    var targetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().ToList();
                    // ontology terms
                    var ontologyTerms = unitOfWork.CreateRepository<IOntologyTermRepository>().All().ToList();
                    // keywords
                    var keywords = unitOfWork.CreateRepository<IKeywordRepository>().All().ToList();

                    // get existing service channel names for channels that are webpage channels
                    // get every channels channelNames in the last query (many channels and each can have many names)
                    // existing webpage channel names
                    var existingWebpageChannelNames = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(sc => sc.TypeId == systemValues.ServiceChannelTypes.Webpage.Id),
                        includes => includes.Include(sc => sc.ServiceChannelNames)).ToList().SelectMany(channel => channel.ServiceChannelNames.Select(scn => scn.Name)).ToList();
                    var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var webChannelRepo = unitOfWork.CreateRepository<IWebpageChannelRepository>();

                    var nameRepo = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                    var descRepo = unitOfWork.CreateRepository<IServiceChannelDescriptionRepository>();

                    var phoneRepo = unitOfWork.CreateRepository<IPhoneRepository>();
                    var serviceChannelPhoneRepo = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                    var emailRepo = unitOfWork.CreateRepository<IEmailRepository>();
                    var serviceChannelEmailRepo = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();

                    // attachment repos
                    var webAttachmentRepo = unitOfWork.CreateRepository<IServiceChannelAttachmentRepository>();
                    var attachmentRepo = unitOfWork.CreateRepository<IAttachmentRepository>();

                    // url repo
                    var webpageUrlRepo = unitOfWork.CreateRepository<IWebpageChannelUrlRepository>();

                    var channelLangRepo = unitOfWork.CreateRepository<IServiceChannelLanguageRepository>();

                    // connectable repos
                    var scTargetGroupRepo = unitOfWork.CreateRepository<IServiceChannelTargetGroupRepository>();
                    var scOntologyRepo = unitOfWork.CreateRepository<IServiceChannelOntologyTermRepository>();
                    var scKeywordRepo = unitOfWork.CreateRepository<IServiceChannelKeywordRepository>();
                    var kwRepo = unitOfWork.CreateRepository<IKeywordRepository>();

                    var defaultConnectionType = unitOfWork.CreateRepository<IServiceChannelConnectionTypeRepository>()
                        .All().FirstOrDefault(x => x.Code == ServiceChannelConnectionTypeEnum.NotCommon.ToString());

                    foreach (var fakeWebpageChannel in srcWebpageChannels)
                    {
                        // console progressbar
                        System.Console.Write("#");

                        if (existingWebpageChannelNames.Contains(fakeWebpageChannel.Name))
                        {
                            logger.LogWarning($"Webpage channel name '{fakeWebpageChannel.Name}' already exists in the database. Possible duplicate from fake PTV data.");
                        }

                        if (string.IsNullOrWhiteSpace(fakeWebpageChannel.Url))
                        {
                            logger.LogWarning($"Fake PTV data webPageChannel entry with id:'{fakeWebpageChannel.Id}' doesn't contain url. Skipping the entry.");
                            continue;
                        }

                        var orgTuple = organizations.Find(x => x.Item1 == fakeWebpageChannel.Organization);

                        if (orgTuple == null)
                        {
                            logger.LogError($"Fake PTV webPageChannel, organization with id '{fakeWebpageChannel.Organization}' not found from organizations list. Cannot add the webpage channel entry with id: {fakeWebpageChannel.Id}.");
                            continue;
                        }

                        // create new service channel for webpage type
                        var sc = new ServiceChannelVersioned
                        {
                            Id = Guid.NewGuid(),
                            Type = systemValues.ServiceChannelTypes.Webpage,
                            Charge = fakeWebpageChannel.Charge,
                            OrganizationId = orgTuple.Item2,
                            PublishingStatus = systemValues.PublishingStatuses.DraftStatusType,
                            UnificRoot = new ServiceChannel { Id = Guid.NewGuid()},
                            ConnectionType = defaultConnectionType
                        };
                        SetCreatedInfo(sc);
                        channelRepo.Add(sc);

                        // create new webpage channel
                        var webChannel = new WebpageChannel
                        {
                            Id = Guid.NewGuid(),
                            ServiceChannelVersioned = sc
                        };
                        SetCreatedInfo(webChannel);
                        webChannelRepo.Add(webChannel);

                        // add channels url
                        webpageUrlRepo.Add(new WebpageChannelUrl
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
//                            Url = fakeWebpageChannel.Url,
                            WebpageChannel = webChannel
                        });

                        // channel name
                        nameRepo.Add(new ServiceChannelName
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
                            Name = fakeWebpageChannel.Name,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.NameTypes.Name
                        });

                        // source channel has alternate names but those don't belong to channels

                        // Descriptions, should we only add not null and not empty descriptions?

                        // channel description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(fakeWebpageChannel.Description),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.Description
                        });

                        // channel short description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(fakeWebpageChannel.ShortDescription),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.ShortDescription
                        });

                        // channel support: phone, email, phoneCharge description (phoneServiceChargeInformation)
                        // Create support phone
                        var phone = new Phone
                        {
                            Number = GetTrimmedText(fakeWebpageChannel.Phone, 20),
                            ChargeType = systemValues.ServiceChargeTypes.Charged,
                            Type = systemValues.PhoneNumberTypes.Phone,
                            Localization = systemValues.DefaultLanguage,
                            ChargeDescription = fakeWebpageChannel.PhoneServiceChargeInformation
                        };

                        SetCreatedInfo(phone);
                        phoneRepo.Add(phone);

                        var serviceChannelPhone = new ServiceChannelPhone
                        {
                            Phone = phone,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(serviceChannelPhone);
                        serviceChannelPhoneRepo.Add(serviceChannelPhone);

                        // Create support email
                        var email = new Email
                        {
                            Value = fakeWebpageChannel.Email,
                            Localization = systemValues.DefaultLanguage
                        };

                        SetCreatedInfo(email);
                        emailRepo.Add(email);

                        var serviceChannelEmail = new ServiceChannelEmail
                        {
                            Email = email,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(serviceChannelEmail);
                        serviceChannelEmailRepo.Add(serviceChannelEmail);

                        // Add attachments
                        if (fakeWebpageChannel.Attachments != null && fakeWebpageChannel.Attachments.Count > 0)
                        {
                            foreach (var fakeAttachment in fakeWebpageChannel.Attachments)
                            {
                                // create attachment and the add it to the channel attachment repo
                                var att = new Attachment
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = DefaultCreatedBy,
                                    Description = GetTrimmedText(fakeAttachment.Description, 150),
                                    Id = Guid.NewGuid(),
                                    Localization = systemValues.DefaultLanguage,
                                    Name = fakeAttachment.Name,
                                    //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                                    Url = fakeAttachment.Url
                                };
                                attachmentRepo.Add(att);

                                webAttachmentRepo.Add(new ServiceChannelAttachment
                                {
                                    Attachment = att,
                                    Created = DateTime.UtcNow,
                                    CreatedBy = DefaultCreatedBy,
                                    ServiceChannelVersioned = webChannel.ServiceChannelVersioned
                                });
                            }
                        }

                        // channel languages
                        if (fakeWebpageChannel.Languages != null && fakeWebpageChannel.Languages.Count > 0)
                        {
                            AddChannelLanguages(systemValues.Languages, fakeWebpageChannel.Languages, sc, channelLangRepo, $"Webpage channel, source entity id: '{fakeWebpageChannel.Id}'");
                        }

                        var channelIdentifier = $"Webpage channel source id: {fakeWebpageChannel.Id}.";

                        // link target group
                        ConnectChannelTargetGroups(sc, scTargetGroupRepo, targetGroups, fakeWebpageChannel.TargetGroups, channelIdentifier);

                        // link service class / currently source doesn't have service classes defined for webpage channels
                        //ConnectChannelServiceClasses(sc, scServiceClassRepo, serviceClasses, fakeWebpageChannel.ServiceClasses, channelIdentifier);

                        // link ontology term
                        ConnectChannelOntologyTerms(sc, scOntologyRepo, ontologyTerms, fakeWebpageChannel.OntologyTerms, channelIdentifier);

                        // link or create new keyword
                        ConnectOrCreateChannelKeywords(sc, scKeywordRepo, kwRepo, keywords, fakeWebpageChannel.Keywords, channelIdentifier, systemValues);

                        addedWebpageChannels.Add(Tuple.Create(fakeWebpageChannel.Id, sc.UnificRoot.Id));
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }

            return addedWebpageChannels;
        }

        private List<Tuple<int, Guid>> ImportElectronicChannels(List<Tuple<int, Guid>> organizations)
        {

            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            if (organizations.Count == 0)
            {
                throw new ArgumentException("There are no organizations provided. The electronic channels cannot be imported without organizations.", nameof(organizations));
            }

            var addedElectronicChannels = new List<Tuple<int, Guid>>();

            var srcEChannels = serviceProvider.GetService<IFakePtvRepository>().GetElectronicTransactionServices();

            if (srcEChannels == null || srcEChannels.Count == 0)
            {
                logger.LogWarning("There are no electronic channels to import from fake PTV repository.");
                return addedElectronicChannels;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var systemValues = GetSystemValues(unitOfWork);
                    var targetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().ToList();
                    var ontologyTerms = unitOfWork.CreateRepository<IOntologyTermRepository>().All().ToList();
                    var keywords = unitOfWork.CreateRepository<IKeywordRepository>().All().ToList();

                    // get existing service channel names for channels that are electronic channels
                    // get every channels channelNames in the last query (many channels and each can have many names)
                    var existingEChannelNames = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(sc => sc.TypeId == systemValues.ServiceChannelTypes.EChannel.Id),
                        includes => includes.Include(sc => sc.ServiceChannelNames)).ToList().SelectMany(channel => channel.ServiceChannelNames.Select(scn => scn.Name)).ToList();


                    var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var eChannelRepo = unitOfWork.CreateRepository<IElectronicChannelRepository>();
                    var eChannelUrlRepo = unitOfWork.CreateRepository<IElectronicChannelUrlRepository>();

                    // attachments
                    var eChannelAttRepo = unitOfWork.CreateRepository<IServiceChannelAttachmentRepository>();
                    var attachmentRepo = unitOfWork.CreateRepository<IAttachmentRepository>();

                    var nameRepo = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                    var descRepo = unitOfWork.CreateRepository<IServiceChannelDescriptionRepository>();

                    var phoneRepo = unitOfWork.CreateRepository<IPhoneRepository>();
                    var serviceChannelPhoneRepo = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                    var emailRepo = unitOfWork.CreateRepository<IEmailRepository>();
                    var serviceChannelEmailRepo = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();

                    var channelLangRepo = unitOfWork.CreateRepository<IServiceChannelLanguageRepository>();

                    var scTargetGroupRepo = unitOfWork.CreateRepository<IServiceChannelTargetGroupRepository>();
                    var scOntologyRepo = unitOfWork.CreateRepository<IServiceChannelOntologyTermRepository>();
                    var scKeywordRepo = unitOfWork.CreateRepository<IServiceChannelKeywordRepository>();
                    var kwRepo = unitOfWork.CreateRepository<IKeywordRepository>();

                    var defaultConnectionType = unitOfWork.CreateRepository<IServiceChannelConnectionTypeRepository>()
                        .All().FirstOrDefault(x => x.Code == ServiceChannelConnectionTypeEnum.NotCommon.ToString());

                    foreach (var fakeEChannel in srcEChannels)
                    {
                        // console progressbar
                        System.Console.Write("#");

                        if (existingEChannelNames.Contains(fakeEChannel.Name))
                        {
                            logger.LogWarning($"eChannel name '{fakeEChannel.Name}' already exists in the database. Possible duplicate from fake PTV data.");
                        }

                        if (string.IsNullOrWhiteSpace(fakeEChannel.Url))
                        {
                            logger.LogWarning($"Fake PTV data eChannel entry with id:'{fakeEChannel.Id}' doesn't contain url. Skipping the entry.");
                            continue;
                        }

                        var orgTuple = organizations.Find(x => x.Item1 == fakeEChannel.Organization);

                        if (orgTuple == null)
                        {
                            logger.LogError($"Fake PTV eChannel, organization with id '{fakeEChannel.Organization}' not found from organizations list. Cannot add the eChannel entry with id: {fakeEChannel.Id}.");
                            continue;
                        }

                        // create new service channel for eChannel type
                        var sc = new ServiceChannelVersioned
                        {
                            Id = Guid.NewGuid(),
                            Type = systemValues.ServiceChannelTypes.EChannel,
                            Charge = fakeEChannel.Charge,
                            OrganizationId = orgTuple.Item2,
                            PublishingStatus = systemValues.PublishingStatuses.DraftStatusType,
                            UnificRoot = new ServiceChannel { Id = Guid.NewGuid() },
                            ConnectionType = defaultConnectionType
                        };
                        SetCreatedInfo(sc);
                        channelRepo.Add(sc);

                        // create new eChannel
                        var ec = new ElectronicChannel
                        {
                            Id = Guid.NewGuid(),
                            ServiceChannelVersioned = sc,
                            RequiresAuthentication = fakeEChannel.RequiresAuthentication,
                            RequiresSignature = fakeEChannel.RequiresSignature,
                            SignatureQuantity = fakeEChannel.RequiresSignature ? 1 : 0 // source doesn't have info how many signatures
                        };
                        SetCreatedInfo(ec);
                        eChannelRepo.Add(ec);

                        // add eChannel url
                        eChannelUrlRepo.Add(new ElectronicChannelUrl
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
//                            Url = fakeEChannel.Url,
                            ElectronicChannel = ec
                        });

                        // channel name
                        nameRepo.Add(new ServiceChannelName
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
                            Name = fakeEChannel.Name,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.NameTypes.Name
                        });

                        // Descriptions, should we only add not null and not empty descriptions?

                        // channel description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(string.Join(" ", fakeEChannel.Description, fakeEChannel.ChargeDescription)),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.Description
                        });

                        // channel short description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(fakeEChannel.ShortDescription),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.ShortDescription
                        });

                        // channel support: phone, email, phoneCharge description (phoneServiceChargeInformation)
                        // Create support phone
                        var phone = new Phone
                        {
                            Number = GetTrimmedText(fakeEChannel.Phone, 20),
                            ChargeType = systemValues.ServiceChargeTypes.Charged,
                            Type = systemValues.PhoneNumberTypes.Phone,
                            Localization = systemValues.DefaultLanguage,
                            ChargeDescription = fakeEChannel.PhoneServiceChargeInformation
                        };

                        SetCreatedInfo(phone);
                        phoneRepo.Add(phone);

                        var serviceChannelPhone = new ServiceChannelPhone
                        {
                            Phone = phone,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(serviceChannelPhone);
                        serviceChannelPhoneRepo.Add(serviceChannelPhone);

                        // Create support email
                        var email = new Email
                        {
                            Value = fakeEChannel.Email,
                            Localization = systemValues.DefaultLanguage
                        };

                        SetCreatedInfo(email);
                        emailRepo.Add(email);

                        var serviceChannelEmail = new ServiceChannelEmail
                        {
                            Email = email,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(serviceChannelEmail);
                        serviceChannelEmailRepo.Add(serviceChannelEmail);

                        // Add attachments
                        if (fakeEChannel.Attachments != null && fakeEChannel.Attachments.Count > 0)
                        {
                            foreach (var fakeAttachment in fakeEChannel.Attachments)
                            {
                                // create attachment and the add it to the channel attachment repo
                                var att = new Attachment
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = DefaultCreatedBy,
                                    Description = GetTrimmedText(fakeAttachment.Description, 150),
                                    Id = Guid.NewGuid(),
                                    Localization = systemValues.DefaultLanguage,
                                    Name = fakeAttachment.Name,
                                    //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                                    Url = fakeAttachment.Url
                                };
                                attachmentRepo.Add(att);

                                eChannelAttRepo.Add(new ServiceChannelAttachment
                                {
                                    Attachment = att,
                                    Created = DateTime.UtcNow,
                                    CreatedBy = DefaultCreatedBy,
                                    ServiceChannelVersioned = ec.ServiceChannelVersioned
                                });
                            }
                        }

                        // channel languages
                        if (fakeEChannel.Languages != null && fakeEChannel.Languages.Count > 0)
                        {
                            AddChannelLanguages(systemValues.Languages, fakeEChannel.Languages, sc, channelLangRepo, $"Electronic channel, source entity id: '{fakeEChannel.Id}'");
                        }

                        var channelIdentifier = $"Electronic channel source id: {fakeEChannel.Id}.";

                        // link target group
                        ConnectChannelTargetGroups(sc, scTargetGroupRepo, targetGroups, fakeEChannel.TargetGroups, channelIdentifier);

                        // link service class / currently not defined in source
                        //ConnectChannelServiceClasses(sc, scServiceClassRepo, serviceClasses, fakeEChannel.ServiceClasses, channelIdentifier);

                        // link ontology term
                        ConnectChannelOntologyTerms(sc, scOntologyRepo, ontologyTerms, fakeEChannel.OntologyTerms, channelIdentifier);

                        // link or create new keyword
                        ConnectOrCreateChannelKeywords(sc, scKeywordRepo, kwRepo, keywords, fakeEChannel.Keywords, channelIdentifier, systemValues);

                        addedElectronicChannels.Add(Tuple.Create(fakeEChannel.Id, sc.UnificRoot.Id));
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }

            return addedElectronicChannels;
        }

        private List<Tuple<int, Guid>> ImportServiceLocationChannels(List<Tuple<int, Guid>> organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            if (organizations.Count == 0)
            {
                throw new ArgumentException("There are no organizations provided. The service location channels cannot be imported without organizations.", nameof(organizations));
            }

            var addedServiceLocationChannels = new List<Tuple<int, Guid>>();

            var srcServiceLocationChannels = serviceProvider.GetService<IFakePtvRepository>().GetOffices();

            if (srcServiceLocationChannels == null || srcServiceLocationChannels.Count == 0)
            {
                logger.LogWarning("There are no service location channels to import from fake PTV repository.");
                return addedServiceLocationChannels;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var systemValues = GetSystemValues(unitOfWork);
                    // get existing service channel names for channels that are service location channels
                    // get every channels channelNames in the last query (many channels and each can have many names)
                    var existingSlChannelNames = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(sc => sc.TypeId == systemValues.ServiceChannelTypes.ServiceLocation.Id),
                        includes => includes.Include(sc => sc.ServiceChannelNames)).ToList().SelectMany(channel => channel.ServiceChannelNames.Select(scn => scn.Name)).ToList();

                    var targetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().ToList();
                    var serviceClasses = unitOfWork.CreateRepository<IServiceClassRepository>().All().ToList();
                    var ontologyTerms = unitOfWork.CreateRepository<IOntologyTermRepository>().All().ToList();
                    var keywords = unitOfWork.CreateRepository<IKeywordRepository>().All().ToList();
                    var municipalities = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IMunicipalityRepository>().All(), i => i.Include(j => j.MunicipalityNames)).ToList();
                    var postalCodes = unitOfWork.CreateRepository<IPostalCodeRepository>().All().ToList();
                    var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
//LGE-SLCA                    var slChannelRepo = unitOfWork.CreateRepository<IServiceLocationChannelRepository>();
                    var nameRepo = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                    var descRepo = unitOfWork.CreateRepository<IServiceChannelDescriptionRepository>();
                    var scTargetGroupRepo = unitOfWork.CreateRepository<IServiceChannelTargetGroupRepository>();
                    var scOntologyRepo = unitOfWork.CreateRepository<IServiceChannelOntologyTermRepository>();
                    var scServiceClassRepo = unitOfWork.CreateRepository<IServiceChannelServiceClassRepository>();
                    var scKeywordRepo = unitOfWork.CreateRepository<IServiceChannelKeywordRepository>();
                    var kwRepo = unitOfWork.CreateRepository<IKeywordRepository>();
                    var scServiceHoursRepo = unitOfWork.CreateRepository<IServiceChannelServiceHoursRepository>();
                    var serviceHoursRepo = unitOfWork.CreateRepository<IServiceHoursRepository>();
                    var shAdditionalInformationRepo = unitOfWork.CreateRepository<IServiceHoursAdditionalInformationRepository>();

                    // address repos
                    var addressRepo = unitOfWork.CreateRepository<IAddressRepository>();
                    var poBoxAddrRepo = unitOfWork.CreateRepository<IPostOfficeBoxNameRepository>();
                    var clsAddressPointRepo = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                    var subAddrPoRepo = unitOfWork.CreateRepository<IAddressPostOfficeBoxRepository>();
                    var clsAddressStreetNameRepo = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
                    //var addressTypeStreet = addressTypeRepo.All().FirstOrDefault(i => i.Code.ToLower() == AddressTypeEnum.Street.ToString());
                    //var addressTypePostBox = addressTypeRepo.All().FirstOrDefault(i => i.Code.ToLower() == AddressTypeEnum.PostOfficeBox.ToString());
                    var postalCodeRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
                    var postalCodeNameRepo = unitOfWork.CreateRepository<IPostalCodeNameRepository>();
                    var addressAdditionalRepo = unitOfWork.CreateRepository<IAddressAdditionalInformationRepository>();

                    // web page repos
                    var channelWebpageRepo = unitOfWork.CreateRepository<IServiceChannelWebPageRepository>();
                    var webpageRepo = unitOfWork.CreateRepository<IWebPageRepository>();

                    var channelLangRepo = unitOfWork.CreateRepository<IServiceChannelLanguageRepository>();

                    // email reps
                    var serviceChannelEmailRepo = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();
                    var emailRepo = unitOfWork.CreateRepository<IEmailRepository>();

                    // phone reps
                    var serviceChannelPhoneRepo = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                    var phoneRepo = unitOfWork.CreateRepository<IPhoneRepository>();

                    var defaultConnectionType = unitOfWork.CreateRepository<IServiceChannelConnectionTypeRepository>()
                        .All().FirstOrDefault(x => x.Code == ServiceChannelConnectionTypeEnum.NotCommon.ToString());

                    foreach (var fakeSlChannel in srcServiceLocationChannels)
                    {
                        // console progressbar
                        System.Console.Write("#");

                        if (existingSlChannelNames.Contains(fakeSlChannel.Name))
                        {
                            logger.LogWarning($"ImportServiceLocationChannels, service location channel name '{fakeSlChannel.Name}' already exists in the database. Possible duplicate from fake PTV data.");
                        }

                        var orgTuple = organizations.Find(x => x.Item1 == fakeSlChannel.Organization);

                        if (orgTuple == null)
                        {
                            logger.LogError($"ImportServiceLocationChannels, fake PTV service location channel, organization with id '{fakeSlChannel.Organization}' not found from organizations list. Cannot add the service location channel entry with id: {fakeSlChannel.Id}.");
                            continue;
                        }

                        // create new service channel for service location channel type
                        var sc = new ServiceChannelVersioned
                        {
                            UnificRoot = new ServiceChannel { Id = Guid.NewGuid() },
                            Id = Guid.NewGuid(),
                            Type = systemValues.ServiceChannelTypes.ServiceLocation,
                            Charge = false,
                            OrganizationId = orgTuple.Item2,
                            PublishingStatus = systemValues.PublishingStatuses.DraftStatusType,
                            ConnectionType = defaultConnectionType
                        };
                        SetCreatedInfo(sc);
                        channelRepo.Add(sc);

                        // get service location email and phone number
                        // source can have many but PTV has only one number and one email
                        string srcPhoneNumber = null;
                        string srcPhoneServiceChargeDescription = null;
                        string srcEmail = null;

                        if (fakeSlChannel.PhoneNumbers != null && fakeSlChannel.PhoneNumbers.Count > 0)
                        {
                            if (fakeSlChannel.PhoneNumbers.Count > 1)
                            {
                                logger.LogWarning($"ImportServiceLocationChannels, fake PTV service location channel with id '{fakeSlChannel.Id}' has more than one phone number. Importing the first one.");
                            }

                            var srcPhone = fakeSlChannel.PhoneNumbers.First();
                            srcPhoneNumber = srcPhone.Phone;
                            srcPhoneServiceChargeDescription = srcPhone.PhoneCallFee;
                        }

                        if (fakeSlChannel.Emails != null && fakeSlChannel.Emails.Count > 0)
                        {
                            if (fakeSlChannel.Emails.Count > 1)
                            {
                                logger.LogWarning($"ImportServiceLocationChannels, fake PTV service location channel with id '{fakeSlChannel.Id}' has more than one email address. Importing the first one.");
                            }

                            srcEmail = fakeSlChannel.Emails.First().Address;
                        }

                        // create new service location channel
//LGE-SLCA                        ServiceLocationChannel slc = new ServiceLocationChannel
//LGE-SLCA                        {
//LGE-SLCA                            CoordinatesSetManually = fakeSLChannel.CoordinatesSetManually,
//LGE-SLCA                            CoordinateSystem = fakeSLChannel.CoordinateSystem,
//LGE-SLCA                            Id = Guid.NewGuid(),
//LGE-SLCA                            Latitude = fakeSLChannel.Latitude,
//LGE-SLCA                            Longitude = fakeSLChannel.Longitude,
//LGE-SLCA                            PhoneServiceCharge = srcPhoneServiceCharge,
//LGE-SLCA                            ServiceChannelVersioned = sc
//LGE-SLCA                        };

                        // email
                        var email = new Email
                        {
                            Localization = systemValues.DefaultLanguage,
                            Value = srcEmail
                        };

                        // service channel email
                        var serviceChannelEmail = new ServiceChannelEmail
                        {
                            Email = email,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(email);
                        SetCreatedInfo(serviceChannelEmail);
                        sc.Emails.Add(serviceChannelEmail);
                        serviceChannelEmailRepo.Add(serviceChannelEmail);
                        emailRepo.Add(email);

                        // fax
                        var phoneFax = new Phone
                        {
                            Type = systemValues.PhoneNumberTypes.Fax,
                            Localization = systemValues.DefaultLanguage,
                            Number = GetTrimmedText("000" + fakeSlChannel.Fax, 20),
                            ChargeType = systemValues.ServiceChargeTypes.Other
                        };

                        var serviceChannelPhoneFax = new ServiceChannelPhone
                        {
                            Phone = phoneFax,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(phoneFax);
                        SetCreatedInfo(serviceChannelPhoneFax);
                        sc.Phones.Add(serviceChannelPhoneFax);
                        serviceChannelPhoneRepo.Add(serviceChannelPhoneFax);
                        phoneRepo.Add(phoneFax);

                        // phone
                        var phonePhone = new Phone
                        {
                            Type = systemValues.PhoneNumberTypes.Phone,
                            Localization = systemValues.DefaultLanguage,
                            Number = GetTrimmedText("111" + srcPhoneNumber, 20),
                            ChargeType = systemValues.ServiceChargeTypes.Other,
                            ChargeDescription = srcPhoneServiceChargeDescription
                        };

                        SetCreatedInfo(phonePhone);

                        var serviceChannelPhonePhone = new ServiceChannelPhone
                        {
                            Phone = phonePhone,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(serviceChannelPhonePhone);
                        sc.Phones.Add(serviceChannelPhonePhone);
                        serviceChannelPhoneRepo.Add(serviceChannelPhonePhone);
                        phoneRepo.Add(phonePhone);

//LGE-SLCA                        ImportTask.SetCreatedInfo(slc);
//LGE-SLCA                        slChannelRepo.Add(slc);

                        // channel name
                        nameRepo.Add(new ServiceChannelName
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
                            Name = fakeSlChannel.Name,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.NameTypes.Name
                        });
                        // Descriptions, should we only add not null and not empty descriptions?

                        // channel description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(fakeSlChannel.Description),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.Description
                        });

                        // channel short description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(fakeSlChannel.ShortDescription),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.ShortDescription
                        });

                        // service channel web pages
                        if (fakeSlChannel.Webpages != null && fakeSlChannel.Webpages.Count > 0)
                        {
                            foreach (var fakeWebPage in fakeSlChannel.Webpages)
                            {
                                if (!string.IsNullOrWhiteSpace(fakeWebPage.Url))
                                {
                                    var wp = new WebPage
                                    {
                                        Id = Guid.NewGuid(),
                                        //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                                        Url = fakeWebPage.Url
                                    };
                                    SetCreatedInfo(wp);
                                    webpageRepo.Add(wp);

                                    var serviceChannelWebPage = new ServiceChannelWebPage
                                    {
                                        ServiceChannelVersioned = sc,
                                        Localization = systemValues.DefaultLanguage,
                                        Name = fakeWebPage.GetWebpageName(),
                                        Type = GetWebPageType(systemValues, fakeWebPage.WebpageType),
                                        WebPage = wp
                                    };
                                    SetCreatedInfo(serviceChannelWebPage);
                                    channelWebpageRepo.Add(serviceChannelWebPage);
                                }
                            }
                        }

                        // service location channel visiting addresses
                        if (fakeSlChannel.VisitAddresses != null && fakeSlChannel.VisitAddresses.Count > 0)
                        {
                            foreach (var va in fakeSlChannel.VisitAddresses)
                            {
                                // municipality can be null
                                var vaMunicipality = GetMunicipalityFromFakePtvMunicipalityString(municipalities, va.Municipality, systemValues);
                                var vaPostalCode = postalCodes.Find(x => x.Code == va.PostalCode);

                                if (vaPostalCode == null)
                                {
                                    logger.LogWarning($"ImportServiceLocationChannels, VisitAddress, creating new postal code '{va.PostalCode}' ({va.PostalDistrict}) because the postal code doesn't exist.  Service location channel id: '{fakeSlChannel.Id}'.");
                                    vaPostalCode = new PostalCode
                                    {
                                        Code = va.PostalCode,
                                        Id = Guid.NewGuid(),
                                    };
                                    SetCreatedInfo(vaPostalCode);
                                    postalCodeRepo.Add(vaPostalCode);

                                    var vaPostalCodeName = new PostalCodeName
                                    {
                                        Localization = systemValues.DefaultLanguage,
                                        Name = va.PostalDistrict,
                                        PostalCode = vaPostalCode
                                    };
                                    SetCreatedInfo(vaPostalCodeName);
                                    postalCodeNameRepo.Add(vaPostalCodeName);

                                    // add the created postal code also to the list
                                    postalCodes.Add(vaPostalCode);
                                }

                                var clsAddressStreetName = GetClsAddressStreetName(va.StreetAddress,
                                    vaPostalCode?.MunicipalityId, clsAddressStreetNameRepo);

                                var streetNumberRangeId = streetService.GetStreetNumberRangeId(unitOfWork,
                                    va.BuildingNumber,
                                    clsAddressStreetName.ClsAddressStreetId);
                                var clsAddressPoint = new ClsAddressPoint
                                {
                                    MunicipalityId = vaMunicipality?.Id ?? vaPostalCode.MunicipalityId ?? Guid.Empty,
                                    IsValid = true,
                                    PostalCode = vaPostalCode,
                                    AddressStreet = clsAddressStreetName.ClsAddressStreet,
                                    StreetNumber = va.BuildingNumber,
                                    AddressStreetNumberId = streetNumberRangeId
                                };

                                var addr = new Address
                                {
                                    Type = systemValues.AddressTypes.Street,
                                    Country = systemValues.DefaultCountry,
                                    Id = Guid.NewGuid(),
                                    ClsAddressPoints = new List<ClsAddressPoint>
                                    {
                                        clsAddressPoint
                                    }
                                };
                                clsAddressPoint.Address = addr;
                                SetCreatedInfo(addr);
                                addressRepo.Add(addr);
                                clsAddressPointRepo.Add(clsAddressPoint);

                                var addrAdditional = new AddressAdditionalInformation
                                {
                                    Address = addr,
                                    Text = GetTrimmedText(va.AddressQualifier, 150),
                                    Localization = systemValues.DefaultLanguage
                                };
                                SetCreatedInfo(addrAdditional);
                                addressAdditionalRepo.Add(addrAdditional);
/*LGE-SLCA
                                slcAddressRepo.Add(new ServiceLocationChannelAddress()
                                {
                                    Address = addr,
                                    Created = DateTime.UtcNow,
                                    CreatedBy = ImportTask.DefaultCreatedBy,
                                    ServiceLocationChannel = slc,
                                    Character = systemValues.AddressCharacters.Visiting
                                });
*/
                            }
                        }

                        // service location channel postal addresses
                        if (fakeSlChannel.PostalAddresses != null && fakeSlChannel.PostalAddresses.Count > 0)
                        {
                            foreach (var pa in fakeSlChannel.PostalAddresses)
                            {
                                var paPostalCode = postalCodes.Find(x => x.Code == pa.PostalCode);

                                if (paPostalCode == null)
                                {
                                    logger.LogWarning($"ImportServiceLocationChannels, PostalAddress, creating new postal code '{pa.PostalCode}' ({pa.PostalDistrict}) because the postal code doesn't exist.  Service location channel id: '{fakeSlChannel.Id}'.");

                                    paPostalCode = new PostalCode
                                    {
                                        Code = pa.PostalCode,
                                        Id = Guid.NewGuid()
                                    };
                                    SetCreatedInfo(paPostalCode);
                                    postalCodeRepo.Add(paPostalCode);

                                    var paPostalCodeName = new PostalCodeName
                                    {
                                        Localization = systemValues.DefaultLanguage,
                                        Name = pa.PostalDistrict,
                                        PostalCode = paPostalCode
                                    };
                                    SetCreatedInfo(paPostalCodeName);
                                    postalCodeNameRepo.Add(paPostalCodeName);

                                    // add the created postal code also to the list
                                    postalCodes.Add(paPostalCode);
                                }

                                var addr = new Address
                                {
                                    //AddressPostOfficeBoxes = new List<AddressPostOfficeBox>() { addressPostOfficeBox },
                                    //Type = addressTypePostBox,
                                    Country = systemValues.DefaultCountry,
                                    Id = Guid.NewGuid(),
                                    //PostalCode = paPostalCode,
                                    //PostOfficeBox = pa.PostOfficeBox,
                                };

                                SetCreatedInfo(addr);
                                addressRepo.Add(addr);

                                // add street address for the postal address if it exists in the source
                                var clsAddressStreetName = GetClsAddressStreetName(pa.StreetAddress,
                                    paPostalCode?.MunicipalityId, clsAddressStreetNameRepo);

                                if (clsAddressStreetName != null)
                                {
                                    var streetNumberRangeId = streetService.GetStreetNumberRangeId(unitOfWork,
                                        pa.BuildingNumber,
                                        clsAddressStreetName.ClsAddressStreetId);
                                    var clsAddressPoint = new ClsAddressPoint
                                    {
                                        Address = addr,
                                        MunicipalityId = paPostalCode?.MunicipalityId ?? Guid.Empty,
                                        IsValid = true,
                                        PostalCode = paPostalCode,
                                        AddressStreet = clsAddressStreetName.ClsAddressStreet,
                                        StreetNumber = pa.BuildingNumber,
                                        AddressStreetNumberId = streetNumberRangeId
                                    };

                                    addr.Type = systemValues.AddressTypes.Street;
                                    SetCreatedInfo(clsAddressPoint);
                                    clsAddressPointRepo.Add(clsAddressPoint);
                                }
                                else if (!string.IsNullOrWhiteSpace(pa.PostOfficeBox))
                                {
                                    var addressPostOfficeBox = new AddressPostOfficeBox
                                    {
                                        PostalCode = paPostalCode
                                    };
                                    var postOfficeAddress = new PostOfficeBoxName
                                    {
                                        AddressPostOfficeBox = addressPostOfficeBox,
                                        Localization = systemValues.DefaultLanguage,
                                        Name = pa.PostOfficeBox
                                    };
                                    addressPostOfficeBox.Address = addr;
                                    addr.Type = systemValues.AddressTypes.PostOfficeBox;
                                    addr.AddressPostOfficeBoxes = new List<AddressPostOfficeBox> {addressPostOfficeBox};
                                    SetCreatedInfo(postOfficeAddress);
                                    poBoxAddrRepo.Add(postOfficeAddress);
                                    subAddrPoRepo.Add(addressPostOfficeBox);
                                }
                                else
                                {
                                    throw new Exception("Missing address for Service location channel");
                                }
/*LGE-SLCA                                slcAddressRepo.Add(new ServiceLocationChannelAddress()
                                {
                                    Address = addr,
                                    Created = DateTime.UtcNow,
                                    CreatedBy = ImportTask.DefaultCreatedBy,
                                    ServiceLocationChannel = slc,
                                    Character = systemValues.AddressCharacters.Postal
                                });
*/
                            }
                        }

                        // channel service area -- Not used - moved to ServiceChannelAreaMunicipality
                        //if (fakeSLChannel.ServiceAreas != null && fakeSLChannel.ServiceAreas.Count > 0)
                        //{
                        //    // service are string is a municipality string
                        //    foreach (var sa in fakeSLChannel.ServiceAreas)
                        //    {
                        //        var matchedMunicipality = GetMunicipalityFromFakePtvMunicipalityString(municipalities, sa, systemValues);

                        //        if (matchedMunicipality != null)
                        //        {
                        //            slChannelServiceAreaRepo.Add(new ServiceLocationChannelServiceArea()
                        //            {
                        //                Created = DateTime.UtcNow,
                        //                CreatedBy = ImportTask.DefaultCreatedBy,
                        //                Municipality = matchedMunicipality,
                        //                ServiceLocationChannel = slc
                        //            });
                        //        }
                        //    }
                        //}

                        var channelIdentifier = $"Service location channel source id: {fakeSlChannel.Id}.";

                        // link target group
                        ConnectChannelTargetGroups(sc, scTargetGroupRepo, targetGroups, fakeSlChannel.TargetGroups, channelIdentifier);

                        // link service class
                        ConnectChannelServiceClasses(sc, scServiceClassRepo, serviceClasses, fakeSlChannel.ServiceClasses, channelIdentifier);

                        // link ontology term
                        ConnectChannelOntologyTerms(sc, scOntologyRepo, ontologyTerms, fakeSlChannel.OntologyTerms, channelIdentifier);

                        // link or create new keyword
                        ConnectOrCreateChannelKeywords(sc, scKeywordRepo, kwRepo, keywords, fakeSlChannel.Keywords, channelIdentifier, systemValues);

                        // service hours
                        if (fakeSlChannel.OpeningTimes != null && fakeSlChannel.OpeningTimes.Count > 0)
                        {
                            AddServiceChannelServiceHours(fakeSlChannel.OpeningTimes, systemValues, sc, scServiceHoursRepo, serviceHoursRepo, shAdditionalInformationRepo);
                        }

                        // channel languages
                        if (fakeSlChannel.Languages != null && fakeSlChannel.Languages.Count > 0)
                        {
                            AddChannelLanguages(systemValues.Languages, fakeSlChannel.Languages, sc, channelLangRepo, $"ImportServiceLocation, source entity id: '{fakeSlChannel.Id}'");
                        }

                        addedServiceLocationChannels.Add(Tuple.Create(fakeSlChannel.Id, sc.UnificRoot.Id));
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }

            return addedServiceLocationChannels;
        }

        private static ClsAddressStreetName GetClsAddressStreetName(
            string streetName,
            Guid? municipalityId,
            IClsAddressStreetNameRepository clsAddressStreetNameRepo)
        {
            if (!string.IsNullOrEmpty(streetName))
            {
                return clsAddressStreetNameRepo.All().Include(sn => sn.ClsAddressStreet)
                    .FirstOrDefault(sn =>
                        sn.Name == streetName &&
                        sn.ClsAddressStreet.MunicipalityId == municipalityId);
            }

            return null;
        }

        private List<Tuple<int, Guid>> ImportPrintableFormChannels(List<Tuple<int, Guid>> organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            if (organizations.Count == 0)
            {
                throw new ArgumentException("There are no organizations provided. The printable form channels cannot be imported without organizations.", nameof(organizations));
            }

            var srcPrintableFormChannels = serviceProvider.GetService<IFakePtvRepository>().GetTransactionForms();

            var addedPrintableFormChannels = new List<Tuple<int, Guid>>();

            if (srcPrintableFormChannels == null || srcPrintableFormChannels.Count == 0)
            {
                logger.LogWarning("ImportPrintableFormChannels, there are no printable form channels to import from fake PTV repository.");
                return addedPrintableFormChannels;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var systemValues = GetSystemValues(unitOfWork);
                    var postalCodes = unitOfWork.CreateRepository<IPostalCodeRepository>().All().ToList();
                    var municipalities = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IMunicipalityRepository>().All(), i => i.Include(j => j.MunicipalityNames)).ToList();

                    var targetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().ToList();
                    var ontologyTerms = unitOfWork.CreateRepository<IOntologyTermRepository>().All().ToList();
                    var keywords = unitOfWork.CreateRepository<IKeywordRepository>().All().ToList();

                    // get existing service channel names for channels that are printable form channels
                    // get every channels channelNames in the last query (many channels and each can have many names)
                    var existingPrintableFormChannelNames = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(sc => sc.TypeId == systemValues.ServiceChannelTypes.PrintableForm.Id),
                        includes => includes.Include(sc => sc.ServiceChannelNames)).ToList().SelectMany(channel => channel.ServiceChannelNames.Select(scn => scn.Name)).ToList();

                    var channelRepo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var pfcRepo = unitOfWork.CreateRepository<IPrintableFormChannelRepository>();

                    var nameRepo = unitOfWork.CreateRepository<IServiceChannelNameRepository>();
                    var descRepo = unitOfWork.CreateRepository<IServiceChannelDescriptionRepository>();

                    var phoneRepo = unitOfWork.CreateRepository<IPhoneRepository>();
                    var serviceChannelPhoneRepo = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
                    var emailRepo = unitOfWork.CreateRepository<IEmailRepository>();
                    var serviceChannelEmailRepo = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();

                    // printable form attachment (this is the actual form to print)
                    var pfaRepo = unitOfWork.CreateRepository<IPrintableFormChannelUrlRepository>();

                    // printable form Identifier
                    var pfiRepo = unitOfWork.CreateRepository<IPrintableFormChannelIdentifierRepository>();
                    // printable form Receiver
//LGE-DA                    var pfrRepo = unitOfWork.CreateRepository<IPrintableFormChannelReceiverRepository>();


                    // attachment repos
                    var pfcAttachmentRepo = unitOfWork.CreateRepository<IServiceChannelAttachmentRepository>();
                    var attachmentRepo = unitOfWork.CreateRepository<IAttachmentRepository>();

                    // delivery address
                    var addrRepo = unitOfWork.CreateRepository<IAddressRepository>();
                    var addrAdditionalInfoRepo = unitOfWork.CreateRepository<IAddressAdditionalInformationRepository>();

                    //var streetAddrRepo = unitOfWork.CreateRepository<IStreetNameRepository>();
                    var clsAddressPointRepo = unitOfWork.CreateRepository<IClsAddressPointRepository>();
                    var clsAddressStreetNameRepo = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
                    var poBoxAddrRepo = unitOfWork.CreateRepository<IPostOfficeBoxNameRepository>();
                    var subAddPoBoxAddrRepo = unitOfWork.CreateRepository<IAddressPostOfficeBoxRepository>();
                    //var subAddStreetAddrRepo = unitOfWork.CreateRepository<IAddressStreetRepository>();
                    //var deliveryDescRepo = unitOfWork.CreateRepository<IPrintableFormChannelDeliveryAddressDescriptionRepository>();
                    var postalcodeRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
                    var postalcodeNameRepo = unitOfWork.CreateRepository<IPostalCodeNameRepository>();

                    var channelLangRepo = unitOfWork.CreateRepository<IServiceChannelLanguageRepository>();

                    var scTargetGroupRepo = unitOfWork.CreateRepository<IServiceChannelTargetGroupRepository>();
                    var scOntologyRepo = unitOfWork.CreateRepository<IServiceChannelOntologyTermRepository>();
                    var scKeywordRepo = unitOfWork.CreateRepository<IServiceChannelKeywordRepository>();
                    var kwRepo = unitOfWork.CreateRepository<IKeywordRepository>();

                    var defaultConnectionType = unitOfWork.CreateRepository<IServiceChannelConnectionTypeRepository>()
                        .All().FirstOrDefault(x => x.Code == ServiceChannelConnectionTypeEnum.NotCommon.ToString());

                    foreach (var fakePrintableFormChannel in srcPrintableFormChannels)
                    {
                        // console progressbar
                        System.Console.Write("#");

                        if (existingPrintableFormChannelNames.Contains(fakePrintableFormChannel.Name))
                        {
                            logger.LogWarning($"ImportPrintableFormChannels, printable form channel name '{fakePrintableFormChannel.Name}' already exists in the database. Possible duplicate from fake PTV data.");
                        }

                        var orgTuple = organizations.Find(x => x.Item1 == fakePrintableFormChannel.Organization);

                        if (orgTuple == null)
                        {
                            logger.LogError($"ImportPrintableFormChannels, fake PTV printable form channel, organization with id '{fakePrintableFormChannel.Organization}' not found from organizations list. Cannot add the printable form channel entry with id: {fakePrintableFormChannel.Id}.");
                            continue;
                        }

                        // create new service channel for printable form type
                        var sc = new ServiceChannelVersioned
                        {
                            Id = Guid.NewGuid(),
                            Type = systemValues.ServiceChannelTypes.PrintableForm,
                            Charge = fakePrintableFormChannel.Charge,
                            OrganizationId = orgTuple.Item2,
                            PublishingStatus = systemValues.PublishingStatuses.DraftStatusType,
                            UnificRoot = new ServiceChannel { Id = Guid.NewGuid() },
                            ConnectionType = defaultConnectionType
                        };
                        SetCreatedInfo(sc);
                        channelRepo.Add(sc);

                        // create new printable form channel
                        var pfc = new PrintableFormChannel
                        {
                            Id = Guid.NewGuid(),
                            ServiceChannelVersioned = sc
                        };
                        SetCreatedInfo(pfc);
                        pfcRepo.Add(pfc);

                        var printableFormChannelIdentifier = new PrintableFormChannelIdentifier
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
                            FormIdentifier = fakePrintableFormChannel.FormIdentifier,
                            PrintableFormChannel = pfc
                        };
                        pfiRepo.Add(printableFormChannelIdentifier);

//LGE-DA                        PrintableFormChannelReceiver printableFormChannelReceiver = new PrintableFormChannelReceiver()
//LGE-DA                        {
//LGE-DA                            Created = DateTime.UtcNow,
//LGE-DA                            CreatedBy = ImportTask.DefaultCreatedBy,
//LGE-DA                            Localization = systemValues.DefaultLanguage,
//LGE-DA                            FormReceiver = fakePrintableFormChannel.FormReceiver,
//LGE-DA                            PrintableFormChannel = pfc
//LGE-DA                        };
//LGE-DA                        pfrRepo.Add(printableFormChannelReceiver);



                        // channel name
                        nameRepo.Add(new ServiceChannelName
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Localization = systemValues.DefaultLanguage,
                            Name = fakePrintableFormChannel.Name,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.NameTypes.Name
                        });

                        // source channel has alternate names but those don't belong to channels

                        // Descriptions, should we only add not null and not empty descriptions?

                        // channel description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(string.Join(" ", fakePrintableFormChannel.Description, fakePrintableFormChannel.ChargeDescription)),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.Description
                        });

                        // channel short description
                        descRepo.Add(new ServiceChannelDescription
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = DefaultCreatedBy,
                            Description = GetTrimmedText(fakePrintableFormChannel.ShortDescription),
                            Localization = systemValues.DefaultLanguage,
                            ServiceChannelVersioned = sc,
                            Type = systemValues.DescriptionTypes.ShortDescription
                        });

                        // channel support: phone, email, phoneCharge description (phoneServiceChargeInformation)
                        // Create support phone
                        var phone = new Phone
                        {
                            Number = GetTrimmedText(fakePrintableFormChannel.Phone, 20),
                            ChargeType = systemValues.ServiceChargeTypes.Charged,
                            Type = systemValues.PhoneNumberTypes.Phone,
                            Localization = systemValues.DefaultLanguage,
                            ChargeDescription = fakePrintableFormChannel.PhoneServiceChargeInformation
                        };

                        SetCreatedInfo(phone);
                        phoneRepo.Add(phone);

                        var serviceChannelPhone = new ServiceChannelPhone
                        {
                            Phone = phone,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(serviceChannelPhone);
                        serviceChannelPhoneRepo.Add(serviceChannelPhone);

                        // Create support email
                        var email = new Email
                        {
                            Value = fakePrintableFormChannel.Email,
                            Localization = systemValues.DefaultLanguage
                        };

                        SetCreatedInfo(email);
                        emailRepo.Add(email);

                        var serviceChannelEmail = new ServiceChannelEmail
                        {
                            Email = email,
                            ServiceChannelVersioned = sc
                        };

                        SetCreatedInfo(serviceChannelEmail);
                        serviceChannelEmailRepo.Add(serviceChannelEmail);

                        // add printableFormChannelUrls (the actual forms)
                        if (fakePrintableFormChannel.Links != null && fakePrintableFormChannel.Links.Count > 0)
                        {
                            // links to actual printable forms
                            foreach (var pfLink in fakePrintableFormChannel.Links)
                            {
                                var pForm = new PrintableFormChannelUrl
                                {
                                    Type = GetPrintableFormChannelUrlType(systemValues, pfLink.FileType),
                                    Id = Guid.NewGuid(),
                                    Localization = systemValues.DefaultLanguage,
                                    //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
//                                    Url = pfLink.Url,
                                    PrintableFormChannel = pfc
                                };
                                SetCreatedInfo(pForm);
                                pfaRepo.Add(pForm);
                            }
                        }

                        // form delivery address (postal code is required for address)
                        if (!string.IsNullOrWhiteSpace(fakePrintableFormChannel.DeliveryPostalCode))
                        {
                            var addrPostalCode = postalCodes.Find(x => x.Code == fakePrintableFormChannel.DeliveryPostalCode);

                            if (addrPostalCode == null)
                            {
                                logger.LogWarning($"ImportPrintableFormChannels, missing postal code '{fakePrintableFormChannel.DeliveryPostalCode}' (district '{fakePrintableFormChannel.DeliveryPostalDistrict}') adding it to the system.");
                                addrPostalCode = new PostalCode
                                {
                                    Code = fakePrintableFormChannel.DeliveryPostalCode,
                                    Id = Guid.NewGuid()
                                };
                                SetCreatedInfo(addrPostalCode);
                                postalcodeRepo.Add(addrPostalCode);

                                var addrPostalCodeName = new PostalCodeName
                                {
                                    Localization = systemValues.DefaultLanguage,
                                    Name = fakePrintableFormChannel.DeliveryPostalDistrict,
                                    PostalCode = addrPostalCode
                                };
                                SetCreatedInfo(addrPostalCodeName);
                                postalcodeNameRepo.Add(addrPostalCodeName);


                                postalCodes.Add(addrPostalCode);
                            }

                            // try to get municipality using postalDistrict (can be null)
                            var addrMunicipality = municipalities.Find(x => string.Compare(x.MunicipalityNames.FirstOrDefault(y=> y.Localization.Code == systemValues.DefaultLanguage.Code)?.Name, fakePrintableFormChannel.DeliveryPostalDistrict, StringComparison.OrdinalIgnoreCase) == 0);

                            var addr = new Address
                            {
                                Country = systemValues.DefaultCountry,
                                Id = Guid.NewGuid(),
                               // Municipality = addrMunicipality,
                               // PostalCode = addrPostalCode,
                                //PostOfficeBox = fakePrintableFormChannel.DeliveryPostOfficeBox
                            };
                            SetCreatedInfo(addr);
                            addrRepo.Add(addr);

                            ClsAddressStreetName clsAddressStreetName = null;
                            if (!string.IsNullOrEmpty(fakePrintableFormChannel.DeliveryStreetAddress))
                            {
                                clsAddressStreetName = clsAddressStreetNameRepo.All().Include(sn => sn.ClsAddressStreet)
                                    .FirstOrDefault(sn =>
                                        sn.Name == fakePrintableFormChannel.DeliveryStreetAddress &&
                                        sn.ClsAddressStreet.MunicipalityId == addrMunicipality.Id);
                            }

                            if (clsAddressStreetName != null)
                            {
                                var streetNumberRangeId = streetService.GetStreetNumberRangeId(unitOfWork,
                                    fakePrintableFormChannel.DeliveryStreetAddressNumber,
                                    clsAddressStreetName.ClsAddressStreetId);
                                var clsAddressPoint = new ClsAddressPoint
                                {
                                    Address = addr,
                                    Municipality = addrMunicipality,
                                    IsValid = true,
                                    PostalCode = addrPostalCode,
                                    AddressStreet = clsAddressStreetName.ClsAddressStreet,
                                    StreetNumber = fakePrintableFormChannel.DeliveryStreetAddressNumber,
                                    AddressStreetNumberId = streetNumberRangeId
                                };
                                addr.Type = systemValues.AddressTypes.Street;
                                SetCreatedInfo(clsAddressPoint);
                                clsAddressPointRepo.Add(clsAddressPoint);
                            }
                            else
                            {
                                var addressPostBox = new AddressPostOfficeBox
                                {
                                    Address = addr,
                                    PostalCode = addrPostalCode,
                                    Municipality = addrMunicipality
                                };
                                var postOfficeAddress = new PostOfficeBoxName
                                {
                                    Name = string.Join(" ", fakePrintableFormChannel.DeliveryStreetAddress, fakePrintableFormChannel.DeliveryStreetAddressNumber),
                                    Localization = systemValues.DefaultLanguage,
                                    AddressPostOfficeBox = addressPostBox
                                };
                                addr.Type = systemValues.AddressTypes.PostOfficeBox;
                                SetCreatedInfo(postOfficeAddress);
                                poBoxAddrRepo.Add(postOfficeAddress);
                                subAddPoBoxAddrRepo.Add(addressPostBox);
                            }

                            // connect address to the channel
//LGE-DA                            pfc.DeliveryAddress = addr;

                            // form delivery address description
                            if (!string.IsNullOrWhiteSpace(fakePrintableFormChannel.DeliveryAddressDescription))
                            {
                                var pfcAddrDesc = new AddressAdditionalInformation
                                {
                                    Text = GetTrimmedText(fakePrintableFormChannel.DeliveryAddressDescription, 150),
                                    Localization = systemValues.DefaultLanguage,
                                    Address = addr
                                };
                                SetCreatedInfo(pfcAddrDesc);
                                addrAdditionalInfoRepo.Add(pfcAddrDesc);
                            }
                        }

                        // add attachments
                        if (fakePrintableFormChannel.Attachments != null && fakePrintableFormChannel.Attachments.Count > 0)
                        {
                            foreach (var fakeAttachment in fakePrintableFormChannel.Attachments)
                            {
                                // create attachment and the add it to the channel attachment repo
                                var att = new Attachment
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = DefaultCreatedBy,
                                    Description = GetTrimmedText(fakeAttachment.Description, 150),
                                    Id = Guid.NewGuid(),
                                    Localization = systemValues.DefaultLanguage,
                                    Name = fakeAttachment.Name,
                                    //PublishingStatus = systemValues.PublishingStatuses.PublishedStatusType,
                                    Url = fakeAttachment.Url
                                };
                                attachmentRepo.Add(att);

                                pfcAttachmentRepo.Add(new ServiceChannelAttachment
                                {
                                    Attachment = att,
                                    Created = DateTime.UtcNow,
                                    CreatedBy = DefaultCreatedBy,
                                    ServiceChannelVersioned = pfc.ServiceChannelVersioned
                                });
                            }
                        }

                        // channel languages
                        if (fakePrintableFormChannel.Languages != null && fakePrintableFormChannel.Languages.Count > 0)
                        {
                            AddChannelLanguages(systemValues.Languages, fakePrintableFormChannel.Languages, sc, channelLangRepo, $"ImportPrintableFormChannels, source entity id: '{fakePrintableFormChannel.Id}'");
                        }

                        var channelIdentifier = $"Printable form channel source id: {fakePrintableFormChannel.Id}.";

                        // link target group
                        ConnectChannelTargetGroups(sc, scTargetGroupRepo, targetGroups, fakePrintableFormChannel.TargetGroups, channelIdentifier);

                        // link service class / not implemented in source currently
                        //ConnectChannelServiceClasses(sc, scServiceClassRepo, serviceClasses, fakePrintableFormChannel.ServiceClasses, channelIdentifier);

                        // link ontology term
                        ConnectChannelOntologyTerms(sc, scOntologyRepo, ontologyTerms, fakePrintableFormChannel.OntologyTerms, channelIdentifier);

                        // link or create new keyword
                        ConnectOrCreateChannelKeywords(sc, scKeywordRepo, kwRepo, keywords, fakePrintableFormChannel.Keywords, channelIdentifier, systemValues);

                        addedPrintableFormChannels.Add(Tuple.Create(fakePrintableFormChannel.Id, sc.UnificRoot.Id));
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }

            return addedPrintableFormChannels;
        }

//        /// <summary>
//        /// Imports the fake PTV general descriptions. Returns a list of tuples where item1 value is the fake PTV dara id and item2 value is the new system Guid for the created StatutoryServiceGeneralDescription.
//        /// </summary>
//        /// <param name="systemValues">PTV general system values</param>
//        /// <returns>Returns a list of tuples where item1 value is the fake PTV dara id and item2 value is the new system Guid for the created StatutoryServiceGeneralDescription.</returns>
//        private List<Tuple<int, Guid>> ImportStatutoryGeneralDescriptions(ImportTaskSystemValues systemValues)
//        {
//            if (systemValues == null)
//            {
//                throw new ArgumentNullException(nameof(systemValues));
//            }
//
//            var srcGeneralDescs = _serviceProvider.GetService<IFakePtvRepository>().GetGeneralDescriptions();
//
//            List<Tuple<int, Guid>> addedGeneralDescriptions = new List<Tuple<int, Guid>>();
//
//            if (srcGeneralDescs == null || srcGeneralDescs.Count == 0)
//            {
//                _logger.LogWarning("There are no general descriptions to import from fake PTV.");
//                return addedGeneralDescriptions;
//            }
//
//            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
//            {
//                IContextManager scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
//
//                scopedCtxMgr.ExecuteWriter(unitOfWork =>
//                {
//                    // target groups
//                    List<TargetGroup> targetGroups = null;
//
//                    // service class
//                    List<ServiceClass> serviceClasses = null;
//
//                    // life events
//                    List<LifeEvent> lifeEvents = null;
//
//                    // ontology terms
//                    List<OntologyTerm> ontologyTerms = null;
//
//                    // existing statutoryServiceNames
//                    List<string> existingDescriptionNames = null;
//                    targetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().ToList();
//                    serviceClasses = unitOfWork.CreateRepository<IServiceClassRepository>().All().ToList();
//                    lifeEvents = unitOfWork.CreateRepository<ILifeEventRepository>().All().ToList();
//                    ontologyTerms = unitOfWork.CreateRepository<IOntologyTermRepository>().All().ToList();
//
//                    // get existing statutory service names for general descriptions
//                    existingDescriptionNames = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>().All().Where(e => e.LocalizationId == systemValues.DefaultLanguage.Id).Select(s => s.Name).ToList();
//
//                    var generalDescRepo = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
//                    var serviceNameRepo = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();
//                    var serviceDescRepo = unitOfWork.CreateRepository<IStatutoryServiceDescriptionRepository>();
//                    var serviceLifeEventRepo = unitOfWork.CreateRepository<IStatutoryServiceLifeEventRepository>();
//                    var serviceTargetGroupRepo = unitOfWork.CreateRepository<IStatutoryServiceTargetGroupRepository>();
//                    var serviceServiceClassRepo = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>();
//                    var serviceOntologyTermRepo = unitOfWork.CreateRepository<IStatutoryServiceOntologyTermRepository>();
//
//                    var defaultServiceType =
//                            unitOfWork.CreateRepository<IServiceTypeRepository>()
//                                .All()
//                                .Where(x => x.Code == ServiceTypeEnum.Service.ToString())
//                                .FirstOrDefault();
//
//                    // use only those entries that are marked as statutory
//                    foreach (var genDesc in srcGeneralDescs.Where(m => m.Statutory))
//                    {
//                        if (existingDescriptionNames.Contains(genDesc.Name))
//                        {
//                            // we have to insert the fake ptv general description so that we can add a general description for a service in ImportServices method
//                            _logger.LogWarning($"General description with the same name '{genDesc.Name}' already exists in the database. Possible duplicate from fake PTV data.");
//                        }
//
//                        // create general description
//                        StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned = new StatutoryServiceGeneralDescriptionVersioned()
//                        {
//                            Id = Guid.NewGuid(),
//                            Type = defaultServiceType,
//                            UnificRoot = new StatutoryServiceGeneralDescription() { Id = Guid.NewGuid()}
//                        };
//                        ImportTask.SetCreatedInfo(generalDescriptionVersioned);
//                        generalDescRepo.Add(generalDescriptionVersioned);
//
//                        // create general description name
//                        StatutoryServiceName ssn = new StatutoryServiceName()
//                        {
//                            Localization = systemValues.DefaultLanguage,
//                            Name = genDesc.Name,
//                            StatutoryServiceGeneralDescriptionVersioned = generalDescriptionVersioned,
//                            Type = systemValues.NameTypes.Name
//                        };
//                        ImportTask.SetCreatedInfo(ssn);
//                        serviceNameRepo.Add(ssn);
//
//                        // create description
//                        StatutoryServiceDescription ssDesc = new StatutoryServiceDescription()
//                        {
//                            Description = genDesc.Description,
//                            Localization = systemValues.DefaultLanguage,
//                            StatutoryServiceGeneralDescriptionVersioned = generalDescriptionVersioned,
//                            Type = systemValues.DescriptionTypes.Description
//                        };
//                        ImportTask.SetCreatedInfo(ssDesc);
//                        serviceDescRepo.Add(ssDesc);
//
//                        // create short description
//                        StatutoryServiceDescription ssShortDesc = new StatutoryServiceDescription()
//                        {
//                            Description = genDesc.ShortDescription,
//                            Localization = systemValues.DefaultLanguage,
//                            StatutoryServiceGeneralDescriptionVersioned = generalDescriptionVersioned,
//                            Type = systemValues.DescriptionTypes.ShortDescription
//                        };
//                        ImportTask.SetCreatedInfo(ssShortDesc);
//                        serviceDescRepo.Add(ssShortDesc);
//
//                        // link life event
//                        if (genDesc.LifeEvents != null && genDesc.LifeEvents.Count > 0)
//                        {
//                            foreach (var le in genDesc.LifeEvents)
//                            {
//                                // the source contains null objects in an array
//                                if (!string.IsNullOrWhiteSpace(le))
//                                {
//                                    var matchedLe = lifeEvents.Find(m => string.Compare(m.Label, le, StringComparison.OrdinalIgnoreCase) == 0);
//
//                                    if (matchedLe != null)
//                                    {
//                                        serviceLifeEventRepo.Add(new StatutoryServiceLifeEvent()
//                                        {
//                                            Created = DateTime.UtcNow,
//                                            CreatedBy = ImportTask.DefaultCreatedBy,
//                                            LifeEvent = matchedLe,
//                                            StatutoryServiceGeneralDescriptionVersioned = generalDescriptionVersioned
//                                        });
//                                    }
//                                    else
//                                    {
//                                        _logger.LogWarning($"LifeEvent with name '{le}' not found from PTV database.");
//                                    }
//                                }
//                            }
//                        }
//
//                        // link target group
//                        if (genDesc.TargetGroups != null && genDesc.TargetGroups.Count > 0)
//                        {
//                            foreach (var tg in genDesc.TargetGroups)
//                            {
//                                var matchedTg = targetGroups.Find(m => string.Compare(m.Label, tg, StringComparison.OrdinalIgnoreCase) == 0);
//
//                                if (matchedTg != null)
//                                {
//                                    serviceTargetGroupRepo.Add(new StatutoryServiceTargetGroup()
//                                    {
//                                        Created = DateTime.UtcNow,
//                                        CreatedBy = ImportTask.DefaultCreatedBy,
//                                        StatutoryServiceGeneralDescriptionVersioned = generalDescriptionVersioned,
//                                        TargetGroup = matchedTg
//                                    });
//                                }
//                                else
//                                {
//                                    _logger.LogWarning($"TargetGroup with label '{tg}' not found from PTV database.");
//                                }
//                            }
//                        }
//
//                        // link service class
//                        if (genDesc.ServiceClasses != null && genDesc.ServiceClasses.Count > 0)
//                        {
//                            foreach (var sc in genDesc.ServiceClasses)
//                            {
//                                var matchedSc = serviceClasses.Find(m => string.Compare(m.Label, sc, StringComparison.OrdinalIgnoreCase) == 0);
//
//                                if (matchedSc != null)
//                                {
//                                    serviceServiceClassRepo.Add(new StatutoryServiceServiceClass()
//                                    {
//                                        Created = DateTime.UtcNow,
//                                        CreatedBy = ImportTask.DefaultCreatedBy,
//                                        ServiceClass = matchedSc,
//                                        StatutoryServiceGeneralDescriptionVersioned = generalDescriptionVersioned
//                                    });
//                                }
//                                else
//                                {
//                                    _logger.LogWarning($"ServiceClass with label '{sc}' not found from PTV database.");
//                                }
//                            }
//                        }
//
//                        // link ontology term
//                        if (genDesc.OntologyTerms != null && genDesc.OntologyTerms.Count > 0)
//                        {
//                            foreach (var ot in genDesc.OntologyTerms)
//                            {
//                                var matchedOt = ontologyTerms.Find(m => string.Compare(m.Label, ot, StringComparison.OrdinalIgnoreCase) == 0);
//
//                                if (matchedOt != null)
//                                {
//                                    serviceOntologyTermRepo.Add(new StatutoryServiceOntologyTerm()
//                                    {
//                                        Created = DateTime.UtcNow,
//                                        CreatedBy = ImportTask.DefaultCreatedBy,
//                                        OntologyTerm = matchedOt,
//                                        StatutoryServiceGeneralDescriptionVersioned = generalDescriptionVersioned
//                                    });
//                                }
//                                else
//                                {
//                                    _logger.LogWarning($"OntologyTerm with label '{ot}' not found from PTV database.");
//                                }
//                            }
//                        }
//
//                        addedGeneralDescriptions.Add(new Tuple<int, Guid>(genDesc.Id, generalDescriptionVersioned.UnificRoot.Id));
//                    }
//
//                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
//                });
//            }
//
//            return addedGeneralDescriptions;
//        }

        private List<Tuple<int, Guid>> ImportServices(
            List<Tuple<int, Guid>> phoneChannels,
            List<Tuple<int, Guid>> webpageChannels,
            List<Tuple<int, Guid>> electronicChannels,
            List<Tuple<int, Guid>> serviceLocationChannels,
            List<Tuple<int, Guid>> printableFormChannels,
            List<Tuple<int, Guid>> organizationIdsMap)
        {
            // TODO: serviceWebpage not implemented, current JSON source doesn't have any webpages for services

            // fake ptv services
            var srcServices = serviceProvider.GetService<IFakePtvRepository>().GetServices();

            var repo = serviceProvider.GetService<IFakePtvRepository>();
            var sourceOrganizations = repo.GetOrganizations();

            var addedServices = new List<Tuple<int, Guid>>();

            if (srcServices == null || srcServices.Count == 0)
            {
                logger.LogWarning("ImportServices, there are no services to import from fake PTV.");
                return addedServices;
            }

            // fake ptv general descriptions
            var srcGeneralDescriptions = serviceProvider.GetService<IFakePtvRepository>().GetGeneralDescriptions();


            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var systemValues = GetSystemValues(unitOfWork);

                    var existingKeywords = unitOfWork.CreateRepository<IKeywordRepository>().All().ToList();
                    var existingLifeEvents = unitOfWork.CreateRepository<ILifeEventRepository>().All().ToList();
                    var existingTargetGroups = unitOfWork.CreateRepository<ITargetGroupRepository>().All().ToList();
                    var existingOntologyTerms = unitOfWork.CreateRepository<IOntologyTermRepository>().All().ToList();
                    var existingServiceClasses = unitOfWork.CreateRepository<IServiceClassRepository>().All().ToList();
                    var existingLanguageCodes = unitOfWork.CreateRepository<ILanguageRepository>().All().ToList();
                    var existingStatutoryServiceNames = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>().All().Include(i => i.StatutoryServiceGeneralDescriptionVersioned).ToList();

                    var serviceRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var serviceNameRepo = unitOfWork.CreateRepository<IServiceNameRepository>();
                    var descriptionRepo = unitOfWork.CreateRepository<IServiceDescriptionRepository>();
                    var keywordRepo = unitOfWork.CreateRepository<IKeywordRepository>();
                    var serviceKeywordRepo = unitOfWork.CreateRepository<IServiceKeywordRepository>();
                    var serviceLifeEventRepo = unitOfWork.CreateRepository<IServiceLifeEventRepository>();
                    var serviceTargetGroupRepo = unitOfWork.CreateRepository<IServiceTargetGroupRepository>();
                    //var serviceMunicipalityRepo = unitOfWork.CreateRepository<IServiceMunicipalityRepository>();
                    var serviceOntologyTermRepo = unitOfWork.CreateRepository<IServiceOntologyTermRepository>();
                    var serviceServiceClassRepo = unitOfWork.CreateRepository<IServiceServiceClassRepository>();
                    var serviceServiceChannelRepo = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                    var notificationChannelRepo = unitOfWork.CreateRepository<IServiceElectronicNotificationChannelRepository>();
                    var communicationChannelRepo = unitOfWork.CreateRepository<IServiceElectronicCommunicationChannelRepository>();
                    var serviceRequirementRepo = unitOfWork.CreateRepository<IServiceRequirementRepository>();
                    var serviceLanguageRepo = unitOfWork.CreateRepository<IServiceLanguageRepository>();

                    var defaultServiceType =
                            unitOfWork
                                .CreateRepository<IServiceTypeRepository>()
                                .All()
                                .FirstOrDefault(x => x.Code == ServiceTypeEnum.Service.ToString());

                    var defaultAreaInformationType = unitOfWork
                            .CreateRepository<IAreaInformationTypeRepository>()
                            .All()
                            .FirstOrDefault(x => x.Code == AreaInformationTypeEnum.WholeCountry.ToString());

                    var defaultFundingType = unitOfWork.CreateRepository<IServiceFundingTypeRepository>()
                        .All()
                        .FirstOrDefault(x => x.Code == ServiceFundingTypeEnum.PubliclyFunded.ToString());

                    foreach (var srcSrv in srcServices)
                    {
                        // console progressbar
                        System.Console.Write("#");
                        //get organizationId
                        var srcOrgId = sourceOrganizations.FirstOrDefault(org => org.Services?.Producing != null && org.Services.Producing.Contains(srcSrv.Id))?.Id;
                        var mainOrg = organizationIdsMap.Find(x => x.Item1 == srcOrgId);
                        if (mainOrg == null)
                        {
                            logger.LogError($"ImportServices, cannot connect to main organization. Source service id: '{srcSrv.Id}'.");
                            continue;
                        }

                        // create new service, source currently uses nationwide and regional as coverage type values, correct values are nationwide and local
                        var newServiceVersioned = new ServiceVersioned
                        {
                            UnificRoot = new Service { Id = Guid.NewGuid() },
                            ElectronicCommunication = srcSrv.ElectronicCommunication ?? false,
                            ElectronicNotification = srcSrv.ElectronicNotification ?? false,
                            Id = Guid.NewGuid(),
                            PublishingStatus = systemValues.PublishingStatuses.DraftStatusType,
                            //ServiceCoverageType = string.Compare(srcSrv.ServiceCoverage, CoverageTypeEnum.Nationwide.ToString(), StringComparison.OrdinalIgnoreCase) == 0 ? systemValues.ServiceCoverageTypes.Nationwide : systemValues.ServiceCoverageTypes.Local,
                            Type = defaultServiceType,
                            FundingType = defaultFundingType,
                            AreaInformationType = defaultAreaInformationType,
                            OrganizationId = mainOrg.Item2
                        };

                        if (srcSrv.GeneralDescription.HasValue)
                        {
                            // find the matching id from source general descriptions
                            // ReSharper disable once PossibleInvalidOperationException
                            var srcGenDesc = srcGeneralDescriptions.Find(x => x.Id == srcSrv.GeneralDescription.Value);

                            if (srcGenDesc != null)
                            {
                                // next use the source general descriptions name to try to match system existing statutory service name
                                var matchedSystemName = existingStatutoryServiceNames.Find(x => string.Compare(x.Name, srcGenDesc.Name, StringComparison.OrdinalIgnoreCase) == 0);

                                if (matchedSystemName != null)
                                {
                                    newServiceVersioned.StatutoryServiceGeneralDescriptionId = matchedSystemName.StatutoryServiceGeneralDescriptionVersioned.UnificRootId;
                                    newServiceVersioned.Type = null;
                                }
                                else
                                {
                                    logger.LogError($"ImportServices, cannot connect general description for new service. Source service id: '{srcSrv.Id}', source general description id: '{srcSrv.GeneralDescription.Value}'. No match to system statutory service name using source name: '{srcGenDesc.Name}'.");
                                }
                            }
                            else
                            {
                                logger.LogError($"ImportServices, cannot connect general description for new service. Source service id: '{srcSrv.Id}', source general description id: '{srcSrv.GeneralDescription.Value}'. Source general descriptions don't contain matching general description id.");
                            }
                        }

                        SetCreatedInfo(newServiceVersioned);
                        serviceRepo.Add(newServiceVersioned);

                        // service communication
                        if (!string.IsNullOrWhiteSpace(srcSrv.ElectronicCommunicationChannel))
                        {
                            communicationChannelRepo.Add(new ServiceElectronicCommunicationChannel
                            {
                                Created = DateTime.UtcNow,
                                CreatedBy = DefaultCreatedBy,
                                ElectronicCommunicationChannel = srcSrv.ElectronicCommunicationChannel,
                                Id = Guid.NewGuid(),
                                Localization = systemValues.DefaultLanguage,
                                ServiceVersioned = newServiceVersioned
                            });
                        }

                        // service notification
                        if (!string.IsNullOrWhiteSpace(srcSrv.ElectronicNotificationChannel))
                        {
                            notificationChannelRepo.Add(new ServiceElectronicNotificationChannel
                            {
                                Created = DateTime.UtcNow,
                                CreatedBy = DefaultCreatedBy,
                                ElectronicNotificationChannel = srcSrv.ElectronicNotificationChannel,
                                Id = Guid.NewGuid(),
                                Localization = systemValues.DefaultLanguage,
                                ServiceVersioned = newServiceVersioned
                            });
                        }

                        // service name
                        var sn = new ServiceName
                        {
                            Localization = systemValues.DefaultLanguage,
                            Name = GetTrimmedText(srcSrv.Name, 100),
                            ServiceVersioned = newServiceVersioned,
                            Type = systemValues.NameTypes.Name
                        };
                        SetCreatedInfo(sn);
                        serviceNameRepo.Add(sn);

                        // service alternate names
                        if (srcSrv.AlternateNames != null && srcSrv.AlternateNames.Count > 0)
                        {
                            // source can have many alternate names but our DB allows only one
                            // confirmed from Ninni that we take just the first one
                            var firstAltName = srcSrv.AlternateNames.First();

                            serviceNameRepo.Add(new ServiceName
                            {
                                Created = DateTime.UtcNow,
                                CreatedBy = DefaultCreatedBy,
                                Localization = systemValues.DefaultLanguage,
                                Name = GetTrimmedText(firstAltName, 100),
                                ServiceVersioned = newServiceVersioned,
                                Type = systemValues.NameTypes.AlternateName
                            });
                        }

                        string serviceChargeDescription = null;

                        // service charge description will be appended to long description
                        if (srcSrv.ServiceChargeDescriptions != null && srcSrv.ServiceChargeDescriptions.Count > 0)
                        {
                            serviceChargeDescription = string.Join(" ", srcSrv.ServiceChargeDescriptions).Trim();
                        }

                        // service description
                        var sd = new ServiceDescription
                        {
                            Description = GetTrimmedText(string.IsNullOrWhiteSpace(serviceChargeDescription) ? srcSrv.Description : string.Join(" ", srcSrv.Description, serviceChargeDescription)),
                            Localization = systemValues.DefaultLanguage,
                            ServiceVersioned = newServiceVersioned,
                            Type = systemValues.DescriptionTypes.Description
                        };
                        SetCreatedInfo(sd);
                        descriptionRepo.Add(sd);

                        // service short description
                        var ssd = new ServiceDescription
                        {
                            Description = GetTrimmedText(srcSrv.ShortDescription),
                            Localization = systemValues.DefaultLanguage,
                            ServiceVersioned = newServiceVersioned,
                            Type = systemValues.DescriptionTypes.ShortDescription
                        };
                        SetCreatedInfo(ssd);
                        descriptionRepo.Add(ssd);

                        // service user instruction
                        var sdUserInstruction = new ServiceDescription
                        {
                            Description = GetTrimmedText(srcSrv.ServiceUserInstructions),
                            Localization = systemValues.DefaultLanguage,
                            ServiceVersioned = newServiceVersioned,
                            Type = systemValues.DescriptionTypes.ServiceUserInstruction
                        };
                        SetCreatedInfo(sdUserInstruction);
                        descriptionRepo.Add(sdUserInstruction);

                        // service requirement
                        if (!string.IsNullOrWhiteSpace(srcSrv.Requirements))
                        {
                            var sReq = new ServiceRequirement
                            {
                                Id = Guid.NewGuid(),
                                Localization = systemValues.DefaultLanguage,
                                Requirement = srcSrv.Requirements,
                                ServiceVersioned = newServiceVersioned
                            };
                            SetCreatedInfo(sReq);
                            serviceRequirementRepo.Add(sReq);
                        }

                        // service languages
                        if (srcSrv.Languages != null && srcSrv.Languages.Count > 0)
                        {
                            foreach (var lang in srcSrv.Languages)
                            {
                                var matchedLang = existingLanguageCodes.Find(x => string.Compare(x.Code, lang, StringComparison.OrdinalIgnoreCase) == 0);

                                if (matchedLang != null)
                                {
                                    var sl = new ServiceLanguage
                                    {
                                        Language = matchedLang,
                                        ServiceVersioned = newServiceVersioned
                                    };
                                    SetCreatedInfo(sl);
                                    serviceLanguageRepo.Add(sl);
                                }
                                else
                                {
                                    logger.LogWarning($"ImportServices, cannot match fake PTV source language code '{lang}' to system language code. Source service id: '{srcSrv.Id}'.");
                                }
                            }
                        }

                        // add keywords and/or create if the keyword doesn't exist
                        ConnectOrCreateKeyword(existingKeywords, srcSrv.Keywords, keywordRepo, serviceKeywordRepo, newServiceVersioned, srcSrv.Id, systemValues);

                        // connect matching life events
                        ConnectLifeEvents(existingLifeEvents, srcSrv.LifeEvents, serviceLifeEventRepo, newServiceVersioned, srcSrv.Id);

                        // connect target groups
                        ConnectTargetGroups(existingTargetGroups, srcSrv.TargetGroups, serviceTargetGroupRepo, newServiceVersioned, srcSrv.Id);

                        // connect municipalities
                        //ConnectMunicipalities(existingMunicipalities, srcSrv.Municipalities, serviceMunicipalityRepo, newServiceVersioned, srcSrv.Id, systemValues);

                        // connect ontology term
                        ConnectOntologyTerms(existingOntologyTerms, srcSrv.OntologyTerms, serviceOntologyTermRepo, newServiceVersioned, srcSrv.Id);

                        // connect service classes
                        ConnectServiceClasses(existingServiceClasses, srcSrv.ServiceClasses, serviceServiceClassRepo, newServiceVersioned, srcSrv.Id);

                        // connect service with service channels
                        if (srcSrv.ServiceToServiceChannel != null)
                        {
                            // webpages
                            ConnectChannelToService(srcSrv.ServiceToServiceChannel.ElectronicInformationServices, webpageChannels, newServiceVersioned, serviceServiceChannelRepo, srcSrv.Id, "webpage");

                            // phone
                            ConnectChannelToService(srcSrv.ServiceToServiceChannel.PhoneChannels, phoneChannels, newServiceVersioned, serviceServiceChannelRepo, srcSrv.Id, "phone");

                            // electronic channels
                            ConnectChannelToService(srcSrv.ServiceToServiceChannel.ElectronicTransactionServices, electronicChannels, newServiceVersioned, serviceServiceChannelRepo, srcSrv.Id, "electronic");

                            // service locations
                            ConnectChannelToService(srcSrv.ServiceToServiceChannel.OfficeChannels, serviceLocationChannels, newServiceVersioned, serviceServiceChannelRepo, srcSrv.Id, "serviceLocation");

                            // printable forms
                            ConnectChannelToService(srcSrv.ServiceToServiceChannel.TransactionForms, printableFormChannels, newServiceVersioned, serviceServiceChannelRepo, srcSrv.Id, "printableForm");
                        }

                        addedServices.Add(new Tuple<int, Guid>(srcSrv.Id, newServiceVersioned.Id));
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }

            return addedServices;
        }

        #region helpers

        private string GetTrimmedText(string text, int count = 2500)
        {
            return !string.IsNullOrEmpty(text) && text.Length > count ? text.Substring(0, count) : text;
        }

        /// <summary>
        /// Add languages to a channel.
        /// </summary>
        /// <param name="systemLanguages">list of system languages</param>
        /// <param name="channelLanguages">list of channels languages</param>
        /// <param name="channelVersioned">channel the languages are added to</param>
        /// <param name="channelLanguageRepo">channel language repository</param>
        /// <param name="channelIdentifier">source channel identifier (like: phone channel and json source id 98292)</param>
        private void AddChannelLanguages(List<Language> systemLanguages, List<string> channelLanguages, ServiceChannelVersioned channelVersioned, IServiceChannelLanguageRepository channelLanguageRepo, string channelIdentifier)
        {
            if (systemLanguages == null)
            {
                throw new ArgumentNullException(nameof(systemLanguages));
            }

            if (channelVersioned == null)
            {
                throw new ArgumentNullException(nameof(channelVersioned));
            }

            if (channelLanguageRepo == null)
            {
                throw new ArgumentNullException(nameof(channelLanguageRepo));
            }

            if (channelLanguages == null || channelLanguages.Count == 0)
            {
                logger.LogWarning($"AddChannelLanguages, no languages defined for channel. Channel identifier: '{channelIdentifier}'.");
                return;
            }

            foreach (var lang in channelLanguages)
            {
                var matchedLang = systemLanguages.Find(x => string.Compare(x.Code, lang, StringComparison.OrdinalIgnoreCase) == 0);

                if (matchedLang != null)
                {
                    var scl = new ServiceChannelLanguage
                    {
                        Language = matchedLang,
                        ServiceChannelVersioned = channelVersioned
                    };
                    SetCreatedInfo(scl);
                    channelLanguageRepo.Add(scl);
                }
                else
                {
                    logger.LogWarning($"AddChannelLanguages, cannot match fake PTV source language code '{lang}' to system language code. Channel identifier: '{channelIdentifier}'.");
                }
            }
        }

        private void AddServiceChannelServiceHours(List<SourceOpeningTime> sourceServiceHours, ImportTaskSystemValues systemValues, ServiceChannelVersioned sc,
            IServiceChannelServiceHoursRepository serviceHoursRepo, IServiceHoursRepository shRepo, IServiceHoursAdditionalInformationRepository serviceHoursAdditionalInformationRepo)
        {
            foreach (var ot in sourceServiceHours)
            {
                // there is no info in the source about the ServiceHourType

                var sh = new ServiceHours
                {
                    //Closes = ot.Closes,
                    Id = Guid.NewGuid(),
                    //Opens = ot.Opens,
                    ServiceHourType = systemValues.ServiceHourTypes.Standard,
                };

                var serviceChannelServiceHours = new ServiceChannelServiceHours
                {
                    ServiceChannelVersioned = sc,
                    ServiceHours = sh
                };

                SetCreatedInfo(serviceChannelServiceHours);

                // map days of week the opening times apply
                MapServiceHoursDaysOfWeek(sh, ot);

                // add service hour to repo
                shRepo.Add(sh);
                serviceHoursRepo.Add(serviceChannelServiceHours);

                // if there is some additional information add that
                if (!string.IsNullOrWhiteSpace(ot.OpeningHoursException))
                {
                    serviceHoursAdditionalInformationRepo.Add(new ServiceHoursAdditionalInformation
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        Localization = systemValues.DefaultLanguage,
                        ServiceHours = sh,
                        Text = ot.OpeningHoursException
                    });
                }
            }
        }

        private static bool IsValidServiceHour(string serviceHour)
        {
            if (string.IsNullOrWhiteSpace(serviceHour))
            {
                return false;
            }

            return TimeSpan.TryParse(serviceHour, out _);
        }

        private void AddOrganizationPostalAddress(OrganizationVersioned org, List<SourcePostalAddress> addresses, List<PostalCode> postalCodes, ImportTaskSystemValues systemValues, IUnitOfWorkWritable unitOfWork)
        {
            if (org == null)
            {
                throw new ArgumentNullException(nameof(org));
            }

            if (systemValues == null)
            {
                throw new ArgumentNullException(nameof(systemValues));
            }

            if (addresses == null || addresses.Count == 0)
            {
                return;
            }

            var postalCodeRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
            var postalCodeNameRepo = unitOfWork.CreateRepository<IPostalCodeNameRepository>();
            var addrRepo = unitOfWork.CreateRepository<IAddressRepository>();
            var clsAddressStreetNameRepository = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
            var clsAddressPointRepository = unitOfWork.CreateRepository<IClsAddressPointRepository>();
            var poBoxAddrRepo = unitOfWork.CreateRepository<IPostOfficeBoxNameRepository>();
            var subAddrPostBoxRepo = unitOfWork.CreateRepository<IAddressPostOfficeBoxRepository>();
            var orgAddrRepo = unitOfWork.CreateRepository<IOrganizationAddressRepository>();

            foreach (var pAddr in addresses)
            {
                // fake data contains bad entries, skip those
                if (string.Compare(pAddr.PostalCode, "x", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(pAddr.PostalDistrict, "x", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(pAddr.StreetAddress, "x", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continue;
                }

                // get postalcode and postalDistrict from address (method trims the and makes the district upper case)
                var pcTuple = GetPostalCode(pAddr);

                if (pcTuple != null)
                {
                    // try to get PostalCode from PTV matching the address
                    var addrPostalCode = postalCodes.Find(x => x.Code == pcTuple.Item1);

                    // if the postalcode wasn't found then create it
                    if (addrPostalCode == null)
                    {
                        logger.LogWarning($"AddOrganizationPostalAddress, creating new postal code '{pcTuple.Item1}' ({pcTuple.Item2}).");

                        addrPostalCode = new PostalCode
                        {
                            Code = pcTuple.Item1,
                            Id = Guid.NewGuid()
                        };
                        SetCreatedInfo(addrPostalCode);
                        postalCodeRepo.Add(addrPostalCode);

                        var addrPostalCodeName = new PostalCodeName
                        {
                            Localization = systemValues.DefaultLanguage,
                            Name = pcTuple.Item2,
                            PostalCode = addrPostalCode
                        };
                        SetCreatedInfo(addrPostalCodeName);
                        postalCodeNameRepo.Add(addrPostalCodeName);

                        // add the entry also to the existing postal codes list
                        postalCodes.Add(addrPostalCode);
                    }

                    // create the postal address
                    var addr = new Address
                    {
                        Country = systemValues.DefaultCountry,
                        Id = Guid.NewGuid(),
                        //PostalCode = addrPostalCode,
                        //PostOfficeBox = pAddr.PostOfficeBox
                    };
                    SetCreatedInfo(addr);
                    addrRepo.Add(addr);

                    var clsAddressStreetName = GetClsAddressStreetName(pAddr.StreetAddress,
                        addrPostalCode.MunicipalityId, clsAddressStreetNameRepository);

                    if (clsAddressStreetName != null)
                    {
                        var streetNumberRangeId = streetService.GetStreetNumberRangeId(unitOfWork,
                            pAddr.BuildingNumber,
                            clsAddressStreetName.ClsAddressStreetId);
                        var clsAddressPoint = new ClsAddressPoint
                        {
                            Address = addr,
                            MunicipalityId = addrPostalCode.MunicipalityId ?? Guid.Empty,
                            IsValid = true,
                            PostalCode = addrPostalCode,
                            AddressStreet = clsAddressStreetName.ClsAddressStreet,
                            StreetNumber = pAddr.BuildingNumber,
                            AddressStreetNumberId = streetNumberRangeId
                        };
                        addr.Type = systemValues.AddressTypes.Street;
                        SetCreatedInfo(clsAddressPoint);
                        clsAddressPointRepository.Add(clsAddressPoint);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(pAddr.PostOfficeBox))
                        {
                            var addressPostOfficeBox = new AddressPostOfficeBox
                            {
                                Address = addr,
                                PostalCode = addrPostalCode
                            };
                            var postOfficeAddress = new PostOfficeBoxName
                            {
                                AddressPostOfficeBox = addressPostOfficeBox,
                                Localization = systemValues.DefaultLanguage,
                                Name = pAddr.PostOfficeBox
                            };
                            addr.Type = systemValues.AddressTypes.PostOfficeBox;
                            SetCreatedInfo(postOfficeAddress);
                            poBoxAddrRepo.Add(postOfficeAddress);
                            subAddrPostBoxRepo.Add(addressPostOfficeBox);
                        }
                    }

                    orgAddrRepo.Add(new OrganizationAddress
                    {
                        Address = addr,
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        OrganizationVersioned = org,
                        Character = systemValues.AddressCharacters.Postal,
                    });
                }
            }
        }

        private void AddOrganizationVisitingAddress(OrganizationVersioned org, List<SourceVisitAddress> addresses, List<PostalCode> postalCodes, ImportTaskSystemValues systemValues, IUnitOfWorkWritable unitOfWork)
        {
            if (org == null)
            {
                throw new ArgumentNullException(nameof(org));
            }

            if (postalCodes == null)
            {
                throw new ArgumentNullException(nameof(postalCodes));
            }

            if (systemValues == null)
            {
                throw new ArgumentNullException(nameof(systemValues));
            }

            if (addresses == null || addresses.Count == 0)
            {
                return;
            }

            var postalCodeRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
            var postalCodeNameRepo = unitOfWork.CreateRepository<IPostalCodeNameRepository>();
            var addressAdditionalRepo = unitOfWork.CreateRepository<IAddressAdditionalInformationRepository>();
            var clsAddressStreetNameRepo = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
            var clsAddressPointRepo = unitOfWork.CreateRepository<IClsAddressPointRepository>();
            var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var addrRepo = unitOfWork.CreateRepository<IAddressRepository>();
            var orgAddrRepo = unitOfWork.CreateRepository<IOrganizationAddressRepository>();

            foreach (var pAddr in addresses)
            {
                // fake data contains bad entries, skip those
                if (string.Compare(pAddr.PostalCode, "x", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(pAddr.PostalDistrict, "x", StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(pAddr.StreetAddress, "x", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continue;
                }

                // get postalcode and postalDistrict from address (method trims the and makes the district upper case)
                var pcTuple = GetPostalCode(pAddr);

                if (pcTuple != null)
                {
                    // try to get PostalCode from PTV matching the address
                    var addrPostalCode = postalCodes.Find(x => x.Code == pcTuple.Item1);

                    // if the postalcode wasn't found then create it
                    if (addrPostalCode == null)
                    {
                        logger.LogWarning($"AddOrganizationVisitingAddress, creating new postal code '{pcTuple.Item1}' ({pcTuple.Item2}).");

                        addrPostalCode = new PostalCode
                        {
                            Code = pcTuple.Item1,
                            Id = Guid.NewGuid()
                        };
                        SetCreatedInfo(addrPostalCode);
                        postalCodeRepo.Add(addrPostalCode);

                        var addrPostalCodeName = new PostalCodeName
                        {
                            Localization = systemValues.DefaultLanguage,
                            Name = pcTuple.Item2,
                            PostalCode = addrPostalCode
                        };
                        SetCreatedInfo(addrPostalCodeName);
                        postalCodeNameRepo.Add(addrPostalCodeName);

                        // add also the created postal code to the existing postal codes list
                        postalCodes.Add(addrPostalCode);
                    }

                    // create the visiting address
                    var addr = new Address
                    {
                        Country = systemValues.DefaultCountry,
                        Id = Guid.NewGuid(),
                        Type = systemValues.AddressTypes.Street
                    };
                    SetCreatedInfo(addr);

                    var clsAddressStreetName = GetClsAddressStreetName(pAddr.StreetAddress,
                        addrPostalCode.MunicipalityId, clsAddressStreetNameRepo);

                    var streetNumberRangeId = streetService.GetStreetNumberRangeId(
                        unitOfWork,
                        pAddr.BuildingNumber,
                        clsAddressStreetName.ClsAddressStreetId);
                    var clsAddressPoint = new ClsAddressPoint
                    {
                        Address = addr,
                        MunicipalityId = addrPostalCode.MunicipalityId ?? Guid.Empty,
                        IsValid = true,
                        PostalCode = addrPostalCode,
                        AddressStreet = clsAddressStreetName.ClsAddressStreet,
                        StreetNumber = pAddr.BuildingNumber,
                        AddressStreetNumberId = streetNumberRangeId
                    };

                    var addrAdditional = new AddressAdditionalInformation
                    {
                        Address = addr,
                        Text = GetTrimmedText(pAddr.AddressQualifier, 150),
                        Localization = systemValues.DefaultLanguage,
                    };
                    SetCreatedInfo(addrAdditional);
                    addressAdditionalRepo.Add(addrAdditional);
                    clsAddressPointRepo.Add(clsAddressPoint);

                    //TODO MUNICIPALITY CHANGE add nae

                    if (!string.IsNullOrWhiteSpace(pAddr.Municipality))
                    {
                        var matchedMunicipality = municipalityRepo.All().FirstOrDefault(x => x.MunicipalityNames.Where(y => y.Localization.Code == "fi").Any(y => y.Name  == pAddr.Municipality));

                        if (matchedMunicipality != null)
                        {
                            clsAddressPoint.Municipality = matchedMunicipality;
                        }
                    }

                    addrRepo.Add(addr);

                    orgAddrRepo.Add(new OrganizationAddress
                    {
                        Address = addr,
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        OrganizationVersioned = org,
                        Character = systemValues.AddressCharacters.Visiting
                    });
                }
            }
        }

        private ImportTaskSystemValues GetSystemValues(IUnitOfWork unitOfWork)
        {
            var sysValues = new ImportTaskSystemValues();

            //            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //            {
            //                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
            //
            //                scopedCtxMgr.ExecuteReader(unitOfWork =>
            //                {
            // publishing status types
            var publishingStatuses = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>().All().ToList();
            sysValues.PublishingStatuses.DeletedStatusType = publishingStatuses.Find(x => x.Code == PublishingStatus.Deleted.ToString());
            sysValues.PublishingStatuses.DraftStatusType = publishingStatuses.Find(x => x.Code == PublishingStatus.Draft.ToString());
            sysValues.PublishingStatuses.PublishedStatusType = publishingStatuses.Find(x => x.Code == PublishingStatus.Published.ToString());

            // default language
            var langCode = DomainConstants.DefaultLanguage.ToLower();
            sysValues.DefaultLanguage = unitOfWork.CreateRepository<ILanguageRepository>().All().FirstOrDefault(l => l.Code.ToLower() == langCode);

            // webpage types
            var webpageTypes = unitOfWork.CreateRepository<IWebPageTypeRepository>().All().ToList();
            sysValues.WebpageTypes.Home = webpageTypes.Find(x => x.Code == WebPageTypeEnum.HomePage.ToString());
            sysValues.WebpageTypes.Social = webpageTypes.Find(x => x.Code == WebPageTypeEnum.SocialPage.ToString());

            // try to get FI country NOTE! Currently in DB there is only Finland
            sysValues.DefaultCountry = unitOfWork.CreateRepository<ICountryRepository>().All().FirstOrDefault(e => e.Code.ToLower() == "fi");

            /*ProducerInfo
            var roleTypes = unitOfWork.CreateRepository<IRoleTypeRepository>().All().ToList();
            sysValues.RoleTypes.Producer = roleTypes.Find(x => x.Code == RoleTypeEnum.Producer.ToString());
            sysValues.RoleTypes.Responsible = roleTypes.Find(x => x.Code == RoleTypeEnum.Responsible.ToString());
            */

            // address types
            var addressCharacters = unitOfWork.CreateRepository<IAddressCharacterRepository>().All().ToList();
            sysValues.AddressCharacters.Postal = addressCharacters.Find(x => x.Code == AddressCharacterEnum.Postal.ToString());
            sysValues.AddressCharacters.Visiting = addressCharacters.Find(x => x.Code == AddressCharacterEnum.Visiting.ToString());

            // address types
            var addressTypes = unitOfWork.CreateRepository<IAddressTypeRepository>().All().ToList();
            sysValues.AddressTypes.Street = addressTypes.Find(x => x.Code == AddressTypeEnum.Street.ToString());
            sysValues.AddressTypes.PostOfficeBox = addressTypes.Find(x => x.Code == AddressTypeEnum.PostOfficeBox.ToString());
            sysValues.AddressTypes.Foreign = addressTypes.Find(x => x.Code == AddressTypeEnum.Foreign.ToString());

            // phone number types
            var phoneNumberTypes = unitOfWork.CreateRepository<IPhoneNumberTypeRepository>().All().ToList();
            sysValues.PhoneNumberTypes.Fax = phoneNumberTypes.Find(x => x.Code == PhoneNumberTypeEnum.Fax.ToString());
            sysValues.PhoneNumberTypes.Phone = phoneNumberTypes.Find(x => x.Code == PhoneNumberTypeEnum.Phone.ToString());
            sysValues.PhoneNumberTypes.Sms = phoneNumberTypes.Find(x => x.Code == PhoneNumberTypeEnum.Sms.ToString());

            // name types
            var nameTypes = unitOfWork.CreateRepository<INameTypeRepository>().All().ToList();
            sysValues.NameTypes.AlternateName = nameTypes.Find(x => x.Code == NameTypeEnum.AlternateName.ToString());
            sysValues.NameTypes.Name = nameTypes.Find(x => x.Code == NameTypeEnum.Name.ToString());

            // description types
            var descTypes = unitOfWork.CreateRepository<IDescriptionTypeRepository>().All().ToList();
            sysValues.DescriptionTypes.Description = descTypes.Find(x => x.Code == DescriptionTypeEnum.Description.ToString());
            sysValues.DescriptionTypes.ServiceUserInstruction = descTypes.Find(x => x.Code == DescriptionTypeEnum.ServiceUserInstruction.ToString());
            sysValues.DescriptionTypes.ShortDescription = descTypes.Find(x => x.Code == DescriptionTypeEnum.ShortDescription.ToString());

            // organization types
            var orgTypes = unitOfWork.CreateRepository<IOrganizationTypeRepository>().All().ToList();
            sysValues.OrganizationTypes.Company = orgTypes.Find(x => x.Code == OrganizationTypeEnum.Company.ToString());
            sysValues.OrganizationTypes.Municipality = orgTypes.Find(x => x.Code == OrganizationTypeEnum.Municipality.ToString());
            sysValues.OrganizationTypes.Organization = orgTypes.Find(x => x.Code == OrganizationTypeEnum.Organization.ToString());
            sysValues.OrganizationTypes.RegionalOrganization = orgTypes.Find(x => x.Code == OrganizationTypeEnum.RegionalOrganization.ToString());
            sysValues.OrganizationTypes.State = orgTypes.Find(x => x.Code == OrganizationTypeEnum.State.ToString());

            // service channel types
            var channelTypes = unitOfWork.CreateRepository<IServiceChannelTypeRepository>().All().ToList();
            sysValues.ServiceChannelTypes.EChannel = channelTypes.Find(x => x.Code == ServiceChannelTypeEnum.EChannel.ToString());
            sysValues.ServiceChannelTypes.Phone = channelTypes.Find(x => x.Code == ServiceChannelTypeEnum.Phone.ToString());
            sysValues.ServiceChannelTypes.ServiceLocation = channelTypes.Find(x => x.Code == ServiceChannelTypeEnum.ServiceLocation.ToString());
            sysValues.ServiceChannelTypes.PrintableForm = channelTypes.Find(x => x.Code == ServiceChannelTypeEnum.PrintableForm.ToString());
            sysValues.ServiceChannelTypes.Webpage = channelTypes.Find(x => x.Code == ServiceChannelTypeEnum.WebPage.ToString());

            // service charge types
            var serviceChargeTypes = unitOfWork.CreateRepository<IServiceChargeTypeRepository>().All().ToList();
            sysValues.ServiceChargeTypes.Charged = serviceChargeTypes.Find(x => x.Code == ServiceChargeTypeEnum.Charged.ToString());
            sysValues.ServiceChargeTypes.Free = serviceChargeTypes.Find(x => x.Code == ServiceChargeTypeEnum.Free.ToString());
            sysValues.ServiceChargeTypes.Other = serviceChargeTypes.Find(x => x.Code == ServiceChargeTypeEnum.Other.ToString());

            // service hour types
            var serviceHourTypes = unitOfWork.CreateRepository<IServiceHourTypeRepository>().All().ToList();
            sysValues.ServiceHourTypes.Exception = serviceHourTypes.Find(x => x.Code == ServiceHoursTypeEnum.Exception.ToString());
            sysValues.ServiceHourTypes.Standard = serviceHourTypes.Find(x => x.Code == ServiceHoursTypeEnum.Standard.ToString());

            // exception hours status types
            var exceptHoursStatusTypes = unitOfWork.CreateRepository<IExceptionHoursStatusTypeRepository>().All().ToList();
            sysValues.ExceptionHoursStatusTypes.Closed = exceptHoursStatusTypes.Find(x => x.Code == ExceptionHoursStatus.Closed.ToString());
            sysValues.ExceptionHoursStatusTypes.Open = exceptHoursStatusTypes.Find(x => x.Code == ExceptionHoursStatus.Open.ToString());

            // printable form attachment types
            var pfaTypes = unitOfWork.CreateRepository<IPrintableFormChannelUrlTypeRepository>().All().ToList();
            sysValues.PrintableFormChannelUrlTypes.Excel = pfaTypes.Find(x => x.Code == PrintableFormChannelUrlTypeEnum.Excel.ToString());
            sysValues.PrintableFormChannelUrlTypes.Word = pfaTypes.Find(x => x.Code == PrintableFormChannelUrlTypeEnum.DOC.ToString());
            sysValues.PrintableFormChannelUrlTypes.Pdf = pfaTypes.Find(x => x.Code == PrintableFormChannelUrlTypeEnum.PDF.ToString());

            // coverage type
            //var coverageTypes = unitOfWork.CreateRepository<IServiceCoverageTypeRepository>().All().ToList();
            //sysValues.ServiceCoverageTypes.Local = coverageTypes.Find(x => x.Code == CoverageTypeEnum.Local.ToString());
            //sysValues.ServiceCoverageTypes.Nationwide = coverageTypes.Find(x => x.Code == CoverageTypeEnum.Nationwide.ToString());

            // system languages
            sysValues.Languages = unitOfWork.CreateRepository<ILanguageRepository>().All().ToList();

            // provision types
            var provisionTypes = unitOfWork.CreateRepository<IProvisionTypeRepository>().All().ToList();
            sysValues.ProvisionTypes.Other = provisionTypes.Find(x => x.Code == ProvisionTypeEnum.Other.ToString());
            sysValues.ProvisionTypes.PurchaseServices = provisionTypes.Find(x => x.Code == ProvisionTypeEnum.PurchaseServices.ToString());
            sysValues.ProvisionTypes.SelfProduced = provisionTypes.Find(x => x.Code == ProvisionTypeEnum.SelfProduced.ToString());

            //Area Role Types
            var areaInformationTypes = unitOfWork.CreateRepository<IAreaInformationTypeRepository>().All().ToList();
            sysValues.AreaInformationTypes.WholeCountry = areaInformationTypes.Find(x => x.Code == AreaInformationTypeEnum.WholeCountry.ToString());
            sysValues.AreaInformationTypes.WholeCountryExceptAlandIslands = areaInformationTypes.Find(x => x.Code == AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
            sysValues.AreaInformationTypes.AreaType = areaInformationTypes.Find(x => x.Code == AreaInformationTypeEnum.AreaType.ToString());

            //Area Types
            var areaTypes = unitOfWork.CreateRepository<IAreaTypeRepository>().All().ToList();
            sysValues.AreaTypes.Municipality = areaTypes.Find(x => x.Code == AreaTypeEnum.Municipality.ToString());
            sysValues.AreaTypes.Province = areaTypes.Find(x => x.Code == AreaTypeEnum.Province.ToString());
            sysValues.AreaTypes.BusinessRegions = areaTypes.Find(x => x.Code == AreaTypeEnum.BusinessRegions.ToString());
            sysValues.AreaTypes.HospitalRegions = areaTypes.Find(x => x.Code == AreaTypeEnum.HospitalRegions.ToString());

            var connectionTypes = unitOfWork.CreateRepository<IServiceChannelConnectionTypeRepository>().All().ToList();
            sysValues.ServiceChannelConnectionType.NotCommon = connectionTypes.Find(t => t.Code == ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            sysValues.ServiceChannelConnectionType.CommonForAll = connectionTypes.Find(t => t.Code == ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
            sysValues.ServiceChannelConnectionType.CommonFor = connectionTypes.Find(t => t.Code == ServiceChannelConnectionTypeEnum.CommonFor.ToString());

            if (sysValues.PublishingStatuses.DeletedStatusType == null || sysValues.PublishingStatuses.DraftStatusType == null || sysValues.PublishingStatuses.PublishedStatusType == null)
            {
                throw new Exception("Publishing status or statuses missing from PTV database.");
            }

            if (sysValues.DefaultCountry == null)
            {
                throw new Exception("Default country missing from PTV database.");
            }

            if (sysValues.DefaultLanguage == null)
            {
                throw new Exception("Default language missing from PTV database.");
            }

            if (sysValues.WebpageTypes.Home == null || sysValues.WebpageTypes.Social == null)
            {
                throw new Exception("Webpage type or types missing from PTV database.");
            }

            /*ProducerInfo
            if (sysValues.RoleTypes.Producer == null || sysValues.RoleTypes.Responsible == null)
            {
                throw new Exception("Role type or types missing from PTV database.");
            }
            */

            if (sysValues.AddressCharacters.Postal == null || sysValues.AddressCharacters.Visiting == null)
            {
                throw new Exception("Address type or types missing from PTV database.");
            }

            if (sysValues.PhoneNumberTypes.Fax == null || sysValues.PhoneNumberTypes.Phone == null || sysValues.PhoneNumberTypes.Sms == null)
            {
                throw new Exception("Phone number type or types missing from PTV database.");
            }

            if (sysValues.NameTypes.AlternateName == null || sysValues.NameTypes.Name == null)
            {
                throw new Exception("Name type or types missing from PTV database.");
            }

            if (sysValues.DescriptionTypes.Description == null || sysValues.DescriptionTypes.ServiceUserInstruction == null || sysValues.DescriptionTypes.ShortDescription == null)
            {
                throw new Exception("Description type or types missing from PTV database.");
            }

            if (sysValues.OrganizationTypes.Company == null || sysValues.OrganizationTypes.Municipality == null ||
                sysValues.OrganizationTypes.Organization == null || sysValues.OrganizationTypes.RegionalOrganization == null || sysValues.OrganizationTypes.State == null)
            {
                throw new Exception("Organization type or types missing from PTV database.");
            }

            if (sysValues.ServiceChannelTypes.EChannel == null || sysValues.ServiceChannelTypes.Phone == null || sysValues.ServiceChannelTypes.ServiceLocation == null ||
                sysValues.ServiceChannelTypes.PrintableForm == null || sysValues.ServiceChannelTypes.Webpage == null)
            {
                throw new Exception("Service channel type or types missing from PTV database.");
            }

            if (sysValues.ServiceChargeTypes.Charged == null || sysValues.ServiceChargeTypes.Free == null || sysValues.ServiceChargeTypes.Other == null)
            {
                throw new Exception("Service charge type or types missing from PTV database.");
            }

            if (sysValues.ServiceHourTypes.Exception == null || sysValues.ServiceHourTypes.Standard == null)
            {
                throw new Exception("Service hour type or types missing from PTV database.");
            }

            if (sysValues.ExceptionHoursStatusTypes.Closed == null || sysValues.ExceptionHoursStatusTypes.Open == null)
            {
                throw new Exception("Exception hours status type or types missing from PTV database.");
            }

            if (sysValues.PrintableFormChannelUrlTypes.Excel == null || sysValues.PrintableFormChannelUrlTypes.Pdf == null || sysValues.PrintableFormChannelUrlTypes.Word == null)
            {
                throw new Exception("Printable form attachment type or types missing from PTV database.");
            }

            //if (sysValues.ServiceCoverageTypes.Local == null || sysValues.ServiceCoverageTypes.Nationwide == null)
            //{
            //    throw new Exception("Service coverage type or types missing from PTV database.");
            //}

            if (sysValues.Languages == null)
            {
                throw new Exception("ImportTaskSystemValues.Languages is null.");
            }

            if (sysValues.ProvisionTypes.Other == null || sysValues.ProvisionTypes.PurchaseServices == null ||
                sysValues.ProvisionTypes.SelfProduced == null)
            {
                throw new Exception("Provision type or types missing from PTV database.");
            }

            if (sysValues.AreaInformationTypes.WholeCountry == null || sysValues.AreaInformationTypes.WholeCountryExceptAlandIslands == null ||
                sysValues.AreaInformationTypes.AreaType == null)
            {
                throw new Exception("Area inforation type or types missing from PTV database.");
            }

            if (sysValues.AreaTypes.Municipality == null || sysValues.AreaTypes.Province == null ||
                sysValues.AreaTypes.BusinessRegions == null || sysValues.AreaTypes.HospitalRegions == null)
            {
                throw new Exception("Area type or types missing from PTV database.");
            }

            if (sysValues.ServiceChannelConnectionType.NotCommon == null || sysValues.ServiceChannelConnectionType.CommonForAll == null || sysValues.ServiceChannelConnectionType.CommonFor == null)
            {
                throw new Exception("Service channel connection type or types missing from PTV database.");
            }

            return sysValues;
        }

        private void ConnectServiceToOrganization(List<int?> sourceServiceIds, List<Tuple<int, Guid>> services, /*ProducerInfo RoleType connectionRole, */
            List<SourceServiceEntity> sourceServices, Tuple<int, Guid> orgToConnect, IOrganizationServiceRepository repo)
        {
            if (sourceServiceIds == null || sourceServiceIds.Count == 0)
            {
                return;
            }

            if (services == null || services.Count == 0)
            {
                return;
            }
/*ProducerInfo
            if (connectionRole == null)
            {
                throw new ArgumentNullException(nameof(connectionRole));
            }*/

            if (orgToConnect == null)
            {
                throw new ArgumentNullException(nameof(orgToConnect));
            }

            if (repo == null)
            {
                throw new ArgumentNullException(nameof(repo));
            }

            foreach (var srcId in sourceServiceIds)
            {
                if (!srcId.HasValue)
                {
                    continue;
                }

                var match = services.Find(m => m.Item1 == srcId);

                if (match != null)
                {

                    // we should only have the sourceServices if the services list is based on the responsible services of source organization
                    if (sourceServices != null && sourceServices.Count > 0)
                    {
                        // we have sourceServices so we can try to look up the provision type based on the source service id
                        // try to find the matching source service id from sourceServices list
                        // if found then try to find the source organization id from the producer organization list
                        // if the organization id is found from the producerOrganization then try use the provision type
                        var matchedSourceService = sourceServices.Find(x => x.Id == srcId);

                        if (matchedSourceService?.ServiceToOrganization?.ProducerOrganization != null && matchedSourceService.ServiceToOrganization.ProvisionType != null)
                        {
                            if (matchedSourceService.ServiceToOrganization.ResponsibleOrganization.Contains(orgToConnect.Item1))
                            {
                                if (matchedSourceService.ServiceToOrganization.ProvisionType.Count > 1)
                                {
                                    logger.LogWarning($"ConnectServiceToOrganization, source org id: '{orgToConnect.Item1}', source service id: '{srcId}' has more than one ProvisionTypes defined, using first one.");
                                }
//                                pt = GetProvisionType(sysValues.ProvisionTypes, matchedSourceService.ServiceToOrganization.ProvisionType.FirstOrDefault());
                            }
                        }
                    }

                    repo.Add(new OrganizationService
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
/*ProducerInfo                        Id = Guid.NewGuid(),*/
                        OrganizationId = orgToConnect.Item2,
/*ProducerInfo                          RoleType = connectionRole,*/
                        ServiceVersionedId = match.Item2,
/*ProducerInfo                          ProvisionType = pt*/
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectServiceToOrganization, unable to connect organization with source id '{orgToConnect.Item1}' (system guid: '{orgToConnect.Item2}') to source service id '{srcId}' because no service matching the source id was found.");
                }
            }
        }

        /// <summary>
        /// Connects service channel to a service using the source id list (serviceChannelIds) and channels list.
        /// </summary>
        /// <param name="serviceChannelIds">source channel ids</param>
        /// <param name="channels">list of original ids and guid of created channel</param>
        /// <param name="serviceVersionedToConnect">service to connect the channels</param>
        /// <param name="repo">the repository where to save data</param>
        /// <param name="sourceServiceId">fake ptv service id (the original id used in json)</param>
        /// <param name="channelType">channel type identifier, only used for logging</param>
        private void ConnectChannelToService(List<int> serviceChannelIds, List<Tuple<int, Guid>> channels, ServiceVersioned serviceVersionedToConnect, IServiceServiceChannelRepository repo, int sourceServiceId, string channelType)
        {
            if (serviceChannelIds == null || serviceChannelIds.Count == 0)
            {
                return;
            }

            if (channels == null || channels.Count == 0)
            {
                return;
            }

            if (serviceVersionedToConnect == null)
            {
                throw new ArgumentNullException(nameof(serviceVersionedToConnect));
            }

            if (repo == null)
            {
                throw new ArgumentNullException(nameof(repo));
            }

            foreach (var channelId in serviceChannelIds)
            {
                var match = channels.Find(m => m.Item1 == channelId);

                if (match != null)
                {
                    repo.Add(new ServiceServiceChannel
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        ServiceId = serviceVersionedToConnect.UnificRootId,
                        ServiceChannelId = match.Item2
                    });
                }
                else
                {
                    logger.LogError($"ConnectChannelToService, source service id: '{sourceServiceId}', channel with source channel id '{channelId}' (channel type: {channelType}) not found from imported channels list.");
                }
            }
        }

        /// <summary>
        /// Connects a service class to a service.
        /// </summary>
        /// <param name="existingServiceClasses">list of existing service classes in system</param>
        /// <param name="sourceServiceClasses">list of source service classes</param>
        /// <param name="serviceServiceClassRepo">service service class repository</param>
        /// <param name="serviceVersionedToConnect">service to connect to</param>
        /// <param name="sourceServiceId">fake ptv service id (the original id used in json)</param>
        private void ConnectServiceClasses(List<ServiceClass> existingServiceClasses, List<string> sourceServiceClasses, IServiceServiceClassRepository serviceServiceClassRepo, ServiceVersioned serviceVersionedToConnect, int sourceServiceId)
        {
            if (existingServiceClasses == null || existingServiceClasses.Count == 0)
            {
                return;
            }

            if (sourceServiceClasses == null || sourceServiceClasses.Count == 0)
            {
                return;
            }

            if (serviceServiceClassRepo == null)
            {
                throw new ArgumentNullException(nameof(serviceServiceClassRepo));
            }

            if (serviceVersionedToConnect == null)
            {
                throw new ArgumentNullException(nameof(serviceVersionedToConnect));
            }

            foreach (var sc in sourceServiceClasses)
            {
                var matched = existingServiceClasses.Find(m => string.Compare(m.Label, sc, StringComparison.OrdinalIgnoreCase) == 0);

                if (matched != null)
                {
                    serviceServiceClassRepo.Add(new ServiceServiceClass
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        ServiceVersioned = serviceVersionedToConnect,
                        ServiceClass = matched
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectServiceClasses, service class with label '{sc}' not found from existing service classes. Original source service id: '{sourceServiceId}'.");
                }
            }
        }

        /// <summary>
        /// Connects an ontology term to a service.
        /// </summary>
        /// <param name="existingOntologyTerms">list of system existing ontology terms</param>
        /// <param name="sourceOntologyTerms">list of source ontology terms</param>
        /// <param name="serviceOntologyTermRepo">service ontology term repository</param>
        /// <param name="serviceVersionedToConnect">service to connect to</param>
        /// <param name="sourceServiceId">fake ptv service id (the original id used in json)</param>
        private void ConnectOntologyTerms(List<OntologyTerm> existingOntologyTerms, List<string> sourceOntologyTerms, IServiceOntologyTermRepository serviceOntologyTermRepo, ServiceVersioned serviceVersionedToConnect, int sourceServiceId)
        {
            if (existingOntologyTerms == null || existingOntologyTerms.Count == 0)
            {
                return;
            }

            if (sourceOntologyTerms == null || sourceOntologyTerms.Count == 0)
            {
                return;
            }

            if (serviceOntologyTermRepo == null)
            {
                throw new ArgumentNullException(nameof(serviceOntologyTermRepo));
            }

            if (serviceVersionedToConnect == null)
            {
                throw new ArgumentNullException(nameof(serviceVersionedToConnect));
            }

            foreach (var ot in sourceOntologyTerms)
            {
                var matched = existingOntologyTerms.Find(m => string.Compare(m.Label, ot, StringComparison.OrdinalIgnoreCase) == 0);

                if (matched != null)
                {
                    serviceOntologyTermRepo.Add(new ServiceOntologyTerm
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        OntologyTerm = matched,
                        ServiceVersioned = serviceVersionedToConnect
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectOntologyTerms, ontology term with label '{ot}' not found from existing ontology terms. Original source service id: '{sourceServiceId}'.");
                }
            }
        }

//        TODO Remove Service municipalities move to Service area municipalities
//        /// <summary>
//        /// Connects a municipality to a service.
//        /// </summary>
//        /// <param name="existingMunicipalities">system existing municipalities</param>
//        /// <param name="sourceMunicipalities">list of source municipalities for the service</param>
//        /// <param name="serviceMunicipalityRepo">service municipality repository</param>
//        /// <param name="serviceVersionedToConnect">service to connect to</param>
//        /// <param name="sourceServiceId">fake ptv service id (the original id used in json)</param>
        //private void ConnectMunicipalities(List<Municipality> existingMunicipalities, List<string> sourceMunicipalities, IServiceMunicipalityRepository serviceMunicipalityRepo, ServiceVersioned serviceVersionedToConnect, int sourceServiceId, ImportTaskSystemValues systemValues)
        //{
        //    if (existingMunicipalities == null || existingMunicipalities.Count == 0)
        //    {
        //        return;
        //    }

        //    if (sourceMunicipalities == null || sourceMunicipalities.Count == 0)
        //    {
        //        return;
        //    }

        //    if (serviceMunicipalityRepo == null)
        //    {
        //        throw new ArgumentNullException(nameof(serviceMunicipalityRepo));
        //    }

        //    if (serviceVersionedToConnect == null)
        //    {
        //        throw new ArgumentNullException(nameof(serviceVersionedToConnect));
        //    }

        //    foreach (var municipality in sourceMunicipalities)
        //    {
        //        var matched = GetMunicipalityFromFakePtvMunicipalityString(existingMunicipalities, municipality, systemValues);

        //        if (matched != null)
        //        {
        //            serviceMunicipalityRepo.Add(new ServiceMunicipality()
        //            {
        //                Created = DateTime.UtcNow,
        //                CreatedBy = ImportTask.DefaultCreatedBy,
        //                Municipality = matched,
        //                ServiceVersioned = serviceVersionedToConnect
        //            });
        //        }
        //        else
        //        {
        //            _logger.LogWarning($"ConnectMunicipalities, municipality with name '{municipality}' not found from existing municipalities. Original source service id: '{sourceServiceId}'.");
        //        }
        //    }
        //}

        /// <summary>
        /// Connects target groups to a service.
        /// </summary>
        /// <param name="existingTargetGroups">system existing target groups</param>
        /// <param name="sourceTargetGroups">source target groups</param>
        /// <param name="serviceTargetGroupRepo">service target group repository</param>
        /// <param name="serviceVersionedToConnect">service to connect to</param>
        /// <param name="sourceServiceId">fake ptv service id (the original id used in json)</param>
        private void ConnectTargetGroups(List<TargetGroup> existingTargetGroups, List<string> sourceTargetGroups, IServiceTargetGroupRepository serviceTargetGroupRepo, ServiceVersioned serviceVersionedToConnect, int sourceServiceId)
        {
            if (existingTargetGroups == null || existingTargetGroups.Count == 0)
            {
                return;
            }

            if (sourceTargetGroups == null || sourceTargetGroups.Count == 0)
            {
                return;
            }

            if (serviceTargetGroupRepo == null)
            {
                throw new ArgumentNullException(nameof(serviceTargetGroupRepo));
            }

            if (serviceVersionedToConnect == null)
            {
                throw new ArgumentNullException(nameof(serviceVersionedToConnect));
            }

            foreach (var tg in sourceTargetGroups)
            {
                var matched = existingTargetGroups.Find(m => string.Compare(m.Label, tg, StringComparison.OrdinalIgnoreCase) == 0);

                if (matched != null)
                {
                    serviceTargetGroupRepo.Add(new ServiceTargetGroup
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        ServiceVersioned = serviceVersionedToConnect,
                        TargetGroup = matched
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectTargetGroups, target group with label '{tg}' not found from existing target groups. Original source service id: '{sourceServiceId}'.");
                }
            }
        }

        /// <summary>
        /// Connects existing life event to a service.
        /// </summary>
        /// <param name="existingLifeEvents">list of system life events</param>
        /// <param name="sourceLifeEvents">source life events list</param>
        /// <param name="serviceLifeEventRepo">repo</param>
        /// <param name="serviceVersionedToConnect">the service where the life event will be connected to</param>
        /// <param name="sourceServiceId">fake ptv service id (the original id used in json)</param>
        private void ConnectLifeEvents(List<LifeEvent> existingLifeEvents, List<string> sourceLifeEvents, IServiceLifeEventRepository serviceLifeEventRepo, ServiceVersioned serviceVersionedToConnect, int sourceServiceId)
        {
            if (serviceLifeEventRepo == null)
            {
                throw new ArgumentNullException(nameof(serviceLifeEventRepo));
            }

            if (serviceVersionedToConnect == null)
            {
                throw new ArgumentNullException(nameof(serviceVersionedToConnect));
            }

            if (existingLifeEvents == null || existingLifeEvents.Count == 0)
            {
                return;
            }

            if (sourceLifeEvents == null || sourceLifeEvents.Count == 0)
            {
                return;
            }

            foreach (var le in sourceLifeEvents)
            {
                var matched = existingLifeEvents.Find(m => string.Compare(m.Label, le, StringComparison.OrdinalIgnoreCase) == 0);

                if (matched != null)
                {
                    serviceLifeEventRepo.Add(new ServiceLifeEvent
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        LifeEvent = matched,
                        ServiceVersioned = serviceVersionedToConnect
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectLifeEvents, life event with label '{le}' not found from existing life events. Source service id: '{sourceServiceId}'.");
                }
            }
        }

        /// <summary>
        /// Connects a source keyword to a service. If the keyword doesn't exist in the existingKeywords list the keyword is created and added to the list.
        /// </summary>
        /// <param name="existingKeywords">system existing keywords</param>
        /// <param name="sourceKeywords">list of source keywords</param>
        /// <param name="keywordRepo">keyword repository</param>
        /// <param name="serviceKeywordRepo">service keyword repository</param>
        /// <param name="serviceVersionedToConnect">service to connect to</param>
        /// <param name="sourceServiceId">fake ptv service id (the original id used in json)</param>
        /// <param name="systemValues"></param>
        private void ConnectOrCreateKeyword(List<Keyword> existingKeywords, List<string> sourceKeywords, IKeywordRepository keywordRepo, IServiceKeywordRepository serviceKeywordRepo, ServiceVersioned serviceVersionedToConnect, int sourceServiceId, ImportTaskSystemValues systemValues)
        {
            if (keywordRepo == null)
            {
                throw new ArgumentNullException(nameof(keywordRepo));
            }

            if (serviceKeywordRepo == null)
            {
                throw new ArgumentNullException(nameof(serviceKeywordRepo));
            }

            if (serviceVersionedToConnect == null)
            {
                throw new ArgumentNullException(nameof(serviceVersionedToConnect));
            }

            if (existingKeywords == null)
            {
                throw new ArgumentNullException(nameof(existingKeywords));
            }

            if (sourceKeywords == null || sourceKeywords.Count == 0)
            {
                return;
            }

            foreach (var kw in sourceKeywords)
            {
                var existingKeyword = existingKeywords.Find(k => string.Compare(k.Name, kw, StringComparison.OrdinalIgnoreCase) == 0);

                if (existingKeyword != null)
                {
                    // link to existing keyword
                    serviceKeywordRepo.Add(new ServiceKeyword
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        Keyword = existingKeyword,
                        ServiceVersioned = serviceVersionedToConnect
                    });
                }
                else
                {
                    logger.LogInformation($"ConnectOrCreateKeyword, keyword '{kw}' not found from existing keywords, creating a new keyword. Original source service id: '{sourceServiceId}'.");

                    // create a new keyword and link to that
                    var newKeyWord = new Keyword
                    {
                        Id = Guid.NewGuid(),
                        Name = kw,
                        Localization = systemValues.DefaultLanguage,
                    };
                    SetCreatedInfo(newKeyWord);
                    keywordRepo.Add(newKeyWord);

                    // add keyword to existing keywords too
                    existingKeywords.Add(newKeyWord);

                    serviceKeywordRepo.Add(new ServiceKeyword
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        Keyword = newKeyWord,
                        ServiceVersioned = serviceVersionedToConnect
                    });
                }
            }
        }

        private void ConnectChannelTargetGroups(ServiceChannelVersioned sc, IServiceChannelTargetGroupRepository repo, List<TargetGroup> existingTargetGroups, List<string> sourceTargetGroups, string sourceChannelIdentifier)
        {
            if (sc == null)
            {
                throw new ArgumentNullException(nameof(sc));
            }

            if (repo == null)
            {
                throw new ArgumentNullException(nameof(repo));
            }

            if (existingTargetGroups == null || existingTargetGroups.Count == 0)
            {
                logger.LogInformation($"ConnectChannelTargetGroups, existing target groups list is null or no entries. {sourceChannelIdentifier}");
                return;
            }

            if (sourceTargetGroups == null || sourceTargetGroups.Count == 0)
            {
                logger.LogInformation($"ConnectChannelTargetGroups, source target groups list is null or no entries. {sourceChannelIdentifier}");
                return;
            }

            foreach (var tg in sourceTargetGroups)
            {
                var matchedTg = existingTargetGroups.Find(m => string.Compare(m.Label, tg, StringComparison.OrdinalIgnoreCase) == 0);

                if (matchedTg != null)
                {
                    repo.Add(new ServiceChannelTargetGroup
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        ServiceChannelVersioned = sc,
                        TargetGroup = matchedTg
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectChannelTargetGroups, TargetGroup with label '{tg}' not found from PTV database. {sourceChannelIdentifier}");
                }
            }
        }

        private void ConnectChannelServiceClasses(ServiceChannelVersioned sc, IServiceChannelServiceClassRepository repo, List<ServiceClass> existingServiceClasses, List<string> sourceServiceClasses, string sourceChannelIdentifier)
        {
            if (sc == null)
            {
                throw new ArgumentNullException(nameof(sc));
            }

            if (repo == null)
            {
                throw new ArgumentNullException(nameof(repo));
            }

            if (existingServiceClasses == null || existingServiceClasses.Count == 0)
            {
                logger.LogInformation($"ConnectChannelServiceClasses, existing service classes list is null or no entries. {sourceChannelIdentifier}");
                return;
            }

            if (sourceServiceClasses == null || sourceServiceClasses.Count == 0)
            {
                logger.LogInformation($"ConnectChannelServiceClasses, source service classes list is null or no entries. {sourceChannelIdentifier}");
                return;
            }

            foreach (var sourceServiceClass in sourceServiceClasses)
            {
                var matchedSc = existingServiceClasses.Find(m => string.Compare(m.Label, sourceServiceClass, StringComparison.OrdinalIgnoreCase) == 0);

                if (matchedSc != null)
                {
                    repo.Add(new ServiceChannelServiceClass
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        ServiceChannelVersioned = sc,
                        ServiceClass = matchedSc
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectChannelServiceClasses, ServiceClass with label '{sourceServiceClass}' not found from PTV database. {sourceChannelIdentifier}");
                }
            }
        }

        private void ConnectChannelOntologyTerms(ServiceChannelVersioned sc, IServiceChannelOntologyTermRepository repo, List<OntologyTerm> existingOntologyTerms, List<string> sourceOntologyTerms, string sourceChannelIdentifier)
        {
            if (sc == null)
            {
                throw new ArgumentNullException(nameof(sc));
            }

            if (repo == null)
            {
                throw new ArgumentNullException(nameof(repo));
            }

            if (existingOntologyTerms == null || existingOntologyTerms.Count == 0)
            {
                logger.LogInformation($"ConnectChannelOntologyTerms, existing ontology terms list is null or no entries. {sourceChannelIdentifier}");
                return;
            }

            if (sourceOntologyTerms == null || sourceOntologyTerms.Count == 0)
            {
                logger.LogInformation($"ConnectChannelOntologyTerms, source ontology terms list is null or no entries. {sourceChannelIdentifier}");
                return;
            }

            foreach (var ot in sourceOntologyTerms)
            {
                var matchedOt = existingOntologyTerms.Find(m => string.Compare(m.Label, ot, StringComparison.OrdinalIgnoreCase) == 0);

                if (matchedOt != null)
                {
                    repo.Add(new ServiceChannelOntologyTerm
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = DefaultCreatedBy,
                        OntologyTerm = matchedOt,
                        ServiceChannelVersioned = sc
                    });
                }
                else
                {
                    logger.LogWarning($"ConnectChannelOntologyTerms, OntologyTerm with label '{ot}' not found from PTV database. {sourceChannelIdentifier}");
                }
            }
        }

        private void ConnectOrCreateChannelKeywords(ServiceChannelVersioned sc, IServiceChannelKeywordRepository repo, IKeywordRepository kwRepo, List<Keyword> existingKeywords, List<string> sourceKeywords, string sourceChannelIdentifier, ImportTaskSystemValues systemValues)
        {
            if (sc == null)
            {
                throw new ArgumentNullException(nameof(sc));
            }

            if (repo == null)
            {
                throw new ArgumentNullException(nameof(repo));
            }

            if (kwRepo == null)
            {
                throw new ArgumentNullException(nameof(kwRepo));
            }

            if (existingKeywords == null)
            {
                throw new ArgumentNullException(nameof(existingKeywords));
            }

            if (sourceKeywords == null || sourceKeywords.Count == 0)
            {
                logger.LogInformation($"ConnectOrCreateChannelKeywords, source keyword list is null or no entries. {sourceChannelIdentifier}");
                return;
            }

            foreach (var kw in sourceKeywords)
            {
                var keywordToConnect = existingKeywords.Find(ekw => string.Compare(ekw.Name, kw, StringComparison.OrdinalIgnoreCase) == 0);

                if (keywordToConnect == null)
                {
                    logger.LogInformation($"ConnectOrCreateChannelKeywords, keyword '{kw}' not found from existing keywords, creating a new keyword. {sourceChannelIdentifier}");

                    // keyword doesn't exist, create a new one
                    keywordToConnect = new Keyword
                    {
                        Id = Guid.NewGuid(),
                        Name = kw,
                        Localization = systemValues.DefaultLanguage
                    };
                    SetCreatedInfo(keywordToConnect);
                    kwRepo.Add(keywordToConnect);
                    // add the new keyword also to the list of existing keywords
                    existingKeywords.Add(keywordToConnect);
                }

                repo.Add(new ServiceChannelKeyword
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = DefaultCreatedBy,
                    Keyword = keywordToConnect,
                    ServiceChannelVersioned = sc
                });
            }
        }

        /// <summary>
        /// Tries to get municipality using the fake ptv municipality string (format sample: 491 Mikkeli).
        /// </summary>
        /// <param name="municipalities">system municipalities</param>
        /// <param name="municipalityString">fake ptv municipality string (format sample: 491 Mikkeli)</param>
        /// <param name="systemValues"></param>
        /// <returns>found municipality or null</returns>
        private Municipality GetMunicipalityFromFakePtvMunicipalityString(List<Municipality> municipalities, string municipalityString, ImportTaskSystemValues systemValues)
        {
            // NOTE: so far seen strings like: "Mikkeli", "491 Mikkeli", "Mikkeli 491"

            Municipality matched = null;

            if (municipalities != null && municipalities.Count > 0 && !string.IsNullOrWhiteSpace(municipalityString))
            {
                var splitCodes = municipalityString.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                // first try to match with the municipality code like 491
                matched = municipalities.Find(x => x.Code == splitCodes[0]);

                // 2nd try to match the name to the first index as there are cases like 'Mikkeli' so the first index will have 'Mikkeli'
                if (matched == null)
                {
                    matched = municipalities.Find(x => string.Compare(x.MunicipalityNames.FirstOrDefault(y => y.Localization.Code == systemValues.DefaultLanguage.Code)?.Name, splitCodes[0], StringComparison.OrdinalIgnoreCase) == 0);
                }

                if (matched == null && splitCodes.Length > 1)
                {
                    // try to match with the municipality name
                    matched = municipalities.Find(x => string.Compare(x.MunicipalityNames.FirstOrDefault(y => y.Localization.Code == systemValues.DefaultLanguage.Code)?.Name, splitCodes[1], StringComparison.OrdinalIgnoreCase) == 0);
                }

                if (matched == null)
                {
                    logger.LogWarning($"GetMunicipalityFromFakePtvMunicipalityString, no municipality match for municipality string: {municipalityString}.");
                }
            }

            return matched;
        }

        private WebPageType GetWebPageType(ImportTaskSystemValues systemValues, string sourceWebPageType)
        {
            if (systemValues == null)
            {
                throw new ArgumentNullException(nameof(systemValues));
            }

            // currently fake ptv source seems to use these values
            // homepage
            // social_media
            // other

            if (string.Compare("social_media", sourceWebPageType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return systemValues.WebpageTypes.Social;
            }

            return systemValues.WebpageTypes.Home;
        }

        private PrintableFormChannelUrlType GetPrintableFormChannelUrlType(ImportTaskSystemValues systemValues, string sourceAttachmentType)
        {
            if (systemValues == null)
            {
                throw new ArgumentNullException(nameof(systemValues));
            }

            PrintableFormChannelUrlType resolvedType = null;

            if (string.Compare("doc", sourceAttachmentType, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("docx", sourceAttachmentType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                resolvedType = systemValues.PrintableFormChannelUrlTypes.Word;
            }
            else if (string.Compare("xls", sourceAttachmentType, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("xlsx", sourceAttachmentType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                resolvedType = systemValues.PrintableFormChannelUrlTypes.Excel;
            }
            else if (string.Compare("pdf", sourceAttachmentType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                resolvedType = systemValues.PrintableFormChannelUrlTypes.Pdf;
            }
            else
            {
                logger.LogWarning($"GetPrintableFormChannelUrlType, unknown source attachment file type: '{sourceAttachmentType}'.");
            }

            return resolvedType;
        }

        private PhoneNumberType GetPhoneNumberType(SourcePhoneEntity srcPhone, SysPhoneNumberTypes phoneNumberTypes)
        {
            if (srcPhone == null)
            {
                throw new ArgumentNullException(nameof(srcPhone));
            }

            if (phoneNumberTypes == null)
            {
                throw new ArgumentNullException(nameof(phoneNumberTypes));
            }

            var mappedType = phoneNumberTypes.Phone;

            if (!string.IsNullOrWhiteSpace(srcPhone.PhoneServiceChannelType))
            {
                if (string.Compare(FakePtvPhoneServiceChannelType.PhoneService, srcPhone.PhoneServiceChannelType, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mappedType = phoneNumberTypes.Phone;
                }
                else if (string.Compare(FakePtvPhoneServiceChannelType.SMSService, srcPhone.PhoneServiceChannelType, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mappedType = phoneNumberTypes.Sms;
                }
                else if (string.Compare(FakePtvPhoneServiceChannelType.FaxService, srcPhone.PhoneServiceChannelType, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mappedType = phoneNumberTypes.Fax;
                }
                else
                {
                    logger.LogWarning($"Unrecognized phoneServiceChannelType:'{srcPhone.PhoneServiceChannelType}' for source phone channel entity with id '{srcPhone.Id}'. Using phone number type.");
                }
            }
            else
            {
                logger.LogWarning($"Source phone channel entity with id '{srcPhone.Id}' has no PhoneServiceChannelType defined. Using phone number type.");
            }

            return mappedType;
        }

        /// <summary>
        /// Sets the entities default created timestamp and creator.
        /// </summary>
        /// <param name="entity"></param>
        private static void SetCreatedInfo(IAuditing entity)
        {
            if (entity != null)
            {
                entity.Created = DateTime.UtcNow;
                entity.CreatedBy = DefaultCreatedBy;
            }
        }

        /// <summary>
        /// Helper method to get organizations postal code in the first item value and in the second the postal code name (district) in upper case letters.
        /// </summary>
        /// <param name="postal">IPostal interface for an address (postalAddress or visitAddress)</param>
        /// <returns>null if there was no postal code otherwise a Tuple that has the postal code in the first item value and in the second the postal code name (district) in upper case letters</returns>
        private static Tuple<string, string> GetPostalCode(ISourceAddress postal)
        {
            if (postal == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(postal.PostalCode) && !string.IsNullOrWhiteSpace(postal.PostalDistrict))
            {
                return Tuple.Create(postal.PostalCode.Trim(), postal.PostalDistrict.Trim().ToUpperInvariant());
            }

            return null;
        }

        [SuppressMessage("ReSharper", "CommentTypo")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private OrganizationType GetOrganizationType(SysOrganizationTypes sysTypes, SourceOrganizationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sysTypes == null)
            {
                throw new ArgumentNullException(nameof(sysTypes));
            }

            OrganizationType mappedType = null;

            // currently the fake ptv contains these entries in JSON
            // ""
            // "Valtio" and "valtio" (State)
            // "Järjestö" (Organization) and "Järjestöt"
            // "Alueellinen yhteistoimintaorganisaatio" (RegionalOrganization)
            // "Kunta" (Municipality)
            // "Yritykset" (Company) and "Yritys"

            if (string.Compare("valtio", entity.OrganizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = sysTypes.State;
            }
            else if (string.Compare("kunta", entity.OrganizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = sysTypes.Municipality;
            }
            else if (string.Compare("Alueellinen yhteistoimintaorganisaatio", entity.OrganizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = sysTypes.RegionalOrganization;
            }
            else if (string.Compare("Järjestö", entity.OrganizationType, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("Järjestöt", entity.OrganizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = sysTypes.Organization;
            }
            else if (string.Compare("Yritykset", entity.OrganizationType, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("Yritys", entity.OrganizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = sysTypes.Company;
            }

            return mappedType;
        }

        private static void MapServiceHoursDaysOfWeek(ServiceHours serviceHours, SourceOpeningTime openingTime)
        {
            if (serviceHours == null)
            {
                throw new ArgumentNullException(nameof(serviceHours));
            }

            if (openingTime == null)
            {
                throw new ArgumentNullException(nameof(openingTime));
            }

            if (openingTime.DaysOfWeek == null || openingTime.DaysOfWeek.Count == 0)
            {
                return;
            }

            // daysOfWeek list should contain strings 1, 2, 3, 4, 5, 6, 7
            // 1=monday, 2=tuesday, 3=wednesday, 4=thursday, 5=friday, 6=saturday, 7=sunday
            // any other value is invalid

            foreach (var code in openingTime.DaysOfWeek)
            {
                if (string.IsNullOrWhiteSpace(code) || code.Length != 1)
                {
                    continue;
                }

                // Source material uses 1-7 index and target entity 0-6 index.
                var weekDayNumber = int.Parse(code) - 1;

                if (Enum.TryParse(weekDayNumber.ToString(), out WeekDayEnum weekDay))
                {
                    var dot = new DailyOpeningTime
                    {
                        DayFrom = (int)weekDay,
                        DayTo = (int)weekDay
                    };

                    if (IsValidServiceHour(openingTime.Closes))
                    {
                        dot.To = TimeSpan.Parse(openingTime.Closes);
                    }

                    if (IsValidServiceHour(openingTime.Opens))
                    {
                        dot.From = TimeSpan.Parse(openingTime.Opens);
                    }
                    serviceHours.DailyOpeningTimes.Add(dot);
                }
            }
        }

        #endregion
    }
}
