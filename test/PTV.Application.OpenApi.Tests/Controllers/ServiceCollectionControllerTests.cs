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

using System;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Database.DataAccess.Interfaces.Services.Security;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ServiceCollectionControllerTests : ControllerTestBase
    {
        private ILogger<V7ServiceCollectionController> logger;

        private Mock<IServiceCollectionService> serviceCollectionServiceMockSetup;
        private Mock<IUserOrganizationService> userOrganizationServiceMockSetup;

        private int version;
        private Guid id;
        private string strId;
        private int pageNumber = 1;

        public ServiceCollectionControllerTests()
        {
            logger = (new Mock<ILogger<V7ServiceCollectionController>>()).Object;


            serviceCollectionServiceMockSetup = new Mock<IServiceCollectionService>();
            userOrganizationServiceMockSetup = new Mock<IUserOrganizationService>();

            version = 7;
            pageSize = 1000;
            id = Guid.NewGuid();
            strId = id.ToString();
        }

        #region GET

        [Fact]
        public void Get_ServiceCollections_CanCall()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollections(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object, 
                commonService, serviceService, userOrganizationServiceMockSetup.Object, settings, logger);

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
        public void Get_IdIsInvalid(string id)
        {
            // Arrange
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userOrganizationServiceMockSetup.Object, settings, logger);

            // Act
            Action act = () => controller.Get(id);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Get_ServiceCollectionNotExists()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionById(It.IsAny<Guid>(), defaultVersion, It.IsAny<bool>())).Returns((V7VmOpenApiServiceCollection)null);
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userOrganizationServiceMockSetup.Object, settings, logger);

            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_ServiceCollectionExists()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(new V7VmOpenApiServiceCollection());
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userOrganizationServiceMockSetup.Object, settings, logger);

            // Act
            var result = controller.Get(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V7VmOpenApiServiceCollection>(okResult.Value);
        }

        #endregion

        #region POST/PUT

        [Fact]
        public void PostServiceCollection_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userOrganizationServiceMockSetup.Object, settings, logger);

            // Act
            var result = controller.Post(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostServiceCollection_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userOrganizationServiceMockSetup.Object, settings, logger);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.Post(new V7VmOpenApiServiceCollectionIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostServiceCollection_RequestIsValid()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.AddServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), false, It.IsAny<int>(), null))
                .Returns(new V7VmOpenApiServiceCollection());
            userServiceMockSetup.Setup(s => s.UserHighestRole()).Returns(UserRoleEnum.Eeva);
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.Post(new V7VmOpenApiServiceCollectionIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiServiceCollection>(okResult.Value);
        }

        [Fact]
        public void PostServiceCollection_ServiceCollectionServiceThrowsException()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.AddServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), false, It.IsAny<int>(), null)).Throws<Exception>();
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            Action act = () => controller.Post(new V7VmOpenApiServiceCollectionIn());

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void PutServiceCollection_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

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
        public void PutServiceCollection_ServiceCollectionIdNotValid(string serviceCollectionId)
        {
            // Arrange
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.Put(serviceCollectionId, new V7VmOpenApiServiceCollectionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutServiceCollection_ServiceCollectionNotExists()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.ServiceCollectionExists(id)).Returns(false);
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.Put(strId, new V7VmOpenApiServiceCollectionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        // TODO: Test below:

        [Fact]
        public void PutServiceCollection_CanModifyServiceCollection()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.ServiceCollectionExists(id)).Returns(true);
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionById(id, 0, false))
                .Returns(new V7VmOpenApiServiceCollection() { PublishingStatus = PublishingStatus.Published.ToString() });
            serviceCollectionServiceMockSetup.Setup(s => s.SaveServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), It.IsAny<bool>(), It.IsAny<int>(), null, null))
                .Returns(new V7VmOpenApiServiceCollection());
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.Put(strId, new V7VmOpenApiServiceCollectionInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiServiceCollection>(okResult.Value);
        }

        [Fact]
        public void PutServiceCollectionBySource_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.PutBySource("sourceId", null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutServiceCollectionBySource_SourceIdNotValid(string sourceId)
        {
            // Arrange
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.PutBySource(sourceId, new V7VmOpenApiServiceCollectionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutServiceCollectionBySource_SourceIdNotFound()
        {
            // Arrange
            var sourceId = "sourceId";
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionBySource(sourceId, 0, false, null)).Returns((V7VmOpenApiServiceCollection)null);
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.PutBySource(sourceId, new V7VmOpenApiServiceCollectionInBase());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PutServiceCollectionBySource_CanModifyServiceCollection()
        {
            // Arrange
            var sourceId = "sourceId";
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionBySource(sourceId, 0, false, null))
                .Returns(new V7VmOpenApiServiceCollection { PublishingStatus = PublishingStatus.Published.ToString() });
            serviceCollectionServiceMockSetup.Setup(s => s.SaveServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), It.IsAny<bool>(), version, sourceId, null))
                .Returns(new V7VmOpenApiServiceCollection());
            var controller = new V7ServiceCollectionController(serviceCollectionServiceMockSetup.Object,
                commonService, serviceService, userService, settings, logger);

            // Act
            var result = controller.PutBySource(sourceId, new V7VmOpenApiServiceCollectionInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiServiceCollection>(okResult.Value);
        }

        #endregion
    }
}
