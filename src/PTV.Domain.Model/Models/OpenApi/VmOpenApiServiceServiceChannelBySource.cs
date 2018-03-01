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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Service service channel - with external source
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceServiceChannelVersionBase" />
    public class VmOpenApiServiceServiceChannelBySource : VmOpenApiServiceServiceChannelBySourceBase, IVmOpenApiServiceServiceChannelBySource
    {
        /// <summary>
        /// The external source id for service.
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        [Required]
        public string ServiceSourceId { get; set; }

        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiServiceHour> ServiceHours { get => base.ServiceHours; set => base.ServiceHours = value; }

        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonIgnore]
        public override VmOpenApiContactDetailsInBase ContactDetails { get => base.ContactDetails; set => base.ContactDetails = value; }
        
        /// <summary>
        /// Convert to latest version
        /// </summary>
        /// <returns></returns>
        public V7VmOpenApiServiceAndChannelRelationBySource ConvertToLatestVersion()
        {
            var vm = GetBaseModel<V7VmOpenApiServiceAndChannelRelationBySource>();
            vm.ServiceSourceId = this.ServiceSourceId;
            return vm;
        }
    }
}
