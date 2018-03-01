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
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class ServiceServiceTestBase : ServiceTestBase
    {
        internal ILogger<DataAccess.Services.ServiceService> Logger { get; private set; }

        internal IGeneralDescriptionService gdService { get; private set; }

        internal Mock<IGeneralDescriptionService> gdServiceMock { get; private set; }

        internal Mock<IServiceVersionedRepository> ServiceRepoMock { get; private set; }

        internal Mock<IServiceNameRepository> ServiceNameRepoMock { get; private set; }

        internal Mock<IServiceChannelVersionedRepository> ServiceChannelRepoMock { get; private set; }

        internal Mock<IServiceChannelNameRepository> ServiceChannelNameRepoMock { get; private set; }

        internal Mock<IOrganizationVersionedRepository> OrganizationVersionedRepoMock { get; private set; }

        internal Mock<IKeywordRepository> KeywordRepoMock { get; private set; }

        internal Mock<ITargetGroupRepository> TargetGroupRepoMock { get; private set; }

        internal Mock<IServiceCollectionServiceRepository> ServiceCollectionRepoMock { get; private set; }

        public ServiceServiceTestBase()
        {
            Logger = new Mock<ILogger<ServiceService>>().Object;

            gdServiceMock = new Mock<IGeneralDescriptionService>();
            gdService = gdServiceMock.Object;

            ServiceRepoMock = new Mock<IServiceVersionedRepository>();
            ServiceNameRepoMock = new Mock<IServiceNameRepository>();
            ServiceChannelRepoMock = new Mock<IServiceChannelVersionedRepository>();
            ServiceChannelNameRepoMock = new Mock<IServiceChannelNameRepository>();
            OrganizationVersionedRepoMock = new Mock<IOrganizationVersionedRepository>();
            KeywordRepoMock = new Mock<IKeywordRepository>();
            TargetGroupRepoMock = new Mock<ITargetGroupRepository>();
            ServiceCollectionRepoMock = new Mock<IServiceCollectionServiceRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(ServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(ServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceNameRepository>()).Returns(ServiceNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(ServiceChannelRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelNameRepository>()).Returns(ServiceChannelNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationVersionedRepository>()).Returns(OrganizationVersionedRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IExternalSourceRepository>()).Returns(ExternalSourceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IKeywordRepository>()).Returns(KeywordRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ITargetGroupRepository>()).Returns(TargetGroupRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceCollectionServiceRepository>()).Returns(ServiceCollectionRepoMock.Object);
        }

        internal OrganizationVersioned GetAndSetOrganizationForService(ServiceVersioned item, Guid statusId, string organizationName,
            Guid? languageId = null, bool setMain = true, bool setOtherResponsible = true, bool setProducer = true)
        {
            // Set organization data
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get(LanguageCode.fi.ToString());
            var organization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(statusId);
            organization.OrganizationNames.Add(new OrganizationName { Name = organizationName, LocalizationId = langId, TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()) });
            organization.LanguageAvailabilities.Add(new OrganizationLanguageAvailability { StatusId = statusId, LanguageId = langId });
            organization.UnificRoot = new Model.Models.Organization { Id = organization.UnificRootId };
            // main responsible
            if (setMain)
            {
                item.Organization = new Model.Models.Organization { Id = organization.UnificRootId };
                item.Organization.Versions.Add(organization);
                item.OrganizationId = organization.UnificRootId;
            }
            // other responsible
            if (setOtherResponsible)
            {
                item.OrganizationServices.Add(new Model.Models.OrganizationService
                {
                    Organization = organization.UnificRoot,
                    OrganizationId = organization.UnificRootId
                });
            }
            // producer
            if (setProducer)
            {
                item.ServiceProducers.Add(new ServiceProducer
                {
                    Organizations = new List<ServiceProducerOrganization> { new ServiceProducerOrganization {
                        Organization = organization.UnificRoot,
                        OrganizationId = organization.UnificRootId,
                    } },
                    ProvisionTypeId = TypeCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()),
                });
                item.ServiceProducers.Add(new ServiceProducer
                {
                    Organizations = new List<ServiceProducerOrganization> { new ServiceProducerOrganization {
                        Organization = organization.UnificRoot,
                        OrganizationId = organization.UnificRootId,
                    } },
                    ProvisionTypeId = TypeCache.Get<ProvisionType>(ProvisionTypeEnum.PurchaseServices.ToString()),
                });
                item.ServiceProducers.Add(new ServiceProducer
                {
                    Organizations = new List<ServiceProducerOrganization> { new ServiceProducerOrganization {
                        Organization = organization.UnificRoot,
                        OrganizationId = organization.UnificRootId,
                    } },
                    ProvisionTypeId = TypeCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()),
                });
            }

            return organization;
        }

        internal ServiceServiceChannel GetAndSetConnectionForService(ServiceVersioned item, Guid statusId, string channelName, Guid? languageId = null)
        {
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get(LanguageCode.fi.ToString());
            var channel = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(statusId);
            channel.ServiceChannelNames.Add(new ServiceChannelName
            {
                Name = channelName,
                LocalizationId = langId,
                TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString())
            });
            channel.LanguageAvailabilities.Add(new ServiceChannelLanguageAvailability { StatusId = statusId, LanguageId = langId });
            channel.UnificRoot = new ServiceChannel { Id = channel.UnificRootId };
            channel.UnificRoot.Versions.Add(channel);
            var connection = new ServiceServiceChannel
            {
                ServiceChannel = channel.UnificRoot,
                ServiceChannelId = channel.UnificRootId,
                Service = item.UnificRoot,
                ServiceId = item.UnificRootId,
            };
            connection.ServiceServiceChannelPhones.Add(new ServiceServiceChannelPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()) }
            });
            connection.ServiceServiceChannelPhones.Add(new ServiceServiceChannelPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()) }
            });
            connection.ServiceServiceChannelPhones.Add(new ServiceServiceChannelPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Other.ToString()) }
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
                    var vm = new VmOpenApiServiceVersionBase()
                    {
                        Id = entity.UnificRootId,
                        GeneralDescriptionId = entity.StatutoryServiceGeneralDescriptionId,
                        Type = string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceTypeEnum>(),
                        ServiceChargeType = string.IsNullOrEmpty(chargeType) ? null : chargeType.GetOpenApiEnumValue<ServiceChargeTypeEnum>(),
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId),
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
                            vm.Organizations.Add(GetOrganizationVm(orgVersioned, "Responsible", null));
                        }
                    }
                    if (entity.OrganizationServices?.Count > 0)
                    {
                        entity.OrganizationServices.ForEach(os =>
                        {
                            var orgVersioned = os.Organization.Versions.FirstOrDefault();
                            if (orgVersioned != null)
                            {
                                vm.Organizations.Add(GetOrganizationVm(orgVersioned, "OtherResponsible", null));
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
                                    vm.Organizations.Add(GetOrganizationVm(orgVersioned, "Producer", provisionType));
                                }
                            });
                        });
                    }
                    if (entity.UnificRoot?.ServiceServiceChannels?.Count > 0)
                    {
                        vm.ServiceChannels = new List<V8VmOpenApiServiceServiceChannel>();
                        entity.UnificRoot.ServiceServiceChannels.ForEach(c =>
                        {
                            var channel = new V8VmOpenApiServiceServiceChannel
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
                        var vm = new VmOpenApiServiceVersionBase()
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
                                vm.Organizations.Add(GetOrganizationVm(orgVersioned, "Responsible", null));
                            }
                        }
                        if (entity.OrganizationServices?.Count > 0)
                        {
                            entity.OrganizationServices.ForEach(os =>
                            {
                                var orgVersioned = os.Organization.Versions.FirstOrDefault();
                                if (orgVersioned != null)
                                {
                                    vm.Organizations.Add(GetOrganizationVm(orgVersioned, "OtherResponsible", null));
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
                                        vm.Organizations.Add(GetOrganizationVm(orgVersioned, "Producer", provisionType));
                                    }
                                });
                            });
                        }
                        if (entity.UnificRoot?.ServiceServiceChannels?.Count > 0)
                        {
                            vm.ServiceChannels = new List<V8VmOpenApiServiceServiceChannel>();
                            entity.UnificRoot.ServiceServiceChannels.ForEach(c =>
                            {
                                var channel = new V8VmOpenApiServiceServiceChannel
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
