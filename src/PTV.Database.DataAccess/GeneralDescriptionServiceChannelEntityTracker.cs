using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<GeneralDescriptionServiceChannel>), RegisterType.Scope)]
    internal class GeneralDescriptionServiceChannelEntityTracker : IEntityTracker<GeneralDescriptionServiceChannel>
    {
        public void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<GeneralDescriptionServiceChannel>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingGeneralDescriptionServiceChannelRepository>();
            var generalDescriptionTrackingRepository = trackingInformation.UnitOfWork.CreateRepository<ITrackingGeneralDescriptionVersionedRepository>();
            var entityEntries = entities.ToList();
            entityEntries.ForEach(i =>
            {
                repository.Add(new TrackingGeneralDescriptionServiceChannel()
                {
                    Id = Guid.NewGuid(),
                    GeneralDescriptionId = i.Entity.StatutoryServiceGeneralDescriptionId,
                    ChannelId = i.Entity.ServiceChannelId,
                    OperationType = i.State.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationId = trackingInformation.OperationId
                });               
            });
            entityEntries.GroupBy(x=>x.Entity.StatutoryServiceGeneralDescriptionId).Select(x=>x.First()).ForEach(i =>
            {
                generalDescriptionTrackingRepository.Add(new TrackingGeneralDescriptionVersioned()
                {
                    Id = Guid.NewGuid(),
                    GenerealDescriptionId = i.Entity.StatutoryServiceGeneralDescriptionId,
                    OperationType = EntityState.Modified.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationId = trackingInformation.OperationId
                });
            });
            
            
        }
    }
}