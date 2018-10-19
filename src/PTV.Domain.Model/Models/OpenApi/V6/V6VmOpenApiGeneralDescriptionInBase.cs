/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of general description for IN api base (PUT)
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionInVersionBase" />
    public class V6VmOpenApiGeneralDescriptionInBase : VmOpenApiGeneralDescriptionInVersionBase
    {
        /// <summary>
        /// List of localized names.
        /// </summary>
        [JsonProperty(Order = 2)]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiLocalizedListItem> Names { get => base.Names; set => base.Names = value; }

        /// <summary>
        /// List of localized descriptions.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ListWithEnum(typeof(DescriptionTypeEnum), "Type")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(2500, "Value", "ServiceUserInstruction")]
        [ListPropertyMaxLength(2500, "Value", "BackgroundDescription")]
        public override IList<VmOpenApiLocalizedListItem> Descriptions { get => base.Descriptions; set => base.Descriptions = value; }

        /// <summary>
        /// Service type. Possible values are: Service, PermissionAndObligation or ProfessionalQualifications.
        /// </summary>        
        [ValidEnum(typeof(ServiceTypeEnum))]
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// Service charge type. Possible values are:  Chargeable or FreeOfCharge.
        /// In version 7 and older: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 12)]
        [AllowedValues("ServiceChargeType", new[] { "Charged", "Free" })]
        public override string ServiceChargeType { get => base.ServiceChargeType; set => base.ServiceChargeType = value; }

        /// <summary>
        /// General description type is not allowed for older versions than 8
        /// </summary>        
        [JsonIgnore]
        public override string GeneralDescriptionType { get => base.GeneralDescriptionType; set => base.GeneralDescriptionType = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 6;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiGeneralDescriptionInVersionBase VersionBase()
        {
            var model = base.GetInVersionBaseModel<VmOpenApiGeneralDescriptionInVersionBase>(VersionNumber());
            return model;
        }
        #endregion
    }
}
