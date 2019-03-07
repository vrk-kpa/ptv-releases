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
using PTV.Domain.Model.Models.OpenApi.V10;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V10
{
    public class V10VmOpenApiWebPageChannelInBaseTests : ModelsTestBase
    {
        [Fact]
        public void ValidModel()
        {
            // Arrange
            const string fi = "fi";
            var channel = new V10VmOpenApiWebPageChannelInBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = "Description", Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = "Summary", Language = fi, Value = "Description"},
                },
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = fi, Value = "http://test.com" } },
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
            errorList.Count.Should().Be(0);
        }

        [Fact]
        public void NotValidModel()
        {
            // Arrange
            const string fi = "fi";
            var channel = new V10VmOpenApiWebPageChannelInBase
            {
                ServiceChannelDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.Description.ToString(), Language = fi, Value = "Description"},
                    new VmOpenApiLocalizedListItem{ Type = DescriptionTypeEnum.ShortDescription.ToString(), Language = fi, Value = "Description"},
                },
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
            errorList.Count.Should().BeGreaterThan(0);
            errorList.FirstOrDefault(r => r.MemberNames.Contains("ServiceChannelDescriptions")).Should().NotBeNull();
            errorList.FirstOrDefault(r => r.MemberNames.Contains("SupportPhones")).Should().NotBeNull();
        }

        [Fact]
        public void CheckPropertiesTest()
        {
            var model = new V10VmOpenApiWebPageChannelInBase
            {
                SupportPhones = new List<V4VmOpenApiPhone> { new V4VmOpenApiPhone() },
                SupportEmails = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                WebPage = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem() },
                AccessibilityClassification = new List<VmOpenApiAccessibilityClassification> { new VmOpenApiAccessibilityClassification() }
            };
            CheckProperties(model, 10);
            CheckProperties(model.SupportPhones.First(), 4);
            CheckProperties(model.SupportEmails.First(), 8);
            CheckProperties(model.WebPage.First(), 8);
            CheckProperties(model.AccessibilityClassification.First(), 10);
        }
    }
}
