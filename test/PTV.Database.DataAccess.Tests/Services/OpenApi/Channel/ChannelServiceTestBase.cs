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
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public abstract class ChannelServiceTestBase : ServiceTestBase
    {
        internal const string USERNAME = "userName";

        internal ILogger<ChannelService> Logger { get; private set; }
        internal ServiceChannelLogic ServiceChannelLogic { get; private set; }
        internal IUrlService UrlService { get; private set; }
        internal Mock<IServiceChannelVersionedRepository> ChannelRepoMock { get; private set; }
        internal Mock<IServiceVersionedRepository> ServiceRepoMock { get; private set; }

        public ChannelServiceTestBase()
        {
            Logger = new Mock<ILogger<ChannelService>>().Object;
            ServiceChannelLogic = new ServiceChannelLogic(new ChannelAttachmentLogic(), new WebPageLogic(), new OpeningHoursLogic(), AddressLogic);
            UrlService = (new Mock<IUrlService>()).Object;
            ChannelRepoMock = new Mock<IServiceChannelVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(ChannelRepoMock.Object);
            ServiceRepoMock = new Mock<IServiceVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(ServiceRepoMock.Object);
        }
        
        internal ChannelService ArrangeForGet(List<ServiceChannelVersioned> list, ServiceChannelTypeEnum channelType)
        {
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.TypeId = TypeCache.Get<ServiceChannelType>(channelType.ToString());
            var id = publishedChannel.Id;
            var rootId = publishedChannel.UnificRootId;

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, PublishingStatus.Published, true)).Returns(id);

            // repositories
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

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
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            
            ArrangeTranslationManager(channelType);
            
            return new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);
        }

        internal void ArrangeTranslationManager(ServiceChannelTypeEnum? channelType = null)
        {
            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny< ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.EChannel) return new VmOpenApiElectronicChannelVersionBase();
                    return new VmOpenApiElectronicChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.Phone) return new VmOpenApiPhoneChannelVersionBase();
                    return new VmOpenApiPhoneChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.ServiceLocation) return new VmOpenApiServiceLocationChannelVersionBase();
                    return new VmOpenApiServiceLocationChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.PrintableForm) return new VmOpenApiPrintableFormChannelVersionBase();
                    return new VmOpenApiPrintableFormChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType.HasValue && channelType != ServiceChannelTypeEnum.WebPage) return new VmOpenApiWebPageChannelVersionBase();
                    return new VmOpenApiWebPageChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });
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
                        list.Add(new VmOpenApiElectronicChannelVersionBase()
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
                        list.Add(new VmOpenApiPhoneChannelVersionBase()
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
                        list.Add(new VmOpenApiServiceLocationChannelVersionBase()
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
                        list.Add(new VmOpenApiPrintableFormChannelVersionBase()
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
                        list.Add(new VmOpenApiWebPageChannelVersionBase()
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
                    entity.ServiceLocationChannels = new List<ServiceLocationChannel> { new ServiceLocationChannel { ServiceChannelVersionedId = entity.Id } };
                    var scChannelRepoMock = new Mock<IServiceLocationChannelRepository>();
                    unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceLocationChannelRepository>()).Returns(scChannelRepoMock.Object);
                    scChannelRepoMock.Setup(g => g.All()).Returns(entity.ServiceLocationChannels.AsQueryable());
                    unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                    It.IsAny<IQueryable<ServiceLocationChannel>>(),
                    It.IsAny<Func<IQueryable<ServiceLocationChannel>, IQueryable<ServiceLocationChannel>>>(),
                    It.IsAny<bool>()
                    )).Returns((IQueryable<ServiceLocationChannel> channelServices, Func<IQueryable<ServiceLocationChannel>, IQueryable<ServiceLocationChannel>> func, bool applyFilters) =>
                    {
                        return channelServices;
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
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get(LanguageCode.fi.ToString());
            var service = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(statusId);
            service.ServiceNames.Add(new ServiceName { Name = channelName, LocalizationId = langId });
            service.LanguageAvailabilities.Add(new ServiceLanguageAvailability { StatusId = statusId, LanguageId = langId });
            service.UnificRoot = new Model.Models.Service { Id = service.UnificRootId };
            service.UnificRoot.Versions.Add(service);
            var connection = new ServiceServiceChannel
            {
                ServiceChannel = item.UnificRoot,
                ServiceChannelId = item.UnificRootId,
                Service = service.UnificRoot,
                ServiceId = service.UnificRootId
            };

            return connection;
        }
    }
}
