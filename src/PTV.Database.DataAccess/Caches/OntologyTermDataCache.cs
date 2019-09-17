﻿/**
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
        
        protected override void LoadData()
        {
            contextManager.ExecuteIsolatedReader(unitOfWork =>
            {
                var ontologyRepo = unitOfWork.CreateRepository<IOntologyTermRepository>();
                var allTermQuery = ontologyRepo.All()
                    .Include(i => i.OntologyTermExactMatches).ThenInclude(i => i.ExactMatch)
                    .Include(i => i.Names)
                    .Where(i => i.IsValid);
                var lastChange = allTermQuery.OrderByDescending(i => i.Modified).Select(i => i.Modified)
                    .FirstOrDefault();
                if (lastChange <= CacheBuildTime)
                {
                    return;
                }

                CacheBuildTime = lastChange;

                var familyQuery = allTermQuery
                    .Include(i => i.Parents).ThenInclude(i => i.Parent).ThenInclude(i => i.Names)
                    .Include(i => i.Children).ThenInclude(i => i.Child).ThenInclude(i => i.Names);
                Data = familyQuery.ToDictionary(x => x.Id);
            });

            this.dataByUri = Data.Values.ToDictionary(x => x.Uri);
            this.dataByName = Data.Values.SelectMany(x => x.Names)
                .Where(x => x.Name != null)
                .GroupBy(x => (Name: x.Name.ToLower(), LanguageId: x.LocalizationId), x => x.OntologyTerm).ToList();
            this.exactMatches = this.Data.Values.SelectMany(i => i.OntologyTermExactMatches)
                .Select(i => i.ExactMatch)
                .DistinctBy(i => i.Uri)
                .ToDictionary(i => i.Uri, i => i.OntologyTermExactMatches.Select(j => j.OntologyTerm).ToList());
            this.topLevel = Data.Values.Where(x => !x.Parents.Any()).ToList().ToList();
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
                .Distinct()
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