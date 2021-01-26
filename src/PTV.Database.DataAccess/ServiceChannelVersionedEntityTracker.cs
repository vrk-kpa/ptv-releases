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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceChannelVersioned>), RegisterType.Scope)]
    internal class ServiceChannelVersionedEntityTracker : IEntityTracker<ServiceChannelVersioned>
    {
        private readonly ITypesCache typesCache;
        private readonly IServiceUtilities serviceUtilities;
        private readonly IPahaTokenAccessor pahaTokenAccessor;

        public ServiceChannelVersionedEntityTracker(
            ICacheManager cacheManager,
            IServiceUtilities serviceUtilities,
            IPahaTokenAccessor pahaTokenAccessor)
        {
            typesCache = cacheManager.TypesCache;
            this.serviceUtilities = serviceUtilities;
            this.pahaTokenAccessor = pahaTokenAccessor;
        }

        public void Perform(TrackingContextInfo trackingInformation,
            IEnumerable<EntityEntry<ServiceChannelVersioned>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var deletedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var notCommonConnectionTypeId =
                typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());

            // track changed channels
            entities
                .GroupBy(entity => entity.Entity.UnificRootId)
                .Select(entity => entity.First())
                .Where(entity => !(entity.Entity.PublishingStatusId == draftStatusId && entity.State == EntityState.Added))
                .Where(entity => entity.Entity.PublishingStatusId != deletedStatusId)
                .ForEach(i =>
                {
                    var currentConnectionType = i.CurrentValues.GetValue<Guid>("ConnectionTypeId");
                    var originalPublishingStatusId = i.OriginalValues.GetValue<Guid>("PublishingStatusId");
                    if ((currentConnectionType == notCommonConnectionTypeId &&
                         i.Entity.PublishingStatusId == publishedStatusId)
                        ||
                        (i.Entity.PublishingStatusId == deletedStatusId &&
                         originalPublishingStatusId != i.Entity.PublishingStatusId))
                    {
                        ProcessNotCommonServiceChannel(trackingInformation, i);
                    }

                    if (serviceUtilities.UserHighestRole() == UserRoleEnum.Eeva &&
                        !serviceUtilities.GetUserOrganizations().Contains(i.Entity.OrganizationId))
                    {
                        repository.All().Where(x => x.UnificRootId == i.Entity.UnificRootId && x.OperationType == EntityState.Modified.ToString())
                            .ForEach(r => repository.Remove(r));
                        var serviceChannelTrack = new TrackingEntityVersioned
                        {
                            Id = Guid.NewGuid(),
                            UnificRootId = i.Entity.UnificRootId,
                            EntityType = EntityTypeEnum.Channel.ToString(),
                            OrganizationId = i.Entity.OrganizationId,
                            OperationType = EntityState.Modified.ToString(),
                            Modified = trackingInformation.TimeStamp,
                            Created = trackingInformation.TimeStamp,
                            CreatedBy = i.Entity.ModifiedBy, //trackingInformation.UserName, (changed because of SFIPTV-422)
                            ModifiedBy = i.Entity.ModifiedBy, // trackingInformation.UserName, (changed because of SFIPTV-422)
                            LastOperationIdentifier = trackingInformation.OperationId,
                            LastOperationTimeStamp = trackingInformation.TimeStamp,
                            LastOperationType = pahaTokenAccessor.UserRole.GetLastOperationType(i.State, i.Entity.LastOperationType)
                        };
                        repository.Add(serviceChannelTrack);
                    };
                });
        }

        private void ProcessNotCommonServiceChannel(TrackingContextInfo trackingInformation,
            EntityEntry<ServiceChannelVersioned> entity)
        {
            var notificationServiceServiceChannelRep = trackingInformation.UnitOfWork.CreateRepository<INotificationServiceServiceChannelRepository>();
            var channelId = entity.Entity.UnificRootId;
            var connectionRep = trackingInformation.UnitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var serviceIds = connectionRep.All()
                .Where(x => channelId == x.ServiceChannelId)
                .Select(x => x.ServiceId);
            var serviceRep = trackingInformation.UnitOfWork.CreateRepository<IServiceVersionedRepository>();
            var filteredServiceIds = serviceRep.All()
                .Where(x => serviceIds.Contains(x.UnificRootId) && x.OrganizationId != entity.Entity.OrganizationId)
                .ToList()
                .DistinctBy(x => x.UnificRootId)
                .ToDictionary(x => x.UnificRootId, y => y.OrganizationId);
            filteredServiceIds.ForEach(x =>
                {
                    notificationServiceServiceChannelRep.Add(new NotificationServiceServiceChannel
                        {
                            Id = Guid.NewGuid(),
                            ServiceId = x.Key,
                            OrganizationId = x.Value,
                            ChannelId = channelId,
                            Modified = trackingInformation.TimeStamp,
                            Created = trackingInformation.TimeStamp,
                            CreatedBy = trackingInformation.UserName,
                            ModifiedBy = trackingInformation.UserName,
                            LastOperationIdentifier = trackingInformation.OperationId,
                            LastOperationTimeStamp = trackingInformation.TimeStamp,
                            LastOperationType = pahaTokenAccessor.UserRole.GetLastOperationType(EntityState.Added)
                        });
                });
        }
    }
}
