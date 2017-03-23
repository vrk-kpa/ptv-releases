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
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service location channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannel" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceLocationChannelVersionBase" />
    public class VmOpenApiServiceLocationChannelVersionBase : VmOpenApiServiceChannel, IVmOpenApiServiceLocationChannelVersionBase
    {
        /// <summary>
        /// Is the service location channel restricted by service area.
        /// </summary>
        [JsonProperty(Order = 10)]
        public bool ServiceAreaRestricted { get; set; }

        /// <summary>
        /// List of phone numbers for the service channel. Includes also fax numbers.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual IList<V4VmOpenApiPhoneWithType> PhoneNumbers { get; set; }

        /// <summary>
        /// List email addresses for the service channel.
        /// </summary>
        [JsonProperty(PropertyName = "emails", Order = 13)]
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
        /// Is the phone service charged for.
        /// </summary>
        [JsonProperty(Order = 19)]
        public bool PhoneServiceCharge { get; set; }

        /// <summary>
        /// List of serviceareas. Used when location service channel is restricted by service area (ServiceAreaRestricted=true). List contains municipality names.
        /// </summary>
        [JsonProperty(Order = 20)]
        public IList<VmOpenApiMunicipality> ServiceAreas { get; set; }

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public IReadOnlyList<V4VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; }

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


        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>
        /// model of previous version
        /// </returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            return GetVersionBaseModel<V4VmOpenApiServiceLocationChannel>();
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceLocationChannelVersionBase, new()
        {
            var vm = base.GetServiceChannelModel<TModel>();
            vm.ServiceAreaRestricted = this.ServiceAreaRestricted;
            vm.PhoneNumbers = this.PhoneNumbers;
            vm.SupportEmails = this.SupportEmails;
            vm.PhoneServiceCharge = this.PhoneServiceCharge;
            vm.ServiceAreas = this.ServiceAreas;
            vm.Addresses = this.Addresses;
            vm.SupportPhones = this.SupportPhones;
            return vm;
        }
        #endregion
    }
}
