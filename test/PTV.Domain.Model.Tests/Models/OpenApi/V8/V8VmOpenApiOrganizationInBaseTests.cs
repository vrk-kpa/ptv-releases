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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V8
{
    public class V8VmOpenApiOrganizationInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData("Nationwide", null)]
        [InlineData("NationwideExceptAlandIslands", null)]
        [InlineData("LimitedType", "Municipality")]
        [InlineData("LimitedType", "BusinessSubRegion")]
        [InlineData("LimitedType", "HospitalDistrict")]
        [InlineData("LimitedType", "Region")]
        public void ValidModel(string areaInformationType, string areaType)
        {
            // Arrange
            var alternateName = "AlternativeName";
            var fi = LanguageCode.fi.ToString();
            var organization = new V8VmOpenApiOrganizationInBase
            {
                OrganizationNames = new List<VmOpenApiLocalizedListItem>
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
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage>
                {
                    new VmOpenApiNameTypeByLanguage
                    {
                        Type = alternateName,
                        Language = fi
                    }
                },
                OrganizationDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{ Type = "Summary", Language = fi, Value = "Description"}
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
                AreaType = areaInformationType,
                SubAreaType = areaType,
                Areas = new List<string> { "area1" },
                PhoneNumbers = new List<V4VmOpenApiPhone>
                {
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = "FreeOfCharge",
                        Language = fi
                    },
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = "Chargeable",
                        Language = fi
                    }
                }
            };            

            // Act
            var resultList = ValidateModel(organization);

            // Assert
            resultList.Count.Should().Be(0);
        }

        [Fact]
        public void AreasRequiredIfAreaTypeLimitedType()
        {
            // Arrange
            var organization = new V8VmOpenApiOrganizationInBase
            {
                AreaType = "LimitedType"
            };

            // Act
            var resultList = ValidateModel(organization);

            // Assert
            resultList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
        }

    }
}
