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

using Microsoft.AspNetCore.Authorization;
using System;
using PTV.Framework.ServiceManager;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.ServiceCollection;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using IServiceCollectionService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceCollectionService;
using IServiceService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceService;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "ServiceCollections"
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/serviceCollection")]
    public class RESTServiceCollectionController : RESTBaseController
    {
        private readonly IServiceCollectionService serviceCollectionService;
        private readonly IServiceService serviceService;
        private readonly IEntityHistoryService entityHistoryService;
        private readonly IServiceManager serviceManager;
        private readonly IFormStateService formStateService;
        private readonly IPahaTokenProcessor pahaTokenProcessor;

        private const string MessageLockedServiceCollection = "ServiceCollection.Exception.MessageLock";

        private const string MessageAddServiceCollectionRole = "ServiceCollection.RoleException.MessageAdd";
        private const string MessageSaveServiceCollectionRole = "ServiceCollection.RoleException.MessageSave";
        private const string MessagePublishServiceCollectionRole = "ServiceCollection.RoleException.MessagePublish";
        private const string MessageWithdrawServiceCollectionRole = "ServiceCollection.RoleException.MessageWithdraw";
        private const string MessageRestoreServiceCollectionRole = "ServiceCollection.RoleException.MessageRestore";
        private const string MessageDeleteServiceCollectionRole = "ServiceCollection.RoleException.MessageDelete";
        private const string MessageLockServiceCollectionRole = "ServiceCollection.RoleException.MessageLock";

        private const string MessageAddServiceAndChannel = "ServiceAndChannel.AddServiceAndChannel.MessageSaved";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="serviceCollectionService">serviceCollection service responsible for operation related to serviceCollection - injected by framework</param>
        /// <param name="serviceService">service service responsible for operation related to service - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="entityHistoryService"></param>
        /// <param name="formStateService">form state service responsible for operation related to form states </param>
        /// <param name="pahaTokenProcessor">user info service responsible for operation related to users infos</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTServiceCollectionController(
            IServiceCollectionService serviceCollectionService,
            IServiceService serviceService,
            IServiceManager serviceManager,
            IEntityHistoryService entityHistoryService,
            IFormStateService formStateService,
            IPahaTokenProcessor pahaTokenProcessor,
            ILogger<RESTServiceCollectionController> logger
        ) : base(logger)
        {
            this.serviceService = serviceService;
            this.serviceCollectionService = serviceCollectionService;
            this.serviceManager = serviceManager;
            this.entityHistoryService = entityHistoryService;
            this.formStateService = formStateService;
            this.pahaTokenProcessor = pahaTokenProcessor;
        }

        /// <summary>
        /// Gets data for serviceCollection form
        /// </summary>
        /// <param name="model">model containing id of serviceCollection or empty</param>
        /// <returns>all data needed for serviceCollection</returns>
        [Route("GetServiceCollection")]
        [HttpPost]
        public IServiceResultWrap GetServiceCollection([FromBody] VmServiceCollectionBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceCollectionService.GetServiceCollection(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves serviceCollection form
        /// </summary>
        /// <param name="model">model serviceCollection</param>
        /// <returns>serviceCollection</returns>
        [Route("SaveServiceCollection")]
        [HttpPost]
        public IServiceResultWrap SaveServiceCollection([FromBody] Domain.Model.Models.V2.ServiceCollection.VmServiceCollectionBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = GetServiceCollectionData(model)

                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveServiceCollectionRole : MessageAddServiceCollectionRole }
                });
        }

        private IVmBase GetServiceCollectionData(Domain.Model.Models.V2.ServiceCollection.VmServiceCollectionBase model)
        {
            VmServiceCollectionOutput result = serviceCollectionService.SaveServiceCollection(model);
            formStateService.Delete(model.Id, null);
            serviceCollectionService.UnLockServiceCollection(result.Id.Value);
            return result;
        }

        /// <summary>
        /// Gets data for serviceCollection header
        /// </summary>
        /// <param name="model">serviceCollection identifier in model</param>
        /// <returns>all data needed for serviceCollection header</returns>
        [Route("GetServiceCollectionHeader")]
        [HttpPost]
        public IServiceResultWrap GetServiceCollectionHeader([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = serviceCollectionService.GetServiceCollectionHeader(model.Id) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Archives the serviceCollection
        /// </summary>
        /// <param name="model">model containing the serviceCollection id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.DeleteServiceCollection(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessageDeleteServiceCollectionRole }
                });
        }
        
        /// <summary>
        /// Archives the serviceCollection
        /// </summary>
        /// <param name="model">model containing the service id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("RemoveEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RemoveEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.RemoveServiceCollection(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessageDeleteServiceCollectionRole }
                });
        }

//        /// <summary>
//        /// Locks the serviceCollection
//        /// </summary>
//        /// <param name="id">serviceCollection id </param>
//        /// <returns>result about locking</returns>
//        [Route("ArchiveEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap ArchiveEntity(Guid id)
//        {
//            return LockService(new VmServiceCollectionBasic() { Id = id});
//        }

//        /// <summary>
//        /// Locks serviceCollection
//        /// </summary>
//        /// <param name="id">serviceCollection id</param>
//        /// <returns>result about locking</returns>
//        [Route("ArchiveLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap ArchiveLanguage(Guid id)
//        {
//            return LockService(new VmServiceCollectionBasic() { Id = id });
//        }

        /// <summary>
        /// Archives language of serviceCollection
        /// </summary>
        /// <param name="model">model containing the serviceCollection id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.ArchiveLanguage(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessageDeleteServiceCollectionRole }
                });
        }

        /// <summary>
        /// Restores language of serviceCollection
        /// </summary>
        /// <param name="model">model containing the serviceCollection id to be restored</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.RestoreLanguage(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessageDeleteServiceCollectionRole },
                    { typeof(RestoreLanguageException), RESTCommonController.MessageRestoreLanguage }
                });
        }

//        /// <summary>
//        /// Locking of serviceCollection
//        /// </summary>
//        /// <param name="id">serviceCollection</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreLanguage(Guid id)
//        {
//            return LockService(new VmServiceCollectionBasic() { Id = id });
//        }

        /// <summary>
        /// Locks the serviceCollection for given id
        /// </summary>
        /// <param name="model">id of serviceCollection to be locked</param>
        /// <returns>locked id otherwise erro message</returns>
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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.LockServiceCollection(model.Id.Value, isLockDisAllowedForArchived) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited },
                    { typeof(RoleActionException), MessageLockServiceCollectionRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }

        /// <summary>
        /// Unlocks the serviceCollection for given id
        /// </summary>
        /// <param name="model">id of serviceCollection to be unlocked</param>
        /// <returns>unlocked id otherwise erro message</returns>
        [Route("UnLockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockEntity([FromBody] VmEntityBasic model)
        {
            formStateService.Delete(model.Id, pahaTokenProcessor.UserName);
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.UnLockServiceCollection(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Withdraws the serviceCollection
        /// </summary>
        /// <param name="model">model containing the serviceCollection id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = serviceCollectionService.RestoreServiceCollection(model.Id.Value),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessageRestoreServiceCollectionRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
                });
        }

//        /// <summary>
//        /// Locks the serviceCollection
//        /// </summary>
//        /// <param name="id">serviceCollection id</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreEntity(Guid id)
//        {
//            return LockService(new VmServiceCollectionBasic() { Id = id });
//        }

//        /// <summary>
//        /// Locks the serviceCollection
//        /// </summary>
//        /// <param name="id">serviceCollection id</param>
//        /// <returns>result about locking</returns>
//        [Route("WithdrawEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap WithdrawEntity(Guid id)
//        {
//            return LockService(new VmServiceCollectionBasic() { Id = id });
//        }

        /// <summary>
        /// Withdraws the serviceCollection
        /// </summary>
        /// <param name="model">model containing the serviceCollection id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.WithdrawServiceCollection(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessageWithdrawServiceCollectionRole },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

//        /// <summary>
//        /// Lock  serviceCollection
//        /// </summary>
//        /// <param name="id">serviceCollection id</param>
//        /// <returns>result about locking</returns>
//        [Route("WithdrawLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap WithdrawLanguage(Guid id)
//        {
//            return LockService(new VmServiceCollectionBasic() {Id = id});
//        }

        /// <summary>
        /// Withdraw language of the serviceCollection
        /// </summary>
        /// <param name="model">model containing the serviceCollection id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.WithdrawLanguage(model) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessageWithdrawServiceCollectionRole },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

        /// <summary>
        /// Get Validated entity
        /// </summary>
        /// <param name="model">model containing the serviceCollection id and language id</param>
        /// <returns>result about publishing</returns>
        [Route("GetValidatedEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap GetValidatedEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceCollectionService.GetValidatedEntity(model) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(LockNotAllowedException), EntityMessageCannotBePublished }
                });
        }

        /// <summary>
        /// Publishs the serviceCollection
        /// </summary>
        /// <param name="model">model containing the serviceCollection id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = serviceCollectionService.PublishServiceCollection(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessagePublished },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessagePublishServiceCollectionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }
        /// <summary>
        /// schedule publishing or archiving of the service collection
        /// </summary>
        /// <param name="model">model containing the service collection id to be scheduled</param>
        /// <returns>result about scheduling</returns>
        [Route("ScheduleEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ScheduleEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = serviceCollectionService.ScheduleServiceCollection(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageSchedulePublish },
                    { typeof(LockException), MessageLockedServiceCollection },
                    { typeof(RoleActionException), MessagePublishServiceCollectionRole },
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
                Data = serviceCollectionService.SaveRelations(model)
            },
            new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddServiceAndChannel },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited }
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
            () => new ServiceResultWrap()
            {
                Data = model.Id.HasValue ? serviceCollectionService.IsConnectable(model.Id.Value) : null,
            },
            new Dictionary<Type, string>()
                {
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited }
                });
        }
        /*/// <summary>
        /// Search service collections for given criteria
        /// </summary>
        /// <param name="model">model containing search criteria</param>
        /// <returns>found services</returns>
        [Route("SearchServiceCollections")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap SearchServiceCollections([FromBody] VmServiceCollectionSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = serviceCollectionService.SearchServiceCollections(model)
                },
                new Dictionary<Type, string>());
        }
        */
        /// <summary>
        /// Gets service collection entity history
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetEntityHistory")]
        [HttpPost]
        public IServiceResultWrap GetServiceCollectionEntityHistory([FromBody] VmHistorySearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = entityHistoryService.GetServiceCollectionEntityHistory(model)
                },
                new Dictionary<Type, string> { }
            );
        }
        /// <summary>
        /// Gets service collection connection history
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetConnectionHistory")]
        [HttpPost]
        public IServiceResultWrap GetServiceCollectionConnectionHistory([FromBody] VmHistorySearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = entityHistoryService.GetServiceCollectionConnectionHistory(model)
                },
                new Dictionary<Type, string> { }
            );
        }
    }
}
