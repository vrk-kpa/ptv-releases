using System;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal partial class ServiceChannelLanguageAvailability : LanguageAvailability
    {
        public Guid ServiceChannelVersionedId { get; set; }

        public virtual ServiceChannelVersioned ServiceChannelVersioned { get; set; }
    }
}