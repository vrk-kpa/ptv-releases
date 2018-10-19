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
            DoTrackingForType<ServiceServiceChannel>(trackingInformation);
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
