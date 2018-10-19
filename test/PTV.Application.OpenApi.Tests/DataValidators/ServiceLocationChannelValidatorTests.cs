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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceLocationChannelValidatorTests : ValidatorTestBase
    {
        private int version = 9;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceLocationChannelValidator(null, organizationService, codeService, serviceService, version);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void DisplayNameTypeNotSet_NameListNotExists_ModelInvalid()
        {
            // Arrange & act
            var type = NameTypeEnum.AlternateName.ToString();
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage>{ new VmOpenApiNameTypeByLanguage
                {
                    Type = type,
                    Language = "language"
                } }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);
            validator.CurrentVersion = new VmOpenApiServiceChannel
            {
                ServiceChannelNames = new List<VmOpenApiLocalizedListItem> {  new VmOpenApiLocalizedListItem
                {
                    Value = "name",
                    Type = type.GetOpenApiEnumValue<NameTypeEnum>(),
                    Language = "language2"
                } }
            };

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void DisplayNameTypeNotSet_NameListNotContainsType_ModelInvalid()
        {
            // Arrange & act
            var language = "language";
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage>{ new VmOpenApiNameTypeByLanguage
                {
                    Type = "Name",
                    Language = language
                } }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);
            validator.CurrentVersion = new VmOpenApiServiceChannel
            {
                ServiceChannelNames = new List<VmOpenApiLocalizedListItem> {  new VmOpenApiLocalizedListItem
                {
                    Value = "Name",
                    Type = "AlternativeName",
                    Language = language
                } }
            };

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(AddressConsts.STREET)] // not allowed - PTV-2910
        [InlineData(AddressConsts.POSTOFFICEBOX)] // not allowed
        [InlineData("Some String")] // not allowed
        public void AddressTypeIsLocation_SubTypeNotValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(AddressConsts.SINGLE)] // allowed - PTV-2910
        [InlineData(AddressConsts.ABROAD)] // allowed
        [InlineData(AddressConsts.OTHER)] // allowed
        public void AddressTypeIsLocation_SubTypeValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);
            
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AddressConsts.SINGLE)] // not allowed - PTV-2910
        [InlineData(AddressConsts.OTHER)] // not allowed
        [InlineData("Some String")] // not allowed
        public void AddressTypeIsPostal_SubTypeNotValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = AddressCharacterEnum.Postal.ToString(),
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);
            
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(AddressConsts.STREET)] // allowed - PTV-2910
        [InlineData(AddressConsts.POSTOFFICEBOX)] // allowed
        [InlineData(AddressConsts.ABROAD)] // allowed
        public void AddressTypeIsPostal_SubTypeValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = AddressCharacterEnum.Postal.ToString(),
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void AddressTypeIsLocation_2SingleSubTypesIncluded_Valid()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    },
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    },
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);
            
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AddressConsts.LOCATION)]
        [InlineData(AddressConsts.POSTAL)]
        public void AddressTypeIsLocation_ForeigAddress_OtherSubTypesIncluded_NotValid(string type)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = type,
                        SubType = AddressConsts.ABROAD
                    },
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = type,
                        SubType = AddressConsts.SINGLE
                    },
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, version);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(AddressConsts.VISITING, AddressConsts.STREET)]
        [InlineData(AddressConsts.POSTAL, AddressConsts.STREET)]
        [InlineData(AddressConsts.POSTAL, AddressConsts.POSTOFFICEBOX)]
        public void V6AddressModel_Valid(string type, string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = type,
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, 6);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void V6AddressModel_NotValid()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V9VmOpenApiAddressLocationIn>
                {
                    new V9VmOpenApiAddressLocationIn
                    {
                        Type = AddressConsts.VISITING,
                        SubType = AddressConsts.POSTOFFICEBOX
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, 6);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
