using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceChannelVersioned>), RegisterType.Scope)]
    internal class ServiceChannelVersionedEntityTracker : IEntityTracker<ServiceChannelVersioned>
    {
        private IVersioningManager versioningManager;
        private ITypesCache typesCache;
        public ServiceChannelVersionedEntityTracker(ICacheManager cacheManager, IVersioningManager versioningManager)
        {
            this.typesCache = cacheManager.TypesCache;
            this.versioningManager = versioningManager;
        }

        public void Perform(TrackingContextInfo trackingInformation,
            IEnumerable<EntityEntry<ServiceChannelVersioned>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()); 
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var deletedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()); 
            var notCommonConnectionTypeId = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            // track changed channels
            entities
                .GroupBy(entity => entity.Entity.UnificRootId)
                .Select(entity => entity.First())
                .Where(entity =>
                    !(entity.Entity.PublishingStatusId == draftStatusId && entity.State == EntityState.Added))
                .ForEach(i =>
                {
                    var originalConnectionType = i.OriginalValues.GetValue<Guid>("ConnectionTypeId");
                    var currentConnectionType = i.CurrentValues.GetValue<Guid>("ConnectionTypeId");
                    if ((originalConnectionType != currentConnectionType  || 
                        (i.State == EntityState.Added && i.Entity.PublishingStatusId == modifiedStatusId)) &&
                        currentConnectionType == notCommonConnectionTypeId)
                    {
                        ProcessNotCommonServiceChannel(trackingInformation, i, i.State == EntityState.Added && i.Entity.PublishingStatusId == modifiedStatusId);
                    }

                    var serviceChannelTrack = new TrackingEntityVersioned()
                    {
                        Id = Guid.NewGuid(),
                        UnificRootId = i.Entity.UnificRootId,
                        EntityType = EntityTypeEnum.Channel.ToString(),
                        OrganizationId = i.Entity.OrganizationId,
                        OperationType = i.Entity.PublishingStatusId == deletedStatusId ? EntityState.Deleted.ToString() : EntityState.Modified.ToString(),
                        Modified = trackingInformation.TimeStamp,
                        Created = trackingInformation.TimeStamp,
                        CreatedBy = trackingInformation.UserName,
                        ModifiedBy = trackingInformation.UserName,
                        LastOperationId = trackingInformation.OperationId
                    };
                    repository.Add(serviceChannelTrack);
                });           
        }

        private void ProcessNotCommonServiceChannel(TrackingContextInfo trackingInformation,
            EntityEntry<ServiceChannelVersioned> entity, bool checkPublishValue)
        {
            if (checkPublishValue)
            {
                var lastPublished = versioningManager.GetSpecificVersionByRoot<ServiceChannelVersioned>(trackingInformation.UnitOfWork, entity.Entity.UnificRootId, PublishingStatus.Published);
                if (lastPublished != null && lastPublished.ConnectionTypeId == entity.Entity.ConnectionTypeId)
                {
                    return;
                }
            }

            var commonChannelrepository = trackingInformation.UnitOfWork.CreateRepository<ITrackingOrganizationChannelRepository>();
            var channelId = entity.Entity.UnificRootId;
            var connectionRep = trackingInformation.UnitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var serviceIds = connectionRep.All()
                .Where(x => channelId == x.ServiceChannelId)
                .Select(x => x.ServiceId);
            var serviceRep = trackingInformation.UnitOfWork.CreateRepository<IServiceVersionedRepository>();
            serviceRep.All()
                .Where(x => serviceIds.Contains(x.UnificRootId) && x.OrganizationId != entity.Entity.OrganizationId)
                .Select(x => x.OrganizationId)
                .Distinct()
                .ForEach(orgId =>
                {
                    var organizationChannelTrack = new TrackingOrganizationChannel()
                    {
                        Id = Guid.NewGuid(),
                        ServiceChannelId = channelId,
                        OrganizationId = orgId,
                        OperationType = entity.State.ToString(),
                        Modified = trackingInformation.TimeStamp,
                        Created = trackingInformation.TimeStamp,
                        CreatedBy = trackingInformation.UserName,
                        ModifiedBy = trackingInformation.UserName,
                        LastOperationId = trackingInformation.OperationId
                    };
                    commonChannelrepository.Add(organizationChannelTrack);
                });
        }
    }
}