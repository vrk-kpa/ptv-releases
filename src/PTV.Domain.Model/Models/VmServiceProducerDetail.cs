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

using System;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service producer detail
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public class VmServiceProducerDetail : IVmOwnerReference
    {
        /// <summary>
        /// Gets or sets the producer organization identifier.
        /// </summary>
        /// <value>
        /// The producer organization identifier.
        /// </value>
        public Guid? ProducerOrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the sub producer organization identifier.
        /// </summary>
        /// <value>
        /// The sub producer organization identifier.
        /// </value>
        public Guid? SubProducerOrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the link to producer.
        /// </summary>
        /// <value>
        /// The link to producer.
        /// </value>
        public string Link { get; set; }
        /// <summary>
        /// Gets or sets the free description.
        /// </summary>
        /// <value>
        /// The free description.
        /// </value>
        public string FreeDescription { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
