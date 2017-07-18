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
    /// Validator for language item list
    /// </summary>
    public class LanguageItemListValidator : BaseValidator<IList<VmOpenApiLanguageItem>>
    {
        private IList<string> requiredLanguages;

        /// <summary>
        /// Ctor - language list validator.
        /// </summary>
        /// <param name="model">Language list model</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="requiredLanguages">The languages that should be included in list.</param>
        public LanguageItemListValidator(IList<VmOpenApiLanguageItem> model, string propertyName, IList<string> requiredLanguages = null) : base(model, propertyName)
        {
            this.requiredLanguages = requiredLanguages;
        }

        /// <summary>
        /// Checks if language item list is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (requiredLanguages == null || requiredLanguages?.Count == 0) return;

            if (Model == null || Model?.Count == 0)
            {
                modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, requiredLanguages.First()));
                return;
            }

            List<string> includedLanguages = Model.Select(i => i.Language).ToList();
            requiredLanguages.ForEach(l =>
            {
                if (!includedLanguages.Contains(l))
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, l));
                }
            });
        }
    }
}
