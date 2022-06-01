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
using PTV.Database.DataAccess.Interfaces.Translators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Interfaces.DbContext
{
    /// <summary>
    /// Unit of Work is scope of database operations. It is used for creating repositories which will work on the same context
    /// </summary>
    public interface IUnitOfWork : ITranslationUnitOfWork
    {
        /// <summary>
        /// Creates new repository which provides access to database, set of queries.
        /// </summary>
        /// <typeparam name="T">Type of repository that should be instantiated</typeparam>
        /// <returns>Instantiated repository</returns>
        T CreateRepository<T>() where T : class;

        /// <summary>
        /// Add chain of includes to query
        /// </summary>
        /// <param name="source"></param>
        /// <param name="includeChain"></param>
        /// <param name="applyFilters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> ApplyIncludes<T>(IQueryable<T> source, Func<IQueryable<T>, IQueryable<T>> includeChain, bool applyFilters = false) where T : class;

        /// <summary>
        /// Call async ToList function synchronous context
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> ToListSync<T>(IQueryable<T> query);

        /// <summary>
        /// Call async FirstOrDefault function synchronous context
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T FirstOrDefaultSync<T>(IQueryable<T> query);

        /// <summary>
        /// Call async SingleOrDefault function synchronous context
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T SingleOrDefaultSync<T>(IQueryable<T> query);
    }
}
