using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.Connections;

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
        VmChannelHeader DeleteChannel(Guid? entityId);
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
        VmEntityHeaderBase PublishChannel(IVmPublishingModel model);
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
        /// Set accessibility register info for service location channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmServiceLocationChannel SetServiceLocationChannelAccessibility(VmEntityRootStatusBase model);
        /// <summary>
        /// Load accessibility register info for service location channel
        /// </summary>
        /// <param name="model">Channel</param>
        /// <returns></returns>
        VmServiceLocationChannel LoadServiceLocationChannelAccessibility(IVmEntityGet model);
    }
}