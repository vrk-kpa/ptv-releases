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
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class GetServicesTests : ServiceServiceTestBase
    {
        [Theory]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(2, 1, 1, 1, 0)]
        [InlineData(2, 1, 2, 2, 1)]
        [InlineData(3, 1, 2, 2, 0)]
        [InlineData(1, 2, 2, 1, 2)]
        [InlineData(2, 2, 2, 1, 0)]
        public void NoDateDefined_Published(int pageNumber, int pageSize, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetServiceEntityList(count, PublishingStatusCache).ToList();

            var result = ArrangeAndAct(null, list, pageNumber, pageSize, EntityStatusExtendedEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(expectedPageCount);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            if (expectedItemCountOnPage == 0)
            {
                vmResult.ItemList.Should().BeNull();
            }
            else
            {
                vmResult.ItemList.Count.Should().Be(expectedItemCountOnPage);
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 2, 1)]
        [InlineData(2, 1, 1, 2, 1)]
        [InlineData(2, 1, 2, 4, 1)]
        [InlineData(3, 1, 2, 4, 1)]
        [InlineData(5, 1, 2, 4, 0)]
        [InlineData(1, 2, 2, 2, 2)]
        [InlineData(2, 2, 2, 2, 2)]
        [InlineData(3, 2, 2, 2, 0)]
        public void NoDateDefined_Archived(int pageNumber, int pageSize, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetServiceEntityList(count, PublishingStatusCache).ToList();

            var result = ArrangeAndAct(null, list, pageNumber, pageSize, EntityStatusExtendedEnum.Archived);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(expectedPageCount);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            if (expectedItemCountOnPage == 0)
            {
                vmResult.ItemList.Should().BeNull();
            }
            else
            {
                vmResult.ItemList.Count.Should().Be(expectedItemCountOnPage);
            }
        }

        [Theory]
        [InlineData(2, 2, 1, 2, 1)]
        [InlineData(3, 2, 2, 3, 2)]
        [InlineData(4, 2, 2, 3, 0)]
        public void NoDateDefined_Active(int pageNumber, int pageSize, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetServiceEntityList(count, PublishingStatusCache).ToList();

            var result = ArrangeAndAct(null, list, pageNumber, pageSize, EntityStatusExtendedEnum.Active);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(expectedPageCount);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            if (expectedItemCountOnPage == 0)
            {
                vmResult.ItemList.Should().BeNull();
            }
            else
            {
                vmResult.ItemList.Count.Should().Be(expectedItemCountOnPage);
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(2, 1, 1, 1, 0)]
        [InlineData(2, 1, 2, 2, 1)]
        [InlineData(3, 1, 2, 2, 0)]
        [InlineData(1, 2, 2, 1, 2)]
        [InlineData(2, 2, 2, 1, 0)]
        public void NoDateDefined_Withdrawn(int pageNumber, int pageSize, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetServiceEntityList(count, PublishingStatusCache).ToList();

            var result = ArrangeAndAct(null, list, pageNumber, pageSize, EntityStatusExtendedEnum.Withdrawn);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(expectedPageCount);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            if (expectedItemCountOnPage == 0)
            {
                vmResult.ItemList.Should().BeNull();
            }
            else
            {
                vmResult.ItemList.Count.Should().Be(expectedItemCountOnPage);
            }
        }

        [Fact]
        public void Archived_OnlyOnePerRootIdReturned()
        {
            var rootId = Guid.NewGuid();
            // Let's create two version for same root service
            var deletedItem = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Deleted), rootId);
            var oldPublishedItem = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.OldPublished), rootId);
            var list = new List<ServiceVersioned> { deletedItem, oldPublishedItem };

            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusExtendedEnum.Archived);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);// Only one item per root should be found
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void Active_OnlyOnePerRootIdReturned()
        {
            var rootId = Guid.NewGuid();
            // Let's create two version for same root service
            var publishedItem = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId);
            var modifiedItem = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId);
            var list = new List<ServiceVersioned> { publishedItem, modifiedItem };

            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusExtendedEnum.Active);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);// Only one item per root should be found
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Count.Should().Be(1);
        }

        private IVmOpenApiModelWithPagingBase<VmOpenApiItem> ArrangeAndAct(DateTime? date, List<ServiceVersioned> entityList, int pageNumber, int pageSize, EntityStatusExtendedEnum status)
        {
            // unitOfWork
            var serviceRepoMock = new Mock<IRepository<ServiceVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(serviceRepoMock.Object);
            serviceRepoMock.Setup(g => g.All()).Returns(entityList.AsQueryable());
            var laRepoMock = new Mock<IRepository<ServiceLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceLanguageAvailability>>()).Returns(laRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<ServiceName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceName>>()).Returns(nameRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var translationManagerMock = translationManagerMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, null, null, null, null);

            // Act
            return service.GetServices(date, pageNumber, pageSize, status);
        }
    }
}
