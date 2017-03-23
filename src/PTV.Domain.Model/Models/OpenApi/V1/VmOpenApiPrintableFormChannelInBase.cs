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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View Model of printable form channel fo IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiPrintableFormChannelInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiPrintableFormChannelInBase" />
    public class VmOpenApiPrintableFormChannelInBase : VmOpenApiPrintableFormChannelInVersionBase, IVmOpenApiPrintableFormChannelInBase
    {
        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [ListPropertyMaxLength(4000, "Value", "Description")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions
        {
            get
            {
                return base.ServiceChannelDescriptions;
            }

            set
            {
                base.ServiceChannelDescriptions = value;
            }
        }

        /// <summary>
        /// Form identifier.
        /// </summary>
        [JsonProperty(Order = 10)]
        public new string FormIdentifier { get; set; }

        /// <summary>
        /// Form receiver.
        /// </summary>
        [JsonProperty(Order = 11)]
        public new string FormReceiver { get; set; }

        /// <summary>
        /// Form delivery address.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual new VmOpenApiAddress DeliveryAddress { get; set; }

        /// <summary>
        /// Description for delivery address. This property is not used in the API anymore. Do not use.
        /// </summary>
        [JsonProperty(Order = 14)]
        [ListPropertyMaxLength(150, "Value")]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public virtual IReadOnlyList<VmOpenApiLanguageItem> DeliveryAddressDescriptions { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// True to delete all existing delivery address descriptions. The DeliveryAddressDescriptions collection should be empty when this property is set to true.
        /// This property is not used in the API anymore. Do not use.
        /// </summary>
        [JsonProperty(Order = 18)]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public bool DeleteAllDeliveryAddressDescriptions { get; set; }

        /// <summary>
        /// List of support contacts for the service channel.
        /// </summary>
        [JsonProperty(Order = 35)]
        public IList<VmOpenApiSupport> SupportContacts { get; set; } = new List<VmOpenApiSupport>();

        /// <summary>
        /// Set to true to delete all existing support contacts for the service channel. The SupportContacts collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 45)]
        public virtual bool DeleteAllSupportContacts { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhone> SupportPhones
        {
            get
            {
                return base.SupportPhones;
            }

            set
            {
                base.SupportPhones = value;
            }
        }

        /// <summary>
        /// List of support email addresses for the service channel.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiLanguageItem> SupportEmails
        {
            get
            {
                return base.SupportEmails;
            }

            set
            {
                base.SupportEmails = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing support email addresses for the service channel. The SupportEmails collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportEmails
        {
            get
            {
                return base.DeleteAllSupportEmails;
            }

            set
            {
                base.DeleteAllSupportEmails = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing support phone numbers for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportPhones
        {
            get
            {
                return base.DeleteAllSupportPhones;
            }

            set
            {
                base.DeleteAllSupportPhones = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing form identifiers for the service channel. The form identifiers collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllFormIdentifiers
        {
            get
            {
                return base.DeleteAllFormIdentifiers;
            }

            set
            {
                base.DeleteAllFormIdentifiers = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing form receivers for the service channel. The form receivers collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllFormReceivers
        {
            get
            {
                return base.DeleteAllFormReceivers;
            }

            set
            {
                base.DeleteAllFormReceivers = value;
            }
        }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 1;
        }

        /// <summary>
        /// Gets the version base.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiPrintableFormChannelInVersionBase>();
            vm.ServiceChannelDescriptions = ServiceChannelDescriptions.SetListValueLength();
            vm.FormIdentifier = FormIdentifier.SetStringValueLength(100).ConvertToLanguageList();
            vm.FormReceiver = FormReceiver.SetStringValueLength(100).ConvertToLanguageList();
            vm.ChannelUrls = ChannelUrls.SetListValueLength(500);
            vm.DeliveryAddress = DeliveryAddress != null ? DeliveryAddress.ConvertToVersion2() : null;
            vm.SupportEmails = TranslateToV2SupportEmails(SupportContacts);
            vm.SupportPhones = TranslateToVersionBaseSupportPhones(SupportContacts);
            vm.DeleteAllSupportEmails = DeleteAllSupportContacts;
            vm.DeleteAllSupportPhones = DeleteAllSupportContacts;
            return vm;
        }
        #endregion
    }
}
