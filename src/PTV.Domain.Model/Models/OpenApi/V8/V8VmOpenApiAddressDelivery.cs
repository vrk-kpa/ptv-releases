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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of delivery address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V7.IV7VmOpenApiAddressDelivery" />
    public class V8VmOpenApiAddressDelivery : IV8VmOpenApiAddressDelivery
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
        /// List of localized form receiver. One per language.
        /// </summary>
        public IList<VmOpenApiLanguageItem> Receiver { get; set; }

        /// <summary>
        /// Converts delivery address into version 7.
        /// </summary>
        /// <returns></returns>
        public V7VmOpenApiAddressDelivery ConvertToVersion7()
        {
            return new V7VmOpenApiAddressDelivery
            {
                SubType = SubType,
                PostOfficeBoxAddress = PostOfficeBoxAddress,
                StreetAddress = StreetAddress,
                DeliveryAddressInText = DeliveryAddressInText
            };
        }
    }
}
