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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.ExternalSources;

namespace PTV.DataImport.Console.Tasks
{
    public class VmJsonMunicipality
    {
        public string MunicipalityCode { get; set; }
        public List<VmJsonName> Names { get; set; }
        public string OrganizationName { get; set; }
        public string BusinessId { get; set; }
    }


    public class CreateMunicipalityOrganizationsTask
    {
        private readonly IServiceProvider serviceProvider;

        private const string DefaultCreatedBy = "CreateMunicipalityOrganizationsTask";

        public CreateMunicipalityOrganizationsTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            ILogger logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateMunicipalityOrganizationsTask>();

            logger.LogDebug("CreateMunicipalityOrganizationsTask .ctor");
        }

        /// <summary>
        /// Creates organizations and businesses for municipalities.
        /// </summary>
        public void Create()
        {
            // Get start data for OrganizationName and BusinessId
            var resourceManager = new ResourceManager();
            var municipalitiesStartData = resourceManager.GetDesrializedJsonResource<List<VmJsonMunicipality>>(JsonResources.MunicipalityOrganizations);

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                var organizationTypeMunicipalityName = OrganizationTypeEnum.Municipality.ToString();
                var defaultPublishingStatusName = PublishingStatus.Published.ToString();
                var defaultAreaInformationTypeName = AreaInformationTypeEnum.WholeCountry.ToString();
                var nameTypeName = NameTypeEnum.Name.ToString();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var municipalityRepository = unitOfWork.CreateRepository<IMunicipalityRepository>();
                    var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var organizationTypeRepository = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                    var publishingStatusTypeRepository = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>();
                    var businessRepository = unitOfWork.CreateRepository<IBusinessRepository>();
                    var nameTypeRepository = unitOfWork.CreateRepository<INameTypeRepository>();
                    var areaInformationTypeRepository = unitOfWork.CreateRepository<IAreaInformationTypeRepository>();
                    var organizationTypeId = organizationTypeRepository.All().Where(x => x.Code == organizationTypeMunicipalityName).Select(x => x.Id).FirstOrDefault();
                    var organizationNameRepository = unitOfWork.CreateRepository<IOrganizationNameRepository>();
                    var organizationDisplayNameTypeRepository = unitOfWork.CreateRepository<IOrganizationDisplayNameTypeRepository>();
                    var defaultPublishingStatusType = publishingStatusTypeRepository.All().FirstOrDefault(x => x.Code == defaultPublishingStatusName);
                    var nameType = nameTypeRepository.All().First(x => x.Code == nameTypeName).Id;
                    var languageRepository = unitOfWork.CreateRepository<ILanguageRepository>();
                    var languages = languageRepository.All().ToList(); // no another language versions needed?
                    var languageId = languages.First(i => i.Code.ToLower() == DomainConstants.DefaultLanguage.ToLower()).Id;
                    var defaultAreaInformationTypeId = areaInformationTypeRepository.All().Where(x => x.Code == defaultAreaInformationTypeName).Select(x => x.Id).FirstOrDefault();

                    // Read all municipalities
                    // exclude Helsinki (091) and Mikkeli (491) from results
                    var municipalitiesTemp = unitOfWork.ApplyIncludes(municipalityRepository.All(), i => i.Include(j => j.MunicipalityNames));
                    var municipalities = municipalitiesTemp.Where(x=> x.Code != "491" && x.Code != "091").ToList();

                    foreach (var municipality in municipalities)
                    {
                        System.Console.Write("#");

                        // Check if the Organization (type == municipality) exists
                        if (!organizationRepository.All().Any(o => o.Municipality.Id == municipality.Id && o.TypeId == organizationTypeId))
                        {
                            var organizationStartData = municipalitiesStartData.FirstOrDefault(s => s.MunicipalityCode == municipality.Code);
                            if (string.IsNullOrEmpty(organizationStartData?.OrganizationName) || string.IsNullOrEmpty(organizationStartData.BusinessId))
                            {
                                System.Console.WriteLine($"Municipality '{municipality.Code}' has missing data, creating of Organization skipped!");
                                continue;
                            }
                            var municipalityName = municipality.MunicipalityNames.FirstOrDefault(x => x.LocalizationId == languageId)?.Name;

                            if (!string.IsNullOrEmpty(municipalityName))
                            {
                                // Create Organization and Business
                                var business = new Business
                                {
                                    Id = Guid.NewGuid(),
                                    Created = DateTime.UtcNow,
                                    CreatedBy = CreateMunicipalityOrganizationsTask.DefaultCreatedBy,
                                    Name = municipalityName,
                                    //Description = municipality.Description,
                                    //PublishingStatus = defaultPublishingStatusType,
                                    Code = organizationStartData.BusinessId
                                };
                                businessRepository.Add(business);
                            }
                            else
                            {
                                System.Console.WriteLine($"Municipality '{municipality.Code}' has no name in language FI, creating of Organization Business skipped!");
                            }
                            var organization = new OrganizationVersioned
                            {
                                Id = Guid.NewGuid(),
                                Municipality = municipality,
                                TypeId = organizationTypeId,
                                AreaInformationTypeId = defaultAreaInformationTypeId,
                                Created = DateTime.UtcNow,
                                CreatedBy = CreateMunicipalityOrganizationsTask.DefaultCreatedBy,
                                //Business = business,
                                PublishingStatus = defaultPublishingStatusType,
                                //DisplayNameTypeId = nameType,
                                UnificRoot = new Organization { Id = Guid.NewGuid() }
                            };

                            var organizationName = new OrganizationName
                            {
                                Created = DateTime.UtcNow,
                                CreatedBy = CreateMunicipalityOrganizationsTask.DefaultCreatedBy,
                                LocalizationId = languageId,
                                Name = organizationStartData.OrganizationName,
                                OrganizationVersioned = organization,
                                TypeId = nameType
                            };

                            var organizationDisplayNameType = new OrganizationDisplayNameType
                            {
                                Created = DateTime.UtcNow,
                                CreatedBy = CreateMunicipalityOrganizationsTask.DefaultCreatedBy,
                                LocalizationId = languageId,
                                OrganizationVersioned = organization,
                                DisplayNameTypeId = nameType
                            };

                            organizationDisplayNameTypeRepository.Add(organizationDisplayNameType);
                            organizationNameRepository.Add(organizationName);
                            organizationRepository.Add(organization);
                        }
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }
        }
    }
}
