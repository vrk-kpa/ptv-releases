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
using Moq;
using PTV.Application.OpenApi.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V6;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class GeneralDescriptionControllerTests : ControllerTestBase
    {
        private ILogger<V7GeneralDescriptionController> logger;

        private Mock<IFintoService> fintoServiceMock;
        private IFintoService fintoService;

        private Guid id;
        private string strId;

        public GeneralDescriptionControllerTests()
        {
            logger = (new Mock<ILogger<V7GeneralDescriptionController>>()).Object;

            fintoServiceMock = new Mock<IFintoService>();
            fintoService = fintoServiceMock.Object;

            id = Guid.NewGuid();
            strId = id.ToString();
        }

        [Fact]
        public void GetGeneralDescriptions_CanCall()
        {
            // Arrange
            var pageNumber = 1;
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptions(null,  pageNumber, It.IsAny<int>())).Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Get(null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void Get_IdIsNull(string gdId)
        {
            // Arrange
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            Action act = () => controller.Get(gdId);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void Get_GeneralDescriptionNotExists()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, defaultVersion, It.IsAny<bool>())).Returns((VmOpenApiGeneralDescriptionVersionBase)null);
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_GeneralDescriptionExists()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, defaultVersion, It.IsAny<bool>())).Returns(new VmOpenApiGeneralDescriptionVersionBase());
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Get(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiGeneralDescriptionVersionBase>(okResult.Value);
        }

        [Fact]
        public void Post_RequestIsNull()
        {
            // Arrange
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Post(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.Post(new V6VmOpenApiGeneralDescriptionIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_RequestIsValid()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.AddGeneralDescription(It.IsAny<VmOpenApiGeneralDescriptionInVersionBase>(), false, defaultVersion))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase());
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Post(new V6VmOpenApiGeneralDescriptionIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<VmOpenApiGeneralDescriptionVersionBase>(okResult.Value);
        }

        [Fact]
        public void Post_serviceThrowsException()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.AddGeneralDescription(It.IsAny<VmOpenApiGeneralDescriptionInVersionBase>(), false, defaultVersion))
                .Throws<Exception>();
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            Action act = () => controller.Post(new V6VmOpenApiGeneralDescriptionIn());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void Put_RequestIsNull()
        {
            // Arrange
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Put(strId, null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void Put_IdNotValid(string gdId)
        {
            // Arrange
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Put(gdId, new V6VmOpenApiGeneralDescriptionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_GeneralDescriptionNotExists()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GeneralDescriptionExists(id)).Returns(false);
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Put(strId, new V6VmOpenApiGeneralDescriptionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_CurrentVersionNotFound()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GeneralDescriptionExists(id)).Returns(true);
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, 0, false))
                .Returns((VmOpenApiGeneralDescriptionVersionBase)null);
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Put(strId, new V6VmOpenApiGeneralDescriptionInBase());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Put_CanModifyGeneralDescription()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GeneralDescriptionExists(id)).Returns(true);
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, 0, false))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase { PublishingStatus = PublishingStatus.Published.ToString() });
            gdServiceMockSetup.Setup(s => s.SaveGeneralDescription(It.IsAny<VmOpenApiGeneralDescriptionInVersionBase>(), defaultVersion))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase());
            var controller = new V7GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService, userService, logger, settings);

            // Act
            var result = controller.Put(strId, new V6VmOpenApiGeneralDescriptionInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<VmOpenApiGeneralDescriptionVersionBase>(okResult.Value);
        }

    }
}
