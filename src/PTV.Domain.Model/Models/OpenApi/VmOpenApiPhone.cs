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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of phone
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiPhoneSimple" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPhone" />
    public class VmOpenApiPhone : VmOpenApiPhoneSimple, IVmOpenApiPhone
    {
        /// <summary>
        /// Additional information.
        /// NOTE! Current PTV database accepts only maximum of 150 characters. If more are added those will be cut out.
        /// </summary>
        [ValueNotEmpty]
        public virtual string AdditionalInformation { get; set; }

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
        public new V4VmOpenApiPhone ConvertToVersion4()
        {
            return ConvertToVersionBase<V4VmOpenApiPhone>();
        }

        /// <summary>
        /// Converts model to version base.
        /// </summary>
        /// <returns></returns>
        protected TModel ConvertToVersionBase<TModel>() where TModel : IV4VmOpenApiPhone, new()
        {
            var vm = GetVersionBaseModel<TModel>();
            vm.AdditionalInformation = this.AdditionalInformation.SetStringValueLength(150);
            vm.ServiceChargeType = this.ServiceChargeType;
            vm.ChargeDescription = this.ChargeDescription;
            return vm;
        }
    }
}
