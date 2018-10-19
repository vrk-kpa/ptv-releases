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
using Microsoft.AspNetCore.Http;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Privileges;
using PTV.Framework;

namespace PTV.Database.DataAccess.Filters
{
    [RegisterService(typeof(AccessRightCacheHolder), RegisterType.Singleton)]
    internal class AccessRightCacheHolder
    {
        public AccessRightCacheHolder(ITypesCache typesCache)
        {
            GetAllAccessRightTypes = typesCache.GetCacheData<AccessRightType>().ToDictionary(i => i.Code.ToLower(), i => i.Id);
        }

        public Dictionary<string, Guid> GetAllAccessRightTypes { get; }
    }

//
//    internal abstract class BlockedEntityFiltering
//    {
//        protected IHttpContextAccessor ContextAccessor;
//        private readonly AccessRightCacheHolder accessRightTypes;
//        protected List<Guid> ActionAssignedAccessRights;
//
//        protected BlockedEntityFiltering(IHttpContextAccessor contextAccessor, AccessRightCacheHolder accessRightTypes)
//
//        {
//            this.ContextAccessor = contextAccessor;
//            this.accessRightTypes = accessRightTypes;
//            this.ActionAssignedAccessRights = AccessRightIdsForAction();
//        }
//
//        protected List<Guid> GetAccessRightIds()
//        {
//            return accessRightTypes.GetAllAccessRightTypes.Values.Select(i => i).ToList();
//        }
//
//        protected List<string> ActionAccessRights(IHttpContextAccessor contextAccessor)
//        {
//            return (ContextAccessor?.HttpContext?.Items?.TryGet(PtvClaims.HandlingPrefixAction + PtvClaims.UserAccessRights) as List<string>) ?? new List<string>();
//        }
//
//        protected List<Guid> AccessRightIdsForAction(IHttpContextAccessor contextAccessor = null)
//        {
//            var actionAccessRights = ActionAccessRights(contextAccessor ?? this.ContextAccessor);
//            return accessRightTypes.GetAllAccessRightTypes.Where(i => actionAccessRights.Contains(i.Key)).Select(i => i.Value).ToList();
//        }
//    }
//
//    [QueryFilter]
//    internal class BlockedEntityServiceFilter : BlockedEntityFiltering, IQueryFilter<ServiceVersioned>
//    {
//        public BlockedEntityServiceFilter(IHttpContextAccessor contextAccessor, AccessRightCacheHolder accessRightTypes) : base(contextAccessor, accessRightTypes)
//        {}
//
//        public IQueryable<T> Filter<T>(IQueryable<T> query) where T : ServiceVersioned
//        {
//            return query.Where(i => !i.UnificRoot.BlockedAccessRights.Select(j => j.AccessBlockedId).Any(m => ActionAssignedAccessRights.Contains(m)));
//        }
//    }
//
//    [QueryFilter]
//    internal class BlockedEntityChannelFilter : BlockedEntityFiltering, IQueryFilter<ServiceChannelVersioned>
//    {
//        public BlockedEntityChannelFilter(IHttpContextAccessor contextAccessor, AccessRightCacheHolder accessRightTypes) : base(contextAccessor, accessRightTypes)
//        { }
//
//        public IQueryable<T> Filter<T>(IQueryable<T> query) where T : ServiceChannelVersioned
//        {
//            return query.Where(i => !i.UnificRoot.BlockedAccessRights.Select(j => j.AccessBlockedId).Any(m => ActionAssignedAccessRights.Contains(m)));
//        }
//    }
//
//    [QueryFilter]
//    internal class BlockedEntityOrganizationFilter : BlockedEntityFiltering, IQueryFilter<OrganizationVersioned>
//    {
//        public BlockedEntityOrganizationFilter(IHttpContextAccessor contextAccessor, AccessRightCacheHolder accessRightTypes) : base(contextAccessor, accessRightTypes)
//        { }
//
//        public IQueryable<T> Filter<T>(IQueryable<T> query) where T : OrganizationVersioned
//        {
//            return query.Where(i => !i.UnificRoot.BlockedAccessRights.Select(j => j.AccessBlockedId).Any(m => ActionAssignedAccessRights.Contains(m)));
//        }
//    }
//
//    [QueryFilter]
//    internal class BlockedEntityGeneralDescriptionFilter : BlockedEntityFiltering, IQueryFilter<StatutoryServiceGeneralDescriptionVersioned>
//    {
//        public BlockedEntityGeneralDescriptionFilter(IHttpContextAccessor contextAccessor, AccessRightCacheHolder accessRightTypes) : base(contextAccessor, accessRightTypes)
//        { }
//
//        public IQueryable<T> Filter<T>(IQueryable<T> query) where T : StatutoryServiceGeneralDescriptionVersioned
//        {
//            return query.Where(i => !i.UnificRoot.BlockedAccessRights.Select(j => j.AccessBlockedId).Any(m => ActionAssignedAccessRights.Contains(m)));
//        }
//    }
}
