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
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationsByAreaTests : OrganizationServiceTestBase
    {
        private DataAccess.Services.OrganizationService _service;
        private OrganizationVersioned _publishedOrganization;
        private Guid _municipalityId;
        private Guid _areaId;
        private Mock<IAreaRepository> _areaRepoMock;
        private Mock<IAreaMunicipalityRepository> _areaMunicipalityRepoMock;

        public GetOrganizationsByAreaTests()
        {
            SetupTypesCacheMock<AreaInformationType>();
            SetupTypesCacheMock<AreaType>();

            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            _publishedOrganization = organizations.FirstOrDefault(o => o.PublishingStatusId == PublishedId);

            _municipalityId = Guid.NewGuid();
            _areaId = Guid.NewGuid();

            var organizationMock = new Mock<IRepository<OrganizationVersioned>>();
            organizationMock.Setup(o => o.All()).Returns(organizations.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationVersioned>>()).Returns(organizationMock.Object);
            var laRepoMock = new Mock<IRepository<OrganizationLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationLanguageAvailability>>()).Returns(laRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<OrganizationName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationName>>()).Returns(nameRepoMock.Object);

            _areaMunicipalityRepoMock = new Mock<IAreaMunicipalityRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaMunicipalityRepository>()).Returns(_areaMunicipalityRepoMock.Object);

            _areaRepoMock = new Mock<IAreaRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaRepository>()).Returns(_areaRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor, CacheManager.PostalCodeCache, null);
        }

        [Fact]
        public void NoEntitiesFound()
        {
            // Act
            var result = _service.GetOrganizationsByArea(Guid.NewGuid(), true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OrganizationsFound(bool includeWholeCountry)
        {
            // Arrange
            _publishedOrganization.OrganizationAreas = new List<OrganizationArea> { new OrganizationArea { AreaId = _areaId } };

            // Act
            var result = _service.GetOrganizationsByArea(_areaId, includeWholeCountry, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void OrganizationsFound_WholeCountry_IncludeWholeCountry()
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());

            // Act
            var result = _service.GetOrganizationsByArea(_areaId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void OrganizationsFound_WholeCountry_NotIncludeWholeCountry()
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());

            // Act
            var result = _service.GetOrganizationsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OrganizationsFound_WholeCountryExceptAland_IncludeWholeCountry_AreNotInAland()
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());

            // Act
            var result = _service.GetOrganizationsByArea(_areaId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void OrganizationsFound_WholeCountryExceptAland_IncludeWholeCountry_AreaInAland()
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var areaMunicipalities = EntityGenerator.GetAreaMunicipalityList(_areaId, _municipalityId);
            var alandMunicipality = new AreaMunicipality { AreaId = Guid.NewGuid(), MunicipalityId = _municipalityId };
            var alandProvince = new Area {
                Id = alandMunicipality.AreaId,
                AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString()),
                Code = "20",
                AreaMunicipalities = new List<AreaMunicipality> { alandMunicipality }
            };
            alandMunicipality.Area = alandProvince;
            areaMunicipalities.Add(alandMunicipality);
            _areaMunicipalityRepoMock.Setup(g => g.All()).Returns(areaMunicipalities.AsQueryable());

            // Act
            var result = _service.GetOrganizationsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OrganizationsFound_WholeCountryExceptAland_NotIncludeWholeCountry_AreNotInAland()
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());

            // Act
            var result = _service.GetOrganizationsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OrganizationsFound_WholeCountryExceptAland_NotIncludeWholeCountry_AreaInAland()
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var areaMunicipalities = EntityGenerator.GetAreaMunicipalityList(_areaId, _municipalityId);
            var alandMunicipality = new AreaMunicipality { AreaId = Guid.NewGuid(), MunicipalityId = _municipalityId };
            var alandProvince = new Area
            {
                Id = alandMunicipality.AreaId,
                AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString()),
                Code = "20",
                AreaMunicipalities = new List<AreaMunicipality> { alandMunicipality }
            };
            alandMunicipality.Area = alandProvince;
            areaMunicipalities.Add(alandMunicipality);
            _areaMunicipalityRepoMock.Setup(g => g.All()).Returns(areaMunicipalities.AsQueryable());

            // Act
            var result = _service.GetOrganizationsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }
    }
}
