using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// Interface for implementation of entity tracker which is responsible for processing entities during saving operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityTracker<T> : IEntityTracker where T : class
    {
        void Perform(TrackingContextInfo trackingInformation, IEnumerable<EntityEntry<T>> entities);
    }

    public interface IEntityTracker { }
}