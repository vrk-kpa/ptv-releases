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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to tasks page
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/tasks")]
    public class RESTTasksController : RESTBaseController
    {
        private readonly IServiceManager serviceManager;
        private readonly ITasksService tasksService;
        private const string MessageDraftServicesPostponed = "Tasks.Draft.Services.Postponed.Message";
        private const string MessagePublishedServicesPostponed = "Tasks.Published.Services.Postponed.Message";
        private const string MessageDraftChannelsPostponed = "Tasks.Draft.Channels.Postponed.Message";
        private const string MessagePublishedChannelsPostponed = "Tasks.Published.Channels.Postponed.Message";
        private const string MessagePTasksTitle = "Tasks.Postpone.Message.Title";
        /// <summary>
        /// Constructor of tasks controller
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="tasksService">service responsible for tasks operation</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        /// </summary>
        public RESTTasksController(ILogger<RESTTasksController> logger, IServiceManager serviceManager, ITasksService tasksService) : base(logger)
        {
            this.serviceManager = serviceManager;
            this.tasksService = tasksService;
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
                () => new ServiceResultWrap() { Data = tasksService.GetTasksNumbers() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets entities for tasks accordion (server side)
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("GetTasksEntities")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTasksEntities([FromBody] VmTasksSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = tasksService.GetTasksEntities(model) },
                new Dictionary<Type, string>());
        }
        
        /// <summary>
        /// Postopone entities
        /// </summary>
        /// <returns>tasks entities</returns>
        [Route("PostponeTasksEntities")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead, AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PostponeTasksEntities([FromBody] VmPostponeTasks model)
        {
            var message = string.Empty;
            switch (model.TaskType)
            {
                case TasksIdsEnum.OutdatedDraftServices:
                    message = MessageDraftServicesPostponed;
                    break;
                case TasksIdsEnum.OutdatedPublishedServices:
                    message = MessagePublishedServicesPostponed;
                    break;
                case TasksIdsEnum.OutdatedDraftChannels:
                    message = MessageDraftChannelsPostponed;
                    break;
                case TasksIdsEnum.OutdatedPublishedChannels:
                    message = MessagePublishedChannelsPostponed;
                    break;
            }

            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = tasksService.PostoponeEntities(model) },
                new Dictionary<Type, string>
                {
                }, new Message(MessagePTasksTitle, null, message));
        }
    }
}
