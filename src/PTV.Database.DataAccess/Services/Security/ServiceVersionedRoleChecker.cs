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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(ServiceVersionedRoleChecker), RegisterType.Transient)]
    internal class ServiceVersionedRoleChecker : RoleCheckerBase
    {

        private readonly IUserOrganizationService userOrganizationService;
        private readonly IUserIdentification userIdentification;

        public ServiceVersionedRoleChecker(IUserIdentification userIdentification, IUserOrganizationService userOrganizationService, IUserInfoService userInfoService, ITypesCache typesCache) : base(userOrganizationService, userInfoService, typesCache)
        {
            this.userOrganizationService = userOrganizationService;
            this.userIdentification = userIdentification;
        }

        protected override DomainEnum Domain => DomainEnum.Services;

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var checkStatus = false;
            var service = entity as ServiceVersioned;
            
            // Have to add check for userOrgStruct. If there is no items within userOrgStruck role will always be Eeva because of call for FirstOrDefault() (see below)!
            if (service != null)
            {            
                var prevOrganizationId = GetOriginalValue<Guid?>(entityEntry, "OrganizationId");               
                checkStatus = CheckEntityRoles
                (
                    entity,
                    entityEntry,
                    prevOrganizationId.IsAssigned() ? new List<Guid?> { service.OrganizationId, prevOrganizationId } : new List<Guid?> { service.OrganizationId }
                );

            }
            if (!checkStatus)
            {
                ThrowError(userIdentification);
            }
        }
    }
}
