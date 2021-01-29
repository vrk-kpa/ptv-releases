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
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System.Collections.Generic;
using Moq;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using System;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceProducerListValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceProducerListValidator(null, null, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ProvisionTypeIsSelfProduced_OrganizationNotInAvailableOrganizations()
        {
            // Arrange
            var validator = new ServiceProducerListValidator(new List<V9VmOpenApiServiceProducerIn>
            {
                new V9VmOpenApiServiceProducerIn
                {
                    ProvisionType = ProvisionTypeEnum.SelfProduced.ToString(),
                    Organizations = new List<Guid>{ Guid.NewGuid()}
                }
            }, new List<Guid> { Guid.NewGuid() }, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ProvisionTypeIsSelfProduced_Valid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var list = new List<Guid> { id };
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, PublishingStatus.Published)).Returns(true);
            var validator = new ServiceProducerListValidator(new List<V9VmOpenApiServiceProducerIn>
            {
                new V9VmOpenApiServiceProducerIn
                {
                    ProvisionType = ProvisionTypeEnum.SelfProduced.ToString(),
                    Organizations = list
                }
            }, list, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ProvisionTypeIsSelfProduced_AdditionalInformationSet()
        {
            // Arrange
            var validator = new ServiceProducerListValidator(new List<V9VmOpenApiServiceProducerIn>
            {
                new V9VmOpenApiServiceProducerIn
                {
                    ProvisionType = ProvisionTypeEnum.SelfProduced.ToString(),
                    AdditionalInformation = new List<VmOpenApiLanguageItem>{ new VmOpenApiLanguageItem { Value = "test" } }
                }
            }, null, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(ProvisionTypeEnum.Other)]
        [InlineData(ProvisionTypeEnum.PurchaseServices)]
        public void ProvisionTypeIsSet_NoOrganizationOrAdditionalInformationSet(ProvisionTypeEnum provisionType)
        {
            // Arrange
            var validator = new ServiceProducerListValidator(new List<V9VmOpenApiServiceProducerIn>
            {
                new V9VmOpenApiServiceProducerIn
                {
                    ProvisionType = provisionType.ToString(),
                }
            }, null, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(ProvisionTypeEnum.Other, true)]
        [InlineData(ProvisionTypeEnum.Other, false)]
        [InlineData(ProvisionTypeEnum.PurchaseServices, true)]
        [InlineData(ProvisionTypeEnum.PurchaseServices, false)]
        public void ValidModel(ProvisionTypeEnum provisionType, bool organizationsSet)
        {
            // Arrange
            var id = Guid.NewGuid();
            var list = new List<Guid> { id };
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, PublishingStatus.Published)).Returns(true);
            var validator = new ServiceProducerListValidator(new List<V9VmOpenApiServiceProducerIn>
            {
                new V9VmOpenApiServiceProducerIn
                {
                    ProvisionType = provisionType.ToString(),
                    Organizations = organizationsSet ? list : null,
                    AdditionalInformation = !organizationsSet ? new List<VmOpenApiLanguageItem>{ new VmOpenApiLanguageItem { Value = "test" } } : null
                }
            }, list, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidModel_OrganizationNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var list = new List<Guid> { id };
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, PublishingStatus.Published)).Returns(false);
            var validator = new ServiceProducerListValidator(new List<V9VmOpenApiServiceProducerIn>
            {
                new V9VmOpenApiServiceProducerIn
                {
                    ProvisionType = ProvisionTypeEnum.SelfProduced.ToString(),
                    Organizations = list
                }
            }, list, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
