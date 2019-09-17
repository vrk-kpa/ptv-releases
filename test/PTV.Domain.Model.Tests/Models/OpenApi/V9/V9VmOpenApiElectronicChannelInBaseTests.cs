﻿/**
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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V9
{
    public class V9VmOpenApiElectronicChannelInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData("Nationwide")]
        [InlineData("NationwideExceptAlandIslands")]
        [InlineData("LimitedType")]
        public void ValidModel(string areaInformationType)
        {
            // Arrange
            const string fi = "fi";
            var channel = new V9VmOpenApiElectronicChannelInBase
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
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = fi, Value = "http://test.com" } },
                SupportPhones = new List<V4VmOpenApiPhone> {
                    new V4VmOpenApiPhone{ ServiceChargeType = "Chargeable", Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = "FreeOfCharge", Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = "Other", Language = fi }
                },
                ServiceHours = new List<V8VmOpenApiServiceHour>
                {
                    new V8VmOpenApiServiceHour{ServiceHourType = "DaysOfTheWeek"},
                    new V8VmOpenApiServiceHour{ServiceHourType = "Exceptional"},
                    new V8VmOpenApiServiceHour{ServiceHourType = "OverMidnight"},
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
            const string fi = "fi";
            var channel = new V9VmOpenApiElectronicChannelInBase
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
                SupportPhones = new List<V4VmOpenApiPhone> {
                    new V4VmOpenApiPhone { ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(), Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = ServiceChargeTypeEnum.Free.ToString(), Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = ServiceChargeTypeEnum.Other.ToString(), Language = fi },
                },
                ServiceHours = new List<V8VmOpenApiServiceHour>
                {
                    new V8VmOpenApiServiceHour{ServiceHourType = ServiceHoursTypeEnum.Exception.ToString()},
                    new V8VmOpenApiServiceHour{ServiceHourType = ServiceHoursTypeEnum.Special.ToString()},
                    new V8VmOpenApiServiceHour{ServiceHourType = ServiceHoursTypeEnum.Standard.ToString()},
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
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
            errorList.FirstOrDefault(r => r.MemberNames.Contains("SupportPhones")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceHours")).Should().NotBeNull();
        }

        [Fact]
        public void AreasRequiredIfAreaTypeLimitedType()
        {
            // Arrange
            var channel = new V9VmOpenApiElectronicChannelInBase
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
            var model = new V9VmOpenApiElectronicChannelInBase
            {
                Attachments = new List<VmOpenApiAttachment> { new VmOpenApiAttachment() },
                SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                ServiceHours = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour { OpeningHour = new List<V8VmOpenApiDailyOpeningTime> { new V8VmOpenApiDailyOpeningTime() } } },
                AccessibilityStatementWebPage = new List<V9VmOpenApiWebPage> { new V9VmOpenApiWebPage() }
            };
            CheckProperties(model, 9);
            CheckProperties(model.Attachments.First(), 8);
            CheckProperties(model.SupportPhones.First(), 4);
            CheckProperties(model.SupportEmails.First(), 8);
            CheckServiceHourProperties(model.ServiceHours.First(), 8);
            CheckProperties(model.AccessibilityStatementWebPage.First(), 9);
        }
    }
}
