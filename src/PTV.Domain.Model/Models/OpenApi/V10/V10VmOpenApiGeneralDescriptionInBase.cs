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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes.ValidationAttributes;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V10
{
    /// <summary>
    /// OPEN API V10 - View Model of general description for IN api base (PUT)
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionInVersionBase" />
    public class V10VmOpenApiGeneralDescriptionInBase : VmOpenApiGeneralDescriptionInVersionBase
    {
        /// <summary>
        /// List of localized names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [JsonProperty(Order = 2)]
        [ListWithOpenApiEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiLocalizedListItem> Names { get => base.Names; set => base.Names = value; }

        /// <summary>
        /// List of localized descriptions. Possible type values are: Description, Summary, BackgroundDescription, UserInstruction, GeneralDescriptionTypeAdditionalInformation, ChargeTypeAdditionalInfo, DeadLine, ProcessingTime, ValidityTime.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ListWithOpenApiEnum(typeof(DescriptionTypeEnum), "Type")]
        [ListPropertyMaxLength(150, "Value", "Summary")]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(2500, "Value", "UserInstruction")]
        [ListPropertyMaxLength(2500, "Value", "BackgroundDescription")]
        [ListPropertyMaxLength(2500, "Value", "GeneralDescriptionTypeAdditionalInformation")]
        public override IList<VmOpenApiLocalizedListItem> Descriptions { get => base.Descriptions; set => base.Descriptions = value; }

        /// <summary>
        /// Service type. Possible values are: Service, PermitOrObligation or ProfessionalQualification.
        /// </summary>
        [ValidOpenApiEnum(typeof(ServiceTypeEnum))]
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// Service charge type. Possible values are:  Chargeable or FreeOfCharge.
        /// </summary>
        [JsonProperty(Order = 12)]
        [AllowedValues("ServiceChargeType", new[] { "Chargeable", "FreeOfCharge" })]
        public override string ServiceChargeType { get => base.ServiceChargeType; set => base.ServiceChargeType = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 10;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiGeneralDescriptionInVersionBase VersionBase()
        {
            return base.GetInVersionBaseModel<VmOpenApiGeneralDescriptionInVersionBase>(VersionNumber());
        }
        #endregion
    }
}
