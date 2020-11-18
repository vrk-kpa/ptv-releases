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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service channel id.
    /// </summary>
    public class ServiceChannelConnectionTypeValidator : BaseValidator<VmOpenApiServiceLocationChannelInVersionBase>
    {
        private readonly ICommonService commonService;
        private readonly bool publishedVersion;

        /// <summary>
        /// Ctor - service channel id validator.
        /// </summary>
        /// <param name="model">Service channel id</param>
        /// <param name="commonService">Common service</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="propertyItemName"></param>
        /// <param name="publishedVersion">Indicates if published or latest version should be fetched.</param>
        public ServiceChannelConnectionTypeValidator(VmOpenApiServiceLocationChannelInVersionBase model, ICommonService commonService, string propertyName, string propertyItemName = null, bool publishedVersion = true) : base(model, propertyName)
        {
            this.commonService = commonService;
            this.publishedVersion = publishedVersion;
            if (!string.IsNullOrEmpty(propertyItemName))
            {
                PropertyName = $"{propertyName}.{propertyItemName}";
            }
        }

        /// <summary>
        /// Validates connection Typpe.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (!Model.IsVisibleForAll && Model.Id.HasValue && Guid.TryParse(Model.OrganizationId, out var orgId) && publishedVersion)
            {
                if (commonService.CheckExistsAstiConnections(Model.Id.Value, orgId))
                {
                    modelState.AddModelError(PropertyName, CoreMessages.OpenApi.ChannelNotVisibleForAllAsti);
                }
            }
        }
    }
}
