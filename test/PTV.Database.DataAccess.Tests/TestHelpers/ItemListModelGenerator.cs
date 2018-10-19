﻿/**
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

namespace PTV.Database.DataAccess.Tests.TestHelpers
{
    public class ItemListModelGenerator
    {

        public List<TOut> Create<T, TOut>(int listCount, Func<int, T> createFunc = null, Action <int, T> initAction = null) where T : TOut, new()
        {
            return CreateImpl(listCount, createFunc, initAction).Cast<TOut>().ToList();
        }

        public List<T> Create<T>(int listCount, Func<int, T> createFunc = null, Action <int, T> initAction = null) where T : new()
        {
            return CreateImpl(listCount, createFunc, initAction);
        }
        private List<T> CreateImpl<T>(int listCount, Func<int, T> createFunc, Action <int, T> initAction) where T : new()
        {
            var list = new List<T>();
            for (int i = 0; i < listCount; i++)
            {
                var item = createFunc != null ? createFunc(i) : new T();
                list.Add(item);
                initAction?.Invoke(i, item);
            }
            return list;
        }
    }
}
