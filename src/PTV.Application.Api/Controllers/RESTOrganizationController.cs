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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Logic;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Organizations"
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/organization")]
    [Controller]
    public class RESTOrganizationController : RESTBaseController
    {
        private readonly IOrganizationService organizationService;
        private readonly IServiceManager serviceManager;

        /// <summary>
        /// Constructor of organization controller
        /// </summary>
        /// <param name="organizationService">organization service responsible for operation related to organizations - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTOrganizationController(IOrganizationService organizationService, IServiceManager serviceManager, ILogger<RESTOrganizationController> logger) : base(logger)
        {
            this.organizationService = organizationService;
            this.serviceManager = serviceManager;
        }
    }
}
