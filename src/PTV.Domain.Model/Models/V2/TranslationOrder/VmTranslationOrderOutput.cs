using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for sending translation order
    /// </summary>
    public class VmTranslationOrderOutput : VmTranslationOrderBase, IVmTranslationOrderInput
    {
        /// <summary>
        /// Gets or sets translation company name.
        /// </summary>
        /// <value>
        /// The translation company name.
        /// </value>
        public string TranslationCompanyName { get; set; }

        /// <summary>
        /// Gets or sets translation company email.
        /// </summary>
        /// <value>
        /// The translation company email.
        /// </value>
        public string TranslationCompanyEmail { get; set; }

        /// <summary>
        /// Gets or sets source language code.
        /// </summary>
        /// <value>
        /// The identfier of source language code.
        /// </value>
        public string SourceLanguageCode { get; set; }

        /// <summary>
        /// Gets or sets target language code.
        /// </summary>
        /// <value>
        /// The identfier of source language code.
        /// </value>
        public string TargetLanguageCode { get; set; }

        //Detail information
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        public Guid? EntityOrganizationId { get; set; }

    }
}