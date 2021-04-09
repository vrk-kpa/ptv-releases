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

using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class ServiceServiceTestBase : ServiceTestBase
    {
        internal ILogger<DataAccess.Services.ServiceService> Logger { get; private set; }

        internal IGeneralDescriptionService gdService { get; private set; }

        internal Mock<IGeneralDescriptionService> gdServiceMock { get; private set; }

        internal Mock<IServiceVersionedRepository> ServiceRepoMock { get; private set; }

        internal Mock<IServiceNameRepository> ServiceNameRepoMock { get; private set; }

        internal Mock<IServiceKeywordRepository> ServiceKeywordRepoMock { get; private set; }

        internal Mock<IServiceServiceClassRepository> ServiceServiceClassRepoMock { get; private set; }

        internal Mock<IServiceOntologyTermRepository> ServiceOntologyTermRepoMock { get; private set; }

        internal Mock<IServiceLifeEventRepository> ServiceLifeEventRepoMock { get; private set; }

        internal Mock<IServiceIndustrialClassRepository> ServiceIndustrialClassRepoMock { get; private set; }

        internal Mock<IServiceTargetGroupRepository> ServiceTargetGroupRepoMock { get; private set; }

        internal Mock<IServiceProducerRepository> ServiceProducerRepoMock { get; private set; }

        internal Mock<IServiceLawRepository> ServiceLawRepoMock { get; private set; }

        internal Mock<IServiceAreaRepository> ServiceAreaRepoMock { get; private set; }

        internal Mock<IMunicipalityRepository> MunicipalityRepoMock { get; private set; }


        internal Mock<IServiceChannelVersionedRepository> ServiceChannelRepoMock { get; private set; }

        internal Mock<IServiceChannelNameRepository> ServiceChannelNameRepoMock { get; private set; }

        internal Mock<IOrganizationVersionedRepository> OrganizationVersionedRepoMock { get; private set; }

        internal Mock<IKeywordRepository> KeywordRepoMock { get; private set; }

        internal Mock<ITargetGroupRepository> TargetGroupRepoMock { get; private set; }

        internal Mock<IServiceCollectionServiceRepository> ServiceCollectionRepoMock { get; private set; }
        internal Mock<IServiceCollectionVersionedRepository> ServiceCollectionVersionedRepoMock { get; private set; }
        internal Mock<IServiceCollectionVersionedRepository> serviceCollectionVersionedRepo { get; private set; }
        
        internal Mock<IServiceLanguageRepository> ServiceLanguageRepoMock  { get; private set; }
        internal Mock<IServiceDescriptionRepository> ServiceDescriptionRepoMock  { get; private set; }
        internal Mock<IServiceLanguageAvailabilityRepository> ServiceLanguageAvailabilityRepoMock  { get; private set; }
        internal Mock<IOrganizationServiceRepository> OrganizationServiceRepoMock  { get; private set; }
        internal Mock<IServiceRequirementRepository> ServiceRequirementRepoMock  { get; private set; }
        internal Mock<IServiceAreaMunicipalityRepository> ServiceAreaMunicipalityRepoMock  { get; private set; }
        internal Mock<IServiceWebPageRepository> ServiceWebPageRepoMock  { get; private set; }
        internal Mock<IOrganizationRepository> OrganizationRepoMock  { get; private set; }
        internal Mock<IServiceRepository> ServiceRootRepoMock  { get; private set; }
        
        internal Mock<ITargetGroupDataCache> targetGroupDataCacheMock  { get; private set; }
        internal ITargetGroupDataCache targetGroupDataCache  { get; private set; }

        public ServiceServiceTestBase()
        {
            Logger = new Mock<ILogger<ServiceService>>().Object;

            targetGroupDataCacheMock = new Mock<ITargetGroupDataCache>();
            targetGroupDataCache = targetGroupDataCacheMock.Object;

            gdServiceMock = new Mock<IGeneralDescriptionService>();
            gdService = gdServiceMock.Object;

            ServiceRepoMock = new Mock<IServiceVersionedRepository>();
            ServiceNameRepoMock = new Mock<IServiceNameRepository>();
            ServiceKeywordRepoMock = new Mock<IServiceKeywordRepository>();
            ServiceServiceClassRepoMock = new Mock<IServiceServiceClassRepository>();
            ServiceOntologyTermRepoMock = new Mock<IServiceOntologyTermRepository>();
            ServiceLifeEventRepoMock = new Mock<IServiceLifeEventRepository>();
            ServiceIndustrialClassRepoMock = new Mock<IServiceIndustrialClassRepository>();
            ServiceTargetGroupRepoMock = new Mock<IServiceTargetGroupRepository>();
            ServiceProducerRepoMock = new Mock<IServiceProducerRepository>();
            ServiceLawRepoMock = new Mock<IServiceLawRepository>();
            ServiceAreaRepoMock = new Mock<IServiceAreaRepository>();
            MunicipalityRepoMock = new Mock<IMunicipalityRepository>();

            ServiceChannelRepoMock = new Mock<IServiceChannelVersionedRepository>();
            ServiceChannelNameRepoMock = new Mock<IServiceChannelNameRepository>();
            OrganizationVersionedRepoMock = new Mock<IOrganizationVersionedRepository>();
            KeywordRepoMock = new Mock<IKeywordRepository>();
            TargetGroupRepoMock = new Mock<ITargetGroupRepository>();
            ServiceCollectionRepoMock = new Mock<IServiceCollectionServiceRepository>();
            ServiceCollectionVersionedRepoMock = new Mock<IServiceCollectionVersionedRepository>();
            
            ServiceLanguageRepoMock = new Mock<IServiceLanguageRepository>();
            ServiceDescriptionRepoMock = new Mock<IServiceDescriptionRepository>();
            ServiceLanguageAvailabilityRepoMock = new Mock<IServiceLanguageAvailabilityRepository>();
            OrganizationServiceRepoMock = new Mock<IOrganizationServiceRepository>();
            ServiceRequirementRepoMock = new Mock<IServiceRequirementRepository>();
            ServiceAreaMunicipalityRepoMock = new Mock<IServiceAreaMunicipalityRepository>();
            ServiceWebPageRepoMock = new Mock<IServiceWebPageRepository>();
            OrganizationRepoMock = new Mock<IOrganizationRepository>();
            ServiceRootRepoMock = new Mock<IServiceRepository>();

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(ServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(ServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceNameRepository>()).Returns(ServiceNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceKeywordRepository>()).Returns(ServiceKeywordRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceServiceClassRepository>()).Returns(ServiceServiceClassRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceOntologyTermRepository>()).Returns(ServiceOntologyTermRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceLifeEventRepository>()).Returns(ServiceLifeEventRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceIndustrialClassRepository>()).Returns(ServiceIndustrialClassRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceTargetGroupRepository>()).Returns(ServiceTargetGroupRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceProducerRepository>()).Returns(ServiceProducerRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceLawRepository>()).Returns(ServiceLawRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceAreaRepository>()).Returns(ServiceAreaRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IMunicipalityRepository>()).Returns(MunicipalityRepoMock.Object);

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(ServiceChannelRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(ServiceChannelRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelNameRepository>()).Returns(ServiceChannelNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationVersionedRepository>()).Returns(OrganizationVersionedRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IExternalSourceRepository>()).Returns(ExternalSourceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IKeywordRepository>()).Returns(KeywordRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ITargetGroupRepository>()).Returns(TargetGroupRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceCollectionServiceRepository>()).Returns(ServiceCollectionRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceCollectionVersionedRepository>()).Returns(ServiceCollectionVersionedRepoMock.Object);
            
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceLanguageRepository>()).Returns(ServiceLanguageRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceDescriptionRepository>()).Returns(ServiceDescriptionRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceLanguageAvailabilityRepository>()).Returns(ServiceLanguageAvailabilityRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationServiceRepository>()).Returns(OrganizationServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceRequirementRepository>()).Returns(ServiceRequirementRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceAreaMunicipalityRepository>()).Returns(ServiceAreaMunicipalityRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceWebPageRepository>()).Returns(ServiceWebPageRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationRepository>()).Returns(OrganizationRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceRepository>()).Returns(ServiceRootRepoMock.Object);
        }

        internal OrganizationVersioned GetAndSetOrganizationForService(ServiceVersioned item, Guid statusId, string organizationName,
            Guid? languageId = null, bool setMain = true, bool setOtherResponsible = true, bool setProducer = true)
        {
            // Set organization data
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get("fi");
            var organization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(statusId);
            organization.OrganizationNames.Add(new OrganizationName { Name = organizationName, LocalizationId = langId, TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()) });
            organization.LanguageAvailabilities.Add(new OrganizationLanguageAvailability { StatusId = statusId, LanguageId = langId });

            // main responsible
            if (setMain)
            {
                item.Organization = new Model.Models.Organization { Id = organization.UnificRootId };
                item.Organization.Versions.Add(organization);
                item.OrganizationId = organization.UnificRootId;
                OrganizationRepoMock.Setup(o => o.All()).Returns(new List<Model.Models.Organization> { item.Organization }.AsQueryable());
            }
            // other responsible
            if (setOtherResponsible)
            {
                var organizationService = new Model.Models.OrganizationService
                {
                    Organization = organization.UnificRoot,
                    OrganizationId = organization.UnificRootId,
                    ServiceVersionedId = item.Id
                };
                
                item.OrganizationServices.Add(organizationService);
                OrganizationServiceRepoMock.Setup(o => o.All()).Returns(new List<Model.Models.OrganizationService> { organizationService }.AsQueryable());
            }
            
            // producer
            if (setProducer)
            {
                var producer1 = new ServiceProducer
                {
                    Organizations = new List<ServiceProducerOrganization> { new ServiceProducerOrganization {
                        Organization = organization.UnificRoot,
                        OrganizationId = organization.UnificRootId,
                    } },
                    ProvisionTypeId = TypeCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()),
                    ServiceVersionedId = item.Id
                };
                var producer2 = new ServiceProducer
                {
                    Organizations = new List<ServiceProducerOrganization> { new ServiceProducerOrganization {
                        Organization = organization.UnificRoot,
                        OrganizationId = organization.UnificRootId,
                    } },
                    ProvisionTypeId = TypeCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()),
                    ServiceVersionedId = item.Id
                };
                var producer3 = new ServiceProducer
                {
                    Organizations = new List<ServiceProducerOrganization> { new ServiceProducerOrganization {
                        Organization = organization.UnificRoot,
                        OrganizationId = organization.UnificRootId,
                    } },
                    ProvisionTypeId = TypeCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()),
                    ServiceVersionedId = item.Id
                };
                item.ServiceProducers.Add(producer1);
                item.ServiceProducers.Add(producer2);
                item.ServiceProducers.Add(producer3);
                ServiceProducerRepoMock.Setup(g => g.All()).Returns(new List<ServiceProducer> { producer1, producer2, producer3 }.AsQueryable());
            }

            return organization;
        }

        internal ServiceServiceChannel GetAndSetConnectionForService(ServiceVersioned item, Guid statusId, string channelName, Guid? languageId = null)
        {
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get("fi");
            var channel = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(statusId);
            channel.ServiceChannelNames.Add(new ServiceChannelName
            {
                Name = channelName,
                LocalizationId = langId,
                TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString())
            });
            channel.LanguageAvailabilities.Add(new ServiceChannelLanguageAvailability { StatusId = statusId, LanguageId = langId });

            var connection = new ServiceServiceChannel
            {
                ServiceChannel = channel.UnificRoot,
                ServiceChannelId = channel.UnificRootId,
                Service = item.UnificRoot,
                ServiceId = item.UnificRootId,
            };
            connection.ServiceServiceChannelPhones.Add(new ServiceServiceChannelPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()), TypeId = TypeCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()) }
            });
            connection.ServiceServiceChannelPhones.Add(new ServiceServiceChannelPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()), TypeId = TypeCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()) }
            });
            connection.ServiceServiceChannelPhones.Add(new ServiceServiceChannelPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Other.ToString()), TypeId = TypeCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()) }
            });
            //item.UnificRoot.ServiceServiceChannels.Add(connection);

            return connection;
        }

        protected void ArrangeTranslateService()
        {
            translationManagerMockSetup.Setup(t => t.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()))
                .Returns((ServiceVersioned entity) =>
                {
                    if (entity == null) return null;

                    var type = entity.TypeId.HasValue && entity.TypeId != Guid.Empty ? TypeCache.GetByValue<ServiceType>(entity.TypeId.Value) : null;
                    var chargeType = entity.ChargeTypeId.HasValue && entity.ChargeTypeId != Guid.Empty ? TypeCache.GetByValue<ServiceChargeType>(entity.ChargeTypeId.Value) : null;
                    var vm = new VmOpenApiServiceVersionBase
                    {
                        Id = entity.UnificRootId,
                        GeneralDescriptionId = entity.StatutoryServiceGeneralDescriptionId,
                        Type = string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceTypeEnum>(),
                        ServiceChargeType = string.IsNullOrEmpty(chargeType) ? null : chargeType.GetOpenApiEnumValue<ServiceChargeTypeEnum>(),
                        AreaType = entity.AreaInformationTypeId == null || entity.AreaInformationTypeId == Guid.Empty ? null
                            : TypeCache.GetByValue<AreaInformationType>(entity.AreaInformationTypeId).GetOpenApiEnumValue<AreaInformationTypeEnum>()
                    };
                    if (entity.PublishingStatusId.IsAssigned())
                    {
                        vm.PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId);
                    }
                    if (entity.ServiceNames?.Count > 0)
                    {
                        vm.ServiceNames = new List<VmOpenApiLocalizedListItem>();
                        entity.ServiceNames.ForEach(n => vm.ServiceNames.Add(new VmOpenApiLocalizedListItem
                        {
                            Value = n.Name,
                            Type = TypeCache.GetByValue<NameType>(n.TypeId).GetOpenApiEnumValue<NameTypeEnum>(),
                            Language = LanguageCache.GetByValue(n.LocalizationId)
                        }));
                    }
                    if (entity.ServiceDescriptions?.Count > 0)
                    {
                        vm.ServiceDescriptions = new List<VmOpenApiLocalizedListItem>();
                        entity.ServiceDescriptions.ForEach(d => vm.ServiceDescriptions.Add(new VmOpenApiLocalizedListItem
                        {
                            Value = d.Description,
                            Type = TypeCache.GetByValue<DescriptionType>(d.TypeId).GetOpenApiEnumValue<DescriptionTypeEnum>(),
                            Language = LanguageCache.GetByValue(d.LocalizationId)
                        }));
                    }
                    if (entity.Areas?.Count > 0)
                    {
                        vm.Areas = new List<VmOpenApiArea>();
                        entity.Areas.ForEach(a => vm.Areas.Add(new VmOpenApiArea
                        {
                            Type = a.Area == null ? null : TypeCache.GetByValue<AreaType>(a.Area.AreaTypeId).GetOpenApiEnumValue<AreaTypeEnum>(),
                        }));
                    }
                    if (entity.Organization != null)
                    {
                        var orgVersioned = entity.Organization.Versions.FirstOrDefault();
                        if (orgVersioned != null)
                        {
                            vm.Organizations.Add(GetOrganizationVm(orgVersioned, CommonConsts.RESPONSIBLE, null));
                        }
                    }
                    if (entity.OrganizationServices?.Count > 0)
                    {
                        entity.OrganizationServices.ForEach(os =>
                        {
                            var orgVersioned = os.Organization.Versions.FirstOrDefault();
                            if (orgVersioned != null)
                            {
                                vm.Organizations.Add(GetOrganizationVm(orgVersioned, CommonConsts.OTHER_RESPONSIBLE, null));
                            }
                        });
                    }
                    if (entity.ServiceProducers?.Count() > 0)
                    {
                        entity.ServiceProducers.ForEach(sp =>
                        {
                            var provisionType = sp.ProvisionTypeId == Guid.Empty ? null : TypeCache.GetByValue<ProvisionType>(sp.ProvisionTypeId).GetOpenApiEnumValue<ProvisionTypeEnum>();
                            sp.Organizations.ForEach(org =>
                            {
                                var orgVersioned = org.Organization.Versions.FirstOrDefault();
                                if (orgVersioned != null)
                                {
                                    vm.Organizations.Add(GetOrganizationVm(orgVersioned, CommonConsts.PRODUCER, provisionType));
                                }
                            });
                        });
                    }
                    if (entity.UnificRoot?.ServiceServiceChannels?.Count > 0)
                    {
                        vm.ServiceChannels = new List<V11VmOpenApiServiceServiceChannel>();
                        entity.UnificRoot.ServiceServiceChannels.ForEach(c =>
                        {
                            var channel = new V11VmOpenApiServiceServiceChannel
                            {
                                ServiceChannel = new VmOpenApiItem { Id = c.ServiceChannelId }
                            };
                            vm.ServiceChannels.Add(channel);
                        });
                    }
                    return vm;
                });

        }

        protected void ArrangeTranslateAllServices()
        {

            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<List<ServiceVersioned>>()))
                .Returns((List < ServiceVersioned> entities) =>
                {
                    if (entities == null) return null;
                    var list = new List<VmOpenApiServiceVersionBase>();
                    entities.ForEach(entity =>
                    {
                        var vm = new VmOpenApiServiceVersionBase
                        {
                            Id = entity.UnificRootId,
                            GeneralDescriptionId = entity.StatutoryServiceGeneralDescriptionId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId),
                        };
                        if (entity.ServiceNames?.Count > 0)
                        {
                            vm.ServiceNames = new List<VmOpenApiLocalizedListItem>();
                            entity.ServiceNames.ForEach(n => vm.ServiceNames.Add(new VmOpenApiLocalizedListItem { Value = n.Name }));
                        }
                        if (entity.Organization != null)
                        {
                            var orgVersioned = entity.Organization.Versions.FirstOrDefault();
                            if (orgVersioned != null)
                            {
                                vm.Organizations.Add(GetOrganizationVm(orgVersioned, CommonConsts.RESPONSIBLE, null));
                            }
                        }
                        if (entity.OrganizationServices?.Count > 0)
                        {
                            entity.OrganizationServices.ForEach(os =>
                            {
                                var orgVersioned = os.Organization.Versions.FirstOrDefault();
                                if (orgVersioned != null)
                                {
                                    vm.Organizations.Add(GetOrganizationVm(orgVersioned, CommonConsts.OTHER_RESPONSIBLE, null));
                                }
                            });
                        }
                        if (entity.ServiceProducers?.Count() > 0)
                        {
                            entity.ServiceProducers.ForEach(sp =>
                            {
                                var provisionType = sp.ProvisionTypeId == Guid.Empty ? null : TypeCache.GetByValue<ProvisionType>(sp.ProvisionTypeId).GetOpenApiEnumValue<ProvisionTypeEnum>();
                                sp.Organizations.ForEach(org =>
                                {
                                    var orgVersioned = org.Organization.Versions.FirstOrDefault();
                                    if (orgVersioned != null)
                                    {
                                        vm.Organizations.Add(GetOrganizationVm(orgVersioned, CommonConsts.PRODUCER, provisionType));
                                    }
                                });
                            });
                        }
                        if (entity.UnificRoot?.ServiceServiceChannels?.Count > 0)
                        {
                            vm.ServiceChannels = new List<V11VmOpenApiServiceServiceChannel>();
                            entity.UnificRoot.ServiceServiceChannels.ForEach(c =>
                            {
                                var channel = new V11VmOpenApiServiceServiceChannel
                                {
                                    ServiceChannel = new VmOpenApiItem { Id = c.ServiceChannelId }
                                };
                                vm.ServiceChannels.Add(channel);
                            });
                        }
                        list.Add(vm);
                    });
                    return list;
                });

        }

        private V6VmOpenApiServiceOrganization GetOrganizationVm(OrganizationVersioned ov, string roleType, string provisionType)
        {
            if (ov == null) return null;

            var vm = new V6VmOpenApiServiceOrganization();
            var publishedLangIds = ov.LanguageAvailabilities.Where(l => l.StatusId == PublishedId).Select(l => l.LanguageId).ToList();
            var orgName = ov.OrganizationNames.Where(n => publishedLangIds.Contains(n.LocalizationId)).FirstOrDefault();
            return new V6VmOpenApiServiceOrganization
            {
                Organization = new VmOpenApiItem { Id = ov.UnificRootId, Name = orgName == null ? null : orgName.Name },
                RoleType = roleType,
                ProvisionType = provisionType,
            };
        }
    }
}
