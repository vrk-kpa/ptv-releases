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

using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "General Description"
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/generalDescription")]
    public class RESTGeneralDescriptionController : RESTBaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;
        private readonly IServiceManager serviceManager;
        private readonly IPahaTokenProcessor pahaTokenProcessor;
        private readonly IFormStateService formStateService;
        private readonly IEntityHistoryService entityHistoryService;


        private const string MessageSaveGeneralDescription = "GeneralDescription.UpdateGeneralDescription.MessageSaved";
        private const string MessageLockedGeneralDescription = "GeneralDescription.Exception.MessageLock";
        private const string MessageArgumentException = "GeneralDescription.Exception.InvalidArgument";
        private const string MessageSaveGeneralDescriptionRole = "GeneralDescription.RoleException.MessageSave";
        private const string MessagePublishGeneralDescriptionRole = "GeneralDescription.RoleException.MessagePublish";
        private const string MessageLockGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageLock";


        private const string MessageWithdrawGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageWithdraw";
        private const string MessageRestoreGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageRestore";
        private const string MessageDeleteGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageDelete";

        private const string MessageTranslationUpdateForbidden = "Translation.TranslationException.MessageUpdateForbidden";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="generalDescriptionService">generalDescriptionS service responsible for operation related to general description - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="formStateService">formState service responsible for operation related to formStates - injected by framework</param>
        /// <param name="pahaTokenProcessor"></param>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        /// <param name="entityHistoryService">history service responsible for operation related to connection history</param>
        public RESTGeneralDescriptionController(
            IGeneralDescriptionService generalDescriptionService,
            IServiceManager serviceManager,
            IFormStateService formStateService,
            IPahaTokenProcessor pahaTokenProcessor,
            ILogger<RESTGeneralDescriptionController> logger,
            IEntityHistoryService entityHistoryService
        ) : base(logger)
        {
            this.generalDescriptionService = generalDescriptionService;
            this.serviceManager = serviceManager;
            this.formStateService = formStateService;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.entityHistoryService = entityHistoryService;
        }

        /// <summary>
        /// Search for general descriptions based on search criteria
        /// </summary>
        /// <param name="searchData">Search criteria</param>
        /// <returns>List of general descriptions</returns>
        [Route("v2/SearchGeneralDescriptions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap SearchGeneralDescriptions([FromBody] VmGeneralDescriptionSearchForm searchData)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = generalDescriptionService.SearchServiceGeneralDescriptions(searchData) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for generalDescription form
        /// </summary>
        /// <param name="model">model containing id of general description or empty</param>
        /// <returns>all data needed for service step part one</returns>
        [Route("GetGeneralDescription")]
        [HttpPost]
        public IServiceResultWrap GetGeneralDescription([FromBody] VmGeneralDescriptionGet model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = generalDescriptionService.GetGeneralDescription(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves general description form
        /// </summary>
        /// <param name="model">model service</param>
        /// <returns>service</returns>
        [Route("SaveGeneralDescription")]
        [HttpPost]
        public IServiceResultWrap SaveGeneralDescription([FromBody] VmGeneralDescriptionInput model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = SaveGeneralDescriptionData(model) },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageSaveGeneralDescriptionRole },
                    { typeof(PtvAppException), null }
                });
        }

        private IVmBase SaveGeneralDescriptionData(VmGeneralDescriptionInput model)
        {
            var result = generalDescriptionService.SaveGeneralDescription(model);
            formStateService.Delete(model.Id, null);
            return result;
        }


        /// <summary>
        /// Gets data for general description header
        /// </summary>
        /// <param name="model">general description identifier in model</param>
        /// <returns>all data needed for general description header</returns>
        [Route("GetGeneralDescriptionHeader")]
        [HttpPost]
        public IServiceResultWrap GetGeneralDescriptionHeader([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = generalDescriptionService.GetGeneralDescriptionHeader(model.Id) },
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
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.GetValidatedEntity(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(LockNotAllowedException), EntityMessageCannotBePublished }
                });
        }

        /// <summary>
        /// Publishes the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = generalDescriptionService.PublishGeneralDescription(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessagePublished },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessagePublishGeneralDescriptionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }

        /// <summary>
        /// schedule publishing or archiving of the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be scheduled</param>
        /// <returns>result about scheduling</returns>
        [Route("ScheduleEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ScheduleEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = generalDescriptionService.ScheduleGeneralDescription(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSchedulePublish },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessagePublishGeneralDescriptionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists },
                    { typeof(SchedulePublishException), RESTCommonController.MessageSchedulePublishError}
                });
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
            () => new ServiceResultWrap
            {
                Data = generalDescriptionService.SaveRelations(model)
            },
            new Dictionary<Type, string>
            {
                    { typeof(string), MessageSaveGeneralDescription },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited }
                });
        }

        /// <summary>
        /// Locks the general description for given id
        /// </summary>
        /// <param name="model">id of general description to be locked</param>
        /// <returns>locked id otherwise error message</returns>
        [Route("LockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap LockEntity([FromBody] VmEntityBasic model)
        {
            return LockGeneralDescription(model, true);
        }

        private IServiceResultWrap LockGeneralDescription(VmEntityBasic model, bool isLockDisAllowedForArchived = false)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id != null ? generalDescriptionService.LockGeneralDescription(model.Id.Value, isLockDisAllowedForArchived) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited },
                    { typeof(RoleActionException), MessageLockGeneralDescriptionRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }

        /// <summary>
        /// Unlocks the general description for given id
        /// </summary>
        /// <param name="model">id of general description to be unlocked</param>
        /// <returns>unlocked id otherwise error message</returns>
        [Route("UnLockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockEntity([FromBody] VmEntityBasic model)
        {
            formStateService.Delete(model.Id, pahaTokenProcessor.UserName);
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id != null ? generalDescriptionService.UnLockGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

//        /// <summary>
//        /// Lock the general description
//        /// </summary>
//        /// <param name="id">general description id</param>
//        /// <returns>result about locking</returns>
//        [Route("WithdrawEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap WithdrawEntity(Guid id)
//        {
//            return LockGeneralDescription(new VmEntityBasic() { Id = id });
//        }

        /// <summary>
        /// Withdraws the general descriptions
        /// </summary>
        /// <param name="model">model containing the general descriptions id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.WithdrawGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageWithdrawGeneralDescriptionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

        /// <summary>
        /// Locke general description
        /// </summary>
        /// <param name="id">general description id</param>
        /// <returns>result about locking</returns>
        [Route("WithdrawLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage(Guid id)
        {
            return LockGeneralDescription(new VmEntityBasic { Id = id });
        }

        /// <summary>
        /// Withdraws language of the general descriptions
        /// </summary>
        /// <param name="model">model containing the general descriptions id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.WithdrawLanguage(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageWithdrawGeneralDescriptionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

//        /// <summary>
//        /// Lock the general description
//        /// </summary>
//        /// <param name="id">general description id</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreEntity(Guid id)
//        {
//            return LockGeneralDescription(new VmEntityBasic() { Id = id });
//        }

        /// <summary>
        /// Restore the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.RestoreGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageRestoreGeneralDescriptionRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
                });
        }

//        /// <summary>
//        /// Locks the general description
//        /// </summary>
//        /// <param name="id">general description id</param>
//        /// <returns>result about locking</returns>
//        [Route("ArchiveEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap ArchiveEntity(Guid id)
//        {
//            return LockGeneralDescription(new VmEntityBasic() { Id = id });
//        }

        /// <summary>
        /// Archives the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.DeleteGeneralDescription(model.Id.Value) : null
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole }
                });
        }

        /// <summary>
        /// Archives the general description
        /// </summary>
        /// <param name="model">model containing the service id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("RemoveEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RemoveEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.RemoveGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole }
                });
        }

//        /// <summary>
//        /// Locks general description
//        /// </summary>
//        /// <param name="id">general description id</param>
//        /// <returns>result about locking</returns>
//        [Route("ArchiveLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap ArchiveLanguage(Guid id)
//        {
//            return LockGeneralDescription(new VmEntityBasic() { Id = id });
//        }

        /// <summary>
        /// Archive language of the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.ArchiveLanguage(model) : null
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage }
                });
        }

//        /// <summary>
//        /// Locks general description
//        /// </summary>
//        /// <param name="id">general description id</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreLanguage(Guid id)
//        {
//            return LockGeneralDescription(new VmEntityBasic() { Id = id });
//        }

        /// <summary>
        /// Restore language of the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be restored</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? generalDescriptionService.RestoreLanguage(model) : null
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole },
                    { typeof(RestoreLanguageException), RESTCommonController.MessageRestoreLanguage }
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
                () => new ServiceResultWrap
                {
                    Data = generalDescriptionService.SendGeneralDescriptionEntityToTranslation(model),
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
                () => new ServiceResultWrap
                {
                    Data = generalDescriptionService.GetGeneralDescriptionTranslationData(vmTranslationDataInput),
                },
                new Dictionary<Type, string>());
        }
        /// <summary>
        /// Check if connections can be managed
        /// </summary>
        /// <param name="model">model containing id of gd</param>
        /// <returns>error message in case of gd can not be connected</returns>
        [Route("IsConnectable")]
        [HttpPost]
        public IServiceResultWrap IsConnectable([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
            () => new ServiceResultWrap
            {
                Data = model.Id.HasValue ? generalDescriptionService.IsConnectable(model.Id.Value) : null,
            },
            new Dictionary<Type, string>
            {
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited }
                });
        }

        /// <summary>
        /// Gets general description entity history
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetEntityHistory")]
        [HttpPost]
        public IServiceResultWrap GetEntityHistory([FromBody] VmHistorySearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = entityHistoryService.GetGeneralDescriptionEntityHistory(model)
                },
                new Dictionary<Type, string>());
        }
        /// <summary>
        /// Gets general description connection history
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetConnectionHistory")]
        [HttpPost]
        public IServiceResultWrap GetConnectionHistory([FromBody] VmHistorySearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = entityHistoryService.GetGeneralDescriptionConnectionHistory(model)
                },
                new Dictionary<Type, string>());
        }

    }
}
