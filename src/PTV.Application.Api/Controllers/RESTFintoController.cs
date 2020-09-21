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

using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Domain.Logic;
using PTV.Framework.Enums;
using PTV.Framework.ServiceManager;
using PTV.Framework.Interfaces;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Services"
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/service")]
    [Controller]
    public class RESTFintoController : RESTBaseController
    {
        private readonly IServiceManager serviceManager;
        private readonly IFintoService fintoService;

        /// <summary>
        /// Constructor of service controller
        /// </summary>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="fintoService">finto service responsible for operation related to finto stuff - injected by framework</param>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        public RESTFintoController(
            IServiceManager serviceManager,
            IFintoService fintoService,
            ILogger<RESTFintoController> logger) : base(logger)
        {
            this.serviceManager = serviceManager;
            this.fintoService = fintoService;
        }

        /// <summary>
        /// Gets the finto terms for given term
        /// </summary>
        /// <param name="model">model containing id of the term</param>
        /// <returns>the tree of the given term</returns>
        [Route("GetFintoTree")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetFintoTree([FromBody] VmNode model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {Data = fintoService.GetFintoTree(model)},
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the finto terms for given term and search value
        /// </summary>
        /// <param name="model">model containing id of the term and search criteria</param>
        /// <returns>the tree of the given term</returns>
        [Route("GetFilteredTree")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetFilteredTree([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {Data = fintoService.GetFilteredTree(model)},
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets flatten list of finto stuff for given search value
        /// </summary>
        /// <param name="model">model containing search criteria</param>
        /// <returns>the list of finto stuff</returns>
        [Route("GetFilteredList")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetFilteredList([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {Data = fintoService.Search(model)},
                new Dictionary<Type, string>());
        }
    }
}
