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

using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services.Security;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to notification page
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/notifications")]
    public class RESTNotificationController : RESTBaseController
    {
        private readonly IServiceManager serviceManager;
        private readonly INotificationService notificationService;
        private readonly IUserOrganizationService userOrganizationService;

        /// <summary>
        /// Constructor for notification controller
        /// </summary>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="notificationService">service responsible for notification operation</param>
        /// <param name="userOrganizationService">service responsible for user organizations</param>
        public RESTNotificationController(
            ILogger<RESTTasksController> logger,
            IServiceManager serviceManager,
            INotificationService notificationService,
            IUserOrganizationService userOrganizationService
        ) : base(
            logger
        )
        {
            this.serviceManager = serviceManager;
            this.notificationService = notificationService;
            this.userOrganizationService = userOrganizationService;
        }

        /// <summary>
        /// Gets numbers of tasks (server side)
        /// </summary>
        /// <returns>tasks numbers</returns>
        [Route("NotificationsNumbers")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTasksNumbers()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = notificationService.GetNotificationsNumbers(CurrentUserOrganizations()) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets service channel in common use
        /// </summary>
        /// <returns></returns>
        [Route("ServiceChannelInCommonUse")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap ServiceChannelInCommonUse([FromBody] VmNotificationSearch search)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {  Data = notificationService.GetChannelInCommonUse(search, CurrentUserOrganizations()) },
                new Dictionary<Type, string>()
            );
        }

        /// <summary>
        /// Gets changed content
        /// </summary>
        /// <returns></returns>
        [Route("ContentUpdated")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap ContentChanged([FromBody] VmNotificationSearch search)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {  Data = notificationService.GetContentChanged(search, CurrentUserOrganizations()) },
                new Dictionary<Type, string>()
            );
        }

        /// <summary>
        /// Gets changed General descriptions
        /// </summary>
        /// <returns></returns>
        [Route("GeneralDescriptionUpdated")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GeneralDescriptionChanged([FromBody] VmNotificationSearch search)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {  Data = notificationService.GetGeneralDescriptionChanged(search) },
                new Dictionary<Type, string>()
            );
        }

        /// <summary>
        /// Gets added General descriptions
        /// </summary>
        /// <returns></returns>
        [Route("GeneralDescriptionCreated")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GeneralDescriptionAdded([FromBody] VmNotificationSearch search)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {  Data = notificationService.GetGeneralDescriptionAdded(search) },
                new Dictionary<Type, string>()
            );
        }

        /// <summary>
        /// Gets connection changes
        /// </summary>
        /// <returns></returns>
        [Route("ConnectionChanges")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap ConnectionChanges([FromBody] VmNotificationSearch search)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {  Data = notificationService.GetConnectionChanges(search, CurrentUserOrganizations()) },
                new Dictionary<Type, string>()
            );
        }

        private List<Guid> CurrentUserOrganizations()
        {
            return userOrganizationService.GetAllUserOrganizationIds().ToList();
        }
    }
}
