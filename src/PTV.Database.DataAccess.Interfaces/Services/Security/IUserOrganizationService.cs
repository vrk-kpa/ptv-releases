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
using Microsoft.AspNetCore.Mvc.Rendering;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;

namespace PTV.Database.DataAccess.Interfaces.Services.Security
{
    public interface IUserOrganizationService
    {
        /// <summary>
        /// Gets user own organizations with all sub organizations
        /// </summary>
        /// <returns></returns>
        IList<Guid> GetAllUserOrganizationIds();

        List<IUserOrganizationRoles> GetOrganizationsAndRolesForLoggedUser();

        UserRoleEnum UserHighestRole();
        UserRoleEnum UserHighestRole(List<List<IUserOrganizationRoles>> organizations);
        UserRoleEnum UserHighestRole(IEnumerable<IUserOrganizationRoles> organizations);
        UserOrganizationsAndRolesResult GetAllUserOrganizationsAndRoles();
        /// <summary>
        /// Gets all organization roles for user from database, contains only connected organizations and roles for user (no sub organizations).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        List<VmUserOrganization> GetOrganizationRolesForUser(Guid userId);

        IReadOnlyList<IUserOrganizationRoles> GetAllUserOrganizationRoles();
        IEnumerable<VmListItem> GetAllUserOrganizations();
        List<Guid> GetCoUsersOfUser(Guid userId, Guid? whereRoleId = null);
        List<Guid> GetAllCoAndSubUsers(IUnitOfWork unitOfWork, Guid userId, Guid? whereRoleId = null);
        List<SelectListItem> GetFullOrganizationsForIds(IList<Guid> orgIds);
        Dictionary<Guid, Guid> GetUserDirectlyAssignedOrganizations(Guid userId);
        List<Guid> GetAllOrganizations();
        List<IUserOrganizationRoleDefinition> GetUserOrganizationsWithRoles(Guid userId);
        void MapUserToOrganization(Guid userId, List<OrganizationRole> organizationsRoles);
        List<Guid> GetUserCompleteOrgList(IUnitOfWork unitOfWork, Guid userId);

        List<Guid> GetUserCompleteOrgList(Guid userId);

        List<VmUserOrganizationSts> GetUsersWithOrganizations(IList<Guid> usersIds = null);

        List<Guid> GetAllCoAndSubUsers(Guid userId, Guid? whereRoleId = null);
        List<List<IUserOrganizationRoles>> GetUserCompleteOrgStructure();
        List<UserOrgRoleMappingData> UpdateUserOrgRolesMapping(List<UserOrgRoleMappingData> mappings, bool removeOthers = false);
    }
}
