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
using System.Threading.Tasks;

namespace PTV.Framework
{
    public static class LinqExtensions
    {
        public static void ForEachNotNull<T>(this IEnumerable<T> enumerable, Action<T> action)
            where T : class
        {
            if (enumerable == null || action == null)
            {
                return;
            }

            foreach (T item in enumerable)
            {
                if (item != null)
                {
                    action(item);
                }
            }
        }
        
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null || action == null)
            {
                return;
            }

            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        public static void AddNullRange<T>(this List<T> list, IEnumerable<T> enumerable)
        {
            if (list == null || enumerable == null)
            {
                return;
            }

            list.AddRange(enumerable);
        }

        public static void AddExceptNull<T>(this List<T> list, T item)
        {
            if (list == null || item == null)
            {
                return;
            }

            list.Add(item);
        }

    }
}
