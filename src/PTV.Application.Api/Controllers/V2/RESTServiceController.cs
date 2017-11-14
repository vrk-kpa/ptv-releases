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
using System;
using PTV.Framework.ServiceManager;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using IServiceService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceService;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "Services"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/service")]
    public class RESTServiceControllerV2 : RESTBaseController
    {
        private readonly IServiceService serviceService;
        private readonly IServiceManager serviceManager;
        
        private const string MessageLockedService = "Service.Exception.MessageLock";       

        private const string MessageAddServiceRole = "Service.RoleException.MessageAdd";
        private const string MessageSaveServiceRole = "Service.RoleException.MessageSave";
        private const string MessagePublishServiceRole = "Service.RoleException.MessagePublish";
        private const string MessageWithdrawServiceRole = "Service.RoleException.MessageWithdraw";
        private const string MessageRestoreServiceRole = "Service.RoleException.MessageRestore";
        private const string MessageDeleteServiceRole = "Service.RoleException.MessageDelete";
        private const string MessageLockServiceRole = "Service.RoleException.MessageLock";

        private const string MessageAddServiceAndChannel = "ServiceAndChannel.AddServiceAndChannel.MessageSaved";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="serviceService">service service responsible for operation related to service - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTServiceControllerV2(IServiceService serviceService, IServiceManager serviceManager, ILogger<RESTChannelController> logger) : base(logger)
        {
            this.serviceService = serviceService;
            this.serviceManager = serviceManager;
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
        public IServiceResultWrap SaveService([FromBody] Domain.Model.Models.V2.Service.VmServiceInput model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = GetServiceData(model)

                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveServiceRole : MessageAddServiceRole }
                });
        }

        private IVmBase GetServiceData(Domain.Model.Models.V2.Service.VmServiceInput model)
        {
            VmServiceOutput result = null;
            switch (model.Action)
            {
                case ActionTypeEnum.Save:
                    result = serviceService.SaveService(model);
                    break;
                case ActionTypeEnum.SaveAndValidate:
                    result = serviceService.SaveAndValidateService(model);
                    break;
                default:
                    result = serviceService.SaveService(model);
                    break;
            }
            serviceService.UnLockService(result.Id.Value);
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
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id.HasValue ? serviceService.DeleteService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageDeleteServiceRole }
                });
        }

        /// <summary>
        /// Locks the service
        /// </summary>
        /// <param name="id">service id </param>
        /// <returns>result about locking</returns>
        [Route("ArchiveEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity(Guid id)
        {
            return LockService(new VmServiceBasic() { Id = id});
        }

        /// <summary>
        /// Locks service
        /// </summary>
        /// <param name="id">service id</param>
        /// <returns>result about locking</returns>
        [Route("ArchiveLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage(Guid id)
        {
            return LockService(new VmServiceBasic() { Id = id });
        }

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
                () => new ServiceLocalizedResultWrap(model)
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
                () => new ServiceLocalizedResultWrap(model)
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

        /// <summary>
        /// Locking of service
        /// </summary>
        /// <param name="id">service</param>
        /// <returns>result about locking</returns>
        [Route("RestoreLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage(Guid id)
        {
            return LockService(new VmServiceBasic() { Id = id });
        }

        /// <summary>
        /// Locks the service for given id
        /// </summary>
        /// <param name="model">id of service to be locked</param>
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
                    Data = model.Id.HasValue ? serviceService.LockService(model.Id.Value, isLockDisAllowedForArchived) : null,
                },
                new Dictionary<Type, string>()
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
        /// <returns>unlocked id otherwise erro message</returns>
        [Route("UnLockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
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
                () => new ServiceResultWrap()
                {
                    Data = serviceService.RestoreService(model.Id.Value),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageRestoreServiceRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
                });
        }

        /// <summary>
        /// Locks the service
        /// </summary>
        /// <param name="id">service id</param>
        /// <returns>result about locking</returns>
        [Route("RestoreEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreEntity(Guid id)
        {
            return LockService(new VmServiceBasic() { Id = id });
        }

        /// <summary>
        /// Locks the service
        /// </summary>
        /// <param name="id">service id</param>
        /// <returns>result about locking</returns>
        [Route("WithdrawEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawEntity(Guid id)
        {
            return LockService(new VmServiceBasic() { Id = id });
        }

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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceService.WithdrawService(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessageWithdrawServiceRole },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

        /// <summary>
        /// Lock  service
        /// </summary>
        /// <param name="id">service id</param>
        /// <returns>result about locking</returns>
        [Route("WithdrawLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage(Guid id)
        {
            return LockService(new VmServiceBasic() {Id = id});
        }

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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceService.WithdrawLanguage(model) : null,
                },
                new Dictionary<Type, string>()
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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? serviceService.GetValidatedEntity(model) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedService },
                    { typeof(LockNotAllowedException), EntityMessageCannotBePublished }
                });
        }

        /// <summary>
        /// Publishs the service
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = serviceService.PublishService(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessagePublished },
                    { typeof(LockException), MessageLockedService },
                    { typeof(RoleActionException), MessagePublishServiceRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
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
                Data = serviceService.SaveRelations(model)
            },
            new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddServiceAndChannel }
                });
        }
    }
}
