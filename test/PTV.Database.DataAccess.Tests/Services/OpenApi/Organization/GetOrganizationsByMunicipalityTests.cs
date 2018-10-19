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
    public class GetOrganizationsByMunicipalityTests : OrganizationServiceTestBase
    {
        private Guid _municipalityId;
        private Guid _areaId;
        private Mock<IAreaRepository> _areaRepoMock;

        public GetOrganizationsByMunicipalityTests()
        {
            _municipalityId = Guid.NewGuid();
            _areaId = Guid.NewGuid();

            SetupTypesCacheMock<OrganizationType>();
            SetupTypesCacheMock<AreaInformationType>();
            SetupTypesCacheMock<AreaType>();

            // unitOfWork
            var areaMunicipalityRepoMock = new Mock<IAreaMunicipalityRepository>();
            areaMunicipalityRepoMock.Setup(g => g.All()).Returns(EntityGenerator.GetAreaMunicipalityList(_areaId, _municipalityId).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaMunicipalityRepository>()).Returns(areaMunicipalityRepoMock.Object);

            _areaRepoMock = new Mock<IAreaRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaRepository>()).Returns(_areaRepoMock.Object);
            
            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<OrganizationVersioned, V8VmOpenApiOrganizationItem>(It.IsAny<IList<OrganizationVersioned>>()))
                .Returns((List<OrganizationVersioned> collection) =>
                {
                    var list = new List<V8VmOpenApiOrganizationItem>();
                    collection.ForEach(i => list.Add(new V8VmOpenApiOrganizationItem { Id = i.UnificRootId }));
                    return list;
                });
        }

        [Fact]
        public void NoEntitiesFound()
        {
            // Arrange            
            // service
            var service = ArrangeAndGetService(EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache));

            // Act
            var result = service.GetOrganizationsByMunicipality(Guid.NewGuid(), null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OrganizationsFound_TypeIsMunicipality()
        {
            // Arrange
            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.MunicipalityId = _municipalityId;
            publishedOrganization.TypeId = TypeCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString());

            // service
            var service = ArrangeAndGetService(organizations);
            
            // Act
            var result = service.GetOrganizationsByMunicipality(_municipalityId, null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void OrganizationsFound_TypeIsNotMunicipality_WholeCountry()
        {
            // Arrange
            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            publishedOrganization.TypeId = TypeCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            
            // service
            var service = ArrangeAndGetService(organizations);

            // Act
            var result = service.GetOrganizationsByMunicipality(_municipalityId, null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }
        
        [Fact]
        public void OrganizationsFound_TypeIsNotMunicipality_WholeCountryExceptAland_MunicipalityNotInAland()
        {
            // Arrange
            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
            publishedOrganization.TypeId = TypeCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            
            // service
            var service = ArrangeAndGetService(organizations);

            // Act
            var result = service.GetOrganizationsByMunicipality(_municipalityId, null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void OrganizationsFound_TypeIsNotMunicipality_WholeCountryButAland_MunicipalityInAland()
        {
            // Arrange
            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
            publishedOrganization.TypeId = TypeCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            
            // service
            var service = ArrangeAndGetService(organizations, true);

            // Act
            var result = service.GetOrganizationsByMunicipality(_municipalityId, null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OrganizationsFound_TypeIsNotMunicipality_AreaType_MunicipalityNotWithinArea()
        {
            // Arrange
            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());
            publishedOrganization.TypeId = TypeCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            
            // service
            var service = ArrangeAndGetService(organizations);

            // Act
            var result = service.GetOrganizationsByMunicipality(_municipalityId, null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OrganizationsFound_TypeIsNotMunicipality_AreaType_OrganizationMunicipalityWithinArea()
        {
            // Arrange
            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());
            publishedOrganization.TypeId = TypeCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            publishedOrganization.OrganizationAreaMunicipalities = new List<OrganizationAreaMunicipality> { new OrganizationAreaMunicipality
            {
                MunicipalityId = _municipalityId
            } };

            // service
            var service = ArrangeAndGetService(organizations);

            // Act
            var result = service.GetOrganizationsByMunicipality(_municipalityId, null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count().Should().Be(1);
        }

        [Fact]
        public void OrganizationsFound_TypeIsNotMunicipality_AreaType_OrganizationAreaMunicipalityWithinArea()
        {
            // Arrange
            var organizations = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());
            publishedOrganization.TypeId = TypeCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            publishedOrganization.OrganizationAreas = new List<OrganizationArea> { new OrganizationArea
            {
                Area = new Area{AreaMunicipalities = new List<AreaMunicipality>{ new AreaMunicipality { MunicipalityId = _municipalityId} }}
            } };

            // service
            var service = ArrangeAndGetService(organizations);

            // Act
            var result = service.GetOrganizationsByMunicipality(_municipalityId, null, 1, 1, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count().Should().Be(1);
        }

        private DataAccess.Services.OrganizationService ArrangeAndGetService(IList<OrganizationVersioned> organizations, bool isAland = false)
        {
            var publishedOrganization = organizations.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();

            if (isAland)
            {
                _areaRepoMock.Setup(g => g.All()).Returns(new List<Area>() { new Area
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
            
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> list, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyFilters) =>
               {
                   return list;
               });

            var organizationMock = new Mock<IOrganizationVersionedRepository>();
            organizationMock.Setup(o => o.All()).Returns(organizations.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationVersioned>>()).Returns(organizationMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            // service
            return new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor);
        }
    }
}
