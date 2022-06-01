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

using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(ITargetGroupCacheInternal), RegisterType.Singleton)]
    internal class TargetGroupCache : LiveDataCache<Guid, TargetGroup>, ITargetGroupCacheInternal
    {
        private readonly IResolveManager resolveManager;
        private readonly ITreeTools treeTools;
        private Dictionary<Guid, TargetGroup> targetGroupIds;
        private Dictionary<string, TargetGroup> uris;
        private Dictionary<string, TargetGroup> yuris;
        private HashSet<string> oldAndNewUris;
        private List<TargetGroupName> targetGroupNames;
        private List<TargetGroup> targetGroupTree;
        private Dictionary<Guid?, List<TargetGroup>> childGroupsByParentIds;
        private List<TargetGroup> topTargetGroups;

        public TargetGroupCache(IResolveManager resolveManager, ITreeTools treeTools)
        {
            MinRefreshInterval = 15;
            this.resolveManager = resolveManager;
            this.treeTools = treeTools;
        }

        protected override bool HasNewData(IUnitOfWork unitOfWork)
        {
            var lastTargetGroupUpdate = unitOfWork.CreateRepository<ITargetGroupRepository>().All()
                .Max(x => x.Modified);
            var lastNameUpdate = unitOfWork.CreateRepository<ITargetGroupNameRepository>().All().Max(x => x.Modified);

            var maxTime = CoreExtensions.Max(lastTargetGroupUpdate, lastNameUpdate);
            var hasNewData = maxTime > CacheBuildTime;
            if (hasNewData)
            {
                CacheBuildTime = maxTime;
            }

            return hasNewData;
        }

        protected override void LoadData()
        {
            resolveManager.RunInScope((rm) =>
            {
                var contextManager = rm.Resolve<IContextManager>();

                if (!contextManager.ExecuteIsolatedReader(HasNewData)) return;

                contextManager.ExecuteIsolatedReader(unitOfWork =>
                {
                    var rep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                    var allTermQuery = rep.AllPure()
                        .Include(i => i.Names)
                        .ThenInclude(i => i.Localization)
                        .Where(i => i.IsValid)
                        .OrderBy(x => x.Label);

                    this.targetGroupIds = allTermQuery.ToDictionary(i => i.Id, i => i);
                    this.childGroupsByParentIds = allTermQuery.Where(i => i.ParentId != null).ToList().GroupBy(i => i.ParentId)
                        .ToDictionary(i => i.Key, i => i.ToList());
                    this.uris = this.targetGroupIds.Values.DistinctBy(i => i.Uri).ToDictionary(i => i.Uri, i => i);
                    this.yuris = this.targetGroupIds.Values.DistinctBy(i => i.YUri).ToDictionary(i => i.YUri, i => i);
                    this.oldAndNewUris = this.targetGroupIds.Values
                        .SelectMany(x => new List<string>{ x.Uri, x.YUri })
                        .ToHashSet();
                    this.targetGroupNames = this.targetGroupIds.Values.SelectMany(i => i.Names).OrderBy(i => i.Name)
                        .Select(i => new TargetGroupName
                        {
                            Name = i.Name.ToLower(),
                            TargetGroup = i.TargetGroup,
                            TargetGroupId = i.TargetGroupId,
                            LocalizationId = i.LocalizationId
                        }).ToList();
                    var allTermList = allTermQuery.ToList();
                    this.targetGroupTree = treeTools.LoadFintoTree(allTermList);
                    this.topTargetGroups = treeTools.LoadFintoTree(allTermList, 1);
                });
            });
        }

        public IEnumerable<string> Uris
        {
            get
            {
                LoadData();
                return oldAndNewUris;
            }
        }

        public bool UriExists(string uri)
        {
            GetData();
            return this.oldAndNewUris.Contains(uri);
        }

        public IList<string> HasUris(IEnumerable<string> toCheckUris)
        {
            GetData();
            return toCheckUris.Where(x => this.oldAndNewUris.Contains(x)).ToList();
        }

        public List<TargetGroup> SearchByName(string name, string orderLanguageCode, List<Guid> languagesIds = null)
        {
            GetData();
            name = name.ToLower();
            return targetGroupNames
                .Where(i => i.Name.Contains(name) &&
                            (languagesIds.IsNullOrEmpty() || languagesIds.Contains(i.LocalizationId)))
                .OrderBy(x => x.Name, StringComparer.Create(
                    new CultureInfo(string.IsNullOrEmpty(orderLanguageCode)
                        ? DomainConstants.DefaultLanguage
                        : orderLanguageCode), true))
                .Select(i => i.TargetGroup).DistinctBy(i => i.Id).Take(SearchResultLimit).ToList();
        }

        public List<TargetGroup> SearchById(Guid id, bool includeParentIds = true)
        {
            var result = new List<TargetGroup>();
            if (targetGroupIds.TryGetValue(id, out var targetGroup))
            {
                result.Add(targetGroup);
            }

            if (includeParentIds && childGroupsByParentIds.TryGetValue(id, out var targetGroups))
            {
                result.AddRange(targetGroups);
            }

            return result;
        }

        public List<TargetGroup> GetAllValid()
        {
            GetData();
            return targetGroupIds.Values.ToList();
        }

        public List<TargetGroup> GetTree()
        {
            GetData();
            return this.targetGroupTree;
        }

        public TargetGroup GetByUri(string uri)
        {
            GetData();
            return uris.TryGetOrDefault(uri) ?? yuris.TryGetOrDefault(uri);
        }

        public DateTime GetLastUpdate()
        {
            return CacheBuildTime;
        }

        public List<TargetGroup> GetTopLevel()
        {
            GetData();
            return topTargetGroups;
        }

        public const string CitizenCode = "KR1";
        public const string EnterpriseCode = "KR2";
        public const string AuthoritiesCode = "KR3";
    }
}