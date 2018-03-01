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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.DataValidators;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Organization base controller
    /// </summary>
    /// <seealso cref="PTV.Application.OpenApi.Controllers.BaseController" />
    public class OrganizationBaseController : BaseController
    {
        private int pageSize;
        private IOrganizationService organizationService;
        private ICodeService codeService;
        private ICommonService commonService;
        private IUserOrganizationService userService;

        /// <summary>
        /// The open api version number.
        /// </summary>
        protected int versionNumber;

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        protected int PageSize { get { return pageSize; } private set { } }

        /// <summary>
        /// Organization service
        /// </summary>
        protected IOrganizationService OrganizationService { get { return organizationService; } private set { } }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationBaseController"/> class.
        /// </summary>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="codeService">The code service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="commonService">The Common service.</param>
        /// <param name="userService">The user info service.</param>
        public OrganizationBaseController(
            IOrganizationService organizationService,
            ICodeService codeService,
            IOptions<AppSettings> settings,
            ILogger logger,
            ICommonService commonService,
            IUserOrganizationService userService,
            int versionNumber)
            : base(userService, settings, logger)
        {
            pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            this.versionNumber = versionNumber;
            this.organizationService = organizationService;
            this.codeService = codeService;
            this.commonService = commonService;
            this.userService = userService;
        }

        /// <summary>
        /// Get organization by id base.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected IActionResult GetById(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();
            var org = organizationService.GetOrganizationById(guid, versionNumber);

            if (org == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Get organization by id list base.
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        protected virtual IActionResult GetByIdListBase(string guids)
        {
            if (string.IsNullOrEmpty(guids))
            {
                ModelState.AddModelError("guids", "Property guids is required.");
                return new BadRequestObjectResult(ModelState);
            }
            List<Guid> guidList = new List<Guid>();
            var list = guids.Split(',');
            try
            {
                list.ForEach(id => guidList.Add(id.Trim().ParseToGuidWithExeption()));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("guids", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }

            var scs = organizationService.GetOrganizations(guidList, versionNumber);

            if (scs == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organizations not found." });
            }

            return Ok(scs);
        }

        /// <summary>
        /// Get organization by id base.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected IActionResult GetSahaById(string id)
        {
            Guid guid = id.ParseToGuidWithExeption();
            var org = organizationService.GetOrganizationSahaById(guid);

            if (org == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Gets the by y (bussines) code base.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>organization</returns>
        protected IActionResult GetByYCodeBase(string code)
        {
            // GET api/organization/businesscode/1234567-8

            var org = organizationService.GetOrganizationsByBusinessCode(code, versionNumber);

            if (org == null || org.Count == 0)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organizations with code '{code}' not found." });
            }

            return Ok(org);
        }
        /// <summary>
        /// Gets the organization by oid base.
        /// </summary>
        /// <param name="oid">The oid.</param>
        /// <returns>organization</returns>
        protected IActionResult GetByOidBase(string oid)
        {
            var org = organizationService.GetOrganizationByOid(oid, versionNumber);

            if (org == null)
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with OID '{oid}' not found." });
            }

            return Ok(org);
        }

        /// <summary>
        /// Gets a list of organizations related to defined municipality.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected IActionResult GetGuidPageByMunicipality(string code, [FromQuery]DateTime? date, [FromQuery]int page)
        {
            // check if municipality with given code exists
            var municipality = codeService.GetMunicipalityByCode(code, true);
            if (municipality == null || !municipality.Id.IsAssigned())
            {
                return NotFound(new VmError() { ErrorMessage = $"Municipality with code '{code}' not found." });
            }
            return Ok(organizationService.GetOrganizationsByMunicipality(municipality.Id, date, page, pageSize));
        }

        /// <summary>
        /// Save organization.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult Post(IVmOpenApiOrganizationInVersionBase request)
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            // Get the base model for organization
            request = request.VersionBase();

            // Check the item values from db and validate
            ValidateParameters(request, true);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var result = organizationService.AddOrganization(request, Settings.AllowAnonymous, versionNumber);
            return Ok(result);
        }

        /// <summary>
        /// Change the organization.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <returns></returns>
        protected IActionResult Put(IVmOpenApiOrganizationInVersionBase request, string id = null, string sourceId = null)
        {
            // check that one of identifiers is defined
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(sourceId))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            // check that the resource exists that we are about to update - id is used, not source id
            if (!string.IsNullOrEmpty(id) && !CheckOrganizationExists(id))
            {
                return NotFound(new VmError() { ErrorMessage = $"Organization with id '{id}' not found." });
            }

            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }
            
            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            // Get the base model for organization
            request = request.VersionBase();

            if (!string.IsNullOrEmpty(id)) { request.Id = id.ParseToGuid(); }
            if (!string.IsNullOrEmpty(sourceId)) { request.SourceId = sourceId; }

            // Validate item values from db and validate
            var validationResult = ValidateRequest(request);
            return validationResult ?? Ok(organizationService.SaveOrganization(request, Settings.AllowAnonymous, versionNumber));
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="createOperation">if set to <c>true</c> [create operation].</param>
        protected void ValidateParameters(IVmOpenApiOrganizationInVersionBase request, bool createOperation = false)
        {
            IList<string> newLanguages = new List<string>();
            IList<string> availableLanguages = new List<string>();

            if (createOperation)
            {
                if (request.PublishingStatus != PublishingStatus.Published.ToString())
                {
                    request.PublishingStatus = PublishingStatus.Draft.ToString();
                }
                newLanguages = request.GetAvailableLanguages(versionNumber);
                if (request.ParentOrganizationId.IsNullOrEmpty())
                {
                    // Check the user role - Pete user is not allowed to create main organization
                    var userRole = userService.UserHighestRole();
                    if (userRole != UserRoleEnum.Eeva)
                    {
                        this.ModelState.AddModelError("Organization", "User has no rights to create this entity!");
                    }
                }
                
            }
            else
            {
                // We are updating existing service.
                // Get the current version and data related to it
                var currentVersion = request.Id.IsAssigned() ? organizationService.GetOrganizationById(request.Id.Value, 0, false) : organizationService.GetOrganizationBySource(request.SourceId, 0, false);
                if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
                {
                    if (request.Id.IsAssigned())
                    {
                        this.ModelState.AddModelError("Organization id", $"Version for organization with id '{request.Id.Value}' not found.");
                    }
                    else
                    {
                        this.ModelState.AddModelError("Organization id", $"Version for service with source id '{request.SourceId}' not found.");
                    }
                }
                else
                {
                    request.CurrentPublishingStatus = currentVersion.PublishingStatus;
                    // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
                    availableLanguages = currentVersion.GetAvailableLanguages(versionNumber).ToList();
                    newLanguages = request.GetAvailableLanguages(versionNumber).Where(i => !availableLanguages.Contains(i)).ToList();
                }
            }
            OrganizationValidator organization = new OrganizationValidator(request, codeService, organizationService, newLanguages, availableLanguages, commonService, versionNumber, createOperation);
            organization.Validate(this.ModelState);
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected IActionResult ValidateRequest<TModel>(TModel request) where TModel : IVmOpenApiOrganizationInVersionBase
        {
            ValidateParameters(request);

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return null;
        }


        /// <summary>
        /// Checks if an organization with given id exists in the database.
        /// </summary>
        /// <param name="organizationId">organization guid as string</param>
        /// <returns>true if the organization exists otherwise false</returns>
        protected bool CheckOrganizationExists(string organizationId)
        {
            if (string.IsNullOrWhiteSpace(organizationId))
            {
                return false;
            }

            Guid? orgId = organizationId.ParseToGuid();

            if (!orgId.IsAssigned())
            {
                return false;
            }

            return commonService.OrganizationExists(orgId.Value);
        }
    }
}
