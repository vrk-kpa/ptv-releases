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
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V10
{
    /// <summary>
    /// OPEN API V10 - View Model of printable from channel for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V10.V10VmOpenApiPrintableFormChannelInBase" />
    public class V10VmOpenApiPrintableFormChannelIn : V10VmOpenApiPrintableFormChannelInBase
    {

        /// <summary>
        /// List of localized urls. Possible type values are: PDF, DOC, Excel.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<VmOpenApiLocalizedListItem> ChannelUrls { get => base.ChannelUrls; set => base.ChannelUrls = value; }

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
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [JsonProperty(Order = 17)]
        [ListRequired]
        public override IList<string> Languages { get => base.Languages; set => base.Languages = value; }

        /// <summary>
        /// Service channel publishing status. Values: Draft or Published.
        /// </summary>
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        /// <summary>
        /// List of related services (GUID).
        /// </summary>
        [JsonProperty(Order = 32)]
        [ListWithGuid]
        public virtual IList<string> Services { get; set; }

        /// <summary>
        /// Set to true to delete all existing delivery addresses for the service channel. The DeliveryAddresses should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllDeliveryAddresses { get => base.DeleteAllDeliveryAddresses; set => base.DeleteAllDeliveryAddresses = value; }

        /// <summary>
        /// Set to true to delete all existing channel urls for the service channel. The ChannelUrls collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllChannelUrls { get => base.DeleteAllChannelUrls; set => base.DeleteAllChannelUrls = value; }

        /// <summary>
        /// Set to true to delete all existing attachments for the service channel. The Attachments collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllAttachments { get => base.DeleteAllAttachments; set => base.DeleteAllAttachments = value; }

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
        /// Set to true to delete all existing form identifiers for the service channel. The form identifiers collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllFormIdentifiers { get => base.DeleteAllFormIdentifiers; set => base.DeleteAllFormIdentifiers = value; }

        #region methods
        /// <summary>
        /// gets the base version
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.VersionBase();
            // Set services - only for POST
            if (Services?.Count > 0)
            {
                Services.ForEach(s => vm.ServiceChannelServices.Add(s.ParseToGuid().Value));
            }
            return vm;
        }
        #endregion
    }
}
