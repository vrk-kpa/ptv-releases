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
using System;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class PublishingStatusValidatorTests : ValidatorTestBase
    {
        [Theory]
        [InlineData("InvalidStatus", "Published")]
        [InlineData("Published", "InvalidStatus")]
        public void StatusInvalid(string currentPublishingStatus, string newPublishingStatus)
        {
            // Arrange
            var validator = new PublishingStatusValidator(newPublishingStatus, currentPublishingStatus);

            // Act
            Action act = () => validator.Validate(controller.ModelState);

            // Assert
            act.ShouldThrowExactly<Exception>().WithMessage("The field is invalid. Please use one of these: *");
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "Draft")]
        [InlineData(null, "Published")]
        [InlineData("Draft", null)] // Is same as Draft -> Draft
        [InlineData("Draft", "Draft")]
        [InlineData("Draft", "Published")]
        [InlineData("Published", null)]
        [InlineData("Published", "Modified")]
        [InlineData("Published", "Published")]
        [InlineData("Published", "Deleted")]
        [InlineData("Deleted", "Modified")]
        [InlineData("Deleted", "Published")]
        [InlineData("Deleted", "Deleted")]
        // Tests for cases that are valid.
        // Allowed updates for publishing status:
        // Draft     -> Draft or Published
        // Published -> Modified, Published, Archived
        // Archived  -> Modified, Published, Archived
        public void StateValid(string currentPublishingStatus, string newPublishingStatus)
        {
            // Arrange
            var validator = new PublishingStatusValidator(newPublishingStatus, currentPublishingStatus);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "Modified")]
        [InlineData(null, "OldPublished")]
        [InlineData(null, "Deleted")]
        [InlineData("Draft", "Modified")]
        [InlineData("Draft", "Deleted")]
        [InlineData("Draft", "OldPublished")]
        [InlineData("Published", "Draft")]
        [InlineData("Published", "OldPublished")]
        [InlineData("Modified", null)]
        [InlineData("Modified", "Draft")]
        [InlineData("Modified", "Modified")]
        [InlineData("Modified", "Published")]
        [InlineData("Modified", "Deleted")]
        [InlineData("Modified", "OldPublished")]
        [InlineData("OldPublished", null)]
        [InlineData("OldPublished", "Draft")]
        [InlineData("OldPublished", "Modified")]
        [InlineData("OldPublished", "Published")]
        [InlineData("OldPublished", "Deleted")]
        [InlineData("OldPublished", "OldPublished")]
        [InlineData("Deleted", null)]
        [InlineData("Deleted", "Draft")]
        [InlineData("Deleted", "OldPublished")]
        // Tests for cases that are invalid.
        public void StateInvalid(string currentPublishingStatus, string newPublishingStatus)
        {
            // Arrange
            var validator = new PublishingStatusValidator(newPublishingStatus, currentPublishingStatus);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
