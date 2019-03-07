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
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of channel search result base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmChannelSearchResultBase" />
    public class VmChannelSearchResultBase : VmEnumEntityBase, IVmChannelSearchResultBase
    {
        /// <summary>
        /// Gets or sets the found channels.
        /// </summary>
        /// <value>
        /// The channels.
        /// </value>
        public IReadOnlyList<IVmChannelListItem> Channels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is more than maximum.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is more than maximum; otherwise, <c>false</c>.
        /// </value>
        public bool IsMoreThanMax { get; set; }

        /// <summary>
        /// Gets or sets the number of all items.
        /// </summary>
        /// <value>
        /// The number of all items.
        /// </value>
        public int NumberOfAllItems { get; set; }
    }
}
