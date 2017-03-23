using System;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal partial class GeneralDescriptionLanguageAvailability : LanguageAvailability
    {
        public Guid StatutoryServiceGeneralDescriptionVersionedId { get; set; }

        public virtual StatutoryServiceGeneralDescriptionVersioned StatutoryServiceGeneralDescriptionVersioned { get; set; }
    }
}