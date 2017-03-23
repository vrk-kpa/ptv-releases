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
using PTV.Framework;
using System;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for general description id.
    /// </summary>
    public class GeneralDescriptionIdValidator : BaseValidator<string>
    {
        private IGeneralDescriptionService generalDescriptionService;

        /// <summary>
        /// Ctor - general description id validator.
        /// </summary>
        /// <param name="model">General description id</param>
        /// <param name="generalDescriptionService">General description service</param>
        /// <param name="propertyName">Property name</param>
        public GeneralDescriptionIdValidator(string model, IGeneralDescriptionService generalDescriptionService, string propertyName = "StatutoryServiceGeneralDescriptionId") : base(model, propertyName)
        {
            this.generalDescriptionService = generalDescriptionService;
        }

        /// <summary>
        /// Checks if municipality code is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(Model))
            {
                return;
            }

            if (!generalDescriptionService.GeneralDescriptionExists(Guid.Parse(Model)))
            {
                modelState.AddModelError("StatutoryServiceGeneralDescriptionId", CoreMessages.OpenApi.RecordNotFound);
            }
        }
    }
}
