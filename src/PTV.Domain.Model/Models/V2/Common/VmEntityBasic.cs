using System;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Converters;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// View model of get entity basic (base)
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityGet" />
    public class VmEntityBasic : VmEntityBase, IVmEntityGet
    {

        /// <summary>
        /// Gets or sets the language id.
        /// </summary>
        /// <value>
        /// The language id.
        /// </value>
        public Guid? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets flag which add publishing validation while getting entity
        /// </summary>
        public bool IncludeValidation { get; set; }
    }
}