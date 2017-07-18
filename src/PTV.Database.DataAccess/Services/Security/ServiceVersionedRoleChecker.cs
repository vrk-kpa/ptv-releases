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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(ServiceVersionedRoleChecker), RegisterType.Transient)]
    internal class ServiceVersionedRoleChecker : RoleCheckerBase
    {

        private readonly IUserOrganizationService userOrganizationService;
        private readonly IUserIdentification userIdentification;
        private readonly ITypesCache typesCache;

        public ServiceVersionedRoleChecker(IUserIdentification userIdentification,
            IHttpContextAccessor ctxAccessor,
            IUserOrganizationService userOrganizationService,
            ITypesCache typesCache) : base(ctxAccessor)
        {
            this.userOrganizationService = userOrganizationService;
            this.typesCache = typesCache;
            this.userIdentification = userIdentification;
        }

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var checkStatus = false;
            var role = GetClaimRole();
            var service = entity as ServiceVersioned;
            var isAdd = IsAdded(entityEntry);

            if (service != null)
            {

                var userOrganizations = userOrganizationService.GetAllUserOrganizations(unitOfWork);
                var savedServiceOrg = GetSavedServiceOrganizationIds(service, unitOfWork);
                var serviceOrg = IsServiceOrganizationTouched(entityEntry) ?
                    GetServiceOrganizationIds(entityEntry, service.Id) :
                    savedServiceOrg;

                switch (role)
                {
                    case UserRoleEnum.Eeva:
                    {
                        checkStatus = true;
                        break;
                    }
                    case UserRoleEnum.Pete:
                    case UserRoleEnum.Shirley:
                    {
                        //can modify
                        checkStatus = CheckAnyIds(userOrganizations, savedServiceOrg) || (isAdd);// && service.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString()));
                        //can save
                        checkStatus = checkStatus && CheckAllIds(userOrganizations, savedServiceOrg.Except(serviceOrg));
                        checkStatus = checkStatus && CheckAllIds(userOrganizations, serviceOrg.Except(savedServiceOrg));
                        checkStatus = checkStatus && CheckAnyIds(userOrganizations, serviceOrg);
                        break;
                    }
                    default:
                    {
                        checkStatus = false;
                        break;
                    }
                }

            }
            if (!checkStatus)
            {
                ThrowError(userIdentification, role);
            }
        }

        private List<Guid?> GetServiceOrganizationIds(EntityEntry<IRoleBased> entityEntry, Guid serviceId)
        {
            return entityEntry.Context.ChangeTracker.Entries<Model.Models.OrganizationService>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Unchanged || x.State == EntityState.Modified)
                .Where(x => x.Entity.ServiceVersionedId == serviceId && x.Entity.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString()))
                .Select(x => x.Entity.OrganizationId)
                .ToList();
        }
        private bool IsServiceOrganizationTouched(EntityEntry<IRoleBased> entityEntry)
        {
            return entityEntry != null ?
                entityEntry.Context.ChangeTracker.Entries<Model.Models.OrganizationService>().Any() :
                false;
        }
        private List<Guid?> GetSavedServiceOrganizationIds(ServiceVersioned service, IUnitOfWorkWritable unitOfWork)
        {
            var rep = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
            var id = unitOfWork.TranslationCloneCache.GetFromCachedSet<ServiceVersioned>()
                .Where(x => x.ClonedEntity.Id == service.Id).Select(x => x.OriginalEntity.Id).SingleOrDefault();
            id = id.IsAssigned() ? id : service.Id;
            var ids = rep.All()
                .Where(x => x.ServiceVersionedId == id)
                .Where(x => x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString()))
                .Select(x => x.OrganizationId)
                .Distinct()
                .ToList();
            return ids ?? new List<Guid?>();
        }
    }
}
