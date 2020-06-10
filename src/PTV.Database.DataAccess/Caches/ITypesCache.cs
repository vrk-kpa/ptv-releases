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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Caches
{
    internal interface ITypesCache : IEntityCacheRefreshable
    {
        bool Compare<T>(Guid? typeId, string filterBy) where T : IType;
        Guid Get<T>(string key) where T : IType;
        Guid Get<T>(string key, string defaultKey) where T : IType;

        string GetByValue<T>(Guid id) where T : IType;

        ICollection<VmType> GetCacheData(string cacheName);
        ITypeCache GetCache(string cacheName);

        ICollection<VmType> GetCacheData<T>() where T : IType;
    }

    internal interface ITypeCache : IEntityCache<string, Guid>
    {
        bool Compare(Guid typeId, string filterBy);
        ICollection<VmType> GetCompleteStuct();
    }
}
