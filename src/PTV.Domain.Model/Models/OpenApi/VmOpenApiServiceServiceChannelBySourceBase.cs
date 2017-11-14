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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Service service channel - with external source
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceServiceChannelVersionBase" />
    public class VmOpenApiServiceServiceChannelBySourceBase : IVmOpenApiServiceServiceChannelBySourceBase
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
        public IList<VmOpenApiLocalizedListItem> Description { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public string ServiceChargeType { get; set; }

        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonProperty(Order = 6)]
        public virtual IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();

        /// <summary>
        /// List of connection related contact details.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual VmOpenApiContactDetailsInBase ContactDetails { get; set; }

        /// <summary>
        /// Indicates if value for property ServiceChargeType should be deleted.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual bool DeleteServiceChargeType { get; set; }

        /// <summary>
        /// Indicates if all descriptions should be deleted.
        /// </summary>
        [JsonProperty(Order = 11)]
        public virtual bool DeleteAllDescriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all service hours should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service hours should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 12)]
        public virtual bool DeleteAllServiceHours { get; set; }

        /// <summary>
        /// Converts model into latest version.
        /// </summary>
        /// <returns></returns>
        public V7VmOpenApiServiceServiceChannelBySourceAsti ConvertToLatestVersion(int openApiVersion = 7)
        {
            var vm = GetBaseModel<V7VmOpenApiServiceServiceChannelBySourceAsti>();

            // In older versions the relatioship model does not introduce DeleteServiceChargeType and DeleteAllDescriptions properties,
            // so we are assuming that if ServiceChargeType or Description is not set within request, those values should be emptied.
            if (openApiVersion < 7)
            {
                if (string.IsNullOrEmpty(vm.ServiceChargeType))
                {
                    vm.DeleteServiceChargeType = true;
                }
                if (vm.Description.Count == 0)
                {
                    vm.DeleteAllDescriptions = true;
                }
            }

            return vm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected TModel GetBaseModel<TModel>() where TModel : IVmOpenApiServiceServiceChannelBySourceBase, new()
        {
            return new TModel()
            {
                ServiceChannelSourceId = this.ServiceChannelSourceId,
                Description = this.Description,
                ServiceChargeType = this.ServiceChargeType,
                ServiceHours = this.ServiceHours,
                ContactDetails = this.ContactDetails
            };
        }
    }
}
