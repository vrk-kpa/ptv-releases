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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IServiceService
    {
        IVmSearchBase SearchServices(IVmServiceSearch vmServiceSearch);
        IVmSearchBase SearchServices(IUnitOfWork unitOfWork, IVmServiceSearch vmServiceSearch);
        IVmOpenApiGuidPageVersionBase GetServices(DateTime? date, int pageNumber, int pageSize, EntityStatusExtendedEnum state, DateTime? dateBefore = null);
        IList<IVmOpenApiServiceVersionBase> GetServices(List<Guid> idList, int openApiVersion);
        IVmOpenApiGuidPageVersionBase GetServicesByServiceChannel(Guid channelId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiGuidPageVersionBase GetServicesByServiceClass(List<Guid> serviceClassIds, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiGuidPageVersionBase GetServicesByMunicipality(Guid municipalityId, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);
        IVmOpenApiGuidPageVersionBase GetServicesByTargetGroup(Guid targetGroupId, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiGuidPageVersionBase GetServicesByType(string type, DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiServiceVersionBase GetServiceById(Guid id, int openApiVersion, VersionStatusEnum status);
        IVmOpenApiServiceVersionBase GetServiceByIdSimple(Guid id, bool getOnlyPublished = true);
        PublishingStatus? GetLatestVersionPublishingStatus(Guid id);
        /// <summary>
        /// Returns a list of services that do not exist (within idList).
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        List<Guid> CheckServices(List<Guid> idList);
        IList<IVmOpenApiServiceVersionBase> GetServicesByServiceChannel(Guid id, DateTime? date, int openApiVersion);
        IVmOpenApiServiceVersionBase GetServiceBySource(string sourceId);
        IVmOpenApiServiceBase AddService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, bool attachProposedChannels);
        IVmOpenApiServiceBase SaveService(IVmOpenApiServiceInVersionBase vm, bool allowAnonymous, int openApiVersion, bool attachProposedChannels, string sourceId = null);
    }
}
