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
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IEntityTracker<ServiceVersioned>), RegisterType.Scope)]
    internal class ServiceVersionedEntityTracker : IEntityTracker<ServiceVersioned>
    {
        private ITypesCache typesCache;
        private IServiceUtilities serviceUtilities;
        public ServiceVersionedEntityTracker(
            ICacheManager cacheManager,
            IServiceUtilities serviceUtilities)
        {
            this.typesCache = cacheManager.TypesCache;
            this.serviceUtilities = serviceUtilities;
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
                .Where(entity => entity.Entity.PublishingStatusId != deletedStatusId)
                .ForEach(i =>
            {
                if (serviceUtilities.UserHighestRole() == UserRoleEnum.Eeva &&
                    !serviceUtilities.GetUserOrganizations().Contains(i.Entity.OrganizationId))
                {
                    repository.All().Where(x => x.UnificRootId == i.Entity.UnificRootId && x.OperationType == EntityState.Modified.ToString())
                        .ForEach(r => repository.Remove(r));
                    var serviceTrack = new TrackingEntityVersioned()
                    {
                        Id = Guid.NewGuid(),
                        UnificRootId = i.Entity.UnificRootId,
                        EntityType = EntityTypeEnum.Service.ToString(),
                        OrganizationId = i.Entity.OrganizationId,
                        OperationType = EntityState.Modified.ToString(),
                        Modified = trackingInformation.TimeStamp,
                        Created = trackingInformation.TimeStamp,
                        CreatedBy = i.Entity.ModifiedBy, //trackingInformation.UserName, (changed because of SFIPTV-422)
                        ModifiedBy = i.Entity.ModifiedBy, //trackingInformation.UserName, (changed because of SFIPTV-422)
                        LastOperationIdentifier = trackingInformation.OperationId,
                        LastOperationTimeStamp = trackingInformation.TimeStamp,
                        LastOperationType = i.State.ToString() 
                    };
                    repository.Add(serviceTrack);
                }
            });
        }
    }
}
