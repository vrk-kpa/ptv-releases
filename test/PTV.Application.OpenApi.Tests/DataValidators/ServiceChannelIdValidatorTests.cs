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
using Moq;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Security;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceChannelIdValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsEmpty()
        {
            // Arrange
            var validator = new ServiceChannelIdValidator(Guid.Empty, channelService, UserRoleEnum.Eeva, "PropertyName");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(Guid.Empty, true), Times.Once());
        }

        [Fact]
        public void ModelSet_ChannelDoesNotExist()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            channelServiceMockSetup.Setup(c => c.GetServiceChannelByIdSimple(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns((VmOpenApiServiceChannel)null);
            var validator = new ServiceChannelIdValidator(channelId, channelService, UserRoleEnum.Eeva, "PropertyName");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once());
        }

        [Theory]
        [InlineData(true, true, UserRoleEnum.Eeva)]
        [InlineData(true, true, UserRoleEnum.Pete)]
        [InlineData(true, false, UserRoleEnum.Eeva)]
        [InlineData(true, false, UserRoleEnum.Pete)]
        [InlineData(false, true, UserRoleEnum.Eeva)]
        [InlineData(false, true, UserRoleEnum.Pete)]
        [InlineData(false, false, UserRoleEnum.Eeva)]
        public void ModelAndUserOrganizationSet_ChannelExists_StateValid(bool channelIsVisibleForAll, bool channelOrganizationIsSameAsUserOrganization, UserRoleEnum role)
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            channelServiceMockSetup.Setup(c => c.GetServiceChannelByIdSimple(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel
                {
                    Id = channelId,
                    IsVisibleForAll = channelIsVisibleForAll,
                    Security = new VmSecurityOwnOrganization { IsOwnOrganization = channelOrganizationIsSameAsUserOrganization}
                });
            var validator = new ServiceChannelIdValidator(channelId, channelService, role, "PropertyName");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once());
        }

        [Fact]
        public void ModelAndUserOrganizationSet_ChannelExists_StateInValid()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            channelServiceMockSetup.Setup(c => c.GetServiceChannelByIdSimple(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel
                {
                    Id = channelId,
                    IsVisibleForAll = false,
                    Security = new VmSecurityOwnOrganization { IsOwnOrganization = false }
                });
            var validator = new ServiceChannelIdValidator(channelId, channelService, UserRoleEnum.Pete, "PropertyName");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once);
        }
    }
}
