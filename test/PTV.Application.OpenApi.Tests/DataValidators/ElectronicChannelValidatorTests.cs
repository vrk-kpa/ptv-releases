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
using PTV.Domain.Model.Enums;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ElectronicChannelValidatorTests : ValidatorTestBase
    {
        private int version = 10;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ElectronicChannelValidator(null, organizationService, codeService, serviceService, commonService, null, version);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RequiredWebPageNotSet()
        {
            // Arrange
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone { Language = "fi"} },
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("WebPage").Should().BeTrue();
        }

        [Fact]
        public void AllAvailableWebPagesNotSet()
        {
            // Arrange
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Value = "http://www.tieto.com", Language = "fi"} },
                    SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone { Language = "se" } },
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("WebPage").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_NoCurrentVersion_RequiredPropertiesMissing()
        {
            // Arrange
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = "fi" } },
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_WebPage_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem {  Language = language },
                        new VmOpenApiLanguageItem { Language = language + "2" }},
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_Attachments__CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    Attachments = new List<VmOpenApiAttachment> { new VmOpenApiAttachment { Language = language },
                        new VmOpenApiAttachment { Language = language + "2" }},
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_SupportPhones__CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone { Language = language },
                        new V4VmOpenApiPhone { Language = language + "2" }},
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_SupportEmails__CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language },
                        new VmOpenApiLanguageItem { Language = language + "2" }},
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_ServiceHours__CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    ServiceHours = new List<V11VmOpenApiServiceHour>
                    {
                        new V11VmOpenApiServiceHour {
                            ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                            AdditionalInformation = new List<VmOpenApiLanguageItem>{new VmOpenApiLanguageItem{ Language = language } }
                        },
                        new V11VmOpenApiServiceHour {
                            ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                            AdditionalInformation = new List<VmOpenApiLanguageItem>{new VmOpenApiLanguageItem{Language = language + "2" } }
                        }
                    },
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void NoLanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> {
                        new VmOpenApiLocalizedListItem { Language = language, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    },
                    WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language} },
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceChannelDescriptions").Should().BeTrue();
            controller.ModelState.ContainsKey("ServiceChannelNames").Should().BeFalse();
            controller.ModelState.ContainsKey("WebPage").Should().BeFalse();
        }

        [Fact]
        public void NoChangesWithinRequiredProperties_CurrentVersionExists_OptionalPropertiesUpdated()
        {
            // Arrange
            var language = "fi";
            var organizationId = Guid.NewGuid();
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language }, OrganizationId = organizationId };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { language });
            var validator = new ElectronicChannelValidator
                (new VmOpenApiElectronicChannelInVersionBase
                {
                    Attachments = new List<VmOpenApiAttachment> { new VmOpenApiAttachment { Language = language} },
                    SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language} },
                    SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone { Language = language} },
                    ServiceHours = new List<V11VmOpenApiServiceHour>
                    {
                        new V11VmOpenApiServiceHour { ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(), AdditionalInformation = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language} } }
                    },
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
