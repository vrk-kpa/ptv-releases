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
using PTV.Framework;
using System;
using System.Collections.Generic;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IChannelService
    {
        IVmChannelSearch GetChannelSearch();

        IVmChannelSearchResult SearchChannelResult(VmChannelSearchParams model);

        IVmChannelSearchResultBase ConnectingChannelSearchResult(VmServiceStep4 vm);

        /// <summary>
        /// Save data for Step1 of Phone Channel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IVmPhoneChannelStep1 SavePhoneChannelStep1(VmPhoneChannelStep1 model);


        /// <summary>
        /// Get a list of service channel id's with paging.
        /// </summary>
        /// <param name="date">Date</param>
        /// <param name="pageNumber">The number of a page to be fetched.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <returns></returns>
        IVmOpenApiGuidPage GetServiceChannels(DateTime? date, int pageNumber = 1, int pageSize = 1000);

        /// <summary>
        /// Get all data related to a specific service channel.
        /// </summary>
        /// <param name="id">Service channel guid.</param>
        /// <returns>The service channel dat</returns>
        IVmOpenApiServiceChannel GetServiceChannel(Guid id);

        /// <summary>
        /// Get all data related to a specific service channel. Version 2.
        /// </summary>
        /// <param name="id">Service channel guid.</param>
        /// <returns>The service channel data</returns>
        IVmOpenApiServiceChannel V2GetServiceChannel(Guid id, bool getOnlyPublished = true);

        /// <summary>
        /// Get a service channel guid by external source.
        /// </summary>
        /// <param name="sourceId">External source id</param>
        /// <param name="userName">User name</param>
        /// <returns>Guid</returns>
        Guid GetServiceChannelBySource(string sourceId, string userName);

        /// <summary>
        /// Get certain type of service channels.
        /// </summary>
        /// <param name="type">The type of service channel</param>
        /// <param name="date">Date</param>
        /// <returns>A list of service channel data</returns>
        IList<IVmOpenApiServiceChannel> GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date);

        /// <summary>
        /// Get certain type of service channels. Version 2.
        /// </summary>
        /// <param name="type">The type of service channel</param>
        /// <param name="date">Date</param>
        /// <returns>A list of service channel data</returns>
        IList<IVmOpenApiServiceChannel> V2GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date);

        /// <summary>
        /// Check if url exist
        /// </summary>
        /// <param name="model">model for chcecking url</param>
        /// <returns></returns>
        IVmUrlChecker CheckUrl(VmUrlChecker model);

        /// <summary>
        /// Get model for electronic channel step1
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmElectronicChannelStep1 GetElectronicChannelStep1(IVmGetChannelStep channelId);

        // <summary>
        /// Get model for web page channel step1
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmWebPageChannelStep1 GetWebPageChannelStep1(IVmGetChannelStep channelId);

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
        IVmPrintableFormChannelStep1 GetPrintableFormChannelStep1(IVmGetChannelStep channelId);

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

        IVmPhoneChannelStep1 GetPhoneChannelStep1(IVmGetChannelStep channelId);

        /// <summary>
        /// Get service channels by organization id.
        /// </summary>
        /// <param name="organizationId">Organization guid</param>
        /// <param name="date">Date</param>
        /// <param name="type">Type of service channels to be returned.</param>
        /// <returns>A list of service channels</returns>
        IList<IVmOpenApiServiceChannel> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, ServiceChannelTypeEnum? type = null);

        /// <summary>
        /// Get service channels by organization id. Version 2.
        /// </summary>
        /// <param name="organizationId">Organization guid</param>
        /// <param name="date">Date</param>
        /// <param name="type">Type of service channels to be returned.</param>
        /// <returns>A list of service channels</returns>
        IList<IVmOpenApiServiceChannel> V2GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, ServiceChannelTypeEnum? type = null);

        IVmEntityBase AddPhoneChannel(VmPhoneChannel model);

        /// <summary>
        /// Get model for location channel step1
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmLocationChannelStep1 GetLocationChannelStep1(IVmGetChannelStep channelId);

        /// <summary>
        /// Get model for location channel step2
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmLocationChannelStep2 GetLocationChannelStep2(IVmGetChannelStep channelId);

        /// <summary>
        /// Get model for location channel step3
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmLocationChannelStep3 GetLocationChannelStep3(IVmGetChannelStep channelId);

        /// <summary>
        /// Get model for opening hours step
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <returns></returns>
        IVmOpeningHours GetOpeningHoursStep(IVmGetChannelStep channelId);

        /// <summary>
        /// Add new Location channel to repository
        /// </summary>
        /// <param name="model">electronic channel model</param>
        /// <returns></returns>
        IVmEntityBase AddLocationChannel(VmLocationChannel model);

        IVmLocationChannelStep1 SaveLocationChannelStep1(VmLocationChannelStep1 model);
        IVmLocationChannelStep2 SaveLocationChannelStep2(VmLocationChannelStep2 model);
        IVmLocationChannelStep3 SaveLocationChannelStep3(VmLocationChannelStep3 model);

        /// <summary>
        /// Add new electronic service channel
        /// </summary>
        /// <param name="vm">Electronic channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created electronic channel</returns>
        IVmOpenApiElectronicChannel AddElectronicChannel(IVmOpenApiElectronicChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new electronic service channel. Version 2
        /// </summary>
        /// <param name="vm">Electronic channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created electronic channel</returns>
        IV2VmOpenApiElectronicChannel V2AddElectronicChannel(IV2VmOpenApiElectronicChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates electronic service channel
        /// </summary>
        /// <param name="vm">Electronic channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated electronic channel</returns>
        IVmOpenApiServiceChannel SaveElectronicChannel(IVmOpenApiElectronicChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates electronic service channel. Version 2.
        /// </summary>
        /// <param name="vm">Electronic channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated electronic channel</returns>
        IVmOpenApiServiceChannel V2SaveElectronicChannel(IV2VmOpenApiElectronicChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new phone service channel
        /// </summary>
        /// <param name="vm">Phone channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created phone channel</returns>
        IVmOpenApiPhoneChannel AddPhoneChannel(IVmOpenApiPhoneChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new phone service channel. Version 2
        /// </summary>
        /// <param name="vm">Phone channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created phone channel</returns>
        IV2VmOpenApiPhoneChannel V2AddPhoneChannel(IV2VmOpenApiPhoneChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates phone channel
        /// </summary>
        /// <param name="vm">Phone channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated phone channel</returns>
        IVmOpenApiServiceChannel SavePhoneChannel(IVmOpenApiPhoneChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates phone channel. Version 2
        /// </summary>
        /// <param name="vm">Phone channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated phone channel</returns>
        IVmOpenApiServiceChannel V2SavePhoneChannel(IV2VmOpenApiPhoneChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new web page service channel
        /// </summary>
        /// <param name="vm">Web page channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created web page channel</returns>
        IVmOpenApiWebPageChannel AddWebPageChannel(IVmOpenApiWebPageChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new web page service channel. Version 2
        /// </summary>
        /// <param name="vm">Web page channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created web page channel</returns>
        IV2VmOpenApiWebPageChannel V2AddWebPageChannel(IV2VmOpenApiWebPageChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates web page channel
        /// </summary>
        /// <param name="vm">Web page channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated web page channel</returns>
        IVmOpenApiServiceChannel SaveWebPageChannel(IVmOpenApiWebPageChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates web page channel. Version 2
        /// </summary>
        /// <param name="vm">Web page channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated web page channel</returns>
        IVmOpenApiServiceChannel V2SaveWebPageChannel(IV2VmOpenApiWebPageChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new printable form channel
        /// </summary>
        /// <param name="vm">Printable form channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created printable form channel</returns>
        IVmOpenApiPrintableFormChannel AddPrintableFormChannel(IVmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new printable form channel. Version 2
        /// </summary>
        /// <param name="vm">Printable form channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created printable form channel</returns>
        IV2VmOpenApiPrintableFormChannel V2AddPrintableFormChannel(IV2VmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates printable form channel
        /// </summary>
        /// <param name="vm">Printable form channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated printable form channel</returns>
        IVmOpenApiServiceChannel SavePrintableFormChannel(IVmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Updates printable form channel. Version 2
        /// </summary>
        /// <param name="vm">Printable form channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated printable form channel</returns>
        IVmOpenApiServiceChannel V2SavePrintableFormChannel(IV2VmOpenApiPrintableFormChannelInBase vm, bool allowAnonymous);

        /// <summary>
        /// Add new service location channel
        /// </summary>
        /// <param name="vm">Service location channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The created service location channel</returns>
        IVmOpenApiServiceLocationChannel AddServiceLocationChannel(IVmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null);

        /// <summary>
        /// Add new service location channel. Version 2
        /// </summary>
        /// <param name="vm">Service location channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <param name="userName"></param>
        /// <returns>The created service location channel</returns>
        IV2VmOpenApiServiceLocationChannel V2AddServiceLocationChannel(IV2VmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null);

        /// <summary>
        /// Update service location channel
        /// </summary>
        /// <param name="vm">Service location channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <returns>The updated service location channel</returns>
        IVmOpenApiServiceChannel SaveServiceLocationChannel(IVmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null);

        /// <summary>
        /// Update service location channel. Version 2
        /// </summary>
        /// <param name="vm">Service location channel model</param>
        /// <param name="allowAnonymous">Is anonymous updates allowed</param>
        /// <param name="userName"></param>
        /// <returns>The updated service location channel</returns>
        IVmOpenApiServiceChannel V2SaveServiceLocationChannel(IV2VmOpenApiServiceLocationChannelInBase vm, bool allowAnonymous, string userName = null);

        /// <summary>
        /// Publish channel
        /// </summary>
        /// <param name="entityId">id of channel to publish</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        IVmEntityBase PublishChannel(Guid? entityId);

        /// <summary>
        /// Publish all channels
        /// </summary>
        /// <param name="channelIds">List of channel ids</param>
        /// <returns>List of base entities containing entityId and publishing status</returns>
        List<IVmEntityBase> PublishChannels(List<Guid> channelIds);

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
        /// <param name="vm">search parameter model use only channelId</param>
        /// <returns></returns>
        IVmChannelServiceStep GetChannelServiceStep(VmChannelSearchParams vm);
    }
}
