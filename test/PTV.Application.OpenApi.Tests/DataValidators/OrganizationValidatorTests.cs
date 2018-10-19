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
using PTV.Database.DataAccess.Interfaces.Services;
using Moq;
using PTV.Domain.Model.Models;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class OrganizationValidatorTests : ValidatorTestBase
    {
        private static int _version = 9;

        public OrganizationValidatorTests()
        {
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new OrganizationValidator(null, codeService, organizationService, null,
                null, commonService, _version, true);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange
            var validator = new OrganizationValidator
                (new VmOpenApiOrganizationInVersionBase()
                {
                    PublishingStatus = PublishingStatus.Published.ToString()
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OrganizationTypeIsNotMunicipality_MunicipalityNotNull()
        {
            // Arrange
            var validator = new OrganizationValidator
                (new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = OrganizationTypeEnum.Company.ToString(),
                    Municipality = "municipality_code"
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void OrganizationTypeIsMunicipality_MunicipalityNotNull()
        {
            // Arrange
            var code = "municipality_code";
            codeServiceMockSetup.Setup(s => s.GetMunicipalityByCode(code, true)).Returns(new VmListItem() { Id = Guid.NewGuid() });
            var validator = new OrganizationValidator
                (new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = OrganizationTypeEnum.Municipality.ToString(),
                    Municipality = code
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Municipality, AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(OrganizationTypeEnum.Municipality, AreaInformationTypeEnum.AreaType, null)]
        [InlineData(OrganizationTypeEnum.Municipality, null, AreaTypeEnum.BusinessRegions)]
        [InlineData(OrganizationTypeEnum.Municipality, null, AreaTypeEnum.HospitalRegions)]
        [InlineData(OrganizationTypeEnum.Municipality, null, AreaTypeEnum.Municipality)]
        [InlineData(OrganizationTypeEnum.Municipality, null, AreaTypeEnum.Province)]
        [InlineData(OrganizationTypeEnum.TT1, AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(OrganizationTypeEnum.TT1, AreaInformationTypeEnum.AreaType, null)]
        [InlineData(OrganizationTypeEnum.TT1, null, AreaTypeEnum.BusinessRegions)]
        [InlineData(OrganizationTypeEnum.TT1, null, AreaTypeEnum.HospitalRegions)]
        [InlineData(OrganizationTypeEnum.TT1, null, AreaTypeEnum.Municipality)]
        [InlineData(OrganizationTypeEnum.TT1, null, AreaTypeEnum.Province)]
        [InlineData(OrganizationTypeEnum.TT2, AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(OrganizationTypeEnum.TT2, AreaInformationTypeEnum.AreaType, null)]
        [InlineData(OrganizationTypeEnum.TT2, null, AreaTypeEnum.BusinessRegions)]
        [InlineData(OrganizationTypeEnum.TT2, null, AreaTypeEnum.HospitalRegions)]
        [InlineData(OrganizationTypeEnum.TT2, null, AreaTypeEnum.Municipality)]
        [InlineData(OrganizationTypeEnum.RegionalOrganization, AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(OrganizationTypeEnum.RegionalOrganization, AreaInformationTypeEnum.WholeCountry, null)]
        [InlineData(null, AreaInformationTypeEnum.WholeCountry, null)]
        [InlineData(null, AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(null, AreaInformationTypeEnum.AreaType, null)]
        [InlineData(null, null, AreaTypeEnum.BusinessRegions)]
        [InlineData(null, null, AreaTypeEnum.HospitalRegions)]
        [InlineData(null, null, AreaTypeEnum.Municipality)]
        [InlineData(null, null, AreaTypeEnum.Province)]
        [InlineData(OrganizationTypeEnum.Company, AreaInformationTypeEnum.WholeCountry, AreaTypeEnum.BusinessRegions)]
        public void OrganizationType_AreaTypeOrSubAreaType_NotValid(OrganizationTypeEnum? type, AreaInformationTypeEnum? areaType, AreaTypeEnum? subArea)
        {
            // Arrange
            var validator = new OrganizationValidator
                (new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = type.HasValue ? type.Value.ToString() : null,
                    AreaType = areaType.HasValue ? areaType.Value.ToString() : null,
                    SubAreaType = subArea.HasValue ? subArea.Value.ToString() : null,
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Municipality)]
        [InlineData(OrganizationTypeEnum.TT1)]
        [InlineData(OrganizationTypeEnum.TT2)]
        [InlineData(null)]
        public void OrganizationType_AreasSet_NotValid(OrganizationTypeEnum? type)
        {
            // Arrange
            var validator = new OrganizationValidator
                (new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = type.HasValue ? type.Value.ToString() : null,
                    Areas = new List<string> { "areaCode" }
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void InvalidAreaType_With_Areas()
        {
            // Arrange
            var validator = new OrganizationValidator
                (new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = OrganizationTypeEnum.Company.ToString(), // State, Organization or Company 
                    AreaType = AreaInformationTypeEnum.WholeCountry.ToString(), // something else than AreaType
                    Areas = new List<string> { "areaCode" }
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Municipality, AreaInformationTypeEnum.WholeCountry)]
        [InlineData(OrganizationTypeEnum.TT1, AreaInformationTypeEnum.WholeCountry)]
        [InlineData(OrganizationTypeEnum.TT2, AreaInformationTypeEnum.WholeCountry)]
        [InlineData(OrganizationTypeEnum.RegionalOrganization, AreaInformationTypeEnum.AreaType)]
        public void OrganizationType_AreaType_Valid(OrganizationTypeEnum type, AreaInformationTypeEnum areType)
        {
            // Arrange
            var validator = new OrganizationValidator
                (new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = type.ToString(),
                    AreaType = areType.ToString()
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.SotePublic)]
        public void ResponsibleOrganizationSet_Valid(OrganizationTypeEnum organizationType)
        {
            // Arrange
            commonServiceMockSetup.Setup(x => x.OrganizationExists(It.IsAny<Guid>(), It.IsAny<PublishingStatus>())).
                Returns(true);
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    ResponsibleOrganizationId = Guid.NewGuid().ToString(),
                    OrganizationType = organizationType.ToString()
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(OrganizationTypeEnum.Company)]
        [InlineData(OrganizationTypeEnum.Municipality)]
        [InlineData(OrganizationTypeEnum.Organization)]
        [InlineData(OrganizationTypeEnum.Region)]
        [InlineData(OrganizationTypeEnum.RegionalOrganization)]
        [InlineData(OrganizationTypeEnum.State)]
        [InlineData(OrganizationTypeEnum.TT1)]
        [InlineData(OrganizationTypeEnum.TT2)]
        public void ResponsibleOrganizationSet_NotValid(OrganizationTypeEnum? organizationType)
        {
            // Arrange
            commonServiceMockSetup.Setup(x => x.OrganizationExists(It.IsAny<Guid>(), It.IsAny<PublishingStatus>())).
                Returns(true);
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    ResponsibleOrganizationId = Guid.NewGuid().ToString(),
                    OrganizationType = organizationType.ToString()
                },
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            if (organizationType.HasValue)
            {
                controller.ModelState.ContainsKey("ResponsibleOrganizationId").Should().BeTrue();
            }
            else
            {
                controller.ModelState.ContainsKey("OrganizationType").Should().BeTrue();
            }
        }

        [Theory]
        [MemberData(nameof(GetValidDates))]
        public void TimedPublishing_Valid(DateTime? validFrom, DateTime? validTo)
        {
            // Arrange
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                codeService, organizationService, null, null, commonService, _version, true);

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
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                codeService, organizationService, null, null, commonService, _version, true);

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
