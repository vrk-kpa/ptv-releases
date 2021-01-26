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

using FluentAssertions;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class AccessibilityClassificationValidatorListTests : ValidatorTestBase
    {
        private int openApiVersion = 10;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new AccessibilityClassificationListValidator(null, openApiVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void AllAvailableLanguagesNotSet()
        {
            // Arrange
            var lang1 = "language1";
            var lang2 = "language2";
            var vm = new List<VmOpenApiAccessibilityClassification> {
                new VmOpenApiAccessibilityClassification { AccessibilityClassificationLevel = AccessibilityClassificationLevelTypeEnum.Unknown.ToString(), Language = lang1 },
            };
            var languageList = new List<string> { lang1, lang2 };
            var validator = new AccessibilityClassificationListValidator(vm, openApiVersion, languageList);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void AllAvailableLanguagesSet()
        {
            // Arrange
            var lang1 = "language1";
            var lang2 = "language2";
            var vm = new List<VmOpenApiAccessibilityClassification> {
                new VmOpenApiAccessibilityClassification { AccessibilityClassificationLevel = AccessibilityClassificationLevelTypeEnum.Unknown.ToString(), Language = lang1 },
                new VmOpenApiAccessibilityClassification { AccessibilityClassificationLevel = AccessibilityClassificationLevelTypeEnum.Unknown.ToString(), Language = lang2 },
            };
            var languageList = new List<string> { lang1, lang2 };
            var validator = new AccessibilityClassificationListValidator(vm, openApiVersion, languageList);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
