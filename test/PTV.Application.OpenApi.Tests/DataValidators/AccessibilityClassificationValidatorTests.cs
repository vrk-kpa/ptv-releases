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
    public class AccessibilityClassificationValidatorTests : ValidatorTestBase
    {
        private int openApiVersion = 10;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new AccessibilityClassificationValidator(null, openApiVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelAAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, null, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, null, false)]
        public void AccessibilityClassification_Valid(AccessibilityClassificationLevelTypeEnum acLevel, WcagLevelTypeEnum? wcagLevel, bool webpageSet)
        {
            // Arrange
            var validator = new AccessibilityClassificationValidator(
                new VmOpenApiAccessibilityClassification
                {
                    AccessibilityClassificationLevel = acLevel.ToString(),
                    WcagLevel = wcagLevel.ToString(),
                    AccessibilityStatementWebPage = webpageSet ? "Webpage name" : null,
                    AccessibilityStatementWebPageName = webpageSet ? "Url" : null
                },
                openApiVersion
            );

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, null, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, null, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelAAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelAAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelAAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelAAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelAAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, null, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelAAA, true)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelAAA, false)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, null, true)]
        public void AccessibilityClassification_NotValid(AccessibilityClassificationLevelTypeEnum acLevel, WcagLevelTypeEnum? wcagLevel, bool webpageSet)
        {
            // Arrange
            var validator = new AccessibilityClassificationValidator(
                new VmOpenApiAccessibilityClassification
                {
                    AccessibilityClassificationLevel = acLevel.ToString(),
                    WcagLevel = wcagLevel.ToString(),
                    AccessibilityStatementWebPage = webpageSet ? "Webpage name" : null,
                    AccessibilityStatementWebPageName = webpageSet ? "Url" : null
                },
                openApiVersion
            );

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
