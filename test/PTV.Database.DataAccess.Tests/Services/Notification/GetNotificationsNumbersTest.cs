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
using Microsoft.EntityFrameworkCore;
using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Domain.Model.Models.V2.Notifications;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Notification
{
    public class GetNotificationsNumbersTest : TestBase
    {

        private readonly NotificationService notificationService;
        private readonly Mock<ICommonServiceInternal> commonServiceMock;
        private readonly Mock<IUserOrganizationService> userOrganizationService;
        private readonly Mock<IPahaTokenAccessor> pahaTokenProcessorMock;
        private readonly Mock<ISearchServiceInternal> searchServiceMock;

        public GetNotificationsNumbersTest()
        {
            userOrganizationService = new Mock<IUserOrganizationService>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            pahaTokenProcessorMock = new Mock<IPahaTokenAccessor>(MockBehavior.Strict);
            searchServiceMock = new Mock<ISearchServiceInternal>(MockBehavior.Strict);

            notificationService = new NotificationService
            (
                contextManagerMock.Object,
                null,
                CacheManagerMock.Object,
                commonServiceMock.Object,
                null,
                null,
                null,
                null,
                pahaTokenProcessorMock.Object,
                searchServiceMock.Object,
                null
            );
            SetupTypesCacheMock<PublishingStatusType>();
        }

        private void BaseTestSetup(int count)
        {
            var myOrgId = "myOrg".GetGuid();
            SetupContextManager<object, VmListItemsData<VmNotificationsBase>>();
            userOrganizationService
                .Setup(x => x.GetAllUserOrganizationRoles(null))
                .Returns(new List<IUserOrganizationRoles>
                    {new UserOrganizationRoles {OrganizationId = myOrgId}});

            pahaTokenProcessorMock.Setup(x => x.UserName).Returns("test");


            RegisterRepository<INotificationServiceServiceChannelRepository, NotificationServiceServiceChannel>
                (CreateNotifications(count, myOrgId).AsQueryable());

            RegisterRepository<ITrackingEntityVersionedRepository, TrackingEntityVersioned>(
                CreateTrackingEntities(count).AsQueryable());

            RegisterRepository<ITrackingGeneralDescriptionVersionedRepository, TrackingGeneralDescriptionVersioned>(
                CreateGdTrackingEntities(count).AsQueryable());

            RegisterRepository<IStatutoryServiceGeneralDescriptionVersionedRepository, StatutoryServiceGeneralDescriptionVersioned>(
                CreateEntities<StatutoryServiceGeneralDescriptionVersioned>(count).AsQueryable());

            RegisterRepository<IStatutoryServiceNameRepository, StatutoryServiceName>
                (CreateGeneralNames(count).AsQueryable());

            RegisterRepository<IServiceNameRepository, ServiceName>
                (CreateServiceNames(count).AsQueryable());

            var searchConnectionResult = CreateConnectionNotifications(count);
            searchServiceMock.Setup(x => x.SearchConnectionsCount(It.IsAny<IVmEntitySearch>()))
                .Returns(searchConnectionResult.Count);

            commonServiceMock.Setup(x =>
                    x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                        It.IsAny<IEnumerable<Guid>>(),
                        unitOfWorkMockSetup.Object,
                        It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>()))
                .Returns(CreateEntities<ServiceVersioned>(count));
            commonServiceMock.Setup(x => x.GetEntityNames(It.IsAny<List<ServiceVersioned>>()))
                .Returns((List<ServiceVersioned> input) => input.ToDictionary(x=>x.UnificRootId, v=>new Dictionary<string, string>{{"fi","name"}}));

            commonServiceMock.Setup(x =>
                    x.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(
                        It.IsAny<List<ServiceVersioned>>()))
                .Returns((List<ServiceVersioned> input) => input.ToDictionary<ServiceVersioned,Guid,IReadOnlyList<VmLanguageAvailabilityInfo>>(
                    k=>k.UnificRootId,
                    v=> new List<VmLanguageAvailabilityInfo>{new VmLanguageAvailabilityInfo{LanguageId = "fi".GetGuid()}}));

            commonServiceMock.Setup(x =>
                    x.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                        It.IsAny<IEnumerable<Guid>>(),
                        unitOfWorkMockSetup.Object,
                        It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>()))
                .Returns(CreateEntities<ServiceChannelVersioned>(count));
            commonServiceMock.Setup(x => x.GetEntityNames(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> input) => input.ToDictionary(x=>x.UnificRootId, v=>new Dictionary<string, string>{{"fi","name"}}));
            commonServiceMock.Setup(x =>
                    x.GetLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                        It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> input) => input.ToDictionary<ServiceChannelVersioned,Guid,IReadOnlyList<VmLanguageAvailabilityInfo>>(
                    k=>k.UnificRootId,
                    v=> new List<VmLanguageAvailabilityInfo>{new VmLanguageAvailabilityInfo{LanguageId = "fi".GetGuid()}}));

            commonServiceMock.Setup(x =>
                    x.GetChannelSubType(It.IsAny<Guid>(), It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns(EntityTypeEnum.Channel.ToString());

            commonServiceMock.Setup(x =>
                    x.GetNotificationEntityCustom<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                        It.IsAny<IEnumerable<Guid>>(),
                        unitOfWorkMockSetup.Object,
                        It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>()))
                .Returns(CreateEntities<StatutoryServiceGeneralDescriptionVersioned>(count));
            commonServiceMock.Setup(x => x.GetEntityNames(It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()))
                .Returns((List<StatutoryServiceGeneralDescriptionVersioned> input) => input.ToDictionary(x=>x.UnificRootId, v=>new Dictionary<string, string>{{"fi","name"}}));

            commonServiceMock.Setup(x =>
                    x.GetLanguageAvailabilites<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                        It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()))
                .Returns((List<StatutoryServiceGeneralDescriptionVersioned> input) => input.ToDictionary<StatutoryServiceGeneralDescriptionVersioned,Guid,IReadOnlyList<VmLanguageAvailabilityInfo>>(
                    k=>k.UnificRootId,
                    v=> new List<VmLanguageAvailabilityInfo>{new VmLanguageAvailabilityInfo{LanguageId = "fi".GetGuid()}}));
        }


        [Fact]
        public void CheckGetNotificationsNumbers()
        {
            var count = 20;
            BaseTestSetup(count);
            var result = notificationService.GetNotificationsNumbers(userOrganizationService.Object.GetAllUserOrganizationRoles().Select(i => i.OrganizationId).ToList());

            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            result.Single(x => x.Id == Domain.Model.Models.V2.Notifications.Notification.ServiceChannelInCommonUse)
                .Count.Should().Be(count);
            result.Single(x => x.Id == Domain.Model.Models.V2.Notifications.Notification.GeneralDescriptionCreated)
                .Count.Should().Be(count);
            result.Single(x => x.Id == Domain.Model.Models.V2.Notifications.Notification.GeneralDescriptionUpdated)
                .Count.Should().Be(count);
            result.Single(x => x.Id == Domain.Model.Models.V2.Notifications.Notification.ContentUpdated)
                .Count.Should().Be(count*2); // services and channels
            result.Single(x => x.Id == Domain.Model.Models.V2.Notifications.Notification.ConnectionChanges)
                .Count.Should().Be(count*6); // serviceconnection(add,edit,remove) and channelsconnection(add,edit,remove)

        }

        private List<NotificationServiceServiceChannel> CreateNotifications(int count, Guid myOrg)
        {
            var result = new List<NotificationServiceServiceChannel>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateNotification(i.ToString(), myOrg, "test"));
            }
            return result;
        }

        private NotificationServiceServiceChannel CreateNotification(string postFix, Guid org, string name)
        {
            var filters = new List<NotificationServiceServiceChannelFilter>();

            return new NotificationServiceServiceChannel
            {
                Id = Guid.NewGuid(),
                OrganizationId = org,
                ServiceId = (EntityTypeEnum.Service+postFix).GetGuid(),
                ChannelId = (EntityTypeEnum.Channel+postFix).GetGuid(),
                CreatedBy = name,
                Created = DateTime.UtcNow,
                Filters = filters
            };
        }

        private List<ServiceName> CreateServiceNames(int count)
        {
            var result = new List<ServiceName>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(new ServiceName
                {
                    Name = "test",
                    ServiceVersionedId = (EntityTypeEnum.Service+i.ToString()).GetGuid(),
                    ServiceVersioned = new ServiceVersioned
                    {
                        UnificRootId = (EntityTypeEnum.Service+i.ToString()).GetGuid()
                    },
                    Localization = new Language
                    {
                        Code = DomainConstants.DefaultLanguage
                    }
                });
            }

            return result;
        }

        private List<StatutoryServiceName> CreateGeneralNames(int count)
        {
            var result = new List<StatutoryServiceName>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(new StatutoryServiceName
                {
                    Name = "test",
                    StatutoryServiceGeneralDescriptionVersionedId = (EntityTypeEnum.Service+i.ToString()).GetGuid(),
                    StatutoryServiceGeneralDescriptionVersioned = new StatutoryServiceGeneralDescriptionVersioned
                    {
                        UnificRootId = (EntityTypeEnum.GeneralDescription+i.ToString()).GetGuid()
                    },
                    Localization = new Language
                    {
                        Code = DomainConstants.DefaultLanguage
                    }
                });
            }

            return result;
        }

        private List<T> CreateEntities<T>(int count) where T : class, IVersionedVolume, new ()
        {
            var result = new List<T>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateEntity<T>(i.ToString()));
            }

            return result;
        }

        private T CreateEntity<T>(string postFix) where T : class, IVersionedVolume, new ()
        {
            return new T
            {
                UnificRootId =
                    typeof(T)==typeof(ServiceVersioned) ?
                    (EntityTypeEnum.Service+postFix).GetGuid() :
                    typeof(T)==typeof(ServiceChannelVersioned) ?
                        (EntityTypeEnum.Channel+postFix).GetGuid() :
                        (EntityTypeEnum.GeneralDescription+postFix).GetGuid(),
                Id = typeof(T)==typeof(ServiceVersioned) ?
                    (EntityTypeEnum.Service+postFix).GetGuid() :
                    typeof(T)==typeof(ServiceChannelVersioned) ?
                        (EntityTypeEnum.Channel+postFix).GetGuid() :
                        (EntityTypeEnum.GeneralDescription+postFix).GetGuid(),
                PublishingStatusId = typeof(T)==typeof(StatutoryServiceGeneralDescriptionVersioned) ?
                    PublishingStatus.Published.ToString().GetGuid() :
                    PublishingStatus.Draft.ToString().GetGuid()
            };
        }

        private List<TrackingEntityVersioned> CreateTrackingEntities(int count)
        {
            var result = new List<TrackingEntityVersioned>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateTrackingEntity(EntityTypeEnum.Channel, i.ToString(), EntityState.Modified.ToString()));
                result.Add(CreateTrackingEntity(EntityTypeEnum.Service, i.ToString(), EntityState.Modified.ToString()));
                result.Add(CreateTrackingEntity(EntityTypeEnum.Channel, i.ToString(), EntityState.Deleted.ToString()));
                result.Add(CreateTrackingEntity(EntityTypeEnum.Service, i.ToString(), EntityState.Deleted.ToString()));
            }
            return result;
        }

        private TrackingEntityVersioned CreateTrackingEntity(EntityTypeEnum entityType, string postFix, string operationType)
        {
            return new TrackingEntityVersioned
            {
                UnificRootId = (entityType+postFix).GetGuid(),
                EntityType = entityType.ToString(),
                OrganizationId = "myOrg".GetGuid(),
                Id = Guid.NewGuid(),
                OperationType = operationType,
                CreatedBy = "test",
                Created = DateTime.UtcNow
            };
        }

        private List<TrackingGeneralDescriptionVersioned> CreateGdTrackingEntities(int count)
        {
            var result = new List<TrackingGeneralDescriptionVersioned>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateGdTrackingEntity(i.ToString(), EntityState.Added.ToString()));
                result.Add(CreateGdTrackingEntity(i.ToString(), EntityState.Modified.ToString()));
            }
            return result;
        }

        private TrackingGeneralDescriptionVersioned CreateGdTrackingEntity(string postFix, string operationType)
        {
            return new TrackingGeneralDescriptionVersioned
            {
                Id = Guid.NewGuid(),
                OperationType = operationType,
                CreatedBy = "test",
                Created = DateTime.UtcNow,
                GenerealDescriptionId = (EntityTypeEnum.GeneralDescription+postFix).GetGuid()
            };
        }

        private List<VmNotificationConnectionListItem> CreateConnectionNotifications( int count)
        {
            var result = new List<VmNotificationConnectionListItem>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateConnectionNotification(EntityTypeEnum.Service, i.ToString(), EntityState.Added.ToString()));
                result.Add(CreateConnectionNotification(EntityTypeEnum.Service, i.ToString(), EntityState.Modified.ToString()));
                result.Add(CreateConnectionNotification(EntityTypeEnum.Service, i.ToString(), EntityState.Deleted.ToString()));
                result.Add(CreateConnectionNotification(EntityTypeEnum.Channel, i.ToString(), EntityState.Added.ToString()));
                result.Add(CreateConnectionNotification(EntityTypeEnum.Channel, i.ToString(), EntityState.Modified.ToString()));
                result.Add(CreateConnectionNotification(EntityTypeEnum.Channel, i.ToString(), EntityState.Deleted.ToString()));
            }
            return result;
        }

        private VmNotificationConnectionListItem CreateConnectionNotification(EntityTypeEnum entityType, string postFix, string operationType)
        {
            return new VmNotificationConnectionListItem
            {
                EntityType = entityType,
                EntityId = entityType == EntityTypeEnum.Service ? (EntityTypeEnum.Service+postFix).GetGuid() : (EntityTypeEnum.Channel+postFix).GetGuid(),
                ConnectedId = entityType == EntityTypeEnum.Service ? (EntityTypeEnum.Channel+postFix).GetGuid() : (EntityTypeEnum.Service+postFix).GetGuid(),
                Id = Guid.NewGuid(),
                OperationType = operationType,
                CreatedBy = "test",
                Created = DateTime.UtcNow.ToEpochTime()
            };
        }
    }
}
