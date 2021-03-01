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
using Xunit;
using Moq;
using PTV.Application.OpenApi.Controllers;
using Microsoft.Extensions.Logging;

using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class V10CommonControllerTests : ControllerTestBase
    {
        private ILogger<V10CommonController> logger;
        private Mock<ITranslationService> translationServiceMockSetup;
        private ITranslationService translationService;
        private Mock<ITasksService> tasksServiceMockSetup;
        private ITasksService tasksService;
        private V10CommonController controller;

        public V10CommonControllerTests()
        {
            var loggerMock = new Mock<ILogger<V10CommonController>>();
            logger = loggerMock.Object;
            translationServiceMockSetup = new Mock<ITranslationService>();
            translationService = translationServiceMockSetup.Object;
            tasksServiceMockSetup = new Mock<ITasksService>();
            tasksService = tasksServiceMockSetup.Object;

            controller = new V10CommonController(commonServiceMockSetup.Object, userService, translationService, tasksService, settings, logger);
        }

        [Fact]
        public void GetServicesAndChannels_ModelIsNull()
        {
            // Act
            Action act = () => controller.GetServicesAndChannels(null, null, null);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void GetServicesAndChannels_OrganizationNotExists()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), PublishingStatus.Published)).Returns(false);

            // Act
            var result = controller.GetServicesAndChannels(Guid.NewGuid().ToString(), null, null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetServicesAndChannels_OrganizationExists()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), PublishingStatus.Published)).Returns(true);
            var page = 1; var pageSize = 10;
            commonServiceMockSetup.Setup(s => s.GetServicesAndChannelsByOrganization(It.IsAny<Guid>(), false, null, page, pageSize, null)).Returns(new VmOpenApiEntityGuidPage());

            // Act
            var result = controller.GetServicesAndChannels(Guid.NewGuid().ToString(), null, null, page);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetTranslationStatistics_CanCall()
        {
            // Act
            var result = controller.GetTranslationStatistics(null, null);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetExpiringServiceChannels_CanCall()
        {
            // Act
            var result = controller.GetExpiringServiceChannels();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetServicesWithoutChannels_CanCall()
        {
            // Act
            var result = controller.GetServicesWithoutChannels();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetChannelsWithoutServices_CanCall()
        {
            // Act
            var result = controller.GetChannelsWithoutServices();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
