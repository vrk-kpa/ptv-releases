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
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ServiceControllerTests : ControllerTestBase
    {
        private ILogger<V7ServiceController> logger;
        
        private Mock<IFintoService> fintoServiceMock;
        private IFintoService fintoService;

        private Guid id;
        private string strId;
        private int pageNumber = 1;

        public ServiceControllerTests()
        {
            logger = (new Mock<ILogger<V7ServiceController>>()).Object;
            
            fintoServiceMock = new Mock<IFintoService>();
            fintoService = fintoServiceMock.Object;

            id = Guid.NewGuid();
            strId = id.ToString();
        }

        [Fact]
        public void Get_Services_CanCall()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

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
        public void Get_IdNotValid(string id)
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

            // Act
            Action act = () => controller.Get(id);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void Get_ServiceNotExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>())).Returns((VmOpenApiServiceVersionBase)null);
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_ServiceExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>())).Returns(new VmOpenApiServiceVersionBase());
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

            // Act
            var result = controller.Get(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
        }

        [Fact]
        public void GetActive_Services_CanCall()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

            // Act
            var result = controller.GetActive(null, pageNumber);

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
        public void GetActive_IdNotValid(string id)
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

            // Act
            Action act = () => controller.GetActive(id);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetActive_ServiceNotExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>())).Returns((VmOpenApiServiceVersionBase)null);
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

            // Act
            var result = controller.GetActive(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetActive_ServiceExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>())).Returns(new VmOpenApiServiceVersionBase());
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);

            // Act
            var result = controller.GetActive(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GetByServiceChannel_ChannelIdNotValid(string id)
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelService, userService, logger);
                        
            // Act
            Action act = () => controller.GetByServiceChannel(id, null, pageNumber);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetByServiceChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.ChannelExists(id)).Returns(false);
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.GetByServiceChannel(strId, null, pageNumber);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByServiceChannel_ChannelExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.ChannelExists(id)).Returns(true);
            serviceServiceMockSetup.Setup(s => s.GetServicesByServiceChannel(id, null, pageNumber, pageSize))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.GetByServiceChannel(strId, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Fact]
        public void GetByServiceClass_ClassNotExists()
        {
            var uri = "invalid_uri";
            fintoServiceMock.Setup(s => s.GetServiceClassByUri(uri)).Returns((VmOpenApiFintoItemVersionBase)null);
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.GetByServiceClass(uri, null, pageNumber);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByServiceClass_ClassExists()
        {
            var uri = "invalid_uri";
            fintoServiceMock.Setup(s => s.GetServiceClassByUri(uri)).Returns(new VmOpenApiFintoItemVersionBase() { Id = id });
            serviceServiceMockSetup.Setup(s => s.GetServicesByServiceClass(id, null, pageNumber, pageSize))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.GetByServiceClass(uri, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Fact]
        public void GetByMunicipality_ModelIsNull()
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.GetByMunicipality(null, null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByMunicipality_MunicipalityNotExists()
        {
            // Arrange
            codeServiceMockSetup.Setup(s => s.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns((VmListItem)null);
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.GetByMunicipality("code", null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByMunicipality_MunicipalityExists()
        {
            // Arrange
            var municipalityId = Guid.NewGuid();
            codeServiceMockSetup.Setup(s => s.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns(new VmListItem { Id = municipalityId });
            var page = 1; var pageSize = 10;
            serviceServiceMockSetup.Setup(s => s.GetServicesByMunicipality(municipalityId, null, page, pageSize)).Returns(new VmOpenApiEntityGuidPage(page, pageSize));
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.GetByMunicipality(municipalityId.ToString(), null, page);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void Post_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.Post(null, false);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.Post(new V7VmOpenApiServiceIn(), false);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_RequestIsValid()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.AddService(It.IsAny<VmOpenApiServiceInVersionBase>(), false, defaultVersion, false))
                .Returns(new V7VmOpenApiService());
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);
            
            // Act
            var result = controller.Post(new V7VmOpenApiServiceIn(), false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiService>(okResult.Value);
        }

        [Fact]
        public void Post_serviceServiceThrowsException()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.AddService(It.IsAny<VmOpenApiServiceInVersionBase>(), false, defaultVersion, false)).Throws<Exception>();
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            Action act = () => controller.Post(new V7VmOpenApiServiceIn(), false);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void Put_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.Put(strId, null, false);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void Put_ServiceIdNotValid(string serviceId)
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.Put(serviceId, new V7VmOpenApiServiceInBase(), false);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_ServiceNotExists()
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.Put(strId, new V7VmOpenApiServiceInBase(), false);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_CurrentVersionNotFound()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(id, false)).Returns((VmOpenApiServiceVersionBase)null);
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.Put(strId, new V7VmOpenApiServiceInBase(), false);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_CanModifyService()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(It.IsAny<Guid>(), false))
                .Returns(new VmOpenApiServiceVersionBase { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            serviceServiceMockSetup.Setup(s => s.SaveService(It.IsAny<VmOpenApiServiceInVersionBase>(), It.IsAny<bool>(), defaultVersion, false, null))
                .Returns(new V7VmOpenApiService());
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.Put(strId, new V7VmOpenApiServiceInBase(), false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiService>(okResult.Value);
        }

        [Fact]
        public void PutBySource_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.PutBySource("sourceId", null, false);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutBySource_SourceIdNotValid(string sourceId)
        {
            // Arrange
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.PutBySource(sourceId, new V7VmOpenApiServiceInBase(), false);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutBySource_SourceIdNotFound()
        {
            // Arrange
            var sourdeId = "sourceId";
            serviceServiceMockSetup.Setup(s => s.GetServiceBySource(sourdeId)).Returns((VmOpenApiServiceVersionBase)null);
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.PutBySource(sourdeId, new V7VmOpenApiServiceInBase(), false);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutBySource_CanModifyService()
        {
            // Arrange
            var sourdeId = "sourceId";
            serviceServiceMockSetup.Setup(s => s.GetServiceBySource(sourdeId))
                .Returns(new VmOpenApiServiceVersionBase(){ PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            serviceServiceMockSetup.Setup(s => s.SaveService(It.IsAny<VmOpenApiServiceInVersionBase>(), It.IsAny<bool>(), defaultVersion, false, sourdeId))
                .Returns(new V7VmOpenApiService());
            var controller = new V7ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoServiceMock.Object, serviceAndChannelService, channelServiceMockSetup.Object, userService, logger);

            // Act
            var result = controller.PutBySource(sourdeId, new V7VmOpenApiServiceInBase(), false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiService>(okResult.Value);
        }
    }
}
