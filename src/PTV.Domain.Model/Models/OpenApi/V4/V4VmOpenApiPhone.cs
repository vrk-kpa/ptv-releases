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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of phone
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V4.V4VmOpenApiPhoneSimple" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V4.IV4VmOpenApiPhone" />
    public class V4VmOpenApiPhone : V4VmOpenApiPhoneSimple, IV4VmOpenApiPhone
    {
        /// <summary>
        /// Additional information.
        /// </summary>
        [ValueNotEmpty]
        [MaxLength(150)]
        public virtual string AdditionalInformation { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Chargeable, FreeOfCharge or Other.
        /// In version 7 and older: Charged, Free or Other.
        /// </summary>
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// Charge description.
        /// </summary>
        [ValueNotEmpty]
        [MaxLength(150)]
        [RequiredIf("ServiceChargeType", "Other")]
        public virtual string ChargeDescription { get; set; }

    }
}
