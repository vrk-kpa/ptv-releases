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
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.ServiceAndChannel
{
    public class SaveServicesAndChannelsTests : ServiceAndChannelTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        [Fact]
        public void ListIncludesNull()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(new List<V7VmOpenApiServiceServiceChannelAstiInBase> { null });

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        [Fact]
        public void ChannelIdIsNull()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(new List<V7VmOpenApiServiceServiceChannelAstiInBase> { new V7VmOpenApiServiceServiceChannelAstiInBase() });

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        [Fact]
        public void CurrentVersionForServiceNotFound()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            ServiceServiceMock.Setup(s => s.GetServiceByIdSimple(serviceId, true)).Returns((VmOpenApiServiceVersionBase)null);

            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceServiceMock.Object, ChannelService, PublishingStatusCache,
                UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(new List<V7VmOpenApiServiceServiceChannelAstiInBase> { new V7VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = serviceId.ToString(),
                ServiceChannelId = channelId.ToString()
            } });

            // Assert
            result.Should().NotBeNull();
            var list = Assert.IsType<List<string>>(result);
            list.Count.Should().Be(1);
            list.FirstOrDefault().Should().EndWith("not found!");
        }

        [Fact]
        public void RightPublishingStatusForServiceNotFound()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            ServiceServiceMock.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { PublishingStatus = PublishingStatus.Modified.ToString() });

            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceServiceMock.Object, ChannelService, PublishingStatusCache,
                UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(new List<V7VmOpenApiServiceServiceChannelAstiInBase> { new V7VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = serviceId.ToString(),
                ServiceChannelId = channelId.ToString()
            } });

            // Assert
            result.Should().NotBeNull();
            var list = Assert.IsType<List<string>>(result);
            list.Count.Should().Be(1);
            list.FirstOrDefault().Should().EndWith("You cannot update service!");
        }

        [Fact]
        public void ChannelNotFound()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            ServiceServiceMock.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { PublishingStatus = PublishingStatus.Published.ToString() });

            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true)).Returns((VmOpenApiServiceChannel)null);

            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceServiceMock.Object, ChannelService, PublishingStatusCache,
                UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(new List<V7VmOpenApiServiceServiceChannelAstiInBase> { new V7VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = serviceId.ToString(),
                ServiceChannelId = channelId.ToString()
            } });

            // Assert
            result.Should().NotBeNull();
            var list = Assert.IsType<List<string>>(result);
            list.Count.Should().Be(1);
            list.FirstOrDefault().Should().EndWith("not found!");
            ServiceServiceMock.Verify(x => x.GetServiceByIdSimple(serviceId, true), Times.Once());
            ChannelServiceMock.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once());
        }

        [Fact]
        public void ChannelNotVisibleForUser()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            ServiceServiceMock.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { PublishingStatus = PublishingStatus.Published.ToString() });

            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel() { Id = channelId, IsVisibleForAll = false, OrganizationId = organizationId });

            UserOrganizationServiceMock.Setup(s => s.GetAllUserOrganizations(unitOfWork)).Returns(new List<Guid>());

            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceServiceMock.Object, ChannelService, PublishingStatusCache,
                UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(new List<V7VmOpenApiServiceServiceChannelAstiInBase> { new V7VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = serviceId.ToString(),
                ServiceChannelId = channelId.ToString()
            } });

            // Assert
            result.Should().NotBeNull();
            var list = Assert.IsType<List<string>>(result);
            list.Count.Should().Be(1);
            list.FirstOrDefault().Should().EndWith("You do not have permission to update data!");
        }

        [Fact]
        public void CanModify()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            var relation = new V7VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = serviceId.ToString(),
                ServiceChannelId = channelId.ToString()
            };
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            ServiceServiceMock.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { PublishingStatus = PublishingStatus.Published.ToString() });

            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel()
                {
                    Id = channelId,
                    IsVisibleForAll = false,
                    OrganizationId = organizationId,
                    ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString(),
                    Security = new VmSecurityOwnOrganization { IsOwnOrganization = true }
                });

            //UserOrganizationServiceMock.Setup(s => s.GetAllUserOrganizations(unitOfWork)).Returns(new List<Guid>() { organizationId });

            translationManagerVModelMockSetup.Setup(s => s.Translate<IVmOpenApiServiceServiceChannelInVersionBase, ServiceServiceChannel>(relation, unitOfWork))
                .Returns(new ServiceServiceChannel());

            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceServiceMock.Object, ChannelService, PublishingStatusCache,
                UserOrganizationChecker);

            // Act
            var result = service.SaveServicesAndChannels(new List<V7VmOpenApiServiceServiceChannelAstiInBase> { relation });

            // Assert
            result.Should().NotBeNull();
            var list = Assert.IsType<List<string>>(result);
            list.Count.Should().Be(1);
            list.FirstOrDefault().Should().EndWith("added or updated.");
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceServiceChannelInVersionBase, ServiceServiceChannel>(It.IsAny<VmOpenApiServiceServiceChannelInVersionBase>(), unitOfWork), Times.Once());
        }
    }
}
