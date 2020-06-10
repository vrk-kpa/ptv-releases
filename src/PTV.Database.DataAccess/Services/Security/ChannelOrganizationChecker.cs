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
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;

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

    [RegisterService(typeof(IUserOrganizationChecker<ServiceCollectionVersioned>), RegisterType.Transient)]
    internal class ServiceCollectionOrganizationChecker : IUserOrganizationChecker<ServiceCollectionVersioned>
    {
        public ISecurityOwnOrganization CheckEntity(ServiceCollectionVersioned entity, IUnitOfWork unitOfWork, IEnumerable<Guid> userOrganizationIds)
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
                IsOwnOrganization = userOrganizationIds.Any(x => x == entity.ParentId || x == entity.UnificRootId)
            };
        }
    }

    [RegisterService(typeof(IUserOrganizationChecker<ServiceVersioned>), RegisterType.Transient)]
    internal class ServiceOrganizationChecker : IUserOrganizationChecker<ServiceVersioned>
    {

        public ISecurityOwnOrganization CheckEntity(ServiceVersioned entity, IUnitOfWork unitOfWork, IEnumerable<Guid> userOrganizationIds)
        {
            bool isOwnOrganization = userOrganizationIds.Contains(entity.OrganizationId);

            return new VmSecurityOwnOrganization
            {
                Id = entity.Id,
                IsOwnOrganization = isOwnOrganization
            };
        }
    }

    [RegisterService(typeof(IUserOrganizationChecker<StatutoryServiceGeneralDescriptionVersioned>), RegisterType.Transient)]
    internal class StatutoryServiceGeneralDescriptionOrganizationChecker : IUserOrganizationChecker<StatutoryServiceGeneralDescriptionVersioned>
    {
        private readonly IUserInfoService userInfoService;
        private readonly IResolveManager resolveManager;

        public StatutoryServiceGeneralDescriptionOrganizationChecker(IUserInfoService userInfoService, IResolveManager resolveManager)
        {
            this.userInfoService = userInfoService;
            this.resolveManager = resolveManager;
        }


        public ISecurityOwnOrganization CheckEntity(StatutoryServiceGeneralDescriptionVersioned entity, IUnitOfWork unitOfWork, IEnumerable<Guid> userOrganizationIds)
        {
            var userRole = userInfoService.GetUserInfo().Role;
            if (userRole != UserRoleEnum.Pete)
            {
                return new VmSecurityOwnOrganization
                {
                    Id = entity.Id,
                    IsOwnOrganization = userRole == UserRoleEnum.Eeva
                };
            }
            var permissions = userInfoService.GetPermissions().TryGetOrDefault(DomainEnum.GeneralDescriptions.ToCamelCase(), null);
            if (permissions?.RulesOwn.HasFlag(PermisionEnum.Update) != true)
            {
                return new VmSecurityOwnOrganization
                {
                    Id = entity.Id,
                    IsOwnOrganization = false
                };
            }

            using (var scope = resolveManager.CreateScope())
            {
                var restrictionFilterManager = scope.ServiceProvider.GetService<IRestrictionFilterManager>();
                var isTypeAllowed = userOrganizationIds.Any(ordId =>
                    restrictionFilterManager.IsTypeGuidAllowed<GeneralDescriptionType>(ordId,
                        entity.GeneralDescriptionTypeId));

                return new VmSecurityOwnOrganization
                {
                    Id = entity.Id,
                    IsOwnOrganization = isTypeAllowed
                };
            }
        }
    }
}
