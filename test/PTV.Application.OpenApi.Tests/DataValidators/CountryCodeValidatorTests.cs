﻿/**
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
using Moq;
using PTV.Application.OpenApi.DataValidators;
using Xunit;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class CountryCodeValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void CodeIsNull()
        {
            // Arrange
            var validator = new CountryCodeValidator(null, codeService, "Country");

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void CodeExists()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns("FI");
            var validator = new CountryCodeValidator("code", codeServiceMockSetup.Object, "Country");

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void CodeNotExistsCodeServiceReturnsNull()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns((string)null);
            var validator = new CountryCodeValidator("code", codeServiceMockSetup.Object, "Country");

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void CodeNotExistsCodeServiceReturnsEmpty()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns("");
            var validator = new CountryCodeValidator("code", codeServiceMockSetup.Object, "Country");

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
