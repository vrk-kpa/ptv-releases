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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Utils
{
    [RegisterService(typeof(IDataServiceFetcher), RegisterType.Transient)]
    internal class DataServiceFetcher : IDataServiceFetcher
    {
        private readonly ITypesCache typesCache;
        private readonly IResolveManager resolveManager;
        private ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;

        public DataServiceFetcher(ICacheManager cacheManager, IResolveManager resolveManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            languageOrderCache = cacheManager.LanguageOrderCache;
            this.resolveManager = resolveManager;
        }

        public IList<TypeDataResult> Fetch(IEnumerable<string> typesName)
        {
            var namedServiceResolver = resolveManager.Resolve<INamedServiceResolver>();
            var unresolvedNames = new List<string>();
            var providedData = new List<TypeDataResult>();
            typesName.ForEach(typeName =>
            {
                var data = typesCache.GetCacheData(typeName);
                if (data == null)
                {
                    unresolvedNames.Add(typeName);
                    return;
                }
                providedData.Add(new TypeDataResult { Data = data.Cast<IVmBase>().ToList(), Name = typeName });
            });
            if (unresolvedNames.Any())
            {
                var contextManager = resolveManager.Resolve<IContextManager>();
                contextManager.ExecuteReader(unitOfWork =>
                {
                    providedData.AddRange(unresolvedNames.Select(typeName =>
                    {
                        var namedService = namedServiceResolver.Resolve(typeName);
                        var data = namedService?.Get(unitOfWork);
                        return new TypeDataResult {Data = data, Name = typeName};
                    }));
                });
            }
            return providedData;
        }

        public VmListItemsData<VmEnumType> FetchType<T>() where T : IType
        {
            var cachedData = typesCache.GetCacheData<T>();
             return new VmListItemsData<VmEnumType>(cachedData.Select(i => new VmEnumType
                 {
                    Access = EVmRestrictionFilterType.Allowed,
                    Id = i.Id,
                    Code = i.Code,
                    OrderNumber = i.OrderNumber,
                    Name = i.Names.OrderBy(x => languageOrderCache.Get(x.LocalizationId)).Select(x => x.Name).FirstOrDefault(),
                    Translation = new VmTranslationItem
                    {
                        Id = i.Id,
                        Texts = i.Names.ToDictionary(n => languageCache.GetByValue(n.LocalizationId), n => n.Name)
                    }

                })
                .ToList());
        }
    }
}
