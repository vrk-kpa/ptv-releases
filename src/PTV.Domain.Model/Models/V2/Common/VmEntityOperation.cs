using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// VmEntityOperation
    /// </summary>
    public class VmEntityOperation
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Created { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof (StringEnumConverter))]
        public PublishingStatus PublishingStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int VersionMajor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int VersionMinor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof (StringEnumConverter))]
        public HistoryAction HistoryAction { get; set; }

    }
}
