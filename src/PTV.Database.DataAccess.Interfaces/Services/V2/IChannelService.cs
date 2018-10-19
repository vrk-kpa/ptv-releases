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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IChannelService
    {
        /// <summary>
        /// Get model for electronic channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmElectronicChannel GetElectronicChannel(IVmEntityGet model);

        /// <summary>
        /// Save new or existing electronic channel to repository
        /// </summary>
        /// <param name="model">electronic channel model</param>
        /// <returns></returns>
        VmElectronicChannel SaveElectronicChannel(VmElectronicChannel model);

        /// <summary>
        /// Get model for web page channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmWebPageChannel GetWebPageChannel(IVmEntityGet model);

        /// <summary>
        /// Save new or existing web page channel to repository
        /// </summary>
        /// <param name="model">web page channel model</param>
        /// <returns></returns>
        VmWebPageChannel SaveWebPageChannel(VmWebPageChannel model);

        /// <summary>
        /// Get model for electronic channel header
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        VmChannelHeader GetChannelHeader(Guid? channelId);

        /// <summary>
        /// Get model for printable form channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmPrintableFormOutput GetPrintableFormChannel(IVmEntityGet model);

        /// <summary>
        /// Save new or existing printable form channel to repository
        /// </summary>
        /// <param name="model">printable form channel model</param>
        /// <returns></returns>
        VmPrintableFormOutput SavePrintableFormChannel(VmPrintableFormInput model);

        /// <summary>
        /// Get model for printable form channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmPhoneChannel GetPhoneChannel(IVmEntityGet model);

        /// <summary>
        /// Save new or existing phone channel to repository
        /// </summary>
        /// <param name="model">phone channel model</param>
        /// <returns></returns>
        VmPhoneChannel SavePhoneChannel(VmPhoneChannel model);

        /// <summary>
        /// Get model for printable form channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmServiceLocationChannel GetServiceLocationChannel(IVmEntityGet model);

        /// <summary>
        /// Save new or existing phone channel to repository
        /// </summary>
        /// <param name="model">phone channel model</param>
        /// <returns></returns>
        VmServiceLocationChannel SaveServiceLocationChannel(VmServiceLocationChannel model);

        /// <summary>
        /// Delete channel
        /// </summary>
        /// <param name="entityId">id of channel to delete</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        VmChannelHeader DeleteChannel(Guid entityId);
        /// <summary>
        /// Lock channel by Id
        /// </summary>
        /// <param name="id">Channel Id</param>
        /// <param name="isLockDisAllowedForArchived">Indicates whether channel can be locked for archived channel</param>
        /// <returns></returns>
        IVmEntityBase LockChannel(Guid id, bool isLockDisAllowedForArchived = false);
        /// <summary>
        /// UnLock channel by Id
        /// </summary>
        /// <param name="id">Channel Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockChannel(Guid id);
        /// <summary>
        /// Withdraw channel
        /// </summary>
        /// <param name="channelId">id</param>
        /// <returns></returns>
        VmChannelHeader WithdrawChannel(Guid channelId);

        /// <summary>
        /// Restore channel
        /// </summary>
        /// <param name="channelId">id</param>
        /// <returns></returns>
        VmChannelHeader RestoreChannel(Guid channelId);

        /// <summary>
        /// Archive Language
        /// </summary>
        /// <param name="model">model with entity id and language id</param>
        /// <returns></returns>
        VmChannelHeader ArchiveLanguage(VmEntityBasic model);

        /// <summary>
        /// Archive Language
        /// </summary>
        /// <param name="model">model with entity id and language id</param>
        /// <returns></returns>
        VmChannelHeader RestoreLanguage(VmEntityBasic model);

        /// <summary>
        /// Withdraw Language
        /// </summary>
        /// <param name="model">model with entity id and language id</param>
        /// <returns></returns>
        VmChannelHeader WithdrawLanguage(VmEntityBasic model);

        /// <summary>
        /// Publish channel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase PublishChannel(IVmLocalizedEntityModel model);
        /// <summary>
        /// Schedule publishing od archiving channel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase ScheduleChannel(IVmLocalizedEntityModel model);
        /// <summary>
        /// Get validated Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmChannelHeader GetValidatedEntity(VmEntityBasic model);
        /// <summary>
        /// Save and validate electornic channel.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmElectronicChannel SaveAndValidateElectronicChannel(VmElectronicChannel model);
        /// <summary>
        /// Save and validate webpage channel.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmWebPageChannel SaveAndValidateWebPageChannel(Domain.Model.Models.V2.Channel.VmWebPageChannel model);
        /// <summary>
        /// Save and validate printable form channel.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmPrintableFormOutput SaveAndValidatePrintableFormChannel(VmPrintableFormInput model);
        /// <summary>
        /// Save and validate phone channel.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Domain.Model.Models.V2.Channel.VmPhoneChannel SaveAndValidatePhoneChannel(Domain.Model.Models.V2.Channel.VmPhoneChannel model);
        /// <summary>
        /// Save and validate service location channel.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceLocationChannel SaveAndValidateServiceLocationChannel(VmServiceLocationChannel model);
        /// <summary>
        /// Gets all channels that can be connected to service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectableChannelSearchResult GetConnectableChannels(VmConnectableChannelSearch search);
        /// <summary>
        /// Gets all channels that can be connected to service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectionsChannelSearchResult GetConnectionsChannels(VmConnectionsChannelSearch search);
        /// <summary>
        /// Save connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmChannelConnectionsOutput SaveRelations(VmConnectionsInput model);
        /// <summary>
        /// Get connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmChannelConnectionsOutput GetRelations(VmConnectionsInput model);
        /// <summary>
        /// Set accessibility register info for service location channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmAccessibilityRegisterSetOut SetServiceLocationChannelAccessibility(VmAccessibilityRegisterSetIn model);
        /// <summary>
        /// Load accessibility register info for service location channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmServiceLocationChannel LoadServiceLocationChannelAccessibility(VmAccessibilityRegisterLoad model);
        /// <summary>
        /// Check channel by Id if is connectable
        /// </summary>
        /// <param name="id">Channel Id</param>
        /// <returns></returns>
        IVmEntityBase IsConnectable(Guid id);
        /// <summary>
        /// Send channel entity to translation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateSaveOutputs SendChannelEntityToTranslation(VmTranslationOrderInput model);
        /// <summary>
        /// Get channel translation data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetChannelTranslationData(VmTranslationDataInput model);        
    }
    
    internal interface IChannelServiceInternal : IChannelService
    {
        /// <summary>
        /// Remove all connection if channel connection type is not for all
        /// </summary>
        /// <param name="versionedIds">channel versioned ids</param>
        /// <param name="unitOfWork"></param>
        void RemoveNotCommonConnections(IEnumerable<Guid> versionedIds, IUnitOfWorkWritable unitOfWork);
    }
}