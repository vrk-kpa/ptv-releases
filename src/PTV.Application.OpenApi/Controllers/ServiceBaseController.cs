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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    public class ServiceBaseController : ValidationBaseController
    {
        private IServiceService serviceService;
        private IGeneralDescriptionService generalDescriptionService;

        /// <summary>
        /// Gets the OrganizationService instance.
        /// </summary>
        protected IServiceService ServiceService { get { return serviceService; } private set { } }

        public ServiceBaseController(
            IServiceService serviceService,
            IOrganizationService organizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            IGeneralDescriptionService generalDescriptionService,
            IFintoService fintoService)
            : base(organizationService, codeService, settings, fintoService)
        {
            this.serviceService = serviceService;
            this.generalDescriptionService = generalDescriptionService;
        }

        public virtual IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            var pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            return Ok(ServiceService.GetServiceIds(date, page, pageSize));
        }
        
        protected void ValidateParameters(IV2VmOpenApiServiceInBase request)
        {
            var i = 0;

            // Validate general description
            if (request.StatutoryServiceGeneralDescriptionId != null && !generalDescriptionService.GeneralDescriptionExists(Guid.Parse(request.StatutoryServiceGeneralDescriptionId)))
            {
                this.ModelState.AddModelError("StatutoryServiceGeneralDescriptionId", CoreMessages.OpenApi.RecordNotFound);
            }

            // Validate municipality codes
            if (request.Municipalities != null)
            {
                foreach (var code in request.Municipalities)
                {
                    ValidateMunicipalityCode(code, string.Format("Municipalities[{0}]", i++));
                }
            }

            // Validate languages
            ValidateLanguageList(request.Languages);

            // Validate Finto items
            ValidateServiceClasses(request.ServiceClasses);
            ValidateOntologyTerms(request.OntologyTerms);
            ValidateTargetGroups(request.TargetGroups);
            ValidateLifeEvents(request.LifeEvents);
            ValidateIndustrialClasses(request.IndustrialClasses);

            // Validate organizations
            i = 0;
            request.ServiceOrganizations.ForEach(o => ValidateOrganization(o.OrganizationId, string.Format("ServiceOrganizations[{0}].OrganizationId", i++)));
        }
    }
}
