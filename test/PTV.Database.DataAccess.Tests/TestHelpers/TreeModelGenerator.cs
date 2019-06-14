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
using PTV.Database.DataAccess.Tests.Translators.Services;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Tests.TestHelpers
{
    public class TreeModelGenerator
    {
        private ItemListModelGenerator itemListModelGenerator;

        public TreeModelGenerator()
        {
            itemListModelGenerator = new ItemListModelGenerator();
        }

        public VmTreeItem CreateTreeItem(TestTreeModelInit treeModel, int listCount = 5)
        {
            var tree = new VmTreeItem {Id = Guid.NewGuid()};
            switch (treeModel)
            {
                case TestTreeModelInit.Empty:
                    break;
                case TestTreeModelInit.List:
                    tree.Children = CreateTreeList(listCount, TestTreeModelInit.Empty);
                    break;
                case TestTreeModelInit.OneRootHierarchy:
                    tree.Children = CreateTreeList(1, TestTreeModelInit.List);
                    break;
                case TestTreeModelInit.MoreRootHierarchy:
                    tree.Children = CreateTreeList(3, TestTreeModelInit.List, TestTreeModelInit.OneRootHierarchy, TestTreeModelInit.MoreRootHierarchy);
                    break;

            }
            return tree;
        }

        public List<IVmTreeItem> CreateTreeList(int listCount, params TestTreeModelInit[] treeModel)
        {
            return itemListModelGenerator.Create<VmTreeItem, IVmTreeItem>(listCount, i => CreateTreeItem(treeModel[i%treeModel.Length]));
        }
    }

    public enum TestTreeModelInit
    {
        Empty,
        List,
        OneRootHierarchy,
        MoreRootHierarchy
    }
}
