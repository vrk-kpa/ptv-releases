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
using PTV.Domain.Model.Models;
using PTV.Framework;
using System.Linq;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Logic
{
    [RegisterService(typeof(VmListItemLogic), RegisterType.Transient)]
    public class VmListItemLogic
    {
        public List<VmTreeItem> FlattenTreeToList(List<VmTreeItem> treeItems)
        {
            var result = new List<VmTreeItem>();

            foreach (var node in treeItems)
            {
                if (node.Id != Guid.Empty)
                {
                    result.Add(new VmTreeItem() { Id = node.Id });
                }

                result.AddRange(FlattenTreeToList(node.Children));
            }

            return result;
        }
        public void SelectTreeItems(List<VmTreeItem> inputTree, bool checkSelected = true, bool checkDisabled = true)
        {
            foreach (var item in inputTree)
            {
                item.IsDisabled = checkDisabled;
                item.IsSelected = checkSelected;

                if (item.Children.Any())
                {
                    SelectTreeItems(item.Children, checkSelected, checkDisabled);
                }
            }
        }

        public void SelectByIds(List<VmSelectableItem> items, List<Guid> ids, bool isDisabled = false)
        {
            items.ForEach(x =>
            {
                if (ids.Contains(x.Id))
                {
                    x.IsSelected = true;
                    x.IsDisabled |= isDisabled;
                }
            });
        }

        public bool SelectByIds(List<VmTreeItem> items, List<Guid> ids, bool isDisabled = false, bool selectWithParent = true)
        {
            bool anySelected = false;
            items.ForEach(x =>
            {
                bool anyChildrenSelected = SelectByIds(x.Children, ids, isDisabled);
                var contains = ids.Contains(x.Id);
                if (contains || (anyChildrenSelected && selectWithParent))
                {
                    x.IsSelected = true;
                    x.IsDisabled |= isDisabled;
                }
                anySelected &= x.IsSelected;
            });
            return anySelected;
        }

        public IReadOnlyList<VmTreeItem> GetSelected(IEnumerable<VmTreeItem> inputTree)
        {
            List<VmTreeItem> result = new List<VmTreeItem>();
            foreach (var item in inputTree.Where(x => x.IsSelected))
            {
                if (!item.IsDisabled)
                {
                    result.Add(item);
                }
                result.AddRange(GetSelected(item.Children));
            }
            return result;
        }

        public IEnumerable<IVmSelectableItem> GetSelected(IEnumerable<IVmSelectableItem> inputTree)
        {
            return inputTree.Where(x => x.IsSelected && !x.IsDisabled);
        }
    }
}
