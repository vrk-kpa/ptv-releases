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

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of phone
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityBase" />
    public interface IVmPhone: IVmEntityBase, IVmOrdered
    {
        /// <summary>
        /// Gets or sets the charge description.
        /// </summary>
        /// <value>
        /// The charge description.
        /// </value>
        string ChargeDescription { get; set; }
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        string AdditionalInformation { get; set; }
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        string Number { get; set; }
        /// <summary>
        /// Gets or sets the prefix number.
        /// </summary>
        /// <value>
        /// The prefix number.
        /// </value>
        VmDialCode PrefixNumber { get; set; }
        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        Guid TypeId { get; set; }
        /// <summary>
        /// Gets or sets the charge type identifier.
        /// </summary>
        /// <value>
        /// The charge type identifier.
        /// </value>
        Guid ChargeTypeId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is finnish service number.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is finnish service number; otherwise, <c>false</c>.
        /// </value>
        bool IsFinnishServiceNumber { get; set; }
    }
}
