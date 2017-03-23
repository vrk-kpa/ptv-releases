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
using PTV.Domain.Model.Models.Localization;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Localization" stuff
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/localization")]
    public class RESTLocalizationController : RESTBaseController
    {
        private readonly ITypeDataService typeDataService;
        private readonly IServiceManager serviceManager;

        /// <summary>
        /// Constructor of localization controller
        /// </summary>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="typeDataService">type data service responsible for operation related to types - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTLocalizationController(IServiceManager serviceManager, ITypeDataService typeDataService, ILogger<RESTLocalizationController> logger) : base(logger)
        {
            this.serviceManager = serviceManager;
            this.typeDataService = typeDataService;
        }

        /// <summary>
        /// Gets translations of all types
        /// </summary>
        /// <returns>transaltions of types</returns>
        [Route("GetTypeData")]
        [HttpGet]
        public IServiceResultWrap GetTypeData()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = typeDataService.GetTranslatedData() },
                new Dictionary<Type, string>());
        }
    }
}
