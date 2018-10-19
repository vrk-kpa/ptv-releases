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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of service and channel relation IN by source (PUT).
    /// </summary>
    public class V6VmOpenApiServiceServiceChannelBySource
    {
        /// <summary>
        /// The external source id for service channel.
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        [Required]
        public string ServiceChannelSourceId { get; set; }

        /// <summary>
        /// List of localized service channel relationship descriptions.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues("Type", allowedValues: new[] { "Description", "ChargeTypeAdditionalInfo" })]
        [ListPropertyMaxLength(500, "Value", "Description")]
        [ListPropertyMaxLength(500, "Value", "ChargeTypeAdditionalInfo")]
        public IList<VmOpenApiLocalizedListItem> Description { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public string ServiceChargeType { get; set; }

        /// <summary>
        /// Converts model into latest version.
        /// </summary>
        /// <returns></returns>
        public V8VmOpenApiServiceServiceChannelBySource ConvertToLatestVersion()
        {
            var vm = new V8VmOpenApiServiceServiceChannelBySource()
            {
                ServiceChannelSourceId = this.ServiceChannelSourceId,
                Description = this.Description,
                ServiceChargeType = this.ServiceChargeType,
            };

            // In older versions the relatioship model does not introduce DeleteServiceChargeType and DeleteAllDescriptions properties,
            // so we are assuming that if ServiceChargeType or Description is not set within request, those values should be emptied.
            if (string.IsNullOrEmpty(vm.ServiceChargeType))
            {
                vm.DeleteServiceChargeType = true;
            }
            if (vm.Description.Count == 0)
            {
                vm.DeleteAllDescriptions = true;
            }

            return vm;
        }
    }
}
