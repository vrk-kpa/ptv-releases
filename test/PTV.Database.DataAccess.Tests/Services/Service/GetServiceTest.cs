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
using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework;
using Xunit;
using CollectionExtensions = Castle.Core.Internal.CollectionExtensions;
using ServiceService = PTV.Database.DataAccess.Services.V2.ServiceService;

namespace PTV.Database.DataAccess.Tests.Services.Service
{
    public class GetServiceTest : TestBase
    {
        private readonly Guid organizationId = "organizationName".GetGuid();

        private readonly ServiceService serviceService;
        private readonly Mock<ITranslationEntity> translateToVmMock;
        private readonly Mock<IConnectionsServiceInternal> connectionsServiceMock;
        private readonly Mock<IServiceUtilities> serviceUtilitiesMock;
        private readonly Mock<ITranslationService> translationServiceMock;
        private readonly Mock<IServiceCollectionServiceInternal> serviceCollectionServiceMock;
        private readonly Mock<ICommonServiceInternal> commonServiceMock;
        private readonly Mock<IVersioningManager> versioningManagerMock;
        private readonly Mock<IPahaTokenAccessor> pahaTokenProcessorMock;
        private readonly Mock<IUserOrganizationService> userOrganizationServiceMock;

        public GetServiceTest()
        {
            translateToVmMock = new Mock<ITranslationEntity>(MockBehavior.Strict);
            connectionsServiceMock = new Mock<IConnectionsServiceInternal>(MockBehavior.Strict);
            serviceUtilitiesMock = new Mock<IServiceUtilities>(MockBehavior.Strict);
            translationServiceMock = new Mock<ITranslationService>(MockBehavior.Strict);
            serviceCollectionServiceMock = new Mock<IServiceCollectionServiceInternal>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            versioningManagerMock = new Mock<IVersioningManager>(MockBehavior.Strict);
            pahaTokenProcessorMock = new Mock<IPahaTokenAccessor>(MockBehavior.Strict);
            userOrganizationServiceMock = new Mock<IUserOrganizationService>(MockBehavior.Strict);
            var expirationServiceMock = new Mock<IExpirationService>();

            serviceService = new ServiceService
            (
                contextManagerMock.Object,
                translateToVmMock.Object,
                null,
                serviceUtilitiesMock.Object,
                commonServiceMock.Object,
                null,
                connectionsServiceMock.Object,
                serviceCollectionServiceMock.Object,
                translationServiceMock.Object,
                CacheManager,
                CacheManager.PublishingStatusCache,
                null,
                null,
                versioningManagerMock.Object,
                null,
                pahaTokenProcessorMock.Object,
                userOrganizationServiceMock.Object,
                null,
                null,
                null, 
                expirationServiceMock.Object,
                null
            );

            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));
        }

        private void TestSetup(string userName = "userName")
        {
            SetupContextManager<object, ServiceVersioned>();
            SetupContextManager<object, VmServiceOutput>();

            translateToVmMock.Setup(x => x.Translate<ServiceVersioned, VmServiceOutput>(It.IsAny<ServiceVersioned>()))
                .Returns((ServiceVersioned input) => TranslateService(input));

            connectionsServiceMock
                .Setup(x => x.GetAllServiceRelations(It.IsAny<IUnitOfWork>(), It.IsAny<List<Guid>>()))
                .Returns((IUnitOfWork uow, List<Guid> ids) => new Dictionary<Guid, List<ServiceServiceChannel>>());

            translateToVmMock
                .Setup(x => x.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(It.IsAny<ICollection<ServiceServiceChannel>>()))
                .Returns((IEnumerable<ServiceServiceChannel> input) => new List<VmConnectionOutput>());

            serviceUtilitiesMock
                .Setup(x => x.GetEntityEditableInfo<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>()))
                .Returns((Guid id, IUnitOfWork uow) => new VmEntityEditableBase());

            serviceUtilitiesMock
                .Setup(x => x.GetAllUserOrganizations())
                .Returns(() => new List<Guid>());

            translationServiceMock
                .Setup(x => x.GetServiceTranslationAvailabilities(It.IsAny<IUnitOfWork>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns((IUnitOfWork uow, Guid evid, Guid urid) => new Dictionary<string, VmTranslationOrderAvailability>());

            serviceCollectionServiceMock
                .Setup(x => x.GetAllServiceRelations(It.IsAny<IUnitOfWork>(), It.IsAny<Guid>()))
                .Returns((IUnitOfWork uow, Guid urid) => new List<VmServiceCollectionConnectionOutput>());

            commonServiceMock.Setup(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> ids) => new List<VmListItem>(ids.Select(x => new VmListItem { Id = x })));

            pahaTokenProcessorMock.Setup(i => i.UserName).Returns(userName);

            userOrganizationServiceMock.Setup(x => x.GetAllUserOrganizationIds(null))
                .Returns(new List<Guid> {organizationId});
        }

        [Fact]
        public void NoNotificationTest()
        {
            TestSetup();

            var service = new ServiceVersioned {Id = Guid.NewGuid(), UnificRootId = Guid.NewGuid()};
            var channel = new ServiceChannelVersioned { Id = Guid.NewGuid(), UnificRootId = Guid.NewGuid(), ServiceChannelNames = new List<ServiceChannelName>()};
            var notificationServiceServiceChannel = new NotificationServiceServiceChannel {Id = Guid.NewGuid(), ServiceId = Guid.NewGuid(), ChannelId = channel.UnificRootId};

            RegisterRepository<IServiceRepository, Model.Models.Service>(new List<Model.Models.Service>().AsQueryable());
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(new List<ServiceVersioned> {service}.AsQueryable());
            RegisterRepository<INotificationServiceServiceChannelRepository, NotificationServiceServiceChannel>(new List<NotificationServiceServiceChannel> {notificationServiceServiceChannel}.AsQueryable());
            RegisterIncludeRepositories();

            var searchCriteria = new VmServiceBasic{Id = service.Id};
            var result = serviceService.GetService(searchCriteria);
            Assert.NotNull(result);
            Assert.Empty(result.DisconnectedConnections);
        }

        [Fact]
        public void NotificationWithFilter()
        {
            var user = "user";
            TestSetup(user);

            var service = new ServiceVersioned {Id = Guid.NewGuid(), UnificRootId = Guid.NewGuid()};
            var channel = new ServiceChannelVersioned { Id = Guid.NewGuid(), UnificRootId = Guid.NewGuid(), ServiceChannelNames = new List<ServiceChannelName>()};
            var notificationServiceServiceChannel = new NotificationServiceServiceChannel {Id = Guid.NewGuid(), ServiceId = service.UnificRootId, ChannelId = channel.UnificRootId};
            var filter = new NotificationServiceServiceChannelFilter {NotificationServiceServiceChannelId = notificationServiceServiceChannel.Id, UserId = user.GetGuid()};
            notificationServiceServiceChannel.Filters.Add(filter);

            RegisterRepository<IServiceRepository, Model.Models.Service>(new List<Model.Models.Service>().AsQueryable());
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(new List<ServiceVersioned> {service}.AsQueryable());
            RegisterRepository<INotificationServiceServiceChannelRepository, NotificationServiceServiceChannel>(new List<NotificationServiceServiceChannel> {notificationServiceServiceChannel}.AsQueryable());
            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(new List<ServiceChannelVersioned> {channel}.AsQueryable());
            RegisterRepository<INotificationServiceServiceChannelFilterRepository, NotificationServiceServiceChannelFilter>(new List<NotificationServiceServiceChannelFilter> {filter}.AsQueryable());
            RegisterIncludeRepositories();

            versioningManagerMock.Setup(x => x.GetLastVersion<ServiceChannelVersioned>(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<Guid>()))
                .Returns((ITranslationUnitOfWork uow, Guid id) => new VersionInfo{EntityId = channel.Id});

            var searchCriteria = new VmServiceBasic{Id = service.Id};
            var result = serviceService.GetService(searchCriteria);
            Assert.NotNull(result);
            Assert.Empty(result.DisconnectedConnections);
        }

        [Fact]
        public void NoDisconnectedChannelNotificationForDifferentOrganization()
        {
            TestSetup();

            var service = new ServiceVersioned
            {
                Id = Guid.NewGuid(), UnificRootId = Guid.NewGuid(), OrganizationId = "differentOrganization".GetGuid()
            };
            var channel = new ServiceChannelVersioned
            {
                Id = Guid.NewGuid(),
                UnificRootId = Guid.NewGuid(),
                TypeId = ServiceChannelTypeEnum.Phone.ToString().GetGuid(),
                ServiceChannelNames = new List<ServiceChannelName>
                {
                    new ServiceChannelName{LocalizationId = "fi".GetGuid()}
                }
            };
            var notificationServiceServiceChannel = new NotificationServiceServiceChannel {Id = Guid.NewGuid(), ServiceId = service.UnificRootId, ChannelId = channel.UnificRootId};

            RegisterRepository<IServiceRepository, Model.Models.Service>(new List<Model.Models.Service>().AsQueryable());
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(new List<ServiceVersioned> {service}.AsQueryable());
            RegisterRepository<INotificationServiceServiceChannelRepository, NotificationServiceServiceChannel>(new List<NotificationServiceServiceChannel> {notificationServiceServiceChannel}.AsQueryable());
            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(new List<ServiceChannelVersioned> {channel}.AsQueryable());
            RegisterIncludeRepositories();
            versioningManagerMock.Setup(x => x.GetLastVersion<ServiceChannelVersioned>(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<Guid>()))
                .Returns((ITranslationUnitOfWork uow, Guid id) => new VersionInfo{EntityId = channel.Id});

            var searchCriteria = new VmServiceBasic{Id = service.Id};
            var result = serviceService.GetService(searchCriteria);
            Assert.NotNull(result);
            Assert.Empty(result.DisconnectedConnections);
        }

        [Fact]
        public void CheckResult()
        {
            TestSetup();

            const string channelName = "phone channel service";

            var service = new ServiceVersioned {Id = Guid.NewGuid(), UnificRootId = Guid.NewGuid(), OrganizationId = organizationId};
            var channel = new ServiceChannelVersioned
            {
                Id = Guid.NewGuid(),
                UnificRootId = Guid.NewGuid(),
                TypeId = ServiceChannelTypeEnum.Phone.ToString().GetGuid(),
                ServiceChannelNames = new List<ServiceChannelName>
                {
                    new ServiceChannelName{LocalizationId = "fi".GetGuid(), Name = channelName}
                }
            };
            var notificationServiceServiceChannel = new NotificationServiceServiceChannel {Id = Guid.NewGuid(), ServiceId = service.UnificRootId, ChannelId = channel.UnificRootId};

            RegisterRepository<IServiceRepository, Model.Models.Service>(new List<Model.Models.Service>().AsQueryable());
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(new List<ServiceVersioned> {service}.AsQueryable());
            RegisterRepository<INotificationServiceServiceChannelRepository, NotificationServiceServiceChannel>(new List<NotificationServiceServiceChannel> {notificationServiceServiceChannel}.AsQueryable());
            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(new List<ServiceChannelVersioned> {channel}.AsQueryable());
            RegisterIncludeRepositories();

            versioningManagerMock.Setup(x => x.GetLastVersion<ServiceChannelVersioned>(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<Guid>()))
                .Returns((ITranslationUnitOfWork uow, Guid id) => new VersionInfo{EntityId = channel.Id});

            var searchCriteria = new VmServiceBasic{Id = service.Id};
            var result = serviceService.GetService(searchCriteria);
            Assert.NotNull(result);
            Assert.NotEmpty(result.DisconnectedConnections);
            result.DisconnectedConnections.Count.Should().Be(1);

            var model = result.DisconnectedConnections.Single();
            model.NotificationId.Should().Be(notificationServiceServiceChannel.Id);
            model.ChannelVersionedId.Should().Be(channel.Id);
            model.ChannelUnificRootId.Should().Be(channel.UnificRootId);
            model.ChannelType.Should().Be(ServiceChannelTypeEnum.Phone.ToString());
            var name = model.Name.Single();
            name.Key.Should().Be("fi");
            name.Value.Should().Be(channelName);
        }

        [Theory]
        [InlineData("user", null, null, 2)]
        [InlineData("user", null, new []{"channel1"}, 2)]
        [InlineData("user", null, new []{"channel1", "channel2"}, 2)]
        [InlineData("user", "user", null, 2)]
        [InlineData("user", "user", new []{"channel1"}, 1)]
        [InlineData("user", "user", new []{"channel1", "channel2"}, 0)]
        [InlineData("user", "otherUser", null, 2)]
        [InlineData("user", "otherUser", new []{"channel1"}, 2)]
        [InlineData("user", "otherUser", new []{"channel1", "channel2"}, 2)]
        public void NotificationFilterTest(
            string currentUser,
            string filterUser,
            string[] channelFilters,
            int countResult)
        {
            const string serviceName = "service";
            const string channel1Name = "channel1";
            const string channel2Name = "channel2";

            var channel1 =  new ServiceChannelVersioned {Id = channel1Name.GetGuid(), UnificRootId = channel1Name.GetGuid()};
            var channel2 =  new ServiceChannelVersioned {Id = channel2Name.GetGuid(), UnificRootId = channel2Name.GetGuid()};

            var service = new ServiceVersioned {Id = serviceName.GetGuid(), UnificRootId = serviceName.GetGuid(), OrganizationId = organizationId};

            var service1Notification = new NotificationServiceServiceChannel {Id = Guid.NewGuid(), ChannelId = channel1.UnificRootId, ServiceId = service.UnificRootId};
            var service2Notification = new NotificationServiceServiceChannel {Id = Guid.NewGuid(), ChannelId = channel2.UnificRootId, ServiceId = service.UnificRootId};
            var notifications = new List<NotificationServiceServiceChannel> { service1Notification, service2Notification};

            if (filterUser != null && channelFilters != null)
            {
                foreach (var channelName in channelFilters)
                {
                    var notification = notifications.SingleOrDefault(n => n.ChannelId == channelName.GetGuid());
                    notification?.Filters.Add(new NotificationServiceServiceChannelFilter
                    {
                        NotificationServiceServiceChannelId = notification.Id,
                        UserId = filterUser.GetGuid()
                    });
                }
            }

            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(new List<ServiceChannelVersioned> {channel1, channel2}.AsQueryable());
            RegisterRepository<IServiceRepository, Model.Models.Service>(new List<Model.Models.Service>().AsQueryable());
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(new List<ServiceVersioned> {service}.AsQueryable());
            RegisterRepository<INotificationServiceServiceChannelRepository, NotificationServiceServiceChannel>(notifications.AsQueryable());
            RegisterIncludeRepositories();

            versioningManagerMock.Setup(x => x.GetLastVersion<ServiceChannelVersioned>(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<Guid>()))
                .Returns((ITranslationUnitOfWork uow, Guid id) => new VersionInfo{EntityId = id});

            TestSetup(currentUser);
            var result = serviceService.GetService(new VmServiceBasic{Id = service.Id}).DisconnectedConnections;

            // verify
            result.Should().NotBeNull();
            result.Count.Should<int>().Be(countResult);
            pahaTokenProcessorMock.Verify(x => x.UserName, Times.Once);
            versioningManagerMock.Verify(x => x.GetLastVersion<ServiceChannelVersioned>(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<Guid>()), Times.Exactly(countResult));
        }

        private void RegisterIncludeRepositories()
        {
            RegisterRepository<IServiceKeywordRepository, ServiceKeyword>(new List<ServiceKeyword>().AsQueryable());
            RegisterRepository<IServiceServiceClassRepository, ServiceServiceClass>(new List<ServiceServiceClass>().AsQueryable());
            RegisterRepository<IServiceOntologyTermRepository, ServiceOntologyTerm>(new List<ServiceOntologyTerm>().AsQueryable());
            RegisterRepository<IServiceLifeEventRepository, ServiceLifeEvent>(new List<ServiceLifeEvent>().AsQueryable());
            RegisterRepository<IServiceIndustrialClassRepository, ServiceIndustrialClass>(new List<ServiceIndustrialClass>().AsQueryable());
            RegisterRepository<IServiceTargetGroupRepository, ServiceTargetGroup>(new List<ServiceTargetGroup>().AsQueryable());
            RegisterRepository<IServiceProducerRepository, ServiceProducer>(new List<ServiceProducer>().AsQueryable());
            RegisterRepository<IServiceLawRepository, ServiceLaw>(new List<ServiceLaw>().AsQueryable());
        }

        private static VmServiceOutput TranslateService(ServiceVersioned service)
        {
            return new VmServiceOutput
            {
                Id = service.Id,
                UnificRootId = service.UnificRootId,
                Organization = service.OrganizationId,
                Name = CollectionExtensions.IsNullOrEmpty(service.ServiceNames)
                    ? new Dictionary<string, string>()
                    : service.ServiceNames.ToDictionary(k => k.Localization.Code, v => v.Name),
                LanguagesAvailabilities = CollectionExtensions.IsNullOrEmpty(service.LanguageAvailabilities)
                    ? new List<VmLanguageAvailabilityInfo>()
                    : service.LanguageAvailabilities.Select(l => new VmLanguageAvailabilityInfo {LanguageId = l.LanguageId}).ToList(),
                ResponsibleOrganizations = new List<Guid>()
            };
        }

    }
}
