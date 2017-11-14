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
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;

namespace PTV.Database.DataAccess.Interfaces.Services.Security
{
    public interface IUserOrganizationService
    {
        /// <summary>
        /// Gets user own organization id
        /// </summary>
        /// <param name="unitOfWork">Unit of work to access database</param>
        /// <returns></returns>
        IList<Guid> GetUserOrganizationId(IUnitOfWork unitOfWork);

        //        /// <summary>
        //        ///
        //        /// </summary>
        //        /// <param name="parentId"></param>
        //        /// <param name="unitOfWork">Unit of work to access database</param>
        //        /// <returns></returns>
        //        IList<Guid> GetAllSubOrganizations(Guid parentId, IUnitOfWorkWritable unitOfWork);

        /// <summary>
        /// Gets user own organizations with all sub organizations
        /// </summary>
        /// <returns></returns>
        IList<Guid> GetAllUserOrganizations();

        /// <summary>
        /// Gets user own organizations with all sub organizations
        /// </summary>
        /// <param name="unitOfWork">Unit of work to access database</param>
        /// <returns></returns>
        IList<Guid> GetAllUserOrganizations(IUnitOfWork unitOfWork);

        void MapUserToOrganization(Guid userId, List<OrganizationRole> organizationsRoles);
        List<Guid> GetOrganizationsIdsLoggedUser();

        List<SelectListItem> GetOrganizationsFullLoggedUser();

        //bool IsUserAssignedToOrganization(string userName);

        List<Guid> GetCoUsersOfUser(Guid userId, Guid? whereRoleId = null);

        List<Guid> GetAllOrganizations();

        /// <summary>
        /// Get all Connected Users
        /// </summary>
        /// <returns>connected users</returns>
        List<VmUserOrganization> GetUsersWithOrganizations(IList<Guid> usersIds = null);

        /// <summary>
        /// Gets list of all connected organizations to user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //List<Guid> GetUserOrganizations(Guid userId);

        List<SelectListItem> GetFullOrganizationsForIds(IList<Guid> orgIds);

        Dictionary<Guid, Guid> GetUserOrganizations(Guid userId, Dictionary<Guid, int> rolePriorityMap = null);
        bool IsUserAssignedToOrganization(Guid userId);
        List<IUserOrganizationRoleDefinition> GetUserOrganizationsWithRoles(Guid userId);
        List<IUserOrganizationRoles> GetOrganizationsAndRolesForLoggedUser();
        List<List<IUserOrganizationRoles>> GetUserCompleteOrgStructure(IUnitOfWork unitOfWork);

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        List<OrganizationRole> GetMapOfUserAndOrganizations(Guid userId);

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        bool SetUserMainOrganization(Guid userId, Guid organizationId);

        /// <summary>
        ///  Temporary solution, solution for connection should come from customer
        /// </summary>
        /// <param name="userId"></param>
        Guid? GetUserMainOrganization(Guid userId);

        void ReAssignMissingRoles(Guid userId, Guid preferedRole);
        List<Guid> GetUsersMissingRoles();
        UserRoleEnum UserHighestRole();
        UserRoleEnum UserHighestRole(List<List<IUserOrganizationRoles>> organizations);
        UserRoleEnum UserHighestRole(IEnumerable<IUserOrganizationRoles> organizations);
        List<Guid> GetUserCompleteOrgList(IUnitOfWork unitOfWork, Guid userId);
        List<Guid> GetUserCompleteOrgList(Guid userId);
        List<List<IUserOrganizationRoles>> GetUserCompleteOrgStructure();
        List<Guid> LoadOrganizationTree(IEnumerable<Guid> rootIds);
        List<Guid> LoadOrganizationTree(IUnitOfWork unitOfWork, IEnumerable<Guid> rootIds);
        Dictionary<Guid, Guid> GetUserDirectlyAssignedOrganizations(Guid userId);
        List<Guid> GetAllCoAndSubUsers(IUnitOfWork unitOfWork, Guid userId, Guid? whereRoleId = null);
        List<Guid> GetAllCoAndSubUsers(Guid userId, Guid? whereRoleId = null);
        UserOrganizationsAndRolesResult GetAllUserOrganizationsAndRoles();
    }
}
