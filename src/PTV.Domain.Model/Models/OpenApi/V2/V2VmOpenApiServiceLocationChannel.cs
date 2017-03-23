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
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V1;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of service location channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiServiceLocationChannel" />
    public class V2VmOpenApiServiceLocationChannel : VmOpenApiServiceLocationChannelVersionBase, IV2VmOpenApiServiceLocationChannel
    {
        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual new IList<VmOpenApiPhoneWithType> PhoneNumbers { get; set; } = new List<VmOpenApiPhoneWithType>();

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// List of serviceareas. Used when location service channel is restricted by service area (ServiceAreaRestricted=true). List contains municipality names.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IList<string> ServiceAreas { get; set; }

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V2VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IList<V2VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V2VmOpenApiServiceHour>();

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version</returns>
        public override int VersionNumber()
        {
            return 2;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiServiceLocationChannel>();
// it's allowed to use obsolete attributes for transaltions V1 <-> V2
#pragma warning disable 612, 618
            vm.Email = SupportEmails.FirstOrDefault()?.Value;

            var phone = PhoneNumbers.FirstOrDefault(p => p.Type == PhoneNumberTypeEnum.Phone.ToString());
            if (phone != null)
            {
                vm.Phone = string.IsNullOrEmpty(phone.PrefixNumber) ? phone.Number : $"{ phone.PrefixNumber } { phone.Number }";
                if (!string.IsNullOrEmpty(phone.ServiceChargeType))
                {
                    vm.ServiceChargeTypes = new List<string>() { phone.ServiceChargeType };
                }
                if (!string.IsNullOrEmpty(phone.ChargeDescription))
                {
                    vm.PhoneChargeDescriptions = new List<VmOpenApiLanguageItem>()
                    {
                        new VmOpenApiLanguageItem() { Value = phone.ChargeDescription, Language = phone.Language }
                    };

                }
            }
            var fax = PhoneNumbers.FirstOrDefault(p => p.Type == PhoneNumberTypeEnum.Fax.ToString());
            if (fax != null)
            {
                vm.Fax = string.IsNullOrEmpty(fax.PrefixNumber) ? fax.Number : $"{ fax.PrefixNumber } { fax.Number }";
            }

            // Coordinates
            var visitingAddress = Addresses.Where(a => a.Type == AddressTypeEnum.Visiting.ToString()).FirstOrDefault();
            if (visitingAddress != null)
            {
                vm.Latitude = visitingAddress.Latitude;
                vm.Longitude = visitingAddress.Longitude;
            }

            vm.ServiceAreas = ServiceAreas;
            vm.Addresses = new List<VmOpenApiAddressWithType>();
            Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion1()));
            vm.ServiceHours = TranslateToV1ServiceHours(ServiceHours);
            vm.WebPages = WebPages;

            return vm;
#pragma warning restore 612, 618
        }

        #endregion
    }
}
