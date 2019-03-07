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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationsHierarchyTests : OrganizationServiceTestBase
    {
        [Theory]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(2, 1, 1, 1, 0)]
        [InlineData(1, 1, 2, 2, 1)]
        [InlineData(2, 1, 2, 2, 1)]
        [InlineData(3, 1, 2, 2, 0)]
        [InlineData(1, 2, 2, 1, 2)]
        [InlineData(2, 2, 2, 1, 0)]
        public void NoDateDefined(int pageNumber, int pageSize, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetOrganizationEntityList(count, PublishingStatusCache).ToList();

            var result = ArrangeAndAct(null, list, pageNumber, pageSize);

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
        public void NoRootOrganizationsFound()
        {
            // Arrange
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache).ToList();
            // Set the parent organization
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var publishedItem = list.FirstOrDefault(o => o.PublishingStatusId == publishedId);
            var parentId = Guid.NewGuid();
            publishedItem.ParentId = parentId;
            publishedItem.Parent = new Model.Models.Organization { Id = parentId };
            publishedItem.Parent.Versions.Add(new OrganizationVersioned { UnificRootId = parentId, PublishingStatusId = publishedId });

            var result = ArrangeAndAct(null, list, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNull();
        }

        private IVmOpenApiGuidPageVersionBase<VmOpenApiItem> ArrangeAndAct(
            DateTime? date,
            List<OrganizationVersioned> entityList,
            int pageNumber,
            int pageSize,
            DateTime? dateBefore = null)
        {
            // unitOfWork
            var organizationMock = new Mock<IRepository<OrganizationVersioned>>();
            organizationMock.Setup(o => o.All()).Returns(entityList.AsQueryable());
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
            return service.GetOrganizationsHierarchy(date, pageNumber, pageSize, dateBefore);
        }
    }
}
