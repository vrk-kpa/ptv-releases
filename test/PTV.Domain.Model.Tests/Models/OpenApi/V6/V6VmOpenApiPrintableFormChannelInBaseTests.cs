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
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V6
{
    public class V6VmOpenApiPrintableFormChannelInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData(AreaInformationTypeEnum.WholeCountry)]
        [InlineData(AreaInformationTypeEnum.WholeCountryExceptAlandIslands)]
        [InlineData(AreaInformationTypeEnum.AreaType)]
        public void ValidModel(AreaInformationTypeEnum areaInformationType)
        {
            // Arrange
            const string fi = "fi";
            var channel = new V6VmOpenApiPrintableFormChannelInBase
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
                PublishingStatus = PublishingStatus.Published.ToString(),
            };


            // Act
            var errorList = ValidateModel(channel);

            // Assert
            errorList.Count.Should().Be(0);
        }

        [Theory]
        [InlineData("Nationwide")]
        [InlineData("NationwideExceptAlandIslands")]
        [InlineData("LimitedType")]
        public void NotValidModel(string areaInformationType)
        {
            // Arrange
            const string fi = "fi";
            var channel = new V6VmOpenApiPrintableFormChannelInBase
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
                ChannelUrls = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Language = fi, Value = "http://test.com" } },
                SupportPhones = new List<V4VmOpenApiPhone> {
                    new V4VmOpenApiPhone{ ServiceChargeType = "Chargeable", Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = "FreeOfCharge", Language = fi },
                    new V4VmOpenApiPhone { ServiceChargeType = "Other", Language = fi }
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };


            // Act
            var errorList = ValidateModel(channel);

            // Assert
            errorList.Count.Should().BeGreaterThan(0);
            errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceChannelDescriptions")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("AreaType")).Should().NotBeNull();
            if (areaInformationType == "LimitedType")
            {
                errorList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
            }
            errorList.FirstOrDefault(r => r.MemberNames.Contains("SupportPhones")).Should().NotBeNull();
        }

        [Fact]
        public void AreasRequiredIfAreaTypeLimitedType()
        {
            // Arrange
            var channel = new V6VmOpenApiPrintableFormChannelInBase
            {
                AreaType = "AreaType"
            };

            // Act
            var errorList = ValidateModel(channel);

            // Assert
            errorList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
        }

        [Fact]
        public void CheckProperties()
        {
            var model = new V6VmOpenApiPrintableFormChannelInBase
            {
                DeliveryAddress = new V5VmOpenApiAddressIn(),
                ChannelUrls = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem() },
                Attachments = new List<VmOpenApiAttachment> { new VmOpenApiAttachment() },
                SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
            };
            CheckProperties(model, 6);
            CheckProperties(model.DeliveryAddress, 5);
            CheckListItemProperties(model.ChannelUrls.First(), 8);
            CheckProperties(model.Attachments.First(), 8);
            CheckProperties(model.SupportEmails.First(), 8);
            CheckProperties(model.SupportPhones.First(), 4);
        }
    }
}
