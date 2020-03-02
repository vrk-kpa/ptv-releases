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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class CheckChannelsTests : ChannelServiceTestBase
    {
        private IChannelService _service;

        public CheckChannelsTests()
        {
            SetupTypesCacheMock<ServiceChannelType>();
            SetupTypesCacheMock<ServiceChannelConnectionType>();

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache,
                VersioningManager, UserOrganizationChecker, LanguageOrderCache, null, null, null);
        }

        [Fact]
        public void IdListIsNull()
        {
            // Act
            var result = _service.CheckChannels(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IdListCountIsZero()
        {
            // Act
            var result = _service.CheckChannels(new List<Guid>());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void CanReturnNotExistingChannelsAndServiceLocationChannels()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var guidList = new List<Guid> { rootId1, rootId2 };
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId1);
            entity.TypeId = TypeCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            var entityList = new List<ServiceChannelVersioned> { entity };
            ChannelRepoMock.Setup(s => s.All()).Returns(entityList.AsQueryable());

            // Act
            var result = _service.CheckChannels(guidList);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiConnectionChannels>(result);
            vmResult.NotExistingChannels.Should().NotBeNullOrEmpty();
            vmResult.NotExistingChannels.Count.Should().Be(1);
            vmResult.NotExistingChannels.First().Should().Be(rootId2);
            vmResult.ServiceLocationChannels.Should().NotBeNullOrEmpty();
            vmResult.ServiceLocationChannels.Count.Should().Be(1);
            vmResult.ServiceLocationChannels.First().Should().Be(rootId1);
        }

        [Fact]
        public void UserOrganizationsSet_NotVisibleForAll_NotOwnOrganization()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var guidList = new List<Guid> { rootId };
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId);
            entity.TypeId = TypeCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            entity.ConnectionTypeId = TypeCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            var entityList = new List<ServiceChannelVersioned> { entity };
            ChannelRepoMock.Setup(s => s.All()).Returns(entityList.AsQueryable());

            // Act
            var result = _service.CheckChannels(guidList, new List<Guid> { orgId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiConnectionChannels>(result);
            vmResult.NotExistingChannels.Should().NotBeNullOrEmpty();
            vmResult.NotExistingChannels.Count.Should().Be(1);
            vmResult.NotExistingChannels.First().Should().Be(rootId);
            vmResult.ServiceLocationChannels.Should().BeNullOrEmpty();
        }

        [Fact]
        public void UserOrganizationsSet_NotVisibleForAll_OwnOrganization()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var guidList = new List<Guid> { rootId };
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId);
            entity.TypeId = TypeCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            entity.ConnectionTypeId = TypeCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            entity.OrganizationId = orgId;
            var entityList = new List<ServiceChannelVersioned> { entity };
            ChannelRepoMock.Setup(s => s.All()).Returns(entityList.AsQueryable());

            // Act
            var result = _service.CheckChannels(guidList, new List<Guid> { orgId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiConnectionChannels>(result);
            vmResult.NotExistingChannels.Should().BeNullOrEmpty();
            vmResult.ServiceLocationChannels.Should().NotBeNullOrEmpty();
            vmResult.ServiceLocationChannels.Count.Should().Be(1);
            vmResult.ServiceLocationChannels.First().Should().Be(rootId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UserOrganizationsSet_VisibleForAll(bool setUserOrganization)
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var guidList = new List<Guid> { rootId };
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId);
            entity.TypeId = TypeCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            entity.ConnectionTypeId = TypeCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
            if (setUserOrganization)
            {
                entity.OrganizationId = orgId;
            }

            var entityList = new List<ServiceChannelVersioned> { entity };
            ChannelRepoMock.Setup(s => s.All()).Returns(entityList.AsQueryable());

            // Act
            var result = _service.CheckChannels(guidList, new List<Guid> { orgId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiConnectionChannels>(result);
            vmResult.NotExistingChannels.Should().BeNullOrEmpty();
            vmResult.ServiceLocationChannels.Should().NotBeNullOrEmpty();
            vmResult.ServiceLocationChannels.Count.Should().Be(1);
            vmResult.ServiceLocationChannels.First().Should().Be(rootId);
        }
    }
}
