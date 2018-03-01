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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(ServiceCollectionVersionedRoleChecker), RegisterType.Transient)]
    internal class ServiceCollectionVersionedRoleChecker : RoleCheckerBase
    {

        private readonly IUserOrganizationService userOrganizationService;
        private readonly IUserIdentification userIdentification;

        public ServiceCollectionVersionedRoleChecker(IUserIdentification userIdentification,
            IHttpContextAccessor ctxAccessor,
            IUserOrganizationService userOrganizationService) : base(ctxAccessor)
        {
            this.userOrganizationService = userOrganizationService;
            this.userIdentification = userIdentification;
        }

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var checkStatus = false;
            var userOrgStruct = GetUserCompleteOrgStructure(this.userOrganizationService, unitOfWork);
            var serviceCollection = entity as ServiceCollectionVersioned;
            var isAdd = IsAdded(entityEntry);

            if (serviceCollection != null)
            {            
                var prevOrganizationId = GetOriginalValue<Guid?>(entityEntry, "OrganizationId");               
                var role = userOrgStruct.SelectMany(x => x).Select(y => y.Role).OrderBy(i => i).FirstOrDefault();
                var userOrganizations = userOrgStruct.SelectMany(x => x).Select(i => i.OrganizationId).ToList();

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
                                
                            checkStatus = CheckAllIds(userOrganizations, new List<Guid?> { serviceCollection.OrganizationId, prevOrganizationId });
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
                ThrowError(userIdentification, userOrgStruct.SelectMany(i => i).Select(j => j.Role).Distinct());
            }
        }
    }
}
