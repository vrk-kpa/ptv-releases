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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceChannelValidatorTests : ValidatorTestBase
    {
        private int version = 11;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new WebPageChannelValidator(null, organizationService,
                codeService, serviceService, commonService, null, version);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        // Related organization tests

        [Fact]
        public void RelatedOrganizationAvailableLanguages_OrganizationSet_Published_V9_NotValid()
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiWebPageChannelInVersionBase
            {
                OrganizationId = id.ToString(),
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            var validator = new WebPageChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OrganizationId").Should().BeTrue();
        }

        [Fact]
        public void RelatedOrganizationAvailableLanguages_OrganizationSet_Published_V9_Valid()
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Language = language,
                    Value = "Name",
                    Type = NameTypeEnum.Name.ToString()
                } },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Language = language,
                    Value = "ShortDescription",
                    Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()
                },
                new VmOpenApiLocalizedListItem {
                    Language = language,
                    Value = "Description",
                    Type = DescriptionTypeEnum.Description.GetOpenApiValue()
                } },
                OrganizationId = id.ToString(),
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Deleted)]
        public void RelatedOrganizationAvailableLanguagesNotValidated_OrganizationSet_NotPublished_V9_Valid(PublishingStatus status)
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Language = language,
                    Type = NameTypeEnum.Name.ToString()
                } },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Language = language,
                    Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()
                },
                new VmOpenApiLocalizedListItem {
                    Language = language,
                    Type = DescriptionTypeEnum.Description.GetOpenApiValue()
                } },
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language } },
                OrganizationId = id.ToString(),
                PublishingStatus = status.ToString()
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            if (status != PublishingStatus.Draft)
            {
                vmCurrentVersion.PublishingStatus = PublishingStatus.Published.ToString();
            }

            var validator = new PhoneChannelValidator (vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Deleted)]
        public void RelatedOrganizationAvailableLanguagesNotValidated_OrganizationSet_Published_V8_Valid(PublishingStatus status)
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Language = language,
                    Type = NameTypeEnum.Name.ToString()
                } },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Language = language,
                    Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()
                },
                new VmOpenApiLocalizedListItem {
                    Language = language,
                    Type = DescriptionTypeEnum.Description.GetOpenApiValue()
                } },
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language } },
                OrganizationId = id.ToString(),
                PublishingStatus = status.ToString()
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            if (status != PublishingStatus.Draft)
            {
                vmCurrentVersion.PublishingStatus = PublishingStatus.Published.ToString();
            }
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, 8);


            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RelatedOrganizationAvailableLanguages_CurrentVersionOrganizationSet_Published_V9_NotValid()
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language }, OrganizationId = id };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            var validator = new PhoneChannelValidator(vm, organizationService, codeService, serviceService, commonService,
                vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OrganizationId").Should().BeTrue();
        }

        [Fact]
        public void RelatedOrganizationAvailableLanguages_CurrentVersionOrganizationSet_Published_V9_Valid()
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language }, OrganizationId = id, PublishingStatus = PublishingStatus.Published.ToString() };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            var validator = new PhoneChannelValidator(vm, organizationService, codeService,
                serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Deleted)]
        public void RelatedOrganizationAvailableLanguagesNotValidated_CurrentVersionOrganizationSet_NotPublished_V9_Valid(PublishingStatus status)
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                PublishingStatus = status.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            var currentVersion = new VmOpenApiServiceChannel
            {
                AvailableLanguages = new List<string> { language},
                OrganizationId = id,
                PublishingStatus = status == PublishingStatus.Draft ? PublishingStatus.Draft.ToString() : PublishingStatus.Published.ToString()
            };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, currentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Deleted)]
        public void RelatedOrganizationAvailableLanguagesNotValidated_CurrentVersionOrganizationSet_V8_Valid(PublishingStatus status)
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language } },
                PublishingStatus = status.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            var currentVersion = new VmOpenApiServiceChannel
            {
                AvailableLanguages = new List<string> { language },
                OrganizationId = id,
                PublishingStatus = status == PublishingStatus.Draft ? PublishingStatus.Draft.ToString() : PublishingStatus.Published.ToString()
            };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, currentVersion, 8);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        // Name and summary tests

        [Fact]
        public void NameAndSummary_Published_V9_NotValid()
        {
            // Arrange & act
            var text = "This is a text.";
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = NameTypeEnum.Name.ToString()
                } },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = DescriptionTypeEnum.ShortDescription.ToString()
                } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, null, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceChannelNames").Should().BeTrue();
        }

        [Theory]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Deleted)]
        public void NameAndSummary_NotPublished_V9_Valid(PublishingStatus status)
        {
            // Arrange & act
            var text = "This is a text.";
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = NameTypeEnum.Name.ToString()
                } },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()
                },
                new VmOpenApiLocalizedListItem {
                    Value = text,
                    Language = language,
                    Type = DescriptionTypeEnum.Description.GetOpenApiValue()
                } },
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language } },
                PublishingStatus = status.ToString()
            };
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, currentVersion, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Deleted)]
        public void NameAndSummaryNotSame_V9_Valid(PublishingStatus status)
        {
            // Arrange & act
            var text = "This is a text.";
            var language = "fi";
            var organizationId = Guid.NewGuid();
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { language });
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = NameTypeEnum.Name.ToString()
                } },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text + "2", // Different description than name!
                    Language = language,
                    Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()
                },
                new VmOpenApiLocalizedListItem {
                    Value = text ,
                    Language = language,
                    Type = DescriptionTypeEnum.Description.GetOpenApiValue()
                }},
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language} },
                OrganizationId = organizationId.ToString(),
                PublishingStatus = status.ToString()
            };
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel
                {
                    AvailableLanguages = new List<string> { language},
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, currentVersion, 9);
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Deleted)]
        public void NameAndSummary_V8_Valid(PublishingStatus status)
        {
            // Arrange & act
            var text = "This is a text.";
            var language = "fi";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = NameTypeEnum.Name.ToString()
                } },
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()
                },
                new VmOpenApiLocalizedListItem
                {
                    Value = text,
                    Language = language,
                    Type = DescriptionTypeEnum.Description.GetOpenApiValue()
                }},
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language } },
                PublishingStatus = status.ToString()
            };
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, currentVersion, 8);
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetValidDates))]
        public void TimedPublishing_Valid(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new PhoneChannelValidator(
                new VmOpenApiPhoneChannelInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                }, organizationService, codeService, serviceService, commonService, null, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInValidDates))]
        public void TimedPublishing_NotValid(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new PhoneChannelValidator(
                new VmOpenApiPhoneChannelInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                }, organizationService, codeService, serviceService, commonService, null, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ValidTo").Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInValidPublishingStatus))]
        public void TimedPublishing_NotValid_PublishingStatus(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new PhoneChannelValidator(
                new VmOpenApiPhoneChannelInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                }, organizationService, codeService, serviceService, commonService, null, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("PublishingStatus").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_NoCurrentVersion_RequiredPropertiesMissing()
        {
            // Arrange
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = "fi" } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, null, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "language";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language },
                    new VmOpenApiLanguageItem { Language = language + "2" }},
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_CurrentVersionExists_NamesMissing()
        {
            // Arrange
            var language = "language";
            var language2 = language + "2";
            var value = "Text";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = language, Value = value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = language, Value = value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = language2, Value = value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = language2, Value = value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() },
                },
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language, Value = value },
                    new VmOpenApiLanguageItem { Language = language2, Value = value }},
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceChannelNames").Should().BeTrue();
        }

        [Fact]
        public void NoLanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var language = "language";
            var value = "Text";
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Language = language, Value = value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() } },
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = language, Value = value } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language } };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceChannelDescriptions").Should().BeTrue();
        }

        [Fact]
        public void RemovingLanguageVersions_CurrentVersionExists_Valid()
        {
            // Arrange
            var language = "language";
            var value = "Text";
            var organizationId = Guid.NewGuid();
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { language });
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = language, Value = value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = language, Value = value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() },
                },
                ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = language, Value = value + " name", Type = NameTypeEnum.Name.GetOpenApiValue() } },
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language } },
                OrganizationId = organizationId.ToString(),
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language, language + "2" } };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RemovingLanguageVersions_CurrentVersionExists_NotValid()
        {
            // Arrange
            var language = "language";
            var value = "Text";
            var organizationId = Guid.NewGuid();
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { language });
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = language, Value = value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = language, Value = value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() },
                },
                PhoneNumbers = new List<V4VmOpenApiPhoneWithType> { new V4VmOpenApiPhoneWithType { Language = language } },
                OrganizationId = organizationId.ToString(),
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceChannel { AvailableLanguages = new List<string> { language, language + "2" } };
            var validator = new PhoneChannelValidator(vm, organizationService,
                codeService, serviceService, commonService, vmCurrentVersion, version);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceChannelNames").Should().BeTrue();
        }

        public static IEnumerable<object[]> GetValidDates()
        {
            yield return new object[] { DateTime.Now, DateTime.Now.AddDays(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMonths(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddYears(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, null, PublishingStatus.Draft };
            yield return new object[] { null, DateTime.Now, PublishingStatus.Published };
        }

        public static IEnumerable<object[]> GetInValidDates()
        {
            yield return new object[] { DateTime.Now.AddDays(1), DateTime.Now, PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddSeconds(10), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMinutes(10), PublishingStatus.Draft };
        }

        public static IEnumerable<object[]> GetInValidPublishingStatus()
        {
            yield return new object[] { DateTime.Now, DateTime.Now.AddDays(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMonths(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, DateTime.Now.AddYears(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, null, PublishingStatus.Published };
            yield return new object[] { null, DateTime.Now, PublishingStatus.Draft };
        }
    }
}
