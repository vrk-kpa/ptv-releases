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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Enums;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "General Description"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/generalDescription")]
    public class RESTGeneralDescriptionControllerV2 : RESTBaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;
        private readonly IServiceManager serviceManager;
        
        private const string MessageSaveGeneralDescription = "GeneralDescription.UpdateGeneralDescription.MessageSaved";
        private const string MessageLockedGeneralDescription = "GeneralDescription.Exception.MessageLock";
        private const string MessageArgumentException = "GeneralDescription.Exception.InvalidArgument";
        
        private const string MessageSaveGeneralDescriptionRole = "GeneralDescription.RoleException.MessageSave";
        private const string MessagePublishGeneralDescriptionRole = "GeneralDescription.RoleException.MessagePublish";

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="generalDescriptionService">generalDescriptionS service responsible for operation related to general description - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTGeneralDescriptionControllerV2(IGeneralDescriptionService generalDescriptionService, IServiceManager serviceManager, ILogger<RESTChannelController> logger) : base((ILogger) logger)
        {
            this.generalDescriptionService = generalDescriptionService;
            this.serviceManager = serviceManager;
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
        public IServiceResultWrap SaveGeneralDescription([FromBody] Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionInput model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = GetGeneralDescriptionData(model) },
                new Dictionary<Type, string>
                {
                    { typeof(string), EntityMessageSaved },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(PtvArgumentException), MessageArgumentException },
                    { typeof(RoleActionException), MessageSaveGeneralDescriptionRole }
                });
        }

        private IVmBase GetGeneralDescriptionData(Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionInput model)
        {
            VmGeneralDescriptionOutput result = null;
            switch (model.Action)
            {
                case ActionTypeEnum.Save:
                    result = generalDescriptionService.SaveGeneralDescription(model);
                    break;
                case ActionTypeEnum.SaveAndValidate:
                    result = generalDescriptionService.SaveAndValidateGeneralDescription(model);
                    break;
                default:
                    result = generalDescriptionService.SaveGeneralDescription(model);
                    break;
            }
            generalDescriptionService.UnLockGeneralDescription(result.Id.Value);
            return result;
        }


        /// <summary>
        /// Gets data for generaldescription header
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
                () => new ServiceResultWrap()
                {
                    Data = model.Id.HasValue ? generalDescriptionService.GetValidatedEntity(model) : null,
                },
                new Dictionary<Type, string>()
                {
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(LockNotAllowedException), EntityMessageCannotBePublished }
                });
        }

        /// <summary>
        /// Publishs the general description
        /// </summary>
        /// <param name="model">model containing the general description id to be published</param>
        /// <returns>result about publishing</returns>
        [Route("PublishEntity")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishEntity([FromBody] VmPublishingModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = generalDescriptionService.PublishGeneralDescription(model),
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessagePublished },
                    { typeof(LockException), MessageLockedGeneralDescription },
                    { typeof(RoleActionException), MessagePublishGeneralDescriptionRole },
                    { typeof(PublishLanguageException), RESTCommonController.MessageNotVisibleLanguage },
                    { typeof(PublishModifiedExistsException), RESTCommonController.MessagePublishModifiedExists }
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
            () => new ServiceResultWrap()
            {
                Data = generalDescriptionService.SaveRelations(model)
            },
            new Dictionary<Type, string>()
                {
                    { typeof(string), MessageSaveGeneralDescription }
                });
        }
    }
}