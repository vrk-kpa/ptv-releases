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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Application.Api.Controllers;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models;

namespace PTV.Application.Web.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/common")]
    [Controller]
    public class RESTCommonController : RESTBaseController
    {
        private IAddressService addressService;
        private IServiceManager serviceManager;
        private ILanguageService languageService;
        private ICommonService commonService;
        public RESTCommonController(ILogger<RESTCommonController> logger, IAddressService addressService, IServiceManager serviceManager, ILanguageService languageService, ICommonService commonService) : base(logger)
        {
            this.addressService = addressService;
            this.serviceManager = serviceManager;
            this.languageService = languageService;
            this.commonService = commonService;
        }

        [Microsoft.AspNetCore.Mvc.Route("GetAddress")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap GetAddresss([FromBody] VmGetCoordinate model)
        {
            return serviceManager.CallService(
               () => new ServiceLocalizedResultWrap(model) { Data = addressService.GetAddress(model) },
               new Dictionary<Type, string>());
        }

        [Microsoft.AspNetCore.Mvc.Route("GetTranslationLanguages")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap GetTranslationLanguages()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = languageService.GetTranslationLanguages() },
               new Dictionary<Type, string>());
        }

        [Microsoft.AspNetCore.Mvc.Route("GetPublishingStatuses")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IServiceResultWrap GetPublishingStatuses()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = commonService.GetPublishingStatuses() },
               new Dictionary<Type, string>());
        }
    }
}
