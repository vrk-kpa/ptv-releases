using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class VmLocalizedForeignAddress
    {
        /// <summary>
        /// Name of the street
        /// </summary>
        public string Text { get; set; }
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
