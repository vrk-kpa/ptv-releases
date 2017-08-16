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
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Common" stuff
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/common")]
    [Controller]
    public class RESTCommonController : RESTBaseController
    {
        private readonly IAddressService addressService;
        private readonly IServiceManager serviceManager;
        private readonly ILanguageService languageService;
        private readonly ICommonService commonService;
        private readonly IFintoService fintoService;
        private readonly IDialCodeService dialCodeService;
        private readonly ICountryService countryService;

        private const string MessageAnnotationSuccess = "Service.Annotations.MessageSucceeded";

        /// <summary>
        /// Message ui used for informing user about existing modified version of enitity
        /// </summary>
        public const string MessageUnableEditLocked = "Common.EntityException.UnableEdit";

        /// <summary>
        /// Message ui used for informing user about requirement that at least one language version must be visible
        /// </summary>
        public const string MessageNotVisibleLanguage = "Common.EntityException.NotVisibleLanguage";

        /// <summary>
        /// Message ui used for informing user about not posibility to publish entity to Modified because another Modified already exists
        /// </summary>
        public const string MessagePublishModifiedExists = "Common.EntityException.PublishModifiedExists";

        /// <summary>
        /// Message ui used for informing user about not posibility to publish entity to Modified because another Modified already exists
        /// </summary>
        public const string MessageWithdrawModifiedExists = "Common.EntityException.WithdrawModifiedExists";

        /// <summary>
        /// Message ui used for informing user about not posibility to publish entity to Modified because another Modified already exists
        /// </summary>
        public const string MessageRestoreModifiedExists = "Common.EntityException.RestoreModifiedExists";

        /// <summary>
        /// Constructor of Common controller
        /// </summary>
        /// <param name="logger">logger commponent to support logging - injected by framework</param>
        /// <param name="addressService">address service responsible for operation related to addresses - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="languageService">language service responsible for operation related to languages - injected by framework</param>
        /// <param name="commonService">common service responsible for operation related to common stuff - injected by framework</param>
        /// <param name="dialCodeService">dial code service responsible for operation related to dial codes - injected by framework</param>
        /// <param name="fintoService">finto service responsible for operation related to finto stuff - injected by framework</param>
        /// <param name="countryService">country service responsible for operation related to countries - injected by framework</param>
        public RESTCommonController(
            ILogger<RESTCommonController> logger,
            IAddressService addressService,
            IServiceManager serviceManager,
            ILanguageService languageService,
            ICommonService commonService,
            IDialCodeService dialCodeService,
            IFintoService fintoService,
            ICountryService countryService) : base(logger)
        {
            this.addressService = addressService;
            this.serviceManager = serviceManager;
            this.languageService = languageService;
            this.commonService = commonService;
            this.fintoService = fintoService;
            this.dialCodeService = dialCodeService;
            this.countryService = countryService;
        }

        /// <summary>
        /// Get data for front page search form
        /// </summary>
        /// <returns>data needed for search form like publishing statuses, types etc.</returns>
        [Route("GetFrontPageSearch")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetFrontPageSearch()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = commonService.GetFrontPageSearch() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the address for given model
        /// </summary>
        /// <param name="model">input parameters to indicate corect address</param>
        /// <returns>address for given model</returns>
        [Route("GetAddress")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddresss([FromBody] VmGetCoordinatesForAddressIn model)
        {
            return serviceManager.CallService(
               () => new ServiceLocalizedResultWrap(model) { Data = addressService.GetAddress(model) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the coordinates for address in given model
        /// </summary>
        /// <param name="model">input parameters to indicate corect address</param>
        /// <returns>coordinates for given model</returns>
        [Route("GetCoordinatesForAddress")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetCoordinatesForAddress([FromBody] VmGetCoordinatesForAddressIn model)
        {
            return serviceManager.CallService(
               () => new ServiceLocalizedResultWrap(model) { Data = addressService.GetAddressWithCoordinates(model) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the all languages, that can be used for storage of content
        /// </summary>
        /// <returns>languages available for content</returns>
        [Route("GetTranslationLanguages")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTranslationLanguages()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = languageService.GetTranslationLanguages() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets all available publishing statues within the appilaction
        /// </summary>
        /// <returns>publishing statuses</returns>
        [Route("GetPublishingStatuses")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetPublishingStatuses()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = commonService.GetPublishingStatuses() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets all supported dial codes used through the applciation ()
        /// </summary>
        /// <param name="searchedCode"></param>
        /// <returns>dial codes </returns>
        [Route("GetDialCodes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetDialCodes([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = dialCodeService.GetDialCodes(searchedCode) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets all supported countries used through the applciation ()
        /// </summary>
        /// <param name="searchedCode"></param>
        /// <returns>countries</returns>
        [Route("GetCountries")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetCountries([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = countryService.GetCountries(searchedCode) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets Ontology term from anotation service for given model
        /// </summary>
        /// <param name="model">ServiceInfo model</param>
        /// <returns>ontology terms returned from annotation tool</returns>
        [Route("GetAnnotations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAnnotations([FromBody] ServiceInfo model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = fintoService.GetAnnotations(model) },
               new Dictionary<Type, string>()
               {
                    { typeof(string), MessageAnnotationSuccess }
               });
        }

        /// <summary>
        /// Gets parent hierarchy for ontology term
        /// </summary>
        /// <param name="model">VmGetFilteredTree model</param>
        /// <returns>ontology term hierarchy from annotation tool</returns>
        [Route("GetAnnotationHierarchy")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAnnotationHierarchy([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() {Data = fintoService.GetAnnotationHierarchy(model)},
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets parent hierarchy for ontology term
        /// </summary>
        /// <param name="model">VmGetFilteredTree model</param>
        /// <returns>ontology term hierarchy from annotation tool</returns>
        [Route("GetTypedData")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTypedData([FromBody]  VmStringList model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = commonService.GetTypedData(model.Strings) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Save Environment Instructions
        /// </summary>
        /// <param name="model">VmEnvironmentInstructionsIn model</param>
        [Route("SaveEnvironmentInstructions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveEnvironmentInstructions([FromBody]  VmEnvironmentInstructionsIn model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = commonService.SaveEnvironmentInstructions(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get Environment Instructions
        /// </summary>
        /// <return> environemnt instructions</return>
        [Route("GetEnvironmentInstructions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetEnvironmentInstructions()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = commonService.GetEnvironmentInstructions() },
                new Dictionary<Type, string>());
        }
    }
}
