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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IOntologyTermDataCache), RegisterType.Singleton)]
    internal class OntologyTermDataCache : LiveDataCache<Guid, OntologyTerm>, IOntologyTermDataCacheInternal, IOntologyTermDataCache
    {
        private const int SearchResultLimit = 50;
        
        private Dictionary<string, OntologyTerm> uris = new Dictionary<string, OntologyTerm>();
        private Dictionary<Guid, OntologyTerm> ontologyIds = new Dictionary<Guid, OntologyTerm>();
        private List<OntologyTermName> ontologyNames = new List<OntologyTermName>();
        private Dictionary<string, List<OntologyTerm>> exactMatches = new Dictionary<string, List<OntologyTerm>>();
        
        private readonly IResolveManager resolveManager;

        public OntologyTermDataCache(IResolveManager resolveManager)
        {
            MinRefreshInterval = 15;
            this.resolveManager = resolveManager;
        }
        
        protected override LiveCacheResult<Guid, OntologyTerm> LoadData()
        {
            if (!ShouldBeRefreshed) return null;
            resolveManager.RunInScope((rm) =>
            {
                var contextManager = rm.Resolve<IContextManager>();
                var cacheBuildTime = CacheBuildTime;
                contextManager.ExecuteIsolatedReader(unitOfWork =>
                {
                    var rep = unitOfWork.CreateRepository<IOntologyTermRepository>();
                    var allTermQuery = rep.AllPure();
                    var lastestChange = allTermQuery.OrderByDescending(i => i.Modified).Select(i => i.Modified).FirstOrDefault();
                    if (lastestChange <= cacheBuildTime)
                    {
                        return (Dictionary<Guid, OntologyTerm>)null;
                    }
                    cacheBuildTime = lastestChange;
                    this.ontologyIds = allTermQuery.Where(i => i.IsValid).Include(i => i.OntologyTermExactMatches).ThenInclude(i => i.ExactMatch).Include(i => i.Names).ToList().ToDictionary(i => i.Id, i => i);
                    this.CacheBuildTime = cacheBuildTime;
                    this.uris = this.ontologyIds.Values.DistinctBy(i => i.Uri).ToDictionary(i => i.Uri, i => i);
                    this.ontologyNames = this.ontologyIds.Values.SelectMany(i => i.Names).OrderBy(i => i.Name).Select(i => new OntologyTermName(){ Name = i.Name.ToLower(), OntologyTerm = i.OntologyTerm, OntologyTermId = i.OntologyTermId, LocalizationId = i.LocalizationId}).ToList();
                    this.exactMatches = this.ontologyIds.Values.SelectMany(i => i.OntologyTermExactMatches).Select(i => i.ExactMatch).DistinctBy(i => i.Uri)
                        .ToDictionary(i => i.Uri, i => i.OntologyTermExactMatches.Select(j => j.OntologyTerm).ToList());
                    return null;
                });
            });
            return null;
        }

        public List<OntologyTerm> SearchByName(string name, List<Guid> languagesIds = null)
        {
            GetData();
            name = name.ToLower();
            return ontologyNames.Where(i => i.Name.Contains(name) && (languagesIds.IsNullOrEmpty() || languagesIds.Contains(i.LocalizationId))).Select(i => i.OntologyTerm).DistinctBy(i => i.Id).Take(SearchResultLimit).ToList();
        }

        public override void InitRefreshCache()
        {
            LoadData();
        }

        public IEnumerable<string> Uris
        {
            get
            {
                GetData();
                return uris.Keys;
            }
        }

        public bool UriExists(string uri)
        {
            GetData();
            return uris.ContainsKey(uri);
        }

        public IList<string> HasUris(IEnumerable<string> toCheckUris)
        {
            GetData();
            return this.uris.Keys.Intersect(toCheckUris).ToList();
        }

        public HashSet<string> GetOntologyUrisForExactMatches(IEnumerable<string> exactMatchesUris)
        {
            GetData();
            return exactMatchesUris.Select(i => exactMatches[i]).SelectMany(i => i).Select(i => i.Uri).ToHashSet();
        }
    }
}