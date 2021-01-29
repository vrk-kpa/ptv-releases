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
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IServiceClassCacheInternal), RegisterType.Singleton)]
    internal class ServiceClassCache : LiveDataCache<Guid, ServiceClass>, IServiceClassCacheInternal
    {
        private readonly IContextManager contextManager;
        private Dictionary<string, ServiceClass> dataByUri;
        private Dictionary<(string Name, Guid LanguageId), ServiceClass> dataByName;
        private Dictionary<Guid?, List<ServiceClass>> childrenByParentIds;
        private readonly ITreeTools treeTools;
        private List<ServiceClass> dataTree;
        private List<ServiceClass> topLevel;

        public ServiceClassCache(IContextManager contextManager, ITreeTools treeTools)
        {
            this.contextManager = contextManager;
            this.treeTools = treeTools;
        }

        protected override bool HasNewData(IUnitOfWork unitOfWork)
        {
            var lastServiceClassUpdate = unitOfWork.CreateRepository<IServiceClassRepository>().All()
                .Max(x => x.Modified);
            var lastNameUpdate = unitOfWork.CreateRepository<IServiceClassNameRepository>().All().Max(x => x.Modified);
            var lastDescriptionUpdate = unitOfWork.CreateRepository<IServiceClassDescriptionRepository>().All()
                .Max(x => x.Modified);

            var maxTime = CoreExtensions.Max(lastServiceClassUpdate, lastNameUpdate, lastDescriptionUpdate);
            var hasNewData = maxTime > CacheBuildTime;
            if (hasNewData)
            {
                CacheBuildTime = maxTime;
            }

            return hasNewData;
        }

        protected override void LoadData()
        {
            contextManager.ExecuteIsolatedReader(unitOfWork =>
            {
                if (!HasNewData(unitOfWork)) return;
                
                var serviceClassRepo = unitOfWork.CreateRepository<IServiceClassRepository>();
                var allClassesQuery = serviceClassRepo.All()
                    .Include(i => i.Names).ThenInclude(i => i.Localization)
                    .Include(i => i.Descriptions).ThenInclude(i => i.Localization)
                    .Where(i => i.IsValid);
                
                this.Data = allClassesQuery.ToDictionary(x => x.Id);
            });

            this.dataByUri = Data.Values.ToDictionary(x => x.Uri);
            this.dataByName = Data.Values.SelectMany(x => x.Names)
                .GroupBy(x => (Name: x.Name.ToLower(), LanguageId: x.LocalizationId), x => x.ServiceClass)
                .ToDictionary(x => x.Key, x => x.Single());
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

        public List<ServiceClass> SearchByName(string name, string orderLanguageCode, List<Guid> languagesIds = null)
        {
            GetData();
            name = name.ToLower();

            return dataByName
                .Where(x => x.Key.Name.Contains(name)  && (languagesIds.IsNullOrEmpty() || languagesIds.Contains(x.Key.LanguageId))
                            || x.Value.Descriptions.Any(y => y.Description.Contains(name) && (languagesIds.IsNullOrEmpty() || languagesIds.Contains(y.LocalizationId))))
                .OrderBy(x => x.Key.Name, StringComparer.Create(
                        new CultureInfo(string.IsNullOrEmpty(orderLanguageCode)
                            ? DomainConstants.DefaultLanguage
                            : orderLanguageCode), true))
                .Select(x => x.Value)
                .DistinctBy(x => x.Id)
                .Take(SearchResultLimit)
                .ToList();
        }

        public List<ServiceClass> SearchById(Guid id, bool searchByParentIds = true)
        {
            GetData();
            var result = new List<ServiceClass>();

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

        public List<ServiceClass> GetAllValid()
        {
            return GetData().Values.ToList();
        }

        public ServiceClass GetByUri(string uri)
        {
            GetData();
            return dataByUri.TryGetOrDefault(uri);
        }

        public List<string> HasTopLevelUris(IEnumerable<string> urisToCheck)
        {
            GetData();
            return this.topLevel.Select(x => x.Uri).Intersect(urisToCheck).ToList();
        }

        public List<ServiceClass> GetTopLevel()
        {
            GetData();
            return this.topLevel;
        }

        public List<ServiceClass> GetTree()
        {
            GetData();
            return dataTree;
        }
    }
}
