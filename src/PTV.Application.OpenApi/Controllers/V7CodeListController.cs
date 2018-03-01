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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Application.OpenApi.Attributes;
using PTV.Database.DataAccess.Interfaces.Services.Security;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api services related methods.
    /// </summary>
    [Route("api/v7/CodeList")]
    //[Route("api/v8/CodeList")]// PTV-3870 - temporarily hide version 8 for release 1.8.
    public class V7CodeListController : BaseController
    {
        private ICommonService commonService;
        private IMunicipalityService municipalityService;
        private ICountryService countryService;
        private IPostalCodeService postalCodeService;
        private ILanguageService languageService;
        private int pageSize;

        /// <summary>
        /// Get the page size defined in application settings.
        /// </summary>
        protected int PageSize { get { return pageSize; } private set { } }

        /// <summary>
        /// Initializes a new instance of the <see cref="V7CodeListController"/> class.
        /// </summary>
        /// <param name="commonService">The common service.</param>
        /// <param name="municipalityService">The municipality service.</param>
        /// <param name="countryService">The country service.</param>
        /// <param name="postalCodeService">The postal code service.</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="userOrganizationService">The user organization service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public V7CodeListController(
            ICommonService commonService,
            IMunicipalityService municipalityService,
            ICountryService countryService,
            IPostalCodeService postalCodeService,
            ILanguageService languageService,
            IUserOrganizationService userOrganizationService,
            IOptions<AppSettings> settings,
            ILogger<V7CodeListController> logger) : base(userOrganizationService, settings, logger)
        {
            this.commonService = commonService;
            this.municipalityService = municipalityService;
            this.countryService = countryService;
            this.postalCodeService = postalCodeService;
            this.languageService = languageService;

            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
        }

        /// <summary>
        /// Gets a list of municipality codes.
        /// </summary>
        /// <returns>List of municipality codes.</returns>
        [AllowAnonymous]
        [HttpGet("GetMunicipalityCodes")]
        [ProducesResponseType(typeof(IList<VmOpenApiCodeListItem>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of municipality codes.", type: typeof(IList<VmOpenApiCodeListItem>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetMunicipalityCodes()
        {
            return Ok(municipalityService.GetMunicipalitiesCodeList());
        }

        /// <summary>
        /// Gets a list of country codes.
        /// </summary>
        /// <returns>List of country codes.</returns>
        [AllowAnonymous]
        [HttpGet("GetCountryCodes")]
        [ProducesResponseType(typeof(IList<VmOpenApiDialCodeListItem>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of country codes.", type: typeof(IList<VmOpenApiDialCodeListItem>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetCountryCodes()
        {
            return Ok(countryService.GetCountryCodeList());
        }


        /// <summary>
        /// Gets a list of postal codes.
        /// </summary>
        /// <param name="page">The page to be fetched.</param>
        /// <returns>List of postal codes.</returns>
        [AllowAnonymous]
        [HttpGet("GetPostalCodes")]
        [ProducesResponseType(typeof(VmOpenApiCodeListPage), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of postal codes.", type: typeof(VmOpenApiCodeListPage))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetPostalCodes([FromQuery]int page = 1)
        {
            return Ok(postalCodeService.GetPostalCodeList(page, pageSize));
        }

        /// <summary>
        /// Gets a list of language codes.
        /// </summary>
        /// <returns>List of language codes.</returns>
        [AllowAnonymous]
        [HttpGet("GetLanguageCodes")]
        [ProducesResponseType(typeof(IList<VmOpenApiCodeListItem>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of language codes.", type: typeof(IList<VmOpenApiCodeListItem>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetLanguageCodes()
        {
            return Ok(languageService.GetLanguageCodeList());
        }

        /// <summary>
        /// Gets a list of area codes filtered by area type.
        /// </summary>
        /// <param name="type">Area type</param>
        /// <returns>List of area codes filtered by area type.</returns>
        [AllowAnonymous]
        [HttpGet("GetAreaCodes/type/{type}")]
        [ValidateAreaType]
        [ProducesResponseType(typeof(IList<VmOpenApiCodeListItem>), (int)HttpStatusCode.OK)] // required otherwise swagger generates 204 content response which isn't used
        [SwaggerResponse((int)HttpStatusCode.OK, description: "A list of area codes.", type: typeof(IList<VmOpenApiCodeListItem>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, description: CoreMessages.OpenApi.BadRequestGeneralMessage, type: typeof(Dictionary<string, IList<string>>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, description: CoreMessages.OpenApi.NotFoundGeneralMessage, type: typeof(IVmError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, description: CoreMessages.OpenApi.InternalServerErrorDescripton, type: typeof(IVmError))]
        public IActionResult GetAreaCodes(string type)
        {
            AreaTypeEnum? areaType = null;
            if (!string.IsNullOrEmpty(type)) { areaType = type.Parse<AreaTypeEnum>(); }

            return Ok(commonService.GetAreaCodeList(areaType));
        }
    }
}
