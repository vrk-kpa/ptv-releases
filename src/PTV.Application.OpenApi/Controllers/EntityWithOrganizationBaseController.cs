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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Attributes;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// Entity connected with organization base controller
    /// </summary>
    public class EntityWithOrganizationBaseController : BaseController
    {
        /// <summary>
        /// Gets the reference to CommonService.
        /// </summary>
        protected ICommonService CommonService { get; }

        /// <summary>
        /// Gets the reference to OrganizationService.
        /// </summary>
        protected IOrganizationService OrganizationService { get; }

        /// <summary>
        /// EntityWithOrganizationBaseController constructor.
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="commonService"></param>
        /// <param name="userOrganizationService"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public EntityWithOrganizationBaseController(
            IOrganizationService organizationService,
            ICommonService commonService,
            IUserOrganizationService userOrganizationService,
            IOptions<AppSettings> settings,
            ILogger logger
            )
            : base(userOrganizationService, settings, logger)
        {
            CommonService = commonService;
            OrganizationService = organizationService;
        }

        /// <summary>
        /// Checks the organization related parameters organizationId, code and oid.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="code"></param>
        /// <param name="oid"></param>
        /// <param name="oneParamRequired"></param>
        /// <returns>Returns the possible erroneous action result and the related organization guids.</returns>
        protected (IActionResult, IList<Guid>) CheckAndGetOrganizationIds(string organizationId, string code, string oid, bool oneParamRequired = false)
        {
            // If oneParamRequired is set to true one of the organization parameters has to be set
            if (oneParamRequired && organizationId.IsNullOrEmpty() && code.IsNullOrEmpty() && oid.IsNullOrEmpty())
            {
                ModelState.AddModelError("organizationId", "One of the organization related parameters 'organizationId', 'code' or 'oid' has to be set.");
                return (BadRequest(ModelState), null);
            }

            // Only one of the organization parameters can be set at a time.
            if ((!organizationId.IsNullOrEmpty() && !code.IsNullOrEmpty()) || (!organizationId.IsNullOrEmpty() && !oid.IsNullOrEmpty()) || (!code.IsNullOrEmpty() && !oid.IsNullOrEmpty()))
            {
                var parameterName = organizationId == null ? "code" : "organizationId";
                ModelState.AddModelError(parameterName, "Only one of the organization related parameters 'organizationId', 'code' or 'oid' can be set at a time.");
                return (BadRequest(ModelState), null);
            }

            // Check that the set organization exists
            if (!organizationId.IsNullOrEmpty())
            {
                var guid = organizationId.ParseToGuidWithExeption();

                // Check if the organization exists with the given id
                if (!CommonService.OrganizationExists(guid, PublishingStatus.Published))
                {
                    return (NotFound(new VmError { ErrorMessage = $"Organization with id '{guid}' not found." }), null);
                }

                return (null, new List<Guid> { guid });
            }

            if (!code.IsNullOrEmpty())
            {
                // Check that the business code is well formed
                if (!Regex.IsMatch(code, ValidationConsts.BusinessCode))
                {
                    ModelState.AddModelError("code", $"The field code must match the regular expression '{ValidationConsts.BusinessCode}'.");
                    return (BadRequest(ModelState), null);
                }

                var guids = OrganizationService.GetOrganizationIdsByBusinessCode(code);
                if (guids?.Count > 0)
                {
                    return (null, guids);
                }

                return (NotFound(new VmError { ErrorMessage = $"Organization with business code '{code}' not found." }), null);
            }

            if (!oid.IsNullOrEmpty())
            {
                // Check that the oid is well formed
                if (!Regex.IsMatch(oid, ValidationConsts.Oid))
                {
                    ModelState.AddModelError("oid", $"The field oid must match the regular expression '{ValidationConsts.Oid}'.");
                    return (BadRequest(ModelState), null);
                }

                var guid = OrganizationService.GetOrganizationIdByOid(oid, true);
                if (guid == Guid.Empty)
                {
                    return (NotFound(new VmError { ErrorMessage = $"Organization with oid '{oid}' not found." }), null);
                }

                return (null, new List<Guid> { guid });
            }

            return (null, null);
        }
    }
}
