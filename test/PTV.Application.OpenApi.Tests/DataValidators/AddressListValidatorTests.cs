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
using PTV.Domain.Model.Models.OpenApi.V7;
using Moq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using System;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class AddressListValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new AddressListValidator<V7VmOpenApiAddressWithForeignIn>(null, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelListIncludesNull()
        {
            // Arrange
            var list = new List<V7VmOpenApiAddressWithForeignIn> { null, new V7VmOpenApiAddressWithForeignIn() };
            var validator = new AddressListValidator<V7VmOpenApiAddressWithForeignIn>(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndCountryCodeNotExists()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns(string.Empty);
            var list = new List<V7VmOpenApiAddressWithForeignIn>
            {
                new V7VmOpenApiAddressWithForeignIn
                {
                    SubType = AddressTypeEnum.Street.ToString(),
                    StreetAddress = new VmOpenApiAddressStreetWithCoordinatesIn
                    {
                        PostalCode = "code",
                        Municipality = "code",
                    },
                    Country = "country"
                } };
            var validator = new AddressListValidator<V7VmOpenApiAddressWithForeignIn>(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetAndCountryCodeExists()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns("code");
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns(new VmListItem { Id = Guid.NewGuid() });
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns("country");
            var list = new List<V7VmOpenApiAddressWithForeignIn>
            {
                new V7VmOpenApiAddressWithForeignIn
                {
                    SubType = AddressTypeEnum.Street.ToString(),
                    StreetAddress = new VmOpenApiAddressStreetWithCoordinatesIn
                    {
                        PostalCode = "code",
                        Municipality = "code",
                    },
                    Country = "country"
                } };
            var validator = new AddressListValidator<V7VmOpenApiAddressWithForeignIn>(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndCountryNotSet()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns("code");
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns(new VmListItem { Id = Guid.NewGuid() });
            var list = new List<V7VmOpenApiAddressWithForeignIn>
            {
                new V7VmOpenApiAddressWithForeignIn
                {
                    SubType = AddressTypeEnum.Street.ToString(),
                    StreetAddress = new VmOpenApiAddressStreetWithCoordinatesIn
                    {
                        PostalCode = "code",
                        Municipality = "code",
                    },
                } };
            var validator = new AddressListValidator<V7VmOpenApiAddressWithForeignIn>(list, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
