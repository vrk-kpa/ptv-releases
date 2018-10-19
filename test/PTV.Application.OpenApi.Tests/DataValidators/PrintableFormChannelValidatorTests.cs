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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V8;
using Xunit;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class PrintableFormChannelValidatorTests : ValidatorTestBase
    {
        private int version = 9;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new PrintableFormChannelValidator(null, organizationService, codeService, serviceService, version);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase()
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void DeliveryAddressSet_StreetNotSet()
        {
            // Arrange
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase()
                {
                    DeliveryAddresses = new List<V8VmOpenApiAddressDeliveryIn>
                    {
                        new V8VmOpenApiAddressDeliveryIn
                            {
                                SubType = AddressTypeEnum.Street.ToString(),
                                StreetAddress = null
                            }
                        
                    }
                },
                organizationService, codeService, serviceService, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void DeliveryAddressSet_PostOfficeBoxNotSet()
        {
            // Arrange
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase()
                {
                    DeliveryAddresses = new List<V8VmOpenApiAddressDeliveryIn>
                    {
                        new V8VmOpenApiAddressDeliveryIn
                            {
                                SubType = AddressTypeEnum.PostOfficeBox.ToString(),
                                PostOfficeBoxAddress = null
                            }
                    }
                },
                organizationService, codeService, serviceService, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
