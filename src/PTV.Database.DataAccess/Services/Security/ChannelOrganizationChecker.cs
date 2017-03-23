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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Security
{
    [RegisterService(typeof(IUserOrganizationChecker<ServiceChannelVersioned>), RegisterType.Transient)]
    internal class ChannelOrganizationChecker : IUserOrganizationChecker<ServiceChannelVersioned>
    {
        public ISecurityOwnOrganization CheckEntity(ServiceChannelVersioned entity, IUnitOfWork unitOfWork, IEnumerable<Guid> userOrganizationIds)
        {
            return new VmSecurityOwnOrganization
            {
                Id = entity.Id,
                IsOwnOrganization = userOrganizationIds.Any(x => x == entity.OrganizationId)
            };
        }
    }

    [RegisterService(typeof(IUserOrganizationChecker<OrganizationVersioned>), RegisterType.Transient)]
    internal class OrganizationChecker : IUserOrganizationChecker<OrganizationVersioned>
    {
        public ISecurityOwnOrganization CheckEntity(OrganizationVersioned entity, IUnitOfWork unitOfWork, IEnumerable<Guid> userOrganizationIds)
        {
            return new VmSecurityOwnOrganization
            {
                Id = entity.Id,
                IsOwnOrganization = userOrganizationIds.Any(x => x == entity.ParentId || x == entity.Id)
            };
        }
    }

    [RegisterService(typeof(IUserOrganizationChecker<ServiceVersioned>), RegisterType.Transient)]
    internal class ServiceOrganizationChecker : IUserOrganizationChecker<ServiceVersioned>
    {
        private ITypesCache typesCache;

        public ServiceOrganizationChecker(ITypesCache typesCache)
        {
            this.typesCache = typesCache;
        }

        public ISecurityOwnOrganization CheckEntity(ServiceVersioned entity, IUnitOfWork unitOfWork, IEnumerable<Guid> userOrganizationIds)
        {
            var organizationServiceRepository = unitOfWork.CreateRepository<IOrganizationServiceRepository>();
            var orgIds = userOrganizationIds.ToList();
            bool isOwnOrganization = organizationServiceRepository.All().Any(x =>
                        x.ServiceVersionedId == entity.Id && orgIds.Contains(x.OrganizationId.Value) &&
                        x.RoleTypeId == typesCache.Get<RoleType>(RoleTypeEnum.Responsible.ToString())
                );

            return new VmSecurityOwnOrganization
            {
                Id = entity.Id,
                IsOwnOrganization = isOwnOrganization
            };
        }
    }
}