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
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Common
{
    public class GetServicesAndChannelsByOrganizationTests : CommonServiceTestBase
    {
        private Guid _organizationId;

        public GetServicesAndChannelsByOrganizationTests()
        {
            _organizationId = Guid.NewGuid();
        }

        [Fact]
        public void NoEntitesFound()
        {
            // Arrange
            // unitOfWork
            var serviceMock = new Mock<IServiceVersionedRepository>();
            serviceMock.Setup(o => o.All()).Returns(EntityGenerator.GetServiceEntityList(0, PublishingStatusCache).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(serviceMock.Object);

            var channelMock = new Mock<IServiceChannelVersionedRepository>();
            channelMock.Setup(o => o.All()).Returns(new List<ServiceChannelVersioned>().AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(channelMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            // common service
            var service = new CommonService(translationManagerMockSetup.Object, TranslationManagerVModel, contextManager, CacheManager.TypesCache, PublishingStatusCache,
                UserOrganizationChecker, DataServiceFetcherMock, serviceUtilities, VersioningManager, ApplicationConfigurationMock, HttpContextAccessorMock);

            // Act
            var result = service.GetServicesAndChannelsByOrganization(Guid.NewGuid(), null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ServicesFound()
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.OrganizationId = _organizationId;

            // unitOfWork
            var serviceMock = new Mock<IServiceVersionedRepository>();
            serviceMock.Setup(o => o.All()).Returns(services.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(serviceMock.Object);

            var channelMock = new Mock<IServiceChannelVersionedRepository>();
            channelMock.Setup(o => o.All()).Returns(new List<ServiceChannelVersioned>().AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(channelMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<EntityBase, VmOpenApiEntityItem>(It.IsAny<IList<EntityBase>>()))
                .Returns(new List<VmOpenApiEntityItem>() { new VmOpenApiEntityItem() });

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            // common service
            var service = new CommonService(translationManagerMockSetup.Object, TranslationManagerVModel, contextManager, CacheManager.TypesCache, PublishingStatusCache,
                UserOrganizationChecker, DataServiceFetcherMock, serviceUtilities, VersioningManager, ApplicationConfigurationMock, HttpContextAccessorMock);

            // Act
            var result = service.GetServicesAndChannelsByOrganization(_organizationId, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().NotBeNullOrEmpty();
            (result as VmOpenApiEntityGuidPage).ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ChannelsFound()
        {
            // Arrange
            var channels = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            var publishedChannel = channels.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.OrganizationId = _organizationId;

            // unitOfWork
            var serviceMock = new Mock<IServiceVersionedRepository>();
            serviceMock.Setup(o => o.All()).Returns(EntityGenerator.GetServiceEntityList(0, PublishingStatusCache).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(serviceMock.Object);

            var channelMock = new Mock<IServiceChannelVersionedRepository>();
            channelMock.Setup(o => o.All()).Returns(channels.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(channelMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<EntityBase, VmOpenApiEntityItem>(It.IsAny<IList<EntityBase>>()))
                .Returns(new List<VmOpenApiEntityItem>() { new VmOpenApiEntityItem() });

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            // common service
            var service = new CommonService(translationManagerMockSetup.Object, TranslationManagerVModel, contextManager, CacheManager.TypesCache, PublishingStatusCache,
                UserOrganizationChecker, DataServiceFetcherMock, serviceUtilities, VersioningManager, ApplicationConfigurationMock, HttpContextAccessorMock);

            // Act
            var result = service.GetServicesAndChannelsByOrganization(_organizationId, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().NotBeNullOrEmpty();
            (result as VmOpenApiEntityGuidPage).ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ServicesAndChannelsFound()
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.OrganizationId = _organizationId;
            var channels = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            var publishedChannel = channels.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.OrganizationId = _organizationId;

            // unitOfWork
            var serviceMock = new Mock<IServiceVersionedRepository>();
            serviceMock.Setup(o => o.All()).Returns(services.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(serviceMock.Object);

            var channelMock = new Mock<IServiceChannelVersionedRepository>();
            channelMock.Setup(o => o.All()).Returns(channels.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(channelMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<EntityBase, VmOpenApiEntityItem>(It.IsAny<IList<EntityBase>>()))
                .Returns(new List<VmOpenApiEntityItem>() { new VmOpenApiEntityItem() });

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            // common service
            var service = new CommonService(translationManagerMockSetup.Object, TranslationManagerVModel, contextManager, CacheManager.TypesCache, PublishingStatusCache,
                UserOrganizationChecker, DataServiceFetcherMock, serviceUtilities, VersioningManager, ApplicationConfigurationMock, HttpContextAccessorMock);

            // Act
            var result = service.GetServicesAndChannelsByOrganization(_organizationId, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(2);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().NotBeNullOrEmpty();
            (result as VmOpenApiEntityGuidPage).ItemList.Count.Should().Be(1);
        }
    }
}
