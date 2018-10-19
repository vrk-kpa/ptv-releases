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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using System;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization id.
    /// </summary>
    public class OrganizationIdValidator : BaseValidator<string>
    {
        private ICommonService commonService;

        /// <summary>
        /// Ctor - organization id validator.
        /// </summary>
        /// <param name="model">Organization id</param>
        /// <param name="commonService">Common service</param>
        /// <param name="propertyName">Property name</param>
        public OrganizationIdValidator(string model, ICommonService commonService, string propertyName = "OrganizationId") : base(model, propertyName)
        {
            this.commonService = commonService;
        }

        /// <summary>
        /// Checks if organization id is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(Model))
            {
                return;
            }
            
            Guid? guid = Model.ParseToGuid();
            if (!guid.IsAssigned())
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
                return;
            }

            if (!commonService.OrganizationExists(guid.Value, PublishingStatus.Published))
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
            }
        }
    }
}
