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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Next.EnumTypes;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IIndustrialClassCacheInternal), RegisterType.Singleton)]
    internal class IndustrialClassDataCache : LiveDataCache<Guid, IndustrialClass>, IIndustrialClassCacheInternal
    {
        private const string INVALID_CODE = "X";
        private readonly IContextManager contextManager;
        private Dictionary<string, IndustrialClass> dataByUri;
        private Dictionary<string, IndustrialClass> dataByYUri;
        private HashSet<string> oldAndNewUris;
        private List<IGrouping<(string Name, Guid LanguageId), IndustrialClass>> dataByName;
        private Dictionary<Guid?, List<IndustrialClass>> childrenByParentIds;
        private readonly ITreeTools treeTools;
        private readonly ILanguageCache languageCache;
        private List<IndustrialClass> dataTree;
        private List<IndustrialClass> topLevel;
        private Dictionary<Guid, IndustrialClass> lastLevel;
        private List<IndustrialClassModel> data125Flat;

        public IndustrialClassDataCache(IContextManager contextManager, ITreeTools treeTools, ILanguageCache languageCache)
        {
            this.contextManager = contextManager;
            this.treeTools = treeTools;
            this.languageCache = languageCache;
        }

        protected override bool HasNewData(IUnitOfWork unitOfWork)
        {
            var lastIndustrialClassUpdate =
                unitOfWork.CreateRepository<IIndustrialClassRepository>().All().Max(x => x.Modified);
            var lastNameUpdate =
                unitOfWork.CreateRepository<IIndustrialClassNameRepository>().All().Max(x => x.Modified);
            var lastDescriptionUpdate =
                unitOfWork.CreateRepository<IIndustrialClassDescriptionRepository>().All().Max(x => x.Modified);

            var maxTime = CoreExtensions.Max(lastIndustrialClassUpdate, lastNameUpdate, lastDescriptionUpdate);
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

                var industrialClassRepo = unitOfWork.CreateRepository<IIndustrialClassRepository>();
                var allClassesQuery = industrialClassRepo.All()
                    .Where(i => i.IsValid && i.Code != INVALID_CODE)
                    .Include(i => i.Names).ThenInclude(i => i.Localization)
                    .Include(i => i.Descriptions).ThenInclude(i => i.Localization);

                this.Data = allClassesQuery
                    .ToDictionary(x => x.Id);

                var dataWithChildren = allClassesQuery.Include(x => x.Children).ToList();
                this.data125Flat = Build125FlatList(dataWithChildren);
            });

            this.lastLevel = Data.Where(x => !string.IsNullOrEmpty(x.Value.Code) && x.Value.Code.Trim().Length == 5).ToDictionary(x => x.Key, x => x.Value);
            this.dataByUri = lastLevel.Values.DistinctBy(x => x.Uri).ToDictionary(x => x.Uri);
            this.dataByYUri = lastLevel.Values.DistinctBy(x => x.Uri).ToDictionary(x => x.YUri);
            this.oldAndNewUris = lastLevel.Values.SelectMany(x => new List<string> {x.Uri, x.YUri}).ToHashSet();
            this.dataByName = lastLevel.Values.SelectMany(x => x.Names)
                .GroupBy(x => (Name: x.Name.ToLower(), LanguageId: x.LocalizationId), x => x.IndustrialClass).ToList();

            var dataValues = Data.Values.ToList();
            this.childrenByParentIds = dataValues.Where(x => x.ParentId != null).GroupBy(x => x.ParentId)
                .ToDictionary(x => x.Key, x => x.ToList());
            this.dataTree = treeTools.LoadFintoTree(dataValues);
            this.topLevel = treeTools.LoadFintoTree(dataValues, 1);
        }

        /// <summary>
        /// Returns just levels 1, 2 and 5 of the industrial class tree.
        /// Level 3 children are replaced by their level 5 descendants.
        /// Items are returned as a flat tree.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="childrenByParentIds"></param>
        /// <returns></returns>
        private List<IndustrialClassModel> Build125FlatList(List<IndustrialClass> input)
        {
            // How this algorithm works: Let's have a table representing individual levels of Industrial classes
            // LEVEL            PARENT ID           ID          CHILD(REN) ID(S)
            // 1                null                A           [ AB ]
            // 2                A                   AB          [ ABC ]
            // 3                AB                  ABC         [ ABCD ]
            // 4                ABC                 ABCD        [ ABCDE ]
            // 5                ABCD                ABCDE       []
            
            var result = new List<IndustrialClassModel>();
            // We know, that the number of characters in a code is equal to the IC level
            var icLevels = input.GroupBy(x => x.Code.Length).ToDictionary(x => x.Key, x => x.ToList());

            var firstLevel = icLevels[1];
            var secondLevel = icLevels[2];
            var thirdLevel = icLevels[3];
            var forthLevel = icLevels[4];
            var fifthLevel = icLevels[5];
            // We want to return the first level as it is
            result.AddRange(firstLevel.Select(x => x.ToModel(languageCache)));

            // In the second level, we need to replace the children from the third level with their descendants from
            // the fifth level. To do this quickly, we can utilize the forth level, which has reference both to the
            // third and fifth level. For more info, see the ReplaceChildren function.
            var adjustedSecondLevel = ReplaceChildren(secondLevel, forthLevel);
            result.AddRange(adjustedSecondLevel);

            // In the fifth level, we need to replace the parents from the forth level with their ancestors from
            // the second level. To do this quickly, we can utilize the third level, which has reference both to the
            // second and forth level. For more info, see the ReplaceParents function.
            var adjustedFifthLevel = ReplaceParents(fifthLevel, thirdLevel);
            result.AddRange(adjustedFifthLevel);

            return result;
        }

        /// <summary>
        /// Replaces parents of one level (usually the fifth) with their ancestors from another level (usually the second).
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        private IEnumerable<IndustrialClassModel> ReplaceParents(List<IndustrialClass> input, List<IndustrialClass> replacement)
        {
            // How this algorithm works: Let's have a table representing individual levels of Industrial classes
            // LEVEL            PARENT ID           ID          CHILD(REN) ID(S)
            // 1                null                A           [ AB ]
            // 2                A                   AB          [ ABC ]
            // 3                AB                  ABC         [ ABCD ]
            // 4                ABC                 ABCD        [ ABCDE ]
            // 5                ABCD                ABCDE       []
            foreach (var item in input)
            {
                // First, we convert the DB model of the IC from the fifth level to a ViewModel.
                var model = item.ToModel(languageCache);
                // Then, we get its parent ID. For IC with the code ABCDE, its parent is ABCD.
                var currentParentId = item.ParentId.Value;
                // Now, from the third level, we find any IC, which has at least one child with the code ABCD. In our
                // example, it is IC with the code ABC.
                var replacementMatch = replacement.FirstOrDefault(x => x.Children.Any(y => y.Id == currentParentId));
                // We take ABC's parent, which is AB.
                var replacementParentId = replacementMatch.ParentId;
                // And assign AB as the parent to ABCDE.
                model.ParentId = replacementParentId;
                yield return model;
            }
        }

        /// <summary>
        /// Replaces children of one level (usually the second) with their descendants from another level (usually the fifth).
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        private IEnumerable<IndustrialClassModel> ReplaceChildren(List<IndustrialClass> input, List<IndustrialClass> replacement)
        {
            // How this algorithm works: Let's have a table representing individual levels of Industrial classes
            // LEVEL            PARENT ID           ID          CHILD(REN) ID(S)
            // 1                null                A           [ AB ]
            // 2                A                   AB          [ ABC ]
            // 3                AB                  ABC         [ ABCD ]
            // 4                ABC                 ABCD        [ ABCDE ]
            // 5                ABCD                ABCDE       []
            foreach (var item in input)
            {
                // First, we convert the DB model of the IC from the second level to a ViewModel.
                var model = item.ToModel(languageCache);
                // Then, we get the IDs of its children. For IC with the code AB, its children are [ ABC ].
                var currentChildIds = item.Children.Select(x => x.Id).Distinct().ToList();
                // Now, from the forth level, we find all ICs, which have ABC as their parent. In our
                // example, it is IC with the code ABCD.
                var replacementMatches = replacement.Where(x => currentChildIds.Contains(x.ParentId.Value));
                // We take ABCD's children, which are [ ABCDE ].
                var replacementChildIds = replacementMatches.SelectMany(x => x.Children).Select(x => x.Id);
                // And assign [ ABCDE ] as the children of AB.
                model.ChildrenIds = replacementChildIds.ToList();
                yield return model;
            }
        }

        public IEnumerable<string> Uris
        {
            get
            {
                GetData();
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
            return dataByUri.TryGetOrDefault(uri) ?? dataByYUri.TryGetOrDefault(uri);
        }

        public DateTime GetLastUpdate()
        {
            return CacheBuildTime;
        }

        public List<IndustrialClassModel> Get125Flat()
        {
            GetData();
            return data125Flat;
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