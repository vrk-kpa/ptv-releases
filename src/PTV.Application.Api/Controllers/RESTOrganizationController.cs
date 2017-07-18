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

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Enums;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Organizations"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/organization")]
    [Controller]
    public class RESTOrganizationController : RESTBaseController
    {
        private readonly IOrganizationService organizationService;
        private readonly IMunicipalityService municipalityService;
        private readonly IServiceManager serviceManager;

        private const string MessageOrganizationStepSave = "Organization.OrganizationStep.MessageSaved";
        private const string MessageArgumentDuplicityException = "Organization.Exception.DuplicityCheck";
        private const string MessageArgumentOrganizationTypeException = "Organization.Exception.OrganizationType";
        private const string MessageAddOrganization = "Organization.AddOrganization.MessageSaved";
        private const string MessageDeleteOrganization = "Organization.EditOrganization.MessageDeleted";
        private const string MessagePublishOrganization = "Organization.EditOrganization.MessagePublished";
        private const string MessageWithdrawOrganization = "Organization.EditOrganization.MessageWithdrawed";
        private const string MessageRestoreOrganization = "Organization.EditOrganization.MessageRestored";
        private const string MessageLockedOrganization = "Organization.Exception.MessageLock";

        private const string MessageAddOrganizationRole = "Organization.RoleException.MessageAdd";
        private const string MessageSaveOrganizationRole = "Organization.RoleException.MessageSave";
        private const string MessagePublishOrganizationRole = "Organization.RoleException.MessagePublish";
        private const string MessageWithdrawOrganizationRole = "Organization.RoleException.MessageWithdraw";
        private const string MessageRestoreOrganizationRole = "Organization.RoleException.MessageRestore";
        private const string MessageDeleteOrganizationRole = "Organization.RoleException.MessageDelete";
        private const string MessageLockOrganizationRole = "Organization.RoleException.MessageLock";

        private const string MessageOrganizationCannotDeleteInUse = "Organization.Exception.CannotRemove";
        private const string MessageOrganizationCyclicDependency = "Organization.Exception.CyclicDependency";


        /// <summary>
        /// Constructor of organization controller
        /// </summary>
        /// <param name="organizationService">organization service responsible for operation related to organizations - injected by framework</param>
        /// <param name="municipalityService">municipality service responsible for operation related to municipalities - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTOrganizationController(IOrganizationService organizationService, IMunicipalityService municipalityService, IServiceManager serviceManager, ILogger<RESTOrganizationController> logger) : base(logger)
        {
            this.organizationService = organizationService;
            this.municipalityService = municipalityService;
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Get data for organizations search form
        /// </summary>
        /// <returns>data needed for search form like publishing statuses, types etc.</returns>
        [Route("GetOrganizationSearch")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetOrganizationSearch()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = organizationService.GetOrganizationSearch() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Searches organization for given criteria
        /// </summary>
        /// <param name="model">model containing search criteria</param>
        /// <returns>searched organizations</returns>
        [Route("SearchOrganizations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap SearchOrganizations([FromBody] VmOrganizationSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = organizationService.SearchOrganizations(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get names for organization id
        /// </summary>
        /// <returns>names for organization id</returns>
        [Route("GetOrganizationNames")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetOrganizationNames([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = organizationService.GetOrganizationNames(model) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets data fororganization step1
        /// </summary>
        /// <param name="model">model containing id of organization or empty</param>
        /// <returns>all data needed for organization step 1</returns>
        [Route("GetOrganizationStep1")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetOrganizationStep1([FromBody] VmGetOrganizationStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = organizationService.GetOrganizationStep1(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Saves organization to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of organization otherwise error message</returns>
        [Route("SaveAllChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveAllChanges([FromBody] VmOrganizationModel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = organizationService.AddApiOrganization(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddOrganization },
                    { typeof(PtvArgumentException), MessageArgumentDuplicityException },
                    { typeof(RoleActionException), MessageAddOrganizationRole },
                    { typeof(PtvServiceArgumentException), MessageArgumentOrganizationTypeException },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency }
                }
            );
        }

        /// <summary>
        /// Saves organization step 1 to database
        /// </summary>
        /// <param name="model">model containing all data needed to be saved</param>
        /// <returns>if succeed, the id of organization otherwise error message</returns>
        [Route("SaveStep1Changes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveStep1Changes([FromBody] VmOrganizationModel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model.Step1Form) { Data = organizationService.SaveOrganizationStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageOrganizationStepSave },
                    { typeof(PtvArgumentException), MessageArgumentDuplicityException },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageSaveOrganizationRole },
                    { typeof(PtvServiceArgumentException), MessageArgumentOrganizationTypeException },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency }
                }
            );
        }

        /// <summary>
        /// Gets the publishing status of organization
        /// </summary>
        /// <param name="model">model containing id of the organization</param>
        /// <returns>publishing status of the organizations</returns>
        [Route("GetOrganizationStatus")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetOrganizationStatus([FromBody] VmOrganizationModel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = organizationService.GetOrganizationStatus(model.Id.Value) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets municipalities for given code or name
        /// </summary>
        /// <param name="searchedCode">code or name of the municipality</param>
        /// <returns>municipalities for given search criteria</returns>
        [Route("GetMunicipalities")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetMunicipalities([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = municipalityService.GetMunicipalities(searchedCode) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Publishs the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be published</param>
        /// <returns>id of published organization</returns>
        [Route("PublishOrganization")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishOrganization([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.PublishOrganization(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessagePublishOrganization },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessagePublishOrganizationRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
                });
        }

        /// <summary>
        /// Withdraws the service
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("WithdrawOrganization")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap WithdrawOrganization([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = organizationService.WithdrawOrganization(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageWithdrawOrganization },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageWithdrawOrganizationRole },
                    { typeof(WithdrawModifiedExistsException), RESTCommonController.MessageWithdrawModifiedExists },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency }
                });
        }

        /// <summary>
        /// Restore the service
        /// </summary>
        /// <param name="model">model containing the service id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("RestoreOrganization")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap RestoreOrganization([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = organizationService.RestoreOrganization(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageRestoreOrganization },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageRestoreOrganizationRole },
                    { typeof(RestoreModifiedExistsException), RESTCommonController.MessageRestoreModifiedExists },
                    { typeof(OrganizationCyclicDependencyException), MessageOrganizationCyclicDependency }
                });
        }

        /// <summary>
        /// Deletes the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be deleted</param>
        /// <returns>id of deleted organization</returns>
        [Route("DeleteOrganization")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap DeleteOrganization([FromBody] VmOrganizationModelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = organizationService.DeleteOrganization(model.Id) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageDeleteOrganization },
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageDeleteOrganizationRole },
                    { typeof(OrganizationNotDeleteInUseException), MessageOrganizationCannotDeleteInUse }
                });
        }

        /// <summary>
        /// Gets organizations for given text
        /// </summary>
        /// <param name="searchText">name of the organization</param>
        /// <returns>found organizations</returns>
        [Route("GetOrganizations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetOrganizations([FromBody] string searchText)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.GetOrganizations(searchText)},
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Locks the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be locked</param>
        /// <returns>id of locked organization otherwise error message</returns>
        [Route("LockOrganization")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap LockOrganization([FromBody] VmOrganizationModelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? organizationService.LockOrganization(model.Id.Value) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedOrganization },
                    { typeof(RoleActionException), MessageLockOrganizationRole },
                    { typeof(ModifiedExistsException), RESTCommonController.MessageUnableEditLocked }
                });
        }

        /// <summary>
        /// Unlocks the organization
        /// </summary>
        /// <param name="model">model containing the id of the organization to be unlocked</param>
        /// <returns>id of unlocked organization otherwise error message</returns>
        [Route("UnLockOrganization")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap UnLockOrganization([FromBody] VmOrganizationModelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? organizationService.UnLockOrganization(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Checks whether the organization is locked or not
        /// </summary>
        /// <param name="model">model containing the id of the organization to be checked</param>
        /// <returns>true otherwise false</returns>
        [Route("IsOrganizationLocked")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap IsOrganizationLocked([FromBody] VmOrganizationModelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? organizationService.IsOrganizationLocked(model.Id.Value) : null,
                },
                new Dictionary<Type, string>() { { typeof(LockException), MessageLockedOrganization } });
        }

        /// <summary>
        /// Chekcs whether organization is editable or not
        /// </summary>
        /// <param name="model">model containing the organization id</param>
        /// <returns>true otherwise false</returns>
        [Route("IsOrganizationEditable")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap IsOrganizationEditable([FromBody] VmOrganizationModelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = model.Id != null ? organizationService.IsOrganizationEditable(model.Id.Value) : null,
                },
                new Dictionary<Type, string>());
        }
    }
}
