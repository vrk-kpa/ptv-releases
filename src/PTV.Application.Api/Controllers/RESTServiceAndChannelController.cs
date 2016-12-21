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
using Microsoft.Extensions.Logging;
using PTV.Domain.Model;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework.ServiceManager;
using PTV.Framework.Interfaces;

namespace PTV.Application.Api.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/serviceAndChannel")]
    [Controller]
    public class RESTServiceAndChannelController : RESTBaseController
    {
        private IServiceAndChannelService serviceAndChannelService;
        private IServiceService serviceService;
        private IChannelService channelService;
        private IServiceManager serviceManager;

        private const string messageAddServiceAndChannel = "ServiceAndChannel.AddServiceAndChannel.MessageSaved";
        private const string messagePublishServiceAndChannel = "ServiceAndChannel.PublishServiceAndChannel.MessageSaved";

        public RESTServiceAndChannelController(IServiceAndChannelService serviceAndChannelService, IServiceService serviceService, IChannelService channelService, IServiceManager serviceManager, ILogger<RESTServiceController> logger) : base(logger)
        {
            this.serviceAndChannelService = serviceAndChannelService;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.serviceManager = serviceManager;
        }

        [Route("SaveRelations")]
        [HttpPost]
        public IServiceResultWrap SaveRelations([FromBody] VmRelations model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = serviceAndChannelService.SaveServiceAndChannels(model),
                },
                new Dictionary<Type, string>() { { typeof(string), messageAddServiceAndChannel } });
        }

        [Route("PublishRelations")]
        [HttpPost]
        public IServiceResultWrap PublishRelations([FromBody] VmPublishServiceAndChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap()
                {
                    Data = new VmPublishServiceAndChannelResult()
                    {
                        Services = serviceService.PublishServices(model.Services),
                        Channels = channelService.PublishChannels(model.Channels)
                    }
                },
                new Dictionary<Type, string>() { { typeof(string), messagePublishServiceAndChannel } });
        }
    }
}
