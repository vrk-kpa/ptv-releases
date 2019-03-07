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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationsTests : OrganizationServiceTestBase
    {
        public GetOrganizationsTests()
        {            
        }

        [Theory]
        [InlineData(1, 1, true, 1, 2, 1)]
        [InlineData(1, 1, false, 1, 1, 1)]
        [InlineData(2, 1, true, 1, 2, 1)]
        [InlineData(2, 1, false, 1, 1, 0)]
        [InlineData(1, 1, true, 2, 4, 1)]
        [InlineData(1, 1, false, 2, 2, 1)]
        [InlineData(2, 1, true, 2, 4, 1)]
        [InlineData(2, 1, false, 2, 2, 1)]
        [InlineData(3, 1, true, 2, 4, 1)]
        [InlineData(5, 1, true, 2, 4, 0)]
        [InlineData(3, 1, false, 2, 2, 0)]
        [InlineData(1, 2, true, 2, 2, 2)]
        [InlineData(1, 2, false, 2, 1, 2)]
        [InlineData(2, 2, true, 2, 2, 2)]
        [InlineData(3, 2, true, 2, 2, 0)]
        [InlineData(2, 2, false, 2, 1, 0)]
        public void NoDateDefined(int pageNumber, int pageSize, bool archived, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetOrganizationEntityList(count, PublishingStatusCache).ToList();
            
            var result = ArrangeAndAct(null, list, pageNumber, pageSize, archived ? EntityStatusEnum.Archived : EntityStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(expectedPageCount);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
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
        [InlineData(true)]
        [InlineData(false)]
        public void DateDefinedAsNow(bool archived)
        {
            var date = DateTime.Now;
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            
            var result = ArrangeAndAct(date, list, 1, 1, archived ? EntityStatusEnum.Archived : EntityStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNull();
        }

        [Fact]
        public void DateDefinedAsYearAgo_Published()
        {
            var date = DateTime.Now.AddYears(-1);
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            
            var result = ArrangeAndAct(date, list, 1, 1, EntityStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void DateDefinedAsYearAgo_Archived()
        {
            var date = DateTime.Now.AddYears(-1);
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            
            var result = ArrangeAndAct(date, list, 1, 1, EntityStatusEnum.Archived);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(2);// Deleted and OldPublished are found
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void NoChannelsAttached()
        {
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache, true, false);
            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void NoServicesAttached()
        {
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache, false, true)
                .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId)).ToList();
            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void NoServicesOrChannelsAttached()
        {
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache, false, false)
                .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId)).ToList();
            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNull();
        }

        [Fact]
        public void Archived_OnlyOnePerRootIdReturned()
        {
            var rootId = Guid.NewGuid();
            // Let's create two version for same root organization
            var deletedItem = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Deleted), rootId);
            var oldPublishedItem = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.OldPublished), rootId);
            var list = new List<OrganizationVersioned> { deletedItem, oldPublishedItem };
            
            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Archived);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);// Only one item per root should be found
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void DateBeforeDefinedAsHourAgo_Published()
        {
            var date = DateTime.Now.AddHours(-1);
            // items created yesterday (DateTime.Now.AddDays(-1))
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);

            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Published, dateBefore: date);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void DateBeforeDefinedAsHourAgo_Archived()
        {
            var date = DateTime.Now.AddHours(-1);
            // items created yesterday (DateTime.Now.AddDays(-1))
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);

            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Archived, dateBefore: date);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(2);// Deleted and OldPublished are found
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void DateBeforeDefinedAsTwoDaysAgo_Published()
        {
            var date = DateTime.Now.AddDays(-2);
            // items created yesterday (DateTime.Now.AddDays(-1))
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);

            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Published, dateBefore: date);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
        }

        [Fact]
        public void DateBeforeDefinedAsTwoDaysAgo_Archived()
        {
            var date = DateTime.Now.AddDays(-2);
            // items created yesterday (DateTime.Now.AddDays(-1))
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);

            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusEnum.Archived, dateBefore: date);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
        }

        [Fact]
        public void DateDefinedAsNow_Withdrawn()
        {
            var date = DateTime.Now;
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);

            var result = ArrangeAndAct(date, list, 1, 1, EntityStatusEnum.Withdrawn);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V8VmOpenApiOrganizationGuidPage>(result);
            vmResult.ItemList.Should().BeNull();
        }

        private IVmOpenApiGuidPageVersionBase<V8VmOpenApiOrganizationItem> ArrangeAndAct(DateTime? date, List<OrganizationVersioned> entityList, int pageNumber, int pageSize,
            EntityStatusEnum status, int version = 8, DateTime? dateBefore = null)
        {
            // unitOfWork
            var organizationMock = new Mock<IRepository<OrganizationVersioned>>();
            organizationMock.Setup(g => g.All()).Returns(entityList.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationVersioned>>()).Returns(organizationMock.Object);
            var laRepoMock = new Mock<IRepository<OrganizationLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationLanguageAvailability>>()).Returns(laRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<OrganizationName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationName>>()).Returns(nameRepoMock.Object);
            
            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var translationManagerMock = translationManagerMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor);

            // Act
            return service.GetOrganizations(date, version, status, pageNumber, pageSize, dateBefore);
        }

    }
}
