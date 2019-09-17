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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class GetServicesByServiceClassTests : ServiceServiceTestBase
    {
        private ServiceService _service;
        private Mock<IStatutoryServiceServiceClassRepository> gdServiceClassRepoMock;
        private Mock<IRepository<ServiceVersioned>> serviceMock;

        public GetServicesByServiceClassTests()
        {
            gdServiceClassRepoMock = new Mock<IStatutoryServiceServiceClassRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceServiceClassRepository>()).Returns(gdServiceClassRepoMock.Object);
            serviceMock = new Mock<IRepository<ServiceVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(serviceMock.Object);
            var laRepoMock = new Mock<IRepository<ServiceLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceLanguageAvailability>>()).Returns(laRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<ServiceName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceName>>()).Returns(nameRepoMock.Object);

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<StatutoryServiceServiceClass>>(),
                It.IsAny<Func<IQueryable<StatutoryServiceServiceClass>, IQueryable<StatutoryServiceServiceClass>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<StatutoryServiceServiceClass> list, Func<IQueryable<StatutoryServiceServiceClass>, IQueryable<StatutoryServiceServiceClass>> func, bool applyFilters) =>
                {
                    return list;
                });

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            // service
            _service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache);
        }

        [Fact]
        public void NoEntitiesFound()
        {
            // Arrange            
            serviceMock.Setup(s => s.All()).Returns(EntityGenerator.GetServiceEntityList(1, PublishingStatusCache).AsQueryable());

            // Act
            var result = _service.GetServicesByServiceClass(new List<Guid> { Guid.NewGuid() }, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GeneralDescriptionServiceClassFound()
        {
            // Arrange
            var serviceClassId = Guid.NewGuid();
            var idList = new List<Guid> { serviceClassId };
            var publishedService = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            var gd = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            gd.ServiceClasses.Add(new StatutoryServiceServiceClass { ServiceClassId = serviceClassId, StatutoryServiceGeneralDescriptionVersioned = gd, StatutoryServiceGeneralDescriptionVersionedId = gd.Id });
            publishedService.StatutoryServiceGeneralDescription = gd.UnificRoot;
            publishedService.StatutoryServiceGeneralDescriptionId = gd.UnificRootId;
            gdServiceClassRepoMock.Setup(r => r.All()).Returns(new List<StatutoryServiceServiceClass> { gd.ServiceClasses.First() }.AsQueryable());
            serviceMock.Setup(s => s.All()).Returns(new List<ServiceVersioned> { publishedService }.AsQueryable());

            // Act
            var result = _service.GetServicesByServiceClass(idList, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ServiceWithServiceClassFound()
        {
            // Arrange
            var serviceClassId = Guid.NewGuid();
            var idList = new List<Guid> { serviceClassId };
            var publishedService = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            publishedService.ServiceServiceClasses.Add(new ServiceServiceClass { ServiceClassId = serviceClassId, ServiceVersioned = publishedService });
            serviceMock.Setup(s => s.All()).Returns(new List<ServiceVersioned> { publishedService }.AsQueryable());

            // Act
            var result = _service.GetServicesByServiceClass(idList, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }
    }
}
