using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for sending translation order state output
    /// </summary>
    public class VmTranslationOrderStateOutput
    {
        /// <summary>
        /// Constructor of transaltion order state
        /// </summary>
        public VmTranslationOrderStateOutput()
        {
           
        }

        /// <summary>
        /// Identifier of translation order state
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the state of tranlsation .
        /// </summary>
        /// <value>
        /// The state of translation.
        /// </value>
        public Guid TranslationStateTypeId { get; set; }

        /// <summary>
        /// Gets or sets the datetime of sent translation.
        /// </summary>
        /// <value>
        /// The datetime of sent translation.
        /// </value>
        public long SentAt { get; set; }

        /// <summary>
        /// Gets or sets the presumed datetime of translation.
        /// </summary>
        /// <value>
        /// The presumed datetime of translation.
        /// </value>
        public long? DeliverAt { get; set; }

        /// <summary>
        /// Gets or sets the translation order.
        /// </summary>
        /// <value>
        /// The translation order.
        /// </value>
        public VmTranslationOrderOutput TranslationOrder { get; set; }
    }
}