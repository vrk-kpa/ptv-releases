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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Attributes.ValidationAttributes;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of contact details (POST).
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiContactDetailsInVersionBase" />
    public class V9VmOpenApiContactDetailsIn : VmOpenApiContactDetailsInVersionBase
    {
        /// <summary>
        /// List of connection related phone numbers.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ListWithOpenApiEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> PhoneNumbers { get => base.PhoneNumbers; set => base.PhoneNumbers = value; }
        
        #region Methods

        /// <summary>
        /// Set contact details values for version 7.
        /// </summary>
        public void SetV7Values()
        {
            PhoneNumbers.ForEach(p => p.SetV7Values());
        }

        #endregion
    }
}
