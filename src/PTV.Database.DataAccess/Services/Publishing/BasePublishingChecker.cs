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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Publishing;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.Publishing
{
    internal abstract class BasePublishingChecker<T> : IBasePublishingChecker<T>
    {
        private T entity;
        protected readonly IResolveManager resolveManager;
        protected List<string> messages;

        public BasePublishingChecker()
        {
        }

        public BasePublishingChecker(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        public void Init(Guid id, IUnitOfWork unitOfWork)
        {
            this.entity = FetchEntity(id, unitOfWork);
        }
       
        protected void NotEmptyGuid<TOutProperty>(Expression<Func<T, TOutProperty>> property, string message = null)
        {
            var val = property.Compile()(entity);
            var value = val as Guid?;

            if (!value.IsAssigned())
            {
                messages.Add(string.IsNullOrEmpty(message) ? "Cannot be empty." : message);
            }
        }

        protected bool NotEmptyString<TOutProperty>(Expression<Func<T, TOutProperty>> property)
        {
            var val = property.Compile()(entity);
            var value = val as string;
            return string.IsNullOrEmpty(value);
        }
        protected bool NotEmptyList<TOutProperty>(Expression<Func<T, TOutProperty>> property)
        {
            var val = property.Compile()(entity);
            var value = val as IList;

            return value == null || value.Count == 0;
        }

        protected TEntity GetEntity<TEntity>(Guid? id, IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain) where TEntity : class, IEntityIdentifier
        {
            if (id.IsAssigned())
            {
                var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = unitOfWork.ApplyIncludes(repository.All(), includeChain, true).SingleOrDefault(x => x.Id == id);
                if (entity == null)
                {
                    throw new EntityNotFoundException($"Entity {typeof(TEntity).Name} with id {id} not found", typeof(TEntity).Name, new[] { id.ToString() });
                }
                return entity;
            }
            return null;
        }

        public abstract bool ValidateEntity();

        public abstract T FetchEntity(Guid id, IUnitOfWork unitOfWork);
    }
}
