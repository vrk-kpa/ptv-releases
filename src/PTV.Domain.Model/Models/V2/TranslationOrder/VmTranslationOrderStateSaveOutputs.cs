using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.V2.Service;
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
            Services = new List<VmServiceOutput>();
            Channels = new List<IVmChannel>();
            GeneralDescriptions = new List<VmGeneralDescriptionOutput>();
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
        public IReadOnlyList<VmServiceOutput> Services { get; set; }
        /// <summary>
        ///  Gets or sets the Translation vailability of Channels
        /// </summary>
        /// <value>
        /// The list of translation vailability of Channels.
        /// </value>
        public IReadOnlyList<IVmChannel> Channels { get; set; }
        /// <summary>
        ///  Gets or sets the Translation vailability of GeneralDescriptions.
        /// </summary>
        /// <value>
        /// The list of translation vailability of GeneralDescriptions.
        /// </value>
        public IReadOnlyList<VmGeneralDescriptionOutput> GeneralDescriptions { get; set; }
    }
}