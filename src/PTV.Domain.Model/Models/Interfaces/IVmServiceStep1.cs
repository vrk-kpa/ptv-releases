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
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of service step 1
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IEnumCollection" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocalized" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmServiceProducer" />
    public interface IVmServiceStep1 : IVmEntityBase, IEnumCollection, IVmLocalized, IVmServiceProducer
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        string ServiceName { get; set; }
        /// <summary>
        /// Gets or sets the alternate name of the service.
        /// </summary>
        /// <value>
        /// The alternate name of the service.
        /// </value>
        string AlternateServiceName { get; set; }
        /// <summary>
        /// Gets or sets the short descriptions.
        /// </summary>
        /// <value>
        /// The short descriptions.
        /// </value>
        string ShortDescriptions { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; set; }
        /// <summary>
        /// Gets or sets the user instruction.
        /// </summary>
        /// <value>
        /// The user instruction.
        /// </value>
        string UserInstruction { get; set; }
        /// <summary>
        /// Gets or sets the service usage.
        /// </summary>
        /// <value>
        /// The service usage.
        /// </value>
        string ServiceUsage { get; set; }
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        string AdditionalInformation { get; set; }
        /// <summary>
        /// Gets or sets the additional information dead line.
        /// </summary>
        /// <value>
        /// The additional information dead line.
        /// </value>
        string AdditionalInformationDeadLine { get; set; }
        /// <summary>
        /// Gets or sets the additional information processing time.
        /// </summary>
        /// <value>
        /// The additional information processing time.
        /// </value>
        string AdditionalInformationProcessingTime { get; set; }
        /// <summary>
        /// Gets or sets the additional information validity time.
        /// </summary>
        /// <value>
        /// The additional information validity time.
        /// </value>
        string AdditionalInformationValidityTime { get; set; }
        /// <summary>
        /// Gets or sets the service coverage type identifier.
        /// </summary>
        /// <value>
        /// The service coverage type identifier.
        /// </value>
        Guid AreaInformationTypeId { get; set; }
        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        List<Guid> Languages { get; set; }
        /// <summary>
        /// Gets or sets the municipalities.
        /// </summary>
        /// <value>
        /// The municipalities.
        /// </value>
        List<VmLaw> Laws { get; set; }

        /// <summary>
        /// Identifier of unific root node
        /// </summary>
        Guid UnificRootId { get; set; }

        /// <summary>
        /// Gets or sets the funding type identifier.
        /// </summary>
        /// <value>
        /// The funding type identifier.
        /// </value>
        Guid FundingTypeId { get; set; }

        /// <summary>
        /// Gets or sets the service vouchers.
        /// </summary>
        /// <value>
        /// The service vouchers.
        /// </value>
        List<VmServiceVoucher> ServiceVouchers { get; set; }
    }
}
