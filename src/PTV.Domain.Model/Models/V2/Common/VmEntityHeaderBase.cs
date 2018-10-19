using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// Base class for header
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Framework.Interfaces.IVmBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.V2.IName" />
    /// <seealso cref="IVmLocalizedEntityModel" />
    public class VmEntityHeaderBase : VmEntityBase, IName, IVmLocalizedEntityModel
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
        
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PublishActionTypeEnum? PublishAction { get; set; }

        Guid IVmLocalizedEntityModel.Id { get => Id ?? Guid.Empty; set => Id = value; }

        /// <summary>
        /// Action operation.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionTypeEnum Action { get; set; }
        
        /// <summary>
        /// Date when entity expires
        /// </summary>
        public long ExpireOn { get; set; }
        /// <summary>
        /// Indicates, whether warning about expiration should be shown
        /// </summary>
        public bool IsWarningVisible { get; set; }
        
        /// <summary>
        /// Setting of translation order availability
        /// </summary>
        public Dictionary<string, VmTranslationOrderAvailability> TranslationAvailability { get; set; }
        
        /// <summary>
        /// SOTE id / oid
        /// </summary>
        public string Oid { get; set; }
    }
}