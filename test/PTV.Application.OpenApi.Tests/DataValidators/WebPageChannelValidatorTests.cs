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

using FluentAssertions;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class WebPageChannelValidatorTests : ValidatorTestBase
    {
        private int version = 8;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new WebPageChannelValidator(null, organizationService, codeService, serviceService, version);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange
            var validator = new WebPageChannelValidator
                (new VmOpenApiWebPageChannelInVersionBase()
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelAAA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelAA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, null)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, null)]
        public void AccessibilityClassificationLevel_WCAGLevel_Valid(AccessibilityClassificationLevelTypeEnum acLevel, WcagLevelTypeEnum? wcagLevel)
        {
            // Arrange
            var validator = new WebPageChannelValidator
                (new VmOpenApiWebPageChannelInVersionBase()
                {
                    AccessibilityClassificationLevel = acLevel.ToString(),
                    WCAGLevel = wcagLevel.ToString()
                },
                organizationService, codeService, serviceService, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, null)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelAA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelAAA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelAA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.NonCompliant, WcagLevelTypeEnum.LevelAAA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelAA)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, WcagLevelTypeEnum.LevelAAA)]
        public void AccessibilityClassificationLevel_WCAGLevel_NotValid(AccessibilityClassificationLevelTypeEnum acLevel, WcagLevelTypeEnum? wcagLevel)
        {
            // Arrange
            var validator = new WebPageChannelValidator
                (new VmOpenApiWebPageChannelInVersionBase()
                {
                    AccessibilityClassificationLevel = acLevel.ToString(),
                    WCAGLevel = wcagLevel.ToString()
                },
                organizationService, codeService, serviceService, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
