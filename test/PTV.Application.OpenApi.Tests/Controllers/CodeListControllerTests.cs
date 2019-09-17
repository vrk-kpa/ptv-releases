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

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class CodeListControllerTests : ControllerTestBase
    {
        private ILogger<V7CodeListController> logger;

        private Mock<IMunicipalityService> municipalityServiceMockSetup;
        private Mock<ICountryService> countryServiceMockSetup;
        private Mock<IPostalCodeService> postalCodeServiceMockSetup;
        private Mock<ILanguageService> languageServiceMockSetup;

        private Guid id;
        private string strId;

        public CodeListControllerTests()
        {
            var loggerMock = new Mock<ILogger<V7CodeListController>>();
            logger = loggerMock.Object;
            
            municipalityServiceMockSetup = new Mock<IMunicipalityService>();
            countryServiceMockSetup = new Mock<ICountryService>();
            postalCodeServiceMockSetup = new Mock<IPostalCodeService>();
            languageServiceMockSetup = new Mock<ILanguageService>();
            
            pageSize = 1000;
            id = Guid.NewGuid();
            strId = id.ToString();
        }

        [Fact]
        public void GetMunicipalityCodes_CanCall()
        {
            // Arrange
            municipalityServiceMockSetup.Setup(s => s.GetMunicipalitiesCodeList()).Returns(new List<VmOpenApiCodeListItem>());
            var controller = new V7CodeListController(commonServiceMockSetup.Object, municipalityServiceMockSetup.Object, countryServiceMockSetup.Object, postalCodeServiceMockSetup.Object, languageServiceMockSetup.Object, userService, settings, logger);

            // Act
            var result = controller.GetMunicipalityCodes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<VmOpenApiCodeListItem>> (okResult.Value);
        }

        [Fact]
        public void GetCountryCodes_CanCall()
        {
            // Arrange
            countryServiceMockSetup.Setup(s => s.GetCountryCodeList()).Returns(new List<VmOpenApiDialCodeListItem>());
            var controller = new V7CodeListController(commonServiceMockSetup.Object, municipalityServiceMockSetup.Object, countryServiceMockSetup.Object, postalCodeServiceMockSetup.Object, languageServiceMockSetup.Object, userService, settings, logger);

            // Act
            var result = controller.GetCountryCodes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<VmOpenApiDialCodeListItem>>(okResult.Value);
        }

        [Fact]
        public void GetPostalCodes_CanCall()
        {
            // Arrange
            var pageNumber = 1;
            postalCodeServiceMockSetup.Setup(s => s.GetPostalCodeList(pageNumber, pageSize)).Returns(new VmOpenApiCodeListPage { PageNumber = pageNumber });
            var controller = new V7CodeListController(commonServiceMockSetup.Object, municipalityServiceMockSetup.Object, countryServiceMockSetup.Object, postalCodeServiceMockSetup.Object, languageServiceMockSetup.Object, userService, settings, logger);

            // Act
            var result = controller.GetPostalCodes(pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<VmOpenApiCodeListPage>(okResult.Value);
            Assert.Equal(pageNumber, model.PageNumber);
        }

        [Fact]
        public void GetLanguageCodes_CanCall()
        {
            // Arrange
            languageServiceMockSetup.Setup(s => s.GetLanguageCodeList()).Returns(new List<VmOpenApiCodeListItem>());
            var controller = new V7CodeListController(commonServiceMockSetup.Object, municipalityServiceMockSetup.Object, countryServiceMockSetup.Object, postalCodeServiceMockSetup.Object, languageServiceMockSetup.Object, userService, settings, logger);

            // Act
            var result = controller.GetLanguageCodes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<VmOpenApiCodeListItem>>(okResult.Value);
        }

        [Fact]
        public void GetAreaCodes_CanCall()
        {
            // Arrange
            var type = AreaTypeEnum.BusinessRegions.ToString();
            commonServiceMockSetup.Setup(s => s.GetAreaCodeList(It.IsAny<AreaTypeEnum>())).Returns(new List<VmOpenApiCodeListItem>());
            var controller = new V7CodeListController(commonServiceMockSetup.Object, municipalityServiceMockSetup.Object, countryServiceMockSetup.Object, postalCodeServiceMockSetup.Object, languageServiceMockSetup.Object, userService, settings, logger);

            // Act
            var result = controller.GetAreaCodes(type);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<VmOpenApiCodeListItem>>(okResult.Value);
        }
    }
}
