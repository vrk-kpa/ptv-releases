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
using PTV.Framework;
using Microsoft.AspNetCore.Http;
using PTV.Database.Model.Models;
using PTV.Framework.ServiceManager;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// Organization role checker
    /// </summary>
    [RegisterService(typeof(OrganizationRoleChecker), RegisterType.Transient)]
    internal class OrganizationRoleChecker : RoleCheckerBase
    {
        private IUserOrganizationService userOrganizationService;
        private IUserIdentification userIdentification;
        private ITypesCache typesCache;

        public OrganizationRoleChecker(
            IUserIdentification userIdentification,
            IHttpContextAccessor ctxAccessor,
            IUserOrganizationService userOrganizationService, ITypesCache typesCache) : base(ctxAccessor)
        {
            this.userOrganizationService = userOrganizationService;
            this.userIdentification = userIdentification;
            this.typesCache = typesCache;
        }

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var checkStatus = true;
            var role = GetClaimRole();
            var organization = entity as OrganizationVersioned;

            if (organization != null)
            {

                var userOrganizations = userOrganizationService.GetAllUserOrganizations(unitOfWork);
                var isAdd = IsAdded(entityEntry) && typesCache.GetByValue<PublishingStatusType>(organization.PublishingStatusId) != PublishingStatus.Modified.ToString();
                var prevParentId = GetOriginalValue<Guid?>(entityEntry, "ParentId");
                var prevPublishingStatusId = GetOriginalValue<Guid?>(entityEntry, "PublishingStatusId");
                switch (role)
                {
                    case UserRoleEnum.Eeva:
                        {
                            checkStatus = true;
                            break;
                        }
                    case UserRoleEnum.Pete:
                        {
                            //Check parents
                            if (isAdd) //can add
                            {
                                checkStatus = CheckAllIds(userOrganizations, new List<Guid?> { organization.ParentId });
                            }
                            else
                            {
                                if (organization.ParentId != prevParentId) //can manage structure 
                                {
                                    checkStatus = CheckAllIds(userOrganizations, new List<Guid?> { organization.ParentId, prevParentId });
                                }
                            }

                            //check update
                            if (checkStatus && !isAdd)
                            {
                                checkStatus = CheckAllIds(userOrganizations, new List<Guid?> { organization.UnificRootId });
                            }
                            //check add
                            if (checkStatus && isAdd)
                            {
                                checkStatus = organization.ParentId.HasValue;
                            }
                            //can not create main org
                            if (checkStatus && !isAdd && prevParentId != null)
                            {
                                checkStatus = organization.ParentId.HasValue;
                            }
                            break;
                        }
                    case UserRoleEnum.Shirley:
                        {
                            checkStatus = CheckAllIds(userOrganizations, new List<Guid?> { organization.UnificRootId});

                            //can not add organization
                            checkStatus = checkStatus && !isAdd;
                            //can not change structure
                            if (checkStatus && entityEntry != null)
                            {
                                checkStatus = checkStatus && organization.ParentId == prevParentId;
                            }
                            //PTV-2044 - code commented, based on the task shirley should be able to publish the organization
                            //can not change publishing status
                            //if (checkStatus && prevPublishingStatusId.HasValue)
                            //{
                            //    checkStatus = checkStatus && organization.PublishingStatusId == prevPublishingStatusId;
                            //}
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
    }
}
