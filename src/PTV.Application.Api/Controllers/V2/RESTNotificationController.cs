using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using System.Threading.Tasks;

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

        /// <summary>
        /// Constructor for notification controller
        /// </summary>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="notificationService">service responsible for notification operation</param>
        public RESTNotificationController(
            ILogger<RESTTasksController> logger,
            IServiceManager serviceManager,
            INotificationService notificationService
        ) : base(
            logger
        )
        {
            this.serviceManager = serviceManager;
            this.notificationService = notificationService;
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
                () => new ServiceResultWrap() { Data = notificationService.GetNotificationsNumbers() },
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
                () => new ServiceResultWrap() {  Data = notificationService.GetChannelInCommonUse(search) },
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
                () => new ServiceResultWrap() {  Data = notificationService.GetContentChanged(search) },
                new Dictionary<Type, string>()
            );
        }
        
        /// <summary>
        /// Gets archived content
        /// </summary>
        /// <returns></returns>
        [Route("ContentArchived")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap ContentArchived([FromBody] VmNotificationSearch search)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() {  Data = notificationService.GetContentArchived(search) },
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
                () => new ServiceResultWrap() {  Data = notificationService.GetGeneralDescriptionChanged(search) },
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
                () => new ServiceResultWrap() {  Data = notificationService.GetGeneralDescriptionAdded(search) },
                new Dictionary<Type, string>()
            );
        }
        
        /// <summary>
        /// Gets arrived translations content
        /// </summary>
        /// <returns></returns>
        [Route("TranslationArrived")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap TranslationArrived([FromBody] VmNotificationSearch search)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() {  Data = notificationService.GetTranslationArrived(search) },
                new Dictionary<Type, string>()
            );
        }
    }
}
