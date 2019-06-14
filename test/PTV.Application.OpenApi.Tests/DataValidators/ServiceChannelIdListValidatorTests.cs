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
using PTV.Domain.Model.Models;
using System.Collections.Generic;
using System;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Domain.Model.Models.Security;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceChannelIdListValidatorTests : ValidatorTestBase
    {
        //private Mock<IChannelService> channelServiceMockSetup;
        //private IChannelService channelService;

        public ServiceChannelIdListValidatorTests()
        {
            channelServiceMockSetup = new Mock<IChannelService>();
            channelService = channelServiceMockSetup.Object;
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceChannelIdListValidator(null, channelService, UserRoleEnum.Eeva);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelListIncludesEmpty()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            channelServiceMockSetup.Setup(c => c.GetServiceChannelByIdSimple(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns((Guid id, bool onlyPublished) =>
                {
                    if (!id.IsAssigned()) return null;
                    return new VmOpenApiServiceChannel()
                    {
                        Id = id,
                        IsVisibleForAll = false,
                        OrganizationId = organizationId
                    };
                });
            var list = new List<Guid>() { Guid.Empty, channelId };
            var validator = new ServiceChannelIdListValidator(list, channelService, UserRoleEnum.Eeva); // new List <Guid> { organizationId });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(Guid.Empty, true), Times.Once());
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once());
        }

        [Fact]
        public void ModelListIncludesChannelForOtherOrganization()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var invalidChannelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            channelServiceMockSetup.Setup(c => c.GetServiceChannelByIdSimple(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns((Guid id, bool onlyPublished) =>
                {
                    if (!id.IsAssigned()) return null;
                    return new VmOpenApiServiceChannel()
                    {
                        Id = id,
                        IsVisibleForAll = false,
                        OrganizationId = id == channelId ? organizationId : Guid.NewGuid(), // Returned organization is different than user organization if the channel id is invalid
                        Security = new VmSecurityOwnOrganization { IsOwnOrganization = false }
                    };
                });            

            var list = new List<Guid>() { channelId, invalidChannelId };
            var validator = new ServiceChannelIdListValidator(list, channelService, UserRoleEnum.Pete);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once());
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(invalidChannelId, true), Times.Once());
        }
    }
}
