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

using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View Model of electronic channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiElectronicChannelInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiElectronicChannelInBase" />
    public class VmOpenApiElectronicChannelInBase : VmOpenApiElectronicChannelInVersionBase, IVmOpenApiElectronicChannelInBase
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
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IList<VmOpenApiServiceHour> ServiceHours { get; set; } = new List<VmOpenApiServiceHour>();

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
        /// gets the Version base.
        /// </summary>
        /// <returns>model</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiElectronicChannelInVersionBase>();
            vm.ServiceChannelDescriptions = ServiceChannelDescriptions.SetListValueLength();
            vm.SupportEmails = TranslateToV2SupportEmails(SupportContacts);
            vm.SupportPhones = TranslateToVersionBaseSupportPhones(SupportContacts);
            vm.Urls = Urls.SetListValueLength(500);
            vm.DeleteAllSupportEmails = DeleteAllSupportContacts;
            vm.DeleteAllSupportPhones = DeleteAllSupportContacts;
            vm.ServiceHours = TranslateToV4ServiceHours(ServiceHours);
            return vm;
        }
        #endregion
    }
}
