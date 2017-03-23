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
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using FluentAssertions;

namespace PTV.Domain.Logic.Tests.VmTree
{
    public class VmTreeLogicTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SelectTreeItemsTest(bool isSelected, bool isDisabled)
        {
            var treeLogic = new VmListItemLogic();
            var parentId = Guid.NewGuid();
            var childId = Guid.NewGuid();
            var model = CreateTree(parentId, childId);
            treeLogic.SelectTreeItems(model, isSelected, isDisabled);

            Assert.True(model.First().IsSelected == isSelected);
            Assert.False(model.First().IsCollapsed);
            Assert.True(model.First().IsDisabled == isDisabled);

            Assert.True(model.First().Children.First().IsSelected == isSelected);
            Assert.True(model.First().Children.First().IsDisabled == isDisabled);
            Assert.False(model.First().Children.First().IsCollapsed);

            model = CreateTree(parentId, childId);
            treeLogic.SelectTreeItems(model, isSelected, isDisabled);

            Assert.True(model.First().IsSelected == isSelected);
            Assert.True(model.First().IsDisabled == isDisabled);
            Assert.False(model.First().IsCollapsed);

            Assert.True(model.First().Children.First().IsSelected == isSelected);
            Assert.False(model.First().Children.First().IsCollapsed);
            Assert.True(model.First().Children.First().IsDisabled == isDisabled);
        }

        private List<VmTreeItem> CreateTree(Guid parrentId, Guid chidId)
        {
            return new List<VmTreeItem>
            {
                new VmTreeItem()
                {
                    Id = parrentId,
                    IsSelected = false,
                    IsDisabled = false,
                    IsCollapsed = false,
                    IsLeaf = false,
                    Name = "name",
                    Children = new List<VmTreeItem>
                    {
                        new VmTreeItem() {
                            Id = chidId,
                            IsSelected = false,
                            IsDisabled = false,
                            IsCollapsed = false,
                            IsLeaf = false,
                            Name = "name"
                        }
                    }
                }
            };
        }
    }
}
