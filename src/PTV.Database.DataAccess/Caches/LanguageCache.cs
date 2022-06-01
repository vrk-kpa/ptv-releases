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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(ILanguageCache), RegisterType.Singleton)]
    [RegisterService(typeof(IFrameworkLanguageCache), RegisterType.Singleton)]
    internal class LanguageCache : IInternalLanguageCache
    {
        private List<Language> cachedLanguages;
        private CacheBuiltResultData<string, Language> cacheData;
        private readonly ICacheBuilder cacheBuilder;

        public LanguageCache(ICacheBuilder cacheBuilder)
        {
            RefreshableCaches.Add(this);
            this.cacheBuilder = cacheBuilder;
            CreateCache();
        }

        private void CreateCache()
        {
            cacheData = cacheBuilder.BuildCache(x => x.Code, x => x.Include(i => i.Names), cacheData);
            cachedLanguages = cacheData.Data.Values.OrderBy(i => i.OrderNumber).ToList();
            AllowedLanguageIds = cachedLanguages.Where(x => x.IsForData).Select(x => x.Id).ToList();
            AllowedLanguageCodes = cachedLanguages.Where(x => x.IsForData).Select(x => x.Code).ToList();
            TranslationOrderLanguageIds = cachedLanguages.Where(x => x.IsForTranslation && x.IsForData).Select(x => x.Id).ToList();
            TranslationOrderLanguageCodes = cachedLanguages.Where(x => x.IsForTranslation && x.IsForData).Select(x => x.Code).ToList();
            LanguageCodes = cachedLanguages.Select(x => x.Code).ToList();
        }

        public List<string> AllowedLanguageCodes { get; private set; }
        public List<Guid> AllowedLanguageIds { get; private set; }
        public List<string> TranslationOrderLanguageCodes { get; private set; }
        public List<Guid> TranslationOrderLanguageIds { get; private set; }
        public List<string> LanguageCodes { get; private set; }
        public DateTime GetLastUpdate()
        {
            return CoreExtensions.Max(cachedLanguages.Max(x => x.Modified),
                cachedLanguages.SelectMany(x => x.Names.Select(y => y.Modified)).Max());
        }

        public bool Filter(Guid languageId, Guid filterBy)
        {
            return languageId == filterBy;
        }

        public T Filter<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable
        {
            return filterable?.FirstOrDefault(i => Filter(i.LocalizationId, filterBy));
        }

        public IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable
        {
            return filterable?.Where(i => Filter(i.LocalizationId, filterBy));
        }

        public List<Language> GetAll()
        {
            return cachedLanguages;
        }

        public bool Exists(string key)
        {
            return this.cacheData.Data.ContainsKey(key);
        }

        public Guid Get(string key)
        {
            return this.cacheData.Data[key].Id;
        }

        public string GetByValue(Guid id)
        {
            return this.cacheData.Data.FirstOrDefault(x => x.Value.Id == id).Key;
        }

        public void CheckAndRefresh()
        {
            CreateCache();
        }
    }
    [RegisterService(typeof(ILanguageOrderCache), RegisterType.Singleton)]
    internal class LanguageOrderCache : ILanguageOrderCache
    {
        private CacheBuiltResultData<Guid, int> cacheData;
        private readonly ICacheBuilder cacheBuilder;


        public LanguageOrderCache(ICacheBuilder cacheBuilder)
        {
            RefreshableCaches.Add(this);
            this.cacheBuilder = cacheBuilder;
            CreateCache();
        }

        private void CreateCache()
        {
            cacheData = cacheBuilder.BuildCache<Language, Guid, int>(i => i.Id, i => i.OrderNumber ?? int.MaxValue, cacheData);
        }

        public int Get(Guid key)
        {
            return cacheData.Data[key];
        }

        public Guid GetByValue(int id)
        {
            return cacheData.Data.FirstOrDefault(x => x.Value == id).Key;
        }

        public bool Exists(Guid key)
        {
            return cacheData.Data.ContainsKey(key);
        }

        public void CheckAndRefresh()
        {
            CreateCache();
        }
    }

    interface IInternalLanguageCache : ILanguageCache
    {
        T Filter<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable;
        IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable;
        List<Language> GetAll();
    }

    internal interface ILanguageOrderCache : IEntityCache<Guid, int>
    {
    }
}
