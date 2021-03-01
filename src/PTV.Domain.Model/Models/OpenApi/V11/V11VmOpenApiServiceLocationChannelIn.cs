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

using Newtonsoft.Json;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V11
{
    /// <summary>
    /// OPEN API V11 - View Model of service location channel for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V11.V11VmOpenApiServiceLocationChannelInBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceLocationChannelIn" />
    public class V11VmOpenApiServiceLocationChannelIn : V11VmOpenApiServiceLocationChannelInBase, IVmOpenApiServiceLocationChannelIn
    {
        /// <summary>
        /// PTV organization identifier of organization responsible for this channel.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public override string OrganizationId { get => base.OrganizationId; set => base.OrganizationId = value; }

        /// <summary>
        /// List of localized service channel names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListWithPropertyValueRequired("Type", "Name")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelNames { get => base.ServiceChannelNames; set => base.ServiceChannelNames = value; }

        /// <summary>
        /// List of localized service channel descriptions. Possible type values are: Description, Summary.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListWithPropertyValueRequired("Type", "Summary")]
        [ListWithPropertyValueRequired("Type", "Description")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get => base.ServiceChannelDescriptions; set => base.ServiceChannelDescriptions = value; }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of ServiceChannelNames.
        /// Is only available for PUT operations, since user cannot add alternative anmes through in api (PTV-4340).
        /// </summary>
        [ListRequired]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [ListRequired]
        public override IList<string> Languages { get => base.Languages; set => base.Languages = value; }

        /// <summary>
        /// List of visiting addresses.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListWithPropertyValueRequired("Type", AddressConsts.LOCATION)]
        public override IList<V9VmOpenApiAddressLocationIn> Addresses { get => base.Addresses; set => base.Addresses = value; }

        /// <summary>
        /// Service channel publishing status. Values: Draft or Published.
        /// </summary>
        [Required]
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        /// <summary>
        /// List of related services (GUID).
        /// </summary>
        [JsonProperty(Order = 32)]
        [ListWithGuid]
        public virtual IList<string> Services { get; set; }

        /// <summary>
        /// Set to true to delete phone number. The prohone property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllPhoneNumbers { get => base.DeleteAllPhoneNumbers; set => base.DeleteAllPhoneNumbers = value; }

        /// <summary>
        /// Set to true to delete fax number. The fax property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllFaxNumbers { get => base.DeleteAllFaxNumbers; set => base.DeleteAllFaxNumbers = value; }

        /// <summary>
        /// Set to true to delete all existing service hours for the service channel. The ServiceHours collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllServiceHours { get => base.DeleteAllServiceHours; set => base.DeleteAllServiceHours = value; }

        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllWebPages { get => base.DeleteAllWebPages; set => base.DeleteAllWebPages = value; }

        /// <summary>
        /// Set to true to delete emails. The email property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportEmails { get => base.DeleteAllSupportEmails; set => base.DeleteAllSupportEmails = value; }

        /// <summary>
        /// Set to true to delete all existing support phones for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        /// <value>
        /// <c>true</c> if all support phones should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public override bool DeleteAllSupportPhones { get => base.DeleteAllSupportPhones; set => base.DeleteAllSupportPhones = value; }

        /// <summary>
        /// Set to true to delete OID. The Oid property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteOid { get => base.DeleteOid; set => base.DeleteOid = value; }

        #region methods

        /// <summary>
        /// gets the base version
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.VersionBase();
            // Set services - Services property availabale only in POST method!
            if (Services?.Count > 0)
            {
                Services.ForEach(s => vm.ServiceChannelServices.Add(s.ParseToGuid().Value));
            }
            return vm;
        }
        #endregion
    }
}
