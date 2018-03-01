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
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(IUserOrganizationService), RegisterType.Transient)]
    internal class UserOrganizationService : IUserOrganizationService
    {
        private ITypesCache typesCache;
        private IContextManager contextManager;
        private IHttpContextAccessor httpContextAccessor;
        private ITranslationEntity translationManagerToVm;
        private IOrganizationTreeDataCache organizationTreeDataCache;

        public UserOrganizationService(ICacheManager cacheManager, IContextManager contextManager, IHttpContextAccessor httpContextAccessor, ITranslationEntity translationManagerToVm, IOrganizationTreeDataCache organizationTreeDataCache)
        {
            this.typesCache = cacheManager.TypesCache;
            this.contextManager = contextManager;
            this.httpContextAccessor = httpContextAccessor;
            this.translationManagerToVm = translationManagerToVm;
            this.organizationTreeDataCache = organizationTreeDataCache;
        }

        public List<Guid> LoadOrganizationTree(IEnumerable<Guid> rootIds)
        {
            return contextManager.ExecuteReader(unitOfWork => LoadOrganizationTree(unitOfWork, rootIds));
        }

        public List<Guid> LoadOrganizationTree(IUnitOfWork unitOfWork, IEnumerable<Guid> rootIds)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var publishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var rootIdsNullable = rootIds.Cast<Guid?>().ToList();
            var subRootIds = orgRep.All().Where(i => ((i.PublishingStatusId == publishedId) || (i.PublishingStatusId == draftId)) && rootIdsNullable.Contains(i.ParentId))
                .Select(i => i.UnificRootId).ToList().Except(rootIds).ToList();
            var result = rootIds.Union(subRootIds).ToList();
            if (subRootIds.Any())
            {
                LoadOrganizationTreeRecursive(orgRep, ref result, subRootIds, publishedId, draftId);
            }
            return result;
        }

        private void LoadOrganizationTreeRecursive(IOrganizationVersionedRepository orgRep, ref List<Guid> result, IList<Guid> rootIds, Guid publishedId, Guid draftId)
        {
            var rootIdsNullable = rootIds.Cast<Guid?>();
            var subRootIds = orgRep.All().Where(i => ((i.PublishingStatusId == publishedId) || (i.PublishingStatusId == draftId)) && rootIdsNullable.Contains(i.ParentId)).Select(i => i.UnificRootId).ToList().Except(rootIds).Except(result).ToList();
            if (subRootIds.Any())
            {
                result.AddRange(subRootIds);
                LoadOrganizationTreeRecursive(orgRep, ref result, subRootIds, publishedId, draftId);
            }
        }


        public IList<Guid> GetUserOrganizationId(IUnitOfWork unitOfWork)
        {
            return GetOrganizationsIdsLoggedUser();
        }


        public List<Guid> GetUsersMissingRoles()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                return connectionRep.All().Where(u => u.RoleId == Guid.Empty).Select(i => i.UserId).ToList();
            });
        }

        public void ReAssignMissingRoles(Guid userId, Guid preferedRole)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var wrongConnections = connectionRep.All().Where(u => u.UserId == userId && u.RoleId == Guid.Empty).ToList();
                wrongConnections.ForEach(c => c.RoleId = preferedRole);
                unitOfWork.Save();
            });
        }

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationsRoles"></param>
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
                toAdd.ForEach(c => connectionRep.Add(new UserOrganization() {OrganizationId = c.OrganizationId, RoleId = c.RoleId, UserId = userId}));
                var toUpdate = organizationsRoles.Where(j => currentUserMaps.Select(i => i.OrganizationId).Contains(j.OrganizationId)).ToList();
                toUpdate.ForEach(c =>
                {
                    var userOrg = currentUserMaps.FirstOrDefault(i => i.OrganizationId == c.OrganizationId) ?? new UserOrganization() { UserId = userId };
                    userOrg.OrganizationId = c.OrganizationId;
                    userOrg.RoleId = c.RoleId;
                });
                unitOfWork.Save();
            });
        }

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        public List<OrganizationRole> GetMapOfUserAndOrganizations(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var currentUserMaps = connectionRep.All().Where(u => u.UserId == userId).ToList();
                return currentUserMaps.Select(i => new OrganizationRole() {OrganizationId = i.OrganizationId, RoleId = i.RoleId}).ToList();
            });
        }

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        public bool SetUserMainOrganization(Guid userId, Guid organizationId)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var currentActive = connectionRep.All().Where(u => u.UserId == userId && u.IsMain).ToList();
                currentActive.ForEach(i => i.IsMain = false);
                var toSet = connectionRep.All().FirstOrDefault(u => u.UserId == userId && u.OrganizationId == organizationId);
                toSet.SafeCall(i => i.IsMain = true);
                unitOfWork.Save();
                return toSet != null;
            });
        }

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        public Guid? GetUserMainOrganization(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var connectionRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var currentActive = connectionRep.All().FirstOrDefault(u => u.UserId == userId && u.IsMain);
                return currentActive?.OrganizationId;
            });
        }

        public List<VmUserOrganization> GetUsersWithOrganizations(IList<Guid> usersIds = null)
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
                    .ToList();

                var result = users.Select(x =>
                {
                    var organization = x.Organization?.Versions.OrderBy(y => y.PublishingStatus.PriorityFallback).FirstOrDefault();

                    return new VmUserOrganization()
                    {
                        UserId = x.UserId,
                        RoleId = x.RoleId,
                        OrganizationId = x.Organization?.Id,
                        OrganizationName = organization?.OrganizationNames.FirstOrDefault(j => j.TypeId == organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.Localization.Code == LanguageCode.fi.ToString())?.DisplayNameTypeId && j.Localization.Code == LanguageCode.fi.ToString())?.Name
                    };
                }).OrderBy(x => x.UserName).ToList();

                return result;
            });
        }

        public Dictionary<Guid, Guid> GetUserOrganizations(Guid userId, Dictionary<Guid, int> rolePriorityMap = null)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var directUserMap = userOrgRep.All().Where(i => i.UserId == userId).ToList().GroupBy(i => i.RoleId).ToList();
                var priorityMap = rolePriorityMap ?? new Dictionary<Guid, int>();
                directUserMap.Select(i => i.Key).Distinct().Except(priorityMap.Keys).ToList().ForEach(i => priorityMap.Add(i, int.MaxValue));
                var result = new Dictionary<Guid, Guid>();
                directUserMap.OrderBy(i => priorityMap[i.Key]).ForEach(i =>
                {
                    var roleRelatedOrgsIds = LoadOrganizationTree(unitOfWork, i.Select(j => j.OrganizationId).ToList()).Distinct().ToDictionary(j => j, j => i.Key);
                    result = result.Merge(roleRelatedOrgsIds);
                });
                return result;
            });
        }

        public Dictionary<Guid, Guid> GetUserDirectlyAssignedOrganizations(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                return userOrgRep.All().Where(i => i.UserId == userId).ToDictionary(i => i.OrganizationId, i=> i.RoleId);
            });
        }

        public List<IUserOrganizationRoleDefinition> GetUserOrganizationsWithRoles(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var psPublish = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                var userData = userOrgRep.All().Where(i => i.UserId == userId && i.Organization.Versions.Any(o=>o.PublishingStatusId == psPublish)).Select(i => new { i.OrganizationId, i.RoleId, i.IsMain }).ToList();
                return userData.Select(i => new UserOrganizationRoleDefinition() {OrganizationId = i.OrganizationId, UserId = userId, RoleId = i.RoleId, IsMain = i.IsMain }).ToList<IUserOrganizationRoleDefinition>();
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

        public bool IsUserAssignedToOrganization(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
                return userOrgRep.All().Any(i => i.UserId == userId);
            });
        }

        public List<Guid> GetOrganizationsIdsLoggedUser()
        {
            var userOrganizationsClaim = httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == PtvClaims.UserOrganizations);
            return !string.IsNullOrEmpty(userOrganizationsClaim?.Value) ? userOrganizationsClaim.Value.ToLower().Split(',').Select(i => i.Split('=').First().ParseToGuid()).Where(i => i != null).Cast<Guid>().ToList() : new List<Guid>();
        }

        public List<IUserOrganizationRoles> GetOrganizationsAndRolesForLoggedUser()
        {
            var userOrganizationsClaim = httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == PtvClaims.UserOrganizations);
            return !string.IsNullOrEmpty(userOrganizationsClaim?.Value)
                ? userOrganizationsClaim.Value.ToLower().Split(',').Select(i => i.Split('=')).Where(i => i.Length == 2)
                    .Select(i =>
                    {
                        var orgId = i[0].ParseToGuid() ?? Guid.Empty;
                        var access = i[1].Split(':');
                        var role = (UserRoleEnum) Enum.Parse(typeof(UserRoleEnum), access[0], true);
                        var isMain = (access.Length == 2 && access[1] == "*");
                        return new UserOrganizationRoles() {OrganizationId = orgId, Role = role, IsMain = isMain};
                    })
                    .Where(i => i.OrganizationId.IsAssigned()).ToList<IUserOrganizationRoles>()
                : new List<IUserOrganizationRoles>();
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
                    .Select(organizaton => new SelectListItem() { Value = organizaton.UnificRootId.ToString(), Text = GetOrganizationDisplayName(organizaton) })
                    .OrderBy(x => x.Text).ToList();
            });
        }

        public List<SelectListItem> GetOrganizationsFullLoggedUser()
        {
            var orgIds = GetOrganizationsIdsLoggedUser();
            if (orgIds.IsNullOrEmpty()) return new List<SelectListItem>();
            return GetFullOrganizationsForIds(orgIds);
        }

        /// <summary>
        /// Tries to get organization display name in Finnish and then fallbacks to other languages.
        /// </summary>
        /// <param name="organization">instance of OrganizationVersioned</param>
        /// <returns>Organization display name if found, otherwise null.</returns>
        private static string GetOrganizationDisplayName(OrganizationVersioned organization)
        {
            string orgDisplayName = null;

            if (organization != null && organization.OrganizationNames != null && organization.OrganizationNames.Count > 0 && organization.OrganizationDisplayNameTypes != null && organization.OrganizationDisplayNameTypes.Count > 0)
            {
                // assumption is that fi language code is always returned first as specified in the enum
                var languageCodes = Enum.GetNames(typeof(LanguageCode));

                Guid? displayNameType = null;
                string languageCode = null;

                foreach (var lc in languageCodes)
                {
                    displayNameType = organization.OrganizationDisplayNameTypes.FirstOrDefault(type => type.Localization.Code == lc)?.DisplayNameTypeId;

                    if (displayNameType.HasValue)
                    {
                        // found displaynametype for a language, exit the foreach
                        languageCode = lc;
                        break;
                    }
                }

                // just a check that a displaynametype has been found for some language, try to get the displayname with type and language
                if (displayNameType.HasValue)
                {
                    orgDisplayName = organization.OrganizationNames.FirstOrDefault(orgName => orgName.TypeId == displayNameType.Value && orgName.Localization.Code == languageCode)?.Name;
                }
            }

            return orgDisplayName;
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

        //        internal OrganizationVersioned GetUserOrganization(IUnitOfWork unitOfWork)
        //        {
        //            var userOrganizationsRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
        //            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
        //            var userName = userIdentification.UserName.ToLower();
        //            var preselectedOrgId = userOrganizationsRep.All().FirstOrDefault(x => x.UserName.ToLower() == userName)?.OrganizationId;
        //            return preselectedOrgId.HasValue ? organizationRep.All().FirstOrDefault(x => x.UnificRootId == preselectedOrgId.Value) : null;
        //        }

        //        private IList<Guid> GetAllSubOrganizations(Guid parentId, IUnitOfWork unitOfWork)
        //        {
        //            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
        //            var result = orgRep.All().Where(i => i.ParentId == parentId).Select(x => x.UnificRootId).Distinct().ToList();
        //            var subOrgIds = new List<Guid>();
        //            foreach (var id in result)
        //            {
        //                subOrgIds.AddRange(GetAllSubOrganizations(id, unitOfWork));
        //            }
        //            result.AddRange(subOrgIds);
        //            return result.ToList();
        //        }

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
        public IList<Guid> GetAllUserOrganizations()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetAllUserOrganizations(unitOfWork);
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
            return contextManager.ExecuteReader(u => GetAllCoAndSubUsers(u, userId, whereRoleId));
        }


        public IList<Guid> GetAllUserOrganizations(IUnitOfWork unitOfWork)
        {
            return GetUserCompleteOrgStructure(unitOfWork).SelectMany(i => i).Select(i => i.OrganizationId).ToList();
        }

        private List<Guid> GetAllChildrenForOrganization(Dictionary<Guid, OrganizationTreeItem> data, Guid orgId)
        {
            var result = new List<Guid>();
            data.TryGetValue(orgId, out var orgInfo);
            if (orgInfo == null) return result;
            result.Add(orgId);
            result.AddRange(ExtractChildrenGuids(orgInfo));
            return result;
        }

        private List<Guid> ExtractChildrenGuids(OrganizationTreeItem orgInfo)
        {
            var result = orgInfo.Children.Select(i => i.UnificRootId).ToList();
            orgInfo.Children.ForEach(ch => result.AddRange(ExtractChildrenGuids(ch)));
            return result;
        }

        public UserOrganizationsAndRolesResult GetAllUserOrganizationsAndRoles()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var orgData = organizationTreeDataCache.GetData();
                var userOrgs = GetOrganizationsAndRolesForLoggedUser();
                var allUserOrgs = new List<UserOrganizationRoles>();
                userOrgs.ForEach(o =>
                {
                    var subOrgs = GetAllChildrenForOrganization(orgData, o.OrganizationId);
                    allUserOrgs.AddRange(subOrgs.Select(i => new UserOrganizationRoles() {OrganizationId = i, Role = o.Role, IsMain = o.IsMain && i == o.OrganizationId}));
                });
                var orgIds = allUserOrgs.Select(i => i.OrganizationId).ToList();
                var orgNames = GetOrganizationNamesForIds(unitOfWork, orgIds);
                var intersect = orgIds.Intersect(orgNames.Select(i => i.Id)).ToList();

                return new UserOrganizationsAndRolesResult()
                {
                    OrganizationRoles = allUserOrgs.Where(i => intersect.Contains(i.OrganizationId)).ToList(),
                    UserOrganizations = orgNames.Where(i => intersect.Contains(i.Id)).ToList(),
                };
            });
        }


        public IReadOnlyList<VmListItem> GetOrganizationNamesForIds(IUnitOfWork unitOfWork, IList<Guid> organizationIds)
        {
            var organizationRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var resultTemp = organizationRepository.All().Where(x => x.PublishingStatusId == psPublished && organizationIds.Contains(x.UnificRootId));
            resultTemp = unitOfWork.ApplyIncludes(resultTemp, q => q.Include(i => i.OrganizationNames).ThenInclude(i => i.Localization).Include(i => i.OrganizationDisplayNameTypes).Include(i => i.LanguageAvailabilities));
            return translationManagerToVm.TranslateAll<OrganizationVersioned, VmListItem>(resultTemp);
        }

        public List<List<IUserOrganizationRoles>> GetUserCompleteOrgStructure(IUnitOfWork unitOfWork)
        {
            var orgRoles = GetOrganizationsAndRolesForLoggedUser();
            var result = new List<List<IUserOrganizationRoles>>();
            foreach (var orgs in orgRoles.GroupBy(x => x.Role).Select(x => x))
            {
                var subOrgIds = orgs.Select(x => x.OrganizationId).ToList();
                GetAllSubOrganizations(subOrgIds.ToList(), unitOfWork, ref subOrgIds);
                var subOrgs = subOrgIds.Select(i => new UserOrganizationRoles() {OrganizationId = i, Role = orgs.First().Role}).ToList<IUserOrganizationRoles>();
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

        public List<Guid> GetUserCompleteOrgList(IUnitOfWork unitOfWork, Guid userId)
        {
            var userOrgs = unitOfWork.CreateRepository<IUserOrganizationRepository>().All().Where(i => i.UserId == userId).Select(i => i.OrganizationId).ToList();
            var subOrgIds = userOrgs.ToList();
            GetAllSubOrganizations(userOrgs.ToList(), unitOfWork, ref subOrgIds);
            return subOrgIds;
        }

        public List<Guid> GetUserCompleteOrgList(Guid userId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var userOrgs = unitOfWork.CreateRepository<IUserOrganizationRepository>().All().Where(i => i.UserId == userId).Select(i => i.OrganizationId).ToList();
                var subOrgIds = userOrgs.ToList();
                GetAllSubOrganizations(userOrgs.ToList(), unitOfWork, ref subOrgIds);
                return subOrgIds;
            });
        }
    }

    [RegisterService(typeof(IUserOrganizationChecker), RegisterType.Scope)]
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
                var organizations = userOrganizationService.GetAllUserOrganizations(unitOfWork);
                result = checker.CheckEntity(entity, unitOfWork, organizations);
            }
            return result;// ?? new VmSecurityOwnOrganization { Id = entity.Id };
        }

    }

}
