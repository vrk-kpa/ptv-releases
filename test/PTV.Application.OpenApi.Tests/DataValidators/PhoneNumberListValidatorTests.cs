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
using Moq;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using PTV.Domain.Model.Models;
using System;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class PhoneNumberListValidatorTests : ValidatorTestBase
    {
        private List<VmOpenApiPhoneSimpleVersionBase> list;

        public PhoneNumberListValidatorTests()
        {
            list = new List<VmOpenApiPhoneSimpleVersionBase>
            {
                new VmOpenApiPhoneSimpleVersionBase { Language = "language1", PrefixNumber = "1" },
                new VmOpenApiPhoneSimpleVersionBase { Language = "language2", PrefixNumber = "2" },
            };
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(null, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIsNullAndRequiredLanguagesSet()
        {
            // Arrange
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(null, codeService, requiredLanguages: new List<string> { "language1" });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelIsNullAndAvailableLanguagesSet()
        {
            // Arrange
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(null, codeService, availableLanguages: new List<string> { "language1" });

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
            codeServiceMockSetup.Setup(c => c.GetDialCode(It.IsAny<string>())).Returns(new VmDialCode { Id = Guid.NewGuid() });
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(list, codeService, requiredLanguages: new List<string> { language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("language3")]
        public void ModelAndRequiredLanguagesSetAndStateInvalid(string language)
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetDialCode(It.IsAny<string>())).Returns(new VmDialCode { Id = Guid.NewGuid() });
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(list, codeService, requiredLanguages: new List<string> { language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("language1")]
        [InlineData("language2")]
        public void ModelAndAvailableLanguagesSetAndPrefixExistsStateValid(string language)
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetDialCode(It.IsAny<string>())).Returns(new VmDialCode { Id = Guid.NewGuid() });
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(list, codeService, requiredLanguages: new List<string> { language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("language3")]
        public void ModelAndAvailableLanguagesSetAndPrefixExistsStateInvalid(string language)
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetDialCode(It.IsAny<string>())).Returns(new VmDialCode { Id = Guid.NewGuid() });
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(list, codeService, requiredLanguages: new List<string> { language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetAndPrefixExistsStateValid()
        {
            // Arrange
            list.Add(new VmOpenApiPhoneSimpleVersionBase { Language = "test", PrefixNumber = null });
            codeServiceMockSetup.Setup(c => c.GetDialCode(It.IsAny<string>())).Returns(new VmDialCode { Id = Guid.NewGuid() });
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIncludesInvalidPrefix()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetDialCode("1")).Returns(new VmDialCode { Id = Guid.NewGuid() });
            codeServiceMockSetup.Setup(c => c.GetDialCode("2")).Returns((VmDialCode)null);
            var validator = new PhoneNumberListValidator<VmOpenApiPhoneSimpleVersionBase>(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
