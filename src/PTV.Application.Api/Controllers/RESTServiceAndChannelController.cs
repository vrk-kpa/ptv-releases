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
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.ServiceManager;
using PTV.Framework.Interfaces;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Services and Channels connections"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/serviceAndChannel")]
    [Controller]
    public class RESTServiceAndChannelController : RESTBaseController
    {
        private readonly IServiceAndChannelService serviceAndChannelService;
        private readonly IServiceService serviceService;
        private readonly IChannelService channelService;
        private readonly IServiceManager serviceManager;

        private const string MessageAddServiceAndChannel = "ServiceAndChannel.AddServiceAndChannel.MessageSaved";
        private const string MessagePublishServiceAndChannel = "ServiceAndChannel.PublishServiceAndChannel.MessageSaved";
        private const string MessageAddServiceAndChannelRole = "ServiceAndChannel.RoleException.MessageSave";
        private const string MessagePublishServiceAndChannelRole = "ServiceAndChannel.RoleException.MessagePublish";

        /// <summary>
        /// Constructor of service and channel connections controller
        /// </summary>
        /// <param name="serviceAndChannelService">service and channel connections service responsible for operation related to service and channel connections - injected by framework</param>
        /// <param name="serviceService">service service responsible for operation related to service - injected by framework</param>
        /// <param name="channelService">channel service responsible for operation related to channel - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTServiceAndChannelController(IServiceAndChannelService serviceAndChannelService, IServiceService serviceService, IChannelService channelService, IServiceManager serviceManager, ILogger<RESTServiceController> logger) : base(logger)
        {
            this.serviceAndChannelService = serviceAndChannelService;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Saves the connection to database
        /// </summary>
        /// <param name="model">model containing information needed for save of the connection</param>
        /// <returns>error message in case of save operation error otherwise nothing</returns>
        [Route("SaveRelations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveRelations([FromBody] VmRelations model)
        {
            return serviceManager.CallService(
            () => new ServiceLocalizedResultWrap()
            {
                Data = new VmServiceAndChannelResult()
                {
                    ConnectedServices = serviceAndChannelService.SaveServiceAndChannels(model),
                }
            },
            new Dictionary<Type, string>()
                {
                    { typeof(string), MessageAddServiceAndChannel },
                    { typeof(RoleActionException), MessageAddServiceAndChannelRole }
                });
        }

        /// <summary>
        /// Publishes the created connections between services and channels
        /// </summary>
        /// <param name="model">information about services and channels to be published</param>
        /// <returns>list of published services and channels or error messages</returns>
        [Route("PublishRelations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap PublishRelations([FromBody] VmPublishServiceAndChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap()
                {
                    Data =  serviceAndChannelService.PublishRelations(model)
                },
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessagePublishServiceAndChannel },
                    { typeof(RoleActionException), MessagePublishServiceAndChannelRole }
                });
        }

        /// <summary>
        /// Gets information detail of the relations for given model
        /// </summary>
        /// <param name="model">model that contains info for needed info, like channel and service ids</param>
        /// <returns>the relation info</returns>
        [Route("GetRelationDetail")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetRelationDetail([FromBody] VmGetRelationDetail model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = serviceAndChannelService.GetRelationDetail(model) },
                new Dictionary<Type, string>());
        }
    }
}
