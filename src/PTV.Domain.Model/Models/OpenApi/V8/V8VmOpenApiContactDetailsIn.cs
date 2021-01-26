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

using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of contact details (POST).
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiContactDetailsInVersionBase" />
    public class V8VmOpenApiContactDetailsIn : VmOpenApiContactDetailsInVersionBase
    {
        /// <summary>
        /// List of connection related phone numbers.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ListWithOpenApiEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> PhoneNumbers { get => base.PhoneNumbers; set => base.PhoneNumbers = value; }

        /// <summary>
        /// List of connection related fax numbers numbers.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhoneSimple> FaxNumbers { get => base.FaxNumbers; set => base.FaxNumbers = value; }

        /// <summary>
        /// List of connection related web pages.
        /// </summary>
        [JsonProperty(Order = 5)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual new IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        #region Methods

        /// <summary>
        /// Converts into latest in base model
        /// </summary>
        /// <returns></returns>
        public override V9VmOpenApiContactDetailsInBase ConvertToInBaseModel()
        {
            var model = base.ConvertToInBaseModel();
            this.WebPages.ForEach(w => model.WebPages.Add(w.ConvertToInBaseModel()));
            return model;
        }

        /// <summary>
        /// Converts into latest in model
        /// </summary>
        /// <returns></returns>
        public override V9VmOpenApiContactDetailsIn ConvertToInModel()
        {
            var model = base.ConvertToInModel();
            this.WebPages.ForEach(w => model.WebPages.Add(w.ConvertToInBaseModel()));
            return model;
        }
        #endregion
    }
}
