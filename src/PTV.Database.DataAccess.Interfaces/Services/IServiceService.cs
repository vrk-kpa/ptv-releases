﻿/**
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
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IServiceService
    {
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServices(DateTime? date, int pageNumber, int pageSize, EntityStatusExtendedEnum state, DateTime? dateBefore = null);
        IVmOpenApiModelWithPagingBase<VmOpenApiExpiringTask> GetTaskServices(int pageNumber, int pageSize, List<Guid> entityIds, int expirationMonths, List<Guid> publishingStatusIds);
        IVmOpenApiModelWithPagingBase<VmOpenApiTask> GetTaskServices(int pageNumber, int pageSize, List<Guid> entityIds, List<Guid> publishingStatusIds);
        IList<IVmOpenApiServiceVersionBase> GetServices(List<Guid> idList, int openApiVersion, bool fillWithAllGdData, bool showHeader);
        IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> GetServicesByOrganization(IList<Guid> organizationIds, int openApiVersion, int pageNumber, bool fillWithAllGdData, bool showHeader);
        IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> GetServicesWithAllDataByMunicipality(Guid municipalityId, bool includeWholeCountry, int openApiVersion, int pageNumber, bool fillWithAllGdData, bool showHeader);
        IVmOpenApiModelWithPagingBase<IVmOpenApiServiceVersionBase> GetServicesWithAllDataByArea(Guid areaId, bool includeWholeCountry, int openApiVersion, int pageNumber, bool fillWithAllGdData, bool showHeader);
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByServiceChannel(Guid channelId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByServiceClass(List<Guid> serviceClassIds, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByMunicipality(Guid municipalityId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByArea(Guid areaId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByTargetGroup(Guid targetGroupId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByIndustrialClass(Guid industrialClassId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServicesByType(string type, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiServiceVersionBase GetServiceById(Guid id, int openApiVersion, VersionStatusEnum status, bool fillWithAllGdData = false, bool showHeader = false);
        IVmOpenApiServiceVersionBase GetServiceByIdSimple(Guid id, bool getOnlyPublished = true);
        PublishingStatus? GetLatestVersionPublishingStatus(Guid id);
        /// <summary>
        /// Returns a list of services that do not exist (within idList).
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        List<Guid> CheckServices(List<Guid> idList);
        IVmOpenApiServiceVersionBase GetServiceBySource(string sourceId);
        IVmOpenApiServiceBase AddService(IVmOpenApiServiceInVersionBase vm, int openApiVersion, bool attachProposedChannels);
        IVmOpenApiServiceVersionBase SaveService(IVmOpenApiServiceInVersionBase vm, int openApiVersion, bool attachProposedChannels, string sourceId = null);
        List<Guid> CheckServicesAccess(List<Guid> serviceIds);
        IList<VmOpenApiArchivedServiceBase> GetArchivedServices(V11VmOpenApiGetArchivedServices parameters, int openApiVersion);
        /// <summary>
        /// Get all not updated services
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="entityIds"></param>
        /// <param name="publishingStatusIds"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiNotUpdatedTask> GetNotUpdatedTaskServices(int pageNumber, int pageSize, List<Guid> entityIds, List<Guid> publishingStatusIds);
    }
}
