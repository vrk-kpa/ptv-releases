using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Security;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// Base class for header
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Framework.Interfaces.IVmBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.V2.IName" />
    /// <seealso cref="IVmPublishingModel" />
    public class VmEntityHeaderBase : VmEntityBase, IName, IVmPublishingModel
    {
        /// <summary>
        /// Info about previous entitites
        /// </summary>
        public IVmEntityBase PreviousInfo { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }

        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public VmVersion Version { get; set; }

        /// <summary>
        /// Entity root identifier.
        /// </summary>
        public Guid UnificRootId { get; set; }

        /// <summary>
        /// Publishing status.
        /// </summary>
        public Guid PublishingStatus { get; set; }

        Guid IVmPublishingModel.Id { get => Id ?? Guid.Empty; set => Id = value; }

        /// <summary>
        /// Action operation.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionTypeEnum Action { get; set; }
    }
}