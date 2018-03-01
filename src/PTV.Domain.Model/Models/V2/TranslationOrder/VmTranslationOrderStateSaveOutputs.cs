using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for list of 
    /// </summary>
    public class VmTranslationOrderStateSaveOutputs : VmBase
    {
        /// <summary>
        /// Constructor of transaltion order state save output
        /// </summary>
        public VmTranslationOrderStateSaveOutputs()
        {
            Translations = new List<VmTranslationOrderStateOutputs>();
            Services = new List<VmTranslationOrderAvailabilitySaveOutputs>();
            Channels = new List<VmTranslationOrderAvailabilitySaveOutputs>();
            GeneralDescriptions = new List<VmTranslationOrderAvailabilitySaveOutputs>();
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
        public IReadOnlyList<VmTranslationOrderStateOutputs> Translations { get; set; }
        /// <summary>
        ///  Gets or sets the Translation availability of Services
        /// </summary>
        /// <value>
        /// The list of vailability of Services
        /// </value>
        public IReadOnlyList<VmTranslationOrderAvailabilitySaveOutputs> Services { get; set; }
        /// <summary>
        ///  Gets or sets the Translation vailability of Channels
        /// </summary>
        /// <value>
        /// The list of translation vailability of Channels.
        /// </value>
        public IReadOnlyList<VmTranslationOrderAvailabilitySaveOutputs> Channels { get; set; }
        /// <summary>
        ///  Gets or sets the Translation vailability of GeneralDescriptions.
        /// </summary>
        /// <value>
        /// The list of translation vailability of GeneralDescriptions.
        /// </value>
        public IReadOnlyList<VmTranslationOrderAvailabilitySaveOutputs> GeneralDescriptions { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class VmTranslationOrderAvailabilitySaveOutputs
    {
        /// <summary>
        /// Id of entity
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Setting of translation order availability
        /// </summary>
        public Dictionary<string, VmTranslationOrderAvailability> TranslationAvailability { get; set; }        
    }
}