using PTV.Database.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.Model.Models
{
    internal class TrackingServiceCollectionService : IEntityIdentifier, IAuditing
    {
        public Guid Id { get; set; }

        public Guid ServiceCollectionId { get; set; }
        public Guid ServiceId { get; set; }

        public string OperationType { get; set; }

        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string LastOperationId { get; set; }
    }
}
