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
using PTV.Framework;
using Microsoft.AspNetCore.Http;
using PTV.Database.Model.Models;
using PTV.Framework.ServiceManager;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// Organization role checker
    /// </summary>
    [RegisterService(typeof(OrganizationRoleChecker), RegisterType.Transient)]
    internal class OrganizationRoleChecker : RoleCheckerBase
    {
        private readonly IUserIdentification userIdentification;
        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly IRoleCheckerManager roleChecker;
        private readonly IUserInfoService userInfoService;

        public OrganizationRoleChecker(
            IRoleCheckerManager roleChecker,
            IUserIdentification userIdentification,
            IUserOrganizationService userOrganizationService, 
            IUserInfoService userInfoService, 
            ICacheManager cacheManager,
            ApplicationConfiguration applicationConfiguration
        ) : base(userOrganizationService, userInfoService, cacheManager)
        {
            this.userIdentification = userIdentification;
            this.applicationConfiguration = applicationConfiguration;
            this.roleChecker = roleChecker;
            this.userInfoService = userInfoService;
        }

        protected override DomainEnum Domain => DomainEnum.Organizations;

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var organization = entity as OrganizationVersioned;
            var checkStatus = true;

            if (organization != null)
            {

                // check cyclic dependency
                if (IsCyclicDependency(unitOfWork, organization.UnificRootId, organization.ParentId))
                {
                    throw new OrganizationCyclicDependencyException();
                }

                var archivedId = CacheManager.TypesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
                var publishedId = CacheManager.TypesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                // check sub-organization hierarchy level
                if (organization.ParentId.IsAssigned())
                {
                    var maxHierarchyLevel = applicationConfiguration.MaxOrganizationHierarchyLevel;
                    if(organization.PublishingStatusId != archivedId &&
                       MaxHierarchyLevelReached(unitOfWork, organization.ParentId, maxHierarchyLevel))
                    {
                        throw new OrganizationMaxHierarchyLevelException(maxHierarchyLevel);
                    }
                }
                else
                {
                    if (entityEntry.Properties.First(i => i.Metadata.Name == nameof(OrganizationVersioned.PublishingStatusId)).OriginalValue is Guid previousStatus)
                    {
                        if (organization.PublishingStatusId == archivedId && previousStatus == publishedId && userInfoService.GetUserInfo().Role != UserRoleEnum.Eeva)
                        {
                            checkStatus = false;
                        }
                    }
                }

                roleChecker.CheckEntity<OrganizationStructureRoleChecker>(entity, entityEntry, unitOfWork);

                checkStatus &= CheckEntityRoles(entity, entityEntry, GetPermissionType(entity, entityEntry) == PermisionEnum.Create
                    ? new List<Guid?> {organization.ParentId}
                    : new List<Guid?> {organization.UnificRootId});

            }
            if (!checkStatus)
            {
                ThrowError(userIdentification);
            }
        }

        private bool IsCyclicDependency(IUnitOfWork unitOfWork, Guid unificRootId, Guid? parentId)
        {
            if (parentId == null) return false;
            if (!unificRootId.IsAssigned() || !parentId.IsAssigned()) return false;
            if (unificRootId == parentId) return true;
            var filteredOutStatuses = new List<Guid>()
            {
                CacheManager.TypesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()),
                CacheManager.TypesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString())
            };
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => i.UnificRootId == parentId.Value && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var allTree = higherOrgs.ToList();
            CyclicCheck(unitOfWork, higherOrgs, ref allTree, filteredOutStatuses);
            return allTree.Contains(unificRootId);
        }

        private void CyclicCheck(IUnitOfWork unitOfWork, List<Guid> orgs, ref List<Guid> allTree, List<Guid> filteredOutStatuses)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var higherOrgs = orgRep.All().Where(i => !filteredOutStatuses.Contains(i.PublishingStatusId)).Where(i => orgs.Contains(i.UnificRootId) && i.ParentId != null).Select(i => i.ParentId.Value).Distinct().ToList();
            var toCheck = higherOrgs.Except(allTree).ToList();
            allTree.AddRange(toCheck);
            if (toCheck.Any())
            {
                CyclicCheck(unitOfWork, toCheck, ref allTree, filteredOutStatuses);
            }
        }

        private bool MaxHierarchyLevelReached(IUnitOfWork unitOfWork, Guid? parentId, int maxHierarchyLavel)
        {
            if (parentId == null) return false;
            var publishedId = CacheManager.TypesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>().All();
            
            for (var i = 0; i < maxHierarchyLavel - 1; i++)
            {
                parentId = organizationRep.SingleOrDefault(o => o.UnificRootId == parentId && o.PublishingStatusId == publishedId)?.ParentId;
                if (!parentId.IsAssigned())
                {
                    return false;
                }
            }
            return true;
        }

        private T GetOrganizationOriginalValue<T>(OrganizationVersioned entity, EntityEntry<IRoleBased> entityEntry, string propertyName, IUnitOfWorkWritable unitOfWork)
        {
            var cachedValue = unitOfWork.TranslationCloneCache.GetFromCachedSet<OrganizationVersioned>()
                .Where(x => x.ClonedEntity.Id == entity.Id).Select(x => (T)x.OriginalEntity.GetItemValue(propertyName)).SingleOrDefault();

            return cachedValue != null ? cachedValue : GetOriginalValue<T>(entityEntry, propertyName);
        }
    }
    /// <summary>
    /// Organization role checker
    /// </summary>
    [RegisterService(typeof(OrganizationStructureRoleChecker), RegisterType.Transient)]
    internal class OrganizationStructureRoleChecker : RoleCheckerBase
    {
        private readonly IUserIdentification userIdentification;

        public OrganizationStructureRoleChecker(
            IUserIdentification userIdentification,
            IUserOrganizationService userOrganizationService, 
            IUserInfoService userInfoService, 
            ICacheManager cacheManager
        ) : base(userOrganizationService, userInfoService, cacheManager)
        {
            this.userIdentification = userIdentification;
        }

        protected override DomainEnum Domain => DomainEnum.OrganizationStructure;

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var organization = entity as OrganizationVersioned;
            var checkStatus = true;

            if (organization != null)
            {
                var previousParentId = GetOrganizationOriginalValue<Guid?>(organization, entityEntry, "ParentId", unitOfWork);

                if (organization.ParentId != previousParentId) //can manage structure 
                {
                    checkStatus = CheckEntityRoles(entity, entityEntry, new List<Guid?> { organization.ParentId, previousParentId, organization.UnificRootId });
                }
                else if (GetPermissionType(entity, entityEntry) == PermisionEnum.Create)
                {
                    checkStatus = CheckEntityRoles(entity, entityEntry, new List<Guid?> { organization.ParentId });
                }
            }
            if (!checkStatus)
            {
                ThrowError(userIdentification);
            }
        }

        private T GetOrganizationOriginalValue<T>(OrganizationVersioned entity, EntityEntry<IRoleBased> entityEntry, string propertyName, IUnitOfWorkWritable unitOfWork)
        {
            var cachedValue = unitOfWork.TranslationCloneCache.GetFromCachedSet<OrganizationVersioned>()
                .Where(x => x.ClonedEntity.Id == entity.Id).Select(x => (T)x.OriginalEntity.GetItemValue(propertyName)).SingleOrDefault();

            return cachedValue != null ? cachedValue : GetOriginalValue<T>(entityEntry, propertyName);
        }
    }
}
