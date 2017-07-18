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
using Moq;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System;
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class OidValidatorTests : ValidatorTestBase
    {
        private Mock<IOrganizationService> organizationServiceMockSetup;
        private IOrganizationService organizationService;

        public OidValidatorTests()
        {
            organizationServiceMockSetup = new Mock<IOrganizationService>();
            organizationService = organizationServiceMockSetup.Object;
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new OidValidator(null, organizationService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndCreateOperationAndOidExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdByOid(It.IsAny<string>())).Returns(Guid.NewGuid());
            var validator = new OidValidator("someOid", organizationService, isCreateOperation: true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetAndCreateOperationAndOidNotExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdByOid(It.IsAny<string>())).Returns(Guid.Empty);
            var validator = new OidValidator("someOid", organizationService, isCreateOperation: true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndNotCreateOperationAndOrganizationOrSourceNotSetShouldThrow()
        {
            // Arrange
            Action act = () => new OidValidator("someOid", organizationService);
            
            // Assert
            act.ShouldThrowExactly<ArgumentException>().WithMessage("Either * or * should be given!");
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", null)]
        [InlineData(null, "someSourceId")]
        [InlineData("12345678-1234-1234-1234-123456789012", "someSourceId")]
        public void ModelSetAndNotCreateOperationAndOrganizationIdSetAndOidMappedToOtherOrganization(string organizationId, string sourceId)
        {
            // Arrange
            Guid? guid = null;
            if (!string.IsNullOrEmpty(organizationId)) { guid = new Guid(organizationId); }
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdByOid(It.IsAny<string>())).Returns(Guid.NewGuid());
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdBySource(It.IsAny<string>())).Returns(Guid.NewGuid());
            var validator = new OidValidator("someOid", organizationService, organizationId: guid, sourceId: sourceId);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetAndNotCreateOperationAndOrganizationIdSetAndOidMappedToThisOrganization()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdByOid(It.IsAny<string>())).Returns(guid);
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdBySource(It.IsAny<string>())).Returns(guid);
            var validator = new OidValidator("someOid", organizationService, organizationId: guid);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndNotCreateOperationAndSourceIdSetAndOidMappedToThisOrganization()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdByOid(It.IsAny<string>())).Returns(guid);
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdBySource(It.IsAny<string>())).Returns(guid);
            var validator = new OidValidator("someOid", organizationService, sourceId: "someSourceId");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", null)]
        [InlineData(null, "someSourceId")]
        [InlineData("12345678-1234-1234-1234-123456789012", "someSourceId")]
        public void ModelSetAndNotCreateOperationAndOidNotExists(string organizationId, string sourceId)
        {
            // Arrange
            Guid? guid = null;
            if (!string.IsNullOrEmpty(organizationId)) { guid = new Guid(organizationId); }
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdByOid(It.IsAny<string>())).Returns(Guid.Empty);
            organizationServiceMockSetup.Setup(c => c.GetOrganizationIdBySource(It.IsAny<string>())).Returns(Guid.Empty);
            var validator = new OidValidator("someOid", organizationService, organizationId: guid, sourceId: sourceId);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
