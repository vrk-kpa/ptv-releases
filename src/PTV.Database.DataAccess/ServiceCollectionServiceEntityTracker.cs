/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
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
