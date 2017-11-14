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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.Localization;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Enums;
using VmGeneralDescription = PTV.Domain.Model.Models.VmGeneralDescription;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "General Descriptions"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/generaldescription")]
    public class RESTGeneralDescriptionController : RESTBaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;
        private readonly IServiceManager serviceManager;

        private const string MessageAddGeneralDescription = "GeneralDescriptionOutput.AddGeneralDescription.MessageSaved";
        private const string MessageSaveGeneralDescription = "GeneralDescriptionOutput.UpdateGeneralDescription.MessageSaved";
        private const string MessageLockedGeneralDescription = "GeneralDescriptionOutput.Exception.MessageLock";
        private const string MessageArgumentException = "GeneralDescriptionOutput.Exception.InvalidArgument";
        private const string MessagePublishGeneralDescription = "GeneralDescriptionOutput.Published.Successfully";

        private const string MessageSaveGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageSave";
        private const string MessageWithdrawGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageWithdraw";
        private const string MessageRestoreGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageRestore";
        private const string MessageDeleteGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageDelete";
        private const string MessageLockGeneralDescriptionRole = "GeneralDescriptionOutput.RoleException.MessageLock";


        /// <summary>
        /// Constructor of General description controller
        /// </summary>
        /// <param name="generalDescriptionService">general description service responsible for operation related to general descriptions - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTGeneralDescriptionController(IGeneralDescriptionService generalDescriptionService, IServiceManager serviceManager, ILogger<RESTGeneralDescriptionController> logger) : base(logger)
        {
            this.generalDescriptionService = generalDescriptionService;
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Search for general descriptions based on search criteria
        /// </summary>
        /// <param name="searchData">Search criteria</param>
        /// <returns>List of general descriptions</returns>
        //[Route("SearchGeneralDescriptions")]
        //[HttpPost]
        //[AccessRightRequirement(AccessRightEnum.UiAppRead)]
        //public IServiceResultWrap SearchGeneralDescriptions([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionSearchForm searchData)
        //    {
        //    return serviceManager.CallService(
        //      () => new ServiceResultWrap() { Data = generalDescriptionService.SearchGeneralDescriptions(searchData) },
        //      new Dictionary<Type, string>());
        //}

        /// <summary>
        /// Search for general descriptions based on search criteria
        /// </summary>
        /// <param name="searchData">Search criteria</param>
        /// <returns>List of general descriptions</returns>
        [Route("v2/SearchGeneralDescriptions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap SearchGeneralDescriptions([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionSearchForm searchData)
        {
            return serviceManager.CallService(
              () => new ServiceResultWrap() { Data = generalDescriptionService.SearchGeneralDescriptions(searchData) },
              new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the genereal description
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>general description</returns>
        //[Route("v2/GetGeneralDescription")]
        //[HttpPost]
        //[AccessRightRequirement(AccessRightEnum.UiAppRead)]
        //public IServiceResultWrap GetGeneralDescription_V2([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionGet model)
        //{
        //    return serviceManager.CallService(
        //      () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescription_V2(model) },
        //      new Dictionary<Type, string>());
        //}

        /// <summary>
        /// Get all sub target groups by parent group ID
        /// </summary>
        /// <param name="targetGroupId">Parent group ID</param>
        /// <returns>List of sub target groups of parent group</returns>
        [Route("GetSubTargetGroups")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetSubTargetGroups([Microsoft.AspNetCore.Mvc.FromBody] Guid targetGroupId)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = generalDescriptionService.GetSubTargetGroups(targetGroupId) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get data for search general description form, i.e. input data for criterias fields
        /// </summary>
        /// <returns>Data for search form</returns>
        [Route("GetGeneralDescriptionSearch")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetGeneralDescriptionSearch()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescriptionSearchForm() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get data for search general description form, i.e. input data for criterias fields
        /// </summary>
        /// <returns>Data for search form</returns>
        //[Route("GetGeneralDescription")]
        //[HttpPost]
        //[AccessRightRequirement(AccessRightEnum.UiAppRead)]
        //public IServiceResultWrap GetGeneralDescription([Microsoft.AspNetCore.Mvc.FromBody] Guid id)
        //{
        //    return serviceManager.CallService(
        //        () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescriptionById(id) },
        //        new Dictionary<Type, string>());
        //}

        /// <summary>
        /// Gets data for general description
        /// </summary>
        /// <param name="model">Model containing id of general description or empty</param>
        /// <returns>all data needed for general description</returns>
        [Route("GetAddGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddGeneralDescription([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionGet model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = generalDescriptionService.GetGeneralDescription(model)
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves general descriptions to database
        /// </summary>
        /// <param name="model">Model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of general descriptions otherwise error message</returns>
        [Route("SaveAllChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveAllChanges([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescription model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    //Data = model.Id.HasValue ? generalDescriptionService.UpdateGeneralDescription(model) : generalDescriptionService.AddGeneralDescription(model),
                },
                new Dictionary<Type, string>() {
                    { typeof(string), model.Id.HasValue ? MessageSaveGeneralDescription : MessageAddGeneralDescription },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageSaveGeneralDescriptionRole }
                });
        }

        /// <summary>
        /// Publish the general descriptions
        /// </summary>
        /// <param name="model">Model containing the general description id to be published</param>
        /// <returns>Result about publishing</returns>
        [Route("PublishGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishGeneralDescription([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = generalDescriptionService.PublishGeneralDescription(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessagePublishGeneralDescription },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessagePublishGeneralDescription },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }

        /// <summary>
        /// Lock the general description
        /// </summary>
        /// <param name="id">general description id</param>
        /// <returns>result about locking</returns>
        [Route("WithdrawEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawEntity(Guid id)
        {
            return LockGeneralDescription(new VmEntityBasic() { Id = id });
        }

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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? generalDescriptionService.WithdrawGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
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
            return LockGeneralDescription(new VmEntityBasic() { Id = id });
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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? generalDescriptionService.WithdrawLanguage(model) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageWithdrawn },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageWithdrawGeneralDescriptionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

        /// <summary>
        /// Lock the general description
        /// </summary>
        /// <param name="id">general description id</param>
        /// <returns>result about locking</returns>
        [Route("RestoreEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreEntity(Guid id)
        {
            return LockGeneralDescription(new VmEntityBasic() { Id = id });
        }

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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? generalDescriptionService.RestoreGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageRestoreGeneralDescriptionRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
                });
        }

        /// <summary>
        /// Locks the general description
        /// </summary>
        /// <param name="id">general description id</param>
        /// <returns>result about locking</returns>
        [Route("ArchiveEntity")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveEntity(Guid id)
        {
            return LockGeneralDescription(new VmEntityBasic() { Id = id });
        }

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
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id.HasValue ? generalDescriptionService.DeleteGeneralDescription(model.Id) : null
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole }
                });
        }

        /// <summary>
        /// Locks general description
        /// </summary>
        /// <param name="id">general description id</param>
        /// <returns>result about locking</returns>
        [Route("ArchiveLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage(Guid id)
        {
            return LockGeneralDescription(new VmEntityBasic() { Id = id });
        }

        /// <summary>
        /// Archive lanugage of the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("ArchiveLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap ArchiveLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id.HasValue ? generalDescriptionService.ArchiveLanguage(model) : null
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageArchived },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole },
                    { typeof(ArchiveLanguageException), RESTCommonController.MessageDeleteLanguage }
                });
        }

        /// <summary>
        /// Locks general description
        /// </summary>
        /// <param name="id">general description id</param>
        /// <returns>result about locking</returns>
        [Route("RestoreLanguage")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage(Guid id)
        {
            return LockGeneralDescription(new VmEntityBasic() { Id = id });
        }

        /// <summary>
        /// Restore lanugage of the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be restored</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreLanguage")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreLanguage([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = model.Id.HasValue ? generalDescriptionService.RestoreLanguage(model) : null
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageRestored },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole },
                    { typeof(RestoreLanguageException), RESTCommonController.MessageRestoreLanguage }
                });
        }

        /// <summary>
        /// Gets the publishing status of the general description
        /// </summary>
        /// <param name="model">model containing the general description id</param>
        /// <returns>publishing status of the general description</returns>
        [Route("GetGeneralDescriptionStatus")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetGeneralDescriptionStatus([FromBody] VmGeneralDescriptionGet model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = generalDescriptionService.GetGeneralDescriptionStatus(model.Id.Value) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Locks the general description for given id
        /// </summary>
        /// <param name="model">id of general description to be locked</param>
        /// <returns>locked id otherwise erro message</returns>
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
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? generalDescriptionService.LockGeneralDescription(model.Id.Value, isLockDisAllowedForArchived) : null,
                },
                new Dictionary<Type, string>()
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
        /// <returns>unlocked id otherwise erro message</returns>
        [Route("UnLockEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockEntity([FromBody] VmEntityBasic model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? generalDescriptionService.UnLockGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get names for general descriptions id
        /// </summary>
        /// <returns>names for general descriptions  id</returns>
        [Route("GetGeneralDescriptionNames")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetGeneralDescriptionNames([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescriptionNames(model) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Chekcs whether general descriptions is editable or not
        /// </summary>
        /// <param name="model">model containing the general descriptions id</param>
        /// <returns>true otherwise false</returns>
        [Route("IsGeneralDescriptionEditable")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap IsGeneralDescriptionEditable([FromBody] VmGeneralDescriptionGet model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? generalDescriptionService.IsGeneralDescriptionEditable(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }
    }
}
