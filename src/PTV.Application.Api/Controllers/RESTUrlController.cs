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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to url handling
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/url")]
    public class RESTUrlController : RESTBaseController
    {
        private readonly IUrlService urlService;
        private readonly IServiceManager serviceManager;
        private readonly IBrokenLinkService brokenLinkService;

        private const string BrokenLinkMessageSaved = "BrokenLink.MessageSaved";
        private const string BrokenLinkMessageDataCorruption = "BrokenLink.DataCorruption";
        private const string BrokenLinkEmptyUrl = "BrokenLink.EmptyUrl";

        /// <summary>
        /// Constructor of url controller
        /// </summary>
        /// <param name="urlService">url service responsible for operation related to urls - injected by framework</param>
        /// <param name="brokenLinkService"></param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        public RESTUrlController(
            IUrlService urlService,
            IBrokenLinkService brokenLinkService,
            IServiceManager serviceManager,
            ILogger<RESTUrlController> logger) : base(logger)
        {
            this.urlService = urlService;
            this.serviceManager= serviceManager;
            this.brokenLinkService = brokenLinkService;
        }


        /// <summary>
        /// Checks validity of the url (server side)
        /// </summary>
        /// <param name="model">model containing url</param>
        /// <returns>result info about url</returns>
        [Route("Check")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public async Task<IServiceResultWrap> Check([FromBody] VmBrokenLink model)
        {
            return await serviceManager.CallServiceAsync(
                async () => new ServiceResultWrap { Data = await brokenLinkService.Check(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateBrokenLink")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap UpdateBrokenLink([FromBody] VmOrganizationBrokenLink model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {Data = brokenLinkService.Update(model)},
                new Dictionary<Type, string>
                {
                    { typeof(string), BrokenLinkMessageSaved },
                    { typeof(ArgumentNullException), BrokenLinkEmptyUrl },
                    { typeof(OverflowException), BrokenLinkMessageDataCorruption }
                });
        }
    }
}
