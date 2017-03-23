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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of electronic channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiElectronicChannelInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiElectronicChannelInBase" />
    public class V2VmOpenApiElectronicChannelInBase : VmOpenApiElectronicChannelInVersionBase, IV2VmOpenApiElectronicChannelInBase
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
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListWithUrl("Value")]
        public virtual new IList<VmOpenApiLanguageItem> Urls { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 15)]
        public virtual new IList<VmOpenApiPhone> SupportPhones { get; set; } = new List<VmOpenApiPhone>();

        /// <summary>
        /// List of support email addresses for the service channel.
        /// </summary>
        [JsonProperty(Order = 16)]
        [EmailAddressList("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IList<VmOpenApiLanguageItem> SupportEmails { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IList<V2VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V2VmOpenApiServiceHour>();

        #region methods
        /// <summary>
        /// Gets the version the number.
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
            var vm = base.GetVersionBaseModel<VmOpenApiElectronicChannelInVersionBase>();
            vm.ServiceChannelDescriptions = ServiceChannelDescriptions.SetListValueLength();
            vm.SupportEmails = SupportEmails.SetListValueLength(100);
            vm.Urls = Urls.SetListValueLength(500);
            SupportPhones.ForEach(p => vm.SupportPhones.Add(p.ConvertToVersion4()));
            ServiceHours.ForEach(h => vm.ServiceHours.Add(h.ConvertToVersion4()));
            return vm;
        }
        #endregion
    }
}
