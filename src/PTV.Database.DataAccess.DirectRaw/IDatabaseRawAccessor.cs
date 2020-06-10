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
using System.Threading.Tasks;

namespace PTV.Database.DataAccess.DirectRaw
{
    public interface IDatabaseRawAccessor
    {
        IList<T> SelectList<T>(string query, object paramObject);
        T SelectOne<T>(string query, object paramObject);
        void Command(string command, object paramObject);
        void Save();
        Task<List<T>> SelectListAsync<T>(string query, object paramObject);
        Task<T> SelectOneAsync<T>(string query, object paramObject);
        void CommandAsync(string command, object paramObject);

        IList<T0> SelectListWithInclude<T0, T1, T2, T3, T4, T5>(string query, object paramObject,
            Func<T0, T1, T2, T3, T4, T5, T0> connectFunc, string splitOn);

        Task<IList<T0>> SelectListWithIncludeAsync<T0, T1, T2, T3, T4, T5>(string query, object paramObject,
            Func<T0, T1, T2, T3, T4, T5, T0> connectFunc, string splitOn);
    }
}
