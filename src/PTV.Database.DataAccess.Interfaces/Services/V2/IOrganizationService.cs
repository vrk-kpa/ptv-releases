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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Organization;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Check if organization exists and creates it if not
        /// </summary>
        /// <param name="organization"></param>
        void CreateNonExistingSahaOrganization(PahaOrganizationDto organization);

        /// <summary>
        /// Get model for organization
        /// </summary>
        /// <param name="model">input model</param>
        /// <returns></returns>
        VmOrganizationOutput GetOrganization(VmOrganizationBasic model);
        /// <summary>
        /// Saves organization
        /// </summary>
        /// <param name="model">input model</param>
        /// <returns>service</returns>
        VmOrganizationOutput SaveOrganization(VmOrganizationInput model);
        /// <summary>
        /// Publish organization.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase PublishOrganization(IVmLocalizedEntityModel model);
        /// <summary>
        /// Schedule publishing od archiving organization
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase ScheduleOrganization(IVmLocalizedEntityModel model);
        /// <summary>
        /// Gets the organization header.
        /// </summary>
        /// <param name="organizationId">The organization identifier.</param>
        /// <returns></returns>
        VmOrganizationHeader GetOrganizationHeader(Guid? organizationId);
        /// <summary>
        /// Delete Organization
        /// </summary>
        /// <param name="organizationId">id of Organization to delete</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        VmOrganizationHeader DeleteOrganization(Guid organizationId);
        /// <summary>
        ///  Remove organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        VmOrganizationHeader RemoveOrganization(Guid organizationId);
        /// <summary>
        /// return counts of related entities of organization to delete</param>
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        VmArchiveResult CheckDeleteOrganization(Guid organizationId);
        /// <summary>
        /// Lock organization by Id
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <param name="isLockDisAllowedForArchived">indicates whether organization can be locked for archived entity</param>
        /// <returns></returns>
        IVmEntityBase LockOrganization(Guid id, bool isLockDisAllowedForArchived = false);
        /// <summary>
        /// UnLock organization by Id
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockOrganization(Guid id);
        /// <summary>
        /// Restore Organization
        /// </summary>
        /// <param name="organizationId">id</param>
        /// <returns></returns>
        VmOrganizationHeader RestoreOrganization(Guid organizationId);
        /// <summary>
        /// Withdraw Organization
        /// </summary>
        /// <param name="organizationId">id</param>
        /// <returns></returns>
        VmOrganizationHeader WithdrawOrganization(Guid organizationId);
        /// <summary>
        /// Archive language of organization
        /// </summary>
        /// <param name="model">model with id of organization and language id</param>
        /// <returns></returns>
        VmOrganizationHeader ArchiveLanguage(VmEntityBasic model);
        /// <summary>
        /// Restore language of organization
        /// </summary>
        /// <param name="model">model with id of organization and language id</param>
        /// <returns></returns>
        VmOrganizationHeader RestoreLanguage(VmEntityBasic model);
        /// <summary>
        /// Withdraw language of organization
        /// </summary>
        /// <param name="model">model with id of organization and language id</param>
        /// <returns></returns>
        VmOrganizationHeader WithdrawLanguage(VmEntityBasic model);
        /// <summary>
        /// Get validated Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmOrganizationHeader GetValidatedEntity(VmEntityBasic model);

        /// <summary>
        /// Gets the organization list.
        /// </summary>
        /// <param name="searchedCode">The searched code.</param>
        /// <returns></returns>
        IVmListItemsData<IVmListItem> GetOrganizationList(VmOrganizationListSearch searchedCode);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Guid> GetMainRootOrganizationsIds(List<PublishingStatus> publishingStatuses = null);
        
        /// <summary>
        /// Returns SAHA id if assigned or PTV id if not
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="ptvRootId"></param>
        /// <returns></returns>
        Guid GetSahaIdForPtvOrgRootId(IUnitOfWork unitOfWork, Guid ptvRootId);

        /// <summary>
        /// Return map for PTV id to SAHA id
        /// </summary>
        /// <param name="ptvRootIds"></param>
        /// <returns></returns>
        Dictionary<Guid, List<Guid>> GetSahaIdsForPtvOrgRootIds(List<Guid> ptvRootIds);
    }

    internal interface IOrganizationServiceInternal
    {
        /// <summary>
        /// Return missing language ids for organization 
        /// </summary>
        /// <param name="organizationId">Organization versioned id</param>
        /// <param name="unitOfWork">unitOfWork</param>
        /// <returns>List of language ids</returns>
        IEnumerable<Guid> GetOrganizationMissingLanguages(Guid? organizationId, IUnitOfWork unitOfWork);

        /// <summary>
        /// Archive organization content connected to organization defined by <see cref="id"/>
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="id"></param>
        /// <param name="action">The archiving type</param>
        /// <returns></returns>
        void CascadeDeleteOrganization(IUnitOfWorkWritable unitOfWork, Guid id, HistoryAction action);

        /// <summary>
        /// Check if organization has any content which should be archive if organization has been archived
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        VmArchiveResult CheckOrganizationContentForDelete(IUnitOfWorkWritable unitOfWork, Guid id);
    }
}