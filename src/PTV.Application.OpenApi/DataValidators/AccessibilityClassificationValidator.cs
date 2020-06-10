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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for accessibility classification.
    /// </summary>
    public class AccessibilityClassificationValidator : BaseValidator<VmOpenApiAccessibilityClassification>
    {
        private readonly int openApiVersion;

        /// <summary>
        ///  Ctor - accessibility classification validator.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="openApiVersion">The open api version</param>
        /// <param name="propertyName"></param>
        public AccessibilityClassificationValidator(
            VmOpenApiAccessibilityClassification model,
            int openApiVersion,
            string propertyName = "AccessibilityClassification")
            : base(model, propertyName)
        {
            this.openApiVersion = openApiVersion;
        }

        /// <summary>
        /// Validates accessibility classification item.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null)
            {
                return;
            }

            // Allowed combinations:
            // If accessibilityClassificationLevel = FullyCompliant, wcagLevel can be AAA or AA.
            // If accessibilityClassificationLevel = PartiallyCompliant, wcagLevel can be A.
            // If accessibilityClassificationLevel = NonCompliant, wcagLevel needs to be empty or null.
            // If accessibilityClassificationLevel = Unknown, wcagLevel needs to be empty or null.
            if (!string.IsNullOrEmpty(Model.AccessibilityClassificationLevel))
            {
                if (Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.FullyCompliant.ToString() &&
                    (string.IsNullOrEmpty(Model.WcagLevel) || Model.WcagLevel == WcagLevelTypeEnum.LevelA.ToString())
                    ||
                    (Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.PartiallyCompliant.ToString() && Model.WcagLevel != WcagLevelTypeEnum.LevelA.ToString())
                    ||
                    (Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.NonCompliant.ToString() && !string.IsNullOrEmpty(Model.WcagLevel))
                    ||
                    (Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.Unknown.ToString() && !string.IsNullOrEmpty(Model.WcagLevel)))
                {
                    if (openApiVersion < 10)
                    {
                        modelState.AddModelError("WcagLevel", $"'{Model.WcagLevel}' is not allowed value of 'WcagLevel' when 'AccessibilityClassificationLevel' has value '{ Model.AccessibilityClassificationLevel }'.");
                    }
                    else
                    {
                        modelState.AddModelError($"{PropertyName}.WcagLevel", $"'{Model.WcagLevel}' is not allowed value of 'WcagLevel' when 'AccessibilityClassificationLevel' has value '{ Model.AccessibilityClassificationLevel }'.");
                    }
                }

                // The accessibility statement web page is required if accessibilityClassificationLevel = FullyCompliant, PartiallyCompliant or NonCompliant. SFIPTV-932
                if ((Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.FullyCompliant.ToString() ||
                    Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.PartiallyCompliant.ToString() ||
                    Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.NonCompliant.ToString())
                    && (Model.AccessibilityStatementWebPageName.IsNullOrEmpty() || Model.AccessibilityStatementWebPage.IsNullOrEmpty()))
                {
                    if (openApiVersion < 10)
                    {
                        modelState.AddModelError("AccessibilityStatementWebPage", string.Format(CoreMessages.OpenApi.RequiredLanguageValueNotFound, Model.Language));
                    }
                    else
                    {
                        modelState.AddModelError($"{PropertyName}.AccessibilityStatementWebPage", $"'AccessibilityStatementWebPage' and 'AccessibilityStatementWebPageName' fields are required when 'AccessibilityClassificationLevel' has value '{Model.AccessibilityClassificationLevel}'.");
                    }
                }

                // The accessibility statement web page cannot be defined when accessibilityClassificationLevel = Unknown. SFIPTV-850, SFIPTV-932
                if (Model.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.Unknown.ToString()
                    && (!Model.AccessibilityStatementWebPageName.IsNullOrEmpty() || !Model.AccessibilityStatementWebPage.IsNullOrEmpty()))
                {
                    if (openApiVersion < 10)
                    {
                        modelState.AddModelError("AccessibilityStatementWebPage", $"'AccessibilityStatementWebPage' cannot be set when 'AccessibilityClassificationLevel' has value '{ Model.AccessibilityClassificationLevel }'.");
                    }
                    else
                    {
                        modelState.AddModelError($"{PropertyName}.AccessibilityStatementWebPage", $"'AccessibilityStatementWebPage' and 'AccessibilityStatementWebPageName' cannot be set when 'AccessibilityClassificationLevel' has value '{ Model.AccessibilityClassificationLevel }'.");
                    }
                }
            }
        }
    }
}
