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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using Newtonsoft.Json;
using PTV.Framework;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of printable form channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiPrintableFormChannelInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiPrintableFormChannelInBase" />
    public class V2VmOpenApiPrintableFormChannelInBase : VmOpenApiPrintableFormChannelInVersionBase, IV2VmOpenApiPrintableFormChannelInBase
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
        public new V2VmOpenApiAddress DeliveryAddress { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 15)]
        public virtual new IList<VmOpenApiPhone> SupportPhones { get; set; } = new List<VmOpenApiPhone>();

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
            return 2;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiPrintableFormChannelInVersionBase>();
            vm.ServiceChannelDescriptions = ServiceChannelDescriptions.SetListValueLength();
            vm.FormIdentifier = FormIdentifier.SetStringValueLength(100).ConvertToLanguageList();
            vm.FormReceiver = FormReceiver.SetStringValueLength(100).ConvertToLanguageList();
            vm.ChannelUrls = ChannelUrls.SetListValueLength(500);
            SupportPhones.ForEach(p => vm.SupportPhones.Add(p.ConvertToVersion4()));
            vm.DeliveryAddress = DeliveryAddress.ConvertToVersion4();
            return vm;
        }
        #endregion
    }
}
