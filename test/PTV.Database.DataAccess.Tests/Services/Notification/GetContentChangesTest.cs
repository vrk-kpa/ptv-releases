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
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
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
    public class GetContentChangesTest : TestBase
    {

        private readonly NotificationService notificationService;
        private readonly Mock<ICommonServiceInternal> commonServiceMock;
        private readonly Mock<ISearchServiceInternal> searchServiceMock;
        private readonly Mock<IUserOrganizationService> userOrganizationService;


        public GetContentChangesTest()
        {
            userOrganizationService = new Mock<IUserOrganizationService>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
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
                null,
                searchServiceMock.Object
            );
            SetupTypesCacheMock<PublishingStatusType>();
        }

        private void BaseTestSetup(EntityTypeEnum entityType, int count, string operationType, int monthAge)
        {
            var myOrgId = "myOrg".GetGuid();
            SetupContextManager<object, IVmSearchBase>();
            userOrganizationService
                .Setup(x => x.GetAllUserOrganizationRoles(null))
                .Returns(new List<IUserOrganizationRoles>
                    {new UserOrganizationRoles {OrganizationId = myOrgId}});

            RegisterRepository<ITrackingEntityVersionedRepository, TrackingEntityVersioned>(
                CreateTrackingEntities(entityType, count, operationType, monthAge).AsQueryable());


            commonServiceMock.Setup(x =>
                    x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                        It.IsAny<IEnumerable<Guid>>(),
                        unitOfWorkMockSetup.Object,
                        It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>()))
                .Returns(CreateEntities<ServiceVersioned>(count));
            commonServiceMock.Setup(x => x.GetEntityNames(It.IsAny<List<ServiceVersioned>>()))
                .Returns((List<ServiceVersioned> input) => input.ToDictionary(x=>x.UnificRootId, v=>new Dictionary<string, string>{{"fi","name"}}));

            commonServiceMock.Setup(x => x.GetChannelNames(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> input) => input.ToDictionary(x=>x.UnificRootId, v=>new Dictionary<string, string>{{"fi","name"}}));

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
        }


        [Theory]
        [InlineData(EntityTypeEnum.Service,2,"Added", 0, 0)]
        [InlineData(EntityTypeEnum.Service,5,"Modified", 5, 0)]
        [InlineData(EntityTypeEnum.Service,5,"Modified", 0, 1)]
        [InlineData(EntityTypeEnum.Service,5,"Modified", 0, 2)]
        [InlineData(EntityTypeEnum.Service,7,"Deleted", 0, 0)]
        [InlineData(EntityTypeEnum.Channel,1,"Added", 0, 0)]
        [InlineData(EntityTypeEnum.Channel,3,"Modified", 3, 0)]
        [InlineData(EntityTypeEnum.Channel,3,"Modified", 0, 1)]
        [InlineData(EntityTypeEnum.Channel,3,"Modified", 0, 2)]
        [InlineData(EntityTypeEnum.Channel,6,"Deleted", 0, 0)]
        public void CheckContentChanged(EntityTypeEnum entityType, int count, string operationType, int resultCount, int monthAge)
        {
            BaseTestSetup(entityType, count, operationType, monthAge);
            var result = (VmNotifications)notificationService.GetContentChanged(
                new VmNotificationSearch
                {
                    SortData = new List<VmSortParam>()
                }, userOrganizationService.Object.GetAllUserOrganizationRoles().Select(i => i.OrganizationId).ToList());

            result.Should().NotBeNull();
            result.Count.Should().Be(resultCount);
            result.Notifications.Count().Should().Be(resultCount);
            result.Notifications.ForEach(x=> Assert.True(x.EntityType == entityType));
            result.Notifications.ForEach(x=> Assert.True(x.OperationType == operationType));
            result.Notifications.ForEach(x=> Assert.True(x.CreatedBy == "test"));

            commonServiceMock.Verify(x=>x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>()), Times.Once);
            commonServiceMock.Verify(x=>x.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>()), Times.Once);
            commonServiceMock.Verify(x => x.GetEntityNames(It.IsAny<List<ServiceVersioned>>()),Times.Once);
            commonServiceMock.Verify(
                x => x.GetChannelNames(It.IsAny<List<ServiceChannelVersioned>>()), Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(
                    It.IsAny<List<ServiceVersioned>>()), Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                    It.IsAny<List<ServiceChannelVersioned>>()), Times.Once);
            userOrganizationService.Verify(x => x.GetAllUserOrganizationRoles(null), Times.Once);
        }

        [Theory]
        [InlineData(EntityTypeEnum.Service,2,"Added", 0, 0)]
        [InlineData(EntityTypeEnum.Service,5,"Modified", 0, 0)]
        [InlineData(EntityTypeEnum.Service,7,"Deleted", 7, 0)]
        [InlineData(EntityTypeEnum.Service,7,"Deleted", 7, 2)]
        [InlineData(EntityTypeEnum.Service,7,"Deleted", 0, 6)]
        [InlineData(EntityTypeEnum.Channel,1,"Added", 0, 0)]
        [InlineData(EntityTypeEnum.Channel,3,"Modified", 0, 0)]
        [InlineData(EntityTypeEnum.Channel,6,"Deleted", 6, 0)]
        [InlineData(EntityTypeEnum.Channel,6,"Deleted", 6, 2)]
        [InlineData(EntityTypeEnum.Channel,6,"Deleted", 0, 6)]
        public void CheckContentArchived(EntityTypeEnum entityType, int count, string operationType, int resultCount, int monthAge)
        {
            BaseTestSetup(entityType, count, operationType, monthAge);
            var result = (VmNotifications)notificationService.GetContentArchived(
                new VmNotificationSearch
                {
                    SortData = new List<VmSortParam>()
                }, userOrganizationService.Object.GetAllUserOrganizationRoles().Select(i => i.OrganizationId).ToList());

            result.Should().NotBeNull();
            result.Count.Should().Be(resultCount);
            result.Notifications.Count().Should().Be(resultCount);
            result.Notifications.ForEach(x=> Assert.True(x.EntityType == entityType));
            result.Notifications.ForEach(x=> Assert.True(x.OperationType == operationType));
            result.Notifications.ForEach(x=> Assert.True(x.CreatedBy == "test"));

            commonServiceMock.Verify(x=>x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>()), Times.Once);
            commonServiceMock.Verify(x=>x.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>()), Times.Once);
            commonServiceMock.Verify(x => x.GetEntityNames(It.IsAny<List<ServiceVersioned>>()),Times.Once);
            commonServiceMock.Verify(
                x => x.GetChannelNames(It.IsAny<List<ServiceChannelVersioned>>()), Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(
                    It.IsAny<List<ServiceVersioned>>()), Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                    It.IsAny<List<ServiceChannelVersioned>>()), Times.Once);
            userOrganizationService.Verify(x => x.GetAllUserOrganizationRoles(null), Times.Once);
        }

        private List<TrackingEntityVersioned> CreateTrackingEntities(EntityTypeEnum entityType, int count, string operationType, int monthAge)
        {
            var result = new List<TrackingEntityVersioned>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateTrackingEntity(entityType, i.ToString(), operationType, monthAge));
            }
            return result;
        }

        private TrackingEntityVersioned CreateTrackingEntity(EntityTypeEnum entityType, string postFix, string operationType, int monthAge)
        {
            return new TrackingEntityVersioned
            {
                UnificRootId = (entityType+postFix).GetGuid(),
                EntityType = entityType.ToString(),
                OrganizationId = "myOrg".GetGuid(),
                Id = Guid.NewGuid(),
                OperationType = operationType,
                CreatedBy = "test",
                Created = DateTime.UtcNow.AddMonths(-monthAge)
            };
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
                UnificRootId = typeof(T)==typeof(ServiceVersioned) ?
                    (EntityTypeEnum.Service+postFix).GetGuid() :
                    (EntityTypeEnum.Channel+postFix).GetGuid(),
                Id = typeof(T)==typeof(ServiceVersioned) ?
                    (EntityTypeEnum.Service+postFix).GetGuid() :
                    (EntityTypeEnum.Channel+postFix).GetGuid(),
                PublishingStatusId = PublishingStatus.Draft.ToString().GetGuid()
            };
        }
    }
}
