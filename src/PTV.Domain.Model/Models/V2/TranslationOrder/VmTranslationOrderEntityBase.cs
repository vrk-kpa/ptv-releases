using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    /// Model for setting data of translation orders
    /// </summary>
    public class VmTranslationOrderEntityBase : ILanguagesAvailabilities
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// Source name - default entity name for non existing availabilities
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Keep in prevoius state
        /// </summary>
        public bool KeepInPreviousState { get; set; }
    }
}
