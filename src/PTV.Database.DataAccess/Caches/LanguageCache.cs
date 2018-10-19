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
    [RegisterService(typeof(IInternalLanguageCache), RegisterType.Singleton)]
    [RegisterService(typeof(IFrameworkLanguageCache), RegisterType.Singleton)]
    internal class LanguageCache : ILanguageCache, IInternalLanguageCache, IFrameworkLanguageCache
    {
        private readonly IResolveManager resolveManager;
        private ITranslationEntity translationManager;
        
        private readonly Dictionary<string, Guid> cacheData;
        private List<VmLanguage> cachedLanguages;
        
        public LanguageCache(ICacheBuilder cacheBuilder, IResolveManager resolveManager, ITranslationEntity translationManager)
        {
            this.resolveManager = resolveManager;
            this.translationManager = translationManager;
            cacheData = cacheBuilder.BuildCache<Language, string, Guid>(i => i.Code, i => i.Id);
        }

        private List<string> _allowedLanguageCodes;
        public List<string> AllowedLanguageCodes
        {
            get
            {
                if (cachedLanguages == null)
                {
                    BuildCachedLanguages();
                }
                
                return _allowedLanguageCodes;
            }
        }

        private List<Guid> _allowedLanguageIds;
        public List<Guid> AllowedLanguageIds
        {
            get
            {
                if (cachedLanguages == null)
                {
                    BuildCachedLanguages();
                }
                
                return _allowedLanguageIds;
            }
        }
        
        private List<string> _translationOrderLanguageCodes;
        public List<string> TranslationOrderLanguageCodes
        {
            get
            {
                if (cachedLanguages == null)
                {
                    BuildCachedLanguages();
                }
                
                return _translationOrderLanguageCodes;
            }
        }

        private List<Guid> _translationOrderLanguageIds;
        public List<Guid> TranslationOrderLanguageIds
        {
            get
            {
                if (cachedLanguages == null)
                {
                    BuildCachedLanguages();
                }
                
                return _translationOrderLanguageIds;
            }
        }

        private List<string> _languageCodes;
        public List<string> LanguageCodes
        {
            get
            {
                if (cachedLanguages == null)
                {
                    BuildCachedLanguages();
                }

                return _languageCodes;
            }
        }

        private void BuildCachedLanguages()
        {
            var contextManager = resolveManager.Resolve<IContextManager>();
            cachedLanguages = contextManager.ExecuteIsolatedReader(unitOfWork =>
            {
                var langRep = unitOfWork.CreateRepository<ILanguageRepository>();
                return translationManager.TranslateAll<Language, VmLanguage>(langRep.All().Where(x => x.IsValid).OrderBy(x => x.OrderNumber)).ToList();
            });
            
            _allowedLanguageIds = cachedLanguages.Where(x => x.IsForData).Select(x => x.Id.Value).OrderBy(x => x).ToList();
            _allowedLanguageCodes = cachedLanguages.Where(x => x.IsForData).Select(x => x.Code).ToList();
            _translationOrderLanguageIds = cachedLanguages.Where(x => x.IsForTranslationOrder && x.IsForData).Select(x => x.Id.Value).OrderBy(x => x).ToList();
            _translationOrderLanguageCodes = cachedLanguages.Where(x => x.IsForTranslationOrder && x.IsForData).Select(x => x.Code).ToList();
            _languageCodes = cachedLanguages.Select(x => x.Code).ToList();
        }

        public LanguageCache(Dictionary<string, Guid> cacheData)
        {
            this.cacheData = cacheData;
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

        public bool Exists(string key)
        {
            return this.cacheData.ContainsKey(key);
        }

        public Guid Get(string key)
        {
            return this.cacheData[key];
        }

        public string GetByValue(Guid id)
        {
            return this.cacheData.FirstOrDefault(x => x.Value == id).Key;
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

        public bool Exists(Guid key)
        {
            return cacheData.ContainsKey(key);
        }
    }


    interface IInternalLanguageCache : IEntityCache<string, Guid>
    {
        T Filter<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable;
        IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable;
    }

    internal interface ILanguageOrderCache : IEntityCache<Guid, int>
    {
//        bool Filter(Guid languageId, LanguageCode filterBy);

//        T Filter<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable;

//        IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable;
    }
}
