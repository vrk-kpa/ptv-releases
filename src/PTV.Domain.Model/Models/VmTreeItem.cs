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
using Newtonsoft.Json;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    public class VmTreeItem : VmSelectableItem, IVmTreeItem, IVmListItem, IVmOwnerReference
    {
//        public string Name { get; set; }
        public bool IsCollapsed { get; set; }
//        public bool IsSelected { get; set; }
//        public bool IsDisabled { get; set; }
        public bool IsFetching { get; set; }
//        public Guid Id { get; set; }
        public bool IsLeaf { get; set; }
        public bool AreChildrenLoaded { get; set; }
        public List<VmTreeItem> Children { get; set; }
        public List<int> OnIndex { get; set; }

        public VmTreeItem()
        {
            Name = string.Empty;
            IsLeaf = false;
            IsCollapsed = true;
            IsSelected = false;
            Children = new List<VmTreeItem>();
            OnIndex = new List<int>();
        }

//        [JsonIgnore]
//        public Guid? OwnerReferenceId { get; set; }
    }
}
