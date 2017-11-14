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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IChannelService
    {
        IVmChannelSearch GetChannelSearch();

        IVmChannelSearchResult SearchChannelResult(VmChannelSearchParams model);

        /// <summary>
        /// Search channels of relation
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        IVmChannelSearchResult RelationSearchChannels(VmChannelSearchParams vm);

        IVmChannelSearchResultBase ConnectingChannelSearchResult(VmServiceStep4 vm);

        /// <summary>
        /// Save data for Step1 of Phone Channel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IVmPhoneChannelStep1 SavePhoneChannelStep1(VmPhoneChannelStep1 model);

        /// <summary>
        /// Get a list of service channel ids and names with paging.
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <returns></returns>
        IVmOpenApiGuidPageVersionBase GetServiceChannels(DateTime? date, int pageNumber, int pageSize, bool archived = false, bool active = false);

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
        IVmOpenApiGuidPageVersionBase GetServiceChannelsByMunicipality(Guid municipalityId, DateTime? date, int pageNumber, int pageSize);

        /// <summary>
        /// Get all data related to a specific service channel.
        /// </summary>
        /// <param name="id">Service channel guid.</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="status">Indicates if published, latest or latest active version of service channel should be returned.</param>
        /// <returns></returns>
        IVmOpenApiServiceChannel GetServiceChannelById(Guid id, int openApiVersion, VersionStatusEnum status);

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
        IList<IVmOpenApiServiceChannel> GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int openApiVersion);

        /// <summary>
        /// Get certain type of service channels
        /// </summary>
        /// <param name="type">The type of service channel.</param>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <returns></returns>
        IVmOpenApiGuidPageVersionBase GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int pageNumber, int pageSize, bool getOnlyPublished = true);

        /// <summary>
        /// Get model for electronic channel step1
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmElectronicChannelStep1 GetElectronicChannelStep1(IVmEntityGet channelId);

        // <summary>
        /// Get model for web page channel step1
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmWebPageChannelStep1 GetWebPageChannelStep1(IVmEntityGet channelId);

        /// <summary>
        /// Save all data related to Step1 of WebPage Channel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IVmWebPageChannelStep1 SaveWebPageChannelStep1(VmWebPageChannelStep1 model);

        // <summary>
        /// Get model for printable form channel step1
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmPrintableFormChannelStep1 GetPrintableFormChannelStep1(IVmEntityGet channelId);

        /// <summary>
        /// Saves data from step 1 of printable form channel
        /// </summary>
        /// <param name="model">input data to save</param>
        /// <returns>saved printable form channel</returns>
        IVmPrintableFormChannelStep1 SavePrintableFormChannelStep1(VmPrintableFormChannelStep1 model);

        // <summary>
        /// Add new web page channel to repository
        /// </summary>
        /// <param name="model">web page channel model</param>
        /// <returns></returns>
        IVmEntityBase AddWebPageChannel(VmWebPageChannel model);

        /// <summary>
        /// Gets the channel names.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        VmEntityNames GetChannelNames(VmEntityBase model);

        // <summary>
        /// Add new printable form channel to repository
        /// </summary>
        /// <param name="model">web page channel model</param>
        /// <returns></returns>
        IVmEntityBase AddPrintableFormChannel(VmPrintableFormChannel vm);

        /// <summary>
        /// Add new electronic channel to repository
        /// </summary>
        /// <param name="model">electronic channel model</param>
        /// <returns></returns>
        IVmEntityBase AddElectronicChannel(VmElectronicChannel model);

        IVmElectronicChannelStep1 SaveElectronicChannelStep1(VmElectronicChannelStep1 model);

        IVmOpeningHours SaveOpeningHoursStep(VmOpeningHoursStep model);

        IVmPhoneChannelStep1 GetPhoneChannelStep1(IVmEntityGet channelId);

        /// <summary>
        ///  Get service channels by organization id.
        /// </summary>
        /// <param name="organizationId">Organization guid</param>
        /// <param name="date">Date</param>
        /// <param name="openApiVersion">Defines which open api version model should be returned.</param>
        /// <param name="type">Type of service channels to be returned.</param>
        /// <returns></returns>
        IList<IVmOpenApiServiceChannel> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int openApiVersion, ServiceChannelTypeEnum? type = null);

        /// <summary>
        /// Get service channels by organization id.
        /// </summary>
        /// <param name="organizationId">Organization guid</param>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <param name="type">Type of service channels to be returned.</param>
        /// <returns></returns>
        IVmOpenApiGuidPageVersionBase GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, ServiceChannelTypeEnum? type = null);

        IVmEntityBase AddPhoneChannel(VmPhoneChannel model);

        /// <summary>
        /// Get model for location channel step1
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmLocationChannelStep1 GetLocationChannelStep1(IVmEntityGet channelId);

        /// <summary>
        /// Get model for opening hours step
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmOpeningHours GetOpeningHoursStep(IVmEntityGet channelId);

        /// <summary>
        /// Add new Location channel to repository
        /// </summary>
        /// <param name="model">electronic channel model</param>
        /// <returns></returns>
        IVmEntityBase AddLocationChannel(VmLocationChannel model);

        IVmLocationChannelStep1 SaveLocationChannelStep1(VmLocationChannelStep1 model);

        /// <summary>
        /// Get publishing status of defined service channel for latest version.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PublishingStatus? GetChannelStatusByRootId(Guid id);

        /// <summary>
        /// Get publishing status of defined service channel for latest version.
        /// </summary>
        /// <param name="sourceId">External source id.</param>
        /// <returns></returns>
        PublishingStatus? GetChannelStatusBySourceId(string sourceId);

        /// <summary>
        /// Add new service channel.
        /// </summary>
        /// <typeparam name="TVmChannelIn"></typeparam>
        /// <param name="vm">Service channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <param name="openApiVersion">Defines which open api view model version should be returned.</param>
        /// <param name="userName"></param>
        /// <returns>The created service channel</returns>
        IVmOpenApiServiceChannel AddServiceChannel<TVmChannelIn>(TVmChannelIn vm, bool allowAnonymous, int openApiVersion, string userName = null)
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
        IVmOpenApiServiceChannel SaveServiceChannel<TVmChannelIn>(TVmChannelIn vm, bool allowAnonymous, int openApiVersion, string userName = null)
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn;

        /// <summary>
        /// Publish channel
        /// </summary>
        /// <param name="model">model with data of channel to publish</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        VmPublishingResultModel PublishChannel(VmPublishingModel model);


        /// <summary>
        /// Withdraw channel
        /// </summary>
        /// <param name="channelId">id</param>
        /// <returns></returns>
        VmPublishingResultModel WithdrawChannel(Guid channelId);

        /// <summary>
        /// Restore channel
        /// </summary>
        /// <param name="channelId">id</param>
        /// <returns></returns>
        VmPublishingResultModel RestoreChannel(Guid channelId);

//        /// <summary>
//        /// Publish all channels
//        /// </summary>
//        /// <param name="channelIds">List of channel ids</param>
//        /// <returns>List of base entities containing entityId and publishing status</returns>
//        List<IVmEntityBase> PublishChannels(List<Guid> channelIds);

        /// <summary>
        /// Delete channel
        /// </summary>
        /// <param name="entityId">id of channel to delete</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        IVmEntityBase DeleteChannel(Guid? entityId);

        /// <summary>
        /// Get channel status
        /// </summary>
        /// <param name="entityId">id of channel</param>
        /// <returns>base entity containing publishing status</returns>
        IVmEntityBase GetChannelStatus(Guid? entityId);

        /// <summary>
        /// Checks if a service channel with given identifier exists in the system.
        /// </summary>
        /// <param name="channelId">guid of the channel</param>
        /// <returns>true if a channel exists otherwise false</returns>
        bool ChannelExists(Guid channelId);

        /// <summary>
        /// Search channel service step by Id
        /// </summary>
        /// <param name="vm">Get channel model use only entityId</param>
        /// <returns></returns>
        IVmChannelServiceStep GetChannelServiceStep(IVmEntityGet vm);

        /// <summary>
        /// Lock channel by Id
        /// </summary>
        /// <param name="id">Channel Id</param>
        /// <returns></returns>
        IVmEntityBase LockChannel(Guid id);
        /// <summary>
        /// UnLock channel by Id
        /// </summary>
        /// <param name="id">Channel Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockChannel(Guid id);

        /// <summary>
        /// Get information if channel is editable
        /// </summary>
        /// <param name="id">Channel Id</param>
        /// <returns></returns>
        IVmEntityBase IsChannelEditable(Guid id);
    }
}
