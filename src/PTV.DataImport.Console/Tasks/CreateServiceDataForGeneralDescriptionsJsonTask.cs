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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;

namespace PTV.DataImport.Console.Tasks
{
    public class CreateServiceDataForGeneralDescriptionsJsonTask
    {
//        private static readonly string GeneralDescriptionsGeneratedFile = Path.Combine("Generated", "GeneralDescriptionOutput.json");
        private const string DefaultCreatedBy = "CreateGeneralDescriptionsJsonTask";

        private readonly IServiceProvider serviceProvider;

        public CreateServiceDataForGeneralDescriptionsJsonTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateServiceDataForGeneralDescriptionsJsonTask>();

            logger.LogDebug("CreateGeneralDescriptionsJsonTask .ctor");
        }

        public void ImportDataFromJSON()
        {
            var langCode = DomainConstants.DefaultLanguage;
//
            //List<Guid> municipalityOrganizationIds = new List<Guid>();
            List<StatutoryServiceGeneralDescriptionVersioned> statutoryServiceGeneralDescriptions;
//            Language defaultLanguage = null;
//
            var draftCode = PublishingStatus.Draft.ToString();
//            //PublishingStatusType defaultPublishingType = null;
//
            var nameTypeName = NameTypeEnum.Name.ToString();
//            //NameType defaultNameType = null;
//
//
//
//            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
//            {
//                var service = serviceScope.ServiceProvider.GetService<IGeneralDescriptionUpdateService>();
//                service?.CreateOrUpdateGeneralDescriptions(importedGeneralDescriptions);
//            }


            // for performance/speed reason read these into memory (there are only ~130 entries on each table currently)

            System.Console.WriteLine();

                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                    scopedCtxMgr.ExecuteWriter(unitOfWork =>
                    {

                        var municipalityRepository = unitOfWork.CreateRepository<IMunicipalityRepository>();
                        var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                        var organizationTypeRepository = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                        var statutoryServiceGeneralDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

                        // Exclude Helsinki and Mikkeli
                        var excludedMunicipalityIds = municipalityRepository.All().Where(x => x.MunicipalityNames.Any(y => y.Name.Equals("Helsinki") || y.Name.Equals("Mikkeli"))).Select(m => m.Id).ToList();

                        // Get all organizations that are municipalities (exclude Mikkeli and Helsinki)
                        var organizationMunicipalityTypeCode = OrganizationTypeEnum.Municipality.ToString();
                        var organizationMunicipalityTypeId = organizationTypeRepository.All().First(x => x.Code == organizationMunicipalityTypeCode).Id;

                        var municipalityOrganizationsUnificRoots =
                            organizationRepository.All()
                                .Where(x => x.TypeId == organizationMunicipalityTypeId && ((x.MunicipalityId == null) || ((x.MunicipalityId != null) && !excludedMunicipalityIds.Contains(x.MunicipalityId.Value))))
                                .Select(m =>  m.UnificRootId)
                                .ToList();

                        statutoryServiceGeneralDescriptions = statutoryServiceGeneralDescriptionRepository.All().ToList();

                        var existingStatutoryServiceNames = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>().All().ToList();
                        var existingStatutoryServiceClasses = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>().All().ToList();

                        var serviceRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                        var serviceNameRepository = unitOfWork.CreateRepository<IServiceNameRepository>();
                        var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                        var serviceServiceClassRepository = unitOfWork.CreateRepository<IServiceServiceClassRepository>();

                        var defaultPublishingType =
                               unitOfWork
                                   .CreateRepository<IPublishingStatusTypeRepository>()
                                   .All()
                                   .FirstOrDefault(m => m.Code == draftCode);
                        var defaultLanguage =
                            unitOfWork.CreateRepository<ILanguageRepository>().All().First(x => x.Code == langCode);
                        var defaultNameType =
                            unitOfWork
                                .CreateRepository<INameTypeRepository>()
                                .All()
                                .FirstOrDefault(x => x.Code == nameTypeName);

                        var defaultAreaInformationType = unitOfWork
                                .CreateRepository<IAreaInformationTypeRepository>()
                                .All()
                                .FirstOrDefault(x => x.Code == AreaInformationTypeEnum.WholeCountry.ToString());

                        var defaultFundingType = unitOfWork.CreateRepository<IServiceFundingTypeRepository>()
                                .All()
                                .FirstOrDefault(x => x.Code == ServiceFundingTypeEnum.PubliclyFunded.ToString());


                        foreach (var statutoryServiceGeneralDescription in statutoryServiceGeneralDescriptions)
                        {
                            System.Console.Write("#");
                            // Link the created general description to each municipality by creating a new service
                            foreach (var municipalityOrganizationRootId in municipalityOrganizationsUnificRoots)
                            {
                                // Create service for each municipality organization
                                var service = new ServiceVersioned
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,
                                    Id = Guid.NewGuid(),
                                    StatutoryServiceGeneralDescriptionId = statutoryServiceGeneralDescription.UnificRootId,
                                    //ServiceCoverageTypeId = null,
                                    Type = null,
                                    FundingType = defaultFundingType,
                                    AreaInformationType = defaultAreaInformationType,
                                    PublishingStatus = defaultPublishingType,
                                    UnificRoot = new Service {  Id = Guid.NewGuid()},
                                    OrganizationId = municipalityOrganizationRootId
                                };

                                var serviceName = new ServiceName
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,
                                    ServiceVersionedId = service.Id,
                                    Name =
                                        existingStatutoryServiceNames.FirstOrDefault(
                                            x => x.StatutoryServiceGeneralDescriptionVersionedId == statutoryServiceGeneralDescription.Id)
                                            ?.Name,
                                    Localization = defaultLanguage,
                                    Type = defaultNameType
                                };

                                // Create OrganizationService linking the service to organization (municipality)
                                var organizationService = new OrganizationService
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,

                                    ServiceVersionedId = service.Id,
                                    OrganizationId = municipalityOrganizationRootId,

                                };

                                var statutoryServiceClass = existingStatutoryServiceClasses.FirstOrDefault(x => x.StatutoryServiceGeneralDescriptionVersionedId == statutoryServiceGeneralDescription.Id);
                                // Create ServiceServiceClass
                                if (statutoryServiceClass != null)
                                {
                                    var serviceServiceClass = new ServiceServiceClass
                                    {
                                        Created = DateTime.UtcNow,
                                        CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,
                                        ServiceVersionedId = service.Id,
                                        ServiceClassId = statutoryServiceClass.ServiceClassId
                                    };
                                    serviceServiceClassRepository.Add(serviceServiceClass);
                                }
                                serviceRepository.Add(service);
                                serviceNameRepository.Add(serviceName);
                                organizationServiceRepository.Add(organizationService);
                            }
                        }
                        unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    });
                }

        }
    }
}
