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
using Xunit;
using System.Collections.Generic;
using Moq;
using PTV.Application.OpenApi.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Models.OpenApi.V2;
using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ConnectionControllerTests : ControllerTestBase
    {
        private ILogger<V7ConnectionController> logger;

        public ConnectionControllerTests()
        {
            var loggerMock = new Mock<ILogger<V7ConnectionController>>();
            logger = loggerMock.Object;
        }

        [Fact]
        public void PostServiceAndChannel_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, userService, serviceService, channelService, commonService, settings, logger);

            // Act
            var result = controller.PostServiceAndChannel(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannel_ModelIsValid()
        {
            // Arrange
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannels(It.IsAny<List<VmOpenApiServiceServiceChannelInVersionBase>>(), It.IsAny<Guid>())).Returns(new List<string>() { "Return message" });
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, userService, serviceService, channelService, commonService, settings, logger);
            var model = new List<V7VmOpenApiServiceAndChannelIn>()
            {
                new V7VmOpenApiServiceAndChannelIn()
                {
                    ServiceId = Guid.NewGuid().ToString(),
                    ServiceChannelId = Guid.NewGuid().ToString(),
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                }
            };

            // Act
            var result = controller.PostServiceAndChannel(model);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var resultOk = result as OkObjectResult;
            resultOk.Value.Should().BeOfType<List<string>>();
            ((List<string>)resultOk.Value).Count.ShouldBeEquivalentTo(1);
        }


        [Fact]
        public void PostServiceAndChannel_ModelIsNotValid()
        {
            // Arrange
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannels(It.IsAny<List<VmOpenApiServiceServiceChannelInVersionBase>>(), It.IsAny<Guid>())).Returns(new List<string>() { "Return message" });
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, userService, serviceService, channelService, commonService, settings, logger);
            controller.ModelState.AddModelError("Validation Error", "Model not valid");
            var model = new List<V7VmOpenApiServiceAndChannelIn>()
            {
                new V7VmOpenApiServiceAndChannelIn()
                {
                    ServiceId = "1",
                    ServiceChannelId = "2",
                    ServiceChargeType = "Test",
                }
            };

            // Act
            var result = controller.PostServiceAndChannel(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

    }
}
