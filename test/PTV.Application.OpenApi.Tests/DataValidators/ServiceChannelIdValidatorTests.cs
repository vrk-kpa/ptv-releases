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
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceChannelIdValidatorTests : ValidatorTestBase
    {
        private Mock<IChannelService> channelServiceMockSetup;
        private IChannelService channelService;

        public ServiceChannelIdValidatorTests()
        {
            channelServiceMockSetup = new Mock<IChannelService>();
            channelService = channelServiceMockSetup.Object;
        }

        [Fact]
        public void ModelIsEmpty()
        {
            // Arrange
            var validator = new ServiceChannelIdValidator(Guid.Empty, channelService, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
        
        [Fact]
        public void ModelSetAndChannelDoesNotExist()
        {
            // Arrange
            channelServiceMockSetup.Setup(c => c.GetServiceChannelById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns((VmOpenApiServiceChannel)null);
            var validator = new ServiceChannelIdValidator(Guid.NewGuid(), channelService, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void ModelAndUserOrganizationSetAndChannelExistsStateValid(bool channelIsVisibleForAll, bool channelOrganizationIsSameAsUserOrganization)
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            channelServiceMockSetup.Setup(c => c.GetServiceChannelById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel()
                {
                    Id = channelId,
                    IsVisibleForAll = channelIsVisibleForAll,
                    OrganizationId = channelOrganizationIsSameAsUserOrganization ? organizationId : Guid.NewGuid()
                });
            var validator = new ServiceChannelIdValidator(channelId, channelService, new List<Guid> { organizationId });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelAndUserOrganizationSetAndChannelExistsStateInValid()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            channelServiceMockSetup.Setup(c => c.GetServiceChannelById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel()
                {
                    Id = channelId,
                    IsVisibleForAll = false,
                    OrganizationId = Guid.NewGuid() // Returned organization is different than user organization
                });
            var validator = new ServiceChannelIdValidator(channelId, channelService, new List<Guid> { Guid.NewGuid() }); // User organization is different than channel organization.

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelAndUserOrganizationSetAndChannelDoesNotExist()
        {
            // Arrange
            channelServiceMockSetup.Setup(c => c.GetServiceChannelById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns((VmOpenApiServiceChannel)null);
            var validator = new ServiceChannelIdValidator(Guid.NewGuid(), channelService, new List<Guid> { Guid.NewGuid() });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
