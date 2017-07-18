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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of printable form channel for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPrintableFormChannelInVersionBase" />
    public class VmOpenApiPrintableFormChannelInVersionBase : VmOpenApiServiceChannelIn, IVmOpenApiPrintableFormChannelInVersionBase
    {
        /// <summary>
        ///  List of localized form identifiers. One per language.
        /// </summary>
        [JsonProperty(Order = 10)]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public IList<VmOpenApiLanguageItem> FormIdentifier { get; set; }

        /// <summary>
        /// List of localized form receivers. One per language.
        /// </summary>
        [JsonProperty(Order = 11)]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public IList<VmOpenApiLanguageItem> FormReceiver { get; set; }

        /// <summary>
        /// Form delivery address.
        /// </summary>
        [JsonProperty(Order = 12)]
        //public virtual V4VmOpenApiAddressIn DeliveryAddress { get; set; }
        public virtual V5VmOpenApiAddressIn DeliveryAddress { get; set; }

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
        /// Set to true to delete all existing delivery address for the service channel. The DeliveryAddress should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 16)]
        public virtual bool DeleteDeliveryAddress { get; set; }

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
        /// Set to true to delete all existing form receivers for the service channel. The form receivers collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 19)]
        public virtual bool DeleteAllFormReceivers { get; set; }

        /// <summary>
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [JsonIgnore]
        public override IList<string> Languages
        {
            get
            {
                return base.Languages;
            }

            set
            {
                base.Languages = value;
            }
        }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiWebPageWithOrderNumber> WebPages
        {
            get
            {
                return base.WebPages;
            }

            set
            {
                base.WebPages = value;
            }
        }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiServiceHour> ServiceHours
        {
            get
            {
                return base.ServiceHours;
            }

            set
            {
                base.ServiceHours = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllWebPages
        {
            get
            {
                return base.DeleteAllWebPages;
            }

            set
            {
                base.DeleteAllWebPages = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing service hours for the service channel. The ServiceHours collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllServiceHours
        {
            get
            {
                return base.DeleteAllServiceHours;
            }

            set
            {
                base.DeleteAllServiceHours = value;
            }
        }

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
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiPrintableFormChannelInVersionBase, new()
        {
            var vm = GetServiceChannelModel<TModel>();
            vm.FormIdentifier = FormIdentifier;
            vm.FormReceiver = FormReceiver;
            vm.DeliveryAddress = DeliveryAddress;
            vm.ChannelUrls = ChannelUrls;
            vm.Attachments = Attachments;
            vm.DeleteDeliveryAddress = DeleteDeliveryAddress;
            vm.DeleteAllChannelUrls = DeleteAllChannelUrls;
            vm.DeleteAllAttachments = DeleteAllAttachments;
            vm.DeleteAllFormIdentifiers = DeleteAllFormIdentifiers;
            vm.DeleteAllFormReceivers = DeleteAllFormReceivers;
            return vm;
        }
        #endregion
    }
}
