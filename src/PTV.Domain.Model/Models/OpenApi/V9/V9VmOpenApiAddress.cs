/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework.Extensions;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressVersionBase" />
    public class V9VmOpenApiAddress : VmOpenApiAddressVersionBase
    {
        /// <summary>
        /// Address sub type, Street, PostOfficeBox, Foreign or Other.
        /// </summary>
        public override string SubType { get => base.SubType; set => base.SubType = value; }

        /// <summary>
        /// Address coordinates with additional information.
        /// </summary>
        [JsonProperty(Order = 5)]
        public VmOpenApiAddressOther OtherAddress { get; set; }

        /// <summary>
        /// Localized list of foreign address information.
        /// </summary>
        [JsonProperty(Order = 6)]
        public virtual IList<VmOpenApiLanguageItem> ForeignAddress { get; set; }

        /// <summary>
        /// Converts to version 7.
        /// </summary>
        /// <returns>converted model to version 7</returns>
        public V7VmOpenApiAddress ConvertToPreviousVersion()
        {
            var vm = base.ConvertToVersion<V7VmOpenApiAddress>();
            vm.ForeignAddress = this.ForeignAddress;
            // Other address is converted into street address
            if (vm.SubType == AddressConsts.OTHER && this.OtherAddress != null)
            {
                vm.SubType = AddressConsts.STREET;
                vm.StreetAddress = new VmOpenApiAddressStreetWithCoordinates
                {
                    Latitude = this.OtherAddress.Latitude,
                    Longitude = this.OtherAddress.Longitude,
                    CoordinateState = CoordinateStates.EnteredByUser.ToString(),
                    PostalCode = this.OtherAddress.PostalCode,
                    PostOffice = this.OtherAddress.PostOffice,
                    Municipality = this.OtherAddress.Municipality,
                    AdditionalInformation = this.OtherAddress.AdditionalInformation
                };
            }
            return vm;
        }
    }
}
