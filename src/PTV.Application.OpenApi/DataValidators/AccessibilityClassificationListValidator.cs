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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for accessibility classification list.
    /// </summary>
    public class AccessibilityClassificationListValidator : BaseValidator<IList<VmOpenApiAccessibilityClassification>>
    {
        private IList<string> availableLanguages;
        private int openApiVersion;

        /// <summary>
        ///  Ctor - accessibility classification list validator.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="availableLanguages">Available languages</param>
        /// <param name="openApiVersion">The open api version</param>
        /// <param name="propertyName"></param>
        public AccessibilityClassificationListValidator(
            IList<VmOpenApiAccessibilityClassification> model,
            IList<string> availableLanguages,
            int openApiVersion,
            string propertyName = "AccessibilityClassification") : base(model, propertyName)
        {
            this.availableLanguages = availableLanguages;
            this.openApiVersion = openApiVersion;
        }

        /// <summary>
        /// Validates accessibility classification list.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Check the model for all available languages - model can be empty, but if any items exist in model, all the languages has to be included (required field)
            if (Model?.Count > 0)
            {
                var includedLanguages = Model.Select(l => l.Language).ToList();
                var notSetLanguages = availableLanguages.Except(includedLanguages);
                if (notSetLanguages.Count() > 0)
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, string.Join(", ", notSetLanguages)));
                }

                // Validate WCAG level and accessibility statement according to selected accessibility classification level (SFIPTV-37)
                var i = 0;
                Model.ForEach(ac =>
                {
                    var accessibilityClassification = new AccessibilityClassificationValidator(ac, openApiVersion, $"{PropertyName}[{i++}]");
                    accessibilityClassification.Validate(modelState);
                });
            }
        }
    }
}
