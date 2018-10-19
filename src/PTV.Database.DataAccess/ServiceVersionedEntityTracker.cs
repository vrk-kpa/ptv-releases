using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceVersioned>), RegisterType.Scope)]
    internal class ServiceVersionedEntityTracker : IEntityTracker<ServiceVersioned>
    {
        private ITypesCache typesCache;
        public ServiceVersionedEntityTracker(ICacheManager cacheManager)
        {
            this.typesCache = cacheManager.TypesCache;
        }
        public void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<ServiceVersioned>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()); 
            var deletedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()); 
            entities
                .GroupBy(entity => entity.Entity.UnificRootId)
                .Select(entity => entity.First())
                .Where(entity => !(entity.Entity.PublishingStatusId == draftStatusId && entity.State == EntityState.Added))
                .ForEach(i =>
            {               
                var serviceTrack = new TrackingEntityVersioned()
                {
                    Id = Guid.NewGuid(),
                    UnificRootId = i.Entity.UnificRootId,
                    EntityType = EntityTypeEnum.Service.ToString(),
                    OrganizationId = i.Entity.OrganizationId,
                    OperationType = i.Entity.PublishingStatusId == deletedStatusId ? EntityState.Deleted.ToString() : EntityState.Modified.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationId = trackingInformation.OperationId
                };
                repository.Add(serviceTrack);
            });
        }
    }
}