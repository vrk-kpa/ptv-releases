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

using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of Address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V4.IV4VmOpenApiAddressIn" />
    public class V4VmOpenApiAddressIn : VmOpenApiAddressInVersionBase, IV4VmOpenApiAddressIn
    {
        /// <summary>
        /// Post office box like PL 310
        /// </summary>
        [RequiredIf("StreetAddress", null)]
        public new string PostOfficeBox { get; set; }

        /// <summary>
        /// List of localized street addresses.
        /// </summary>
        [RequiredIf(AddressConsts.POSTOFFICEBOX, "", typeof(string))]
        public override IList<VmOpenApiLanguageItem> StreetAddress
        {
            get
            {
                return base.StreetAddress;
            }

            set
            {
                base.StreetAddress = value;
            }
        }


        /// <summary>
        /// Service location latitude coordinate.
        /// </summary>
        [JsonIgnore]
        public override string Latitude
        {
            get
            {
                return base.Latitude;
            }

            set
            {
                base.Latitude = value;
            }
        }

        /// <summary>
        /// Service location longitude coordinate.
        /// </summary>
        [JsonIgnore]
        public override string Longitude
        {
            get
            {
                return base.Longitude;
            }

            set
            {
                base.Longitude = value;
            }
        }

        /// <summary>
        /// Converts model to latest version.
        /// </summary>
        /// <returns></returns>
        public V7VmOpenApiAddressDeliveryIn LatestVersionDeliveryAddress()
        {
            var vm = base.ConvertToDeliveryAddressVersionIn<V7VmOpenApiAddressDeliveryIn>();

            if (!string.IsNullOrEmpty(this.PostOfficeBox) && vm.PostOfficeBoxAddress != null)
            {
                vm.PostOfficeBoxAddress.PostOfficeBox = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Value = PostOfficeBox, Language = LanguageCode.fi.ToString() } };                
            }
            
            return vm;
        }
    }
}
