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
    [RegisterService(typeof(ILifeEventCacheInternal), RegisterType.Singleton)]
    internal class LifeEventCache : LiveDataCache<Guid, LifeEvent>, ILifeEventCacheInternal
    {
        private readonly IContextManager contextManager;
        private Dictionary<string, LifeEvent> dataByUri;
        private List<IGrouping<(string Name, Guid LanguageId), LifeEvent>> dataByName;
        private Dictionary<Guid?, List<LifeEvent>> childrenByParentIds;
        private readonly ITreeTools treeTools;
        private List<LifeEvent> dataTree;
        private List<LifeEvent> topLevel;

        public LifeEventCache(IContextManager contextManager, ITreeTools treeTools)
        {
            this.contextManager = contextManager;
            this.treeTools = treeTools;
        }

        protected override bool HasNewData(IUnitOfWork unitOfWork)
        {
            var lastLifeEventUpdate = unitOfWork.CreateRepository<ILifeEventRepository>().All()
                .Max(x => x.Modified);
            var lastNameUpdate = unitOfWork.CreateRepository<ILifeEventNameRepository>().All()
                .Max(x => x.Modified);

            var maxTime = CoreExtensions.Max(lastLifeEventUpdate, lastNameUpdate);
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
                
                var lifeEventRepo = unitOfWork.CreateRepository<ILifeEventRepository>();
                var allEventsQuery = lifeEventRepo.All()
                    .Include(i => i.Names).ThenInclude(i => i.Localization)
                    .Where(i => i.IsValid);
                
                this.Data = allEventsQuery.ToDictionary(x => x.Id);
            });

            this.dataByUri = Data.Values.ToDictionary(x => x.Uri);
            this.dataByName = Data.Values.SelectMany(x => x.Names)
                .GroupBy(x => (Name: x.Name.ToLower(), LanguageId: x.LocalizationId), x => x.LifeEvent).ToList();
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

        public List<LifeEvent> SearchByName(string name, string orderLanguageCode, List<Guid> languagesIds = null)
        {
            GetData();
           name = name.Trim().ToLower();
            return dataByName
                .Where(x => x.Key.Name.Contains(name) &&
                            (languagesIds.IsNullOrEmpty() || languagesIds.Contains(x.Key.LanguageId)))
                .OrderBy(x => x.Key.Name, StringComparer.Create(
                    new CultureInfo(string.IsNullOrEmpty(orderLanguageCode)
                        ? DomainConstants.DefaultLanguage
                        : orderLanguageCode), true))
                .SelectMany(x => x)
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Label)
                .Take(SearchResultLimit)
                .ToList();
        }

        public List<LifeEvent> SearchById(Guid id, bool searchByParentIds = true)
        {
            GetData();
            var result = new List<LifeEvent>();

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

        public List<LifeEvent> GetAllValid()
        {
            return GetData().Values.ToList();
        }

        public List<LifeEvent> GetTopLevel()
        {
            GetData();
            return topLevel;
        }

        public List<LifeEvent> GetTree()
        {
            GetData();
            return dataTree;
        }

        public LifeEvent GetByUri(string uri)
        {
            GetData();
            return dataByUri.TryGetOrDefault(uri);
        }
    }
}
