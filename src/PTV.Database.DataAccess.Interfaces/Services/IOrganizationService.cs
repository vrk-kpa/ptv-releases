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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Get all published organizations as a list of organization ids and names. Paging is used.
        /// </summary>
        /// <param name="date">The date after organization has been created/modified</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="pageSize">The number of id's in single page</param>
        /// <returns>A list of organization ids with pageing data</returns>
        IVmOpenApiModelWithPagingBase<V8VmOpenApiOrganizationItem> GetOrganizations(DateTime? date, int openApiVersion, EntityStatusEnum status, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);

        /// <summary>
        /// Get all the data related to requested organizations.
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="openApiVersion"></param>
        /// <returns></returns>
        IList<IVmOpenApiOrganizationVersionBase> GetOrganizations(List<Guid> idList, int openApiVersion, bool showHeader);

        /// <summary>
        /// Get all published organizations as a list of organization ids and names. Paging is used.
        /// </summary>
        /// <param name="date">The date after organization has been created/modified</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="pageSize">The number of id's in single page</param>
        /// <param name="archived"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiOrganizationSaha> GetOrganizationsSaha(DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);

        /// <summary>
        /// Get published organizations related to certain municipality as a list of organization ids and names. Paging is used.
        /// </summary>
        /// <param name="municipalityId">municipality id</param>
        /// <param name="includeWholeCountry">Indicates if organizations marked to provide services for whole country (or whole country except Åland) be included.</param>
        /// <param name="date">The date after organization has been created/modified</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="pageSize">The number of id's in single page</param>
        /// <param name="dateBefore">The date before organization has been created/modified</param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<V8VmOpenApiOrganizationItem> GetOrganizationsByMunicipality(Guid municipalityId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);

        /// <summary>
        /// Get published organizations related to certain area as a list of organization ids and names. Paging is used.
        /// </summary>
        /// <param name="areaId">area id</param>
        /// <param name="includeWholeCountry">Indicates if organizations marked to provide services for whole country (or whole country except Åland) be included.</param>
        /// <param name="date">The date after organization has been created/modified</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="pageSize">The number of id's in single page</param>
        /// <param name="dateBefore">The date before organization has been created/modified</param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<V8VmOpenApiOrganizationItem> GetOrganizationsByArea(Guid areaId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);

        /// <summary>
        /// Get all data of published organizations related to defined municipality. Paging is used.
        /// </summary>
        /// <param name="municipalityId">municipality id</param>
        /// <param name="includeWholeCountry">Indicates if organizations marked to provide services for whole country (or whole country except Åland) be included.</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="openApiVersion"></param>
        /// <param name="showHeader"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<IVmOpenApiOrganizationVersionBase> GetOrganizationsWithAllDataByMunicipality(Guid municipalityId, bool includeWholeCountry, int pageNumber, int openApiVersion, bool showHeader);

        /// <summary>
        /// Get all data of published organizations related to defined area. Paging is used.
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="includeWholeCountry"></param>
        /// <param name="pageNumber"></param>
        /// <param name="openApiVersion"></param>
        /// <param name="showHeader"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<IVmOpenApiOrganizationVersionBase> GetOrganizationsWithAllDataByArea(Guid areaId, bool includeWholeCountry, int pageNumber, int openApiVersion, bool showHeader);

        /// <summary>
        /// Get published root organizations as a list of organization ids and names. Paging is used.
        /// </summary>
        /// <param name="date">The date after organization has been created/modified</param>
        /// <param name="pageNumber">The page to be fetched</param>
        /// <param name="pageSize">The number of id's in single page</param>
        /// <param name="dateBefore">The date before organization has been created/modified</param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetOrganizationsHierarchy(DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="getOnlyPublished">Defines if only published entities should be returned.</param>
        /// <param name="showHeader"></param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase GetOrganizationById(Guid id, int openApiVersion, bool getOnlyPublished = true, bool showHeader = false);

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <returns></returns>
        IVmOpenApiOrganizationSaha GetOrganizationSahaById(Guid id);

        /// <summary>
        /// Get data of organization with defined y code.
        /// </summary>
        /// <param name="code">Finnish y code</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IList<IVmOpenApiOrganizationVersionBase> GetOrganizationsByBusinessCode(string code, int openApiVersion, bool showHeader);

        /// <summary>
        /// Get a list of organization root ids which business code is the one defined by parameter.
        /// </summary>
        /// <param name="code">Business code</param>
        /// <returns></returns>
        IList<Guid> GetOrganizationIdsByBusinessCode(string code);

        /// <summary>
        /// Get data of defined organization.
        /// </summary>
        /// <param name="oid">Organization oid</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase GetOrganizationByOid(string oid, int openApiVersion, bool showHeader);

        /// <summary>
        /// Get guid for organization with defined oid.
        /// </summary>
        /// <param name="oid">Organization oid</param>
        /// <returns>Guid</returns>
        Guid GetOrganizationIdByOid(string oid, bool publishedVersion = false);

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
        /// Get organization hierarchy for a single organization. Includes parent organizations and all sub organizations.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IVmOpenApiOrganizationHierarchy GetOrganizationsHierarchy(Guid id);

        /// <summary>
        /// Add organization.
        /// </summary>
        /// <param name="vm">The organization data</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase AddOrganization(IVmOpenApiOrganizationInVersionBase vm, int openApiVersion);

        /// <summary>
        /// Checks if given organization exists and is one of users own.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of available languages</returns>
        List<string> GetAvailableLanguagesForOwnOrganization(Guid id);

        /// <summary>
        /// Save organization.
        /// </summary>
        /// <param name="vm">The organization data</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns>The organization data</returns>
        IVmOpenApiOrganizationVersionBase SaveOrganization(IVmOpenApiOrganizationInVersionBase vm, int openApiVersion);

        Guid? GetOrganizationIdByBusinessCode(string businessCode);
    }
}
