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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;

namespace PTV.Domain.Model.Tests.Models.OpenApi.V4
{
    public class V4VmOpenApiServiceInBaseTests : ModelsTestBase
    {
        [Theory]
        [InlineData(ServiceTypeEnum.Service, ServiceChargeTypeEnum.Free)]
        [InlineData(ServiceTypeEnum.PermissionAndObligation, ServiceChargeTypeEnum.Charged)]
        public void ValidModel(ServiceTypeEnum type, ServiceChargeTypeEnum serviceChargeType)
        {
            // Arrange
            var fi = LanguageCode.fi.ToString();
            var service = new V4VmOpenApiServiceInBase
            {
                Type = type.ToString(),
                ServiceChargeType = serviceChargeType.ToString(),
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
                PublishingStatus = PublishingStatus.Published.ToString(),
                ServiceOrganizations = new List<V4VmOpenApiServiceOrganization>
                {
                    new V4VmOpenApiServiceOrganization
                    {
                        RoleType = "Responsible",
                        ProvisionType = ProvisionTypeEnum.SelfProduced.ToString()
                    },
                    new V4VmOpenApiServiceOrganization
                    {
                        RoleType = "Producer",
                        ProvisionType = ProvisionTypeEnum.PurchaseServices.ToString()
                    },
                    new V4VmOpenApiServiceOrganization
                    {
                        RoleType = "Producer",
                        ProvisionType = ProvisionTypeEnum.Other.ToString()
                    }
                }
            };

            // Act
            var errorList = ValidateModel(service);

            // Assert
            errorList.Count.Should().Be(0);

            // Make sure GeneralDescriptionId is shown as statutoryServiceGeneralDescriptionId
            var property = typeof(V4VmOpenApiServiceInBase).GetProperties().SelectMany(p => p.GetCustomAttributes<JsonPropertyAttribute>()).FirstOrDefault(jp => jp.PropertyName == "statutoryServiceGeneralDescriptionId");
            property.Should().NotBeNull();
        }
    }
}
