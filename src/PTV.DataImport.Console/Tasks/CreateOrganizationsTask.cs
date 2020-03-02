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
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.DataImport.Console.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;

namespace PTV.DataImport.Console.Tasks
{
    public class CreateOrganizationsJsonTask
    {
        private static readonly string OrganizationsGeneratedFile = Path.Combine("Generated", "OrganizationsAdditional.json");
        private const string DefaultCreatedBy = "CreateOrganizationsJsonTask";

        private readonly IServiceProvider serviceProvider;

        public CreateOrganizationsJsonTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            ILogger logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateOrganizationsJsonTask>();

            logger.LogDebug("CreateOrganizationsJsonTask .ctor");
        }

        public void ImportDataFromJSON()
        {
            var importedOrganizations = JsonConvert.DeserializeObject<List<SourceOrganizationEntity>>(File.ReadAllText(CreateOrganizationsJsonTask.OrganizationsGeneratedFile), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var langCode = DomainConstants.DefaultLanguage;

            var nameTypeName = NameTypeEnum.Name.ToString();

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var defaultLanguage = unitOfWork.CreateRepository<ILanguageRepository>().All().First(x => x.Code == langCode);
                    var defaultNameType = unitOfWork.CreateRepository<INameTypeRepository>().All().FirstOrDefault(x => x.Code == nameTypeName);
                    var publishingStatusNamePublished = PublishingStatus.Published.ToString();
                    var defaultAreaInformationType = AreaInformationTypeEnum.WholeCountry.ToString();

                    var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                    var businessRepository = unitOfWork.CreateRepository<IBusinessRepository>();
                    var organizationTypeRepository = unitOfWork.CreateRepository<IOrganizationTypeRepository>();
                    var organizationNameRepository = unitOfWork.CreateRepository<IOrganizationNameRepository>();
                    var organizationDisplayNameTypeRepository = unitOfWork.CreateRepository<IOrganizationDisplayNameTypeRepository>();
                    var publishingStatusTypeRepository = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>();
                    var areaInformationTypeRepository = unitOfWork.CreateRepository<IAreaInformationTypeRepository>();

                    var publishingStatusPublished = publishingStatusTypeRepository.All().FirstOrDefault(x => x.Code == publishingStatusNamePublished);
                    var areaInformationType = areaInformationTypeRepository.All().FirstOrDefault(x => x.Code == defaultAreaInformationType);

                    foreach (var importedOrganization in importedOrganizations)
                    {
                        System.Console.Write("#");

                        // Create Organization and Business
                        var business = new Business
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.UtcNow,
                            CreatedBy = CreateOrganizationsJsonTask.DefaultCreatedBy,
                            Name = importedOrganization.Name,
                            //PublishingStatus = publishingStatusPublished,
                            Code = importedOrganization.BusinessId
                        };

                        var organizationTypeName = importedOrganization.OrganizationType.Trim().Replace(" ", string.Empty);
                        // ReSharper disable once PossibleInvalidOperationException
                        var organizationTypeNameEnum = GetOrganizationType(organizationTypeName).Value;
                        var organizationType = organizationTypeRepository.All().FirstOrDefault(x => x.Code == organizationTypeNameEnum.ToString());

                        var organization = new OrganizationVersioned
                        {
                            Id = Guid.NewGuid(),
                            Type = organizationType,
                            Created = DateTime.UtcNow,
                            CreatedBy = CreateOrganizationsJsonTask.DefaultCreatedBy,
                            Business = business,
                            PublishingStatus = publishingStatusPublished,
                            //DisplayNameType = defaultNameType,
                            AreaInformationType = areaInformationType,
                        };

                        var organizationName = new OrganizationName
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = CreateOrganizationsJsonTask.DefaultCreatedBy,
                            Localization = defaultLanguage,
                            Name = importedOrganization.Name,
                            OrganizationVersioned = organization,
                            Type = defaultNameType
                        };

                        var organizationDisplayNameType = new OrganizationDisplayNameType
                        {
                            Created = DateTime.UtcNow,
                            CreatedBy = CreateOrganizationsJsonTask.DefaultCreatedBy,
                            Localization = defaultLanguage,
                            OrganizationVersioned = organization,
                            DisplayNameType = defaultNameType
                        };

                        businessRepository.Add(business);
                        organizationRepository.Add(organization);
                        organizationNameRepository.Add(organizationName);
                        organizationDisplayNameTypeRepository.Add(organizationDisplayNameType);
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }
        }

        private OrganizationTypeEnum? GetOrganizationType(string organizationType)
        {
            OrganizationTypeEnum? mappedType = null;

            if (string.Compare("valtio", organizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = OrganizationTypeEnum.State;
            }
            else if (string.Compare("kunta", organizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = OrganizationTypeEnum.Municipality;
            }
            else if (string.Compare("Alueellinen yhteistoimintaorganisaatio", organizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = OrganizationTypeEnum.RegionalOrganization;
            }
            else if (string.Compare("Järjestö", organizationType, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("Järjestöt", organizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = OrganizationTypeEnum.Organization;
            }
            else if (string.Compare("Yritykset", organizationType, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("Yritys", organizationType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                mappedType = OrganizationTypeEnum.Company;
            }

            return mappedType;
        }
    }
}
