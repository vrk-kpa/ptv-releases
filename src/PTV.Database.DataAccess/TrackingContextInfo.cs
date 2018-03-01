using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Interfaces.DbContext;

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// Holder class of parameters for Entity Tracking Manager
    /// </summary>
    public class TrackingContextInfo
    {
        public IUnitOfWorkWritable UnitOfWork { get; set; }
        public ChangeTracker ChangeTracker { get; set; }
        public string UserName { get; set; }
        public DateTime TimeStamp { get; set; }
        public string OperationId { get; set; }
        public IEnumerable<EntityState> EntityStates { get; set; }
    }
}