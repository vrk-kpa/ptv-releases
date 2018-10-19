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
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of list item data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmListItemsData{T}" />
    public class VmListItemsData<T> : List<T>, IVmListItemsData<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmListItemsData{T}"/> class.
        /// </summary>
        public VmListItemsData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmListItemsData{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public VmListItemsData(IEnumerable<T> collection) : base(collection)
        {
        }
    }

    /// <summary>
    /// View model of dictionary item data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmListItemsData{T}" />
    public class VmDictionaryItemsData<T> : Dictionary<string, T>, IVmDictionaryItemsData<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmListItemsData{T}"/> class.
        /// </summary>
        public VmDictionaryItemsData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmDictionaryItemsData{T}"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary whose elements are copied to the new list.</param>
        public VmDictionaryItemsData(IDictionary<string, T> dictionary) : base(dictionary)
        {
        }
    }
}
