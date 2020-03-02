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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for validation duplicity of service names.
    /// </summary>
    public class ServiceNamesDuplicityValidator : BaseValidator<IList<VmOpenApiLocalizedListItem>>
    {
        private readonly ICommonService commonService;
        private readonly Guid? mainOrganizationId;
        private readonly Guid? serviceId;

        /// <summary>
        /// duplicity of service names.
        /// </summary>
        /// <param name="model">organization id list</param>
        /// <param name="commonService">Common service</param>
        /// <param name="mainOrganizationId"></param>
        /// <param name="serviceId"></param>
        /// <param name="propertyName"></param>
        public ServiceNamesDuplicityValidator(IList<VmOpenApiLocalizedListItem> model, ICommonService commonService, Guid? mainOrganizationId, Guid? serviceId = null, string propertyName = "ServiceNames") : base(model, propertyName)
        {
            this.commonService = commonService;
            this.mainOrganizationId = mainOrganizationId;
            this.serviceId = serviceId;
        }

        /// <summary>
        /// Checks if exist duplicity of service names.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (mainOrganizationId.HasValue)
            {
                Model?.ForEach(name =>
                {
                    if (commonService.CheckExistsServiceNameWithinOrganization(name.Value, mainOrganizationId.Value, serviceId))
                    {
                        modelState.AddModelError(PropertyName,string.Format(CoreMessages.OpenApi.DuplicateNamesNotAllowed, name.Value, name.Language));
                    }
                });
            }
        }
    }
}
