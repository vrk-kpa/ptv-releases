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
using Microsoft.IdentityModel.Tokens;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to tasks page
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/admin")]
    public class RESTAdminMngtSahaMappingController : RESTBaseController
    {
        private readonly ISahaMappingService sahaMappingService;
        private readonly IServiceManager serviceManager;
        private readonly IAdminTasksService adminTasksService;
        
        private const string MessageSaveMapping = "AdminMapping.MessageSave";
        private const string MessageRemoveMapping = "AdminMapping.MessageRemoved";
                  
        private const string MessageFetchFailedTranslationOrder = "Admin.Tasks.FailedTranslationOrder.MessageFetched";
        private const string MessageTranslationOrderCanceled = "Admin.Tasks.TranslationOrder.MessageCanceled";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceManager"></param>
        /// <param name="sahaMappingService"></param>
        /// <param name="adminTasksService"></param>
        public RESTAdminMngtSahaMappingController(ILogger<RESTAdminMngtSahaMappingController> logger, IServiceManager serviceManager, ISahaMappingService sahaMappingService, IAdminTasksService adminTasksService) : base(logger)
        {
            this.serviceManager = serviceManager;
            this.sahaMappingService = sahaMappingService;
            this.adminTasksService = adminTasksService;
        }
 
        /// <summary>
        /// Gets entities for tasks accordion (server side)
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("GetAllMappings")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAllMappings()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = sahaMappingService.GetAllMappings() },
                new Dictionary<Type, string>());
        }
        /// <summary>
        /// Gets entities for admin mapping accordion (server side)
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("GetMappings")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetMappings([FromBody] VmGuidList model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = sahaMappingService.GetMappings(model.Data) },
                new Dictionary<Type, string>());
        }
        
        /// <summary>
        /// Gets entities for tasks accordion (server side)
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("RemoveMapping")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap RemoveMapping([FromBody] SahaMappingDefinitionBase model)
        {
            return serviceManager.CallService(
                () =>
                {
                    sahaMappingService.RemoveSahaIdMapping(model);
                    return new ServiceResultWrap() {Data = null};
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageRemoveMapping }                    
                });
        }
        /// <summary>
        /// Update PTV SaHa mapping
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("UpdateMapping")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap UpdateMapping([FromBody] SahaMappingDefinition model)
        {
            return serviceManager.CallService(
                () =>
                {
                    sahaMappingService.MapSahaIdToPtvId(model);
                    return new ServiceResultWrap() {Data = null};
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageSaveMapping }                    
                });
        }
        
        
        /// <summary>
        /// Gets numbers of tasks (server side)
        /// </summary>
        /// <returns>tasks numbers</returns>
        [Route("GetTasksNumbers")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTasksNumbers()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = adminTasksService.GetTasksNumbers() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets entities for tasks accordion
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("GetTasksEntities")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTasksEntities([FromBody] VmAdminTasksSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = adminTasksService.GetTasksEntities(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Fetch all failed translation order again
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("FetchFailedTranslationOrders")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap FetchFailedTranslationOrders([FromBody] VmAdminTasksSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = adminTasksService.FetchFailedTranslationOrders(model), 
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), MessageFetchFailedTranslationOrder },
                });
        }
        
        /// <summary>
        /// Remove translation order 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CancelTranslationOrder")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap CancelTranslationOrder([FromBody] VmAdminTasksSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap()
                {
                    Data = adminTasksService.CancelTranslationOrder(model),
                },
                new Dictionary<Type, string>
                {
                    { typeof(string), MessageTranslationOrderCanceled },
                });
        }
    }
}