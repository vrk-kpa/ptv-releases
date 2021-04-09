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

using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service producer
    /// </summary>
    public class VmOpenApiServiceProducerIn : VmOpenApiServiceProducerBase
    {
        /// <summary>
        /// The order of service voucher.
        /// </summary>
        [JsonProperty(Order = 1)]
        [Required]
        public override int OrderNumber { get => base.OrderNumber; set => base.OrderNumber = value; }

        /// <summary>
        /// Gets or sets the organization id information.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        [ListRequiredIf("ProvisionType", "SelfProduced")] // for version 7 and down
        [ListRequiredIf("ProvisionType", "SelfProducedServices")] // for version 8 and up (PTV-4049)
        [JsonProperty(Order = 3)]
        public virtual IList<Guid> Organizations { get; set; } = new List<Guid>();

        /// <summary>
        /// Converts to latest in base model.
        /// </summary>
        /// <returns></returns>
        public V9VmOpenApiServiceProducerIn ConvertToInVersionBase(int version)
        {
            return new V9VmOpenApiServiceProducerIn
            {
                OrderNumber = this.OrderNumber,
                OwnerReferenceId = this.OwnerReferenceId,
                ProvisionType = this.ProvisionType,
                Organizations = this.Organizations,
                AdditionalInformation = this.AdditionalInformation
            };
        }
    }
}
