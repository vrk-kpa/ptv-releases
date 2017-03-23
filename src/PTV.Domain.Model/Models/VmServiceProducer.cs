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
    /// View model of service producer
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public class VmServiceProducer : VmEntityBase, IVmOwnerReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceProducer"/> class.
        /// </summary>
        public VmServiceProducer()
        {
            SelfProduced = new VmServiceProducerDetail();
            VoucherServices = new VmServiceProducerDetail();
            PurchaseServices = new VmServiceProducerDetail();
            Other = new VmServiceProducerDetail();
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }
        /// <summary>
        /// Gets or sets the provision type identifier.
        /// </summary>
        /// <value>
        /// The provision type identifier.
        /// </value>
        public Guid? ProvisionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the self produced detail.
        /// </summary>
        /// <value>
        /// The self produced detail.
        /// </value>
        public VmServiceProducerDetail SelfProduced { get; set; }
        /// <summary>
        /// Gets or sets the voucher services detail.
        /// </summary>
        /// <value>
        /// The voucher services detail.
        /// </value>
        public VmServiceProducerDetail VoucherServices { get; set; }
        /// <summary>
        /// Gets or sets the purchase services detail.
        /// </summary>
        /// <value>
        /// The purchase services detail.
        /// </value>
        public VmServiceProducerDetail PurchaseServices { get; set; }
        /// <summary>
        /// Gets or sets the other detail.
        /// </summary>
        /// <value>
        /// The other detail.
        /// </value>
        public VmServiceProducerDetail Other { get; set; }

        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
    }
}
