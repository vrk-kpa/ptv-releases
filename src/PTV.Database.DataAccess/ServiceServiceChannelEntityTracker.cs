using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceServiceChannel>), RegisterType.Scope)]
    internal class ServiceServiceChannelEntityTracker : IEntityTracker<ServiceServiceChannel>
    {
        public void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<ServiceServiceChannel>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingServiceServiceChannelRepository>();
            var entityEntries = entities.ToList();
            entityEntries.ForEach(i =>
            {
                repository.Add(new TrackingServiceServiceChannel()
                {
                    Id = Guid.NewGuid(),
                    ServiceId = i.Entity.ServiceId,
                    ChannelId = i.Entity.ServiceChannelId,
                    OperationType = i.State.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationId = trackingInformation.OperationId
                });
            });
            var entityRepository = trackingInformation.UnitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            entityEntries.GroupBy(x=>x.Entity.ServiceChannelId).Select(x=>x.First()).ForEach(i =>
            {
                entityRepository.Add(new TrackingEntityVersioned()
                {
                    Id = Guid.NewGuid(),
                    UnificRootId = i.Entity.ServiceChannelId,
                    OrganizationId = GetEntityOrganization<ServiceChannelVersioned>(i.Entity.ServiceChannelId, trackingInformation.UnitOfWork),
                    EntityType = EntityTypeEnum.Channel.ToString(),
                    OperationType = EntityState.Modified.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationId = trackingInformation.OperationId
                });
            });
            entityEntries.GroupBy(x=>x.Entity.ServiceId).Select(x=>x.First()).ForEach(i =>
            {
                entityRepository.Add(new TrackingEntityVersioned()
                {
                    Id = Guid.NewGuid(),
                    UnificRootId = i.Entity.ServiceId,
                    OrganizationId = GetEntityOrganization<ServiceVersioned>(i.Entity.ServiceId, trackingInformation.UnitOfWork),
                    EntityType = EntityTypeEnum.Service.ToString(),
                    OperationType = EntityState.Modified.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationId = trackingInformation.OperationId
                });
            });
        }

        private Guid GetEntityOrganization<TEntity>(Guid unificRootId, IUnitOfWork unitOfWork) where TEntity : class, IVersionedVolume, IAuditing, IOrganizationInfo
        {
            var rep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            return rep.All().Where(x => x.UnificRootId == unificRootId).OrderByDescending(x => x.Modified).First()
                .OrganizationId;
        }
    }
}