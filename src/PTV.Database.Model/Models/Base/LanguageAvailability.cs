using System;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.Model.Models.Base
{
    internal abstract class LanguageAvailability : EntityBase, ILanguageAvailability
    {
        public Guid LanguageId { get; set; }

        public virtual Language Language { get; set; }

        public Guid StatusId { get; set; }

        public virtual PublishingStatusType Status { get; set; }
    }
}