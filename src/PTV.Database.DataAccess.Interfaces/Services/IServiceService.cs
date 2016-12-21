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
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IServiceService
    {
        IVmGetServiceSearch GetServiceSearch();
        IVmServiceSearchResult SearchServices(IVmServiceSearch vmServiceSearch);
        /// <summary>
        /// Get service for service id
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        IVmServiceSearchResult SearchRelationService(IVmServiceSearch model);
        /// <summary>
        /// Get services for channel id
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        IVmServiceSearchResult SearchRelationServices(IVmServiceSearch model);
        IVmServiceStep1 GetServiceStep1(IVmGetServiceStep vmGetServiceStep);
        IVmServiceStep2 GetServiceStep2(IVmGetServiceStep vmGetServiceStep);
        IVmServiceStep3 GetServiceStep3(IVmGetServiceStep vmGetServiceStep);
        IVmServiceStep4ChannelData GetServiceStep4Channeldata(IVmGetServiceStep vmGetServiceStep);
        IVmEntityBase AddService(VmService model);
        IVmEntityBase PublishService(Guid? serviceId);

        /// <summary>
        /// Publish all services
        /// </summary>
        /// <param name="serviceIds">List of services ids</param>
        /// <returns></returns>
        List<IVmEntityBase> PublishServices(List<Guid> serviceIds);
        IVmEntityBase DeleteService(Guid? serviceId);
        IVmOpenApiGuidPage GetServiceIds(DateTime? date, int pageNumber = 1, int pageSize = 1000);
        IVmOpenApiService GetService(Guid id, bool getOnlyPublished = true);
        IV2VmOpenApiService V2GetService(Guid id, bool getOnlyPublished = true);
        IList<VmOpenApiService> GetServicesByServiceChannel(Guid id, DateTime? date);
        IList<V2VmOpenApiService> V2GetServicesByServiceChannel(Guid id, DateTime? date);
        IVmOpenApiService GetServiceBySource(string sourceId, string userName = null);
        IV2VmOpenApiService AddService(IVmOpenApiServiceInBase vm, bool allowAnonymous, string userName = null);
        IV2VmOpenApiService V2AddService(IV2VmOpenApiServiceInBase vm, bool allowAnonymous, string userName = null, bool version1 = false);
        IVmServiceStep1 SaveStep1Changes(Guid serviceId, VmServiceStep1 model);
        IVmServiceStep2 SaveStep2Changes(Guid serviceId, VmServiceStep2 model);
        IVmServiceStep3 SaveStep3Changes(Guid serviceId, VmServiceStep3 model);
        IVmServiceStep4ChannelData SaveStep4Changes(Guid serviceId, List<Guid> model);
        IVmEntityBase GetServiceStatus(Guid? serviceId);
        IList<string> AddChannelsForService(Guid serviceId, IList<Guid> channelIds, bool allowAnonymous, string userName = null);
        IVmOpenApiService SaveService(IVmOpenApiServiceInBase vm, bool allowAnonymous, string sourceId = null, string userName = null);
        IV2VmOpenApiService V2SaveService(IV2VmOpenApiServiceInBase vm, bool allowAnonymous, string sourceId = null, string userName = null, bool version1 = false);
        bool ServiceExists(Guid serviceId);
    }
}
