using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for list of 
    /// </summary>
    public class VmTranslationOrderStateOutputs : VmBase
    {
        /// <summary>
        /// Constructor of transaltion order state
        /// </summary>
        public VmTranslationOrderStateOutputs()
        {
            TranslationOrderStates = new List<VmTranslationOrderStateOutput>();
        }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The charge type id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        ///  Gets or sets the Translation order states
        /// </summary>
        /// <value>
        /// The list of translation order state.
        /// </value>
        public IReadOnlyList<VmTranslationOrderStateOutput> TranslationOrderStates { get; set; }

        /// <summary>
        /// Gets or sets the target languages ids.
        /// </summary>
        /// <value>
        /// The target languagues used in translation.
        /// </value>
        public List<Guid> TargetLanguagesInUse { get; set; }
    }
}