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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(ITreeTools), RegisterType.Transient)]
    internal class TreeTools : ITreeTools
    {
        public List<T> LoadFintoTree<T>(List<T> inputQuery, int? levelsToLoad = null,
            ICollection<Guid?> ids = null)
            where T : class, IHierarchy<T>, IEntityIdentifier, IIsValid
        {
            if (levelsToLoad < 1)
            {
                return new List<T>();
            }

            var result = ids == null
                ? inputQuery.Where(x => x.ParentId == null).ToList()
                : inputQuery.Where(x => ids.Contains(x.ParentId)).ToList();

            LoadTreeRecursive(inputQuery, result, --levelsToLoad);
            return result;
        }

        private void LoadTreeRecursive<T>(List<T> everything, IEnumerable<T> parents, int? levelsToLoad) where T
            : class, IHierarchy<T>, IEntityIdentifier, IIsValid
        {
            if (levelsToLoad < 1)
            {
                return;
            }

            foreach (var parent in parents)
            {
                var children = everything.Where(x => x.ParentId == parent.Id).ToList();

                if (children.Count <= 0)
                {
                    continue;
                }

                parent.Children = children;
                foreach (var child in children)
                {
                    child.Parent = parent;
                }
                LoadTreeRecursive(everything, children, --levelsToLoad);
            }
        }

        public List<IVmTreeItem> ReplaceLevelByLevel(IEnumerable<IVmTreeItem> tree,
            int replaceChildrenInLevel, int byChildrenInLevel)
        {
            var tempTree = tree.ToList();
            if (replaceChildrenInLevel > 0)
            {
                foreach (var item in tempTree)
                {
                    item.Children = ReplaceLevelByLevel(item.Children, replaceChildrenInLevel - 1, byChildrenInLevel - 1);
                }

                return tempTree;
            }

            if (byChildrenInLevel <= 1) return tempTree.SelectMany(x => x.Children).ToList();

            var joinedChildren = new List<IVmTreeItem>();

            return tempTree.Aggregate(joinedChildren, (current, item) => current.Concat(ReplaceLevelByLevel(item.Children, replaceChildrenInLevel, byChildrenInLevel - 1)).ToList());
        }
    }


}
