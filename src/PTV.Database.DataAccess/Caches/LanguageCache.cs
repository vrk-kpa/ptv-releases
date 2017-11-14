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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(ILanguageCache), RegisterType.Singleton)]
    internal class LanguageCache : ILanguageCache
    {
        private readonly Dictionary<string, Guid> cacheData;

        public LanguageCache(ICacheBuilder cacheBuilder)
        {
            cacheData = cacheBuilder.BuildCache<Language, string, Guid>(i => i.Code, i => i.Id);
        }

        public LanguageCache(Dictionary<string, Guid> cacheData)
        {
            this.cacheData = cacheData;
        }

        public bool Filter(Guid languageId, LanguageCode filterBy)
        {
            return languageId == this.cacheData[filterBy.ToString()];
        }

        public bool Filter(Guid languageId, Guid filterBy)
        {
            return languageId == filterBy;
        }

        public T Filter<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable
        {
            return filterable?.FirstOrDefault(i => Filter(i.LocalizationId, filterBy));
        }

        public T Filter<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable
        {
            return filterable?.FirstOrDefault(i => Filter(i.LocalizationId, filterBy));
        }

        public IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable
        {
            return filterable?.Where(i => Filter(i.LocalizationId, filterBy));
        }

        public IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable
        {
            return filterable?.Where(i => Filter(i.LocalizationId, filterBy));
        }

        public Guid Get(string key)
        {
            return this.cacheData[key];
        }

        public Guid Get(LanguageCode key)
        {
            return this.cacheData[key.ToString()];
        }

        public string GetByValue(Guid id)
        {
            return this.cacheData.FirstOrDefault(x => x.Value == id).Key;
        }

        public LanguageCode GetByValueEnum(Guid key)
        {
            return GetByValue(key).Parse<LanguageCode>();
        }
    }
    [RegisterService(typeof(ILanguageOrderCache), RegisterType.Singleton)]
    internal class LanguageOrderCache : ILanguageOrderCache
    {
        private readonly Dictionary<Guid, int> cacheData;

        public LanguageOrderCache(ICacheBuilder cacheBuilder)
        {
            cacheData = cacheBuilder.BuildCache<Language, Guid, int>(i => i.Id, i => i.OrderNumber ?? int.MaxValue);
        }

        public LanguageOrderCache(Dictionary<Guid, int> cacheData)
        {
            this.cacheData = cacheData;
        }

        public int Get(Guid key)
        {
            return cacheData[key];
        }

        public Guid GetByValue(int id)
        {
            return cacheData.FirstOrDefault(x => x.Value == id).Key;
        }
    }

    internal interface ILanguageCache : IEntityCache<string, Guid>
    {
        bool Filter(Guid languageId, LanguageCode filterBy);
        bool Filter(Guid languageId, Guid filterBy);
        T Filter<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable;
        T Filter<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable;
        IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable;
        IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable;
        Guid Get(LanguageCode key);
        LanguageCode GetByValueEnum(Guid key);
    }
    internal interface ILanguageOrderCache : IEntityCache<Guid, int>
    {
//        bool Filter(Guid languageId, LanguageCode filterBy);

//        T Filter<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable;

//        IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable;
    }
}
