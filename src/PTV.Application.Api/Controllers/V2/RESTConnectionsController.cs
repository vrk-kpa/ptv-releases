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

using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Application.Api.Controllers.V2
{
    /// <summary>
    /// REST controller for actions related to all "Connections"
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/connections")]
    public class RESTConnectionsController : RESTBaseController
    {
        private readonly IServiceManager serviceManager;
        private readonly IConnectionsService connectionsService;

        /// <summary>
        /// Constructor of connections controller
        /// </summary>
        /// <param name="connectionsService">connectionsService service responsible for operation related to connections - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTConnectionsController(IConnectionsService connectionsService, IServiceManager serviceManager, ILogger<RESTConnectionsController> logger) : base((ILogger)logger)
        {
            this.connectionsService = connectionsService;
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
        public IServiceResultWrap SaveRelations([FromBody] VmConnectionsPageInput model)
        {
            return serviceManager.CallService(
            () => new ServiceResultWrap()
            {
                Data = connectionsService.SaveRelations(model)
            },
            new Dictionary<Type, string>()
                {
                    { typeof(string), EntityMessageSaved }
                });
        }
    }
}