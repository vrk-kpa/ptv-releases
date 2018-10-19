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
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelsTests : ChannelServiceTestBase
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
            var list = EntityGenerator.GetServiceChannelEntityList(count, PublishingStatusCache).ToList();

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
            var list = EntityGenerator.GetServiceChannelEntityList(count, PublishingStatusCache).ToList();

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
            var list = EntityGenerator.GetServiceChannelEntityList(count, PublishingStatusCache).ToList();

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
            var list = EntityGenerator.GetServiceChannelEntityList(count, PublishingStatusCache).ToList();

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
            // Let's create two versions for same root channel
            var deletedItem = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Deleted), rootId);
            var oldPublishedItem = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.OldPublished), rootId);
            var list = new List<ServiceChannelVersioned> { deletedItem, oldPublishedItem };

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
            // Let's create two versions for same root channel
            var publishedItem = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId);
            var modifiedItem = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId);
            var list = new List<ServiceChannelVersioned> { publishedItem, modifiedItem };

            var result = ArrangeAndAct(null, list, 1, 1, EntityStatusExtendedEnum.Active);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);// Only one item per root should be found
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Count.Should().Be(1);
        }

        private IVmOpenApiGuidPageVersionBase ArrangeAndAct(DateTime? date, List<ServiceChannelVersioned> entityList, int pageNumber, int pageSize, EntityStatusExtendedEnum status)
        {
            // unitOfWork
            var channelRepoMock = new Mock<IRepository<ServiceChannelVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(channelRepoMock.Object);
            channelRepoMock.Setup(g => g.All()).Returns(entityList.AsQueryable());

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<ServiceChannelVersioned>>(),
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<ServiceChannelVersioned> items, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
                {
                    return items;
                });

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceChannelVersioned, VmOpenApiItem>(It.IsAny<IList<ServiceChannelVersioned>>()))
                .Returns((ICollection<ServiceChannelVersioned> collection) =>
                {
                    var list = new List<VmOpenApiItem>();
                    if (collection?.Count > 0)
                    {
                        collection.ToList().ForEach(c => list.Add(new VmOpenApiItem
                        {
                            Id = c.UnificRootId,
                        }));
                    }
                    return list;
                });

            var translationManagerMock = translationManagerMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            var service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker, LanguageOrderCache, CloningManager);

            // Act
            return service.GetServiceChannels(date, pageNumber, pageSize, status);            
        }

    }
}
