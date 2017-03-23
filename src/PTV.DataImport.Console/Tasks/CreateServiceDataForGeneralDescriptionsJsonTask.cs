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
using Microsoft.Extensions.DependencyInjection;
using PTV.DataImport.ConsoleApp.Services;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PTV.DataImport.ConsoleApp.Models;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class CreateServiceDataForGeneralDescriptionsJsonTask
    {
        private static readonly string GeneralDescriptionsGeneratedFile = Path.Combine("Generated", "GeneralDescription.json");
        private const string DefaultCreatedBy = "CreateGeneralDescriptionsJsonTask";

        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public CreateServiceDataForGeneralDescriptionsJsonTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateServiceDataForGeneralDescriptionsJsonTask>();

            _logger.LogDebug("CreateGeneralDescriptionsJsonTask .ctor");
        }

        public void ImportDataFromJSON()
        {
//            var importedGeneralDescriptions = JsonConvert.DeserializeObject<List<ImportStatutoryServiceGeneralDescription>>(File.ReadAllText(CreateGeneralDescriptionsJsonTask.GeneralDescriptionsGeneratedFile), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            string langCode = LanguageCode.fi.ToString();
//
            //List<Guid> municipalityOrganizationIds = new List<Guid>();
            List<StatutoryServiceGeneralDescriptionVersioned> statutoryServiceGeneralDescriptions = new List<StatutoryServiceGeneralDescriptionVersioned>();
//            Language defaultLanguage = null;
//
            string draftCode = PublishingStatus.Draft.ToString();
//            //PublishingStatusType defaultPublishingType = null;
//
            var nameTypeName = NameTypeEnum.Name.ToString();
//            //NameType defaultNameType = null;
//
            var roleTypeResponsibleName = RoleTypeEnum.Responsible.ToString();
//
            var serviceTypeService = ServiceTypeEnum.Service.ToString();
//
//            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
//            {
//                var service = serviceScope.ServiceProvider.GetService<IGeneralDescriptionUpdateService>();
//                service?.CreateOrUpdateGeneralDescriptions(importedGeneralDescriptions);
//            }


            // for performance/speed reason read these into memory (there are only ~130 entries on each table currently)

            Console.WriteLine();

                using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
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
                        var organizationMunicipalityTypeId = organizationTypeRepository.All().Where(x => x.Code == organizationMunicipalityTypeCode).First().Id;

                        var municipalityOrganizations =
                            organizationRepository.All()
                                .Where(x => x.TypeId == organizationMunicipalityTypeId && !excludedMunicipalityIds.Contains(x.MunicipalityId.Value))
                                .Select(m => new { m.Id, m.UnificRootId })
                                .ToList();

                        statutoryServiceGeneralDescriptions = statutoryServiceGeneralDescriptionRepository.All().ToList();

                        var existingStatutoryServiceNames = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>().All().ToList();
                        var existingStatutoryServiceClasses = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>().All().ToList();

                        var serviceRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                        var serviceNameRepository = unitOfWork.CreateRepository<IServiceNameRepository>();
                        var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
                        var serviceServiceClassRepository = unitOfWork.CreateRepository<IServiceServiceClassRepository>();

                        var defaultPublishingType =
                               unitOfWork.CreateRepository<IPublishingStatusTypeRepository>()
                                   .All()
                                   .Where(m => m.Code == draftCode)
                                   .FirstOrDefault();
                        var defaultLanguage =
                            unitOfWork.CreateRepository<ILanguageRepository>().All().Where(x => x.Code == langCode).First();
                        var defaultRoleType =
                            unitOfWork.CreateRepository<IRoleTypeRepository>()
                                .All()
                                .Where(x => x.Code == roleTypeResponsibleName)
                                .FirstOrDefault();
                        var defaultNameType =
                            unitOfWork.CreateRepository<INameTypeRepository>()
                                .All()
                                .Where(x => x.Code == nameTypeName)
                                .FirstOrDefault();


                        foreach (var statutoryServiceGeneralDescription in statutoryServiceGeneralDescriptions)
                        {
                            Console.Write("#");
                            // Link the created general description to each municipality by creating a new service
                            foreach (var municipalityOrganization in municipalityOrganizations)
                            {
                                // Create service for each municipality organization
                                var service = new ServiceVersioned()
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,
                                    Id = Guid.NewGuid(),
                                    StatutoryServiceGeneralDescriptionId = statutoryServiceGeneralDescription.UnificRootId,
                                    //ServiceCoverageTypeId = null,
                                    Type = null,
                                    PublishingStatus = defaultPublishingType,
                                    UnificRoot = new Service() {  Id = Guid.NewGuid()}
                                };

                                var serviceName = new ServiceName()
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,
                                    ServiceVersionedId = service.Id,
                                    Name =
                                        existingStatutoryServiceNames.FirstOrDefault(
                                            x => x.StatutoryServiceGeneralDescriptionVersionedId == statutoryServiceGeneralDescription.Id).Name,
                                    Localization = defaultLanguage,
                                    Type = defaultNameType
                                };

                                // Create OrganizationService linking the service to organization (municipality)
                                var organizationService = new OrganizationService()
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,
                                    Id = Guid.NewGuid(),
                                    ServiceVersionedId = service.Id,
                                    OrganizationId = municipalityOrganization.UnificRootId,
                                    RoleType = defaultRoleType
                                };

                                // Create ServiceServiceClass
                                var serviceServiceClass = new ServiceServiceClass()
                                {
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateServiceDataForGeneralDescriptionsJsonTask.DefaultCreatedBy,
                                    ServiceVersionedId = service.Id,
                                    ServiceClassId =
                                        existingStatutoryServiceClasses.Where(
                                            x => x.StatutoryServiceGeneralDescriptionVersionedId == statutoryServiceGeneralDescription.Id)
                                            .FirstOrDefault()
                                            .ServiceClassId
                                };

                                serviceRepository.Add(service);
                                serviceNameRepository.Add(serviceName);
                                organizationServiceRepository.Add(organizationService);
                                serviceServiceClassRepository.Add(serviceServiceClass);
                            }
                            unitOfWork.Save(SaveMode.AllowAnonymous);
                        }
                        //unitOfWork.Save(SaveMode.AllowAnonymous);
                    });
                }

        }

        private static List<KeyValuePair<string, string>> ProcessTerms(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            List<KeyValuePair<string, string>> terms = new List<KeyValuePair<string, string>>();
            var pattern = @"([\s\S]*?)\s*\[(.+?)[\]\s]";

            foreach (Match match in Regex.Matches(data, pattern, RegexOptions.Multiline))
            {
                var name = match.Groups[2].Value.Replace("[", "").Replace("]", "").Trim();
                var value = match.Groups[1].Value.Replace("[", "").Replace("]", "").Trim();

                terms.Add(new KeyValuePair<string, string>(name, value));
            }

            return terms;
        }

    }
}
