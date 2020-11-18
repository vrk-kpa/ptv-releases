/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
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
using PTV.Framework.Converters;

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
        [JsonConverter(typeof(NotNullGuidConverter))]
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
        /// Temporary id for new entities (not saved to database)
        /// </summary>
        public Guid? AlternativeId { get; set; }

        /// <summary>
        /// Date when entity expires
        /// </summary>
        public long ExpireOn { get; set; }
        /// <summary>
        /// Indicates, whether warning about expiration should be shown
        /// </summary>
        public bool IsExpireWarningVisible { get; set; }
        
        /// <summary>
        /// Indicates, whether warning about not updated content should be shown
        /// </summary>
        public bool IsNotUpdatedWarningVisible { get; set; }

        /// <summary>
        /// Setting of translation order availability
        /// </summary>
        public Dictionary<string, VmTranslationOrderAvailability> TranslationAvailability { get; set; }

        /// <summary>
        /// SOTE id / oid
        /// </summary>
        public string Oid { get; set; }
        /// <summary>
        /// Missing language translations of organization
        /// </summary>
        public IEnumerable<Guid> MissingLanguages { get; set; }
    }
}
