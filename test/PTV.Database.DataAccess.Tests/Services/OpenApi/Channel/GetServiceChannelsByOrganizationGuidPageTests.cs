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
using PTV.Database.DataAccess.Interfaces.Services;
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

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelsByOrganizationGuidPageTests : ChannelServiceTestBase
    {
        private Guid _orgId;
        private Mock<IRepository<ServiceChannelVersioned>> _channelRepoMock;
        private IChannelService _service;

        public GetServiceChannelsByOrganizationGuidPageTests()
        {
            _orgId = Guid.NewGuid();

            SetupTypesCacheMock<ServiceChannelType>();

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            _channelRepoMock = new Mock<IRepository<ServiceChannelVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(_channelRepoMock.Object);
            var laRepoMock = new Mock<IRepository<ServiceChannelLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelLanguageAvailability>>()).Returns(laRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<ServiceChannelName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelName>>()).Returns(nameRepoMock.Object);
            var trackingRepoMock = new Mock<ITrackingServiceServiceChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ITrackingServiceServiceChannelRepository>()).Returns(trackingRepoMock.Object);

            _service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache,
                VersioningManager, UserOrganizationChecker, LanguageOrderCache, null);
        }

        [Fact]
        public void PageNumberAsZero()
        {
            // Act
            var result = _service.GetServiceChannelsByOrganization(_orgId, null, 0, 10);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNull();
            _channelRepoMock.Verify(x => x.All(), Times.Never);
        }
        
        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.ServiceLocation)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        [InlineData(null)]
        public void CanGetChannels(ServiceChannelTypeEnum? type)
        {
            var count = 10;
            var entityList = EntityGenerator.GetServiceChannelEntityList(count, PublishingStatusCache).ToList();
            entityList.ForEach(i =>
            {
                i.OrganizationId = _orgId;
                i.TypeId = TypeCache.Get<ServiceChannelType>(type.HasValue ? type.ToString() : ServiceChannelTypeEnum.Phone.ToString());
            });

            _channelRepoMock.Setup(g => g.All()).Returns(entityList.AsQueryable());

            // Act
            var result = _service.GetServiceChannelsByOrganization(_orgId, null, 1, 10, type);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Count.Should().Be(count);
            _channelRepoMock.Verify(x => x.All(), Times.Once);
        }

        [Fact]
        public void DateDefinedAsNow()
        {
            var date = DateTime.Now;
            var count = 10;
            var type = ServiceChannelTypeEnum.WebPage;
            var entityList = EntityGenerator.GetServiceChannelEntityList(count, PublishingStatusCache).ToList();
            entityList.ForEach(i =>
            {
                i.OrganizationId = _orgId;
                i.TypeId = TypeCache.Get<ServiceChannelType>(type.ToString());
            });

            _channelRepoMock.Setup(g => g.All()).Returns(entityList.AsQueryable());

            // Act
            var result = _service.GetServiceChannelsByOrganization(_orgId, date, 1, 10, type);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNull();
            _channelRepoMock.Verify(x => x.All(), Times.Once);
            unitOfWorkMockSetup.Verify(x => x.ApplyIncludes(It.IsAny<IQueryable<ServiceChannelVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
               It.IsAny<bool>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiItem>(It.IsAny<List<ServiceChannelVersioned>>()), Times.Never);
        }
    }
}
