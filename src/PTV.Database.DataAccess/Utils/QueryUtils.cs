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
using PTV.Framework;

namespace PTV.Database.DataAccess.Utils
{
    public static class QueryUtils
    {
        public static PagedData<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber)
        {
            var data = query.Skip(pageNumber.PositiveOrZero() * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems + 1).ToList();
            return new PagedData<T>()
            {
                Data = data.Take(CoreConstants.MaximumNumberOfAllItems).ToList(),
                MoreAvailable = data.Count > CoreConstants.MaximumNumberOfAllItems
            };
        }
    }

    public class PagedData<T>
    {
        public List<T> Data { get; set; }
        public bool MoreAvailable { get; set; }
    }
}
