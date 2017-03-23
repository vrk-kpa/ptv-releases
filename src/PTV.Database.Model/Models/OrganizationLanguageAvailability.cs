using System;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal partial class OrganizationLanguageAvailability : LanguageAvailability
    {
        public Guid OrganizationVersionedId { get; set; }

        public virtual OrganizationVersioned OrganizationVersioned { get; set; }
    }
}