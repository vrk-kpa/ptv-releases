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
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using System;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class LocalizedListValidatorTests : ValidatorTestBase
    {
        private List<VmOpenApiLocalizedListItem> list;

        public LocalizedListValidatorTests()
        {
            list = new List<VmOpenApiLocalizedListItem>()
            {
                new VmOpenApiLocalizedListItem { Language = "language1", Type = "type1" },
                new VmOpenApiLocalizedListItem { Language = "language2", Type = "type2" },
                new VmOpenApiLocalizedListItem { Language = "language1", Type = "type2" },
                new VmOpenApiLocalizedListItem { Language = "language2", Type = "type1" },
            };
        }

        [Fact]
        public void AllParametersAreNull()
        {
            // Act
            Action act = () => new LocalizedListValidator(null, null, null, null, null);

            // Assert
            act.ShouldThrowExactly< ArgumentNullException>().WithMessage("Property name must be defined.\r\nParameter name: propertyName");
        }

        [Fact]
        public void PropertyNameSetAllOtherParametersAreNull()
        {
            // Arrange
            var validator = new LocalizedListValidator(null, "PropertyName", null, null, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIsNullAndRequiredLanguagesSet()
        {
            // Arrange
            var validator = new LocalizedListValidator(null, "PropertyName", new List<string>() { "language1" }, null, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
        
        [Fact]
        public void ModelIsNullAndAvailableLanguagesSet()
        {
            // Arrange
            var validator = new LocalizedListValidator(null, "PropertyName", null, null, new List<string>() { "language1" });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("language1")]
        [InlineData("language2")]
        public void ModelAndRequiredLanguagesSetAndStateValid(string language)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", new List<string>() { language }, null, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("language3")]
        public void ModelAndRequiredLanguagesSetAndStateInvalid(string language)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", new List<string>() { language }, null, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("type1")]
        [InlineData("type2")]
        public void ModelAndRequiredLanguagesAndTypeSetAndStateValid(string type)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", new List<string>() { "language1", "language2" }, new List<string> { "type1" }, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("type3")]
        public void ModelAndRequiredLanguagesAndTypeSetAndStateInvalid(string type)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", new List<string>() { "language1", "language2" }, new List<string> { type }, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("language1")]
        [InlineData("language2")]
        public void ModelAndAvailableLanguagesSetAndStateValid(string language)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", null, null, new List<string>() { language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("language3")]
        public void ModelAndAvailableLanguagesSetAndStateInvalid(string language)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", null, null, new List<string>() { language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("type1")]
        [InlineData("type2")]
        public void ModelAndAvailableLanguagesAndTypeSetAndStateValid(string type)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", new List<string>() { "language1", "language2" }, new List<string> { type }, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("type3")]
        public void ModelAndAvailableLanguagesAndTypeSetAndStateInvalid(string type)
        {
            // Arrange
            var validator = new LocalizedListValidator(list, "PropertyName", new List<string>() { "language1", "language2" }, new List<string> { type }, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
