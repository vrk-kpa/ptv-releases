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
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Models;
using Moq;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class StreetAddressValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new StreetAddressValidator(null, "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MunicipalitySet_MunicipalityValidated()
        {
            // Arrange
            var code = "111";
            codeServiceMockSetup.Setup(x => x.GetMunicipalityByCode(code, true)).Returns(new VmListItem { Id = Guid.NewGuid()});
            var validator = new StreetAddressValidator(new VmOpenApiAddressStreetWithCoordinatesIn { Municipality = code }, "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            codeServiceMockSetup.Verify(x => x.GetMunicipalityByCode(code, true), Times.Once);
        }

        [Fact]
        public void PostalCodeSet_PostalCodeValidated()
        {
            // Arrange
            var code = "111";
            codeServiceMockSetup.Setup(x => x.GetPostalCodeByCode(code, true)).Returns(code);
            var validator = new StreetAddressValidator(new VmOpenApiAddressStreetWithCoordinatesIn { PostalCode = code }, "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            codeServiceMockSetup.Verify(x => x.GetPostalCodeByCode(code, true), Times.Once);
        }
    }
}
