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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Notifications;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Notification
{
    public class GetGeneralDescriptionChangesTest : TestBase
    {
        private readonly NotificationService notificationService;
        private readonly Mock<ICommonServiceInternal> commonServiceMock;
        private readonly Mock<IVersioningManager> versioningManagerMock;

        public GetGeneralDescriptionChangesTest()
        {
            new Mock<IUserOrganizationService>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            versioningManagerMock = new Mock<IVersioningManager>();


            notificationService = new NotificationService
            (
                contextManagerMock.Object,
                versioningManagerMock.Object,
                CacheManagerMock.Object,
                commonServiceMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                 null
            );
            SetupTypesCacheMock<PublishingStatusType>();
        }

        private void BaseTestSetup(int count, string operationType)
        {
            SetupContextManager<object, IVmSearchBase>();

            RegisterRepository<ITrackingGeneralDescriptionVersionedRepository, TrackingGeneralDescriptionVersioned>(
                CreateTrackingEntities(count, operationType).AsQueryable());

            RegisterRepository<IStatutoryServiceGeneralDescriptionVersionedRepository, StatutoryServiceGeneralDescriptionVersioned>(
                CreateEntities<StatutoryServiceGeneralDescriptionVersioned>(count).AsQueryable());

            RegisterRepository<IStatutoryServiceNameRepository, StatutoryServiceName>
                (CreateGeneralNames(count).AsQueryable());

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


        [Theory]
        [InlineData(2,"Added", 0)]
        [InlineData(5,"Modified", 5)]
        [InlineData(7,"Deleted", 0)]
        public void CheckGeneralDescriptionChanged(int count, string operationType, int resultCount)
        {
            BaseTestSetup(count, operationType);
            var result = (VmNotifications)notificationService.GetGeneralDescriptionChanged(
                new VmNotificationSearch
                {
                    SortData = new List<VmSortParam>()
                });

            result.Should().NotBeNull();
            result.Count.Should().Be(resultCount);
            result.Notifications.Count().Should().Be(resultCount);
            result.Notifications.ForEach(x=> Assert.True(x.EntityType == EntityTypeEnum.GeneralDescription));
            result.Notifications.ForEach(x=> Assert.True(x.OperationType == operationType));
            result.Notifications.ForEach(x=> Assert.True(x.CreatedBy == "test"));

            commonServiceMock.Verify(x=>x.GetNotificationEntityCustom<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>()), Times.Once);

            commonServiceMock.Verify(x => x.GetEntityNames(It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()),Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                    It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Once);
        }

        [Theory]
        [InlineData(2,"Added", 2)]
        [InlineData(5,"Modified", 0)]
        [InlineData(7,"Deleted", 0)]
        public void CheckGeneralDescriptionAdded(int count, string operationType, int resultCount)
        {
            BaseTestSetup(count, operationType);
            var result = (VmNotifications)notificationService.GetGeneralDescriptionAdded(
                new VmNotificationSearch
                {
                    SortData = new List<VmSortParam>()
                });

            result.Should().NotBeNull();
            result.Count.Should().Be(resultCount);
            result.Notifications.Count().Should().Be(resultCount);
            result.Notifications.ForEach(x=> Assert.True(x.EntityType == EntityTypeEnum.GeneralDescription));
            result.Notifications.ForEach(x=> Assert.True(x.OperationType == operationType));
            result.Notifications.ForEach(x=> Assert.True(x.CreatedBy == "test"));

            commonServiceMock.Verify(x=>x.GetNotificationEntityCustom<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                It.IsAny<IEnumerable<Guid>>(),
                unitOfWorkMockSetup.Object,
                It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>()), Times.Once);

            commonServiceMock.Verify(x => x.GetEntityNames(It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()),Times.Once);
            commonServiceMock.Verify(x =>
                x.GetLanguageAvailabilites<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                    It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Once);
        }

        private List<TrackingGeneralDescriptionVersioned> CreateTrackingEntities(int count, string operationType)
        {
            var result = new List<TrackingGeneralDescriptionVersioned>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateTrackingEntity(i.ToString(), operationType));
            }
            return result;
        }

        private TrackingGeneralDescriptionVersioned CreateTrackingEntity(string postFix, string operationType)
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
                    (EntityTypeEnum.GeneralDescription+postFix).GetGuid(),
                Id =
                    (EntityTypeEnum.GeneralDescription+postFix).GetGuid(),
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid()
            };
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
    }
}
