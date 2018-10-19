﻿/**
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
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class OrganizationControllerTests : ControllerTestBase
    {
        private ILogger<V9OrganizationController> logger;

        private Guid id;
        private string strId;
        private V9OrganizationController controller;

        public OrganizationControllerTests()
        {
            var loggerMock = new Mock<ILogger<V9OrganizationController>>();
            logger = loggerMock.Object;

            id = Guid.NewGuid();
            strId = id.ToString();

            controller = new V9OrganizationController(organizationServiceMockSetup.Object, codeService, settings, logger, commonService, userService); ;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("InvalidStatus")]
        public void GetOrganizations_StatusNotValid(string status)
        {
            // Arrange
            var pageNumber = 1;
            
            // Act
            var result = controller.Get(null, null, pageNumber, status);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(EntityStatusEnum.Published)]
        [InlineData(EntityStatusEnum.Archived)]
        [InlineData(EntityStatusEnum.Withdrawn)]
        public void GetOrganizations_CanCall(EntityStatusEnum status)
        {
            // Arrange
            var pageNumber = 1;
            organizationServiceMockSetup.Setup(s => s.GetOrganizations(null, defaultVersion, status, pageNumber, It.IsAny<int>(), null)).Returns(new VmOpenApiOrganizationGuidPage(pageNumber, pageSize));

            // Act
            var result = controller.Get(null, null, pageNumber, status.ToString());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<VmOpenApiOrganizationGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
            organizationServiceMockSetup.Verify(s => s.GetOrganizations(null, defaultVersion, status, pageNumber, It.IsAny<int>(), null), Times.Once());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void Get_IdIsNull(string id)
        {
            // Act
            Action act = () => controller.Get(id);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void Get_OrganizationNotExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationById(It.IsAny<Guid>(), defaultVersion, It.IsAny<bool>())).Returns((V8VmOpenApiOrganization)null);

            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_OrganizationExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationById(It.IsAny<Guid>(), defaultVersion, It.IsAny<bool>())).Returns(new V8VmOpenApiOrganization());

            // Act
            var result = controller.Get(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V8VmOpenApiOrganization>(okResult.Value);
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
            organizationServiceMockSetup.Setup(s => s.GetOrganizations(It.IsAny<List<Guid>>(), defaultVersion))
                .Returns((IList<IVmOpenApiOrganizationVersionBase>)null);

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
            organizationServiceMockSetup.Setup(s => s.GetOrganizations(It.IsAny<List<Guid>>(), defaultVersion))
                .Returns((List<Guid> list, int openApiVersion) =>
                {
                    IList<IVmOpenApiOrganizationVersionBase> gdList = new List<IVmOpenApiOrganizationVersionBase>();
                    list.ForEach(g => gdList.Add(new VmOpenApiOrganizationVersionBase { Id = g }));
                    return gdList;
                });

            // Act
            var result = controller.GetByIdList(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var channels = Assert.IsType<List<IVmOpenApiOrganizationVersionBase>>(okResult.Value);
            channels.Count.Should().Be(1);
        }

        [Fact]
        public void GetSahaOrganizations_CanCall()
        {
            // Arrange
            var pageNumber = 1;
            organizationServiceMockSetup.Setup(s => s.GetOrganizationsSaha(null, pageNumber, It.IsAny<int>(), null)).Returns(new VmOpenApiOrganizationSahaGuidPage(pageNumber, pageSize));

            // Act
            var result = controller.GetSaha(null, null, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<VmOpenApiOrganizationSahaGuidPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Fact]
        public void GetSaha_IdIsNull()
        {
            // Act
            Action act = () => controller.GetSaha(null);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetSaha_OrganizationNotExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationSahaById(It.IsAny<Guid>())).Returns((VmOpenApiOrganizationSaha)null);

            // Act
            var result = controller.GetSaha(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetSaha_OrganizationExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationSahaById(It.IsAny<Guid>())).Returns(new VmOpenApiOrganizationSaha());

            // Act
            var result = controller.GetSaha(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiOrganizationSaha>(okResult.Value);
        }

        [Fact]
        public void GetByYCode_CodeNotExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationsByBusinessCode(It.IsAny<string>(), defaultVersion)).Returns((List<IVmOpenApiOrganizationVersionBase>)null);

            // Act
            var result = controller.GetByYCode(null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByYCode_CodeExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationsByBusinessCode(It.IsAny<string>(), defaultVersion)).Returns(new List<IVmOpenApiOrganizationVersionBase>() { new V8VmOpenApiOrganization() });

            // Act
            var result = controller.GetByYCode("code");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<IVmOpenApiOrganizationVersionBase>>(okResult.Value);
            Assert.Equal(1, model.Count());
        }

        [Fact]
        public void GetByYOid_OidNotExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationByOid(It.IsAny<string>(), defaultVersion)).Returns((V8VmOpenApiOrganization)null);

            // Act
            var result = controller.GetByOid(null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByYOid_OidExists()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationByOid(It.IsAny<string>(), defaultVersion)).Returns(new V8VmOpenApiOrganization());

            // Act
            var result = controller.GetByOid("code");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V8VmOpenApiOrganization>(okResult.Value);
        }

        [Fact]
        public void GetByMunicipality_ModelIsNull()
        {
            // Act
            var result = controller.GetByMunicipality(null, null, null);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetByMunicipality_MunicipalityNotExists()
        {
            // Arrange
            codeServiceMockSetup.Setup(s => s.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns((VmListItem)null);

            // Act
            var result = controller.GetByMunicipality("code", null, null);

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
            organizationServiceMockSetup.Setup(s => s.GetOrganizationsByMunicipality(municipalityId, null, page, pageSize, defaultVersion, null)).Returns(new V8VmOpenApiOrganizationGuidPage(page, pageSize));

            // Act
            var result = controller.GetByMunicipality(municipalityId.ToString(), null, null, page);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<V8VmOpenApiOrganizationGuidPage>(okResult.Value);
        }
        
        [Fact]
        public void GetHierarchy_CanCall()
        {
            // Arrange
            var pageNumber = 1;
            organizationServiceMockSetup.Setup(s => s.GetOrganizationsHierarchy(null, pageNumber, It.IsAny<int>(), null)).Returns(new V3VmOpenApiGuidPage(pageNumber, pageSize));

            // Act
            var result = controller.GetHierarchy(null, null, pageNumber);

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
        public void GetHierarchyById_IdIsNotValid(string id)
        {
            // Act
            Action act = () => controller.GetHierarchy(id);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetHierarchyById_NotFound()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.GetOrganizationsHierarchy(It.IsAny<Guid>())).Returns((VmOpenApiOrganizationHierarchy)null);

            // Act
            var result = controller.GetHierarchy(Guid.NewGuid().ToString());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetHierarchyById_CanGetResult()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            organizationServiceMockSetup.Setup(s => s.GetOrganizationsHierarchy(It.IsAny<Guid>()))
                .Returns((Guid id) => { return new VmOpenApiOrganizationHierarchy { Id = id }; });

            // Act
            var result = controller.GetHierarchy(organizationId.ToString());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<VmOpenApiOrganizationHierarchy>(okResult.Value);
            model.Id.Should().Be(organizationId);
        }

        [Fact]
        public void Post_RequestIsNull()
        {
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
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.Post(new V9VmOpenApiOrganizationIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Theory]
        [InlineData(UserRoleEnum.Pete)]
        [InlineData(UserRoleEnum.Shirley)]
        public void Post_PeteOrShirleyCannotAddMainOrganization(UserRoleEnum role)
        {
            // Arrange
            userServiceMockSetup.Setup(s => s.UserHighestRole()).Returns(role);

            // Act
            var result = controller.Post(new V9VmOpenApiOrganizationIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_EevaCanAddMainOrganization()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.AddOrganization(It.IsAny<VmOpenApiOrganizationInVersionBase>(), false, defaultVersion)).Returns(new V8VmOpenApiOrganization());
            userServiceMockSetup.Setup(s => s.UserHighestRole()).Returns(UserRoleEnum.Eeva);

            // Act
            var result = controller.Post(new V9VmOpenApiOrganizationIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<V8VmOpenApiOrganization>(okResult.Value);
        }

        [Fact]
        public void Post_OrganizationServiceThrowsException()
        {
            // Arrange
            organizationServiceMockSetup.Setup(s => s.AddOrganization(It.IsAny<VmOpenApiOrganizationInVersionBase>(), false, defaultVersion)).Throws<Exception>();

            // Act
            Action act = () => controller.Post(new V9VmOpenApiOrganizationIn());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void Put_RequestIsNull()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), null)).Returns(true);

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
        public void Put_OrganizationIdNotValid(string organizationId)
        {
            // Act
            var result = controller.Put(organizationId, new V9VmOpenApiOrganizationInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_OrganizationNotExists()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), null)).Returns(false);

            // Act
            var result = controller.Put(strId, new V9VmOpenApiOrganizationInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_CurrentVersionNotFound()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), null)).Returns(true);
            organizationServiceMockSetup.Setup(s => s.GetOrganizationById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>())).Returns((VmOpenApiOrganizationVersionBase)null);

            // Act
            var result = controller.Put(strId, new V9VmOpenApiOrganizationInBase());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Put_CanModifyOrganization()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), null)).Returns(true);
            organizationServiceMockSetup.Setup(s => s.GetOrganizationById(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(new VmOpenApiOrganizationVersionBase { PublishingStatus = PublishingStatus.Published.ToString() });
            organizationServiceMockSetup.Setup(s => s.SaveOrganization(It.IsAny<VmOpenApiOrganizationInVersionBase>(), It.IsAny<bool>(), defaultVersion)).Returns(new V8VmOpenApiOrganization());

            // Act
            var result = controller.Put(strId, new V9VmOpenApiOrganizationInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V8VmOpenApiOrganization>(okResult.Value);
        }

        [Fact]
        public void PutBySource_RequestIsNull()
        {
            // Arrange
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), null)).Returns(true);

            // Act
            var result = controller.PutBySource("sourceId", null);

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
            var result = controller.PutBySource(sourceId, new V9VmOpenApiOrganizationInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void PutBySource_SourceIdNotFound()
        {
            // Arrange
            var sourdeId = "sourceId";
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), null)).Returns(true);
            organizationServiceMockSetup.Setup(s => s.GetOrganizationBySource(sourdeId, It.IsAny<int>(), It.IsAny<bool>())).Returns(new VmOpenApiOrganizationVersionBase { PublishingStatus = PublishingStatus.Published.ToString() });
            organizationServiceMockSetup.Setup(s => s.SaveOrganization(It.IsAny<VmOpenApiOrganizationInVersionBase>(), It.IsAny<bool>(), defaultVersion)).Throws<Exception>();

            // Act
            Action act = () => controller.PutBySource(sourdeId, new V9VmOpenApiOrganizationInBase());

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void PutBySource_CanModifyOrganization()
        {
            // Arrange
            var sourdeId = "sourceId";
            commonServiceMockSetup.Setup(s => s.OrganizationExists(It.IsAny<Guid>(), null)).Returns(true);
            organizationServiceMockSetup.Setup(s => s.GetOrganizationBySource(sourdeId, It.IsAny<int>(), It.IsAny<bool>())).Returns(new VmOpenApiOrganizationVersionBase { PublishingStatus = PublishingStatus.Published.ToString() });
            organizationServiceMockSetup.Setup(s => s.SaveOrganization(It.IsAny<VmOpenApiOrganizationInVersionBase>(), It.IsAny<bool>(), defaultVersion)).Returns(new V8VmOpenApiOrganization());

            // Act
            var result = controller.PutBySource(sourdeId, new V9VmOpenApiOrganizationInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<V8VmOpenApiOrganization>(okResult.Value);
        }
    }
}
