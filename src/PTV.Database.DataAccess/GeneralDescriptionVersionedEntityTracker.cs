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
    [RegisterService(typeof(IEntityTracker<StatutoryServiceGeneralDescriptionVersioned>), RegisterType.Scope)]
    internal class GeneralDescriptionVersionedEntityTracker : IEntityTracker<StatutoryServiceGeneralDescriptionVersioned>
    {
        private ITypesCache typesCache;
        public GeneralDescriptionVersionedEntityTracker(ICacheManager cacheManager)
        {
            this.typesCache = cacheManager.TypesCache;
        }
        public void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<StatutoryServiceGeneralDescriptionVersioned>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingGeneralDescriptionVersionedRepository>();
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()); 
            var deletedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()); 
            entities
                .GroupBy(entity => entity.Entity.UnificRootId)
                .Select(entity => entity.First())
                .Where(entity => entity.Entity.PublishingStatusId != deletedStatusId)
                .ForEach(i =>
            {              
                var gdTrack = new TrackingGeneralDescriptionVersioned()
                {
                    Id = Guid.NewGuid(),
                    GenerealDescriptionId = i.Entity.UnificRootId,
                    OperationType = i.Entity.PublishingStatusId == draftStatusId && i.State == EntityState.Added ? EntityState.Added.ToString() : EntityState.Modified.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationId = trackingInformation.OperationId
                };
                repository.Add(gdTrack);
            });
        }
    }
}