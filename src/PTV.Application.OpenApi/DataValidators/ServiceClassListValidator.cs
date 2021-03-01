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

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service class list.
    /// </summary>
    public class ServiceClassListValidator : BaseValidator<IList<string>>
    {
        private readonly IFintoService fintoService;
        private readonly int validCount;
        private readonly bool checkMainClasses;

        /// <summary>
        /// Ctor - service class list validator.
        /// </summary>
        /// <param name="model">Service class list</param>
        /// <param name="fintoService">Finto item service</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="validCount">Indicates what is the valid number of items within the list. If set to zero item count is ignored.</param>
        /// <param name="checkMainClasses">Indicated whether the main classes should be checked or not.</param>
        public ServiceClassListValidator(IList<string> model,
            IFintoService fintoService,
            string propertyName = "ServiceClasses",
            int validCount = 0,
            bool checkMainClasses = false)
            : base(model, propertyName)
        {
            this.fintoService = fintoService;
            this.validCount = validCount;
            this.checkMainClasses = checkMainClasses;
        }

        /// <summary>
        /// Amount of service classes that are attached with related general description.
        /// </summary>
        public int GeneralDescriptionServiceClassCount { get; set; }

        /// <summary>
        /// Checks if service class list is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            // Validate the item count (SFIPTV-39)
            if (validCount > 0)
            {
                var amount = Model?.Count;
                if (GeneralDescriptionServiceClassCount > 0)
                {
                    amount += GeneralDescriptionServiceClassCount;
                }
                if (amount > validCount)
                {
                    modelState.AddModelError($"{PropertyName}", $"Only {validCount} items allowed!");
                    if (GeneralDescriptionServiceClassCount > 0)
                    {
                        modelState.AddModelError($"{PropertyName}", $"Attached general description already includes {GeneralDescriptionServiceClassCount} service classes!");
                    }
                }
            }

            var result = fintoService.CheckServiceClasses(Model.ToList());
            var notExistingUris = result.Item1;
            if (notExistingUris?.Count > 0)
            {
                modelState.AddModelError(PropertyName, $"Some of the uris were not found: '{string.Join(", ", notExistingUris)}'");
            }
            // Check if all the service classes were main service classes (SFIPTV-39)
            // Check only if 'checkMainClasses' is set to true. (SFIPTV-53)
            if (checkMainClasses)
            {
                var mainServiceClasses = result.Item2;
                if (mainServiceClasses != null && !Model.Except(mainServiceClasses).Any())
                {
                    modelState.AddModelError(PropertyName, "All the service classes are main service classes. Not allowed!");
                }
            }
        }
    }
}
