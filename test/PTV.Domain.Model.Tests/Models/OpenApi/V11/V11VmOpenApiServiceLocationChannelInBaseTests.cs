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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V11
{
    public class V11VmOpenApiServiceLocationChannelInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData("Nationwide")]
        [InlineData("NationwideExceptAlandIslands")]
        [InlineData("LimitedType")]
        public void ValidModel(string areaInformationType)
        {
            // Arrange
            var fi = "fi";
            var channel = new V11VmOpenApiServiceLocationChannelInBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = "Description", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "Summary", Language = fi, Value = "Description"},
                },
                AreaType = areaInformationType,
                Areas = areaInformationType == "LimitedType" ? new List<VmOpenApiAreaIn>
                {
                    new VmOpenApiAreaIn{ Type = "Municipality", AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = "Region", AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = "BusinessSubRegion", AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = "HospitalDistrict", AreaCodes = new List<string>{"code" } },
                } : null,
                PhoneNumbers = new List<V4VmOpenApiPhone> {
                    new V4VmOpenApiPhone{ ServiceChargeType = "Chargeable", Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = "FreeOfCharge", Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = "Other", Language = fi }
                },
                ServiceHours = new List<V11VmOpenApiServiceHour>
                {
                    new V11VmOpenApiServiceHour{ServiceHourType = "DaysOfTheWeek"},
                    new V11VmOpenApiServiceHour{ServiceHourType = "Exceptional"},
                    new V11VmOpenApiServiceHour{ServiceHourType = "OverMidnight"},
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };


            // Act
            var errorList = ValidateModel(channel);

            // Assert
            errorList.Count.Should().Be(0);
        }

        [Theory]
        [InlineData(AreaInformationTypeEnum.WholeCountry)]
        [InlineData(AreaInformationTypeEnum.WholeCountryExceptAlandIslands)]
        [InlineData(AreaInformationTypeEnum.AreaType)]
        public void NotValidModel(AreaInformationTypeEnum areaInformationType)
        {
            // Arrange
            var fi = "fi";
            var channel = new V11VmOpenApiServiceLocationChannelInBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.Description.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ShortDescription.ToString(), Language = fi, Value = "Description"},
                },
                AreaType = areaInformationType.ToString(),
                Areas = areaInformationType == AreaInformationTypeEnum.AreaType ? new List<VmOpenApiAreaIn>
                {
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.Municipality.ToString(), AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.Province.ToString(), AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.BusinessRegions.ToString(), AreaCodes = new List<string>{"code" } },
                    new VmOpenApiAreaIn{ Type = AreaTypeEnum.HospitalRegions.ToString(), AreaCodes = new List<string>{"code" } },
                } : null,
                PhoneNumbers = new List<V4VmOpenApiPhone> {
                    new V4VmOpenApiPhone { ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(), Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = ServiceChargeTypeEnum.Free.ToString(), Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = ServiceChargeTypeEnum.Other.ToString(), Language = fi },
                },
                ServiceHours = new List<V11VmOpenApiServiceHour>
                {
                    new V11VmOpenApiServiceHour{ServiceHourType = ServiceHoursTypeEnum.Exception.ToString()},
                    new V11VmOpenApiServiceHour{ServiceHourType = ServiceHoursTypeEnum.Special.ToString()},
                    new V11VmOpenApiServiceHour{ServiceHourType = ServiceHoursTypeEnum.Standard.ToString()},
                },
                PublishingStatus = "InValidPublishingStatus",
            };


            // Act
            var errorList = ValidateModel(channel);

            // Assert
            errorList.Count.Should().BeGreaterThan(0);
            errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceChannelDescriptions")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("AreaType")).Should().NotBeNull();
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                errorList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
            }
            errorList.FirstOrDefault(r => r.MemberNames.Contains("PhoneNumbers")).Should().NotBeNull();
            //errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceHours")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("PublishingStatus")).Should().NotBeNull();
        }

        [Fact]
        public void AreasRequiredIfAreaTypeLimitedType()
        {
            // Arrange
            var channel = new V11VmOpenApiServiceLocationChannelInBase
            {
                AreaType = "LimitedType"
            };

            // Act
            var errorList = ValidateModel(channel);

            // Assert
            errorList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
        }

        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V11VmOpenApiServiceLocationChannelInBase
            {
                FaxNumbers = new List<V4VmOpenApiPhoneSimple> { new V4VmOpenApiPhoneSimple() },
                PhoneNumbers = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                WebPages = new List<V9VmOpenApiWebPage> { new V9VmOpenApiWebPage() },
                Addresses = new List<V9VmOpenApiAddressLocationIn> { new V9VmOpenApiAddressLocationIn() },
                ServiceHours = new List<V11VmOpenApiServiceHour> { new V11VmOpenApiServiceHour { OpeningHour = new List<V8VmOpenApiDailyOpeningTime> { new V8VmOpenApiDailyOpeningTime() } } },
            };
            CheckProperties(model, 11);
            CheckProperties(model.FaxNumbers.First(), 4);
            CheckProperties(model.PhoneNumbers.First(), 4);
            CheckProperties(model.SupportEmails.First());
            CheckProperties(model.WebPages.First(), 9);
            CheckProperties(model.Addresses.First(), 9);
            CheckServiceHourProperties(model.ServiceHours.First(), 11);
        }
    }
}
