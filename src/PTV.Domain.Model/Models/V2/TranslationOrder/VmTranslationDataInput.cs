using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for translation data
    /// </summary>
    public class VmTranslationDataInput
    {
        /// <summary>
        /// Identifier of entity.
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Source Language of data
        /// </summary>
        public Guid SourceLanguage { get; set; }
    }
}