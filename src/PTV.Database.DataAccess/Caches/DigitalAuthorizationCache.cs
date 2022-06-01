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
    [RegisterService(typeof(IDigitalAuthorizationCacheInternal), RegisterType.Singleton)]
    internal class DigitalAuthorizationCache : LiveDataCache<Guid, DigitalAuthorization>, IDigitalAuthorizationCacheInternal
    {
        private readonly IContextManager contextManager;
        private Dictionary<string, DigitalAuthorization> dataByUri;
        private List<IGrouping<(string Name, Guid LanguageId), DigitalAuthorization>> dataByName;
        private Dictionary<Guid?, List<DigitalAuthorization>> childrenByParentIds;
        private readonly ITreeTools treeTools;
        private List<DigitalAuthorization> dataTree;
        private List<DigitalAuthorization> topLevel;

        public DigitalAuthorizationCache(IContextManager contextManager, ITreeTools treeTools)
        {
            this.contextManager = contextManager;
            this.treeTools = treeTools;
        }

        protected override bool HasNewData(IUnitOfWork unitOfWork)
        {
            var lastDigitalAuthUpdate = unitOfWork.CreateRepository<IDigitalAuthorizationRepository>().All()
                .Max(x => x.Modified);
            var lastNameUpdate = unitOfWork.CreateRepository<IDigitalAuthorizationNameRepository>().All()
                .Max(x => x.Modified);

            var maxTime = CoreExtensions.Max(lastDigitalAuthUpdate, lastNameUpdate);
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
                
                var digitalAuthorizationRepo = unitOfWork.CreateRepository<IDigitalAuthorizationRepository>();
                var allAuthorizationsQuery = digitalAuthorizationRepo.All()
                    .Include(i => i.Names).ThenInclude(i => i.Localization)
                    .Where(i => i.IsValid);
                
                this.Data = allAuthorizationsQuery.ToDictionary(x => x.Id);
            });

            this.dataByUri = Data.Values.ToDictionary(x => x.Uri);
            this.dataByName = Data.Values.SelectMany(x => x.Names)
                .GroupBy(x => (Name: x.Name.ToLower(), LanguageId: x.LocalizationId), x => x.DigitalAuthorization).ToList();
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

        public List<DigitalAuthorization> SearchByName(string name, string orderLanguageCode, List<Guid> languagesIds = null)
        {
            GetData();
            name = name.ToLower();
            return dataByName
                .Where(x => x.Key.Name.Contains(name) && (languagesIds.IsNullOrEmpty() || languagesIds.Contains(x.Key.LanguageId)))
                .OrderBy(x => x.Key.Name, StringComparer.Create(
                    new CultureInfo(string.IsNullOrEmpty(orderLanguageCode)
                        ? DomainConstants.DefaultLanguage
                        : orderLanguageCode), true))
                .SelectMany(x => x)
                .DistinctBy(x => x.Id)
                .Take(SearchResultLimit)
                .ToList();
        }

        public List<DigitalAuthorization> SearchById(Guid id, bool searchByParentIds = true)
        {
            GetData();
            var result = new List<DigitalAuthorization>();

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

        public List<DigitalAuthorization> GetAsFintoTree()
        {
            // Same logic as for the old UI. Top level nodes have no parent
            return treeTools.LoadFintoTree(GetTree());
        }

        public List<DigitalAuthorization> GetAllValid()
        {
            return GetData().Values.ToList();
        }

        public List<DigitalAuthorization> GetTree()
        {
            GetData();
            return dataTree;
        }

        public List<DigitalAuthorization> GetTopLevel()
        {
            GetData();
            return topLevel;
        }

        public DigitalAuthorization GetByUri(string uri)
        {
            GetData();
            return dataByUri.TryGetOrDefault(uri);
        }

        public DateTime GetLastUpdate()
        {
            return CacheBuildTime;
        }
    }
}
