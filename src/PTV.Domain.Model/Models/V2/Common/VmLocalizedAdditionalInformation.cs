using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// Localized additional information
    /// </summary>
    public class VmLocalizedAdditionalInformation
    {
        /// <summary>
        /// Additional information content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Localization Id
        /// </summary>
        public Guid LocalizationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
