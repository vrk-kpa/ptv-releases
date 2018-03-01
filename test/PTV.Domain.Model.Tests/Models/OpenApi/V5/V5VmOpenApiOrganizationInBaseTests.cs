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
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Reflection;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V5
{
    public class V5VmOpenApiOrganizationInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData(AreaInformationTypeEnum.WholeCountry, null)]
        [InlineData(AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Municipality)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.BusinessRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.HospitalRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Province)]
        public void ValidModel(AreaInformationTypeEnum areaInformationType, AreaTypeEnum? areaType)
        {
            // Arrange
            var alternateName = "AlternateName";
            var fi = LanguageCode.fi.ToString();
            var organization = new V5VmOpenApiOrganizationInBase
            {
                OrganizationNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem
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
                    } },
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage> { new VmOpenApiNameTypeByLanguage
                {
                    Type = alternateName,
                    Language = fi
                } },
                PublishingStatus = PublishingStatus.Published.ToString(),
                AreaType = areaInformationType.ToString(),
                SubAreaType = areaType.HasValue ? areaType.Value.ToString() : null,
                Areas = new List<string> { "area1" },
                PhoneNumbers = new List<V4VmOpenApiPhone>
                {
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = ServiceChargeTypeEnum.Free.ToString(),
                        Language = fi
                    },
                    new V4VmOpenApiPhone
                    {
                        ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                        Language = fi
                    }
                },
                Emails = new List<V4VmOpenApiEmail>()
            };

            // Act
            var resultList = ValidateModel(organization);

            // Assert
            resultList.Count.Should().Be(0);
            // Make sure Emails is shown as EmailAddresses
            var emails = typeof(V5VmOpenApiOrganizationInBase).GetProperties().SelectMany(p => p.GetCustomAttributes<JsonPropertyAttribute>()).FirstOrDefault(jp => jp.PropertyName == "emailAddresses");
            emails.Should().NotBeNull();
        }

        [Fact]
        public void AreasRequiredIfAreaTypeLimitedType()
        {
            // Arrange
            var organization = new V5VmOpenApiOrganizationInBase
            {
                AreaType = AreaInformationTypeEnum.AreaType.ToString()
            };

            // Act
            var resultList = ValidateModel(organization);

            // Assert
            resultList.FirstOrDefault(r => r.MemberNames.Contains("Areas")).Should().NotBeNull();
        }
    }
}
