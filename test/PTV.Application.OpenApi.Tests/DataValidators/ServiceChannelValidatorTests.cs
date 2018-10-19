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
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Enums;
using System.Collections.Generic;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceChannelValidatorTests : ValidatorTestBase
    {
        private int version = 9;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceChannelValidator<VmOpenApiServiceChannelIn>(null, "ServiceChannel", organizationService,
                codeService, serviceService, version);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        // Related organization tests

        [Fact]
        public void RelatedOrganizationAvailableLanguages_OrganizationSet_Published_V9_NotValid()
        {
            // Arrange & act
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceChannelIn
            {
                OrganizationId = id.ToString(),
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, version);
            validator.AvailableLanguages = new List<string> { "fi" };

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
            var vm = new VmOpenApiServiceChannelIn
            {
                OrganizationId = id.ToString(),
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, version);
            validator.AvailableLanguages = new List<string> { language };

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
            var vm = new VmOpenApiServiceChannelIn
            {
                OrganizationId = id.ToString(),
                PublishingStatus = status.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, version);
            validator.AvailableLanguages = new List<string> { language };
            if (status != PublishingStatus.Draft)
            {
                validator.CurrentVersion = new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString() };
            }

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
            var vm = new VmOpenApiServiceChannelIn
            {
                OrganizationId = id.ToString(),
                PublishingStatus = status.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, 8);
            validator.AvailableLanguages = new List<string> { "fi" };
            if (status != PublishingStatus.Draft)
            {
                validator.CurrentVersion = new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString() };
            }

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
            var vm = new VmOpenApiServiceChannelIn
            {
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService, codeService, serviceService, version);
            validator.AvailableLanguages = new List<string> { "fi" };
            validator.CurrentVersion = new VmOpenApiServiceChannel { OrganizationId = id };

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
            var vm = new VmOpenApiServiceChannelIn
            {
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService, codeService, serviceService, version);
            validator.AvailableLanguages = new List<string> { language };
            validator.CurrentVersion = new VmOpenApiServiceChannel { OrganizationId = id, PublishingStatus = PublishingStatus.Published.ToString() };

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
            var vm = new VmOpenApiServiceChannelIn
            {
                PublishingStatus = status.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string> { language });
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, version);
            validator.AvailableLanguages = new List<string> { language };
            validator.CurrentVersion = new VmOpenApiServiceChannel
            {
                OrganizationId = id,
                PublishingStatus = status == PublishingStatus.Draft ? PublishingStatus.Draft.ToString() : PublishingStatus.Published.ToString()
            };

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
            var vm = new VmOpenApiServiceChannelIn
            {
                PublishingStatus = status.ToString()
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(new List<string>());
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, 8);
            validator.AvailableLanguages = new List<string> { "fi" };
            validator.CurrentVersion = new VmOpenApiServiceChannel
            {
                OrganizationId = id,
                PublishingStatus = status == PublishingStatus.Draft ? PublishingStatus.Draft.ToString() : PublishingStatus.Published.ToString()
            };

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
            var vm = new VmOpenApiServiceChannelIn
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
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, 9);
            
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
            var vm = new VmOpenApiServiceChannelIn
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
                PublishingStatus = status.ToString()
            };
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, 9);
           if (status != PublishingStatus.Draft)
            {
                validator.CurrentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
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
            var vm = new VmOpenApiServiceChannelIn
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
                    Type = DescriptionTypeEnum.ShortDescription.ToString()
                } },
                PublishingStatus = status.ToString()
            };
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, 9);
            if (status != PublishingStatus.Draft)
            {
                validator.CurrentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
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
            var vm = new VmOpenApiServiceChannelIn
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
                PublishingStatus = status.ToString()
            };
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, 8);
            if (status != PublishingStatus.Draft)
            {
                validator.CurrentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
        
        [Theory]
        [MemberData(nameof(GetValidDates))]
        public void TimedPublishing_Valid(DateTime? validFrom, DateTime? validTo)
        {
            // Arrange
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(
                new VmOpenApiServiceChannelIn()
                {
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                "ServiceChannel", organizationService, codeService, serviceService, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInValidDates))]
        public void TimedPublishing_NotValid(DateTime? validFrom, DateTime? validTo)
        {
            // Arrange
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(
                new VmOpenApiServiceChannelIn()
                {
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                "ServiceChannel", organizationService, codeService, serviceService, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ValidTo").Should().BeTrue();
        }

        private static IEnumerable<object[]> GetValidDates()
        {
            yield return new object[] { DateTime.Now, DateTime.Now.AddDays(1) };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMonths(1) };
            yield return new object[] { DateTime.Now, DateTime.Now.AddYears(1) };
            yield return new object[] { DateTime.Now, null };
            yield return new object[] { null, DateTime.Now };
        }

        private static IEnumerable<object[]> GetInValidDates()
        {
            yield return new object[] { DateTime.Now.AddDays(1), DateTime.Now };
            yield return new object[] { DateTime.Now, DateTime.Now.AddSeconds(10) };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMinutes(10) };
        }
    }
}
