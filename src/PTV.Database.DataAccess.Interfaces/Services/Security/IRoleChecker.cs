﻿/**
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

using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Interfaces.Services.Security
{
    internal interface IRoleCheckerManager
    {
        /// <summary>
        /// Chceck if user can update or create entity based on role
        /// </summary>
        /// <param name="entity">Updated entity</param>
        /// <param name="entityEntry">Entity with all infos about update</param>
        /// <param name="unitOfWork">Unity of work</param>
        void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork);

        /// <summary>
        /// Chceck if user can update or create entity based on role
        /// </summary>
        /// <param name="entity">Updated entity</param>
        /// <param name="entityEntry">Entity with all infos about update</param>
        /// <param name="unitOfWork">Unity of work</param>
        void CheckEntity<TChecker>(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork) where TChecker : class, IRoleChecker;
    }
    
    internal interface IRoleChecker
    {
        /// <summary>
        /// Chceck if user can update or create entity based on role
        /// </summary>
        /// <param name="entity">Updated entity</param>
        /// <param name="entityEntry">Entity with all infos about update</param>
        /// <param name="unitOfWork">Unity of work</param>
        void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork);
    }
}
