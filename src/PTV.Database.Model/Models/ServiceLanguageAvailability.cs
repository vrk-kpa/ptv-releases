using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal partial class ServiceLanguageAvailability : LanguageAvailability
    {
        public Guid ServiceVersionedId { get; set; }

        public virtual ServiceVersioned ServiceVersioned { get; set; }
    }
}
