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
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IIndustrialClassCacheInternal), RegisterType.Singleton)]
    internal class IndustrialClassDataCache : LiveDataCache<Guid, IndustrialClass>, IIndustrialClassCacheInternal
    {
        private const string INVALID_CODE = "X";
        private readonly IContextManager contextManager;
        private Dictionary<string, IndustrialClass> dataByUri;
        private List<IGrouping<(string Name, Guid LanguageId), IndustrialClass>> dataByName;
        private Dictionary<Guid?, List<IndustrialClass>> childrenByParentIds;
        private readonly ITreeTools treeTools;
        private List<IndustrialClass> dataTree;
        private List<IndustrialClass> topLevel;
        private Dictionary<Guid, IndustrialClass> lastLevel;

        public IndustrialClassDataCache(IContextManager contextManager, ITreeTools treeTools)
        {
            this.contextManager = contextManager;
            this.treeTools = treeTools;
        }
        
        protected override void LoadData()
        {
            contextManager.ExecuteIsolatedReader(unitOfWork =>
            {
                var industrialClassRepo = unitOfWork.CreateRepository<IIndustrialClassRepository>();
                var allClassesQuery = industrialClassRepo.All()
                    .Include(i => i.Names).ThenInclude(i => i.Localization)
                    .Where(i => i.IsValid && i.Code != INVALID_CODE);
                
                var lastChange = allClassesQuery.OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                if (lastChange <= CacheBuildTime)
                {
                    return;
                }

                CacheBuildTime = lastChange;
                this.Data = allClassesQuery.ToDictionary(x => x.Id);
            });

            this.lastLevel = Data.Where(x => !string.IsNullOrEmpty(x.Value.Code)  && x.Value.Code.Trim().Length == 5).ToDictionary(x => x.Key, x => x.Value);
            this.dataByUri = lastLevel.Values.DistinctBy(x => x.Uri).ToDictionary(x => x.Uri);
            this.dataByName = lastLevel.Values.SelectMany(x => x.Names)
                .GroupBy(x => (Name: x.Name.ToLower(), LanguageId: x.LocalizationId), x => x.IndustrialClass).ToList();
            this.childrenByParentIds = Data.Values.Where(x => x.ParentId != null).GroupBy(x => x.ParentId)
                .ToDictionary(x => x.Key, x => x.ToList());
            this.dataTree = treeTools.LoadFintoTree(Data.Values.ToList());
            this.topLevel = treeTools.LoadFintoTree(Data.Values.ToList(), 1);
        }

        public IEnumerable<string> Uris
        {
            get
            {
                GetData();
                return dataByUri.Keys.ToList();
            }
        }

        public bool UriExists(string uri)
        {
            GetData();
            return dataByUri.ContainsKey(uri);
        }

        public IList<string> HasUris(IEnumerable<string> toCheckUris)
        {
            GetData();
            return this.dataByUri.Keys.Intersect(toCheckUris).ToList();
        }

        public List<IndustrialClass> SearchByName(string name, string orderLanguageCode, List<Guid> languagesIds = null)
        {
            GetData();
            name = name.ToLower();
            return dataByName
                .Where(x => x.Key.Name.Contains(name) &&
                            (languagesIds.IsNullOrEmpty() || languagesIds.Contains(x.Key.LanguageId)))
                .OrderBy(x => x.Key.Name, StringComparer.Create(
                    new CultureInfo(string.IsNullOrEmpty(orderLanguageCode)
                        ? DomainConstants.DefaultLanguage
                        : orderLanguageCode), true))
                .SelectMany(x => x)
                .DistinctBy(x => x.Id)
                .Take(SearchResultLimit)
                .ToList();
        }

        public List<IndustrialClass> SearchById(Guid id, bool searchByParentIds = true)
        {
            GetData();
            var result = new List<IndustrialClass>();

            if (Data.TryGetValue(id, out var item))
            {
                result.Add(item);
            }

            if (searchByParentIds && childrenByParentIds.TryGetValue(id, out var children))
            {
                result.AddRange(children);
            }

            return result;
        }

        public List<IndustrialClass> GetAllValid()
        {
            return GetData().Values.ToList();
        }

        public List<IndustrialClass> GetTopLevel()
        {
            GetData();
            return topLevel;
        }

        public IndustrialClass GetByUri(string uri)
        {
            GetData();
            return dataByUri.TryGetOrDefault(uri);
        }

        public List<IndustrialClass> GetAllForLastLevel()
        {
            GetData();
            return lastLevel.Values.ToList();
        }

        public List<IndustrialClass> GetTree()
        {
            GetData();
            return dataTree;
        }
    }
}
