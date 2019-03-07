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
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
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
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()); 
            entities
                .GroupBy(entity => entity.Entity.UnificRootId)
                .Select(entity => entity.First())
                .Where(entity => entity.Entity.PublishingStatusId != draftStatusId)                            
                .ToList()
                .ForEach(i =>
                {
                    var versionMetaData = i.Entity.Versioning?.Meta != null
                        ? JsonConvert.DeserializeObject<VmHistoryMetaData>(i.Entity.Versioning.Meta) : null;
                    var isWithdrawItem = versionMetaData != null &&
                                        versionMetaData.HistoryAction == HistoryAction.Withdraw;
                    var firstVersion = i.Entity.Versioning?.VersionMajor == 1;
                    
                    // remove deleted and withdraw items
                    repository.All().Where(x =>
                            x.GenerealDescriptionId == i.Entity.UnificRootId &&
                            (i.Entity.PublishingStatusId == deletedStatusId || isWithdrawItem))
                        .ForEach(item => repository.Remove(item));

                    
                    // process added/amended tab
                    if (i.Entity.PublishingStatusId == publishedStatusId && i.State == EntityState.Modified)
                    {
                        // remove previous published version in Amended GD section
                        repository.All()
                            .Where(x => x.GenerealDescriptionId == i.Entity.UnificRootId && x.OperationType == EntityState.Modified.ToString())
                            .ForEach(r=>repository.Remove(r));
                        var gdAddTrack = new TrackingGeneralDescriptionVersioned()
                        {
                            Id = Guid.NewGuid(),
                            GenerealDescriptionId = i.Entity.UnificRootId,
                            OperationType = firstVersion ? EntityState.Added.ToString() : EntityState.Modified.ToString(),
                            Modified = trackingInformation.TimeStamp,
                            Created = trackingInformation.TimeStamp,
                            CreatedBy = trackingInformation.UserName,
                            ModifiedBy = trackingInformation.UserName,
                            LastOperationId = trackingInformation.OperationId
                        };
                        repository.Add(gdAddTrack);
                    }                                                         
                });
        }
    }
}
