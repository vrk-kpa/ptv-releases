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
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.EntityCloners
{
    /// <summary>
    /// Manager class initiating the cloning process of entity
    /// </summary>
    [RegisterService(typeof(ICloningManager), RegisterType.Transient)]
    public class CloningManager : ICloningManager
    {
        private readonly IResolveManager resolveManager;

        public CloningManager(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        /// <summary>
        /// Perform cloning (copying) of entity
        /// </summary>
        /// <typeparam name="T">Type of entity which will be cloned</typeparam>
        /// <param name="entity">Entity which will be cloned</param>
        /// <param name="unitOfWork">Unit of work instance for calling DB operations</param>
        /// <returns>Cloned entity</returns>
        public T CloneEntity<T>(T entity, ITranslationUnitOfWork unitOfWork) where T : class
        {
            var cloner = resolveManager.Resolve<IEntityCloner<T>>();
            return cloner.PerformCloning(unitOfWork, entity, cloningMode: CloningMode.Cloning);
        }

        public void DeleteEntity<T>(T entity, ITranslationUnitOfWork unitOfWork) where T : class
        {
            var cloner = resolveManager.Resolve<IEntityCloner<T>>();
            cloner.PerformCloning(unitOfWork, entity, cloningMode: CloningMode.Deleting);
        }

        public bool IsCloneable<T>() where T : class
        {
            var cloner = resolveManager.Resolve<IEntityCloner<T>>(true);
            return cloner != null;
        }
    }
}
