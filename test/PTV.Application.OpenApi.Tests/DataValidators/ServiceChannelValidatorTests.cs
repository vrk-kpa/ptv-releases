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
using PTV.Domain.Model.Models.Interfaces.OpenApi;

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
                codeService, serviceService, null, null, null, version);

            // Assert
            act.Should().Throw<ArgumentNullException>();
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
                codeService, serviceService, new List<string> { "fi" }, null, null, version);

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
                codeService, serviceService, new List<string> { language }, null, null, version);

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
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString() };
            }

            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, new List<string> { language }, null, currentVersion, version);
            
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
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString() };
            }
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, new List<string> { "fi" }, null, currentVersion, 8);
            

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
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService, codeService, serviceService,
                new List<string> { "fi" }, null, new VmOpenApiServiceChannel { OrganizationId = id }, version);

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
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService, codeService,
                serviceService, new List<string> { language }, null, new VmOpenApiServiceChannel { OrganizationId = id, PublishingStatus = PublishingStatus.Published.ToString() }, version);
            
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
            var currentVersion = new VmOpenApiServiceChannel
            {
                OrganizationId = id,
                PublishingStatus = status == PublishingStatus.Draft ? PublishingStatus.Draft.ToString() : PublishingStatus.Published.ToString()
            };
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, new List<string> { language }, null, currentVersion, version);
            
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
            var currentVersion = new VmOpenApiServiceChannel
            {
                OrganizationId = id,
                PublishingStatus = status == PublishingStatus.Draft ? PublishingStatus.Draft.ToString() : PublishingStatus.Published.ToString()
            };
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, new List<string> { "fi" }, null, currentVersion, 8);

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
                codeService, serviceService, null, null, null, 9);
            
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
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, null, null, currentVersion, 9);
           
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
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, null, null, currentVersion, 9);
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
            IVmOpenApiServiceChannel currentVersion = null;
            if (status != PublishingStatus.Draft)
            {
                currentVersion = new VmOpenApiServiceChannel
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                };
            }
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(vm, "ServiceChannel", organizationService,
                codeService, serviceService, null, null, currentVersion, 8);
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
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(
                new VmOpenApiServiceChannelIn()
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                "ServiceChannel", organizationService, codeService, serviceService, null, null, null, 9);

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
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(
                new VmOpenApiServiceChannelIn()
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                "ServiceChannel", organizationService, codeService, serviceService, null, null, null, 9);

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
            var validator = new ServiceChannelValidator<VmOpenApiServiceChannelIn>(
                new VmOpenApiServiceChannelIn()
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                "ServiceChannel", organizationService, codeService, serviceService, null, null, null, 9);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("PublishingStatus").Should().BeTrue();
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
