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
using System.Collections.Generic;
using System;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Service collection service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceCollectionService" />
    public class VmOpenApiServiceCollectionService : IVmOpenApiServiceCollectionService
    {
        /// <summary>
        /// Service collection service Id
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Localized list of service collection service names
        /// </summary>
        public virtual IList<VmOpenApiLocalizedListItem> Name { get; set; }
        /// <summary>
        /// Localized list of service collection service descriptions
        /// </summary>
        public virtual IList<VmOpenApiLocalizedListItem> Description { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
        /// <summary>
        /// Gets or sets the order numnber
        /// </summary>
        /// <value>
        /// The order number.
        /// </value>
        [JsonIgnore]
        public int OrderNumber { get; set; }
    }
}
