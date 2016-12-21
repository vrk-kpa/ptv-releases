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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess
{
    /// <summary>
    /// Unit of Work is scope of database operations. It is used for creating repositories which will work on the same context
    /// </summary>
    [RegisterService(typeof(IUnitOfWork), RegisterType.Transient)]
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly IResolveManager resolveManager;
        private IPrefilteringManager prefilteringManager;
        protected PtvDbContext DbContext { get; private set; }

        public UnitOfWork(IResolveManager resolveManager, PtvDbContext dbContext)
        {
            this.resolveManager = resolveManager;
            this.DbContext = dbContext;
            Configure();
            CustomConfigure();
        }

        public void LoadNavigationProperty<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, ICollection<TProperty>>> property) where TEntity : class where TProperty : class
        {
            this.DbContext.Entry(entity).Load(property);
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
        /// Apply includes to target query. Use this method for includes to make it testable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Target query where includes will be applied</param>
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
    }
}
