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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class GetServicesByMunicipalityTests : ServiceServiceTestBase
    {
        private Guid _municipalityId;
        private Guid _areaId;
        private Mock<IAreaRepository> _areaRepoMock;

        public GetServicesByMunicipalityTests()
        {
            _municipalityId = Guid.NewGuid();
            _areaId = Guid.NewGuid();

            SetupTypesCacheMock<AreaInformationType>();
            SetupTypesCacheMock<AreaType>();

            // unitOfWork
            var areaMunicipalityRepoMock = new Mock<IAreaMunicipalityRepository>();
            areaMunicipalityRepoMock.Setup(g => g.All()).Returns(EntityGenerator.GetAreaMunicipalityList(_areaId, _municipalityId).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaMunicipalityRepository>()).Returns(areaMunicipalityRepoMock.Object);

            _areaRepoMock = new Mock<IAreaRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaRepository>()).Returns(_areaRepoMock.Object);

            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceVersioned, VmOpenApiItem>(It.IsAny<IList<ServiceVersioned>>()))
                .Returns((List<ServiceVersioned> collection) =>
                {
                    var list = new List<VmOpenApiItem>();
                    collection.ForEach(i => list.Add(new VmOpenApiItem { Id = i.UnificRootId }));
                    return list;
                });
        }

        [Fact]
        public void NoEntitiesFound()
        {
            // Arrange
            // service
            var service = ArrangeAndGetService(EntityGenerator.GetServiceEntityList(1, PublishingStatusCache));
            // Act
            var result = service.GetServicesByMunicipality(Guid.NewGuid(), true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ServicesFound_WholeCountry_IncludeWholeCountry()
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());

            // service
            var service = ArrangeAndGetService(services);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ServicesFound_WholeCountry_NotIncludeWholeCountry()
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());

            // service
            var service = ArrangeAndGetService(services);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ServicesFound_WholeCountryExceptAland_MunicipalityNotInAland()
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());

            // service
            var service = ArrangeAndGetService(services);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ServicesFound_WholeCountryExceptAland_MunicipalityNotInAland_NotIncludeWholeCountry()
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());

            // service
            var service = ArrangeAndGetService(services);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServicesFound_WholeCountryExceptAland_MunicipalityInAland(bool includeWholeCountry)
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());

            // service
            var service = ArrangeAndGetService(services, true);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, includeWholeCountry, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServicesFound_AreaType_MunicipalityNotWithinArea(bool includeWholeCountry)
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());

            // service
            var service = ArrangeAndGetService(services, true);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, includeWholeCountry, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServicesFound_AreaType_MunicipalityWithinArea(bool includeWholeCountry)
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());
            publishedService.AreaMunicipalities = new List<ServiceAreaMunicipality> {  new ServiceAreaMunicipality
            {
                MunicipalityId = _municipalityId
            } };

            // service
            var service = ArrangeAndGetService(services, true);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, includeWholeCountry, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count().Should().Be(1);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServicesFound_AreaType_AreaMunicipalityWithinArea(bool includeWholeCountry)
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());
            publishedService.Areas = new List<ServiceArea> {  new ServiceArea
            {
                Area = new Area{ AreaMunicipalities = new List<AreaMunicipality> { new AreaMunicipality{ MunicipalityId = _municipalityId } } }
            } };

            // service
            var service = ArrangeAndGetService(services, true);

            // Act
            var result = service.GetServicesByMunicipality(_municipalityId, includeWholeCountry, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count().Should().Be(1);
        }

        private ServiceService ArrangeAndGetService(IList<ServiceVersioned> services, bool isAland = false)
        {
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();

            if (isAland)
            {
                _areaRepoMock.Setup(g => g.All()).Returns(new List<Area>
                { new Area
                {
                    Id = _areaId,
                    AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString()),
                    Code = "20"
                } }.AsQueryable());
            }
            else
            {
                _areaRepoMock.Setup(g => g.All()).Returns(new List<Area>().AsQueryable());
            }

            var serviceMock = new Mock<IRepository<ServiceVersioned>>();
            serviceMock.Setup(o => o.All()).Returns(services.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(serviceMock.Object);
            var laRepoMock = new Mock<IRepository<ServiceLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceLanguageAvailability>>()).Returns(laRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<ServiceName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceName>>()).Returns(nameRepoMock.Object);


            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            // service
            return new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, null, null, null, null);
        }

    }
}
