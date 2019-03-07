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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// Base role checker
    /// </summary>
    [RegisterService(typeof(IRoleCheckerManager), RegisterType.Transient)]
    [RegisterService(typeof(IRoleChecker), RegisterType.Transient)]
    internal class RoleChecker : IRoleCheckerManager, IRoleChecker
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
                {typeof(Organization), typeof(OrganizationStructureRoleChecker) },
                {typeof(Locking), typeof(LockingRoleChecker) },
                {typeof(ServiceServiceChannel), typeof(ConnectionRoleChecker)},
                {typeof(StatutoryServiceGeneralDescriptionVersioned), typeof(StatutoryServiceGeneralDescriptionChecker)},
                {typeof(AppEnvironmentData), typeof(AppEnvironmentDataChecker)},
                {typeof(GeneralDescriptionServiceChannel), typeof(GeneralDescriptionConnectionRoleChecker)},
                {typeof(ServiceCollection), typeof(ServiceCollectionRoleChecker)},
                {typeof(ServiceCollectionVersioned), typeof(ServiceCollectionVersionedRoleChecker)}
            };
        }

        public void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var roleChecker = (IRoleChecker)resolveManager.Resolve(registerRoleCheckers[entity.GetType()]);
            roleChecker.CheckEntity(entity, entityEntry, unitOfWork);
        }
        
        public void CheckEntity<TChecker>(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork) where TChecker : class, IRoleChecker
        {
            var roleChecker = resolveManager.Resolve<TChecker>();
            roleChecker.CheckEntity(entity, entityEntry, unitOfWork);
        }
    }

    internal abstract class RoleCheckerBase : IRoleChecker
    {
        private IUserOrganizationService userOrganizationService;
        private IUserInfoService userInfoService;
        private Dictionary<Guid, IUserOrganizationRoles> userOrganizationRoles;
        private IPublishingStatusCache publishingStatusCache;
        
        protected ICacheManager CacheManager { get; }
        protected abstract DomainEnum Domain { get; }

        protected RoleCheckerBase(IUserOrganizationService userOrganizationService, IUserInfoService userInfoService, ICacheManager cacheManager)
        {
            this.userOrganizationService = userOrganizationService;
            this.userInfoService = userInfoService;
            this.publishingStatusCache = cacheManager.PublishingStatusCache;
            CacheManager = cacheManager;
        }
        
        private Dictionary<Guid, IUserOrganizationRoles> GetUserOrgStructure()
        {
            return userOrganizationService
                .GetAllUserOrganizationRoles()
                .ToDictionary(x => x.OrganizationId);
        }

        private Dictionary<Guid, IUserOrganizationRoles> UserOrganizations => userOrganizationRoles = userOrganizationRoles ?? GetUserOrgStructure();

        private bool CheckRule(PermisionEnum rule, PermisionEnum permissionToCheck)
        {
            return rule.HasFlag(permissionToCheck);
        }
        
        private bool CheckPermission(PermisionEnum permissionToCheck, bool isOwnOrganization)
        {
            var permissions = userInfoService.GetPermissions();
            var permission = permissions.TryGet(Domain.ToCamelCase());
            return permission != null &&
              CheckRule(isOwnOrganization ? permission.RulesOwn : permission.RulesAll, permissionToCheck);
        }

        protected bool CheckEntityRoles(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IEnumerable<Guid?> organizationIds)
        {
            if (organizationIds.IsNullOrEmpty())
            {
                return CheckPermission(GetPermissionType(entity, entityEntry), false);
            }
            bool result = true;
            organizationIds
                .ForEach(id => 
                    result = result && CheckPermission(GetPermissionType(entity, entityEntry), id.HasValue && IsOwnOrganization(id.Value))
                  );
            return result;
        }

        protected T GetEntity<T>(Guid id, IUnitOfWorkCachedSearching unitOfWork, Func<IQueryable<T>, IQueryable<T>> includeChain = null) where T : class, IEntityIdentifier
        {
            return unitOfWork.Find<T>(id, includeChain);
        }

        protected bool IsAdded(IRoleBased entity, EntityEntry<IRoleBased> entityEntry)
        {
            return entityEntry?.State == EntityState.Added;
        }

        protected virtual PermisionEnum GetPermissionType(IRoleBased entity, EntityEntry<IRoleBased> entityEntry)
        {
            if (entity is IPublishingStatus psEntity)
            {
                
                switch (publishingStatusCache.GetEnumValue(psEntity.PublishingStatusId))
                {
                    case PublishingStatus.Published:
                    case PublishingStatus.OldPublished:
                        return PermisionEnum.Publish;
                    case PublishingStatus.Modified:
                        return PermisionEnum.Update;
                    case PublishingStatus.Deleted:
                        return PermisionEnum.Delete;
                }
            }
            
            return IsAdded(entity, entityEntry) ? PermisionEnum.Create : PermisionEnum.Update;
        }
        
        protected virtual bool IsOwnOrganization(Guid organizationId)
        {
            return UserOrganizations.ContainsKey(organizationId);
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

        protected void ThrowError(IUserIdentification userIdentification)
        {
            throw new RoleActionException("User has no rights to update or create this entity!",
                new List<string> {userIdentification.UserName, string.Join(",", UserOrganizations.Values.Select(x => x.Role).Distinct())});
        }
    }
}
