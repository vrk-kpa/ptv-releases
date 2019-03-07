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
using Xunit;
using System;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class V8ConnectionValidatorTests : ValidatorTestBase
    {
        private const string PROPERTY_NAME = "Property name";

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ConnectionBaseValidator<V8VmOpenApiServiceAndChannelIn, V8VmOpenApiContactDetailsIn, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(null, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void NotValidServiceHoursAttached()
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceAndChannelIn>{ new V8VmOpenApiServiceAndChannelIn
            {
                ServiceHours = new List<V8VmOpenApiServiceHour>{ new V8VmOpenApiServiceHour { ServiceHourType = ServiceHoursTypeEnum.Special.ToString() } }
            }};
            var validator = new ConnectionBaseValidator<V8VmOpenApiServiceAndChannelIn, V8VmOpenApiContactDetailsIn, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ContactDetailsAttached()
        {
            // Arrange
            var code = "code";
            var vm = new List<V8VmOpenApiServiceAndChannelIn>{ new V8VmOpenApiServiceAndChannelIn
            {
                ContactDetails = new V8VmOpenApiContactDetailsIn
                {
                    Addresses = new List<V7VmOpenApiAddressContactIn>{ new V7VmOpenApiAddressContactIn
                    {
                        Type = AddressConsts.VISITING,
                        SubType = AddressConsts.STREET,
                        StreetAddress = new VmOpenApiAddressStreetWithCoordinatesIn
                        {
                            Municipality = code,
                            PostalCode = code,                            
                        },
                        Country = code
                    } },
                    PhoneNumbers = new List<V4VmOpenApiPhone>{ new V4VmOpenApiPhone { PrefixNumber = code } }
                }
            }};
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(code, true)).Returns(new VmListItem { Id = Guid.NewGuid() });
            codeServiceMockSetup.Setup(c => c.GetPostalCodeByCode(code, true)).Returns(code);
            codeServiceMockSetup.Setup(c => c.GetCountryByCode(code)).Returns(code);
            codeServiceMockSetup.Setup(c => c.GetDialCode(code)).Returns(new VmDialCode { Id = Guid.NewGuid() });
            var validator = new ConnectionBaseValidator<V8VmOpenApiServiceAndChannelIn, V8VmOpenApiContactDetailsIn, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, codeServiceMockSetup.Object);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            codeServiceMockSetup.Verify(c => c.GetMunicipalityByCode(code, true), Times.Once);
            codeServiceMockSetup.Verify(c => c.GetPostalCodeByCode(code, true), Times.Once);
            codeServiceMockSetup.Verify(c => c.GetCountryByCode(code), Times.Once);
            codeServiceMockSetup.Verify(c => c.GetDialCode(code), Times.Once);
        }
    }
}
