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
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceLocationChannelValidatorTests : ValidatorTestBase
    {
        private int version = 9;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceLocationChannelValidator(null, organizationService, codeService, serviceService, commonService, null, version);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DisplayNameTypeNotSet_NameListNotExists_ModelInvalid()
        {
            // Arrange & act
            var type = NameTypeEnum.AlternateName.ToString();
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage>{ new VmOpenApiNameTypeByLanguage
                {
                    Type = type,
                    Language = "language"
                } }
            };
            var currentVersion = new VmOpenApiServiceChannel
            {
                ServiceChannelNames = new List<VmOpenApiLocalizedListItem> {  new VmOpenApiLocalizedListItem
                {
                    Value = "name",
                    Type = type.GetOpenApiEnumValue<NameTypeEnum>(),
                    Language = "language2"
                } }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, currentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("DisplayNameType").Should().BeTrue();
        }

        [Fact]
        public void DisplayNameTypeNotSet_NameListNotContainsType_ModelInvalid()
        {
            // Arrange & act
            var language = "language";
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage>{ new VmOpenApiNameTypeByLanguage
                {
                    Type = "Name",
                    Language = language
                } }
            };
            var currentVersion = new VmOpenApiServiceChannel
            {
                ServiceChannelNames = new List<VmOpenApiLocalizedListItem> {  new VmOpenApiLocalizedListItem
                {
                    Value = "Name",
                    Type = "AlternativeName",
                    Language = language
                } }
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, currentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("DisplayNameType").Should().BeTrue();
        }

        [Theory]
        [InlineData(AddressConsts.STREET)] // not allowed - PTV-2910
        [InlineData(AddressConsts.POSTOFFICEBOX)] // not allowed
        [InlineData("Some String")] // not allowed
        public void AddressTypeIsLocation_SubTypeNotValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
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
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, null, version);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Addresses[0].SubType").Should().BeTrue();
        }

        [Theory]
        [InlineData(AddressConsts.SINGLE)] // allowed - PTV-2910
        [InlineData(AddressConsts.ABROAD)] // allowed
        [InlineData(AddressConsts.OTHER)] // allowed
        public void AddressTypeIsLocation_SubTypeValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
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
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, null, version);

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
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
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
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Addresses[0].SubType").Should().BeTrue();
        }

        [Theory]
        [InlineData(AddressConsts.STREET)] // allowed - PTV-2910
        [InlineData(AddressConsts.POSTOFFICEBOX)] // allowed
        [InlineData(AddressConsts.ABROAD)] // allowed
        public void AddressTypeIsPostal_SubTypeValid(string subType)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
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
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, null, version);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void AddressTypeIsLocation_2SingleSubTypesIncluded_Valid()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
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
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(AddressConsts.LOCATION)]
        public void AddressTypeIsLocation_ForeigAddress_OtherSubTypesIncluded_NotValid(string type)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
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
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, null, version);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Addresses").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_NoCurrentVersion_RequiredPropertiesMissing()
        {
            // Arrange & act
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                Emails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = "fi" } },
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_Emails_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange & act
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                Emails = new List<VmOpenApiLanguageItem>{new VmOpenApiLanguageItem { Language = language },
                    new VmOpenApiLanguageItem { Language = language + "2"}},
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_FaxNumbers_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange & act
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                FaxNumbers = new List<V4VmOpenApiPhoneSimple> { new V4VmOpenApiPhoneSimple { Language = language },
                    new V4VmOpenApiPhoneSimple { Language = language + "2" } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_PhoneNumbers_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange & act
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                PhoneNumbers = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone { Language = language },
                    new V4VmOpenApiPhone { Language = language + "2"}},
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_WebPages_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange & act
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                WebPages = new List<V9VmOpenApiWebPage> { new V9VmOpenApiWebPage { Language = language },
                    new V9VmOpenApiWebPage { Language = language + "2"}},
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_ServiceHours_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange & act
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                ServiceHours = new List<V11VmOpenApiServiceHour> {
                        new V11VmOpenApiServiceHour { ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(), AdditionalInformation = new List<VmOpenApiLanguageItem>{new VmOpenApiLanguageItem{ Language = language } } },
                        new V11VmOpenApiServiceHour { ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(), AdditionalInformation = new List<VmOpenApiLanguageItem>{new VmOpenApiLanguageItem{Language = language + "2" } } } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void NoLanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange & act
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> {
                        new VmOpenApiLocalizedListItem { Language = language, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceChannelDescriptions").Should().BeTrue();
            controller.ModelState.ContainsKey("ServiceChannelNames").Should().BeFalse();
            controller.ModelState.ContainsKey("Addresses").Should().BeFalse();
        }

        [Fact]
        public void NoChangesWithinRequiredProperties_CurrentVersionExists_OptionalPropertiesUpdated()
        {
            // Arrange & act
            var language = "fi";
            var organizationId = Guid.NewGuid();
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language }, OrganizationId = organizationId };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { language });
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                Emails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                FaxNumbers = new List<V4VmOpenApiPhoneSimple> { new V4VmOpenApiPhoneSimple { Language = language } },
                PhoneNumbers = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone { Language = language } },
                WebPages = new List<V9VmOpenApiWebPage> { new V9VmOpenApiWebPage { Language = language } },
                ServiceHours = new List<V11VmOpenApiServiceHour> {
                        new V11VmOpenApiServiceHour { ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(), AdditionalInformation = new List<VmOpenApiLanguageItem>{new VmOpenApiLanguageItem{ Language = language } } } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new ServiceLocationChannelValidator(vm, organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
