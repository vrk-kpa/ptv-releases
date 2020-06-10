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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IChannelService
    {
        /// <summary>
        /// Get a list of service channel ids and names with paging.
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <param name="status">The status of items to be fetched.</param>
        /// <param name="dateBefore">Date</param>
        /// <param name="organizationIds">Organization id list.</param>
        /// <param name="isVisibleForAll">Indicated should the returned items be marked as 'IsVisibleForAll'.</param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannels(DateTime? date, int pageNumber, int pageSize, EntityStatusExtendedEnum status = EntityStatusExtendedEnum.Published, DateTime? dateBefore = null, IList<Guid> organizationIds = null, bool isVisibleForAll = false);

        IVmOpenApiModelWithPagingBase<VmOpenApiExpiringTask> GetTaskServiceChannels(int pageNumber, int pageSize, List<Guid> entityIds, DateTime lifeTime, List<Guid> publishingStatusIds);

        IVmOpenApiModelWithPagingBase<VmOpenApiTask> GetTaskServiceChannels(int pageNumber, int pageSize, List<Guid> entityIds, List<Guid> publishingStatusIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="openApiVersion"></param>
        /// <param name="showHeader"></param>
        /// <returns></returns>
        IList<IVmOpenApiServiceChannel> GetServiceChannels(List<Guid> idList, int openApiVersion, bool showHeader);

        IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> GetServiceChannelsWithAllDataByOrganization(IList<Guid> organizationIds, int openApiVersio, int pageNumber, bool showHeader);
        IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> GetServiceChannelsWithAllDataByMunicipality(Guid municipalityId, bool includeWholeCountry, int openApiVersio, int pageNumber, bool showHeader);
        IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> GetServiceChannelsWithAllDataByArea(Guid municipalityId, bool includeWholeCountry, int openApiVersio, int pageNumber, bool showHeader);

        /// <summary>
        /// Get a list of service channel ids and names with paging.
        /// </summary>
        /// <param name="municipalityId">Municipality id.</param>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page</param>
        /// <param name="archived"></param>
        /// <param name="getOnlyPublished"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByMunicipality(Guid municipalityId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);

        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByArea(Guid areaId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);

        /// <summary>
        /// Get all data related to a specific service channel.
        /// </summary>
        /// <param name="id">Service channel guid.</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="status">Indicates if published, latest or latest active version of service channel should be returned.</param>
        /// <param name="showHeader"></param>
        /// <returns></returns>
        IVmOpenApiServiceChannel GetServiceChannelById(Guid id, int openApiVersion, VersionStatusEnum status, bool showHeader = false);

        /// <summary>
        /// Get all basic data related to a specific service channel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IVmOpenApiServiceChannel GetServiceChannelByIdSimple(Guid id, bool getOnlyPublished = true);

        /// <summary>
        /// Get all data related to a specific service channel.
        /// </summary>
        /// <param name="sourceId">External source id</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="getOnlyPublished">Indicates if only published version of service channel should be returned.</param>
        /// <param name="userName">User name</param>
        /// <returns></returns>
        IVmOpenApiServiceChannel GetServiceChannelBySource(string sourceId);

        /// <summary>
        /// Get certain type of service channels
        /// </summary>
        /// <param name="type">The type of service channel.</param>
        /// <param name="date">Date</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <returns></returns>
        IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetailsByType(ServiceChannelTypeEnum type, DateTime? date, int openApiVersion, bool showHeader);

        /// <summary>
        /// Get certain type of service channels
        /// </summary>
        /// <param name="type">The type of service channel.</param>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int pageNumber, int pageSize, bool getOnlyPublished = true, DateTime? dateBefore = null);

        /// <summary>
        ///  Get service channels by organization id.
        /// </summary>
        /// <param name="organizationId">Organization guid</param>
        /// <param name="date">Date</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="showHeader"></param>
        /// <param name="type">Type of service channels to be returned.</param>
        /// <returns></returns>
        IList<IVmOpenApiServiceChannel> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int openApiVersion, bool showHeader, ServiceChannelTypeEnum? type = null);

        /// <summary>
        /// Get service channels by organization id.
        /// </summary>
        /// <param name="organizationId">Organization guid</param>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <param name="type">Type of service channels to be returned.</param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, ServiceChannelTypeEnum? type = null, DateTime? dateBefore = null);

        /// <summary>
        /// Checks if all the channels exist within idList. Returns also list of channels which type is ServiceLocation.
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="userOrganizations"></param>
        /// <returns></returns>
        VmOpenApiConnectionChannels CheckChannels(List<Guid> idList, List<Guid> userOrganizations = null);

        /// <summary>
        /// Add new service channel.
        /// </summary>
        /// <typeparam name="TVmChannelIn"></typeparam>
        /// <param name="vm">Service channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <param name="openApiVersion">Defines which open api view model version should be returned.</param>
        /// <param name="userName"></param>
        /// <returns>The created service channel</returns>
        IVmOpenApiServiceChannel AddServiceChannel<TVmChannelIn>(TVmChannelIn vm, int openApiVersion, string userName = null)
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn;

        /// <summary>
        /// Update service channel.
        /// </summary>
        /// <typeparam name="TVmChannelIn"></typeparam>
        /// <param name="vm">Service channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <param name="openApiVersion">Defines which open api view model version should be returned.</param>
        /// <param name="userName"></param>
        /// <returns></returns>
        IVmOpenApiServiceChannel SaveServiceChannel<TVmChannelIn>(TVmChannelIn vm, int openApiVersion, string userName = null)
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn;

        /// <summary>
        /// Checks if a service channel with given identifier exists in the system.
        /// </summary>
        /// <param name="channelId">guid of the channel</param>
        /// <returns>true if a channel exists otherwise false</returns>
        bool ChannelExists(Guid channelId);

        /// <summary>
        /// Returns the publishing status of latest version.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PublishingStatus? GetLatestVersionPublishingStatus(Guid id);
    }
}
