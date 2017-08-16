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
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Domain.Logic.Finto
{
    [RegisterService(typeof(IndustrialClassLogic), RegisterType.Singleton)]
    public class IndustrialClassLogic
    {
        public List<VmIndustrialClassJsonItem> BuildTree(List<VmIndustrialClassJsonItem> list)
        {
            Dictionary<int, VmIndustrialClassJsonItem> parents = new Dictionary<int, VmIndustrialClassJsonItem>
            {
                { 1, null},
                { 2, null},
                { 3, null},
                { 4, null},
                { 5, null},
            };
            foreach (var item in list)
            {
                VmIndustrialClassJsonItem root = null;
                int parentLevel = item.Level - 1;
                parents[item.Level] = item;
                if (parentLevel > 0 && parentLevel < 5)
                {
                    root = parents[parentLevel];
                }
                if (root != null)
                {
                    if (root.Children == null)
                    {
                        root.Children = new List<VmIndustrialClassJsonItem>();
                    }
                    root.Children.Add(item);
                    //                    parent.Parents.Count.Should()
                    //                        .BeLessThan(2, $"{x} not in {parent.Parents.FirstOrDefault()}. Parent {item.Id}");
                    item.Parent = root.Code;
                }
            }
            return list.Where(x => string.IsNullOrEmpty(x.Parent)).ToList();
        }

    }
}
