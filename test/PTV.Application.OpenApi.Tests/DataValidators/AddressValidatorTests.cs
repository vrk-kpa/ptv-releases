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
using Moq;
using PTV.Application.OpenApi.DataValidators;
using System;
using Xunit;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class AddressValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new AddressValidator(null, "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(true, true, true, false)]
        [InlineData(false, true, true, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, true, false, true)]
        public void ModelSetAndCountryCodeNotExists(bool postalCodeSet, bool municipalitySet, bool countrySet, bool shouldBeValid)
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>())).Returns("code");
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>())).Returns(new VmListItem { Id = Guid.NewGuid() });
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns(string.Empty);

            var validator = new AddressValidator(TestModel(postalCodeSet, municipalitySet, countrySet), "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            if (shouldBeValid)
            {
                controller.ModelState.IsValid.Should().BeTrue();
            }
            else
            {
                controller.ModelState.IsValid.Should().BeFalse();
            }            
        }
        
        [Theory]
        [InlineData(true, true, true, false)]
        [InlineData(false, true, true, false)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, false)]
        public void ModelSetAndMunicipalityCodeNotExists(bool postalCodeSet, bool municipalitySet, bool countrySet, bool shouldBeValid)
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>())).Returns("code");
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>())).Returns((VmListItem)null);
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns("code");

            var validator = new AddressValidator(TestModel(postalCodeSet, municipalitySet, countrySet), "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            if (shouldBeValid)
            {
                controller.ModelState.IsValid.Should().BeTrue();
            }
            else
            {
                controller.ModelState.IsValid.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(true, true, true, false)]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, true, false)]
        [InlineData(true, true, false, false)]
        public void ModelSetAndPostalCodeNotExists(bool postalCodeSet, bool municipalitySet, bool countrySet, bool shouldBeValid)
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>())).Returns(string.Empty);
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>())).Returns(new VmListItem { Id = Guid.NewGuid() });
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns("code");

            var validator = new AddressValidator(TestModel(postalCodeSet, municipalitySet, countrySet), "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            if (shouldBeValid)
            {
                controller.ModelState.IsValid.Should().BeTrue();
            }
            else
            {
                controller.ModelState.IsValid.Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, true)]
        public void ModelSetAndAddressExists(bool postalCodeSet, bool municipalitySet, bool countrySet, bool shouldBeValid)
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(It.IsAny<string>())).Returns("code");
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>())).Returns(new VmListItem { Id = Guid.NewGuid() });
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(It.IsAny<string>())).Returns("code");

            var validator = new AddressValidator(TestModel(postalCodeSet, municipalitySet, countrySet), "Address", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            if (shouldBeValid)
            {
                controller.ModelState.IsValid.Should().BeTrue();
            }
            else
            {
                controller.ModelState.IsValid.Should().BeFalse();
            }
        }

        private V5VmOpenApiAddressIn TestModel(bool postalCodeSet, bool municipalitySet, bool countrySet)
        {
            return new V5VmOpenApiAddressIn()
            {
                PostalCode = postalCodeSet ? "code" : null,
                Municipality = municipalitySet ? "code" : null,
                Country = countrySet ? "country" : null
            };
        }
    }
}
