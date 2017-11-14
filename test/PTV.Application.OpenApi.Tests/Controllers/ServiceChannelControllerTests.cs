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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ServiceChannelControllerTests : ControllerTestBase
    {
        private ILogger<V7ServiceChannelController> logger;

        private IFintoService fintoService;

        private Guid id;
        private string strId;

        public ServiceChannelControllerTests()
        {
            var loggerMock = new Mock<ILogger<V7ServiceChannelController>>();
            logger = loggerMock.Object;

            fintoService = (new Mock<IFintoService>()).Object;

            id = Guid.NewGuid();
            strId = id.ToString();
        }

        #region GET
        [Fact]
        public void Get_Channels_CanCall()
        {
            // Arrange
            var pageNumber = 1;
            channelServiceMockSetup.Setup(s => s.GetServiceChannels(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.Get(null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
            channelServiceMockSetup.Verify(x => x.GetServiceChannels(null, pageNumber, It.IsAny<int>(), false, false), Times.Once);
        }
        [Fact]
        public void Get_Channels_CanCall_Archived()
        {
            // Arrange
            var pageNumber = 1;
            channelServiceMockSetup.Setup(s => s.GetServiceChannels(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.Get(null, pageNumber, true);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
            channelServiceMockSetup.Verify(x => x.GetServiceChannels(null, pageNumber, 1000, true, false), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void Get_IdIsInvalid(string id)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.Get(id);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void Get_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelById(It.IsAny<Guid>(), defaultVersion, VersionStatusEnum.Published)).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelById(id, defaultVersion, VersionStatusEnum.Published), Times.Once);
        }

        [Fact]
        public void Get_ChannelExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelById(It.IsAny<Guid>(), defaultVersion, VersionStatusEnum.Published)).Returns(new VmOpenApiServiceChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.Get(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelById(id, defaultVersion, VersionStatusEnum.Published), Times.Once);
        }

        [Fact]
        public void GetActive_Channels_CanCallWithoutType()
        {
            // Arrange
            var pageNumber = 1;
            channelServiceMockSetup.Setup(s => s.GetServiceChannels(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetActive(null, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannels(null, pageNumber, It.IsAny<int>(), false, true), Times.Once);
        }

        [Fact]
        public void GetActive_Channels_TypeNotValid()
        {
            // Arrange
            var pageNumber = 1;
            var type = "invalid_type";
            channelServiceMockSetup.Setup(s => s.GetServiceChannels(null, pageNumber, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetActive(null, type, pageNumber);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetActive_Channels_TypeValid()
        {
            // Arrange
            var pageNumber = 1;
            var channelType = ServiceChannelTypeEnum.Phone;
            channelServiceMockSetup.Setup(s => s.GetServiceChannelsByType(channelType, null, pageNumber, pageSize, It.IsAny<bool>())).Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetActive(null, channelType.ToString(), pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelsByType(channelType, null, pageNumber, It.IsAny<int>(), false), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GetActive_IdIsInvalid(string id)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.GetActive(id);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetActive_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetActive(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelById(id, defaultVersion, VersionStatusEnum.LatestActive), Times.Once);
        }

        [Fact]
        public void GetActive_ChannelExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>())).Returns(new VmOpenApiServiceChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetActive(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelById(id, defaultVersion, VersionStatusEnum.LatestActive), Times.Once);
        }

        [Fact]
        public void GetByType_CanCall()
        {
            // Arrange
            var pageNumber = 1;
            var channelType = ServiceChannelTypeEnum.Phone;
            channelServiceMockSetup.Setup(s => s.GetServiceChannelsByType(channelType, null, pageNumber, pageSize, It.IsAny<bool>())).Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetByType(channelType.ToString(), null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
        }

        [Fact]
        public void GetByType_TypeIsInValid()
        {
            // Arrange
            var pageNumber = 1;
            var type = "invalid_type";
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);
                        
            // Act
            Action act = () => controller.GetByType(type, null, pageNumber);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GetByOrganizationId_OrganizationIdIsInValid(string id)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.GetByOrganizationId(id, null, 1);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetByOrganizationId_OrganizationNotExists()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, It.IsAny<PublishingStatus>())).Returns(false);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonServiceMockSetup.Object, userService);

            // Act
            var result = controller.GetByOrganizationId(strId, null, 1);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByOrganizationId_OrganizationExists()
        {
            // Arrange
            var pageNumber = 1;
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, It.IsAny<PublishingStatus>())).Returns(true);
            channelServiceMockSetup.Setup(s => s.GetServiceChannelsByOrganization(id, null, pageNumber, pageSize, null)).Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetByOrganizationId(strId, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GetByOrganizationIdAndType_OrganizationIdIsInValid(string id)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.GetByOrganizationIdAndType(id, null, null, 1);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetByOrganizationIdAndType_OrganizationNotExists()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, It.IsAny<PublishingStatus>())).Returns(false);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonServiceMockSetup.Object, userService);

            // Act
            var result = controller.GetByOrganizationIdAndType(strId, null, null, 1);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByOrganizationIdAndType_TypeIsInValid()
        {
            // Arrange
            var pageNumber = 1;
            var type = "invalid_type";
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, It.IsAny<PublishingStatus>())).Returns(true);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonServiceMockSetup.Object, userService);

            // Act
            Action act = () => controller.GetByOrganizationIdAndType(strId, type, null, pageNumber);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetByOrganizationIdAndTyped_OrganizationExistsAndTypeValid()
        {
            // Arrange
            var pageNumber = 1;
            var channelType = ServiceChannelTypeEnum.Phone;
            commonServiceMockSetup.Setup(s => s.OrganizationExists(id, It.IsAny<PublishingStatus>())).Returns(true);
            channelServiceMockSetup.Setup(s => s.GetServiceChannelsByOrganization(id, null, pageNumber, pageSize, channelType)).Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetByOrganizationIdAndType(strId, channelType.ToString(), null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
        }

        [Fact]
        public void GetByMunicipality_ModelIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

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
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

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
            var page = 1;
            channelServiceMockSetup.Setup(s => s.GetServiceChannelsByMunicipality(municipalityId, null, page, pageSize)).Returns(new V3VmOpenApiGuidPage(page, pageSize));
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.GetByMunicipality(municipalityId.ToString(), null, page);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
        }

        #endregion

        #region POST/PUT electronic
        [Fact]
        public void PostEChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostEChannel(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostEChannel_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.PostEChannel(new V6VmOpenApiElectronicChannelIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostEChannel_RequestIsValid()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiElectronicChannelInVersionBase>(), false, defaultVersion, null))
                .Returns(new V7VmOpenApiElectronicChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostEChannel(new V6VmOpenApiElectronicChannelIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiElectronicChannel>(okResult.Value);
        }

        [Fact]
        public void PostEChannel_ChannelServiceThrowsException()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiElectronicChannelInVersionBase>(), false, defaultVersion, null)).Throws<Exception>();
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.PostEChannel(new V6VmOpenApiElectronicChannelIn());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void PutEChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannel(strId, null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void PutEChannel_ChannelIdNotValid(string channelId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannel(channelId, new V6VmOpenApiElectronicChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutEChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannel(strId, new V6VmOpenApiElectronicChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutEChannel_CurrentVersionNotFound()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannel(strId, new V6VmOpenApiElectronicChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutEChannel_CanModifyChannel()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel() { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiElectronicChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiElectronicChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannel(strId, new V6VmOpenApiElectronicChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiElectronicChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutEChannelBySource_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannelBySource("sourceId", null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutEChannelBySource_SourceIdNotValid(string sourceId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannelBySource(sourceId, new V6VmOpenApiElectronicChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutEChannelBySource_SourceIdNotFound()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId)).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannelBySource(sourceId, new V6VmOpenApiElectronicChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }

        [Fact]
        public void PutEChannelBySource_CanModifyChannel()
        {
            // Arrange
            var sourdeId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourdeId))
                .Returns(new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiElectronicChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiElectronicChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutEChannelBySource(sourdeId, new V6VmOpenApiElectronicChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiElectronicChannel>(okResult.Value);
        }
        #endregion

        #region POST/PUT phone
        [Fact]
        public void PostPhoneChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostPhoneChannel(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostPhoneChannel_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.PostPhoneChannel(new V7VmOpenApiPhoneChannelIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostPhoneChannel_RequestIsValid()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiPhoneChannelInVersionBase>(), false, defaultVersion, null))
                .Returns(new V7VmOpenApiPhoneChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostPhoneChannel(new V7VmOpenApiPhoneChannelIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiPhoneChannel>(okResult.Value);
        }

        [Fact]
        public void PostPhoneChannel_ChannelServiceThrowsException()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiPhoneChannelInVersionBase>(), false, defaultVersion, null)).Throws<Exception>();
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.PostPhoneChannel(new V7VmOpenApiPhoneChannelIn());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void PutPhoneChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannel(strId, null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void PutPhoneChannel_ChannelIdNotValid(string channelId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannel(channelId, new V7VmOpenApiPhoneChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutPhoneChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannel(strId, new V7VmOpenApiPhoneChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutPhoneChannel_CurrentVersionNotFound()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannel(strId, new V7VmOpenApiPhoneChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutPhoneChannel_CanModifyChannel()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.ChannelExists(id)).Returns(true);
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel() { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiPhoneChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiPhoneChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannel(strId, new V7VmOpenApiPhoneChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiPhoneChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutPhoneChannelBySource_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannelBySource("sourceId", null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutPhoneChannelBySource_SourceIdNotValid(string sourceId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannelBySource(sourceId, new V7VmOpenApiPhoneChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutPhoneChannelBySource_SourceIdNotFound()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId)).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannelBySource(sourceId, new V7VmOpenApiPhoneChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }

        [Fact]
        public void PutPhoneChannelBySource_CanModifyChannel()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId))
                .Returns(new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiPhoneChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiPhoneChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPhoneChannelBySource(sourceId, new V7VmOpenApiPhoneChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiPhoneChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }
        #endregion

        #region POST/PUT web page
        [Fact]
        public void PostWebPageChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostWebPageChannel(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostWebPageChannel_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.PostWebPageChannel(new V6VmOpenApiWebPageChannelIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostWebPageChannel_RequestIsValid()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiWebPageChannelInVersionBase>(), false, defaultVersion, null))
                .Returns(new V7VmOpenApiWebPageChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostWebPageChannel(new V6VmOpenApiWebPageChannelIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiWebPageChannel>(okResult.Value);
        }

        [Fact]
        public void PostWebPageChannel_ChannelServiceThrowsException()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiWebPageChannelInVersionBase>(), false, defaultVersion, null)).Throws<Exception>();
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.PostWebPageChannel(new V6VmOpenApiWebPageChannelIn());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void PutWebPageChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannel(strId, null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void PutWebPageChannel_ChannelIdNotValid(string channelId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannel(channelId, new V6VmOpenApiWebPageChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutWebPageChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannel(strId, new V6VmOpenApiWebPageChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutWebPageChannel_CurrentVersionNotFound()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannel(strId, new V6VmOpenApiWebPageChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutWebPageChannel_CanModifyChannel()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel() { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiWebPageChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiWebPageChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannel(strId, new V6VmOpenApiWebPageChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiWebPageChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutWebPageChannelBySource_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannelBySource("sourceId", null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutWebPageChannelBySource_SourceIdNotValid(string sourceId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannelBySource(sourceId, new V6VmOpenApiWebPageChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutWebPageChannelBySource_SourceIdNotFound()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId)).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannelBySource(sourceId, new V6VmOpenApiWebPageChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }

        [Fact]
        public void PutWebPageChannelBySource_CanModifyChannel()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId))
                .Returns(new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiWebPageChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiWebPageChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutWebPageChannelBySource(sourceId, new V6VmOpenApiWebPageChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiWebPageChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }
        #endregion

        #region POST/PUT printable form
        [Fact]
        public void PostPrintableFormChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostPrintableFormChannel(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostPrintableFormChannel_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.PostPrintableFormChannel(new V7VmOpenApiPrintableFormChannelIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostPrintableFormChannel_RequestIsValid()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiPrintableFormChannelInVersionBase>(), false, defaultVersion, null))
                .Returns(new V7VmOpenApiPrintableFormChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostPrintableFormChannel(new V7VmOpenApiPrintableFormChannelIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiPrintableFormChannel>(okResult.Value);
        }

        [Fact]
        public void PostPrintableFormChannel_ChannelServiceThrowsException()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiPrintableFormChannelInVersionBase>(), false, defaultVersion, null)).Throws<Exception>();
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.PostPrintableFormChannel(new V7VmOpenApiPrintableFormChannelIn());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void PutPrintableFormChannell_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannel(strId, null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void PutPrintableFormChannel_ChannelIdNotValid(string channelId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannel(channelId, new V7VmOpenApiPrintableFormChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutPrintableFormChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannel(strId, new V7VmOpenApiPrintableFormChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutPrintableFormChannel_CurrentVersionNotFound()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannel(strId, new V7VmOpenApiPrintableFormChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutPrintableFormChannel_CanModifyChannel()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel() { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiPrintableFormChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiPrintableFormChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannel(strId, new V7VmOpenApiPrintableFormChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiPrintableFormChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutPrintableFormChannelBySource_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannelBySource("sourceId", null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutPrintableFormChannelBySource_SourceIdNotValid(string sourceId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannelBySource(sourceId, new V7VmOpenApiPrintableFormChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutPrintableFormChannelBySource_SourceIdNotFound()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId)).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannelBySource(sourceId, new V7VmOpenApiPrintableFormChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }

        [Fact]
        public void PutPrintableFormChannelBySource_CanModifyChannel()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId))
                .Returns(new VmOpenApiServiceChannel { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiPrintableFormChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiPrintableFormChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutPrintableFormChannelBySource(sourceId, new V7VmOpenApiPrintableFormChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiPrintableFormChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }
        #endregion

        #region POST/PUT service location
        [Fact]
        public void PostServiceLocationChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostServiceLocationChannel(null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostServiceLocationChannel_RequestIsNotValid()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.PostServiceLocationChannel(new V7VmOpenApiServiceLocationChannelIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void PostServiceLocationChannel_RequestIsValid()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiServiceLocationChannelInVersionBase>(), false, defaultVersion, null))
                .Returns(new V7VmOpenApiServiceLocationChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PostServiceLocationChannel(new V7VmOpenApiServiceLocationChannelIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiServiceLocationChannel>(okResult.Value);
        }

        [Fact]
        public void PostServiceLocationChannel_ChannelServiceThrowsException()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.AddServiceChannel(It.IsAny<VmOpenApiServiceLocationChannelInVersionBase>(), false, defaultVersion, null)).Throws<Exception>();
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            Action act = () => controller.PostServiceLocationChannel(new V7VmOpenApiServiceLocationChannelIn());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void PutServiceLocationChannel_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannel(strId, null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void PutServiceLocationChannel_ChannelIdNotValid(string channelId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannel(channelId, new V7VmOpenApiServiceLocationChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutServiceLocationChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannel(strId, new V7VmOpenApiServiceLocationChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutServiceLocationChannel_CurrentVersionNotFound()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>())).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannel(strId, new V7VmOpenApiServiceLocationChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutServiceLocationChannel_CanModifyChannel()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(id, It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel() { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiServiceLocationChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiServiceLocationChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannel(strId, new V7VmOpenApiServiceLocationChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiServiceLocationChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, false), Times.Once);
        }

        [Fact]
        public void PutServiceLocationChannelBySource_RequestIsNull()
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannelBySource("sourceId", null);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutServiceLocationChannelBySource_SourceIdNotValid(string sourceId)
        {
            // Arrange
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannelBySource(sourceId, new V7VmOpenApiServiceLocationChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutServiceLocationChannelBySource_SourceIdNotFound()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId)).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannelBySource(sourceId, new V7VmOpenApiServiceLocationChannelInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }

        [Fact]
        public void PutServiceLocationChannelBySource_CanModifyChannel()
        {
            // Arrange
            var sourceId = "sourceId";
            channelServiceMockSetup.Setup(s => s.GetServiceChannelBySource(sourceId))
                .Returns(new VmOpenApiServiceChannel{ PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            channelServiceMockSetup.Setup(s => s.SaveServiceChannel(It.IsAny<VmOpenApiServiceLocationChannelInVersionBase>(), It.IsAny<bool>(), defaultVersion, null))
                .Returns(new V7VmOpenApiServiceLocationChannel());
            var controller = new V7ServiceChannelController(channelServiceMockSetup.Object, organizationService, codeService,
                serviceService, settings, fintoService, logger, commonService, userService);

            // Act
            var result = controller.PutServiceLocationChannelBySource(sourceId, new V7VmOpenApiServiceLocationChannelInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V7VmOpenApiServiceLocationChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelBySource(sourceId), Times.Once);
        }
        #endregion
    }
}
