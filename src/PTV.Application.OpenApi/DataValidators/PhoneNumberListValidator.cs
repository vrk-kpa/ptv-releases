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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for phone number list.
    /// </summary>
    public class PhoneNumberListValidator<TModel> : BaseValidator<IList<TModel>> where TModel : IVmOpenApiPhoneSimpleVersionBase
    {
        private readonly ICodeService codeService;
        private readonly IList<string> requiredLanguages;
        private readonly IList<string> availableLanguages;

        /// <summary>
        /// Ctor - phone number list validator.
        /// </summary>
        /// <param name="model">Prefix number</param>
        /// <param name="codeService">Code service</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="requiredLanguages">The languages that has just been added into main model and therefore should be included in list.</param>
        /// <param name="availableLanguages">The languages that are available in main model and therefore need to be validated.</param>
        public PhoneNumberListValidator(IList<TModel> model, ICodeService codeService, string propertyName = "PhoneNumbers", IList<string> requiredLanguages = null, IList<string> availableLanguages = null) : base(model, propertyName)
        {
            this.codeService = codeService;
            this.requiredLanguages = requiredLanguages;
            this.availableLanguages = availableLanguages;
        }

        /// <summary>
        /// Checks if phone number list is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Check the model for required languages - the model needs to include these language items (model cannot be empty!)
            if (requiredLanguages != null && requiredLanguages.Count > 0)
            {
                if (Model == null || Model.Count == 0)
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, requiredLanguages.First()));
                }
                else
                {
                    ValidateModelForLanguages(requiredLanguages, modelState);
                }
            }

            // Check the model for all available languages - model can be empty, but if any items exist in model, all the languages has to be included (required field)
            if (availableLanguages != null && availableLanguages.Count > 0)
            {
                if (Model != null && Model.Count > 0)
                {
                    ValidateModelForLanguages(availableLanguages, modelState);
                }
            }

            if (Model == null || Model.Count == 0)
                return;

            // Validate the model data
            var i = 0;
            Model.ForEach(phone =>
            {
                var prefix = new PrefixNumberValidator(phone.PrefixNumber, codeService, $"{PropertyName}[{ i++ }].PrefixNumber");
                prefix.Validate(modelState);
            });
        }

        private void ValidateModelForLanguages(IList<string> languages, ModelStateDictionary modelState)
        {
            var includedLanguages = Model.Select(l => l.Language).ToList();
            languages.ForEach(l =>
            {
                if (!includedLanguages.Contains(l))
                {
                    // Let's not add the error again if it already can be found! (SFIPTV-2089)
                    SetErrorWithinModelState(modelState, string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, l));
                }
            });
        }
    }
}
