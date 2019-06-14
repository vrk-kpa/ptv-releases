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

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
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
    public class GetConnectionChangesTest : TestBase
    {

        private readonly NotificationService notificationService;
        private Mock<ICommonServiceInternal> commonServiceMock;
        private Mock<ISearchServiceInternal> searchServiceMock;
        private Mock<IUserOrganizationService> userOrganizationService;


        public GetConnectionChangesTest()
        {
            userOrganizationService = new Mock<IUserOrganizationService>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            searchServiceMock = new Mock<ISearchServiceInternal>(MockBehavior.Strict);
            
            notificationService = new NotificationService
            (
                contextManagerMock.Object,
                null,
                null,
                CacheManagerMock.Object,
                commonServiceMock.Object,
                null,
                null,
                null,
                null,
                null,
                searchServiceMock.Object,
                userOrganizationService.Object
            );
            SetupTypesCacheMock<PublishingStatusType>();
        }

        private void BaseTestSetup(EntityTypeEnum entityType, int count, string operationType)
        {
            var myOrgId = "myOrg".GetGuid();
            SetupContextManager<object, IVmSearchBase>();
            userOrganizationService
                .Setup(x => x.GetAllUserOrganizationRoles(null))
                .Returns(new List<IUserOrganizationRoles>
                    {new UserOrganizationRoles {OrganizationId = myOrgId}});
//            userOrganizationService 
//                .Setup(x => x.GetAllUserOrganizationIds())
//                .Returns(new List<Guid> { myOrgId});
            var searchConnectionResult = CreateNotifications(entityType, count, operationType);
            searchServiceMock.Setup(x => x.SearchConnections(It.IsAny<IVmEntitySearch>()))
                .Returns((IVmEntitySearch input) => new VmSearchResult<IVmNotificationConnectionListItem>
                {
                    SearchResult = searchConnectionResult,
                    Count = searchConnectionResult.Count,
                    MaxPageCount = input.MaxPageCount,
                    Skip = input.Skip,
                    PageNumber = input.PageNumber,
                });

            commonServiceMock.Setup(x =>
                    x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                        It.IsAny<IEnumerable<Guid>>(),
                        unitOfWorkMockSetup.Object,
                        It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
                        null))
                .Returns(CreateEntities<ServiceVersioned>(count));
            commonServiceMock.Setup(x => x.GetEntityNames<ServiceVersioned>(It.IsAny<List<ServiceVersioned>>()))
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
                        It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                        null))
                .Returns(CreateEntities<ServiceChannelVersioned>(count));
            commonServiceMock.Setup(x => x.GetEntityNames<ServiceChannelVersioned>(It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> input) => input.ToDictionary(x=>x.UnificRootId, v=>new Dictionary<string, string>{{"fi","name"}}));
            commonServiceMock.Setup(x =>
                    x.GetLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                        It.IsAny<List<ServiceChannelVersioned>>()))
                .Returns((List<ServiceChannelVersioned> input) => input.ToDictionary<ServiceChannelVersioned,Guid,IReadOnlyList<VmLanguageAvailabilityInfo>>(
                    k=>k.UnificRootId, 
                    v=> new List<VmLanguageAvailabilityInfo>{new VmLanguageAvailabilityInfo{LanguageId = "fi".GetGuid()}}));
        }
        
       
        [Theory]
        [InlineData(EntityTypeEnum.Service,2,"Added")]
        [InlineData(EntityTypeEnum.Service,5,"Modified")]
        [InlineData(EntityTypeEnum.Service,7,"Deleted")]
        [InlineData(EntityTypeEnum.Channel,1,"Added")]
        [InlineData(EntityTypeEnum.Channel,3,"Modified")]
        [InlineData(EntityTypeEnum.Channel,6,"Deleted")]
        public void CheckConnectionChanges(EntityTypeEnum entityType, int count, string operationType)
        {
            BaseTestSetup(entityType, count, operationType);
            var result = (VmNotifications)notificationService.GetConnectionChanges(new VmNotificationSearch(), userOrganizationService.Object.GetAllUserOrganizationRoles().Select(i => i.OrganizationId).ToList());
                
            result.Should().NotBeNull();
            result.Count.Should().Be(count);
            result.Notifications.Count().Should().Be(count);
            result.Notifications.ForEach(x=> Assert.True(x.VersionedId != x.ConnectedVersionedId));
            result.Notifications.ForEach(x=> Assert.True(x.EntityType == entityType));
            result.Notifications.ForEach(x=> Assert.True(x.ConnectedMainEntityType != entityType));
            result.Notifications.ForEach(x=> Assert.True(x.OperationType == operationType));
            result.Notifications.ForEach(x=> Assert.True(x.CreatedBy == "test"));

            searchServiceMock.Verify(x => x.SearchConnections(It.IsAny<IVmEntitySearch>()), Times.Once);
            commonServiceMock.Verify(x=>x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
                null), Times.Once);
            commonServiceMock.Verify(x=>x.GetNotificationEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                null), Times.Once);
            commonServiceMock.Verify(x => x.GetEntityNames<ServiceVersioned>(It.IsAny<List<ServiceVersioned>>()),Times.Once);
            commonServiceMock.Verify(
                x => x.GetEntityNames<ServiceChannelVersioned>(It.IsAny<List<ServiceChannelVersioned>>()), Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(
                    It.IsAny<List<ServiceVersioned>>()), Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                    It.IsAny<List<ServiceChannelVersioned>>()), Times.Once);
            userOrganizationService.Verify(x => x.GetAllUserOrganizationRoles(null), Times.Once);
        }

        private List<VmNotificationConnectionListItem> CreateNotifications(EntityTypeEnum entityType, int count, string operationType)
        {
            var result = new List<VmNotificationConnectionListItem>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateNotification(entityType, i.ToString(), operationType));
            }
            return result;
        }

        private VmNotificationConnectionListItem CreateNotification(EntityTypeEnum entityType, string postFix, string operationType)
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