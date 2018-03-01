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
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceLocationChannelValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceLocationChannelValidator(null, commonService, codeService, serviceService);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(AddressConsts.STREET)] // not allowed - PTV-2910
        [InlineData(AddressConsts.POSTOFFICEBOX)] // not allowed
        public void AddressTypeIsLocation_SubTypeNotValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(AddressConsts.SINGLE)] // allowed - PTV-2910
        [InlineData(AddressConsts.MULTIPOINT)] // allowed
        [InlineData(AddressConsts.ABROAD)] // allowed
        public void AddressTypeIsLocation_SubTypeValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AddressConsts.SINGLE)] // not allowed - PTV-2910
        [InlineData(AddressConsts.MULTIPOINT)] // not allowed
        public void AddressTypeIsPostal_SubTypeNotValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressCharacterEnum.Postal.ToString(),
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


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
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressCharacterEnum.Postal.ToString(),
                        SubType = subType
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void AddressTypeIsLocation_SubTypeMultipoint_OtherTypesIncluded_NotValid()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.MULTIPOINT
                    },
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void AddressTypeIsLocation_SubTypSingle_OtherTypesIncluded_NotValid()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    },
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.FOREIGN
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void AddressTypeIsLocation_MoreThan2SingleSubTypesIncluded_NotValid()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    },
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    },
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    }
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void AddressTypeIsLocation_2SIngleSubTypesIncluded_Valid()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    },
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = AddressConsts.LOCATION,
                        SubType = AddressConsts.SINGLE
                    },
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


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
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = type,
                        SubType = AddressConsts.ABROAD
                    },
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = type,
                        SubType = AddressConsts.SINGLE
                    },
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(AddressConsts.LOCATION)]
        [InlineData(AddressConsts.POSTAL)]
        public void AddressTypeIsLocation_SeveralForeigAddressesIncluded_NotValid(string type)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                Addresses = new List<V7VmOpenApiAddressWithMovingIn>
                {
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = type,
                        SubType = AddressConsts.ABROAD
                    },
                    new V7VmOpenApiAddressWithMovingIn
                    {
                        Type = type,
                        SubType = AddressConsts.ABROAD
                    },
                }
            };
            var validator = new ServiceLocationChannelValidator(vm, commonService, codeService, serviceService);
            
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
