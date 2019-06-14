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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of general description
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionVersionBase" />
    public class V8VmOpenApiGeneralDescription : VmOpenApiGeneralDescriptionVersionBase
    {
        /// <summary>
        /// List of statutory service general description languages.
        /// </summary>
        [JsonProperty(Order = 4)]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public virtual IList<string> Languages { get; set; }

        #region methods
        /// <summary>
        /// Open api version number.
        /// </summary>
        /// <returns>Open api version number</returns>
        public override int VersionNumber()
        {
            return 8;
        }

        /// <summary>
        /// Converts model to previous version.
        /// </summary>
        /// <returns></returns>
        public override IVmOpenApiGeneralDescriptionVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V7VmOpenApiGeneralDescription>();
            // Change property values into ones used in older versions (PTV-2184)
            vm.Type = string.IsNullOrEmpty(vm.Type) ? vm.Type : vm.Type.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>();
            vm.ServiceChargeType = string.IsNullOrEmpty(vm.ServiceChargeType) ? vm.ServiceChargeType : vm.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>();
            vm.Names?.ForEach(n => n.SetV7Values<NameTypeEnum>());
            vm.Descriptions?.ForEach(d => d.SetV7Values<DescriptionTypeEnum>());
            vm.ServiceChannels?.ForEach(c => c.SetV7Values());
            return vm;
        }
        #endregion
    }
}
