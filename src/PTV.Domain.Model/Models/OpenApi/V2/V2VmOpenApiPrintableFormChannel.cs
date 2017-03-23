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
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V1;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of printable form channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiPrintableFormChannelVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiPrintableFormChannel" />
    public class V2VmOpenApiPrintableFormChannel : VmOpenApiPrintableFormChannelVersionBase, IV2VmOpenApiPrintableFormChannel
    {
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
        public new V2VmOpenApiAddressWithCoordinates DeliveryAddress { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 15)]
        public virtual new IList<VmOpenApiPhone> SupportPhones { get; set; } = new List<VmOpenApiPhone>();

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

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
            return 2;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<VmOpenApiPrintableFormChannel>();
            vm.FormIdentifier = FormIdentifier;
            vm.FormReceiver = FormReceiver;
            if (DeliveryAddress != null)
            {
                vm.DeliveryAddress = new VmOpenApiAddress()
                {
                    PostOfficeBox = DeliveryAddress.PostOfficeBox,
                    PostalCode = DeliveryAddress.PostalCode,
                    PostOffice = DeliveryAddress.PostOffice,
                    StreetAddress = DeliveryAddress.StreetAddress,
                    Municipality = DeliveryAddress.Municipality,
                    Country = DeliveryAddress.Country,
                    AdditionalInformations = DeliveryAddress.AdditionalInformations
                };
                if (!string.IsNullOrEmpty(DeliveryAddress.StreetNumber))
                {
                    // Add the street number after each street address
                    vm.DeliveryAddress.StreetAddress.ForEach(a => a.Value = $"{a.Value} {DeliveryAddress.StreetNumber}");
                }
            }

            vm.ServiceHours = TranslateToV1ServiceHours(ServiceHours);
            vm.SupportContacts = TranslateToV1SupportContacts(SupportEmails, SupportPhones);
            return vm;
        }
        #endregion
    }
}
