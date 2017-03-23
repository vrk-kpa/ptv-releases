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
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of service step 4
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmServiceStep4" />
    public class VmServiceStep4 : VmEnumEntityBase, IVmServiceStep4
    {
        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        /// <value>
        /// The name of the channel.
        /// </value>
        public string ChannelName { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>
        /// The organizations.
        /// </value>
        public List<VmListItem> Organizations { get; set; }
        /// <summary>
        /// Gets or sets the selected channel types.
        /// </summary>
        /// <value>
        /// The selected channel types.
        /// </value>
        public List<Guid> SelectedChannelTypes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceStep4"/> class.
        /// </summary>
        public VmServiceStep4()
        {
            ChannelName = string.Empty;
            Organizations = new List<VmListItem>();
            SelectedChannelTypes = new List<Guid>();
        }
    }
}
