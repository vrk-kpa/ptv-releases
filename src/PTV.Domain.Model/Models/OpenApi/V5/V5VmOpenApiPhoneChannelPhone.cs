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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V5
{
    /// <summary>
    /// OPEN API V5 - View Model of phone
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V4.V4VmOpenApiPhoneSimple" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V5.IV5VmOpenApiPhoneChannelPhone" />
    public class V5VmOpenApiPhoneChannelPhone : V4VmOpenApiPhoneSimple, IV5VmOpenApiPhoneChannelPhone
    {
        /// <summary>
        /// Phone number type (Phone, Sms or Fax).
        /// </summary>
        [ValidEnum(typeof(PhoneNumberTypeEnum))]
        [Required(AllowEmptyStrings = false)]
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exists in all languages.
        /// </summary>
        /// <value>
        /// <c>true</c> if exists in all languages; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool ExistsOnePerLanguage { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other.
        /// </summary>
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// Charge description.
        /// </summary>
        [ValueNotEmpty]
        [MaxLength(150)]
        [RequiredIf("ServiceChargeType", "Other")]
        public virtual string ChargeDescription { get; set; }

        /// <summary>
        /// Converts model to version 4.
        /// </summary>
        /// <returns></returns>
        public V4VmOpenApiPhoneWithType ConvertToVersion4()
        {
            return ConvertToVersion4<V4VmOpenApiPhoneWithType>();
        }

        /// <summary>
        /// Base class for converting phone model into version 4.
        /// </summary>
        /// <returns></returns>
        protected TModel ConvertToVersion4<TModel>() where TModel : IV4VmOpenApiPhoneWithType, new()
        {
            var vm = GetVersionBaseModel<TModel>();
            vm.Type = this.Type;
            vm.ServiceChargeType = this.ServiceChargeType;
            vm.ChargeDescription = this.ChargeDescription;
            return vm;
        }
    }
}
