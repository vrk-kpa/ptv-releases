using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for translation order state input
    /// </summary>
    public class VmTranslationOrderStateInput : IVmBase
    {
        /// <summary>
        /// Constructor of translation company
        /// </summary>
        public VmTranslationOrderStateInput()
        {
           
        }

        /// <summary>
        /// Gets or sets identfier of order state.
        /// </summary>
        /// <value>
        /// The identfier of Order state.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets  translation order id.
        /// </summary>
        /// <value>
        /// The translation order id.
        /// </value>
        public Guid TranslationOrderId { get; set; }

        /// <summary>
        /// Gets or sets translation state id.
        /// </summary>
        /// <value>
        /// The translation state id.
        /// </value>
        public Guid TranslationStateId { get; set; }
        
        /// <summary>
        /// Gets or sets checked value.
        /// </summary>
        /// <value>
        /// The checked value.
        /// </value>
        public bool Checked { get; set; }


        /// <summary>
        /// Gets or sets last value.
        /// </summary>
        /// <value>
        /// The last value.
        /// </value>
        public bool Last { get; set; }
        
        
        /// <summary>
        /// Additional information related to state
        /// </summary>
        public string InfoDetails { get; set; }

    }
}