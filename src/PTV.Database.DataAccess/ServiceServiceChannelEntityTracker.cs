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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceServiceChannel>), RegisterType.Scope)]
    internal class ServiceServiceChannelEntityTracker : IEntityTracker<ServiceServiceChannel>
    {
        private readonly IPahaTokenAccessor pahaTokenAccessor;

        public ServiceServiceChannelEntityTracker(IPahaTokenAccessor pahaTokenAccessor)
        {
            this.pahaTokenAccessor = pahaTokenAccessor;
        }

        public void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<ServiceServiceChannel>> entities)
        {
            var repository = trackingInformation.UnitOfWork.CreateRepository<ITrackingServiceServiceChannelRepository>();
            var entityEntries = entities.ToList();
            entityEntries.ForEach(i =>
            {
                repository.Add(new TrackingServiceServiceChannel
                {
                    Id = Guid.NewGuid(),
                    ServiceId = i.Entity.ServiceId,
                    ChannelId = i.Entity.ServiceChannelId,
                    OperationType = i.State.ToString(),
                    Modified = trackingInformation.TimeStamp,
                    Created = trackingInformation.TimeStamp,
                    CreatedBy = trackingInformation.UserName,
                    ModifiedBy = trackingInformation.UserName,
                    LastOperationIdentifier = trackingInformation.OperationId,
                    LastOperationTimeStamp = trackingInformation.TimeStamp,
                    LastOperationType = pahaTokenAccessor.UserRole.GetLastOperationType(i.State, i.Entity.LastOperationType)
                });
            });
        }
    }
}
