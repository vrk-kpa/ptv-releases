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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi.V2;

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
        IVmOpenApiGuidPage GetOrganizationIds(DateTime? date, int pageNumber = 1, int pageSize = 1000);

        /// <summary>
        /// Gets prepared data for form search organization
        /// </summary>
        /// <returns>prepared data for search form</returns>
        IVmGetOrganizationSearch GetOrganizationSearch();

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganization GetOrganization(Guid id, bool getOnlyPublished = true);

        /// <summary>
        /// Get data of defined organization. Version 2.
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <returns>The organization data</returns>
        IV2VmOpenApiOrganization V2GetOrganization(Guid id, bool getOnlyPublished = true);

        /// <summary>
        /// Get data of organization with defined y code .
        /// </summary>
        /// <param name="code">Finnish y code</param>
        /// <returns>The organization data</returns>
        IList<VmOpenApiOrganization> GetOrganizationsByBusinessCode(string code);

        /// <summary>
        /// Get data of organization with defined y code .
        /// </summary>
        /// <param name="code">Finnish y code</param>
        /// <returns>The organization data</returns>
        IList<V2VmOpenApiOrganization> V2GetOrganizationsByBusinessCode(string code);

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="oid">Organization oid</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganization GetOrganizationByOid(string oid);

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="oid">Organization oid</param>
        /// <returns>The organization data</returns>
        IV2VmOpenApiOrganization V2GetOrganizationByOid(string oid);

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
        /// Add organization
        /// </summary>
        /// <param name="vm">The organization data.</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganization AddOrganization(IVmOpenApiOrganizationInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add organization. Version 2
        /// </summary>
        /// <param name="vm">The organization data.</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed.</param>
        /// <returns>The organization data</returns>
        IV2VmOpenApiOrganization V2AddOrganization(IV2VmOpenApiOrganizationInBase vm, bool allowAnonymous, bool version1 = false);

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
        /// <param name="organizationId">id of organization</param>
        /// <returns>data for step 1 of the organization</returns>
        IVmOrganizationStep1 GetOrganizationStep1(Guid? organizationId);


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
        /// <param name="entityId">id of Organization to publish</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        IVmEntityBase PublishOrganization(Guid? entityId);

        /// <summary>
        /// Delete Organization
        /// </summary>
        /// <param name="entityId">id of Organization to delete</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        IVmEntityBase DeleteOrganization(Guid? entityId);

        /// <summary>
        /// Get Organizations
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns>List of organizatios by searched text.</returns>
        IVmListItemsData<IVmListItem> GetOrganizations(string searchText);

        /// <summary>
        /// Save organization
        /// </summary>
        /// <param name="vm">The organization data.</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed.</param>
        /// <returns></returns>
        IVmOpenApiOrganization SaveOrganization(IVmOpenApiOrganizationInBase vm, bool allowAnonymous);


        /// <summary>
        /// Save organization. Version 2
        /// </summary>
        /// <param name="vm">The organization data.</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed.</param>
        /// <returns></returns>
        IV2VmOpenApiOrganization V2SaveOrganization(IV2VmOpenApiOrganizationInBase vm, bool allowAnonymous, bool version1 = false);

        /// <summary>
        /// Gets the type of organization address
        /// </summary>
        /// <param name="sourceId">External source id</param>
        /// <returns></returns>
        string GetOrganizationAddressType(string sourceId);

        /// <summary>
        /// Gets the type of organization web page
        /// </summary>
        /// <param name="sourceId">External source id</param>
        /// <returns></returns>
        string GetOrganizationWebPageType(string sourceId);
    }
}
