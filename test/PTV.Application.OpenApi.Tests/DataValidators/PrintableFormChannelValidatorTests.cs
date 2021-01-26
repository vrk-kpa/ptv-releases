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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V8;
using Xunit;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class PrintableFormChannelValidatorTests : ValidatorTestBase
    {
        private int version = 9;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new PrintableFormChannelValidator(null, organizationService, codeService, serviceService, commonService, null, version);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
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
        public void DeliveryAddressSet_StreetNotSet()
        {
            // Arrange
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
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
                organizationService, codeService, serviceService, commonService, null, version);

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
                (new VmOpenApiPrintableFormChannelInVersionBase
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
                organizationService, codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_NoCurrentVersion_RequiredPropertiesMissing()
        {
            // Arrange
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
                {
                    DeliveryAddresses = new List<V8VmOpenApiAddressDeliveryIn>
                    {
                        new V8VmOpenApiAddressDeliveryIn
                            {
                                SubType = AddressTypeEnum.PostOfficeBox.ToString(),
                                PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn
                                {
                                    AdditionalInformation = new List<VmOpenApiLanguageItem>{new VmOpenApiLanguageItem { Language = "fi"} }
                                }
                            }
                    },
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
        public void LanguageVersionsAdded_FormIdentifier_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
                {
                    FormIdentifier = new List<VmOpenApiLanguageItem>
                    {
                        new VmOpenApiLanguageItem { Language = language},
                        new VmOpenApiLanguageItem { Language = language + "2"},
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
        public void LanguageVersionsAdded_DeliveryAddresses_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
                {
                    DeliveryAddresses = new List<V8VmOpenApiAddressDeliveryIn>
                    {
                        new V8VmOpenApiAddressDeliveryIn
                        {
                            SubType = AddressTypeEnum.PostOfficeBox.ToString(),
                            PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn
                            {
                                AdditionalInformation = new List<VmOpenApiLanguageItem>{
                                    new VmOpenApiLanguageItem { Language = language},
                                    new VmOpenApiLanguageItem { Language = language + "2"},
                                }
                            }
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
        public void LanguageVersionsAdded_SupportPhones_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
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
        public void LanguageVersionsAdded_SupportEmails_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
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
        public void LanguageVersionsAdded_Attachments_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
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
        public void NoLanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "fi";
            var language2 = "fi2";
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
                {
                    ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> {
                        new VmOpenApiLocalizedListItem { Language = language, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                        new VmOpenApiLocalizedListItem { Language = language2, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    },
                    ChannelUrls = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Language = language }},
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                organizationService, codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceChannelDescriptions").Should().BeTrue();
            controller.ModelState.ContainsKey("ChannelUrls").Should().BeTrue();
        }

        [Fact]
        public void NoChangesWithinRequiredProperties_CurrentVersionExists_OptionalPropertiesUpdated()
        {
            // Arrange
            var language = "fi";
            var organizationId = Guid.NewGuid();
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language }, OrganizationId = organizationId };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { language });
            var validator = new PrintableFormChannelValidator
                (new VmOpenApiPrintableFormChannelInVersionBase
                {
                    FormIdentifier = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                    DeliveryAddresses = new List<V8VmOpenApiAddressDeliveryIn>
                    {
                        new V8VmOpenApiAddressDeliveryIn
                        {
                            SubType = AddressTypeEnum.PostOfficeBox.ToString(),
                            PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn
                            {
                                AdditionalInformation = new List<VmOpenApiLanguageItem>{
                                    new VmOpenApiLanguageItem { Language = language}
                                }
                            }
                        }
                    },
                    SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone { Language = language } },
                    SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                    Attachments = new List<VmOpenApiAttachment> { new VmOpenApiAttachment { Language = language }},
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
