using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceCollectionService>), RegisterType.Scope)]
    internal class ServiceCollectionServiceEntityTracker : IEntityTracker<ServiceCollectionService>
    {
        public void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<ServiceCollectionService>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingServiceCollectionServiceRepository>();
            entities.ForEach(i =>
            {
                repository.Add(new TrackingServiceCollectionService()
                {
                    Id = Guid.NewGuid(),
                    ServiceId = i.Entity.ServiceId,
                    ServiceCollectionId = i.Entity.ServiceCollectionId,
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
