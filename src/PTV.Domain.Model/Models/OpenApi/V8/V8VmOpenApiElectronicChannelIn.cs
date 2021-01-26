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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of electronic channel for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V8.V8VmOpenApiElectronicChannelInBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V8.IV8VmOpenApiElectronicChannelIn" />
    public class V8VmOpenApiElectronicChannelIn : V8VmOpenApiElectronicChannelInBase, IV8VmOpenApiElectronicChannelIn
    {
        /// <summary>
        /// List of localized urls.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<VmOpenApiLanguageItem> WebPage { get => base.WebPage; set => base.WebPage = value; }

        /// <summary>
        /// PTV organization identifier of organization responsible for this channel.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public override string OrganizationId { get => base.OrganizationId; set => base.OrganizationId = value; }

        /// <summary>
        /// List of localized service channel names.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<VmOpenApiLanguageItem> ServiceChannelNames { get => base.ServiceChannelNames; set => base.ServiceChannelNames = value; }

        /// <summary>
        /// List of localized service channel descriptions. Possible type values are: Description, Summary.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListWithPropertyValueRequired("Type", "Summary")]
        [ListWithPropertyValueRequired("Type", "Description")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get => base.ServiceChannelDescriptions; set => base.ServiceChannelDescriptions = value; }

        /// <summary>
        /// Service channel publishing status. Values: Draft or Published.
        /// </summary>
        [Required]
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        /// <summary>
        /// List of related services (GUID).
        /// </summary>
        [JsonProperty(Order = 32)]
        [ListWithGuid]
        public virtual IList<string> Services { get; set; }

        // SFIPTV-1963
        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonIgnore]
        public override bool DeleteAllWebPagesOld { get => base.DeleteAllWebPagesOld; set => base.DeleteAllWebPagesOld = value; }

        /// <summary>
        /// Set to true to delete all existing support email addresses for the service channel. The SupportEmails collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportEmails { get => base.DeleteAllSupportEmails; set => base.DeleteAllSupportEmails = value; }

        /// <summary>
        /// Set to true to delete all existing support phone numbers for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportPhones { get => base.DeleteAllSupportPhones; set => base.DeleteAllSupportPhones = value; }

        /// <summary>
        /// Set to true to delete all existing service hours for the service channel. The ServiceHours collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllServiceHours { get => base.DeleteAllServiceHours; set => base.DeleteAllServiceHours = value; }

        /// <summary>
        /// Set to true to delete all existing attachments. The attachments collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllAttachments { get => base.DeleteAllAttachments; set => base.DeleteAllAttachments = value; }

        #region methods

        /// <summary>
        /// gets the base version
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.VersionBase();
            // Set services
            if (Services?.Count > 0)
            {
                Services.ForEach(s => vm.ServiceChannelServices.Add(s.ParseToGuid().Value));
            }
            return vm;
        }
        #endregion
    }
}
