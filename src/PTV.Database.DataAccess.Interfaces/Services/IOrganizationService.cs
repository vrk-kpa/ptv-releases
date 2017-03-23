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
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IOrganizationService
    {
        void TestMethod();

        /// <summary>
        /// Get all published organizations as a list of organization ids. Paging is used.
        /// </summary>
        /// <param name="date">The date after organization has been created/modified</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="pageSize">The number of id's in single page</param>
        /// <returns>A list of organization ids with pageing data</returns>
        IVmOpenApiGuidPageVersionBase GetOrganizations(DateTime? date, int pageNumber = 1, int pageSize = 1000);

        /// Get all published organizations as a list of organization ids and names. Paging is used.
        /// </summary>
        /// <param name="date">The date after organization has been created/modified</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="pageSize">The number of id's in single page</param>
        /// <returns>A list of organization ids with pageing data</returns>
        IVmOpenApiGuidPageVersionBase V3GetOrganizations(DateTime? date, int pageNumber = 1, int pageSize = 1000);

        /// <summary>
        /// Gets prepared data for form search organization
        /// </summary>
        /// <returns>prepared data for search form</returns>
        IVmGetOrganizationSearch GetOrganizationSearch();

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="getOnlyPublished">Defines if only published entities should be returned.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase GetOrganizationById(Guid id, int openApiVersion, bool getOnlyPublished = true);

        /// <summary>
        /// Get data of organization with defined y code.
        /// </summary>
        /// <param name="code">Finnish y code</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsByBusinessCode(string code, int openApiVersion);

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="oid">Organization oid</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase GetOrganizationByOid(string oid, int openApiVersion);

        /// <summary>
        /// Get guid for organization with defined oid.
        /// </summary>
        /// <param name="oid">Organization oid</param>
        /// <returns>Guid</returns>
        Guid GetOrganizationIdByOid(string oid);

        /// <summary>
        /// Get guid for organization with defined source id.
        /// </summary>
        /// <param name="sourceId">External source id for organization</param>
        /// <returns>Guid</returns>
        Guid GetOrganizationIdBySource(string sourceId);

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="sourceId">External source id.</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="getOnlyPublished">Defines if only published entities should be returned.</param>
        /// <returns></returns>
        IVmOpenApiOrganizationVersionBase GetOrganizationBySource(string sourceId, int openApiVersion, bool getOnlyPublished = true);

        /// <summary>
        /// Add organization.
        /// </summary>
        /// <param name="vm">The organization data</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase AddOrganization(IVmOpenApiOrganizationInVersionBase vm, bool allowAnonymous, int openApiVersion);

        /// <summary>
        /// Checks if organization with defined id exists
        /// </summary>
        /// <param name="id">The organization id</param>
        /// <returns>true/false</returns>
        bool OrganizationExists(Guid id);

        /// <summary>
        /// Search organizations
        /// </summary>
        /// <param name="vmOrganizationSearch">inputs for search</param>
        /// <returns>Found organizations</returns>
        IVmOrganizationSearchResult SearchOrganizations(IVmOrganizationSearch vmOrganizationSearch, bool takeAll = false);

        /// <summary>
        /// Get Data to the step 1 of the organization
        /// </summary>
        /// <param name="model">VmGetOrganizationStep model</param>
        /// <returns>data for step 1 of the organization</returns>
        IVmOrganizationStep1 GetOrganizationStep1(IVmGetOrganizationStep model);


        /// <summary>
        /// Save data of step 1 of the organization
        /// </summary>
        /// <param name="model">vm of organization</param>
        /// <returns>save data for step 1 of the organization</returns>
        IVmOrganizationStep1 SaveOrganizationStep1(VmOrganizationModel model);

        /// <summary>
        /// Get Organization status
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IVmEntityBase GetOrganizationStatus(Guid? organizationId);


        /// <summary>
        /// Add organization
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IVmEntityBase AddApiOrganization(VmOrganizationModel model);

        /// <summary>
        /// Publish Organization
        /// </summary>
        /// <param name="model">Model with data of Organization to publish</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        VmPublishingResultModel PublishOrganization(VmPublishingModel model);

        /// <summary>
        /// Delete Organization
        /// </summary>
        /// <param name="organizationId">id of Organization to delete</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        IVmEntityBase DeleteOrganization(Guid? organizationId);

        /// <summary>
        /// Get Organizations
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns>List of organizatios by searched text.</returns>
        IVmListItemsData<IVmListItem> GetOrganizations(string searchText);

        /// <summary>
        /// Save organization.
        /// </summary>
        /// <param name="vm">The organization data</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed.</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase SaveOrganization(IVmOpenApiOrganizationInVersionBase vm, bool allowAnonymous, int openApiVersion);

        /// <summary>
        /// Get the publishing status of defined organization for latest version.
        /// </summary>
        /// <param name="id">Unique root id</param>
        /// <returns></returns>
        PublishingStatus? GetOrganizationStatusByRootId(Guid id);

        /// <summary>
        /// Get the publishing status of defined organization for latest version.
        /// </summary>
        /// <param name="sourceId">External source id</param>
        /// <returns></returns>
        PublishingStatus? GetOrganizationStatusBySourceId(string sourceId);

        /// <summary>
        /// Lock organization by Id
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        IVmEntityBase LockOrganization(Guid id);
        /// <summary>
        /// UnLock organization by Id
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockOrganization(Guid id);
        /// <summary>
        /// Throw exception if organization is locked
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        IVmEntityBase IsOrganizationLocked(Guid id);

        /// <summary>
        /// Gets the organization names.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        VmEntityNames GetOrganizationNames(VmEntityBase model);
    }
}
