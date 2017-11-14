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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PTV.Database.DataAccess.Interfaces.Services;
using System;
using PTV.Framework.ServiceManager;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Enums;
using VmElectronicChannel = PTV.Domain.Model.Models.VmElectronicChannel;
using VmPhoneChannel = PTV.Domain.Model.Models.VmPhoneChannel;
using VmWebPageChannel = PTV.Domain.Model.Models.VmWebPageChannel;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Channels"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/channel")]
    public class RESTChannelController : RESTBaseController
    {
        private readonly IChannelService channelService;
        private readonly IServiceManager serviceManager;
        private readonly ILanguageService languageService;

        private const string MessageAddElectronicChannel = "Service.AddElectronicChannel.MessageSaved";
        private const string MessageAddPrintableFormChannel = "Service.AddPrintableFormChannel.MessageSaved";
        private const string MessageChannelStepSave = "Service.ChannelStep.MessageSaved";
        private const string MessagePublishChannel = "Channel.Published.Successfully";
        private const string MessageWithdrawChannel = "Channel.Withdraw.Successfully";
        private const string MessageRestoreChannel = "Channel.Restore.Successfully";
        private const string MessageDeleteChannel = "Channel.Deleted.Successfully";
        private const string MessageAddWebPageChannel = "Service.AddWebPageChannel.MessageSaved";
        private const string MessageAddPhoneChannel = "Service.AddPhoneChannel.MessageSaved";
        private const string MessageAddLocationChannel = "Service.AddLocationChannel.MessageSaved";
        private const string MessageArgumentException = "Service.Exception.InvalidArgument";
        private const string MessageLockedChannel = "Channel.Exception.MessageLock";

        private const string MessageAddChannelRole = "Channel.RoleException.MessageAdd";
        private const string MessageSaveChannelRole = "Channel.RoleException.MessageSave";
        private const string MessagePublishChannelRole = "Channel.RoleException.MessagePublish";
        private const string MessageWithdrawChannelRole = "Channel.RoleException.MessageWithdraw";
        private const string MessageRestoreChannelRole = "Channel.RoleException.MessageRestore";
        private const string MessageDeleteChannelRole = "Channel.RoleException.MessageDelete";
        private const string MessageLockChannelRole = "Channel.RoleException.MessageLock";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="channelService">channel service responsible for operation related to channels - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="languageService">language service responsible for operation related to languages - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTChannelController(IChannelService channelService, IServiceManager serviceManager, ILanguageService languageService, ILogger<RESTChannelController> logger) : base(logger)
        {
            this.channelService = channelService;
            this.serviceManager = serviceManager;
            this.languageService = languageService;
        }


        /// <summary>
        /// Get data for channels search form
        /// </summary>
        /// <returns>data needed for search form like publishing statuses, types etc.</returns>
        [Route("GetChannelSearch")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetChannelSearch()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = channelService.GetChannelSearch() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Search for channel by criterias provided as model
        /// </summary>
        /// <param name="model">Model containing search criterias</param>
        /// <returns>List of channels</returns>
        [Route("ChannelSearchResult")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap ChannelSearchResult([FromBody] VmChannelSearchParams model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = model.IncludedRelations ? channelService.RelationSearchChannels(model) : channelService.SearchChannelResult(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get names for channel id
        /// </summary>
        /// <returns>names for channel id</returns>
        [Route("GetChannelNames")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetChannelNames([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = channelService.GetChannelNames(model) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the connected serives for channel
        /// </summary>
        /// <param name="model">model containing data to get correct connected services</param>
        /// <returns>connected services</returns>
        [Route("GetChannelServiceStep")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetChannelServiceStep([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetChannelServiceStep(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the searched channels for service step 4 (obsolete at 08.02.2017)
        /// </summary>
        /// <param name="model">search criteria</param>
        /// <returns>searched channels</returns>
        [Obsolete]
        [Route("ConnectingChannelSearchResultData")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap ConnectingChannelSearchResultData([FromBody] VmServiceStep4 model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = channelService.ConnectingChannelSearchResult(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for electronic channel step1
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for electronic channel step 1</returns>
        [Route("GetAddElectronicChannelStep1")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddElectronicChannelStep1([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetElectronicChannelStep1(model)},
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for web page channel step1
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for web page channel step 1</returns>
        [Route("GetAddWebPageChannelStep1")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddWebPageChannelStep1([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetWebPageChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for phone channel step1
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for phone channel step 1</returns>
        [Route("GetAddPhoneChannelStep1")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddPhoneChannelStep1([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetPhoneChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for service location channel step1
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for service location channel step 1</returns>
        [Route("GetAddLocationChannelStep1")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddLocationChannelStep1([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetLocationChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for printable form channel step1
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for printable form channel step 1</returns>
        [Route("GetAddPrintableFormChannelStep1")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddPrintableFormChannelStep1([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetPrintableFormChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for service location channel step4 or electronic channel step2 or phone channel step 2 or just openning hours for given model
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for service location channel step 1</returns>
        [Route("GetAddLocationChannelStep4")]
        [Route("GetAddElectronicChannelStep2")]
        [Route("GetAddPhoneChannelStep2")]
        [Route("GetOpeningHoursStep")]
        [HttpGet]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetOpeningHoursStep([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetOpeningHoursStep(model) },
                new Dictionary<Type, string>() { { typeof(LockException), MessageLockedChannel } });
        }

        /// <summary>
        /// Saves electronic channel to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveAllElectronicChannelChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveAllElectronicChannelChanges([FromBody] VmElectronicChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddElectronicChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddElectronicChannel },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageAddChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves service location channel to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveAllLocationChannelChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveAllLocationChannelChanges([FromBody] VmLocationChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddLocationChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddLocationChannel },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageAddChannelRole }
                });
        }

        /// <summary>
        /// Gets all available languages in databse
        /// </summary>
        /// <returns>languages</returns>
        [Route("GetWorldLanguages")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetWorldLanguages()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = languageService.GetWorldLanguages() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves phone channel to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("PhoneSaveAllChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PhoneSaveAllChanges([FromBody] VmPhoneChannel model)
        {

            var data = model;
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddPhoneChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddPhoneChannel },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageAddChannelRole }
                });
        }

        /// <summary>
        /// Saves web channel to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveAllWebPageChannelChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveAllWebPageChannelChanges([FromBody] VmWebPageChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddWebPageChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddWebPageChannel },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageAddChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves web channel step 1 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveWebPageStep1Changes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveWebPageStep1Changes([FromBody] VmWebPageChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveWebPageChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageChannelStepSave },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageSaveChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves web channel step 1 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SavePhoneChannelStep1Changes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SavePhoneChannelStep1Changes([FromBody] VmPhoneChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SavePhoneChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageChannelStepSave },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageSaveChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves printable channel to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveAllPrintableFormChannelChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveAllPrintableFormChannelChanges([FromBody] VmPrintableFormChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddPrintableFormChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddPrintableFormChannel },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageAddChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves printable channel step 1 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SavePrintableFormChannelStep1Changes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SavePrintableFormChannelStep1Changes([FromBody] VmPrintableFormChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SavePrintableFormChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageChannelStepSave },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageSaveChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves electronic channel step 1 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveElectronicChannelStep1Changes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveElectronicChannelStep1Changes([FromBody] VmElectronicChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveElectronicChannelStep1(model ) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageChannelStepSave },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageSaveChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves data for service location channel step4 or electronic channel step2 or phone channel step 2 or just openning hours for given model
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveElectronicChannelStep2Changes")]
        [Route("SavePhoneChannelStep2Changes")]
        [Route("SaveLocationChannelStep4Changes")]
        [Route("SaveOpeningHoursStep")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveOpeningHoursStep([FromBody] VmOpeningHoursStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveOpeningHoursStep(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageChannelStepSave },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageSaveChannelRole }
                }
            );
        }

        /// <summary>
        /// Saves service location channel step 1 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveLocationChannelStep1Changes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveLocationChannelStep1Changes([FromBody] VmLocationChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveLocationChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageChannelStepSave },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageSaveChannelRole }
                }
            );
        }

        /// <summary>
        /// Publishs the channel
        /// </summary>
        /// <param name="model">model containing the channel id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishChannel")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishChannel([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data =  channelService.PublishChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessagePublishChannel },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessagePublishChannelRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }

        /// <summary>
        /// Gets the publishing status of the channel
        /// </summary>
        /// <param name="model">model containing the channel id</param>
        /// <returns>publishing status of the channel</returns>
        [Route("GetChannelStatus")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetChannelStatus([FromBody] VmChannelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetChannelStatus(model.Id.Value) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Chekcs whether channel is editable or not
        /// </summary>
        /// <param name="model">model containing the channel id</param>
        /// <returns>true otherwise false</returns>
        [Route("IsChannelEditable")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap IsChannelEditable([FromBody] VmChannelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? channelService.IsChannelEditable(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

    }
}
