﻿/**
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

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class PostalCodeValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void CodeIsNull()
        {
            // Arrange
            var validator = new PostalCodeValidator(null, codeService);

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void CodeExists()
        {
            // Arrange
            var code = "code";
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>(), true)).Returns(code);
            var validator = new PostalCodeValidator(code, codeServiceMockSetup.Object);

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            codeServiceMockSetup.Verify(x => x.GetPostalCodeByCode(code, true), Times.Once);
        }

        [Fact]
        public void CodeNotExistsCodeServiceReturnsNull()
        {
            // Arrange
            var code = "code";
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>(), true)).Returns((string)null);
            var validator = new PostalCodeValidator(code, codeServiceMockSetup.Object);

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            codeServiceMockSetup.Verify(x => x.GetPostalCodeByCode(code, true), Times.Once);
        }

        [Fact]
        public void CodeNotExistsCodeServiceReturnsEmpty()
        {
            // Arrange
            var code = "code";
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>(), true)).Returns(string.Empty);
            var validator = new PostalCodeValidator(code, codeServiceMockSetup.Object);

            //Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            codeServiceMockSetup.Verify(x => x.GetPostalCodeByCode(code, true), Times.Once);
        }
    }
}
