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
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ServiceCollectionControllerTests : ControllerTestBase
    {
        private ILogger<V11ServiceCollectionController> logger;

        private Mock<IServiceCollectionService> serviceCollectionServiceMockSetup;
        private Mock<IUserOrganizationService> userOrganizationServiceMockSetup;

        private int version;
        private Guid id;
        private string strId;
        private int pageNumber = 1;

        private V11ServiceCollectionController controller;

        public ServiceCollectionControllerTests()
        {
            logger = new Mock<ILogger<V11ServiceCollectionController>>().Object;


            serviceCollectionServiceMockSetup = new Mock<IServiceCollectionService>();
            userOrganizationServiceMockSetup = new Mock<IUserOrganizationService>();

            userOrganizationServiceMockSetup.Setup(x => x.GetAllUserOrganizationIds(null)).Returns(new List<Guid>());

            version = 11;
            pageSize = 1000;
            id = Guid.NewGuid();
            strId = id.ToString();

            controller = new V11ServiceCollectionController(serviceCollectionServiceMockSetup.Object, organizationService,
                commonService, serviceService, channelService, userOrganizationServiceMockSetup.Object, settings, logger);
        }

        #region GET

        [Fact]
        public void Get_ServiceCollections_CanCall()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollections(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.Get(null, null, pageNumber);

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
            // Act
            Action act = () => controller.Get(id);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Get_ServiceCollectionNotExists()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionById(It.IsAny<Guid>(), defaultVersion, It.IsAny<bool>(), false)).Returns((V7VmOpenApiServiceCollection)null);

            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_ServiceCollectionExists()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), false)).Returns(new V7VmOpenApiServiceCollection());

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
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.Post(new V11VmOpenApiServiceCollectionIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostServiceCollection_RequestIsValid()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.AddServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), It.IsAny<int>(), null))
                .Returns(new V11VmOpenApiServiceCollection());
            userServiceMockSetup.Setup(s => s.UserHighestRole()).Returns(UserRoleEnum.Eeva);

            // Act
            var result = controller.Post(new V11VmOpenApiServiceCollectionIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V11VmOpenApiServiceCollection>(okResult.Value);
        }

        [Fact]
        public void PostServiceCollection_ServiceCollectionServiceThrowsException()
        {
            // Arrange
            serviceCollectionServiceMockSetup.Setup(s => s.AddServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), It.IsAny<int>(), null)).Throws<Exception>();

            // Act
            Action act = () => controller.Post(new V11VmOpenApiServiceCollectionIn());

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void PutServiceCollection_RequestIsNull()
        {
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
            // Act
            var result = controller.Put(serviceCollectionId, new V11VmOpenApiServiceCollectionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutServiceCollection_ServiceCollectionNotExists()
        {
            // Arrange
//            serviceCollectionServiceMockSetup.Setup(s => s.ServiceCollectionExists(id)).Returns(false);

            // Act
            var result = controller.Put(strId, new V11VmOpenApiServiceCollectionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        // TODO: Test below:

        [Fact]
        public void PutServiceCollection_CanModifyServiceCollection()
        {
            // Arrange
//            serviceCollectionServiceMockSetup.Setup(s => s.ServiceCollectionExists(id)).Returns(true);
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionById(id, 0, false, false))
                .Returns(new V7VmOpenApiServiceCollection { PublishingStatus = PublishingStatus.Published.ToString() });
            serviceCollectionServiceMockSetup.Setup(s => s.SaveServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), It.IsAny<int>(), null, null))
                .Returns(new V7VmOpenApiServiceCollection());

            // Act
            var result = controller.Put(strId, new V11VmOpenApiServiceCollectionInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiServiceCollection>(okResult.Value);
        }

        [Fact]
        public void PutServiceCollectionBySource_RequestIsNull()
        {
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
            // Act
            var result = controller.PutBySource(sourceId, new V11VmOpenApiServiceCollectionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutServiceCollectionBySource_SourceIdNotFound()
        {
            // Arrange
            var sourceId = "sourceId";
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionBySource(sourceId, 0, false, null, false)).Returns((V7VmOpenApiServiceCollection)null);

            // Act
            var result = controller.PutBySource(sourceId, new V11VmOpenApiServiceCollectionInBase());

            // Assert
            var badResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(badResult.Value);
        }

        [Fact]
        public void PutServiceCollectionBySource_CanModifyServiceCollection()
        {
            // Arrange
            var sourceId = "sourceId";
            serviceCollectionServiceMockSetup.Setup(s => s.GetServiceCollectionBySource(sourceId, 0, false, null, false))
                .Returns(new V11VmOpenApiServiceCollection { PublishingStatus = PublishingStatus.Published.ToString() });
            serviceCollectionServiceMockSetup.Setup(s => s.SaveServiceCollection(It.IsAny<VmOpenApiServiceCollectionInVersionBase>(), version, sourceId, null))
                .Returns(new V11VmOpenApiServiceCollection());

            // Act
            var result = controller.PutBySource(sourceId, new V11VmOpenApiServiceCollectionInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V11VmOpenApiServiceCollection>(okResult.Value);
        }

        #endregion
    }
}
