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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using UserRoleEnum = PTV.Domain.Model.Enums.Security.UserRoleEnum;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// ServiceChannel role checker
    /// </summary>
    [RegisterService(typeof(ChannelRoleChecker), RegisterType.Transient)]
    internal class ChannelRoleChecker : RoleCheckerBase
    {
        private IUserOrganizationService userOrganizationService;
        private IUserIdentification userIdentification;
        private ITypesCache typesCache;

        public ChannelRoleChecker(
            IUserIdentification userIdentification,
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
            var userOrgStruct = GetUserCompleteOrgStructure(this.userOrganizationService, unitOfWork);
            var channel = entity as ServiceChannelVersioned;
            var checkStatus = false;

            if (channel != null)
            {
                //var userOrganizations = userOrganizationService.GetAllUserOrganizations(unitOfWork);
                //var isAdd = IsAdded(entityEntry);
                var prevOrganizationId = GetOriginalValue<Guid?>(entityEntry, "OrganizationId");
                foreach (var oneOrgTree in userOrgStruct)
                {
                    checkStatus = false;

                    var role = oneOrgTree.First()?.Role;
                    var userOrganizations = oneOrgTree.Select(i => i.OrganizationId).ToList();
                    switch (role)
                    {
                        case UserRoleEnum.Eeva:
                        {
                            checkStatus = true;
                            break;
                        }
                        case UserRoleEnum.Pete:
                        {
                            //can add or modify
                            checkStatus = CheckAllIds(userOrganizations, new List<Guid?> {channel.OrganizationId, prevOrganizationId});
                            break;
                        }
                        case UserRoleEnum.Shirley:
                        {
                            //                            //can modify
                            //                            checkStatus = !isAdd && CheckAllIds(userOrganizations, new List<Guid?> { channel.OrganizationId, prevOrganizationId });
                            //                            //can not remove
                            //                            checkStatus = checkStatus && channel.PublishingStatusId != typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
                            checkStatus = CheckAllIds(userOrganizations, new List<Guid?> {channel.OrganizationId, prevOrganizationId});
                            break;
                        }
                        default:
                        {
                            checkStatus = false;
                            break;
                        }
                    }
                    if (checkStatus) break;
                }
            }
            if (!checkStatus)
            {
                ThrowError(userIdentification, userOrgStruct.SelectMany(i => i).Select(j => j.Role).Distinct());
            }
        }
    }
}
