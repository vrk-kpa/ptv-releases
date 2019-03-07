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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class GeneralDescriptionControllerTests : ControllerTestBase
    {
        private ILogger<V9GeneralDescriptionController> logger;
        private int _defaultVersion;

        private Mock<IFintoService> fintoServiceMock;
        private IFintoService fintoService;
        
        private Mock<IOntologyTermDataCache> ontologyTermDataCacheMock;
        private IOntologyTermDataCache ontologyTermDataCache;

        private Guid id;
        private string strId;

        private V9GeneralDescriptionController controller;

        public GeneralDescriptionControllerTests()
        {
            logger = (new Mock<ILogger<V9GeneralDescriptionController>>()).Object;
            _defaultVersion = 9;//defaultVersion;

            fintoServiceMock = new Mock<IFintoService>();
            fintoService = fintoServiceMock.Object;
            
            ontologyTermDataCacheMock = new Mock<IOntologyTermDataCache>();
            ontologyTermDataCache = ontologyTermDataCacheMock.Object;

            id = Guid.NewGuid();
            strId = id.ToString();

            controller = new V9GeneralDescriptionController(gdServiceMockSetup.Object, codeService, fintoService,ontologyTermDataCache, userService, logger, settings);

        }

        [Fact]
        public void GetGeneralDescriptions_CanCall()
        {
            // Arrange
            var pageNumber = 1;
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptions(null, pageNumber, It.IsAny<int>(), null)).Returns(new V3VmOpenApiGuidPage { PageNumber = pageNumber });
            
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
        public void Get_IdIsNull(string gdId)
        {
            // Act
            Action act = () => controller.Get(gdId);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Get_GeneralDescriptionNotExists()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, _defaultVersion, It.IsAny<bool>(), It.IsAny<bool>())).Returns((VmOpenApiGeneralDescriptionVersionBase)null);
            
            // Act
            var result = controller.Get(strId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_GeneralDescriptionExists()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, _defaultVersion, It.IsAny<bool>(), It.IsAny<bool>())).Returns(new VmOpenApiGeneralDescriptionVersionBase());
            
            // Act
            var result = controller.Get(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiGeneralDescriptionVersionBase>(okResult.Value);
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
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptions(It.IsAny<List<Guid>>(), _defaultVersion))
                .Returns((IList<IVmOpenApiGeneralDescriptionVersionBase>)null);
            
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
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptions(It.IsAny<List<Guid>>(), _defaultVersion))
                .Returns((List<Guid> list, int openApiVersion) =>
                {
                    IList<IVmOpenApiGeneralDescriptionVersionBase> gdList = new List<IVmOpenApiGeneralDescriptionVersionBase>();
                    list.ForEach(g => gdList.Add(new VmOpenApiGeneralDescriptionVersionBase { Id = g }));
                    return gdList;
                });
           
            // Act
            var result = controller.GetByIdList(strId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var channels = Assert.IsType<List<IVmOpenApiGeneralDescriptionVersionBase>>(okResult.Value);
            channels.Count.Should().Be(1);
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
            controller.ModelState.AddModelError("Request", "NotValid");

            // Act
            var result = controller.Post(new V9VmOpenApiGeneralDescriptionIn());

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badResult.Value);
        }

        [Fact]
        public void Post_RequestIsValid()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.AddGeneralDescription(It.IsAny<VmOpenApiGeneralDescriptionInVersionBase>(), _defaultVersion))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase());
            
            // Act
            var result = controller.Post(new V9VmOpenApiGeneralDescriptionIn());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<VmOpenApiGeneralDescriptionVersionBase>(okResult.Value);
        }

        [Fact]
        public void Post_serviceThrowsException()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.AddGeneralDescription(It.IsAny<VmOpenApiGeneralDescriptionInVersionBase>(), _defaultVersion))
                .Throws<Exception>();
            
            // Act
            Action act = () => controller.Post(new V9VmOpenApiGeneralDescriptionIn());

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Put_RequestIsNull()
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
        public void Put_IdNotValid(string gdId)
        {
            // Act
            var result = controller.Put(gdId, new V9VmOpenApiGeneralDescriptionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_GeneralDescriptionNotExists()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GeneralDescriptionExists(id)).Returns(false);
            
            // Act
            var result = controller.Put(strId, new V9VmOpenApiGeneralDescriptionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_CurrentVersionNotFound()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GeneralDescriptionExists(id)).Returns(true);
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, 0, false, It.IsAny<bool>()))
                .Returns((VmOpenApiGeneralDescriptionVersionBase)null);
            
            // Act
            var result = controller.Put(strId, new V9VmOpenApiGeneralDescriptionInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<VmError>(notFoundResult.Value);
        }

        [Fact]
        public void Put_CanModifyGeneralDescription()
        {
            // Arrange
            gdServiceMockSetup.Setup(s => s.GeneralDescriptionExists(id)).Returns(true);
            gdServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(id, 0, false, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase { PublishingStatus = PublishingStatus.Published.ToString() });
            gdServiceMockSetup.Setup(s => s.SaveGeneralDescription(It.IsAny<VmOpenApiGeneralDescriptionInVersionBase>(), _defaultVersion))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase());
            
            // Act
            var result = controller.Put(strId, new V9VmOpenApiGeneralDescriptionInBase());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<VmOpenApiGeneralDescriptionVersionBase>(okResult.Value);
        }

    }
}
