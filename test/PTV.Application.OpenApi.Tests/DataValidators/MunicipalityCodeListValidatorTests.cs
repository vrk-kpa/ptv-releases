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
using System.Collections.Generic;
using System;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class MunicipalityCodeListValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new MunicipalityCodeListValidator(null, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelListIncludesNull()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>(), true)).Returns(new VmListItem { Id = Guid.NewGuid() });
            var list = new List<string> { null, "code" };
            var validator = new MunicipalityCodeListValidator(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelListIncludesInvalidLanguage()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode("code", true)).Returns(new VmListItem { Id = Guid.NewGuid(), Code = "code" });
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode("invalidCode", true)).Returns((VmListItem)null);
            var list = new List<string> { null, "code", "invalidCode" };
            var validator = new MunicipalityCodeListValidator(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
