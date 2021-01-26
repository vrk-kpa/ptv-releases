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
using PTV.Database.DataAccess.Interfaces.Translators;

namespace PTV.Database.DataAccess.EntityCloners
{
    /// <summary>
    /// Interface for "cloner", ie. definitions for creating copy of entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityCloner<TEntity> where TEntity : class
    {
        /// <summary>
        /// Mehod defining list of properties which should be cloned
        /// </summary>
        void CloningDefinition();

        /// <summary>
        /// Perform cloning of entity by calling spcified definitions
        /// </summary>
        /// <param name="unitOfWork">Unit of work instance for creating requests to DB</param>
        /// <param name="entity">Instance of entity which will be cloned</param>
        /// <param name="beforeAddAction">Optional additional action which can be called before entity is added to db set</param>
        /// <param name="cloningMode"></param>
        /// <returns>Copy of entity</returns>
        TEntity PerformCloning(ITranslationUnitOfWork unitOfWork, TEntity entity, Action<TEntity> beforeAddAction = null, CloningMode cloningMode = CloningMode.Cloning);
    }
}
