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
using Moq;
using PTV.Domain.Model.Models;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class OrganizationValidatorTests : ValidatorTestBase
    {
        private static int _version = 9;

        public OrganizationValidatorTests()
        {}

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new OrganizationValidator(null, null, codeService, organizationService, null,
                null, commonService, _version, true);

            // Assert
            act.Should().Throw<ArgumentNullException>();
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
                null, // current version
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
                null, // current version
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
                null, // current version
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
                null, // current version
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
                null, // current version
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
                null, // current version
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
                null, // current version
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
            commonServiceMockSetup.Setup(x => x.OrganizationExists(It.IsAny<Guid>(), It.IsAny<PublishingStatus>())).Returns(true);
            commonServiceMockSetup.Setup(x => x.OrganizationIsSote(It.IsAny<string>())).Returns(true);
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase
                {
                    ResponsibleOrganizationId = Guid.NewGuid().ToString(),
                    ParentOrganizationId = Guid.NewGuid().ToString(),
                    OrganizationType = organizationType.ToString()
                },
                null, // current version
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
            commonServiceMockSetup.Setup(x => x.OrganizationExists(It.IsAny<Guid>(), It.IsAny<PublishingStatus>())).Returns(true);
            commonServiceMockSetup.Setup(x => x.OrganizationIsSote(It.IsAny<string>())).Returns(false);
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    ResponsibleOrganizationId = Guid.NewGuid().ToString(),
                    OrganizationType = organizationType.ToString()
                },
                null, // current version
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
        public void TimedPublishing_Valid(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                null, // current version
                codeService, organizationService, null, null, commonService, _version, true);

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
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                null, // current version
                codeService, organizationService, null, null, commonService, _version, true);

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
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                null, // current version
                codeService, organizationService, null, null, commonService, _version, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("PublishingStatus").Should().BeTrue();
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Company, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.Company, OrganizationTypeEnum.SotePublic)]
        [InlineData(OrganizationTypeEnum.Municipality, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.Municipality, OrganizationTypeEnum.SotePublic)]
        [InlineData(OrganizationTypeEnum.Organization, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.Organization, OrganizationTypeEnum.SotePublic)]
        [InlineData(OrganizationTypeEnum.Region, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.Region, OrganizationTypeEnum.SotePublic)]
        [InlineData(OrganizationTypeEnum.RegionalOrganization, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.RegionalOrganization, OrganizationTypeEnum.SotePublic)]
        [InlineData(OrganizationTypeEnum.State, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.State, OrganizationTypeEnum.SotePublic)]
        [InlineData(OrganizationTypeEnum.TT1, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.TT1, OrganizationTypeEnum.SotePublic)]
        [InlineData(OrganizationTypeEnum.TT2, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.TT2, OrganizationTypeEnum.SotePublic)]
        public void OrganizationTypeSote_ChangeOrganizationType_NotValid(OrganizationTypeEnum newType, OrganizationTypeEnum currentType)
        {
            // Arrange
            var parentOrgnizationId = Guid.NewGuid();
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = newType.ToString(),
                    ParentOrganizationId = parentOrgnizationId.ToString(),
                    PublishingStatus = PublishingStatus.Published.ToString(),
                },
                new VmOpenApiOrganizationVersionBase { OrganizationType = currentType.ToString(), PublishingStatus = PublishingStatus.Published.ToString() },
                codeService, organizationService, null, null, commonService, _version, true);

            commonServiceMockSetup.Setup(x => x.OrganizationIsSote(OrganizationTypeEnum.SotePrivate.ToString())).Returns(true);
            commonServiceMockSetup.Setup(x => x.OrganizationIsSote(OrganizationTypeEnum.SotePublic.ToString())).Returns(true);
            commonServiceMockSetup.Setup(x => x.OrganizationExists(parentOrgnizationId, PublishingStatus.Published)).Returns(true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OrganizationType").Should().BeTrue();
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.SotePublic, OrganizationTypeEnum.SotePrivate)]
        [InlineData(OrganizationTypeEnum.SotePrivate, OrganizationTypeEnum.SotePublic)]
        public void OrganizationTypeSote_ChangeOrganizationType_Valid(OrganizationTypeEnum newType, OrganizationTypeEnum currentType)
        {
            // Arrange
            var parentOrgnizationId = Guid.NewGuid();
            var validator = new OrganizationValidator(
                new VmOpenApiOrganizationInVersionBase()
                {
                    OrganizationType = newType.ToString(),
                    ParentOrganizationId = parentOrgnizationId.ToString(),
                    PublishingStatus = PublishingStatus.Published.ToString(),
                },
                new VmOpenApiOrganizationVersionBase { OrganizationType = currentType.ToString(), PublishingStatus = PublishingStatus.Published.ToString() },
                codeService, organizationService, null, null, commonService, _version, true);
            commonServiceMockSetup.Setup(x => x.OrganizationExists(It.IsAny<Guid>(), It.IsAny<PublishingStatus>())).Returns(true);
            commonServiceMockSetup.Setup(x => x.OrganizationIsSote(OrganizationTypeEnum.SotePrivate.ToString())).Returns(true);
            commonServiceMockSetup.Setup(x => x.OrganizationIsSote(OrganizationTypeEnum.SotePublic.ToString())).Returns(true);
            commonServiceMockSetup.Setup(x => x.OrganizationExists(parentOrgnizationId, PublishingStatus.Published)).Returns(true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
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
