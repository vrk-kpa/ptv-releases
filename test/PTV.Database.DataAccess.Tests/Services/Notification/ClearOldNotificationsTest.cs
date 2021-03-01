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
using Xunit;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Services.Notification
{
    public class ClearOldNotificationsTest : TestBase
    {

        private readonly NotificationService notificationService;

        public ClearOldNotificationsTest()
        {
            notificationService = new NotificationService
            (
                contextManagerMock.Object,
                null,
                CacheManagerMock.Object,
                null,
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

        private void BaseTestSetup()
        {
            SetupContextManager<object, IVmSearchBase>();
        }


        [Theory]
        [InlineData(10,0, 0, 0)]
        [InlineData(10,1, 0, 10)]
        [InlineData(10,2, 0, 10)]
        [InlineData(10,6, 10, 10)]
        [InlineData(10,12,10, 10)]
        //count - number of notifications
        //mothAge - age in month of notifications
        //trackingResult - number of removed notifications from TrackingEntityVersionedRepository
        //restResult - number of removed notifications from every other tracking tables
        public void CheckClearOldNotifications(int count, int monthAge, int trackingResult, int restResult)
        {
            BaseTestSetup();
            var trackingEntities =
                CreateTrackingEntities(EntityTypeEnum.Channel, count, EntityState.Deleted.ToString(), monthAge);
            var trackingEntitiesRepo = RegisterRepository<ITrackingEntityVersionedRepository, TrackingEntityVersioned>(
                trackingEntities.AsQueryable());
            trackingEntitiesRepo.Setup(
                r => r.Remove(It.IsAny<TrackingEntityVersioned>()));
            var trackingGdEntities = CreateEntities<TrackingGeneralDescriptionVersioned>(count, monthAge);
            var trackingGeneralDescriptionRepo = RegisterRepository<IRepository<TrackingGeneralDescriptionVersioned>, TrackingGeneralDescriptionVersioned>(
                trackingGdEntities.AsQueryable());
            trackingGeneralDescriptionRepo.Setup(
                r => r.Remove(It.IsAny<TrackingGeneralDescriptionVersioned>()));
            var trackingTranslationEntities = CreateEntities<TrackingTranslationOrder>(count, monthAge);
            var trackingTranslationOrderRepo = RegisterRepository<IRepository<TrackingTranslationOrder>, TrackingTranslationOrder>(
                trackingTranslationEntities.AsQueryable());
            trackingTranslationOrderRepo.Setup(
                r => r.Remove(It.IsAny<TrackingTranslationOrder>()));
            var notifications = CreateEntities<NotificationServiceServiceChannel>(count, monthAge);
            var notificationsRepo = RegisterRepository<IRepository<NotificationServiceServiceChannel>, NotificationServiceServiceChannel>(
                notifications.AsQueryable());
            notificationsRepo.Setup(
                r => r.Remove(It.IsAny<NotificationServiceServiceChannel>()));

            notificationService.ClearOldNotifications(null);

            trackingEntitiesRepo.Verify(
                r => r.Remove(It.IsAny<TrackingEntityVersioned>()), Times.Exactly(trackingResult));
            trackingGeneralDescriptionRepo.Verify(
                r => r.Remove(It.IsAny<TrackingGeneralDescriptionVersioned>()), Times.Exactly(restResult));
            trackingTranslationOrderRepo.Verify(
                r => r.Remove(It.IsAny<TrackingTranslationOrder>()), Times.Exactly(restResult));
            notificationsRepo.Verify(
                r => r.Remove(It.IsAny<NotificationServiceServiceChannel>()), Times.Exactly(restResult));

        }


        private List<TrackingEntityVersioned> CreateTrackingEntities(EntityTypeEnum entityType, int count,
            string operationType, int monthAge)
        {
            var result = new List<TrackingEntityVersioned>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateTrackingEntity(entityType, i.ToString(), operationType, monthAge));
            }

            return result;
        }

        private TrackingEntityVersioned CreateTrackingEntity(EntityTypeEnum entityType, string postFix,
            string operationType, int monthAge)
        {
            return new TrackingEntityVersioned
            {
                UnificRootId = (entityType + postFix).GetGuid(),
                EntityType = entityType.ToString(),
                OrganizationId = "myOrg".GetGuid(),
                Id = Guid.NewGuid(),
                OperationType = operationType,
                CreatedBy = "test",
                Created = DateTime.UtcNow.AddMonths(-monthAge)
            };
        }

        private List<T> CreateEntities<T>(int count, int monthAge) where T: class, IAuditing, new()
        {
            var result = new List<T>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(CreateEntity<T>(monthAge));
            }

            return result;
        }

        private T CreateEntity<T>(int monthAge) where T: class, IAuditing, new()
        {
            return new T
            {
                CreatedBy = "test",
                Created = DateTime.UtcNow.AddMonths(-monthAge)
            };
        }

    }
}
