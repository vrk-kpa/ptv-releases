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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "General Descriptions"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Microsoft.AspNetCore.Mvc.Route("api/generaldescription")]
    public class RESTGeneralDescriptionController : RESTBaseController
    {
        private readonly IGeneralDescriptionService generalDescriptionService;
        private readonly IServiceManager serviceManager;

        /// <summary>
        /// Constructor of General description controller
        /// </summary>
        /// <param name="generalDescriptionService">general description service responsible for operation related to general descriptions - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        public RESTGeneralDescriptionController(IGeneralDescriptionService generalDescriptionService, IServiceManager serviceManager, ILogger<RESTGeneralDescriptionController> logger) : base(logger)
        {
            this.generalDescriptionService = generalDescriptionService;
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Search for general descriptions based on search criteria
        /// </summary>
        /// <param name="searchData">Search criteria</param>
        /// <returns>List of general descriptions</returns>
        [Microsoft.AspNetCore.Mvc.Route("SearchGeneralDescriptions")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap SearchGeneralDescriptions([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionSearchForm searchData)
            {
            return serviceManager.CallService(
              () => new ServiceResultWrap() { Data = generalDescriptionService.SearchGeneralDescriptions(searchData) },
              new Dictionary<Type, string>());
        }

        /// <summary>
        /// Search for general descriptions based on search criteria
        /// </summary>
        /// <param name="searchData">Search criteria</param>
        /// <returns>List of general descriptions</returns>
        [Microsoft.AspNetCore.Mvc.Route("v2/SearchGeneralDescriptions")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap SearchGeneralDescriptions_V2([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionSearchForm searchData)
        {
            return serviceManager.CallService(
              () => new ServiceResultWrap() { Data = generalDescriptionService.SearchGeneralDescriptions_V2(searchData) },
              new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the genereal description
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>general description</returns>
        [Microsoft.AspNetCore.Mvc.Route("v2/GetGeneralDescription")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap GetGeneralDescription_V2([Microsoft.AspNetCore.Mvc.FromBody] VmGeneralDescriptionIn model)
        {
            return serviceManager.CallService(
              () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescription_V2(model) },
              new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get all sub target groups by parent group ID
        /// </summary>
        /// <param name="targetGroupId">Parent group ID</param>
        /// <returns>List of sub target groups of parent group</returns>
        [Microsoft.AspNetCore.Mvc.Route("GetSubTargetGroups")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap GetSubTargetGroups([Microsoft.AspNetCore.Mvc.FromBody] Guid targetGroupId)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = generalDescriptionService.GetSubTargetGroups(targetGroupId) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get data for search general description form, i.e. input data for criterias fields
        /// </summary>
        /// <returns>Data for search form</returns>
        [Microsoft.AspNetCore.Mvc.Route("GetGeneralDescriptionSearch")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IServiceResultWrap GetGeneralDescriptionSearch()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescriptionSearchForm() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get data for search general description form, i.e. input data for criterias fields
        /// </summary>
        /// <returns>Data for search form</returns>
        [Microsoft.AspNetCore.Mvc.Route("GetGeneralDescription")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap GetGeneralDescription([Microsoft.AspNetCore.Mvc.FromBody] Guid id)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = generalDescriptionService.GetGeneralDescriptionById(id) },
                new Dictionary<Type, string>());
        }
    }
}
