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
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public abstract class ChannelServiceTestBase : ServiceTestBase
    {
        internal const string USERNAME = "userName";
        internal const string NAME = "name";
        internal const string DESCRIPTION = "description";

        internal ILogger<ChannelService> Logger { get; private set; }
        internal Mock<IServiceChannelVersionedRepository> ChannelRepoMock { get; private set; }
        internal Mock<IElectronicChannelRepository> EChannelRepoMock { get; private set; }
        internal Mock<IPrintableFormChannelRepository> PrintableFormChannelRepoMock { get; private set; }
        internal Mock<IWebpageChannelRepository> WebPageChannelRepoMock { get; private set; }
        internal Mock<IServiceChannelEmailRepository> ChannelEmailRepoMock { get; private set; }
        internal Mock<IServiceChannelPhoneRepository> ChannelPhoneRepoMock { get; private set; }
        internal Mock<IServiceChannelWebPageRepository> ChannelWebPageRepoMock { get; private set; }
        internal Mock<IServiceChannelAddressRepository> ChannelAddressRepoMock { get; private set; }
        internal Mock<IServiceChannelAreaRepository> ChannelAreaRepoMock { get; private set; }
        internal Mock<IMunicipalityRepository> MunicipalityRepoMock { get; private set; }
        internal Mock<IServiceChannelServiceHoursRepository> ChannelServiceHourRepoMock { get; private set; }
        internal Mock<IServiceVersionedRepository> ServiceRepoMock { get; private set; }
        internal Mock<IAccessibilityRegisterRepository> AccessibilityRegisterRepoMock { get; private set; }
        internal Mock<IPostalCodeRepository> PostalCodeRepositoryMock { get; private set; }
        internal Mock<IStatutoryServiceGeneralDescriptionVersionedRepository> GeneralDescriptionRepoMock { get; private set; }
        internal Mock<IOntologyTermRepository> OntologyTermRepoMock { get; private set; }
        internal Mock<IServiceChannelSocialHealthCenterRepository> SocialHealthCenterRepoMock { get; private set; }
        internal Mock<IAddressRepository> AddressRepoMock { get; private set; }
        internal Mock<IClsAddressPointRepository> ClsAddressPointRepoMock { get; private set; }
        internal Mock<IAddressPostOfficeBoxRepository> AddressPostOfficeBoxRepoMock { get; private set; }
        internal Mock<IAddressForeignRepository> AddressForeignRepoRepoMock { get; private set; }
        internal Mock<IAddressOtherRepository> AddressOtherRepoMock { get; private set; }
        internal Mock<ICountryRepository> CountryRepoMock { get; private set; }
        internal Mock<IAddressAdditionalInformationRepository> AddressAdditionalInformationRepoMock { get; private set; }
        internal Mock<IAddressCoordinateRepository> AddressCoordinateRepoMock { get; private set; }
        internal Mock<IAddressReceiverRepository> AddressReceiverRepoMock { get; private set; }
        internal Mock<IServiceCollectionServiceChannelRepository> ServiceCollectionChannelRepoMock { get; private set; }
        internal Mock<IServiceCollectionVersionedRepository> ServiceCollectionVersionedRepoMock { get; private set; }
        
        public ChannelServiceTestBase()
        {
            Logger = new Mock<ILogger<ChannelService>>().Object;
            ChannelRepoMock = new Mock<IServiceChannelVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(ChannelRepoMock.Object);
            EChannelRepoMock = new Mock<IElectronicChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IElectronicChannelRepository>()).Returns(EChannelRepoMock.Object);
            PrintableFormChannelRepoMock = new Mock<IPrintableFormChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IPrintableFormChannelRepository>()).Returns(PrintableFormChannelRepoMock.Object);
            WebPageChannelRepoMock = new Mock<IWebpageChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IWebpageChannelRepository>()).Returns(WebPageChannelRepoMock.Object);
            ChannelEmailRepoMock = new Mock<IServiceChannelEmailRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelEmailRepository>()).Returns(ChannelEmailRepoMock.Object);
            ChannelPhoneRepoMock = new Mock<IServiceChannelPhoneRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelPhoneRepository>()).Returns(ChannelPhoneRepoMock.Object);
            ChannelWebPageRepoMock = new Mock<IServiceChannelWebPageRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelWebPageRepository>()).Returns(ChannelWebPageRepoMock.Object);
            ChannelAddressRepoMock = new Mock<IServiceChannelAddressRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelAddressRepository>()).Returns(ChannelAddressRepoMock.Object);
            ChannelAreaRepoMock = new Mock<IServiceChannelAreaRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelAreaRepository>()).Returns(ChannelAreaRepoMock.Object);
            MunicipalityRepoMock = new Mock<IMunicipalityRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IMunicipalityRepository>()).Returns(MunicipalityRepoMock.Object);
            ChannelServiceHourRepoMock = new Mock<IServiceChannelServiceHoursRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelServiceHoursRepository>()).Returns(ChannelServiceHourRepoMock.Object);
            ServiceRepoMock = new Mock<IServiceVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(ServiceRepoMock.Object);
            AccessibilityRegisterRepoMock = new Mock<IAccessibilityRegisterRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAccessibilityRegisterRepository>()).Returns(AccessibilityRegisterRepoMock.Object);
            PostalCodeRepositoryMock = new Mock<IPostalCodeRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IPostalCodeRepository>()).Returns(PostalCodeRepositoryMock.Object);
            GeneralDescriptionRepoMock = new Mock<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>()).Returns(GeneralDescriptionRepoMock.Object);
            OntologyTermRepoMock = new Mock<IOntologyTermRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOntologyTermRepository>()).Returns(OntologyTermRepoMock.Object);
            SocialHealthCenterRepoMock = new Mock<IServiceChannelSocialHealthCenterRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelSocialHealthCenterRepository>()).Returns(SocialHealthCenterRepoMock.Object);
            AddressRepoMock = new Mock<IAddressRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAddressRepository>()).Returns(AddressRepoMock.Object);
            ClsAddressPointRepoMock = new Mock<IClsAddressPointRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IClsAddressPointRepository>()).Returns(ClsAddressPointRepoMock.Object);
            AddressPostOfficeBoxRepoMock = new Mock<IAddressPostOfficeBoxRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAddressPostOfficeBoxRepository>()).Returns(AddressPostOfficeBoxRepoMock.Object);
            AddressForeignRepoRepoMock = new Mock<IAddressForeignRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAddressForeignRepository>()).Returns(AddressForeignRepoRepoMock.Object);
            AddressOtherRepoMock = new Mock<IAddressOtherRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAddressOtherRepository>()).Returns(AddressOtherRepoMock.Object);
            CountryRepoMock = new Mock<ICountryRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ICountryRepository>()).Returns(CountryRepoMock.Object);
            AddressAdditionalInformationRepoMock = new Mock<IAddressAdditionalInformationRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAddressAdditionalInformationRepository>()).Returns(AddressAdditionalInformationRepoMock.Object);
            AddressCoordinateRepoMock = new Mock<IAddressCoordinateRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAddressCoordinateRepository>()).Returns(AddressCoordinateRepoMock.Object);
            AddressReceiverRepoMock = new Mock<IAddressReceiverRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAddressReceiverRepository>()).Returns(AddressReceiverRepoMock.Object);
            ServiceCollectionChannelRepoMock = new Mock<IServiceCollectionServiceChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceCollectionServiceChannelRepository>())
                .Returns(ServiceCollectionChannelRepoMock.Object);
            ServiceCollectionVersionedRepoMock = new Mock<IServiceCollectionVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceCollectionVersionedRepository>())
                .Returns(ServiceCollectionVersionedRepoMock.Object);
        }

        internal ChannelService ArrangeForGet(List<ServiceChannelVersioned> list, ServiceChannelTypeEnum channelType)
        {
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.TypeId = TypeCache.Get<ServiceChannelType>(channelType.ToString());
            publishedChannel.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());
            publishedChannel.Areas.Add(new ServiceChannelArea { ServiceChannelVersionedId = publishedChannel.Id, Area = new Area { AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString()) } });
            publishedChannel.Areas.Add(new ServiceChannelArea { ServiceChannelVersionedId = publishedChannel.Id, Area = new Area { AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString()) } });
            publishedChannel.Areas.Add(new ServiceChannelArea { ServiceChannelVersionedId = publishedChannel.Id, Area = new Area { AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Municipality.ToString()) } });
            publishedChannel.Areas.Add(new ServiceChannelArea { ServiceChannelVersionedId = publishedChannel.Id, Area = new Area { AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString()) } });
            publishedChannel.ServiceChannelNames.Add(new ServiceChannelName { Name = NAME, TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()) });
            publishedChannel.ServiceChannelDescriptions.Add(new ServiceChannelDescription { Description = DESCRIPTION, TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) });
            publishedChannel.Phones.Add(new ServiceChannelPhone { ServiceChannelVersionedId = publishedChannel.Id, Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()) } });
            publishedChannel.Phones.Add(new ServiceChannelPhone { ServiceChannelVersionedId = publishedChannel.Id, Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()) } });
            publishedChannel.Phones.Add(new ServiceChannelPhone { ServiceChannelVersionedId = publishedChannel.Id, Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Other.ToString()) } });
            publishedChannel.ServiceChannelServiceHours.Add(new ServiceChannelServiceHours { ServiceChannelVersionedId = publishedChannel.Id, ServiceHours = new ServiceHours { ServiceHourTypeId = TypeCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Standard.ToString()) } });
            publishedChannel.ServiceChannelServiceHours.Add(new ServiceChannelServiceHours { ServiceChannelVersionedId = publishedChannel.Id, ServiceHours = new ServiceHours { ServiceHourTypeId = TypeCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString()) } });
            publishedChannel.ServiceChannelServiceHours.Add(new ServiceChannelServiceHours { ServiceChannelVersionedId = publishedChannel.Id, ServiceHours = new ServiceHours { ServiceHourTypeId = TypeCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Special.ToString()) } });
            var id = publishedChannel.Id;
            var rootId = publishedChannel.UnificRootId;

            switch (channelType)
            {
                case ServiceChannelTypeEnum.EChannel:
                    Mock<IElectronicChannelRepository> eChannelRepoMock = new Mock<IElectronicChannelRepository>();
                    unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IElectronicChannelRepository>()).Returns(eChannelRepoMock.Object);
                    publishedChannel.ElectronicChannels.Add(new ElectronicChannel { ServiceChannelVersionedId = publishedChannel.Id, LocalizedUrls = new List<ElectronicChannelUrl> { new ElectronicChannelUrl { WebPage = new WebPage { Url = "Url" } } } });
                    eChannelRepoMock.Setup(g => g.All()).Returns(publishedChannel.ElectronicChannels.AsQueryable());
                    break;
                case ServiceChannelTypeEnum.WebPage:
                    publishedChannel.WebPages.Add(new ServiceChannelWebPage { ServiceChannelVersionedId = publishedChannel.Id, WebPage = new WebPage() });
                    break;
                case ServiceChannelTypeEnum.PrintableForm:
                    publishedChannel.WebPages.Add(new ServiceChannelWebPage { ServiceChannelVersionedId = publishedChannel.Id, WebPage = new WebPage() });
                    break;
                case ServiceChannelTypeEnum.Phone:
                    publishedChannel.WebPages.Add(new ServiceChannelWebPage { ServiceChannelVersionedId = publishedChannel.Id, WebPage = new WebPage() });
                    break;
                case ServiceChannelTypeEnum.ServiceLocation:
                    publishedChannel.WebPages.Add(new ServiceChannelWebPage { ServiceChannelVersionedId = publishedChannel.Id, WebPage = new WebPage() });
                    break;
                default:
                    break;
            }

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, PublishingStatus.Published, true)).Returns(id);

            // repositories
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ChannelAreaRepoMock.Setup(g => g.All()).Returns(publishedChannel.Areas.AsQueryable());
            ChannelPhoneRepoMock.Setup(g => g.All()).Returns(publishedChannel.Phones.AsQueryable());
            ChannelServiceHourRepoMock.Setup(g => g.All()).Returns(publishedChannel.ServiceChannelServiceHours.AsQueryable());
            if (publishedChannel.WebPages?.Count > 0)
            {
                ChannelWebPageRepoMock.Setup(g => g.All()).Returns(publishedChannel.WebPages.AsQueryable());
            }

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<ServiceChannelVersioned>>(),
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<ServiceChannelVersioned> channelServices, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
                {
                    return channelServices;
                });
            // Channel connections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceServiceChannel>>(),
               It.IsAny<Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceServiceChannel> items, Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>> func, bool applyFilters) =>
               {
                   return items;
               });
            // Related services
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceVersioned> items, Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });

            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            ArrangeTranslationManager(channelType);

            return new ChannelService(
                contextManager,
                translationManagerMockSetup.Object,
                TranslationManagerVModel,
                Logger,
                serviceUtilities,
                CommonService,
                AddressService,
                CacheManager,
                PublishingStatusCache,
                VersioningManager,
                UserOrganizationChecker,
                LanguageOrderCache,
                null,
                UrlServiceMock.Object,
                ExpirationServiceMock.Object,
                null,
                null);
        }

        internal void ArrangeTranslationManager(ServiceChannelTypeEnum? channelType = null)
        {
            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.EChannel) return new VmOpenApiElectronicChannelVersionBase();
                    var vm = GetChannel<VmOpenApiElectronicChannelVersionBase>(entity);
                    if (entity.ElectronicChannels?.Count > 0)
                    {
                        entity.ElectronicChannels.First().LocalizedUrls?.ToList().ForEach(u => vm.WebPages.Add(new V9VmOpenApiWebPage()));
                    }
                    return vm;
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.Phone) return new VmOpenApiPhoneChannelVersionBase();
                    var vm = GetChannel<VmOpenApiPhoneChannelVersionBase>(entity);
                    if (entity.Phones?.Count > 0)
                    {
                        vm.PhoneNumbers = new List<V4VmOpenApiPhoneWithType>();
                        entity.Phones.ForEach(p => vm.PhoneNumbers.Add(new V4VmOpenApiPhoneWithType
                        {
                            ServiceChargeType = p.Phone != null && p.Phone.ChargeTypeId != Guid.Empty ? TypeCache.GetByValue<ServiceChargeType>(p.Phone.ChargeTypeId).GetOpenApiEnumValue<ServiceChargeTypeEnum>() : null
                        }));
                    }
                    if (entity.WebPages?.Count > 0)
                    {
                        entity.WebPages.ForEach(u => vm.WebPages.Add(new V9VmOpenApiWebPage()));
                    }
                    return vm;
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.ServiceLocation) return new VmOpenApiServiceLocationChannelVersionBase();
                    var vm = GetChannel<VmOpenApiServiceLocationChannelVersionBase>(entity);
                    if (entity.Phones?.Count > 0)
                    {
                        vm.PhoneNumbers = new List<V4VmOpenApiPhoneWithType>();
                        entity.Phones.ForEach(p => vm.PhoneNumbers.Add(new V4VmOpenApiPhoneWithType
                        {
                            ServiceChargeType = p.Phone != null && p.Phone.ChargeTypeId != Guid.Empty ? TypeCache.GetByValue<ServiceChargeType>(p.Phone.ChargeTypeId).GetOpenApiEnumValue<ServiceChargeTypeEnum>() : null
                        }));
                    }
                    if (entity.WebPages?.Count > 0)
                    {
                        entity.WebPages.ForEach(u => vm.WebPages.Add(new V9VmOpenApiWebPage()));
                    }
                    if (entity.Addresses?.Count > 0)
                    {
                        vm.Addresses = new List<V9VmOpenApiAddressLocation>();
                        entity.Addresses.ForEach(a =>
                        {
                            var vmAddress = new V9VmOpenApiAddressLocation();
                            if (a.Address?.ClsAddressPoints?.Count > 0)
                            {
                                var item = a.Address.ClsAddressPoints.First();
                                vmAddress.StreetAddress = new VmOpenApiAddressStreetWithCoordinates
                                {
                                    PostalCode = item.PostalCode.Code,
                                    Municipality = new VmOpenApiMunicipality { Code = item.Municipality.Code }
                                };
                            }
                            vm.Addresses.Add(vmAddress);
                        });
                    }
                    return vm;
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.PrintableForm) return new VmOpenApiPrintableFormChannelVersionBase();
                    var vm = GetChannel<VmOpenApiPrintableFormChannelVersionBase>(entity);

                    if (entity.WebPages?.Count > 0)
                    {
                        entity.WebPages.ForEach(u => vm.WebPages.Add(new V9VmOpenApiWebPage()));
                    }

                    if (entity.Addresses?.Count > 0)
                    {
                        vm.DeliveryAddresses = new List<V8VmOpenApiAddressDelivery>();
                        entity.Addresses.ForEach(a =>
                        {
                            var vmAddress = new V8VmOpenApiAddressDelivery();
                            if (a.Address?.ClsAddressPoints?.Count > 0)
                            {
                                var item = a.Address.ClsAddressPoints.First();
                                vmAddress.StreetAddress = new VmOpenApiAddressStreet
                                {
                                    PostalCode = item.PostalCode.Code,
                                    Municipality = new VmOpenApiMunicipality { Code = item.Municipality?.Code}
                                };
                            }
                            vm.DeliveryAddresses.Add(vmAddress);
                        });
                    }
                    return vm;
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.WebPage) return new VmOpenApiWebPageChannelVersionBase();
                    var vm = GetChannel<VmOpenApiWebPageChannelVersionBase>(entity);
                    if (entity.WebPages?.Count > 0)
                    {
                        entity.WebPages.ForEach(u => vm.WebPages.Add(new V9VmOpenApiWebPage()));
                    }
                    return vm;
                });

            translationManagerMockSetup.Setup(t => t.TranslateAll<OntologyTerm, V4VmOpenApiOntologyTerm>(It.IsAny<IEnumerable<OntologyTerm>>()))
                .Returns((IEnumerable<OntologyTerm> ot) => ot.Select(o => new V4VmOpenApiOntologyTerm(){ Id = o.Id}).ToList());
        }

        internal void ArrangeAllTranslationManager(ServiceChannelTypeEnum? channelType = null)
        {
            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> entities) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.EChannel) return new List<VmOpenApiElectronicChannelVersionBase>();
                    var list = new List<VmOpenApiElectronicChannelVersionBase>();
                    entities.ForEach(entity =>
                    {
                        list.Add(new VmOpenApiElectronicChannelVersionBase
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        });
                    });
                    return list;
                });

            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> entities) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.Phone) return new List<VmOpenApiPhoneChannelVersionBase>();
                    var list = new List<VmOpenApiPhoneChannelVersionBase>();
                    entities.ForEach(entity =>
                    {
                        list.Add(new VmOpenApiPhoneChannelVersionBase
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        });
                    });
                    return list;
                });

            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> entities) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.ServiceLocation) return new List<VmOpenApiServiceLocationChannelVersionBase>();
                    var list = new List<VmOpenApiServiceLocationChannelVersionBase>();
                    entities.ForEach(entity =>
                    {
                        list.Add(new VmOpenApiServiceLocationChannelVersionBase
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        });
                    });
                    return list;
                });

            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> entities) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.PrintableForm) return new List<VmOpenApiPrintableFormChannelVersionBase>();
                    var list = new List<VmOpenApiPrintableFormChannelVersionBase>();
                    entities.ForEach(entity =>
                    {
                        list.Add(new VmOpenApiPrintableFormChannelVersionBase
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        });
                    });
                    return list;
                });

            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> entities) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.WebPage) return new List<VmOpenApiWebPageChannelVersionBase>();
                    var list = new List<VmOpenApiWebPageChannelVersionBase>();
                    entities.ForEach(entity =>
                    {
                        list.Add(new VmOpenApiWebPageChannelVersionBase
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        });
                    });
                    return list;
                });
        }

        internal ServiceChannelVersioned GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum? type)
        {
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            entity.TypeId = TypeCache.Get<ServiceChannelType>(type.HasValue ? type.ToString() : ServiceChannelTypeEnum.Phone.ToString());

            switch (type)
            {
                case ServiceChannelTypeEnum.EChannel:
                    entity.ElectronicChannels = new List<ElectronicChannel> { new ElectronicChannel { ServiceChannelVersionedId = entity.Id } };
                    var eChannelRepoMock = new Mock<IElectronicChannelRepository>();
                    unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IElectronicChannelRepository>()).Returns(eChannelRepoMock.Object);
                    eChannelRepoMock.Setup(g => g.All()).Returns(entity.ElectronicChannels.AsQueryable());
                    unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                    It.IsAny<IQueryable<ElectronicChannel>>(),
                    It.IsAny<Func<IQueryable<ElectronicChannel>, IQueryable<ElectronicChannel>>>(),
                    It.IsAny<bool>()
                    )).Returns((IQueryable<ElectronicChannel> channelServices, Func<IQueryable<ElectronicChannel>, IQueryable<ElectronicChannel>> func, bool applyFilters) =>
                    {
                        return channelServices;
                    });
                    break;
                case ServiceChannelTypeEnum.WebPage:
                    entity.WebpageChannels = new List<WebpageChannel> { new WebpageChannel { ServiceChannelVersionedId = entity.Id } };
                    var wChannelRepoMock = new Mock<IWebpageChannelRepository>();
                    unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IWebpageChannelRepository>()).Returns(wChannelRepoMock.Object);
                    wChannelRepoMock.Setup(g => g.All()).Returns(entity.WebpageChannels.AsQueryable());
                    unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                    It.IsAny<IQueryable<WebpageChannel>>(),
                    It.IsAny<Func<IQueryable<WebpageChannel>, IQueryable<WebpageChannel>>>(),
                    It.IsAny<bool>()
                    )).Returns((IQueryable<WebpageChannel> channelServices, Func<IQueryable<WebpageChannel>, IQueryable<WebpageChannel>> func, bool applyFilters) =>
                    {
                        return channelServices;
                    });
                    break;
                case ServiceChannelTypeEnum.PrintableForm:
                    entity.PrintableFormChannels = new List<PrintableFormChannel> { new PrintableFormChannel { ServiceChannelVersionedId = entity.Id } };
                    var pChannelRepoMock = new Mock<IPrintableFormChannelRepository>();
                    unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IPrintableFormChannelRepository>()).Returns(pChannelRepoMock.Object);
                    pChannelRepoMock.Setup(g => g.All()).Returns(entity.PrintableFormChannels.AsQueryable());
                    unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                    It.IsAny<IQueryable<PrintableFormChannel>>(),
                    It.IsAny<Func<IQueryable<PrintableFormChannel>, IQueryable<PrintableFormChannel>>>(),
                    It.IsAny<bool>()
                    )).Returns((IQueryable<PrintableFormChannel> channelServices, Func<IQueryable<PrintableFormChannel>, IQueryable<PrintableFormChannel>> func, bool applyFilters) =>
                    {
                        return channelServices;
                    });
                    break;
                case ServiceChannelTypeEnum.ServiceLocation:
                    unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                    It.IsAny<IQueryable<AccessibilityRegister>>(),
                    It.IsAny<Func<IQueryable<AccessibilityRegister>, IQueryable<AccessibilityRegister>>>(),
                    It.IsAny<bool>()
                    )).Returns((IQueryable<AccessibilityRegister> registers, Func<IQueryable<AccessibilityRegister>, IQueryable<AccessibilityRegister>> func, bool applyFilters) =>
                    {
                        return registers;
                    });
                    break;
                case ServiceChannelTypeEnum.Phone:
                default:
                    break;
            }

            return entity;
        }

        internal ServiceServiceChannel GetAndSetConnectionForChannel(ServiceChannelVersioned item, Guid statusId, string channelName, Guid? languageId = null)
        {
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get("fi");
            var service = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(statusId);
            service.ServiceNames.Add(new ServiceName { Name = channelName, LocalizationId = langId, TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()) });
            service.LanguageAvailabilities.Add(new ServiceLanguageAvailability { StatusId = statusId, LanguageId = langId });
            service.ServiceOntologyTerms.Add(new ServiceOntologyTerm{ OntologyTerm = new OntologyTerm{Id = "Onto1".GetGuid()}});
            service.ServiceOntologyTerms.Add(new ServiceOntologyTerm{ OntologyTerm = new OntologyTerm{Id = "Onto2".GetGuid()}});
            service.StatutoryServiceGeneralDescription = new StatutoryServiceGeneralDescription
            {
                Versions = new List<StatutoryServiceGeneralDescriptionVersioned>
                {
                    new StatutoryServiceGeneralDescriptionVersioned
                    {
                        UnificRootId = "GdOnto".GetGuid(),
                        PublishingStatusId = statusId,
                        OntologyTerms = new List<StatutoryServiceOntologyTerm>
                        {
                            new StatutoryServiceOntologyTerm
                            {
                                OntologyTerm = new OntologyTerm{Id = "Onto1".GetGuid()}
                            },
                            new StatutoryServiceOntologyTerm
                            {
                                OntologyTerm = new OntologyTerm{Id = "Onto3".GetGuid()}
                            }
                        }
                    }
                }
            };
            service.StatutoryServiceGeneralDescriptionId = "GdOnto".GetGuid();

            var connection = new ServiceServiceChannel
            {
                ServiceChannel = item.UnificRoot,
                ServiceChannelId = item.UnificRootId,
                Service = service.UnificRoot,
                ServiceId = service.UnificRootId
            };

            connection.Service.ServiceServiceChannels = new List<ServiceServiceChannel>{connection};

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

            return connection;
        }

        private TModel GetChannel<TModel>(ServiceChannelVersioned entity) where TModel : IVmOpenApiServiceChannel, new()
        {
            var vm = new TModel
            {
                Id = entity.UnificRootId,
                PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId),
                AreaType = entity.AreaInformationTypeId.IsAssigned() ? TypeCache.GetByValue<AreaInformationType>(entity.AreaInformationTypeId.Value).GetOpenApiEnumValue<AreaInformationTypeEnum>() : null
            };
            if (entity.ServiceChannelNames?.Count > 0)
            {
                vm.ServiceChannelNames = new List<VmOpenApiLocalizedListItem>();
                entity.ServiceChannelNames.ForEach(n => vm.ServiceChannelNames.Add(new VmOpenApiLocalizedListItem
                {
                    Type = TypeCache.GetByValue<NameType>(n.TypeId).GetOpenApiEnumValue<NameTypeEnum>(),
                    Value = n.Name
                }));
            }

            if (entity.ServiceChannelDescriptions?.Count > 0)
            {
                vm.ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem>();
                entity.ServiceChannelDescriptions.ForEach(d => vm.ServiceChannelDescriptions.Add(new VmOpenApiLocalizedListItem
                {
                    Type = TypeCache.GetByValue<DescriptionType>(d.TypeId).GetOpenApiEnumValue<DescriptionTypeEnum>(),
                    Value = d.Description
                }));
            };

            if (entity.Areas?.Count > 0)
            {
                vm.Areas = new List<VmOpenApiArea>();
                entity.Areas.ForEach(a => vm.Areas.Add(new VmOpenApiArea
                {
                    Type = a.Area != null && a.Area.AreaTypeId != Guid.Empty ? TypeCache.GetByValue<AreaType>(a.Area.AreaTypeId).GetOpenApiEnumValue<AreaTypeEnum>() : null
                }));
            }

            if (entity.ServiceChannelServiceHours?.Count > 0)
            {
                vm.ServiceHours = new List<V11VmOpenApiServiceHour>();
                entity.ServiceChannelServiceHours.ForEach(h => vm.ServiceHours.Add(new V11VmOpenApiServiceHour
                {
                    ServiceHourType = h.ServiceHours != null && h.ServiceHours.ServiceHourTypeId != Guid.Empty ? TypeCache.GetByValue<ServiceHourType>(h.ServiceHours.ServiceHourTypeId).GetOpenApiEnumValue<ServiceHoursTypeEnum>() : null
                }));
            }

            if (entity.Phones?.Count > 0)
            {
                vm.SupportPhones = new List<V4VmOpenApiPhone>();
                entity.Phones.ForEach(p => vm.SupportPhones.Add(new V4VmOpenApiPhone
                {
                    ServiceChargeType = p.Phone != null && p.Phone.ChargeTypeId != Guid.Empty ? TypeCache.GetByValue<ServiceChargeType>(p.Phone.ChargeTypeId).GetOpenApiEnumValue<ServiceChargeTypeEnum>() : null
                }));
            }

            return vm;
        }
    }
}
