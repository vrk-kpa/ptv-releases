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
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Cloning;

namespace PTV.Database.DataAccess.Interfaces.Translators
{
    public interface ITranslationUnitOfWork
    {
        DbSet<T> GetSet<T>() where T : class;

        ITranslationCloneCache TranslationCloneCache { get; }

        void LoadNavigationProperty<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> property) where TEntity : class where TProperty : class;
        void LoadNavigationProperty<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, ICollection<TProperty>>> property) where TEntity : class where TProperty : class;
        void LoadNavigationProperty<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property) where TEntity : class where TProperty : class;

        void DetachEntity<T>(T entity) where T : class;
        
        void TouchEntity<T>(T entity) where T : class;

        void DetachOrRemoveEntity<T>(T entity) where T : class;

        void DetachOrRemoveEntities<T>(List<T> entities) where T : class;
    }
}
