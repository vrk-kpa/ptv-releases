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
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceCollectionValidatorTests : ValidatorTestBase
    {
        private int _openApiVersion = 11;

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceCollectionValidator(null, serviceService, new List<string> { "language" }, organizationService, _openApiVersion);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange & act
            var validator = new ServiceCollectionValidator(new VmOpenApiServiceCollectionInVersionBase { PublishingStatus = PublishingStatus.Published.ToString() },
                serviceService, null, organizationService, _openApiVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        public void OrganizationNotValid_MainResponsibleOrganization(int openApiVersion)
        {
            // Arrange & act
            Guid organizationId = Guid.NewGuid();
            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns((List<string>)null);
            var validator = new ServiceCollectionValidator(new VmOpenApiServiceCollectionInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                OrganizationId = organizationId.ToString()
            }, serviceService, null, organizationService, openApiVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("MainResponsibleOrganization").Should().BeTrue();
        }

        [Theory]
        [InlineData(11)]
        public void OrganizationNotValid_OrganizationId(int openApiVersion)
        {
            // Arrange & act
            Guid organizationId = Guid.NewGuid();
            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns((List<string>)null);
            var validator = new ServiceCollectionValidator(new VmOpenApiServiceCollectionInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                OrganizationId = organizationId.ToString()
            }, serviceService, null, organizationService, openApiVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OrganizationId").Should().BeTrue();
        }
    }
}
