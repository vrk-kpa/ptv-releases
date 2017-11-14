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
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API - View Model of Service service channel - with external source
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceServiceChannelVersionBase" />
    public class V7VmOpenApiServiceAndChannelRelationBySource : VmOpenApiServiceServiceChannelBySourceBase
    {
        /// <summary>
        /// The external source id for service.
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        [Required]
        public string ServiceSourceId { get; set; }

        /// <summary>
        /// List of connection related contact details.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual new VmOpenApiContactDetailsIn ContactDetails { get; set; }

        /// <summary>
        /// Indicates if value for property ServiceChargeType should be deleted.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteServiceChargeType { get => base.DeleteServiceChargeType; set => base.DeleteServiceChargeType = value; }

        /// <summary>
        /// Indicates if all descriptions should be deleted.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllDescriptions { get => base.DeleteAllDescriptions; set => base.DeleteAllDescriptions = value; }

        /// <summary>
        /// Gets or sets a value indicating whether all service hours should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service hours should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public override bool DeleteAllServiceHours { get => base.DeleteAllServiceHours; set => base.DeleteAllServiceHours = value; }
    }
}
