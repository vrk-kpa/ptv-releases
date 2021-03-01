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
    public class GetChannelInCommonUseTest : TestBase
    {

        private readonly NotificationService notificationService;
        private readonly Mock<ICommonServiceInternal> commonServiceMock;
        private readonly Mock<IUserOrganizationService> userOrganizationService;
        private readonly Mock<IPahaTokenAccessor> pahaTokenProcessorMock;

        public GetChannelInCommonUseTest()
        {
            userOrganizationService = new Mock<IUserOrganizationService>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            pahaTokenProcessorMock = new Mock<IPahaTokenAccessor>(MockBehavior.Strict);

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
                null,
                null
            );
            SetupTypesCacheMock<PublishingStatusType>();
        }

        private void BaseTestSetup(int myUserCount, int myUserFilterCount, int otherOrgCount)
        {
            var myOrgId = "myOrg".GetGuid();
            SetupContextManager<object, IVmSearchBase>();
            userOrganizationService
                .Setup(x => x.GetAllUserOrganizationRoles(null))
                .Returns(new List<IUserOrganizationRoles>
                    {new UserOrganizationRoles {OrganizationId = myOrgId}});

            pahaTokenProcessorMock.Setup(x => x.UserName).Returns("test");

            RegisterRepository<INotificationServiceServiceChannelRepository, NotificationServiceServiceChannel>
                (CreateNotifications(myUserCount, myUserFilterCount, otherOrgCount, myOrgId).AsQueryable());

            RegisterRepository<IServiceNameRepository, ServiceName>
                (CreateServiceNames(myUserCount).AsQueryable());


            commonServiceMock.Setup(x =>
                    x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                        It.IsAny<IEnumerable<Guid>>(),
                        unitOfWorkMockSetup.Object,
                        It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>()))
                .Returns(CreateEntities<ServiceVersioned>(myUserCount));
            commonServiceMock.Setup(x => x.GetEntityNames(It.IsAny<List<ServiceVersioned>>()))
                .Returns((List<ServiceVersioned> input) => input.ToDictionary(x=>x.UnificRootId, v=>new Dictionary<string, string>{{"fi","name"}}));

             commonServiceMock.Setup(x =>
                    x.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(
                        It.IsAny<List<ServiceVersioned>>()))
                .Returns((List<ServiceVersioned> input) => input.ToDictionary<ServiceVersioned,Guid,IReadOnlyList<VmLanguageAvailabilityInfo>>(
                    k=>k.UnificRootId,
                    v=> new List<VmLanguageAvailabilityInfo>{new VmLanguageAvailabilityInfo{LanguageId = "fi".GetGuid()}}));
        }


        [Theory]
        [InlineData(2, 0, 0)]
        [InlineData(2, 0, 2)]
        [InlineData(2, 2, 0)]
        [InlineData(2, 2, 2)]
        [InlineData(0, 0, 2)]
        public void CheckChannelInCommonUse(int myUserCount, int myUserFilterCount, int otherOrgCount)
        {
            BaseTestSetup(myUserCount, myUserFilterCount, otherOrgCount);
            var result = (VmNotifications)notificationService.GetChannelInCommonUse(new VmNotificationSearch
            {
              SortData   = new List<VmSortParam>()
            }, userOrganizationService.Object.GetAllUserOrganizationRoles().Select(i => i.OrganizationId).ToList());

            result.Should().NotBeNull();
            result.Count.Should().Be(myUserCount-myUserFilterCount);
            result.Notifications.Count().Should().Be(myUserCount-myUserFilterCount);
            result.Notifications.ForEach(x=> Assert.True(x.CreatedBy == "test"));

            commonServiceMock.Verify(x=>x.GetNotificationEntity<ServiceVersioned, ServiceLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>()), Times.Once);
            commonServiceMock.Verify(x => x.GetEntityNames(It.IsAny<List<ServiceVersioned>>()),Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(
                    It.IsAny<List<ServiceVersioned>>()), Times.Once);
            userOrganizationService.Verify(x => x.GetAllUserOrganizationRoles(null), Times.Once);
        }

        private List<NotificationServiceServiceChannel> CreateNotifications(int myUserCount, int myUserFilterCount, int otherOrgCount, Guid myOrg)
        {
            var result = new List<NotificationServiceServiceChannel>();
            for (int i = 1; i <= myUserCount; i++)
            {
                result.Add(CreateNotification(i.ToString(), myOrg, "test", myUserFilterCount));
            }
            for (int i = 1; i <= otherOrgCount; i++)
            {
                result.Add(CreateNotification(i.ToString(), "otherOrg".GetGuid(), "test", 0));
            }
            return result;
        }

        private NotificationServiceServiceChannel CreateNotification(string postFix, Guid org, string name, int myUserFilterCount)
        {
            var filters = new List<NotificationServiceServiceChannelFilter>();
            for (int i = 1; i <= myUserFilterCount; i++)
            {
                filters.Add(new NotificationServiceServiceChannelFilter
                {
                    UserId = name.GetGuid()
                });
            }
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
