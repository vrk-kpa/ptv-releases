using System;
using System.Collections.Generic;
using System.Text;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.Model.Models
{
    internal class TrackingEntityVersioned : IEntityIdentifier, IAuditing
    {
        public Guid Id { get; set; }

        public Guid UnificRootId { get; set; }
        public Guid OrganizationId { get; set; }
        public string EntityType { get; set; }      
        public string OperationType { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string LastOperationId { get; set; }
    }
}
