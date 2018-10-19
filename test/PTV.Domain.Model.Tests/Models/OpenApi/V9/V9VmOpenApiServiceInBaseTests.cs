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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V9
{
    public class V9VmOpenApiServiceInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData("Service", "Chargeable", "Nationwide")]
        [InlineData("PermitOrObligation", "FreeOfCharge", "NationwideExceptAlandIslands")]
        [InlineData("ProfessionalQualification", null, "LimitedType")]
        public void ValidModel(string type, string serviceChargeType, string areaInformationType)
        {
            // Arrange
            var alternateName = "AlternativeName";
            var fi = "fi";
            var service = new V9VmOpenApiServiceInBase
            {
                Type = type,
                ServiceChargeType = serviceChargeType,
                ServiceNames = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Type = alternateName,
                        Language = fi,
                        Value = "Text for name"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Type = NameTypeEnum.Name.ToString(),
                        Language = fi,
                        Value = "Text for name"
                    }
                },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = "Description", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "Summary", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "UserInstruction", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ValidityTime", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ProcessingTime", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "DeadLine", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ChargeTypeAdditionalInfo", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "ServiceType", Language = fi, Value = "Description"},
                },
                AreaType = areaInformationType,
                Areas = areaInformationType == "LimitedType" ? new List<VmOpenApiAreaIn>
                {
                    new VmOpenApiAreaIn{ Type = "Municipality", AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = "Region", AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = "BusinessSubRegion", AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = "HospitalDistrict", AreaCodes = new List<string>{"code" } },
                } : null,
                PublishingStatus = PublishingStatus.Published.ToString(),
                ServiceProducers = new List<V9VmOpenApiServiceProducerIn>
                {
                    new V9VmOpenApiServiceProducerIn
                    {
                        ProvisionType = "SelfProducedServices"
                    },
                    new V9VmOpenApiServiceProducerIn
                    {
                        ProvisionType = "ProcuredServices"
                    },
                    new V9VmOpenApiServiceProducerIn
                    {
                        ProvisionType = "Other"
                    }
                }
            };

            // Act
            var errorList = ValidateModel(service);

            // Assert
            errorList.Count.Should().Be(0);
        }

        [Theory]
        [InlineData(ServiceTypeEnum.Service, ServiceChargeTypeEnum.Free, AreaInformationTypeEnum.WholeCountry)]
        [InlineData(ServiceTypeEnum.PermissionAndObligation, ServiceChargeTypeEnum.Charged, AreaInformationTypeEnum.WholeCountryExceptAlandIslands)]
        [InlineData(ServiceTypeEnum.ProfessionalQualifications, null, AreaInformationTypeEnum.AreaType)]
        public void NotValidModel(ServiceTypeEnum type, ServiceChargeTypeEnum? serviceChargeType, AreaInformationTypeEnum areaInformationType)
        {
            // Arrange
            var fi = "fi";
            var service = new V9VmOpenApiServiceInBase
            {
                Type = type.ToString(),
                ServiceChargeType = serviceChargeType.HasValue ? serviceChargeType.Value.ToString() : null,
                ServiceNames = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Type = NameTypeEnum.AlternateName.ToString(),
                        Language = fi,
                        Value = "Text for name"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Type = NameTypeEnum.Name.ToString(),
                        Language = fi,
                        Value = "Text for name"
                    }
                },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.Description.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ShortDescription.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ServiceUserInstruction.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.DeadLineAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString(), Language = fi, Value = "Description"},
                },
                AreaType = areaInformationType.ToString(),
                Areas = areaInformationType == AreaInformationTypeEnum.AreaType ? new List<VmOpenApiAreaIn>
                {
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.Municipality.ToString(), AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.Province.ToString(), AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.BusinessRegions.ToString(), AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.HospitalRegions.ToString(), AreaCodes = new List<string>{"code" } },
                } : null,
                PublishingStatus = "InvalidPublishingStatus",
                ServiceProducers = new List<V9VmOpenApiServiceProducerIn>
                {
                    new V9VmOpenApiServiceProducerIn
                    {
                        ProvisionType = ProvisionTypeEnum.SelfProduced.ToString()
                    },
                    new V9VmOpenApiServiceProducerIn
                    {
                        ProvisionType = ProvisionTypeEnum.PurchaseServices.ToString()
                    },
                    new V9VmOpenApiServiceProducerIn
                    {
                        ProvisionType = ProvisionTypeEnum.Other.ToString()
                    }
                }
            };

            // Act
            var errorList = ValidateModel(service);

            // Assert
            errorList.Count.Should().BeGreaterThan(0);
            if (type != ServiceTypeEnum.Service) { errorList.FirstOrDefault(r => r.MemberNames.Contains("Type")).Should().NotBeNull(); }
            if (serviceChargeType.HasValue) { errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceChargeType")).Should().NotBeNull(); }
            errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceNames")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceDescriptions")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("AreaType")).Should().NotBeNull();
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                errorList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
            }
            errorList.FirstOrDefault(r => r.MemberNames.Contains("PublishingStatus")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceProducers")).Should().NotBeNull();
        }

        [Fact]
        public void AreasRequiredIfAreaTypeLimitedType()
        {
            // Arrange
            var service = new V9VmOpenApiServiceInBase
            {
                AreaType = "LimitedType"
            };

            // Act
            var errorList = ValidateModel(service);

            // Assert
            errorList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
        }

        [Fact]
        public void CheckProperties()
        {
            var model = new V9VmOpenApiServiceInBase
            {
                Legislation = new List<V4VmOpenApiLaw> { new V4VmOpenApiLaw() },
                ServiceProducers = new List<V9VmOpenApiServiceProducerIn> { new V9VmOpenApiServiceProducerIn() },
                ServiceVouchers = new List<V9VmOpenApiServiceVoucher> { new V9VmOpenApiServiceVoucher() },
            };
            CheckProperties(model, 9);
            CheckProperties(model.Legislation.First(), 4);
            CheckProperties(model.ServiceProducers.First(), 9);
            CheckProperties(model.ServiceVouchers.First(), 9);
        }
    }
}
