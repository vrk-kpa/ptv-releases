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

using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;
using PTV.Framework;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of service location channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelInVersionBase" />
    public class V6VmOpenApiServiceLocationChannelInBase : VmOpenApiServiceLocationChannelInVersionBase, IV6VmOpenApiServiceLocationChannelInBase
    {
        /// <summary>
        /// Localized list of service channel names.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }

        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "ShortDescription" })]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get => base.ServiceChannelDescriptions; set => base.ServiceChannelDescriptions = value; }

        /// <summary>
        /// Area type. Possible values are: WholeCountry, WholeCountryExceptAlandIslands or AreaType.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ValidEnum(typeof(AreaInformationTypeEnum))]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [JsonProperty(Order = 8)]
        [ListRequiredIf("AreaType", "AreaType")]
        [ListWithEnum(typeof(AreaTypeEnum), "Type")]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [ListWithEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> PhoneNumbers { get => base.PhoneNumbers; set => base.PhoneNumbers = value; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual new IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        [ListWithEnum(typeof(ServiceHoursTypeEnum), "ServiceHourType")]
        public virtual new IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();

        /// <summary>
        /// Service channel OID. Must match the regex @"^[A-Za-z0-9.-]*$".
        /// </summary>
        [JsonIgnore]
        public override string Oid { get => base.Oid; set => base.Oid = value; }

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 24)]
        public new virtual IList<V5VmOpenApiAddressWithTypeIn> Addresses { get; set; } = new List<V5VmOpenApiAddressWithTypeIn>();

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of ServiceChannelNames.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Set to true to delete OID. The Oid property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteOid { get => base.DeleteOid; set => base.DeleteOid = value; }

        /// <summary>
        /// Date when item should be published.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidFrom { get => base.ValidFrom; set => base.ValidFrom = value; }

        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidTo { get => base.ValidTo; set => base.ValidTo = value; }

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
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel(VersionNumber());

            // SFIPTV-236
            HandleNames(vm, ServiceChannelNames);
                        
            Addresses.ForEach(a => vm.Addresses.Add(a.LatestVersion<V9VmOpenApiAddressLocationIn>()));
            this.WebPages.ForEach(w => vm.WebPages.Add(w.ConvertToInBaseModel()));
            this.ServiceHours.ForEach(h => vm.ServiceHours.Add(h.ConvertToLatestVersion()));
            return vm;
        }
        #endregion
    }
}
