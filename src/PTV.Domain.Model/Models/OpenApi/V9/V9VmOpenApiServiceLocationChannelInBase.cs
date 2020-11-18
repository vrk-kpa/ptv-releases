﻿/**
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes.ValidationAttributes;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of service location channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelInVersionBase" />
    public class V9VmOpenApiServiceLocationChannelInBase : VmOpenApiServiceLocationChannelInVersionBase
    {
        /// <summary>
        /// List of organization names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListRequiredToMatchDisplayNameTypes("DisplayNameType")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelNames { get => base.ServiceChannelNames; set => base.ServiceChannelNames = value; }

        /// <summary>
        /// List of localized service channel descriptions. Possible type values are: Description, Summary.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "Summary" })]
        [ListPropertyMaxLength(150, "Value", "Summary")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get => base.ServiceChannelDescriptions; set => base.ServiceChannelDescriptions = value; }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of ServiceChannelNames.
        /// </summary>
        [JsonProperty(Order = 6)]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Area type. Possible values are: Nationwide, NationwideExceptAlandIslands or LimitedType.
        /// </summary>
        [JsonProperty(Order = 7)]
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
        public override IList<V4VmOpenApiPhone> PhoneNumbers { get => base.PhoneNumbers; set => base.PhoneNumbers = value; }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        [ListWithOpenApiEnum(typeof(ServiceHoursTypeEnum), "ServiceHourType")]
        public virtual new IList<V8VmOpenApiServiceHour> ServiceHours { get; set; }

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [Required]
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
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel(VersionNumber());
            // SFIPTV-1912
            this.ServiceHours?.ForEach(h => vm.ServiceHours.Add(h.ConvertToInVersionBase()));
            return vm;
        }
        #endregion
    }
}
