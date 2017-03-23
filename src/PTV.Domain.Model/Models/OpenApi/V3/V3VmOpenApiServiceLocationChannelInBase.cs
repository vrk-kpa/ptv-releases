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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of service location channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiServiceLocationChannelInBase" />
    public class V3VmOpenApiServiceLocationChannelInBase : VmOpenApiServiceLocationChannelInVersionBase, IV3VmOpenApiServiceLocationChannelInBase
    {
        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [ListPropertyMaxLength(2500, "Value", "Description")]
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
        /// List email addresses for the service channel.
        /// </summary>
        [JsonProperty(PropertyName = "emails", Order = 12)]
        [EmailAddressList("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IList<VmOpenApiLanguageItem> SupportEmails { get; set; }

        /// <summary>
        /// Service location contact fax numbers.
        /// </summary>
        [JsonProperty(Order = 13)]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IList<VmOpenApiPhoneSimple> FaxNumbers { get; set; } = new List<VmOpenApiPhoneSimple>();

        /// <summary>
        /// List of phone numbers for the service channel. Includes also fax numbers.
        /// </summary>
        [JsonProperty(Order = 14)]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IList<VmOpenApiPhone> PhoneNumbers { get; set; } = new List<VmOpenApiPhone>();

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public new IList<V3VmOpenApiWebPage> WebPages { get; set; } = new List<V3VmOpenApiWebPage>();

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 24)]
        public virtual new IList<V2VmOpenApiAddressWithType> Addresses { get; set; }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IList<V2VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V2VmOpenApiServiceHour>();

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 3;
        }

        /// <summary>
        /// Get base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiServiceLocationChannelInVersionBase>();
            FaxNumbers.ForEach(f => vm.FaxNumbers.Add(f.ConvertToVersion4()));
            PhoneNumbers.ForEach(p => vm.PhoneNumbers.Add(p.ConvertToVersion4()));
            WebPages.ForEach(w => vm.WebPages.Add(w.ConvertToWebPageWithOrderNumber()));
            ServiceHours.ForEach(h => vm.ServiceHours.Add(h.ConvertToVersion4()));
            Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion4()));
            vm.SupportEmails = SupportEmails.SetListValueLength(100);
            return vm;
        }
        #endregion
    }
}
