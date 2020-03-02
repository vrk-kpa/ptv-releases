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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Framework;

namespace PTV.Database.DataAccess.Utils
{
    [RegisterService(typeof(DataUtils), RegisterType.Transient)]
    public class DataUtils
    {
        internal IEnumerable<T> GetSpecificLanguageItem<T>(IEnumerable<T> items, Guid requestedLanguageId) where T : ILocalizable
        {
            return items.Where(x => x.LocalizationId == requestedLanguageId);
        }

        internal void JoinHierarchy<T>(IDictionary<string, T> source, Func<T, IEnumerable<string>> getKeysFunc, Action<T, T> joinKeyItemWithSourceItemAction)
        {
            foreach (var item in source.Values)
            {

                getKeysFunc(item).ForEach(key =>
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        T keyItem = source.TryGet(key);
                        if (keyItem != null)
                        {
                            joinKeyItemWithSourceItemAction(keyItem, item);
//                    entity.Parent = parent;
//                    entity.ParentId = parent.Id;
//                    parent.Children.Add(entity);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Merge new and saved entities and returns updated collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unitOfWork"></param>
        /// <param name="collection"></param>
        /// <param name="savedFilterExpression">query filter for search existing entities</param>
        /// <param name="getSelectedIdFunc">function which returns id of related entity (selected)</param>
        /// <returns></returns>
        internal ICollection<T> UpdateCollectionForReferenceTable<T>(IUnitOfWorkWritable unitOfWork, ICollection<T> collection, Expression<Func<T, bool>> savedFilterExpression, Func<T, Guid> getSelectedIdFunc, Expression<Func<T, bool>> addFilterExpression = null, Action<T, T> updateEntity = null)
        {
            if (collection == null) return new List<T>();
            var updatedIds = collection.Select(getSelectedIdFunc).ToList();
            var repository = unitOfWork.CreateRepository<IRepository<T>>();
            var currentEntities = repository.All()
                 .Where(savedFilterExpression)
                .ToDictionary(getSelectedIdFunc);
            var result = collection.Where(x => updatedIds.Contains(getSelectedIdFunc(x))).Select(x =>
            {
                T entity;
                var temp = currentEntities.TryGetValue(getSelectedIdFunc(x), out entity) ? entity : x;
                updateEntity?.Invoke(temp, x);
                return temp;
            }).ToList();
            if (addFilterExpression != null)
            {
                result.AddRange(repository.All().Where(addFilterExpression));
            }
            return result;
        }

        internal ICollection<T> UpdateCollectionWithRemove<T>(IUnitOfWorkWritable unitOfWork, ICollection<T> collection, Expression<Func<T, bool>> savedFilterExpression) where T : class, IEntityIdentifier
        {
            if (collection == null) return new List<T>();
            var updatedIds = collection.Where(x => x != null).Select(x => x.Id).ToList();
            var repository = unitOfWork.CreateRepository<IRepository<T>>();

            List<T> currentEntities = unitOfWork.TranslationCloneCache.GetFromCachedSet<T>().Select(x => x.ClonedEntity).Where(savedFilterExpression.Compile()).ToList();
            if (currentEntities.Count == 0)
            {
                currentEntities = repository.All().Where(savedFilterExpression).ToList();
            }
            var toRemove = currentEntities.Where(x => !updatedIds.Contains(x.Id)).ToList();
            toRemove.ForEach(x => repository.Remove(x));
            return toRemove;
        }

//        public void AddSqlScripts(MigrationBuilder migrationBuilder, string path)
//        {
//            var directory = new DirectoryInfo(path);
//            if (directory.Exists)
//            {
//                foreach (var file in directory.GetFiles("*.sql").OrderBy(x => x.Name))
//                {
//                    Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
//                    migrationBuilder.Sql(File.ReadAllText(file.FullName));
//                }
//
//            }
//        }
//
//        public void AddSqlScript(MigrationBuilder migrationBuilder, string path)
//        {
//            var file = new FileInfo(path);
//            if (file.Exists)
//            {
//                Console.WriteLine($"Running script {file.Name}. Full path {file.FullName}.");
//                migrationBuilder.Sql(File.ReadAllText(file.FullName));
//            }
//            else
//            {
//                Console.Error.WriteLine($"Script file {file.Name} not found. Full path {file.FullName}.");
//            }
//        }

        internal ICollection<OrganizationService> GetOrganizationServices(IUnitOfWorkWritable unitOfWork, Guid serviceId)
        {
            return unitOfWork.CreateRepository<IOrganizationServiceRepository>().All().Where(x => x.ServiceVersionedId == serviceId).ToList();
        }

        internal void RemoveItemCollection<T>(IUnitOfWorkWritable unitOfWork, Func<T, bool> getSelectedFunc)
            where T : EntityBase
        {
            // Remove all possible old items
            var rep = unitOfWork.CreateRepository<IRepository<T>>();
            var toRemove = rep.All().Where(getSelectedFunc).ToList();
            toRemove.ForEach(i => rep.Remove(i));
        }
    }
}
