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
**/

using System;
using PTV.Domain.Model.Models.Interfaces;
using Newtonsoft.Json;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiPhone : VmOpenApiPhoneSimple, IVmOpenApiPhone
    {
        /// <summary>
        /// Additional information.
        /// </summary>
        public virtual string AdditionalInformation { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other.
        /// </summary>
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// Charge description.
        /// </summary>
        [MaxLength(150)]
        [RequiredIf("ServiceChargeType", "Other")]
        public virtual string ChargeDescription { get; set; }
    }
}
