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

using System;
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of tree item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmSelectableItem" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmTreeItem" />
    public class VmTreeItem : VmSelectableItem, IVmTreeItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether this tree item is leaf.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this tree item is leaf; otherwise, <c>false</c>.
        /// </value>
        public bool IsLeaf { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether children of item are loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if children are loaded; otherwise, <c>false</c>.
        /// </value>
        public bool AreChildrenLoaded { get; set; }
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<IVmTreeItem> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmTreeItem"/> class.
        /// </summary>
        public VmTreeItem()
        {
            Name = string.Empty;
            IsLeaf = false;
            Children = new List<IVmTreeItem>();
        }
    }

    /// <summary>
    /// View model of tree item
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmSelectableItem" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmTreeItem" />
    public class VmTreeItemWithStatus : VmTreeItem
    {
        /// <summary>
        /// Publishing status of item
        /// </summary>
        [JsonProperty("publishingStatus")]
        public Guid PublishingStatusId { get; set; }
    }
}
