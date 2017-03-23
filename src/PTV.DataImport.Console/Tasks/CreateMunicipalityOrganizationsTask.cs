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
using System.Linq.Expressions;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Interfaces;
using Newtonsoft.Json.Linq;
using PTV.ExternalSources;
using Microsoft.EntityFrameworkCore;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class CreateMunicipalityOrganizationsTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        private const string DefaultCreatedBy = "CreateMunicipalityOrganizationsTask";
        private static readonly string MunicipalitiesGeneratedFile = Path.Combine("Generated", "Municipality.json");

        public CreateMunicipalityOrganizationsTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateMunicipalityOrganizationsTask>();

            _logger.LogDebug("CreateMunicipalityOrganizationsTask .ctor");
        }

        /// <summary>
        /// Creates organizations and businesses for municipalities.
        /// </summary>
        public void Create()
        {
            // Get start data for OrganizationName and BusinessId
            var resourceManager = new ResourceManager();
            var municipalitiesStartData = resourceManager.GetDesrializedJsonResource<List<JObject>>(JsonResources.Municipality);

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                var organizationTypeMunicipalityName = OrganizationTypeEnum.Municipality.ToString();
                var defaultPublishingStatusName = PublishingStatus.Published.ToString();
                var nameTypeName = NameTypeEnum.Name.ToString();
                string langCode = LanguageCode.fi.ToString();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var municipalityRepository = unitOfWork.CreateRepository<IMunicipalityRepository>();
                    var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var organizationTypeRepository = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                    var publishingStatusTypeRepository = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>();
                    var businessRepository = unitOfWork.CreateRepository<IBusinessRepository>();
                    var nameTypeRepository = unitOfWork.CreateRepository<INameTypeRepository>();
                    var organizationTypeId = organizationTypeRepository.All().Where(x => x.Code == organizationTypeMunicipalityName).Select(x => x.Id).FirstOrDefault();
                    var organizationNameRepository = unitOfWork.CreateRepository<IOrganizationNameRepository>();
                    var defaultPublishingStatusType = publishingStatusTypeRepository.All().Where(x => x.Code == defaultPublishingStatusName).FirstOrDefault();
                    var nameType = nameTypeRepository.All().First(x => x.Code == nameTypeName).Id;
                    var languageRepository = unitOfWork.CreateRepository<ILanguageRepository>();
                    var languageId = languageRepository.All().Where(x => x.Code == langCode).First().Id;

                    // Read all municipalities
                    // exclude Helsinki (091) and Mikkeli (491) from results
                    var municipalitiesTemp = unitOfWork.ApplyIncludes(municipalityRepository.All(), i => i.Include(j => j.MunicipalityNames));
                    var municipalities = municipalitiesTemp.Where(x=> x.Code != "491" && x.Code != "091").ToList();

                    foreach (var municipality in municipalities)
                    {
                        Console.Write("#");

                        // Check if the Organization (type == municipality) exists
                        if (organizationRepository.All().Where(o => o.Municipality.Id == municipality.Id && o.TypeId == organizationTypeId).FirstOrDefault() == null)
                        {
                            var organizationStartDataName = municipalitiesStartData.Where(s => s.GetValue("municipalityCode")?.ToString() == municipality.Code).Select(s => s.GetValue("organizationName")?.ToString()).FirstOrDefault();
                            var organizationStartDataBusinessId = municipalitiesStartData.Where(s => s.GetValue("municipalityCode")?.ToString() == municipality.Code).Select(s => s.GetValue("businessId")?.ToString()).FirstOrDefault();
                            var municipalityName = municipality.MunicipalityNames.FirstOrDefault(x => x.LocalizationId == languageId)?.Name;

                            // Create Organization and Business
                            var business = new Business()
                            {
                                Id = Guid.NewGuid(),
                                Created = DateTime.UtcNow,
                                CreatedBy = CreateMunicipalityOrganizationsTask.DefaultCreatedBy,
                                Name = municipalityName,
                                //Description = municipality.Description,
                                //PublishingStatus = defaultPublishingStatusType,
                                Code = organizationStartDataBusinessId
                            };

                            var organization = new OrganizationVersioned()
                            {
                                Id = Guid.NewGuid(),
                                Municipality = municipality,
                                TypeId = organizationTypeId,
                                Created = DateTime.UtcNow,
                                CreatedBy = CreateMunicipalityOrganizationsTask.DefaultCreatedBy,
                                Business = business,
                                PublishingStatus = defaultPublishingStatusType,
                                DisplayNameTypeId = nameType,
                                UnificRoot = new Organization() { Id = Guid.NewGuid() }
                            };

                            var organizationName = new OrganizationName()
                            {
                                Created = DateTime.UtcNow,
                                CreatedBy = CreateMunicipalityOrganizationsTask.DefaultCreatedBy,
                                LocalizationId = languageId,
                                Name = organizationStartDataName,
                                OrganizationVersioned = organization,
                                TypeId = nameType
                            };

                            businessRepository.Add(business);
                            organizationRepository.Add(organization);
                            organizationNameRepository.Add(organizationName);

                            unitOfWork.Save(SaveMode.AllowAnonymous);
                        }
                    }

                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }
    }
}
