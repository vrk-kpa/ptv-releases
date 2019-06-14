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
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service id.
    /// </summary>
    public class ServiceIdValidator : BaseValidator<Guid>
    {
        private IServiceService serviceService;

        /// <summary>
        /// Ctor - service id validator.
        /// </summary>
        /// <param name="model">Service id</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="userOrganizationId">Current user own organization id</param>
        /// <param name="propertyName">Property name</param>
        public ServiceIdValidator(Guid model, IServiceService serviceService, string propertyName = "ServiceId") : base(model, propertyName)
        {
            this.serviceService = serviceService;
        }

        /// <summary>
        /// Validates channel id.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            var service = serviceService.GetServiceByIdSimple(Model);
            if (service == null || !service.Id.IsAssigned())
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
                return;
            }
        }
    }
}
