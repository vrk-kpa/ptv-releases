using Microsoft.AspNetCore.Authorization;
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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models;
using PTV.Database.DataAccess.Interfaces.Services.Security;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "Channels"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/channel")]
    public class RESTChannelControllerV2 : RESTBaseController
    {
        private readonly IChannelService channelService;
        private readonly IServiceManager serviceManager;
        private readonly ITranslationService translationService;
        private readonly IUserInfoService userService;
        private readonly Database.DataAccess.Interfaces.Services.IFormStateService formStateService;

        private const string MessageArgumentException = "Service.Exception.InvalidArgument";
        private const string MessageLockedChannel = "Channel.Exception.MessageLock";

        private const string MessageAddChannelRole = "Channel.RoleException.MessageAdd";
        private const string MessageSaveChannelRole = "Channel.RoleException.MessageSave";
        private const string MessagePublishChannelRole = "Channel.RoleException.MessagePublish";
        private const string MessageWithdrawChannelRole = "Channel.RoleException.MessageWithdraw";
        private const string MessageRestoreChannelRole = "Channel.RoleException.MessageRestore";
        private const string MessageDeleteChannelRole = "Channel.RoleException.MessageDelete";
        private const string MessageLockChannelRole = "Channel.RoleException.MessageLock";

        private const string MessageAddServiceAndChannel = "ServiceAndChannel.AddServiceAndChannel.MessageSaved";
        private const string MessageTranslationUpdateForbidden = "Translation.TranslationException.MessageUpdateForbidden";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="channelService">channel service responsible for operation related to channels - injected by framework</param>
        /// <param name="translationService"></param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="formStateService">formState service responsible for operation related to formStates - injected by framework</param>
        /// <param name="userService"></param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTChannelControllerV2(
            IChannelService channelService,
            ITranslationService translationService,
            IServiceManager serviceManager,
            Database.DataAccess.Interfaces.Services.IFormStateService formStateService,
            IUserInfoService userService,
            ILogger<RESTChannelController> logger
        ) : base(logger)
        {
            this.channelService = channelService;
            this.serviceManager = serviceManager;
            this.translationService = translationService;
            this.formStateService = formStateService;
            this.userService = userService;
        }

        #region Electronic
        /// <summary>
        /// Gets data for electronic channel
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for electronic channel step 1</returns>
        [Route("GetElectronicChannel")]
        [HttpPost]
        public IServiceResultWrap GetElectronicChannel([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.GetElectronicChannel(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves electronic channel to database
        /// </summary>s
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveElectronicChannel")]
        [HttpPost]
        public IServiceResultWrap SaveElectronicChannel([FromBody] VmElectronicChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = GetElectronicChannelData(model)
                }
                ,
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveChannelRole : MessageAddChannelRole }
                }
            );
        }

        private IVmBase GetElectronicChannelData(VmElectronicChannel model)
        {
            VmElectronicChannel result = null;
            switch (model.Action)
            {
                case ActionTypeEnum.Save:
                    result = channelService.SaveElectronicChannel(model);
                    break;
                case ActionTypeEnum.SaveAndValidate:
                    result = channelService.SaveAndValidateElectronicChannel(model);
                    break;
                default:
                    result = channelService.SaveElectronicChannel(model);
                    break;
            }
            channelService.UnLockChannel(result.Id.Value);
            formStateService.Delete(result.Id.Value, userService.GetClaimName());
            return result;
        }
        #endregion

        #region Web page
        /// <summary>
        /// Gets data for electronic channel
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for electronic channel step 1</returns>
        [Route("GetWebPageChannel")]
        [HttpPost]
        public IServiceResultWrap GetWebPageChannel([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.GetWebPageChannel(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves electronic channel to database
        /// </summary>s
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveWebPageChannel")]
        [HttpPost]
        public IServiceResultWrap SaveWebPageChannel([FromBody] VmWebPageChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = GetWebPageChannelData(model)
                }
                ,
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveChannelRole : MessageAddChannelRole }
                }
            );
        }

        private IVmBase GetWebPageChannelData(VmWebPageChannel model)
        {
            VmWebPageChannel result = null;
            switch (model.Action)
            {
                case ActionTypeEnum.Save:
                    result = channelService.SaveWebPageChannel(model);
                    break;
                case ActionTypeEnum.SaveAndValidate:
                    result = channelService.SaveAndValidateWebPageChannel(model);
                    break;
                default:
                    result = channelService.SaveWebPageChannel(model);
                    break;
            }
            channelService.UnLockChannel(result.Id.Value);
            formStateService.Delete(result.Id.Value, userService.GetClaimName());
            return result;
        }
        #endregion

        #region PrintableForm
        /// <summary>
        /// Gets data for printable form channel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetPrintableFormChannel")]
        [HttpPost]
        public IServiceResultWrap GetPrintableFormChannel([FromBody] VmChannelBasic model)
        {
            var result = serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.GetPrintableFormChannel(model) },
                new Dictionary<Type, string>());
            return result;
        }
        /// <summary>
        /// Saves printable form channel to database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SavePrintableFormChannel")]
        [HttpPost]
        public IServiceResultWrap SavePrintableFormChannel([FromBody] VmPrintableFormInput model)
        {
            var result = serviceManager.CallService(
                () => new ServiceResultWrap() { Data = GetPrintableFormChannelData(model) },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveChannelRole : MessageAddChannelRole }
                });
            return result;
        }

        private IVmBase GetPrintableFormChannelData(VmPrintableFormInput model)
        {
            VmPrintableFormOutput result = null;
            switch (model.Action)
            {
                case ActionTypeEnum.Save:
                    result = channelService.SavePrintableFormChannel(model);
                    break;
                case ActionTypeEnum.SaveAndValidate:
                    result = channelService.SaveAndValidatePrintableFormChannel(model);
                    break;
                default:
                    result = channelService.SavePrintableFormChannel(model);
                    break;
            }
            channelService.UnLockChannel(result.Id.Value);
            formStateService.Delete(result.Id.Value, userService.GetClaimName());
            return result;
        }
        #endregion

        #region Phone

        /// <summary>
        /// Gets data for electronic channel
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for electronic channel step 1</returns>
        [Route("GetPhoneChannel")]
        [HttpPost]
        public IServiceResultWrap GetPhoneChannel([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.GetPhoneChannel(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves electronic channel to database
        /// </summary>s
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SavePhoneChannel")]
        [HttpPost]
        public IServiceResultWrap SavePhoneChannel([FromBody] VmPhoneChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = GetPhoneChannelData(model)
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveChannelRole : MessageAddChannelRole }
                }
            );
        }

        private IVmBase GetPhoneChannelData(VmPhoneChannel model)
        {
            VmPhoneChannel result = null;
            switch (model.Action)
            {
                case ActionTypeEnum.Save:
                    result = channelService.SavePhoneChannel(model);
                    break;
                case ActionTypeEnum.SaveAndValidate:
                    result = channelService.SaveAndValidatePhoneChannel(model);
                    break;
                default:
                    result = channelService.SavePhoneChannel(model);
                    break;
            }
            formStateService.Delete(result.Id.Value, userService.GetClaimName());
            return result;
        }
        #endregion

        #region Service location

        /// <summary>
        /// Gets data for electronic channel
        /// </summary>
        /// <param name="model">model containing id of channel or empty</param>
        /// <returns>all data needed for electronic channel step 1</returns>
        [Route("GetServiceLocationChannel")]
        [HttpPost]
        public IServiceResultWrap GetServiceLocationChannel([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.GetServiceLocationChannel(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves electronic channel to database
        /// </summary>s
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SaveServiceLocationChannel")]
        [HttpPost]
        public IServiceResultWrap SaveServiceLocationChannel([FromBody] VmServiceLocationChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = GetServiceLocationChannelData(model)
                }
                ,
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveChannelRole : MessageAddChannelRole }
                }
            );
        }

        private IVmBase GetServiceLocationChannelData(VmServiceLocationChannel model)
        {
            VmServiceLocationChannel result = null;
            switch (model.Action)
            {
                case ActionTypeEnum.Save:
                    result = channelService.SaveServiceLocationChannel(model);
                    break;
                case ActionTypeEnum.SaveAndValidate:
                    result = channelService.SaveAndValidateServiceLocationChannel(model);
                    break;
                default:
                    result = channelService.SaveServiceLocationChannel(model);
                    break;
            }
            channelService.UnLockChannel(result.Id.Value);
            formStateService.Delete(result.Id.Value, userService.GetClaimName());
            return result;
        }

        /// <summary>
        /// Set accessibility register info for service location channel
        /// </summary>s
        /// <param name="model">model containing all data</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("SetServiceLocationChannelAccessibility")]
        [HttpPost]
        public IServiceResultWrap SetServiceLocationChannelAccessibility([FromBody] VmEntityRootStatusBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.SetServiceLocationChannelAccessibility(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Load accessibility register info for service location channel
        /// </summary>s
        /// <param name="model">model containing all data</param>
        /// <returns>if succeed, the id of channel otherwise error message</returns>
        [Route("LoadServiceLocationChannelAccessibility")]
        [HttpPost]
        public IServiceResultWrap LoadServiceLocationChannelAccessibility([FromBody] VmChannelBasic model)
        {
            return null;
//            return serviceManager.CallService(
//                () => new ServiceResultWrap { Data = channelService.SetServiceLocationChannelAccessibility(model) },
//                new Dictionary<Type, string>());
        }

        #endregion

        /// <summary>
        /// Get channel header
        /// </summary>
        /// <param name="model">Contains id of channel</param>
        /// <returns></returns>
        [Route("GetChannelHeader")]
        [HttpPost]
        public IServiceResultWrap GetChannelHeader([FromBody] VmChannelBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = channelService.GetChannelHeader(model.Id) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Lock the channel
        /// </summary>
        /// <param name="id">channel id</param>
        /// <returns>result about locking</returns>
        [Route("RestoreEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreEntity(Guid id)
        {
            return LockChannel(new VmChannelBasic() { Id = id });
        }

        /// <summary>
        /// Restore the channel
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? channelService.RestoreChannel(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageRestoreChannelRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
                });
        }

        /// <summary>
        /// Locks channel
        /// </summary>
        /// <param name="id">channel id</param>
        /// <returns>result about locking</returns>
        [Route("ArchiveLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage(Guid id)
        {
            return LockChannel(new VmChannelBasic() { Id = id });
        }

        /// <summary>
        /// Archives language of the channel
        /// </summary>
        /// <param name="model">model containing the channel id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id.HasValue ? channelService.ArchiveLanguage(model) : null
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageDeleteChannelRole },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage }
                });
        }

        /// <summary>
        /// Locks channel
        /// </summary>
        /// <param name="id">channel id</param>
        /// <returns>result about locking</returns>
        [Route("RestoreLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage(Guid id)
        {
            return LockChannel(new VmChannelBasic() { Id = id });
        }

        /// <summary>
        /// Restores language of the channel
        /// </summary>
        /// <param name="model">model containing the channel id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id.HasValue ? channelService.RestoreLanguage(model) : null
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageDeleteChannelRole },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageRestoreLanguage }
                });
        }

        /// <summary>
        /// Locks the channel
        /// </summary>
        /// <param name="id">channel id</param>
        /// <returns>result about locking</returns>
        [Route("ArchiveEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity(Guid id)
        {
            return LockChannel(new VmChannelBasic() { Id = id });
        }

        /// <summary>
        /// Archives the channel
        /// </summary>
        /// <param name="model">model containing the channel id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id.HasValue ? channelService.DeleteChannel(model.Id) : null
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageDeleteChannelRole }
                });
        }

        /// <summary>
        /// Lock the channel
        /// </summary>
        /// <param name="id">channel id</param>
        /// <returns>result about locking</returns>
        [Route("WithdrawEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawEntity(Guid id)
        {
            return LockChannel(new VmChannelBasic() { Id = id });
        }

        /// <summary>
        /// Withdraws the channel
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? channelService.WithdrawChannel(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageWithdrawChannelRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

        /// <summary>
        /// Locke channel
        /// </summary>
        /// <param name="id">channel id</param>
        /// <returns>result about locking</returns>
        [Route("WithdrawLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage(Guid id)
        {
            return LockChannel(new VmChannelBasic() { Id = id });
        }

        /// <summary>
        /// Withdraws language of the channel
        /// </summary>
        /// <param name="model">model containing the service id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? channelService.WithdrawLanguage(model) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessageWithdrawChannelRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

        /// <summary>
        /// Locks the channel for given id
        /// </summary>
        /// <param name="model">id of channel to be locked</param>
        /// <returns>locked id otherwise erro message</returns>
        [Route("LockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap LockEntity([FromBody] VmEntityBasic model)
        {
            return LockChannel(model, true);
        }

        private IServiceResultWrap LockChannel(VmEntityBasic model, bool isLockDisAllowedForArchived = false)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? channelService.LockChannel(model.Id.Value, isLockDisAllowedForArchived) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited },
                    { typeof(RoleActionException), MessageLockChannelRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }
        /// <summary>
        /// Unlocks the channel for given id
        /// </summary>
        /// <param name="model">id of channel to be unlocked</param>
        /// <returns>unlocked id otherwise erro message</returns>
        [Route("UnLockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? channelService.UnLockChannel(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get Validated entity
        /// </summary>
        /// <param name="model">model containing the service id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("GetValidatedEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap GetValidatedEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? channelService.GetValidatedEntity(model) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(LockNotAllowedException), EntityMessageCannotBePublished }
                });
        }

        /// <summary>
        /// Publishs the channel
        /// </summary>
        /// <param name="model">model containing the channel id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = channelService.PublishChannel(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessagePublished },
                    { typeof(LockException), MessageLockedChannel },
                    { typeof(RoleActionException), MessagePublishChannelRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }
        /// <summary>
        /// Get connectable channels
        /// </summary>
        [Route("GetConnectableChannels")]
        [HttpPost]
        public IServiceResultWrap GetConnectableChannels([FromBody] VmConnectableChannelSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.GetConnectableChannels(model) },
                new Dictionary<Type, string>());
        }
        /// <summary>
        /// Get search connectable channels for connection page
        /// </summary>
        [Route("GetConnectionsChannels")]
        [HttpPost]
        public IServiceResultWrap GetConnectionsChannels([FromBody] VmConnectionsChannelSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = channelService.GetConnectionsChannels(model) },
                new Dictionary<Type, string>());
        }
        /// <summary>
        /// Saves the connection to database
        /// </summary>
        /// <param name="model">model containing information needed for save of the connection</param>
        /// <returns>error message in case of save operation error otherwise nothing</returns>
        [Route("SaveRelations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveRelations([FromBody] VmConnectionsInput model)
        {
            return serviceManager.CallService(
            () => new ServiceResultWrap()
            {
                Data = channelService.SaveRelations(model)
            },
            new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddServiceAndChannel }
                });
        }


        /// <summary>
        /// Save and send entity to translation
        /// </summary>
        /// <param name="model">model containing the service id to be translated</param>
        /// <returns>result about translation</returns>
        [Route("SendEntityToTranslation")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SendEntityToTranslation([FromBody] VmTranslationOrderInput model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = translationService.SendChannelEntityToTranslation(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(PtvTranslationException), MessageTranslationUpdateForbidden }
                });
        }

        /// <summary>
        /// Get Translation data
        /// </summary>
        /// <param name="vmTranslationDataInput"></param>
        /// <returns></returns>
        [Route("GetTranslationData")]
        [HttpPost]
        public IServiceResultWrap GetTranslationData([FromBody] VmTranslationDataInput vmTranslationDataInput)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = translationService.GetChannelTranslationData(vmTranslationDataInput),
                },
                new Dictionary<Type, string> { }
            );
        }
    }
}
