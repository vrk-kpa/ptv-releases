using Newtonsoft.Json;
using System;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// Localized street name
    /// </summary>
    public class VmLocalizedStreetName
    {
        /// <summary>
        /// Name of the street
        /// </summary>
        public string Name { get; set; }
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
