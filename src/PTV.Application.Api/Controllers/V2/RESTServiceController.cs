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
using System;
using PTV.Framework.ServiceManager;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using IServiceService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceService;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "Services"
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/service")]
    public class RESTServiceController : RESTBaseController
    {
        private readonly IServiceService serviceService;
        private readonly IServiceManager serviceManager;
        private readonly IFormStateService formStateService;
        private readonly IPahaTokenProcessor pahaTokenProcessor;
        private readonly IEntityHistoryService entityHistoryService;
        private readonly IConnectionsService connectionService;

        private const string MessageLockedService = "Service.Exception.MessageLock";

        private const string MessageAddServiceRole = "Service.RoleException.MessageAdd";
        private const string MessageSaveServiceRole = "Service.RoleException.MessageSave";
        private const string MessagePublishServiceRole = "Service.RoleException.MessagePublish";
        private const string MessageWithdrawServiceRole = "Service.RoleException.MessageWithdraw";
        private const string MessageRestoreServiceRole = "Service.RoleException.MessageRestore";
        private const string MessageDeleteServiceRole = "Service.RoleException.MessageDelete";
        private const string MessageLockServiceRole = "Service.RoleException.MessageLock";

        private const string MessageAddServiceAndChannel = "ServiceAndChannel.AddServiceAndChannel.MessageSaved";
        private const string MessageTranslationUpdateForbidden = "Translation.TranslationException.MessageUpdateForbidden";
        private const string MessageDuplicityNames = "Service.DuplicityNameCheckException.MessageSave";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="serviceService">service service responsible for operation related to service - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="formStateService">form state service responsible for operation related to form states </param>
        /// <param name="pahaTokenProcessor">user info service responsible for operation related to users infos</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        /// <param name="connectionHistoryService">connection history service responsible for operation related to connection history</param>
        /// <param name="connectionService">service responsible for connections</param>
        public RESTServiceController(
            IServiceService serviceService,
            IServiceManager serviceManager,
            IFormStateService formStateService,
            IPahaTokenProcessor pahaTokenProcessor,
            ILogger<RESTServiceController> logger,
            IEntityHistoryService connectionHistoryService,
            IConnectionsService connectionService
        ) : base(logger)
        {
            this.serviceService = serviceService;
            this.serviceManager = serviceManager;
            this.formStateService = formStateService;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.entityHistoryService = connectionHistoryService;
            this.connectionService = connectionService;
        }

        /// <summary>
        /// Search for available keywords in specific language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SearchKeywords")]
        [HttpPost]
        public IServiceResultWrap SearchKeywords([FromBody] VmKeywordSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceService.KeywordSearch(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for service form
        /// </summary>
        /// <param name="model">model containing id of service or empty</param>
        /// <returns>all data needed for service step part one</returns>
        [Route("GetService")]
        [HttpPost]
        public IServiceResultWrap GetService([FromBody] VmServiceBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceService.GetService(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves service form
        /// </summary>
        /// <param name="model">model service</param>
        /// <returns>service</returns>
        [Route("SaveService")]
        [HttpPost]
        public IServiceResultWrap SaveService([FromBody] VmServiceInput model)
        {
            Console.WriteLine($"Save service with id: {model.Id}");
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = SaveServiceData(model)

                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(DuplicityCheckException), MessageDuplicityNames },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveServiceRole : MessageAddServiceRole },
                });
        }

        private IVmBase SaveServiceData(VmServiceInput model)
        {
            var result = serviceService.SaveService(model);
            formStateService.Delete(model.Id, null);
            return result;
        }

        /// <summary>
        /// Gets data for service header
        /// </summary>
        /// <param name="model">service identifier in model</param>
        /// <returns>all data needed for service header</returns>
        [Route("GetServiceHeader")]
        [HttpPost]
        public IServiceResultWrap GetServiceHeader([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceService.GetServiceHeader(model.Id) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Archives the service
        /// </summary>
        /// <param name="model">model containing the service id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? serviceService.DeleteService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageDeleteServiceRole }
                });
        }

//        /// <summary>
//        /// Locks the service
//        /// </summary>
//        /// <param name="id">service id </param>
//        /// <returns>result about locking</returns>
//        [Route("ArchiveEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap ArchiveEntity(Guid id)
//        {
//            return LockService(new VmServiceBasic() { Id = id});
//        }

//        /// <summary>
//        /// Locks service
//        /// </summary>
//        /// <param name="id">service id</param>
//        /// <returns>result about locking</returns>
//        [Route("ArchiveLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap ArchiveLanguage(Guid id)
//        {
//            return LockService(new VmServiceBasic() { Id = id });
//        }

        /// <summary>
        /// Archives language of service
        /// </summary>
        /// <param name="model">model containing the service id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? serviceService.ArchiveLanguage(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageDeleteServiceRole }
                });
        }

        /// <summary>
        /// Restores language of service
        /// </summary>
        /// <param name="model">model containing the service id to be restored</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? serviceService.RestoreLanguage(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageDeleteServiceRole },
                    { typeof(RestoreLanguageException), RESTCommonController.MessageRestoreLanguage }
                });
        }

//        /// <summary>
//        /// Locking of service
//        /// </summary>
//        /// <param name="id">service</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreLanguage(Guid id)
//        {
//            return LockService(new VmServiceBasic() { Id = id });
//        }

        /// <summary>
        /// Locks the service for given id
        /// </summary>
        /// <param name="model">id of service to be locked</param>
        /// <returns>locked id otherwise error message</returns>
        [Route("LockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap LockEntity([FromBody] VmEntityBasic model)
        {
            return LockService(model, true);
        }

        private IServiceResultWrap LockService(VmEntityBasic model, bool isLockDisAllowedForArchived = false)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? serviceService.LockService(model.Id.Value, isLockDisAllowedForArchived) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(LockException), MessageLockedService },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited },
                    { typeof(RoleActionException), MessageLockServiceRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }

        /// <summary>
        /// Unlocks the service for given id
        /// </summary>
        /// <param name="model">id of service to be unlocked</param>
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
                    Data = model.Id.HasValue ? serviceService.UnLockService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Withdraws the service
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = serviceService.RestoreService(model.Id ?? Guid.Empty),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageRestoreServiceRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
                });
        }

//        /// <summary>
//        /// Locks the service
//        /// </summary>
//        /// <param name="id">service id</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreEntity(Guid id)
//        {
//            return serviceManager.CallService(
//                () => new ServiceResultWrap
//                {
//                    Data = serviceService.CheckEntityForRestore(id)
//                },
//                new Dictionary<Type, string>
//                {
//                    { typeof(RoleActionException), MessageLockServiceRole },
//                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked },
//                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
//                });
//        }

//        /// <summary>
//        /// Locks the service
//        /// </summary>
//        /// <param name="id">service id</param>
//        /// <returns>result about locking</returns>
//        [Route("WithdrawEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap WithdrawEntity(Guid id)
//        {
//            return serviceManager.CallService(
//                () => new ServiceResultWrap
//                {
//                    Data = serviceService.CheckEntityForWithdraw(id)
//                },
//                new Dictionary<Type, string>
//                {
//                    { typeof(RoleActionException), MessageLockServiceRole },
//                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked },
//                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
//                });
//        }

        /// <summary>
        /// Withdraws the service
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? serviceService.WithdrawService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageWithdrawServiceRole },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

//        /// <summary>
//        /// Lock  service
//        /// </summary>
//        /// <param name="id">service id</param>
//        /// <returns>result about locking</returns>
//        [Route("WithdrawLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap WithdrawLanguage(Guid id)
//        {
//            return LockService(new VmServiceBasic() {Id = id});
//        }

        /// <summary>
        /// Withdraw language of the service
        /// </summary>
        /// <param name="model">model containing the service id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? serviceService.WithdrawLanguage(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageWithdrawServiceRole },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
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
                    Data = model.Id.HasValue ? serviceService.GetValidatedEntity(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(LockException), MessageLockedService },
                    { typeof(LockNotAllowedException), EntityMessageCannotBePublished }
                });
        }

        /// <summary>
        /// Publishes the service
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = serviceService.PublishService(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessagePublished },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessagePublishServiceRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }

        /// <summary>
        /// schedule publishing or archiving of the service
        /// </summary>
        /// <param name="model">model containing the service id to be scheduled</param>
        /// <returns>result about scheduling</returns>
        [Route("ScheduleEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ScheduleEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = serviceService.ScheduleService(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSchedulePublish },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessagePublishServiceRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists },
                    { typeof(SchedulePublishException), RESTCommonController.MessageSchedulePublishError}
                });
        }

        /// <summary>
        /// Get connectable services
        /// </summary>
        [Route("GetConnectableServices")]
        [HttpPost]
        public IServiceResultWrap GetConnectableServices([FromBody] VmConnectableServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceService.GetConnectableService(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get organization service with connections services
        /// </summary>
        [Route("GetOrganizationConnections")]
        [HttpPost]
        public IServiceResultWrap GetOrganizationConnections([FromBody] VmConnectionsServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceService.GetOrganizationConnections(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get connection page services search
        /// </summary>
        [Route("GetConnectionsServices")]
        [HttpPost]
        public IServiceResultWrap GetConnectionsServices([FromBody] VmConnectionsServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceService.GetConnectionsService(model) },
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
            () => new ServiceResultWrap
            {
                Data = connectionService.SaveServiceRelations(model)
            },
            new Dictionary<Type, string>
                {
                    { typeof(string), MessageAddServiceAndChannel },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited },
                    { typeof(CoordinateException), RESTCommonController.CoordinatesConnectionFailed },
                });
        }

        /// <summary>
        /// Check if connections can be managed
        /// </summary>
        /// <param name="model">model containing id of service</param>
        /// <returns>error message in case of service can not be connected</returns>
        [Route("IsConnectable")]
        [HttpPost]
        public IServiceResultWrap IsConnectable([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
            () => new ServiceResultWrap
            {
                Data = model.Id.HasValue ? serviceService.IsConnectable(model.Id.Value) : null,
            },
            new Dictionary<Type, string>
                {
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited }
                });
        }

        /// <summary>
        /// Create XLIFF
        /// </summary>
        /// <param name="model">model service</param>
        [Route("GenerateXliff")]
        [HttpPost]
        public IServiceResultWrap GenerateXliff([FromBody] VmServiceTranslation model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {Data = serviceService.GenerateXliff(model)},
                new Dictionary<Type, string>());
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
                    Data = serviceService.SendServiceEntityToTranslation(model),
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
                    Data = serviceService.GetServiceTranslationData(vmTranslationDataInput),
                },
                new Dictionary<Type, string>());
        }
        /// <summary>
        /// Gets service connection history
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
                    Data = entityHistoryService.GetServiceConnectionHistory(model)
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets service entity history
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
                    Data = entityHistoryService.GetServiceEntityHistory(model)
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Archives the service
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
                    Data = model.Id.HasValue ? serviceService.RemoveService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageDeleteServiceRole }
                });
        }

        /// <summary>
        /// Disable notifications for disconnected channels
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("DisableDisconnectedChannelNotifications")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead, AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap DisableDisconnectedChannelNotifications([FromBody] VmGuidList model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {Data = serviceService.DisableDisconnectedChannelNotifications(model)},
                new Dictionary<Type, string>());
        }
    }
}
