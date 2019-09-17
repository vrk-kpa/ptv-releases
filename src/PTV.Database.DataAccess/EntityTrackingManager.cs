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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// EntityTrackingManager is handling all known entites in entity framework context and perform logging and tracking operation on them
    /// </summary>
    [RegisterService(typeof(IEntityTrackingManager), RegisterType.Scope)]
    internal class EntityTrackingManager : IEntityTrackingManager
    {
        private readonly IResolveManager resolveManager;

        public EntityTrackingManager(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }


        public void ProcessKnownEntities(TrackingContextInfo trackingInformation)
        {
            // Register trackers for entity types
            DoTrackingForType<ServiceServiceChannel>(trackingInformation,  new List<EntityState> {EntityState.Modified, EntityState.Added, EntityState.Deleted} );
            DoTrackingForType<ServiceCollectionService>(trackingInformation);
            DoTrackingForType<GeneralDescriptionServiceChannel>(trackingInformation);
            var changeStates = new List<EntityState> {EntityState.Modified, EntityState.Added};
            DoTrackingForType<ServiceVersioned>(trackingInformation, changeStates);
            DoTrackingForType<ServiceChannelVersioned>(trackingInformation, changeStates);
            DoTrackingForType<StatutoryServiceGeneralDescriptionVersioned>(trackingInformation, changeStates);
        }


        private void DoTrackingForType<T>(TrackingContextInfo trackingInformation, IEnumerable<EntityState> entityStates = null) where T : class
        {
            var entityTracker = resolveManager.Resolve<IEntityTracker<T>>();
            var entities = trackingInformation.ChangeTracker.Entries<T>();
            
            if (!trackingInformation.EntityStates.IsNullOrEmpty())
            {
                var states = entityStates ?? trackingInformation.EntityStates; 
                entities = entities.Where(i => states.Contains(i.State));
            }
            entityTracker.Perform(trackingInformation, entities.ToList());
        }
    }
}
