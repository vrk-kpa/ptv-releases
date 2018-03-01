using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceServiceChannel>), RegisterType.Scope)]
    internal class ServiceServiceChannelEntityTracker : IEntityTracker<ServiceServiceChannel>
    {
        public void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<ServiceServiceChannel>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingServiceServiceChannelRepository>();
            entities.ForEach(i =>
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
        }
    }
}