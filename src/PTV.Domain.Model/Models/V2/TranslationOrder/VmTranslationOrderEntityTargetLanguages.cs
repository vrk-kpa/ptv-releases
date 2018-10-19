using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    /// Translation entity target languages
    /// </summary>
    public class VmTranslationOrderEntityTargetLanguages
    {
        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public Guid EntityRootId { get; set; }


        /// <summary>
        /// Target languages for translation
        /// </summary>
        public List<Guid> TargetLanguages { get; set; }
    }
}