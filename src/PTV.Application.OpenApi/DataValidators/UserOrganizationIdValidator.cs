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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for user organization id.
    /// </summary>
    public class UserOrganizationIdValidator : BaseValidator<string>
    {
        private readonly ICommonService commonService;
        private readonly IList<Guid> userOrganizations;

        /// <summary>
        /// Ctor - User organization id validator.
        /// </summary>
        /// <param name="model">Organization id</param>
        /// <param name="commonService">Common service</param>
        /// <param name="userOrganizations">Current user own organizations (including sub organizations).</param>
        /// <param name="propertyName">Property name</param>
        public UserOrganizationIdValidator(string model, ICommonService commonService, IList<Guid> userOrganizations, string propertyName = "OrganizationId") : base(model, propertyName)
        {
            this.commonService = commonService;
            this.userOrganizations = userOrganizations;
        }

        /// <summary>
        /// Checks if organization id is valid or not and one of user organizations.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(Model))
            {
                return;
            }

            var guid = Model.ParseToGuid();
            if (!guid.IsAssigned())
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
                return;
            }

            if (!commonService.OrganizationExists(guid.Value, PublishingStatus.Published))
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
            }

            // Check that the organization is one of user organizations
            if (userOrganizations == null || !userOrganizations.Contains(guid.Value))
            {
                modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.UserOrganizationRequired, Model));
            }
        }
    }
}
