using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    ///  View model for requesting publishing of entity
    /// </summary>
    public class VmPublishingModel : IVmPublishingModel
    {
        /// <summary>
        /// List of languages and their statuses of published entity
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Identifier of entity that should be published
        /// </summary>
        public Guid Id { get; set; }
    }
}