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
using System.Linq;
using Xunit;
using System.Reflection;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V7
{
    public class V7VmOpenApiServiceInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData(ServiceTypeEnum.Service, ServiceChargeTypeEnum.Free, AreaInformationTypeEnum.WholeCountry)]
        [InlineData(ServiceTypeEnum.PermissionAndObligation, ServiceChargeTypeEnum.Charged, AreaInformationTypeEnum.WholeCountryExceptAlandIslands)]
        [InlineData(ServiceTypeEnum.ProfessionalQualifications, null, AreaInformationTypeEnum.AreaType)]
        public void ValidModel(ServiceTypeEnum type, ServiceChargeTypeEnum? serviceChargeType, AreaInformationTypeEnum areaInformationType)
        {
            // Arrange
            const string fi = "fi";
            var service = new V7VmOpenApiServiceInBase
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
                PublishingStatus = PublishingStatus.Published.ToString(),
                ServiceProducers = new List<VmOpenApiServiceProducerIn>
                {
                    new VmOpenApiServiceProducerIn
                    {
                        ProvisionType = ProvisionTypeEnum.SelfProduced.ToString()
                    },
                    new VmOpenApiServiceProducerIn
                    {
                        ProvisionType = ProvisionTypeEnum.PurchaseServices.ToString()
                    },
                    new VmOpenApiServiceProducerIn
                    {
                        ProvisionType = ProvisionTypeEnum.Other.ToString()
                    }
                }
            };

            // Act
            var errorList = ValidateModel(service);

            // Assert
            errorList.Count.Should().Be(0);

            // Make sure GeneralDescriptionId is shown as statutoryServiceGeneralDescriptionId
            var gd = typeof(V7VmOpenApiServiceInBase).GetProperties().SelectMany(p => p.GetCustomAttributes<JsonPropertyAttribute>()).FirstOrDefault(jp => jp.PropertyName == "statutoryServiceGeneralDescriptionId");
            gd.Should().NotBeNull();

            // Make sure DeleteGeneralDescriptionId is shown as deleteStatutoryServiceGeneralDescriptionId
            var dgd = typeof(V7VmOpenApiServiceInBase).GetProperties().SelectMany(p => p.GetCustomAttributes<JsonPropertyAttribute>()).FirstOrDefault(jp => jp.PropertyName == "deleteStatutoryServiceGeneralDescriptionId");
            dgd.Should().NotBeNull();
        }

        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V7VmOpenApiServiceInBase
            {
                Legislation = new List<V4VmOpenApiLaw> { new V4VmOpenApiLaw() },
                ServiceProducers = new List<VmOpenApiServiceProducerIn> { new VmOpenApiServiceProducerIn() },
                ServiceVouchers = new List<VmOpenApiServiceVoucher> { new VmOpenApiServiceVoucher() },
            };
            CheckProperties(model, 7);
            CheckProperties(model.Legislation.First(), 4);
            CheckProperties(model.ServiceProducers.First(), 8);
            CheckProperties(model.ServiceVouchers.First(), 8);
        }
    }
}
