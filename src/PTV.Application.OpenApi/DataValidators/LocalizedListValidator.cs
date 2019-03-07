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

using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for list containing localized list items.
    /// </summary>
    public class LocalizedListValidator : BaseValidator<IList<VmOpenApiLocalizedListItem>>
    {
        private IList<string> requiredLanguages;
        private IList<string> requiredTypes;
        IList<string> availableLanguages;

        /// <summary>
        /// Ctor - localized item list validator.
        /// </summary>
        /// <param name="model">Localized item list</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="requiredLanguages">The languages that should be included in list.</param>
        /// <param name="requiredTypes">The types that should be included in list.</param>
        /// <param name="availableLanguages"></param>
        public LocalizedListValidator(IList<VmOpenApiLocalizedListItem> model, string propertyName, IList<string> requiredLanguages = null,
            IList<string> requiredTypes = null, IList<string> availableLanguages = null) : base(model, propertyName)
        {
            this.requiredLanguages = requiredLanguages;
            this.requiredTypes = requiredTypes;
            this.availableLanguages = availableLanguages;
        }

        /// <summary>
        /// Validates localized item list.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Check the model for required languages - the model needs to include these language items (model cannot be empty!)
            if (requiredLanguages != null && requiredLanguages?.Count > 0)
            {
                if (Model == null || Model.Count == 0)
                {
                    if (requiredTypes == null || requiredTypes.Count == 0)
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, requiredLanguages.First()));
                    }
                    else
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredValueWithLanguageAndTypeNotFound, requiredTypes.First(), requiredLanguages.First()));
                    }
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
        }

        private void ValidateModelForLanguages(IList<string> languages, ModelStateDictionary modelState)
        {
            // If required types is not set check only that required lnaguages exist
            if (requiredTypes == null || requiredTypes.Count == 0)
            {
                List<string> includedLanguages = Model.Select(l => l.Language).ToList();
                languages.ForEach(l =>
                {
                    if (!includedLanguages.Contains(l))
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, l));
                    }
                });
            }
            else
            {
                // Need to check that required types with required languages exists
                requiredTypes.ForEach(type =>
                {
                    languages.ForEach(lang =>
                    {
                        if (Model.Where(i => i.Type == type && i.Language == lang).FirstOrDefault() == null)
                        {
                            modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredValueWithLanguageAndTypeNotFound, type, lang));
                        }
                    });
                });
            }
            
        }
    }
}
