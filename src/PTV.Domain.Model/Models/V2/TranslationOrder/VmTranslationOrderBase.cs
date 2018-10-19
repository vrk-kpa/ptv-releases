using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for sending translation order state
    /// </summary>
    public class VmTranslationOrderBase
    {
        /// <summary>
        /// Constructor of transaltion order state
        /// </summary>
        public VmTranslationOrderBase()
        {
           
        }

        /// <summary>
        /// Identifier of translation
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// Identifier of translation
        /// </summary>
        public Guid? PreviousTranslationOrderId { get; set; }

        /// <summary>
        /// Identifier of contentId
        /// </summary>
        public Guid? ContentId { get; set; }
        
        /// <summary>
        /// Identifier of entityId that should be translate
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Unific root id
        /// </summary>
        public Guid EntityRootId { get; set; }

        /// <summary>
        /// Gets or sets source language.
        /// </summary>
        /// <value>
        /// The identfier of source language.
        /// </value>
        public Guid SourceLanguage { get; set; }

        /// <summary>
        /// Gets or sets target language.
        /// </summary>
        /// <value>
        /// The identfier of target language.
        /// </value>
        public Guid TargetLanguage { get; set; }

        /// <summary>
        /// Gets or sets order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public long OrderIdentifier { get; set; }

        /// <summary>
        /// Gets or sets sender name.
        /// </summary>
        /// <value>
        /// The identfier of sender name.
        /// </value>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the sender email.
        /// </summary>
        /// <value>
        /// The sender email.
        /// </value>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets the addiitonal information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        public string AdditionalInformation { get; set; }

    }
}