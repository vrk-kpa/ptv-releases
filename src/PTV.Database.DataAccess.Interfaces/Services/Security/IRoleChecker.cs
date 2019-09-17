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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Interfaces.Services.Security
{
    internal class EntityOrgRelevant
    {
        public EntityEntry<IRoleBased> Entity { get; }
        public bool? IsOwnOrganization { get; }

        internal EntityOrgRelevant(EntityEntry<IRoleBased> entity, bool? isOwnOrganization)
        {
            this.Entity = entity;
            this.IsOwnOrganization = isOwnOrganization;
        }
    }
    
    
    internal interface IRoleCheckerManager : IRoleChecker
    {
//        //List<EntityOrgRelevant> GetOrganizationRelevantEntities();
//    
//        /// <summary>
//        /// Check if user can update or create entity based on role
//        /// </summary>
//        /// <param name="entity">Updated entity</param>
//        /// <param name="entityEntry">Entity with all infos about update</param>
//        /// <param name="unitOfWork">Unity of work</param>
//        void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork);

        /// <summary>
        /// Check if user can update or create entity based on role
        /// </summary>
        /// <param name="entity">Updated entity</param>
        /// <param name="entityEntry">Entity with all infos about update</param>
        /// <param name="unitOfWork">Unity of work</param>
        void CheckEntity<TChecker>(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork) where TChecker : class, IRoleChecker;
    }
    
    internal interface IRoleChecker
    {
        /// <summary>
        /// Check entity if it is related to own organization
        /// </summary>
        /// <returns></returns>
        List<EntityOrgRelevant> AssignmentsToOwnOrganization();
        
        /// <summary>
        /// Check if user can update or create entity based on role
        /// </summary>
        /// <param name="entity">Updated entity</param>
        /// <param name="entityEntry">Entity with all infos about update</param>
        /// <param name="unitOfWork">Unity of work</param>
        void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork);
    }
}
