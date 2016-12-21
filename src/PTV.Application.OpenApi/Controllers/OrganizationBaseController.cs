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
using PTV.Framework;
using System;
using System.Net;

namespace PTV.Application.OpenApi.Controllers
{
    public class OrganizationBaseController : ValidationBaseController
    {
        public OrganizationBaseController(IOrganizationService organizationService, ICodeService codeService, IOptions<AppSettings> settings, IFintoService fintoService) :
            base(organizationService, codeService, settings, fintoService)
        { }

        protected IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            var pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            return Ok(OrganizationService.GetOrganizationIds(date, page, pageSize));
        }

        protected void ValidateParameters(IV2VmOpenApiOrganizationInBase request, bool createOperation = false)
        {
            ValidateMunicipalityCode(request.Municipality, "Municipality");
            ValidateAddressList(request.Addresses);
            ValidateOrganization(request.ParentOrganizationId, "ParentOrganizationId");
            if (createOperation)
            {
                ValidateOid(request.Oid);
            }
            else if (request.Id.HasValue && request.Id != Guid.Empty)
            {
                ValidateOid(request.Oid, request.Id.Value);
            }
            else if (!string.IsNullOrEmpty(request.SourceId))
            {
                ValidateOid(request.Oid, request.SourceId);
            }
        }

        protected IActionResult ValidateRequest<TModel>(TModel request) where TModel : IV2VmOpenApiOrganizationInBase
        {
            if (request == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                // TODO : this should be changed to model error collection! We cannot return two different types per bad request (we can not document this).
                return new ObjectResult(new VmError() { ErrorMessage = CoreMessages.OpenApi.RequestIsNull });
            }

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

            if (!orgId.HasValue)
            {
                return false;
            }

            return OrganizationService.OrganizationExists(orgId.Value);
        }
    }
}
