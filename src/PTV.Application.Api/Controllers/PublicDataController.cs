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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using IOrganizationService = PTV.Database.DataAccess.Interfaces.Services.V2.IOrganizationService;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Relaying controller for map calls
    /// </summary>
    [Microsoft.AspNetCore.Mvc.Route("api/publicData")]
    [AllowAnonymous]
    [Controller]
    public class PublicDataController
    {
        private ILogger<PublicDataController> logger;
        private readonly ITranslationService translationService;
        private readonly IServiceManager serviceManager;
        private readonly ICommonService commonService;
        readonly IOrganizationService organizationService;

        /// <summary>
        /// Public data controller
        /// </summary>
        /// <param name="translationService"></param>
        /// <param name="logger"></param>
        /// <param name="serviceManager"></param>
        /// <param name="commonService"></param>
        /// <param name="organizationService"></param>
        public PublicDataController(ITranslationService translationService, ILogger<PublicDataController> logger, IServiceManager serviceManager, ICommonService commonService, IOrganizationService organizationService)
        {
            this.logger = logger;
            this.translationService = translationService;
            this.commonService = commonService;
            this.organizationService = organizationService;
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Get Translation data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Route("GetTranslationData")]
        //[HttpGet]
        [HttpGet("GetTranslationData/{id}")]
        [AllowAnonymous]
        public string GetTranslationData(Guid id)
        {
            return translationService.GetTranslationDataJson(id);
        }

        /// <summary>
        /// Get all enum types
        /// </summary>
        /// <returns>enum types needed by application.</returns>
        [Route("GetEnumTypes")]
        [HttpGet]
        [AllowAnonymous]
        public IServiceResultWrap GetEnumTypes()
        {
            
            return serviceManager.CallService(
                    () => new ServiceResultWrap { Data = commonService.GetEnumTypesForLogin() },
                    new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets postal codes for given search code/municipality name
        /// </summary>
        /// <param name="query">postal code or municipality name</param>
        /// <returns>postal codes</returns>
        [Route("GetOrganizationList")]
        [HttpPost]
        public IServiceResultWrap GetOrganizationList([FromBody] VmOrganizationListSearch query)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.GetOrganizationList(query) },
                new Dictionary<Type, string>());
        }

    }
}
