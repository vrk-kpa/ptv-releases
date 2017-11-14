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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// Base role checker
    /// </summary>
    [RegisterService(typeof(IRoleChecker), RegisterType.Transient)]
    internal class RoleChecker : IRoleChecker
    {
        protected readonly IResolveManager resolveManager;
        private Dictionary<Type, Type> registerRoleCheckers;

        public RoleChecker(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;

            // register roleCheckers
            registerRoleCheckers = new Dictionary<Type, Type>()
            {
                {typeof(Service), typeof(ServiceRoleChecker) },
                {typeof(ServiceVersioned), typeof(ServiceVersionedRoleChecker) },
                {typeof(ServiceChannel), typeof(RootChannelRoleChecker)},
                {typeof(ServiceChannelVersioned), typeof(ChannelRoleChecker) },
                {typeof(OrganizationVersioned), typeof(OrganizationRoleChecker) },
                {typeof(Locking), typeof(LockingRoleChecker) },
                {typeof(ServiceServiceChannel), typeof(ConnectionRoleChecker)},
                {typeof(StatutoryServiceGeneralDescriptionVersioned), typeof(StatutoryServiceGeneralDescriptionChecker)},
                {typeof(AppEnvironmentData), typeof(AppEnvironmentDataChecker)},
                {typeof(GeneralDescriptionServiceChannel), typeof(GeneralDescriptionConnectionRoleChecker)},
                {typeof(ServiceCollection), typeof(ServiceCollectionRoleChecker)},
                {typeof(ServiceCollectionVersioned), typeof(ServiceCollectionVersionedRoleChecker)}
            };
        }

        public virtual void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var roleChecker = (IRoleChecker)resolveManager.Resolve(registerRoleCheckers[entity.GetType()]);
            roleChecker.CheckEntity(entity, entityEntry, unitOfWork);
        }
    }

    internal abstract class RoleCheckerBase : IRoleChecker
    {
        private readonly IHttpContextAccessor ctxAccessor;
        private const string RoleClaimIdentifier = "role";

        protected RoleCheckerBase(IHttpContextAccessor ctxAccessor)
        {
            this.ctxAccessor = ctxAccessor;
        }

//        private IEnumerable<Claim> GetClaims()
//        {
//            return ctxAccessor?.HttpContext?.User?.Claims;
//        }

        protected List<List<IUserOrganizationRoles>> GetUserCompleteOrgStructure(IUserOrganizationService userOrganizationService, IUnitOfWork unitOfWork)
        {
            return userOrganizationService.GetUserCompleteOrgStructure(unitOfWork);
        }

//        protected UserRoleEnum GetClaimRole()
//        {
//            var role = GetClaims().FirstOrDefault(i => i.Type == RoleClaimIdentifier);
//            UserRoleEnum result;
//            if (Enum.TryParse(role.Value, out result))
//            {
//                return result;
//            }
//            return UserRoleEnum.Shirley;
//        }

        protected bool CheckAllIds(IList<Guid> allIds, IEnumerable<Guid?> ids)
        {
            if (allIds.IsNullOrEmpty()) return true;
            var result = true;
            ids.OfType<Guid>().ForEach(id => result = result && allIds.Contains(id));
            return result;
        }

        protected bool CheckAnyIds(IList<Guid> allIds, IEnumerable<Guid?> ids)
        {
            if (allIds.IsNullOrEmpty()) return false;
            return ids.OfType<Guid>().Any(x => allIds.Contains(x));
        }

        protected T GetEntity<T>(Guid id, IUnitOfWorkCachedSearching unitOfWork, Func<IQueryable<T>, IQueryable<T>> includeChain = null) where T : class, IEntityIdentifier
        {
            return unitOfWork.Find<T>(id, includeChain);
        }

        protected bool IsAdded(EntityEntry<IRoleBased> entityEntry)
        {
            if (entityEntry != null)
            {
                return entityEntry.State == EntityState.Added;
            }
            return false;
        }

        protected T GetOriginalValue<T>(EntityEntry<IRoleBased> entityEntry, string propertyName)
        {
            if (entityEntry != null)
            {
                return (T)entityEntry.Property(propertyName).OriginalValue;
            }
            return default(T);
        }

        public abstract void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork);

        protected void ThrowError(IUserIdentification userIdentification, IEnumerable<UserRoleEnum> roles)
        {
            throw new RoleActionException("User has no rights to update or create this entity!",
                new List<string> {userIdentification.UserName, string.Join(",", roles)});
        }
    }
}
