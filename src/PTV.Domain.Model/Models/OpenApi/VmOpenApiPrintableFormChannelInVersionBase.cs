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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of printable form channel for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPrintableFormChannelInVersionBase" />
    public class VmOpenApiPrintableFormChannelInVersionBase : VmOpenApiServiceChannelIn, IVmOpenApiPrintableFormChannelInVersionBase
    {
        // SFIPTV-236
        /// <summary>
        /// Localized list of service channel names.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }

        /// <summary>
        ///  List of localized form identifiers. One per language.
        /// </summary>
        [JsonProperty(Order = 10)]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public IList<VmOpenApiLanguageItem> FormIdentifier { get; set; }
        
        /// <summary>
        /// Gets or sets the delivery addresses.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual IList<V8VmOpenApiAddressDeliveryIn> DeliveryAddresses { get; set; }

        /// <summary>
        /// List of localized channel urls.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListWithEnum(typeof(PrintableFormChannelUrlTypeEnum), "Type")]
        [ListWithUrl("Value")]
        [ListPropertyMaxLength(500, "Value")]
        public virtual IList<VmOpenApiLocalizedListItem> ChannelUrls { get; set; }

        /// <summary>
        /// List of attachments.
        /// </summary>
        [JsonProperty(Order = 15)]
        public IReadOnlyList<VmOpenApiAttachment> Attachments { get; set; } = new List<VmOpenApiAttachment>();

        /// <summary>
        /// Set to true to delete all existing delivery addresses for the service channel. The DeliveryAddresses should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 16)]
        public virtual bool DeleteAllDeliveryAddresses { get; set; }

        /// <summary>
        /// Set to true to delete all existing channel urls for the service channel. The ChannelUrls collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 17)]
        public virtual bool DeleteAllChannelUrls { get; set; }

        /// <summary>
        /// Set to true to delete all existing attachments for the service channel. The Attachments collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 19)]
        public virtual bool DeleteAllAttachments { get; set; }

        /// <summary>
        /// Set to true to delete all existing form identifiers for the service channel. The form identifiers collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 19)]
        public virtual bool DeleteAllFormIdentifiers { get; set; }

        /// <summary>
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [JsonIgnore]
        public override IList<string> Languages { get; set; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonIgnore]
        public override IList<V9VmOpenApiWebPage> WebPages { get; set; }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonIgnore]
        public override IList<V8VmOpenApiServiceHour> ServiceHours { get => base.ServiceHours; set => base.ServiceHours = value; }


        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllWebPages { get; set; }

        /// <summary>
        /// Set to true to delete all existing service hours for the service channel. The ServiceHours collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllServiceHours { get; set; }

        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            return base.VersionBase();
        }

        /// <summary>
        /// Gets the version base model.
        /// </summary>
        /// <returns>base version model</returns>
        protected VmOpenApiPrintableFormChannelInVersionBase GetVersionBaseModel(int version)
        {
            var vm = GetServiceChannelModel<VmOpenApiPrintableFormChannelInVersionBase>(version);
            if (ServiceChannelNames?.Count > 0)
            {
                if (vm.ServiceChannelNamesWithType == null) { vm.ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem>(); }
                ServiceChannelNames.ForEach(name => vm.ServiceChannelNamesWithType.Add(
                    new VmOpenApiLocalizedListItem { Value = name.Value, Language = name.Language, Type = NameTypeEnum.Name.ToString() }
                ));
            }
            vm.FormIdentifier = FormIdentifier;
            vm.DeliveryAddresses = DeliveryAddresses;
            vm.ChannelUrls = ChannelUrls;
            vm.Attachments = Attachments;
            vm.DeleteAllDeliveryAddresses = DeleteAllDeliveryAddresses;
            vm.DeleteAllChannelUrls = DeleteAllChannelUrls;
            vm.DeleteAllAttachments = DeleteAllAttachments;
            vm.DeleteAllFormIdentifiers = DeleteAllFormIdentifiers;
            return vm;
        }
        #endregion
    }
}
