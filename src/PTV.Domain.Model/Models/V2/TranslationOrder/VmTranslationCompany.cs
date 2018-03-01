using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.V2.TranslationOrder
{
    /// <summary>
    ///  View model for sending translation order state
    /// </summary>
    public class VmTranslationCompany : IVmBase
    {
        /// <summary>
        /// Constructor of translation company
        /// </summary>
        public VmTranslationCompany()
        {
           
        }

        /// <summary>
        /// Identifier of translation company
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the email of translation company.
        /// </summary>
        /// <value>
        /// The email of translation company.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the url of translation company.
        /// </summary>
        /// <value>
        /// The url of translation company.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the name of translation company.
        /// </summary>
        /// <value>
        /// The name of translation company.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of translation company.
        /// </summary>
        /// <value>
        /// The description of translation company.
        /// </value>
        public string Description { get; set; }


    }
}