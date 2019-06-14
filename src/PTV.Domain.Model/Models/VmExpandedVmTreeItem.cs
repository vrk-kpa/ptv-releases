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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of epanded tree item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmTreeItem" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmExpandedVmTreeItem" />
    public class VmExpandedVmTreeItem : VmTreeItem, IVmExpandedVmTreeItem, IVmRestrictableType
    {
        /// <summary>
        /// 
        /// </summary>
        public EVmRestrictionFilterType Access { get; set; }
    }

    /// <summary>
    /// View model of epanded tree item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmTreeItem" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmExpandedVmTreeItem" />
    public class VmFilteredTreeItem : VmListItem, IVmTreeItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether children of item are loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if children are loaded; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool AreChildrenLoaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is leaf.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is leaf; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Gets or sets the children. Contains filtered children, serialized as <see cref="FilteredChildren"/>
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        [JsonIgnore]
        public List<IVmTreeItem> Children { get; set; }

        /// <summary>
        /// Gets the filtered children stored in <see cref="Children"/>
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<IVmTreeItem> FilteredChildren => Children;
    }
}
