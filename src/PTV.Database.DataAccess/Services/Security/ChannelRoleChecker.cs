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
using PTV.Domain.Model.Enums.Security;
using UserRoleEnum = PTV.Domain.Model.Enums.Security.UserRoleEnum;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// ServiceChannel role checker
    /// </summary>
    [RegisterService(typeof(ChannelRoleChecker), RegisterType.Transient)]
    internal class ChannelRoleChecker : RoleCheckerBase
    {
        private readonly IUserIdentification userIdentification;

        public ChannelRoleChecker(
            IUserIdentification userIdentification,
            IUserOrganizationService userOrganizationService, 
            IUserInfoService userInfoService,
            ICacheManager cacheManager)
        : base(userOrganizationService, userInfoService, cacheManager)
        {
            this.userIdentification = userIdentification;
        }

        protected override DomainEnum Domain => DomainEnum.Channels;

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var channel = entity as ServiceChannelVersioned;
            var checkStatus = false;

            if (channel != null)
            {
                var prevOrganizationId = GetOriginalValue<Guid?>(entityEntry, "OrganizationId");
                checkStatus = CheckEntityRoles
                    (
                        entity,
                        entityEntry,
                        prevOrganizationId.IsAssigned() ? new List<Guid?> { channel.OrganizationId, prevOrganizationId } : new List<Guid?> { channel.OrganizationId }
                    );
            }
            if (!checkStatus)
            {
                ThrowError(userIdentification);
            }
        }
    }
}
