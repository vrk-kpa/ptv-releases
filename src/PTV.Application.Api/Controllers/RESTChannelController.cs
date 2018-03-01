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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PTV.Database.DataAccess.Interfaces.Services;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Enums;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Channels"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/channel")]
    public class RESTChannelController : RESTBaseController
    {
        private readonly IChannelService channelService;
        private readonly IServiceManager serviceManager;

        /// <summary>
        /// Constructor of channel controller
        /// </summary>
        /// <param name="channelService">channel service responsible for operation related to channels - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTChannelController(IChannelService channelService, IServiceManager serviceManager, ILogger<RESTChannelController> logger) : base(logger)
        {
            this.channelService = channelService;
            this.serviceManager = serviceManager;
        }


        /// <summary>
        /// Search for channel by criterias provided as model
        /// </summary>
        /// <param name="model">Model containing search criterias</param>
        /// <returns>List of channels</returns>
        [Route("ChannelSearchResult")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap ChannelSearchResult([FromBody] VmChannelSearchParams model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SearchChannelResult(model) },
                new Dictionary<Type, string>());
        }
    }
}
