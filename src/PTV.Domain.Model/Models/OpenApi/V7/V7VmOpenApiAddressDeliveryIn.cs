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
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of delivery address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V7.IV7VmOpenApiAddressDeliveryIn" />
    public class V7VmOpenApiAddressDeliveryIn : VmOpenApiBase, IV7VmOpenApiAddressDeliveryIn
    {
        /// <summary>
        /// Address sub type, Street, PostOfficeBox or NoAddress.
        /// </summary>
        [AllowedValues(propertyName: "SubType", allowedValues: new[] { AddressConsts.STREET, AddressConsts.POSTOFFICEBOX, AddressConsts.NOADDRESS })]
        [Required]
        public string SubType { get; set; }

        /// <summary>
        /// Post office box address
        /// </summary>
        [RequiredIf("SubType", AddressConsts.POSTOFFICEBOX)]
        public virtual VmOpenApiAddressPostOfficeBoxIn PostOfficeBoxAddress { get; set; }

        /// <summary>
        /// Street address.
        /// </summary>
        [RequiredIf("SubType", AddressConsts.STREET)]
        public virtual VmOpenApiAddressStreetIn StreetAddress { get; set; }

        /// <summary>
        /// Localized list of foreign address information.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyMaxLength(150, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListRequiredIf("SubType", AddressConsts.NOADDRESS)]
        public IList<VmOpenApiLanguageItem> DeliveryAddressInText { get; set; }
    }
}
