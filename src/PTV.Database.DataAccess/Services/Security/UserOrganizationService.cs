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
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(IUserOrganizationService), RegisterType.Transient)]
    internal class UserOrganizationService : IUserOrganizationService
    {
        private IContextManager contextManager;
        private IOrganizationTreeDataCache organizationTreeDataCache;
        private IPahaTokenAccessor pahaTokenAccessor;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public UserOrganizationService(ICacheManager cacheManager, IContextManager contextManager,IOrganizationTreeDataCache organizationTreeDataCache, IPahaTokenAccessor pahaTokenAccessor)
        {
            this.typesCache = cacheManager.TypesCache;
            this.contextManager = contextManager;
            this.organizationTreeDataCache = organizationTreeDataCache;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.languageCache = cacheManager.LanguageCache;
        }

        public List<UserOrgRoleMappingData> UpdateUserOrgRolesMapping(List<UserOrgRoleMappingData> mappings, bool removeOthers = false)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var orgs = unitOfWork.CreateRepository<IOrganizationRepository>().All().Select(i => i.Id).ToList();
                if (removeOthers) rep.DeleteAll();

                mappings = mappings.Where(i => orgs.Contains(i.OrganizationId)).ToList();

                var toUpdate = rep.All().Where(i => mappings.Any(j => j.UserId == i.UserId && j.OrganizationId == i.OrganizationId)).ToList();
                toUpdate.ForEach(i =>
                    {
                        i.RoleId = mappings.First(j => j.UserId == i.UserId && j.OrganizationId == i.OrganizationId).RoleId;
                    });
                var toAdd = mappings.Where(i => !toUpdate.Any(j => j.UserId == i.UserId && j.OrganizationId == i.OrganizationId));
                toAdd.ForEach(i =>
                    {
                        rep.Add(new UserOrganization
                        {
                            UserId = i.UserId,
                            OrganizationId = i.OrganizationId,
                            RoleId = i.RoleId
                        });
                    });
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var allMappings = unitOfWork.CreateRepository<IUserOrganizationRepository>().All().ToList();
                return mappings.Where(i => allMappings.Any(j => j.UserId == i.UserId && j.OrganizationId == i.OrganizationId && j.RoleId == i.RoleId)).ToList();
            });
        }

        public List<IUserOrganizationRoles> GetOrganizationsAndRolesForLoggedUser()
        {
            return new List<IUserOrganizationRoles> { new UserOrganizationRoles { OrganizationId = pahaTokenAccessor.ActiveOrganizationId, Role = pahaTokenAccessor.UserRole, IsMain = true }};
        }

        public UserRoleEnum UserHighestRole()
        {
            var userOrgs = GetOrganizationsAndRolesForLoggedUser();
            return UserHighestRole(userOrgs);
        }

        public UserRoleEnum UserHighestRole(List<List<IUserOrganizationRoles>> organizations)
        {
            if (organizations.IsNullOrEmpty()) return UserRoleEnum.Shirley;
            return UserHighestRole(organizations.SelectMany(i => i));
        }

        public UserRoleEnum UserHighestRole(IEnumerable<IUserOrganizationRoles> organizations)
        {
            if (organizations.IsNullOrEmpty()) return UserRoleEnum.Shirley;
            return organizations.Select(i => i.Role).OrderBy(i => i).FirstOrDefault();
        }

        public List<VmUserOrganization> GetOrganizationRolesForUser(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var data = rep.All().Where(i => i.UserId == userId).ToList();
                return data.Select(i => new VmUserOrganization {OrganizationId = i.OrganizationId, RoleId = i.RoleId, UserId = i.UserId, IsMain = i.IsMain}).ToList();
            });
        }

        public IList<Guid> GetAllUserOrganizationIds(List<PublishingStatus> allowedPublishingStatuses = null)
        {
            return GetAllUserOrganizationRoles(allowedPublishingStatuses).Select(x => x.OrganizationId).ToList();
        }

        private List<Guid> GetAllChildrenForOrganization(Dictionary<Guid, OrganizationTreeItem> data, Guid orgId, List<Guid> allowedPublishingStatusIds)
        {
            var result = new List<Guid>();
            data.TryGetValue(orgId, out var orgInfo);
            if (orgInfo == null) return result;
            result.Add(orgId);
            result.AddRange(ExtractChildrenGuids(orgInfo, allowedPublishingStatusIds));
            return result;
        }

        private List<Guid> ExtractChildrenGuids(OrganizationTreeItem orgInfo, List<Guid> allowedPublishingStatusIds)
        {
            if (allowedPublishingStatusIds.Any())
            {
                orgInfo.Children = orgInfo.Children.Where(x => x.Organization != null && allowedPublishingStatusIds.Contains(x.Organization.PublishingStatusId)).ToList();
            }

            var result = orgInfo.Children.Select(i => i.UnificRootId).ToList();
            orgInfo.Children.ForEach(ch => result.AddRange(ExtractChildrenGuids(ch, allowedPublishingStatusIds)));
            return result;
        }

        public UserOrganizationsAndRolesResult GetAllUserOrganizationsAndRoles()
        {
            var allUserOrgs = GetAllUserOrganizationRoles();
            var orgData = organizationTreeDataCache.GetData();

// SOTE has been disabled (SFIPTV-1177)
//            var regionType = typesCache.Get<OrganizationType>(OrganizationTypeEnum.Region.ToString());

            return new UserOrganizationsAndRolesResult
            {
                OrganizationRoles = allUserOrgs,
                UserOrganizations = GetOrganizationNamesForIds(allUserOrgs.Select(x => x.OrganizationId).ToList()),

// SOTE has been disabled (SFIPTV-1177)
//                IsRegionUserOrganization = allUserOrgs.Any(x => orgData.TryGet(x.OrganizationId)?.Organization.TypeId == regionType)
            };

        }

        private IReadOnlyList<VmListItem> GetOrganizationNamesForIds(IList<Guid> organizationIds)
        {
            if (organizationIds.Count > 0)
            {
                var data = organizationTreeDataCache.GetFlatDataVm();
                return organizationIds.Select(x => data.TryGet(x)).Where(x => x != null).ToList();
            }
            return new List<VmListItem>();
        }

        public IReadOnlyList<IUserOrganizationRoles> GetAllUserOrganizationRoles(List<PublishingStatus> allowedPublishingStatuses = null)
        {
            var orgData = organizationTreeDataCache.GetData();
            var allowedPublishingStatusIds = allowedPublishingStatuses != null
                ? allowedPublishingStatuses.Select(publishingStatus => typesCache.Get<PublishingStatusType>(publishingStatus.ToString())).ToList()
                : new List<Guid>();

            if (allowedPublishingStatusIds.Any())
            {
                orgData = orgData.Where(x => x.Value?.Organization != null && allowedPublishingStatusIds.Contains(x.Value.Organization.PublishingStatusId))
                .ToDictionary(i => i.Key, i => i.Value);
            }
            var userOrgs = GetOrganizationsAndRolesForLoggedUser();
            var allUserOrgs = new List<UserOrganizationRoles>();
            userOrgs.ForEach(o =>
            {
                var subOrgs = GetAllChildrenForOrganization(orgData, o.OrganizationId, allowedPublishingStatusIds);
                allUserOrgs.AddRange(subOrgs.Select(i => new UserOrganizationRoles
                {
                    OrganizationId = i,
                    Role = o.Role,
                    IsMain = o.IsMain && i == o.OrganizationId
                }));
            });
//            var orgIds = allUserOrgs.Select(i => i.OrganizationId).ToList();
//            var orgNames = GetOrganizationNamesForIds(orgIds);
//            var intersect = orgIds.Intersect(orgNames.Select(i => i.Id)).ToList();

            return allUserOrgs;//.Where(i => intersect.Contains(i.OrganizationId)).ToList();
        }

        public IEnumerable<VmListItem> GetAllUserOrganizations(List<PublishingStatus> allowedPublishingStatuses = null)
        {
            return GetOrganizationNamesForIds(GetAllUserOrganizationIds(allowedPublishingStatuses));
        }

        public List<Guid> GetCoUsersOfUser(Guid userId, Guid? whereRoleId = null)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var query = userOrgRep.All().Where(i => i.UserId == userId);
                if (whereRoleId != null)
                {
                    query = query.Where(i => i.RoleId == whereRoleId.Value);
                }
                var userOrgids = query.Select(i => i.OrganizationId).ToList();
                if (userOrgids.IsNullOrEmpty()) return new List<Guid>();
                return userOrgRep.All().Where(i => userOrgids.Contains(i.OrganizationId)).Select(i => i.UserId).ToList();
            });
        }

        public List<Guid> GetAllCoAndSubUsers(IUnitOfWork unitOfWork, Guid userId, Guid? whereRoleId = null)
        {
            var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
            var userOrgs = userOrgRep.All().Where(i => i.UserId == userId && (whereRoleId == null || i.RoleId == whereRoleId)).Select(i => i.OrganizationId).ToList();
            GetAllSubOrganizations(userOrgs.ToList(), unitOfWork, ref userOrgs);
            var userCoUsers = userOrgRep.All().Where(i => userOrgs.Contains(i.OrganizationId)).Select(i => i.UserId).ToList();
            return userCoUsers;
        }

        public List<Guid> GetAllCoAndSubUsers(Guid userId, Guid? whereRoleId = null)
        {
            return contextManager.ExecuteReader(unitOfWork => GetAllCoAndSubUsers(unitOfWork, userId, whereRoleId));
        }


        private void GetAllSubOrganizations(List<Guid> parentIds, IUnitOfWork unitOfWork, ref List<Guid> resultOrgs)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var parentIdsNullable = parentIds.Cast<Guid?>().ToList();
            var result = orgRep.All().Where(i => parentIdsNullable.Contains(i.ParentId)).Select(x => x.UnificRootId).Distinct().ToList();
            var newOrgs = result.Except(resultOrgs).ToList();
            resultOrgs.AddRange(newOrgs);
            if (newOrgs.Any())
            {
                GetAllSubOrganizations(newOrgs, unitOfWork, ref resultOrgs);
            }
        }

        public List<SelectListItem> GetFullOrganizationsForIds(IList<Guid> orgIds)
        {
            if (orgIds.IsNullOrEmpty()) return new List<SelectListItem>();
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            return contextManager.ExecuteReader(unitOfWork =>
            {
                IQueryable<OrganizationVersioned> organizations = unitOfWork.CreateRepository<IOrganizationVersionedRepository>()
                    .All()
                    .Where(x => x.PublishingStatusId == psPublished || x.PublishingStatusId == psDraft)
                    .Include(i => i.OrganizationNames).ThenInclude(i => i.Localization)
                    .Include(i => i.OrganizationDisplayNameTypes).ThenInclude(i => i.Localization);
                return organizations.Where(i => orgIds.Contains(i.UnificRootId)).ToList()
                    .Select(organizaton => new SelectListItem { Value = organizaton.UnificRootId.ToString(), Text = GetOrganizationDisplayName(organizaton) })
                    .OrderBy(x => x.Text).ToList();
            });
        }

        /// <summary>
        /// Tries to get organization display name in Finnish and then fallbacks to other languages.
        /// </summary>
        /// <param name="organization">instance of OrganizationVersioned</param>
        /// <returns>Organization display name if found, otherwise null.</returns>
        private string GetOrganizationDisplayName(OrganizationVersioned organization)
        {
            string orgDisplayName = null;

            if (organization?.OrganizationNames != null && organization.OrganizationNames.Count > 0 && organization.OrganizationDisplayNameTypes != null && organization.OrganizationDisplayNameTypes.Count > 0)
            {
                // assumption is that fi language code is always returned first as specified in the enum
                var languageCodes = languageCache.AllowedLanguageCodes;
                Guid? displayNameType = null;
                string languageCode = null;

                foreach (var lc in languageCodes)
                {
                    displayNameType = organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.Localization.Code == lc)?.DisplayNameTypeId;

                    if (!displayNameType.HasValue) continue;
                    // found displaynametype for a language, exit the foreach
                    languageCode = lc;
                    break;
                }

                // just a check that a displaynametype has been found for some language, try to get the displayname with type and language
                if (displayNameType.HasValue)
                {
                    orgDisplayName = organization.OrganizationNames.FirstOrDefault(orgName => orgName.TypeId == displayNameType.Value && orgName.Localization.Code == languageCode)?.Name;
                }
            }

            return orgDisplayName;
        }

        public Dictionary<Guid, Guid> GetUserDirectlyAssignedOrganizations(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                return userOrgRep.All().Where(i => i.UserId == userId).ToDictionary(i => i.OrganizationId, i => i.RoleId);
            });
        }

        public List<Guid> GetAllOrganizations()
        {
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var psPublish = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                return userOrgRep.All().Where(i => i.PublishingStatusId == psDraft || i.PublishingStatusId == psPublish).Select(i => i.UnificRootId).ToList();
            });
        }

        public List<IUserOrganizationRoleDefinition> GetUserOrganizationsWithRoles(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var psPublish = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var userData = userOrgRep.All().Where(i => i.UserId == userId && i.Organization.Versions.Any(o => o.PublishingStatusId == psPublish)).Select(i => new { i.OrganizationId, i.RoleId, i.IsMain }).ToList();
                return userData.Select(i => new UserOrganizationRoleDefinition { OrganizationId = i.OrganizationId, UserId = userId, RoleId = i.RoleId, IsMain = i.IsMain }).ToList<IUserOrganizationRoleDefinition>();
            });
        }

        public void MapUserToOrganization(Guid userId, List<OrganizationRole> organizationsRoles)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var currentUserMaps = connectionRep.All().Where(u => u.UserId == userId).ToList();
                var orgIds = organizationsRoles.Select(i => i.OrganizationId).ToList();
                var toDelete = currentUserMaps.Where(i => !orgIds.Contains(i.OrganizationId));
                toDelete.ForEach(c => connectionRep.Remove(c));
                var toAdd = organizationsRoles.Where(j => !currentUserMaps.Select(i => i.OrganizationId).Contains(j.OrganizationId)).ToList();
                toAdd.ForEach(c => connectionRep.Add(new UserOrganization { OrganizationId = c.OrganizationId, RoleId = c.RoleId, UserId = userId }));
                var toUpdate = organizationsRoles.Where(j => currentUserMaps.Select(i => i.OrganizationId).Contains(j.OrganizationId)).ToList();
                toUpdate.ForEach(c =>
                {
                    var userOrg = currentUserMaps.FirstOrDefault(i => i.OrganizationId == c.OrganizationId) ?? new UserOrganization { UserId = userId };
                    userOrg.OrganizationId = c.OrganizationId;
                    userOrg.RoleId = c.RoleId;
                });
                unitOfWork.Save();
            });
        }

        public List<Guid> GetUserCompleteOrgList(IUnitOfWork unitOfWork, Guid userId)
        {
            var userOrgs = unitOfWork.CreateRepository<IUserOrganizationRepository>().All().Where(i => i.UserId == userId).Select(i => i.OrganizationId).ToList();
            var subOrgIds = userOrgs.ToList();
            GetAllSubOrganizations(userOrgs.ToList(), unitOfWork, ref subOrgIds);
            return subOrgIds;
        }


        public List<VmUserOrganizationSts> GetUsersWithOrganizations(IList<Guid> usersIds = null)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var query = usersIds == null ? connectionRep.All() : connectionRep.All().Where(i => usersIds.Contains(i.UserId));

                var users = unitOfWork.ApplyIncludes(query, q =>
                        q.Include(i => i.Organization)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.PublishingStatus)
                            .Include(i => i.Organization)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.OrganizationNames)
                            .ThenInclude(i => i.Localization)
                            .Include(i => i.Organization)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.OrganizationDisplayNameTypes)
                            .ThenInclude(i => i.Localization))
                            .Include(i => i.Organization)
                            .ThenInclude(i => i.Versions)
                            .ThenInclude(i => i.Business)
                    .ToList();

                var result = users.Select(x =>
                {
                    var organization = x.Organization?.Versions.OrderBy(y => y.PublishingStatus.PriorityFallback).FirstOrDefault();

                    return new VmUserOrganizationSts
                    {
                        UserId = x.UserId,
                        RoleId = x.RoleId,
                        IsParent = !organization?.ParentId.HasValue ?? false,
                        OrganizationId = x.OrganizationId,
                        BusinessCode = organization?.Business?.Code,
                        OrganizationName = organization?.OrganizationNames.FirstOrDefault(j => j.TypeId == organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.Localization.Code == DomainConstants.DefaultLanguage)?.DisplayNameTypeId && j.Localization.Code == DomainConstants.DefaultLanguage)?.Name
                    };
                }).OrderBy(x => x.UserName).ToList();

                return result;
            });
        }

        public List<List<IUserOrganizationRoles>> GetUserCompleteOrgStructure(IUnitOfWork unitOfWork)
        {
            var orgRoles = GetOrganizationsAndRolesForLoggedUser();
            var result = new List<List<IUserOrganizationRoles>>();
            foreach (var orgs in orgRoles.GroupBy(x => x.Role).Select(x => x))
            {
                var subOrgIds = orgs.Select(x => x.OrganizationId).ToList();
                GetAllSubOrganizations(subOrgIds.ToList(), unitOfWork, ref subOrgIds);
                var subOrgs = subOrgIds.Select(i => new UserOrganizationRoles { OrganizationId = i, Role = orgs.First().Role }).ToList<IUserOrganizationRoles>();
                if (subOrgs.Any())
                {
                    result.Add(subOrgs);
                }
            }

            return result;
        }

        public List<List<IUserOrganizationRoles>> GetUserCompleteOrgStructure()
        {
            return contextManager.ExecuteReader(GetUserCompleteOrgStructure);
        }
    }

    [RegisterService(typeof(IUserOrganizationChecker), RegisterType.Transient)]
    internal class UserOrganizationChecker : IUserOrganizationChecker
    {
        private IResolveManager resolveManager;
        private IUserOrganizationService userOrganizationService;

        public UserOrganizationChecker(IResolveManager resolveManager, IUserOrganizationService userOrganizationService)
        {
            this.resolveManager = resolveManager;
            this.userOrganizationService = userOrganizationService;
        }

        public ISecurityOwnOrganization CheckEntity<T>(T entity, IUnitOfWork unitOfWork) //where T : IRoleBased, IEntityIdentifier
        {
            ISecurityOwnOrganization result = null;
            var checker = resolveManager.Resolve<IUserOrganizationChecker<T>>(true);
            if (checker != null)
            {
                var organizations = userOrganizationService.GetAllUserOrganizationIds();
                result = checker.CheckEntity(entity, unitOfWork, organizations);
            }
            return result;// ?? new VmSecurityOwnOrganization { Id = entity.Id };
        }

    }

}
