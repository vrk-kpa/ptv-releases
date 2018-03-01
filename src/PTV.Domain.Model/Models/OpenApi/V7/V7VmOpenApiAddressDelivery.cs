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

using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using System.Collections.Generic;
using System;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of delivery address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V7.IV7VmOpenApiAddressDelivery" />
    public class V7VmOpenApiAddressDelivery : IV7VmOpenApiAddressDelivery
    {
        /// <summary>
        /// Address sub type, Street, PostOfficeBox or NoAddress.
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// Post office box address
        /// </summary>
        public virtual VmOpenApiAddressPostOfficeBox PostOfficeBoxAddress { get; set; }

        /// <summary>
        /// Street address.
        /// </summary>
        public virtual VmOpenApiAddressStreet StreetAddress { get; set; }

        /// <summary>
        /// Localized list of delivery address information.
        /// </summary>
        public IList<VmOpenApiLanguageItem> DeliveryAddressInText { get; set; }

        /// <summary>
        /// Converts delivery address into version 5.
        /// </summary>
        /// <returns></returns>
        public V5VmOpenApiAddressWithCoordinates ConvertToVersion5()
        {
            if (SubType == AddressTypeEnum.Street.ToString() && StreetAddress != null)
            {
                return new V5VmOpenApiAddressWithCoordinates()
                {
                    StreetAddress = StreetAddress.Street,
                    StreetNumber = StreetAddress.StreetNumber,
                    PostalCode = StreetAddress.PostalCode,
                    PostOffice = StreetAddress.PostOffice,
                    Municipality = StreetAddress.Municipality,
                    AdditionalInformations = StreetAddress.AdditionalInformation,
                    Country = "FI"
                };
            }
            
            if (SubType == AddressTypeEnum.PostOfficeBox.ToString() && PostOfficeBoxAddress != null)
            {
                return new V5VmOpenApiAddressWithCoordinates()
                {
                    PostOfficeBox = PostOfficeBoxAddress.PostOfficeBox,
                    PostalCode = PostOfficeBoxAddress.PostalCode,
                    PostOffice = PostOfficeBoxAddress.PostOffice,
                    Municipality = PostOfficeBoxAddress.Municipality,
                    AdditionalInformations = PostOfficeBoxAddress.AdditionalInformation,
                    Country = "FI"
                };
            }

            if (SubType == AddressTypeEnum.NoAddress.ToString())
            {
                return new V5VmOpenApiAddressWithCoordinates()
                {
                    AdditionalInformations = DeliveryAddressInText,
                    Country = "FI"
                };
            }

            return new V5VmOpenApiAddressWithCoordinates() { Country = "FI" };
        }
    }
}
