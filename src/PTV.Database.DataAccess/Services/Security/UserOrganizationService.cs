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
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(IUserOrganizationService), RegisterType.Transient)]
    internal class UserOrganizationService : IUserOrganizationService
    {
        private IUserIdentification userIdentification;
        private ITypesCache typesCache;

        public UserOrganizationService(IUserIdentification userIdentification, ITypesCache typesCache)
        {
            this.userIdentification = userIdentification;
            this.typesCache = typesCache;
        }

        public Guid? GetUserOrganizationId(IUnitOfWork unitOfWork)
        {
            var userRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
            var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
            var userName = userIdentification.UserName.ToLower();
            var result = userRep.All().FirstOrDefault(i => i.UserName.ToLower() == userName)?.OrganizationId;
            // check if organization exists
//            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            if (result.IsAssigned() && !organizationRep.All().Any(x => x.Id == result && x.Versions.Any(j => j.PublishingStatusId == psPublished)))
            {
                return null;
            }
            return result;
        }

//        internal OrganizationVersioned GetUserOrganization(IUnitOfWork unitOfWork)
//        {
//            var userOrganizationsRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
//            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
//            var userName = userIdentification.UserName.ToLower();
//            var preselectedOrgId = userOrganizationsRep.All().FirstOrDefault(x => x.UserName.ToLower() == userName)?.OrganizationId;
//            return preselectedOrgId.HasValue ? organizationRep.All().FirstOrDefault(x => x.UnificRootId == preselectedOrgId.Value) : null;
//        }

        private IList<Guid> GetAllSubOrganizations(Guid parentId, IUnitOfWork unitOfWork)
        {
            var orgRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var result = orgRep.All().Where(i => i.ParentId == parentId).Select(x => x.UnificRootId).Distinct().ToList();
            var subOrgIds = new List<Guid>();
            foreach (var id in result)
            {
                subOrgIds.AddRange(GetAllSubOrganizations(id, unitOfWork));
            }
            result.AddRange(subOrgIds);
            return result.ToList();
        }

        public IList<Guid> GetAllUserOrganizations(IUnitOfWork unitOfWork)
        {
            var userOrgId = GetUserOrganizationId(unitOfWork);
            if (!userOrgId.IsAssigned())
            {
                return new List<Guid>();
            }
            var subOrgIds = GetAllSubOrganizations(userOrgId.Value, unitOfWork).ToList();
            subOrgIds.Add(userOrgId.Value);
            return subOrgIds;
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
