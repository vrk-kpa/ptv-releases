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

using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of electronic channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiElectronicChannelInVersionBase" />
    public class V9VmOpenApiElectronicChannelInBase : VmOpenApiElectronicChannelInVersionBase
    {
        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "Summary" })]
        [ListPropertyMaxLength(150, "Value", "Summary")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get => base.ServiceChannelDescriptions; set => base.ServiceChannelDescriptions = value; }

        /// <summary>
        /// Area type. Possible values are: Nationwide, NationwideExceptAlandIslands or LimitedType.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ValidOpenApiEnum(typeof(AreaInformationTypeEnum))]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [JsonProperty(Order = 8)]
        [ListRequiredIf("AreaType", "LimitedType")]
        [ListWithOpenApiEnum(typeof(AreaTypeEnum), "Type")]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [ListWithOpenApiEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> SupportPhones { get => base.SupportPhones; set => base.SupportPhones = value; }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [ListWithOpenApiEnum(typeof(ServiceHoursTypeEnum), "ServiceHourType")]
        public override IList<V8VmOpenApiServiceHour> ServiceHours { get => base.ServiceHours; set => base.ServiceHours = value; }

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 9;
        }

        /// <summary>
        /// gets the base version
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            return base.GetVersionBaseModel(VersionNumber());
        }
        #endregion
    }
}
