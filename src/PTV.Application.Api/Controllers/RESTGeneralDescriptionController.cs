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
using PTV.Framework;
using PTV.Framework.Enums;

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

        private const string MessageAddGeneralDescription = "GeneralDescription.AddGeneralDescription.MessageSaved";
        private const string MessageSaveGeneralDescription = "GeneralDescription.UpdateGeneralDescription.MessageSaved";
        private const string MessageLockedGeneralDescription = "GeneralDescription.Exception.MessageLock";
        private const string MessageArgumentException = "GeneralDescription.Exception.InvalidArgument";
        private const string MessagePublishGeneralDescription = "GeneralDescription.Published.Successfully";
        private const string MessageWithdrawGeneralDescription = "GeneralDescription.Withdraw.Successfully";
        private const string MessageRestoreGeneralDescription = "GeneralDescription.Restore.Successfully";
        private const string MessageDeleteGeneralDescription = "GeneralDescription.Deleted.Successfully";

        private const string MessageAddGeneralDescriptionRole = "GeneralDescription.RoleException.MessageAdd";
        private const string MessageSaveGeneralDescriptionRole = "GeneralDescription.RoleException.MessageSave";
        private const string MessagePublishGeneralDescriptionRole = "GeneralDescription.RoleException.MessagePublish";
        private const string MessageWithdrawGeneralDescriptionRole = "GeneralDescription.RoleException.MessageWithdraw";
        private const string MessageRestoreGeneralDescriptionRole = "GeneralDescription.RoleException.MessageRestore";
        private const string MessageDeleteGeneralDescriptionRole = "GeneralDescription.RoleException.MessageDelete";
        private const string MessageLockGeneralDescriptionRole = "GeneralDescription.RoleException.MessageLock";


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
        [Route("SearchGeneralDescriptions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap SearchGeneralDescriptions([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionSearchForm searchData)
            {
            return serviceManager.CallService(
              () => new ServiceResultWrap() { Data = generalDescriptionService.SearchGeneralDescriptions(searchData) },
              new Dictionary<Type, string>());
        }

        /// <summary>
        /// Search for general descriptions based on search criteria
        /// </summary>
        /// <param name="searchData">Search criteria</param>
        /// <returns>List of general descriptions</returns>
        [Route("v2/SearchGeneralDescriptions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap SearchGeneralDescriptions_V2([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionSearchForm searchData)
        {
            return serviceManager.CallService(
              () => new ServiceResultWrap() { Data = generalDescriptionService.SearchGeneralDescriptions_V2(searchData) },
              new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the genereal description
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>general description</returns>
        [Route("v2/GetGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetGeneralDescription_V2([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionIn model)
        {
            return serviceManager.CallService(
              () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescription_V2(model) },
              new Dictionary<Type, string>());
        }

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
        [Route("GetGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetGeneralDescription([Microsoft.AspNetCore.Mvc.FromBody] Guid id)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescriptionById(id) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data for general description
        /// </summary>
        /// <param name="model">Model containing id of general description or empty</param>
        /// <returns>all data needed for general description</returns>
        [Route("GetAddGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddGeneralDescription([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionIn model)
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
                    Data = model.Id.HasValue ? generalDescriptionService.UpdateGeneralDescription(model) : generalDescriptionService.AddGeneralDescription(model),
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
        /// Withdraws the general descriptions
        /// </summary>
        /// <param name="model">model containing the general descriptions id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawGeneralDescription([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = generalDescriptionService.WithdrawGeneralDescription(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageWithdrawGeneralDescription },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageWithdrawGeneralDescriptionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists }
                });
        }

        /// <summary>
        /// Restore the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreGeneralDescription([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = generalDescriptionService.RestoreGeneralDescription(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageRestoreGeneralDescription },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageRestoreGeneralDescriptionRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists }
                });
        }

        /// <summary>
        /// Deletes the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be deleted</param>
        /// <returns>result about publishing</returns>
        [Route("DeleteGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap DeleteGeneralDescription([FromBody] VmGeneralDescriptionIn model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = generalDescriptionService.DeleteGeneralDescription(model.Id) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageDeleteGeneralDescription },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageDeleteGeneralDescriptionRole }
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
        public IServiceResultWrap GetGeneralDescriptionStatus([FromBody] VmGeneralDescriptionIn model)
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
        [Route("LockGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap LockGeneralDescription([FromBody] VmGeneralDescriptionIn model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? generalDescriptionService.LockGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessageLockGeneralDescriptionRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }

        /// <summary>
        /// Unlocks the general description for given id
        /// </summary>
        /// <param name="model">id of general description to be unlocked</param>
        /// <returns>unlocked id otherwise erro message</returns>
        [Route("UnLockGeneralDescription")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockGeneralDescription([FromBody] VmGeneralDescriptionIn model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? generalDescriptionService.UnLockGeneralDescription(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Chekcs whether general description is locked or not
        /// </summary>
        /// <param name="model">model containing the general description id</param>
        /// <returns>true otherwise false</returns>
        [Route("IsGeneralDescriptionLocked")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap IsGeneralDescriptionLocked([FromBody] VmGeneralDescriptionIn model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? generalDescriptionService.IsGeneralDescriptionLocked(model.Id.Value) : null,
                },
                new Dictionary<Type, string>() { { typeof(LockException), MessageLockedGeneralDescription } });
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
        public IServiceResultWrap IsGeneralDescriptionEditable([FromBody] VmGeneralDescriptionIn model)
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
