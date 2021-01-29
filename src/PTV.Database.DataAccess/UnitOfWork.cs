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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Cloning;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess
{
    interface IUnitOfWorkContextInitializer
    {
        void InitContext(DbContext dbContext);
    }

    /// <summary>
    /// Unit of Work is scope of database operations. It is used for creating repositories which will work on the same context
    /// </summary>
    [RegisterService(typeof(IUnitOfWork), RegisterType.Transient)]
    internal class UnitOfWork : IUnitOfWork, IUnitOfWorkContextInitializer
    {
        protected readonly IResolveManager resolveManager;
        private IPrefilteringManager prefilteringManager;
        protected DbContext DbContext { get; private set; }
        private IHttpContextAccessor httpContextAccessorHolder = null;
        private IHttpContextAccessor HttpContext => httpContextAccessorHolder ?? (httpContextAccessorHolder = resolveManager.Resolve<IHttpContextAccessor>());
        private CancellationToken CancellationToken => HttpContext?.HttpContext?.RequestAborted ?? new CancellationToken(false);
        private IEntityNavigationsMap entityNavigationsMap = null;

        protected IEntityNavigationsMap EntityReferencesMap => entityNavigationsMap ?? (entityNavigationsMap = resolveManager.Resolve<IEntityNavigationsMap>());

        public UnitOfWork(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        void IUnitOfWorkContextInitializer.InitContext(DbContext dbContext)
        {
            this.DbContext = dbContext;
            Configure();
            CustomConfigure();
        }

        public void LoadCollection<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> property) where TEntity : class where TProperty : class
        {
            this.DbContext.Entry(entity)?.Load(property);
        }

        public void LoadCollection<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, ICollection<TProperty>>> property) where TEntity : class where TProperty : class
        {
            this.DbContext.Entry(entity)?.Load(property);
        }

        private bool AreTypesEqual(params Type[] types)
        {
            if (types.Length < 2) return true;
            for (int i = 0; i < types.Length - 1; i++)
            {
                if ((types[i] == null) || (types[i+1] == null) || (types[i].Name != types[i+1].Name))
                {
                    return false;
                }
            }
            return true;
        }

        public void LoadNavigation<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property) where TEntity : class where TProperty : class
        {
            if (entity == null) return;
            if (property.Compile()(entity) != default(TProperty)) return;
            var propertyName = ((property.Body as MemberExpression)?.Member as PropertyInfo)?.Name;
            var entityPropsDefs = EntityReferencesMap.NavigationsMap[typeof(TEntity)];
            var foreignKeysForNavigation = entityPropsDefs.NavigationsMap.First(i => i.Navigation.Name == propertyName).ForeignKeys;
            if (foreignKeysForNavigation.IsNullOrEmpty()) return;
            var sourceForeignKeyValues = foreignKeysForNavigation.ToDictionary(i => i.Name, i => new { DValue = entity.GetPropertyObjectValue(i.Name), DType = i.Type});
            var targetPropertyPrimaryKeys = EntityReferencesMap.NavigationsMap[typeof(TProperty)].PrimaryKeysDefinition.ToDictionary(i => i.Name, i => new {DType = i.Type});
            var query = this.DbContext.Set<TProperty>() as IQueryable<TProperty>;
            if (sourceForeignKeyValues.Count != 1 || targetPropertyPrimaryKeys.Count != 1 || !AreTypesEqual(sourceForeignKeyValues.Values.FirstOrDefault()?.DType, targetPropertyPrimaryKeys.Values.FirstOrDefault()?.DType))
            {
                if (sourceForeignKeyValues.Count != targetPropertyPrimaryKeys.Count
                    || sourceForeignKeyValues.Keys.Except(targetPropertyPrimaryKeys.Keys).Any()
                    || sourceForeignKeyValues.Keys.Any(i => !AreTypesEqual(sourceForeignKeyValues[i].DType, targetPropertyPrimaryKeys[i].DType)))
                {
                    throw new Exception($"Error in relationship from {typeof(TEntity).Name} to {typeof(TProperty).Name}. Foreign and Primary key(s) does not match");
                }
                sourceForeignKeyValues.ForEach(i =>
                {
                    query = query.Where(CoreExtensions.CreateLambdaEqual<TProperty>(i.Key, i.Value.DValue));
                });
            }
            else
            {
                query = query.Where(CoreExtensions.CreateLambdaEqual<TProperty>(targetPropertyPrimaryKeys.Keys.First(), sourceForeignKeyValues.Values.First().DValue));
            }
            entity.SetPropertyValue(property, query.FirstOrDefault());
            this.DbContext.Entry(entity).Reference(property).IsModified = false;
        }

        /// <summary>
        /// Creates new repository which provides access to database, set of queries.
        /// </summary>
        /// <typeparam name="T">Type of repository that should be instantiated</typeparam>
        /// <returns>Instantiated repository</returns>
        public T CreateRepository<T>() where T : class
        {
            return resolveManager.Resolve<T>();
        }

        private IPrefilteringManager PrefilteringManager()
        {
            return this.prefilteringManager = prefilteringManager ?? resolveManager.Resolve<IPrefilteringManager>();
        }

        /// <summary>
        /// Call async ToList function synchronous context
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ToListSync<T>(IQueryable<T> query)
        {
            return Asyncs.HandleAsyncInSync(() => query.ToListAsync(CancellationToken));
        }

        /// <summary>
        /// Call async FirstOrDefault function synchronous context
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FirstOrDefaultSync<T>(IQueryable<T> query)
        {
            return Asyncs.HandleAsyncInSync(() => query.FirstOrDefaultAsync(CancellationToken));
        }

        /// <summary>
        /// Call async SingleOrDefault function synchronous context
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SingleOrDefaultSync<T>(IQueryable<T> query)
        {
            return Asyncs.HandleAsyncInSync(() => query.SingleOrDefaultAsync(CancellationToken));
        }

        /// <summary>
        /// Apply includes to target query. Use this method for includes to make it testable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Target query where includes will be applipathed</param>
        /// <param name="includeChain">Includes that will be added to query</param>
        /// <param name="applyFilters">Should query filters be applied</param>
        /// <returns>Query with includes</returns>
        public IQueryable<T> ApplyIncludes<T>(IQueryable<T> source, Func<IQueryable<T>, IQueryable<T>> includeChain, bool applyFilters = false) where T : class
        {
            if (applyFilters)
            {
                source = PrefilteringManager().ApplyFilters(source);
            }
            return includeChain(source);
        }

        /// <summary>
        /// Apply includes to target query. Use this method for includes to make it testable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Target query where includes will be applied</param>
        /// <param name="includeChain">Includes that will be added to query</param>
        /// <param name="applyFilters">Should query filters be applied</param>
        /// <returns>Query with includes</returns>
        public IQueryable<T> ApplyIncludes<T>(ref IQueryable<T> source, Func<IQueryable<T>, IQueryable<T>> includeChain, bool applyFilters = false) where T : class
        {
            if (applyFilters)
            {
                source = PrefilteringManager().ApplyFilters(source);
            }
            return source = includeChain(source);
        }

        private void Configure()
        {
            DbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected virtual void CustomConfigure()
        {
            DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Get Entity Set for specified entity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Set for specified entity</returns>
        DbSet<T> ITranslationUnitOfWork.GetSet<T>()
        {
            var entitySet = DbContext.Set<T>();
            if (entitySet == null)
            {
                throw new Exception(string.Format(CoreMessages.DbSetNotFound, typeof(T).Name));
            }
            return entitySet;
        }

        public T GetFromKnown<T>(Func<T, bool> condition) where T : class
        {
            return DbContext.ChangeTracker.Entries<T>()?.FirstOrDefault(i => i != null && condition(i.Entity))?.Entity;
        }

        ITranslationCloneCache ITranslationUnitOfWork.TranslationCloneCache => new TranslationCloneCache(null);

        void ITranslationUnitOfWork.DetachEntity<T>(T entity)
        {}

        void ITranslationUnitOfWork.DetachOrRemoveEntity<T>(T entity)
        {}

        void ITranslationUnitOfWork.DetachOrRemoveEntities<T>(List<T> entities)
        {}
    }
}
