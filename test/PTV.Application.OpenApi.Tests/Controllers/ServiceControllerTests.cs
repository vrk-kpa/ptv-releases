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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.OpenApi.V10;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ServiceControllerTests : ControllerTestBase
    {
        private ILogger<V10ServiceController> logger;

        private Mock<IFintoService> fintoServiceMock;
        private IFintoService fintoService;

        private Mock<IOntologyTermDataCache> ontologyTermDataCacheMock;
        private IOntologyTermDataCache ontologyTermDataCache;

        private Guid id;
        private string strId;
        private int pageNumber = 1;
        private readonly int version = 10;

        private V10ServiceController controller;

        public ServiceControllerTests()
        {
            logger = (new Mock<ILogger<V10ServiceController>>()).Object;

            fintoServiceMock = new Mock<IFintoService>();
            fintoService = fintoServiceMock.Object;

            ontologyTermDataCacheMock = new Mock<IOntologyTermDataCache>();
            ontologyTermDataCache = ontologyTermDataCacheMock.Object;

            id = Guid.NewGuid();
            strId = id.ToString();

            controller = new V10ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, ontologyTermDataCache, channelService, userService, organizationService, logger);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("IvalidStatus")]
        public void Get_Services_StatusNotValid(string status)
        {
            // Act
            var result = controller.Get(null, null, pageNumber, status);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(EntityStatusEnum.Published)]
        [InlineData(EntityStatusEnum.Archived)]
        [InlineData(EntityStatusEnum.Withdrawn)]
        public void Get_Services_CanCall(EntityStatusEnum status)
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(null, pageNumber, It.IsAny<int>(), It.IsAny<EntityStatusExtendedEnum>(), null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.Get(null, null, pageNumber, status.ToString());

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
            // Act
            Action act = () => controller.Get(id);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Get_ServiceNotExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>(), It.IsAny<bool>())).Returns((VmOpenApiServiceVersionBase)null);

            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_ServiceExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<VersionStatusEnum>(), It.IsAny<bool>())).Returns(new VmOpenApiServiceVersionBase());

            // Act
            var result = controller.Get(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
            serviceServiceMockSetup.Verify(s => s.GetServiceById(id, 10, VersionStatusEnum.Published, false), Times.Once);
        }

        [Fact]
        public void GetWithGdData_ServiceNotExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>(), It.IsAny<bool>())).Returns((VmOpenApiServiceVersionBase)null);

            // Act
            var result = controller.GetWithGdData(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetWithGdData_ServiceExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<VersionStatusEnum>(), It.IsAny<bool>())).Returns(new VmOpenApiServiceVersionBase());

            // Act
            var result = controller.GetWithGdData(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
            serviceServiceMockSetup.Verify(s => s.GetServiceById(id, 10, VersionStatusEnum.Published, true), Times.Once);
        }

        [Theory]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GetByIdList_IdIsInvalid(string id)
        {
            // Act
            var result = controller.GetByIdList(id);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<SerializableError>(badResult.Value);
            error.ContainsKey("guids").Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetByIdList_IdsRequired(string idList)
        {
            // Act
            var result = controller.GetByIdList(idList);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<SerializableError>(badResult.Value);
            error.ContainsKey("guids").Should().BeTrue();
        }

        [Fact]
        public void GetByIdList_NoItemsFound()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(It.IsAny<List<Guid>>(), version, false))
                .Returns((IList<IVmOpenApiServiceVersionBase>)null);

            // Act
            var result = controller.GetByIdList(strId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void GetByIdList_ItemsFound()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(It.IsAny<List<Guid>>(), version, false))
                .Returns((List<Guid> list, int v, bool f) =>
                {
                    IList<IVmOpenApiServiceVersionBase> serviceList = new List<IVmOpenApiServiceVersionBase>();
                    list.ForEach(g => serviceList.Add(new VmOpenApiServiceVersionBase { Id = g }));
                    return serviceList;
                });

            // Act
            var result = controller.GetByIdList(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var channels = Assert.IsType<List<IVmOpenApiServiceVersionBase>>(okResult.Value);
            channels.Count.Should().Be(1);
        }

        [Theory]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GetByIdListWithGdData_IdIsInvalid(string id)
        {
            // Act
            var result = controller.GetByIdListWithGdData(id);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<SerializableError>(badResult.Value);
            error.ContainsKey("guids").Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetByIdListWithGdData_IdsRequired(string idList)
        {
            // Act
            var result = controller.GetByIdListWithGdData(idList);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<SerializableError>(badResult.Value);
            error.ContainsKey("guids").Should().BeTrue();
        }

        [Fact]
        public void GetByIdListWithGdData_NoItemsFound()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(It.IsAny<List<Guid>>(), version, true))
                .Returns((IList<IVmOpenApiServiceVersionBase>)null);

            // Act
            var result = controller.GetByIdListWithGdData(strId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void GetByIdListWithGdData_ItemsFound()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(It.IsAny<List<Guid>>(), version, true))
                .Returns((List<Guid> list, int v, bool f) =>
                {
                    IList<IVmOpenApiServiceVersionBase> serviceList = new List<IVmOpenApiServiceVersionBase>();
                    list.ForEach(g => serviceList.Add(new VmOpenApiServiceVersionBase { Id = g }));
                    return serviceList;
                });

            // Act
            var result = controller.GetByIdListWithGdData(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var channels = Assert.IsType<List<IVmOpenApiServiceVersionBase>>(okResult.Value);
            channels.Count.Should().Be(1);
        }

        [Fact]
        public void GetActive_Services_CanCall()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServices(null, pageNumber, It.IsAny<int>(), It.IsAny<EntityStatusExtendedEnum>(), null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.GetActive(null, null, pageNumber);

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
            // Act
            Action act = () => controller.GetActive(id);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void GetActive_ServiceNotExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), defaultVersion, It.IsAny<VersionStatusEnum>(), It.IsAny<bool>())).Returns((VmOpenApiServiceVersionBase)null);

            // Act
            var result = controller.GetActive(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetActive_ServiceExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<VersionStatusEnum>(), It.IsAny<bool>())).Returns(new VmOpenApiServiceVersionBase());

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
            // Act
            Action act = () => controller.GetByServiceChannel(id, null, null, pageNumber);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void GetByServiceChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.ChannelExists(id)).Returns(false);

            // Act
            var result = controller.GetByServiceChannel(strId, null, null, pageNumber);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByServiceChannel_ChannelExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.ChannelExists(id)).Returns(true);
            serviceServiceMockSetup.Setup(s => s.GetServicesByServiceChannel(id, null, pageNumber, pageSize, null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.GetByServiceChannel(strId, null, null, pageNumber);

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

            // Act
            var result = controller.GetByServiceClass(uri, null, null, pageNumber);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByServiceClass_ClassExists()
        {
            var uri = "valid_uri";
            var idList = new List<Guid> { id };
            fintoServiceMock.Setup(s => s.GetServiceClassByUri(uri)).Returns(new VmOpenApiFintoItemVersionBase { Id = id });
            serviceServiceMockSetup.Setup(s => s.GetServicesByServiceClass(idList, null, pageNumber, pageSize, null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.GetByServiceClass(uri, null, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            serviceServiceMockSetup.Verify(x => x.GetServicesByServiceClass(idList, null, pageNumber, pageSize, null), Times.Once);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Fact]
        public void GetByServiceClass_AlsoParentClassServicesAttached()
        {
            var uri = "valid_uri";
            var parentId = Guid.NewGuid();
            var idList = new List<Guid> { id, parentId };
            fintoServiceMock.Setup(s => s.GetServiceClassByUri(uri)).Returns(new VmOpenApiFintoItemVersionBase { Id = id, ParentId = parentId });
            serviceServiceMockSetup.Setup(s => s.GetServicesByServiceClass(idList, null, pageNumber, pageSize, null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.GetByServiceClass(uri, null, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            serviceServiceMockSetup.Verify(x => x.GetServicesByServiceClass(idList, null, pageNumber, pageSize, null), Times.Once);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Fact]
        public void GetByMunicipality_ModelIsNull()
        {
            // Act
            var ctr = new V9ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, ontologyTermDataCache, channelService, userService, organizationService, new Mock<ILogger<V9ServiceController>>().Object);
            var result = ctr.GetByMunicipality(null, null, null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByMunicipality_MunicipalityNotExists()
        {
            // Arrange
            var ctr = new V9ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, ontologyTermDataCache, channelService, userService, organizationService, new Mock<ILogger<V9ServiceController>>().Object);
            codeServiceMockSetup.Setup(s => s.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns((VmListItem)null);

            // Act
            var result = ctr.GetByMunicipality("code", null, null);

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
            serviceServiceMockSetup.Setup(s => s.GetServicesByMunicipality(municipalityId, true, null, page, pageSize, null)).Returns(new V3VmOpenApiGuidPage());
            var ctr = new V9ServiceController(serviceServiceMockSetup.Object, commonService, codeService, settings, gdService,
                fintoService, ontologyTermDataCache, channelService, userService, organizationService, new Mock<ILogger<V9ServiceController>>().Object);

            // Act
            var result = ctr.GetByMunicipality(municipalityId.ToString(), null, null, page);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetByArea_ModelIsNull(bool includeWholeCountry)
        {
            // Act
            var result = controller.GetByArea(null, null, includeWholeCountry, null, null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData(AreaTypeEnum.BusinessRegions, true)]
        [InlineData(AreaTypeEnum.BusinessRegions, false)]
        [InlineData(AreaTypeEnum.HospitalRegions, true)]
        [InlineData(AreaTypeEnum.HospitalRegions, false)]
        [InlineData(AreaTypeEnum.Municipality, true)]
        [InlineData(AreaTypeEnum.Municipality, false)]
        [InlineData(AreaTypeEnum.Province, true)]
        [InlineData(AreaTypeEnum.Province, false)]
        public void GetByArea_AreaNotExists(AreaTypeEnum area, bool includeWholeCountry)
        {
            // Arrange
            codeServiceMockSetup.Setup(s => s.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns((VmListItem)null);
            codeServiceMockSetup.Setup(s => s.GetAreaIdByCodeAndType(It.IsAny<string>(), It.IsAny<string>())).Returns((Guid?)null);

            // Act
            var result = controller.GetByArea(area.ToString(), "code", includeWholeCountry, null, null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData(AreaTypeEnum.BusinessRegions, true)]
        [InlineData(AreaTypeEnum.BusinessRegions, false)]
        [InlineData(AreaTypeEnum.HospitalRegions, true)]
        [InlineData(AreaTypeEnum.HospitalRegions, false)]
        [InlineData(AreaTypeEnum.Municipality, true)]
        [InlineData(AreaTypeEnum.Municipality, false)]
        [InlineData(AreaTypeEnum.Province, true)]
        [InlineData(AreaTypeEnum.Province, false)]
        public void GetByArea_AreaExists(AreaTypeEnum area, bool includeWholeCountry)
        {
            // Arrange
            var areaId = Guid.NewGuid();
            codeServiceMockSetup.Setup(s => s.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns(new VmListItem { Id = areaId });
            codeServiceMockSetup.Setup(s => s.GetAreaIdByCodeAndType(It.IsAny<string>(), It.IsAny<string>())).Returns(areaId);
            var page = 1;
            serviceServiceMockSetup.Setup(s => s.GetServicesByMunicipality(areaId, includeWholeCountry, null, page, pageSize, null)).Returns(new V3VmOpenApiGuidPage());
            serviceServiceMockSetup.Setup(s => s.GetServicesByArea(areaId, includeWholeCountry, null, page, pageSize, null)).Returns(new V3VmOpenApiGuidPage());

            // Act
            var result = controller.GetByArea(area.ToString(), "code", includeWholeCountry, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetByTargetGroup_UriNotValid(string uri)
        {
            fintoServiceMock.Setup(s => s.GetTargetGroupByUri(uri)).Returns((VmOpenApiFintoItemVersionBase)null);

            // Act
            var result = controller.GetByTargetGroup(uri, null, null, pageNumber);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void GetByTargetGroup_GroupNotExists()
        {
            var uri = "invalid_uri";
            fintoServiceMock.Setup(s => s.GetTargetGroupByUri(uri)).Returns((VmOpenApiFintoItemVersionBase)null);

            // Act
            var result = controller.GetByTargetGroup(uri, null, null, pageNumber);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByTargetGroup_GroupExists()
        {
            var uri = "valid_uri";
            fintoServiceMock.Setup(s => s.GetTargetGroupByUri(uri)).Returns(new VmOpenApiFintoItemVersionBase { Id = id });
            serviceServiceMockSetup.Setup(s => s.GetServicesByTargetGroup(id, null, pageNumber, pageSize, null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.GetByTargetGroup(uri, null, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NotValidType")]
        public void GetByType_TypeNotValid(string type)
        {
            // Act
            Action act = () => controller.GetByType(type, null, null, pageNumber);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("Service")]
        [InlineData("PermitOrObligation")]
        [InlineData("ProfessionalQualification")]
        public void GetByTyp_CanCall(string type)
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServicesByType(It.IsAny<string>(), null, pageNumber, pageSize, null))
                .Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });

            // Act
            var result = controller.GetByType(type, null, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V3VmOpenApiGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Fact]
        public void Post_RequestIsNull()
        {
            // Act
            var result = controller.Post(null, false);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_RequestIsNotValid()
        {
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.Post(new V9VmOpenApiServiceIn(), false);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_RequestIsValid()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.AddService(It.IsAny<VmOpenApiServiceInVersionBase>(), It.IsAny<int>(), false))
                .Returns(new V10VmOpenApiService());

            // Act
            var result = controller.Post(new V9VmOpenApiServiceIn(), false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V10VmOpenApiService>(okResult.Value);
        }

        [Fact]
        public void Post_serviceServiceThrowsException()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.AddService(It.IsAny<VmOpenApiServiceInVersionBase>(), It.IsAny<int>(), false)).Throws<Exception>();

            // Act
            Action act = () => controller.Post(new V9VmOpenApiServiceIn(), false);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Put_RequestIsNull()
        {
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
            // Act
            var result = controller.Put(serviceId, new V9VmOpenApiServiceInBase(), false);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_ServiceNotExists()
        {
            // Act
            var result = controller.Put(strId, new V9VmOpenApiServiceInBase(), false);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_CurrentVersionNotFound()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(id, false)).Returns((VmOpenApiServiceVersionBase)null);

            // Act
            var result = controller.Put(strId, new V9VmOpenApiServiceInBase(), false);

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
            serviceServiceMockSetup.Setup(s => s.SaveService(It.IsAny<VmOpenApiServiceInVersionBase>(), It.IsAny<int>(), false, null))
                .Returns(new V10VmOpenApiService());

            // Act
            var result = controller.Put(strId, new V9VmOpenApiServiceInBase(), false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V10VmOpenApiService>(okResult.Value);
        }

        [Fact]
        public void PutBySource_RequestIsNull()
        {
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
            // Act
            var result = controller.PutBySource(sourceId, new V9VmOpenApiServiceInBase(), false);

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

            // Act
            var result = controller.PutBySource(sourdeId, new V9VmOpenApiServiceInBase(), false);

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
                .Returns(new VmOpenApiServiceVersionBase { PublishingStatus = PublishingStatus.Published.ToString(), Security = new VmSecurityOwnOrganization { IsOwnOrganization = true } });
            serviceServiceMockSetup.Setup(s => s.SaveService(It.IsAny<VmOpenApiServiceInVersionBase>(), It.IsAny<int>(), false, sourdeId))
                .Returns(new V10VmOpenApiService());

            // Act
            var result = controller.PutBySource(sourdeId, new V9VmOpenApiServiceInBase(), false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V10VmOpenApiService>(okResult.Value);
        }
    }
}
