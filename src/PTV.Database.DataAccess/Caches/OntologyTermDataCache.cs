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
using Microsoft.EntityFrameworkCore.Internal;
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
    [RegisterService(typeof(IOntologyTermDataCache), RegisterType.Singleton)]
    internal class OntologyTermDataCache : LiveDataCache<Guid, OntologyTerm>, IOntologyTermDataCacheInternal, IOntologyTermDataCache
    {
        private readonly IContextManager contextManager;
        private Dictionary<string, OntologyTerm> dataByUri;
        private Dictionary<string, List<OntologyTerm>> exactMatches;
        private List<IGrouping<(string Name, Guid LanguageId), OntologyTerm>> dataByName;
        private List<OntologyTerm> topLevel;

        public OntologyTermDataCache(IContextManager contextManager)
        {
            this.contextManager = contextManager;
        }

        protected override bool HasNewData(IUnitOfWork unitOfWork)
        {
            var lastOntologyUpdate = unitOfWork.CreateRepository<IOntologyTermRepository>().All().Max(x => x.Modified);
            var lastNameUpdate = unitOfWork.CreateRepository<IOntologyTermNameRepository>().All().Max(x => x.Modified);
            var lastParentUpdate = unitOfWork.CreateRepository<IOntologyTermParentRepository>().All()
                .Max(x => x.Modified);
            var lastConnectionUpdate = unitOfWork.CreateRepository<IOntologyTermExactMatchRepository>().All()
                .Max(x => x.Modified);
            var lastExactMatchUpdate = unitOfWork.CreateRepository<IExactMatchRepository>().All().Max(x => x.Modified);

            var maxTime = CoreExtensions.Max(lastOntologyUpdate, lastNameUpdate, lastParentUpdate, lastConnectionUpdate,
                lastExactMatchUpdate);
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

                var ontologyRepo = unitOfWork.CreateRepository<IOntologyTermRepository>();
                var exactMatchRepo = unitOfWork.CreateRepository<IOntologyTermExactMatchRepository>();
                var allTermQuery = ontologyRepo.All().Where(i => i.IsValid);

                var ontologiesWithNames = allTermQuery
                    .Include(x => x.Names)
                    .Include(x => x.Parents)
                    .Include(x => x.Children)
                    .AsNoTracking()
                    .ToDictionary(x => x.Id);
                var exactMatchesDict = exactMatchRepo.All()
                    .Include(x => x.ExactMatch)
                    .Include(x=>x.OntologyTerm)
                    .AsNoTracking()
                    .ToList()
                    .GroupBy(x => x.OntologyTermId)
                    .ToDictionary(x => x.Key, x => x.ToList());

                foreach (var ontology in ontologiesWithNames)
                {
                    ontology.Value.OntologyTermExactMatches = exactMatchesDict.TryGetOrDefault(ontology.Key);
                    foreach (var parent in ontology.Value.Parents)
                    {
                        parent.Parent = ontologiesWithNames.TryGetOrDefault(parent.ParentId);
                    }

                    foreach (var child in ontology.Value.Children)
                    {
                        child.Child = ontologiesWithNames.TryGetOrDefault(child.ChildId);
                    }
                }

                Data = ontologiesWithNames;
            });

            this.dataByUri = Data.Values.ToDictionary(x => x.Uri);
            this.dataByName = Data.Values.SelectMany(x => x.Names)
                .Where(x => x.Name != null)
                .GroupBy(x => (Name: x.Name.ToLower(), LanguageId: x.LocalizationId), x => x.OntologyTerm).ToList();
            this.exactMatches = this.Data.Values.Where(i => i.OntologyTermExactMatches != null)
                .SelectMany(i => i.OntologyTermExactMatches)
                .Select(i => i.ExactMatch)
                .DistinctBy(i => i.Uri)
                .ToDictionary(i => i.Uri, i => i.OntologyTermExactMatches.Select(j => j.OntologyTerm).ToList());
            this.topLevel = Data.Values.Where(x => !x.Parents.Any()).ToList();
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

        public HashSet<string> GetOntologyUrisForExactMatches(IEnumerable<string> exactMatchesUris)
        {
            GetData();
            return exactMatchesUris.Select(i => exactMatches[i]).SelectMany(i => i).Select(i => i.Uri).ToHashSet();
        }

        public List<OntologyTerm> SearchByName(string name, string orderLanguageCode, List<Guid> languagesIds = null)
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

        public List<OntologyTerm> SearchById(Guid id, bool searchByParentIds = true)
        {
            GetData();
            var result = new List<OntologyTerm>();

            if (Data.TryGetValue(id, out var item))
            {
                result.Add(item);
            }

            if (searchByParentIds && Data.TryGetValue(id, out var family))
            {
                result.AddRange(family.Children.Select(x => x.Child));
            }

            return result.Distinct().ToList();
        }

        public List<OntologyTerm> GetAllValid()
        {
            GetData();
            return Data.Values.ToList();
        }

        public OntologyTerm GetByUri(string uri)
        {
            GetData();
            return dataByUri.TryGetOrDefault(uri);
        }

        public List<OntologyTerm> GetOntologyTermsForExactMatches(IEnumerable<string> uris)
        {
            GetData();
            return exactMatches
                .Where(match => uris.Any(uri => string.Equals(uri, match.Key, StringComparison.CurrentCultureIgnoreCase)))
                .SelectMany(match => match.Value)
                .DistinctBy(x=>x.Id)
                .ToList();
        }

        public List<OntologyTerm> GetTopLevel()
        {
            GetData();
            return topLevel;
        }

        public List<OntologyTerm> GetTree()
        {
            return GetAllValid();
        }
    }
}
