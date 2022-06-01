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
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;
using PTV.Domain.Model.Models.V2.Organization;
using PTV.Framework.Enums;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Framework.Exceptions.DataAccess;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "Organizations"
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/organization")]
    public class RESTOrganizationControllerV2 : RESTBaseController
    {
        private readonly IOrganizationService organizationService;
        private readonly IEntityHistoryService entityHistoryService;
        private readonly IServiceManager serviceManager;
        private readonly IPahaTokenProcessor pahaTokenProcessor;
        private readonly Database.DataAccess.Interfaces.Services.IFormStateService formStateService;

        private const string MessageArgumentDuplicityException = "Organization.Exception.DuplicityCheck";
        private const string MessageArgumentOidFormatException = "Organization.Exception.OidFormat";
        private const string MessageArgumentOrganizationTypeException = "Organization.Exception.OrganizationType";
        private const string MessageLockedOrganization = "Organization.Exception.MessageLock";

        private const string MessageAddOrganizationRole = "Organization.RoleException.MessageAdd";
        private const string MessageSaveOrganizationRole = "Organization.RoleException.MessageSave";
        private const string MessagePublishOrganizationRole = "Organization.RoleException.MessagePublish";
        private const string MessageWithdrawOrganizationRole = "Organization.RoleException.MessageWithdraw";
        private const string MessageRestoreOrganizationRole = "Organization.RoleException.MessageRestore";
        private const string MessageDeleteOrganizationRole = "Organization.RoleException.MessageDelete";
        private const string MessageLockOrganizationRole = "Organization.RoleException.MessageLock";

        private const string MessageOrganizationCannotDeleteInUse = "Organization.Exception.UserInUse.CannotRemove";
        private const string MessageOrganizationCyclicDependency = "Organization.Exception.CyclicDependency";
        private const string MessageWithdrawConnectionExists = "Organization.Exception.WithdrawConnection";
        private const string MessageOrganizationMaxHierarchyLevel = "Organization.Exception.MaxHierarchyLevel";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>OS
        /// <param name="organizationService">organization service responsible for operation related to organization - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="entityHistoryService"></param>
        /// <param name="formStateService">formState service responsible for operation related to formStates - injected by framework</param>
        /// <param name="pahaTokenProcessor"></param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTOrganizationControllerV2(
            IOrganizationService organizationService,
            IServiceManager serviceManager,
            IEntityHistoryService entityHistoryService,
            Database.DataAccess.Interfaces.Services.IFormStateService formStateService,
            IPahaTokenProcessor pahaTokenProcessor,
            ILogger<RESTOrganizationControllerV2> logger
        ) : base(logger)
        {
            this.organizationService = organizationService;
            this.serviceManager = serviceManager;
            this.entityHistoryService = entityHistoryService;
            this.formStateService = formStateService;
            this.pahaTokenProcessor = pahaTokenProcessor;
        }

        /// <summary>
        /// Gets data for service form
        /// </summary>
        /// <param name="model">model containing id of organization or empty</param>
        /// <returns>all data needed for organization</returns>
        [Route("GetOrganization")]
        [HttpPost]
        public IServiceResultWrap GetOrganization([FromBody] VmOrganizationBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = organizationService.GetOrganization(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves organization form
        /// </summary>
        /// <param name="model">model organization</param>
        /// <returns>organization</returns>
        [Route("SaveOrganization")]
        [HttpPost]
        public IServiceResultWrap SaveOrganization([FromBody] VmOrganizationInput model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = SaveOrganizationData(model) },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(PtvArgumentException), MessageArgumentDuplicityException },
                    { typeof(IdFormatException), MessageArgumentOidFormatException },
                    { typeof(RoleActionException), model.Id.HasValue ? MessageSaveOrganizationRole : MessageAddOrganizationRole },
                    { typeof(PtvServiceArgumentException), MessageArgumentOrganizationTypeException },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(OrganizationMaxHierarchyLevelException), MessageOrganizationMaxHierarchyLevel },
                    { typeof(CoordinateException), RESTCommonController.CoordinatesConnectionFailed },
                });
        }

        private IVmBase SaveOrganizationData(VmOrganizationInput model)
        {
            var result = organizationService.SaveOrganization(model);
            formStateService.Delete(model.Id, null);
            return result;
        }


        /// <summary>
        /// Gets data for organization header
        /// </summary>
        /// <param name="model">organization identifier in model</param>
        /// <returns>all data needed for organization header</returns>
        [Route("GetOrganizationHeader")]
        [HttpPost]
        public IServiceResultWrap GetOrganizationHeader([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = organizationService.GetOrganizationHeader(model.Id) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Locks the organization
        /// </summary>
        /// <param name="id">organization id</param>
        /// <returns>result about locking</returns>
        [Route("ArchiveEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity(Guid id)
        {
            var entityLock = LockOrganization(new VmOrganizationBasic { Id = id });
            if (entityLock.Messages.Errors.Count > 0)
            {
                return entityLock;
            }
            return serviceManager.CallService(
               () => new ServiceResultWrap
               {
                   // todo: call this check from ui
                   Data = organizationService.CheckDeleteOrganization(id)
               },
               new Dictionary<Type, string>
               {
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageDeleteOrganizationRole },
                    { typeof(OrganizationNotDeleteInUserUseException), MessageOrganizationCannotDeleteInUse }
               });
        }

        /// <summary>
        /// Archives the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be deleted</param>
        /// <returns>id of deleted organization</returns>
        [Route("ArchiveEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? organizationService.DeleteOrganization(model.Id.Value) : null
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageDeleteOrganizationRole },
                    { typeof(OrganizationNotDeleteInUserUseException), MessageOrganizationCannotDeleteInUse }
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
                    Data = model.Id.HasValue ? organizationService.RemoveOrganization(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageDeleteOrganizationRole }
                });
        }

//        /// <summary>
//        /// Locks organization
//        /// </summary>
//        /// <param name="id">organization id</param>
//        /// <returns>result about locking</returns>
//        [Route("ArchiveLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap ArchiveLanguage(Guid id)
//        {
//            return LockOrganization(new VmOrganizationBasic() { Id = id });
//        }

        /// <summary>
        /// Archives language of the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be deleted</param>
        /// <returns>id of deleted organization</returns>
        [Route("ArchiveLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? organizationService.ArchiveLanguage(model) : null
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageDeleteOrganizationRole },
                    { typeof(OrganizationNotDeleteInUserUseException), MessageOrganizationCannotDeleteInUse },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage }
                });
        }

//        /// <summary>
//        /// Locks organization
//        /// </summary>
//        /// <param name="id">organization id</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreLanguage")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreLanguage(Guid id)
//        {
//            return LockOrganization(new VmOrganizationBasic() { Id = id });
//        }

        /// <summary>
        /// Restores language of the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be restored</param>
        /// <returns>id of deleted organization</returns>
        [Route("RestoreLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? organizationService.RestoreLanguage(model) : null
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageDeleteOrganizationRole },
                    { typeof(OrganizationNotDeleteInUserUseException), MessageOrganizationCannotDeleteInUse },
                    { typeof(RestoreLanguageException), RESTCommonController.MessageRestoreLanguage }
                });
        }

        /// <summary>
        /// Locks the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be locked</param>
        /// <returns>id of locked organization otherwise error message</returns>
        [Route("LockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap LockEntity([FromBody] VmEntityBasic model)
        {
            return LockOrganization(model, true);
        }

        private IServiceResultWrap LockOrganization(VmEntityBasic model, bool isLockDisAllowedForArchived = false)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? organizationService.LockOrganization(model.Id.Value, isLockDisAllowedForArchived) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(LockNotAllowedException), EntityMessageCannotBeEdited },
                    { typeof(RoleActionException), MessageLockOrganizationRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }

        /// <summary>
        /// Unlocks the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be unlocked</param>
        /// <returns>id of unlocked organization otherwise error message</returns>
        [Route("UnLockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockEntity([FromBody] VmEntityBasic model)
        {
            formStateService.Delete(model.Id, pahaTokenProcessor.UserName);
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = model.Id.HasValue ? organizationService.UnLockOrganization(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

//        /// <summary>
//        /// Lock the organization
//        /// </summary>
//        /// <param name="id">organization id</param>
//        /// <returns>result about locking</returns>
//        [Route("RestoreEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap RestoreEntity(Guid id)
//        {
//            return LockOrganization(new VmOrganizationBasic() { Id = id });
//        }

        /// <summary>
        /// Restore the organization
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
                    Data = model.Id.HasValue ? organizationService.RestoreOrganization(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageRestoreOrganizationRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency }
                });
        }

//        /// <summary>
//        /// Lock the organization
//        /// </summary>
//        /// <param name="id">organization id</param>
//        /// <returns>result about locking</returns>
//        [Route("WithdrawEntity")]
//        [HttpGet]
//        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
//        public IServiceResultWrap WithdrawEntity(Guid id)
//        {
//            return LockOrganization(new VmOrganizationBasic() { Id = id });
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
                    Data = model.Id.HasValue ? organizationService.WithdrawOrganization(model.Id.Value) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageWithdrawOrganizationRole },
                    { typeof(WithdrawConnectedExistsException), MessageWithdrawConnectionExists },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency }
                });
        }

        /// <summary>
        /// Locke organization
        /// </summary>
        /// <param name="id">organization id</param>
        /// <returns>result about locking</returns>
        [Route("WithdrawLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawLanguage(Guid id)
        {
            return LockOrganization(new VmOrganizationBasic { Id = id });
        }

        /// <summary>
        /// Withdraws language of the service
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
                    Data = model.Id.HasValue ? organizationService.WithdrawLanguage(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageWithdrawOrganizationRole },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency }
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
                    Data = model.Id.HasValue ? organizationService.GetValidatedEntity(model) : null,
                },
                new Dictionary<Type, string>
                {
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(LockNotAllowedException), EntityMessageCannotBePublished }
                });
        }

        /// <summary>
        /// Publishs the organization
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
                    Data = organizationService.PublishOrganization(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessagePublished },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessagePublishOrganizationRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }

        /// <summary>
        /// schedule publishing or archiving of the organization
        /// </summary>
        /// <param name="model">model containing the organization id to be scheduled</param>
        /// <returns>result about scheduling</returns>
        [Route("ScheduleEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ScheduleEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = organizationService.ScheduleOrganization(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSchedulePublish },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessagePublishOrganizationRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists },
                    { typeof(SchedulePublishException), RESTCommonController.MessageSchedulePublishError}
                });
        }
        /// <summary>
        /// Gets organization entity history
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetEntityHistory")]
        [HttpPost]
        public IServiceResultWrap GetOrganizationEntityHistory([FromBody] VmHistorySearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = entityHistoryService.GetOrganizationEntityHistory(model)
                },
                new Dictionary<Type, string>());
        }
    }
}
