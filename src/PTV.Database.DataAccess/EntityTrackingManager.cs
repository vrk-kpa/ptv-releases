using System.Linq;
using System.Text;
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
        }


        private void DoTrackingForType<T>(TrackingContextInfo trackingInformation) where T : class
        {
            var entityTracker = resolveManager.Resolve<IEntityTracker<T>>();
            var entities = trackingInformation.ChangeTracker.Entries<T>();
            if (!trackingInformation.EntityStates.IsNullOrEmpty())
            {
                entities = entities.Where(i => trackingInformation.EntityStates.Contains(i.State));
            }
            entityTracker.Perform(trackingInformation, entities.ToList());
        }
    }
}
