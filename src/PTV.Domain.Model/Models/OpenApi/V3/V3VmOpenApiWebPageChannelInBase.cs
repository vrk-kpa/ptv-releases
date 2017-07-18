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
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of web page channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiWebPageChannelInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiWebPageChannelInBase" />
    public class V3VmOpenApiWebPageChannelInBase : VmOpenApiWebPageChannelInVersionBase, IV3VmOpenApiWebPageChannelInBase
    {
        /// <summary>
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListWithUrl("Value")]
        public virtual new IList<VmOpenApiLanguageItem> Urls { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 15)]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IList<VmOpenApiPhone> SupportPhones { get; set; } = new List<VmOpenApiPhone>();

        /// <summary>
        /// List of support email addresses for the service channel.
        /// </summary>
        [JsonProperty(Order = 16)]
        [EmailAddressList("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual new IList<VmOpenApiLanguageItem> SupportEmails { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// Indicates if channel can be used (referenced within services) by other users from other organizations.
        /// </summary>
        [JsonIgnore]
        public override bool IsVisibleForAll { get => base.IsVisibleForAll; set => base.IsVisibleForAll = value; }


        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 3;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiWebPageChannelInVersionBase>();
            SupportPhones.ForEach(p => vm.SupportPhones.Add(p.ConvertToVersion4()));
            vm.SupportEmails = SupportEmails.SetListValueLength(100);
            vm.Urls = Urls.SetListValueLength(500);
            return vm;
        }
        #endregion
    }
}
