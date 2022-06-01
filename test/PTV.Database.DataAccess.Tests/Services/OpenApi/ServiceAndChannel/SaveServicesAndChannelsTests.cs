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
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.ServiceAndChannel
{
    public class SaveServicesAndChannelsTests : ServiceAndChannelTestBase
    {
        private ServiceAndChannelService _service;

        public SaveServicesAndChannelsTests()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);
            _service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object,
                translationManagerVModelMockSetup.Object, Logger, serviceUtilities, ServiceService, ChannelService,
                PublishingStatusCache, UserOrganizationChecker, VersioningManager, TypeCache, AddressService, CacheManager.PostalCodeCache, UrlServiceMock.Object);
        }
        [Fact]
        public void ModelIsNull()
        {
            // Act
            var result = _service.SaveServicesAndChannels(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        [Fact]
        public void ListIncludesNull()
        {
            // Act
            var result = _service.SaveServicesAndChannels(new List<V11VmOpenApiServiceServiceChannelAstiInBase> { null });

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<string>>();
        }

        [Fact]
        public void ChannelIdIsNull()
        {
            // Act
            var result = _service.SaveServicesAndChannels(new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase() });

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
            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns((PublishingStatus?)null);

            // Act
            var result = _service.SaveServicesAndChannels(new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
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

            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns(PublishingStatus.Modified);

            // Act
            var result = _service.SaveServicesAndChannels(new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
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

            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns(PublishingStatus.Published);

            ChannelServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(channelId)).Returns(PublishingStatus.Published);
            //ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true)).Returns((VmOpenApiServiceChannel)null);

            // Act
            var result = _service.SaveServicesAndChannels(new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = serviceId.ToString(),
                ServiceChannelId = channelId.ToString()
            } });

            // Assert
            result.Should().NotBeNull();
            var list = Assert.IsType<List<string>>(result);
            list.Count.Should().Be(1);
            list.FirstOrDefault().Should().EndWith("not found!");
            ServiceServiceMock.Verify(x => x.GetLatestVersionPublishingStatus(serviceId), Times.Once());
            ChannelServiceMock.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once());
        }

        [Fact]
        public void ChannelNotVisibleForUser()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();

            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns(PublishingStatus.Published);

            ChannelServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(channelId)).Returns(PublishingStatus.Published);
            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel { Id = channelId, IsVisibleForAll = false, OrganizationId = organizationId });

            UserOrganizationServiceMock.Setup(s => s.GetAllUserOrganizationIds(null)).Returns(new List<Guid>());
            ServiceRepoMock.Setup(g => g.All()).Returns((new List<ServiceVersioned> { new ServiceVersioned
            {
                UnificRootId = serviceId,
                LanguageAvailabilities = new List<ServiceLanguageAvailability>
                {
                    new ServiceLanguageAvailability{LanguageId = LanguageCache.Get("fi")}
                }
            } }).AsQueryable());
            ServiceChannelRepoMock.Setup(g => g.All()).Returns((new List<ServiceChannelVersioned>{ new ServiceChannelVersioned
            {
                UnificRootId = channelId,
                LanguageAvailabilities = new List<ServiceChannelLanguageAvailability>
                {
                    new ServiceChannelLanguageAvailability{LanguageId = LanguageCache.Get("fi")}
                }
            } }).AsQueryable());
            // Act
            var result = _service.SaveServicesAndChannels(new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
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

        [Theory]
        [InlineData("added or updated.", "fi", "fi", 1)]
        [InlineData("ignored, because connection has no common language.", "fi", "sv", 0)]
        public void CanModify(string message, string serviceLang, string channelLang, int useTranslatorCount)
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var organizationId = Guid.NewGuid();
            var relation = new V11VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = serviceId.ToString(),
                ServiceChannelId = channelId.ToString()
            };

            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns(PublishingStatus.Published);

            ChannelServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(channelId)).Returns(PublishingStatus.Published);
            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel
                {
                    Id = channelId,
                    IsVisibleForAll = false,
                    OrganizationId = organizationId,
                    ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString(),
                    Security = new VmSecurityOwnOrganization { IsOwnOrganization = true }
                });

            translationManagerVModelMockSetup.Setup(s => s.Translate<V11VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(relation, unitOfWorkMockSetup.Object))
                .Returns(new ServiceServiceChannel());

            ServiceRepoMock.Setup(g => g.All()).Returns((new List<ServiceVersioned> { new ServiceVersioned
            {
                UnificRootId = serviceId,
                LanguageAvailabilities = new List<ServiceLanguageAvailability>
                {
                    new ServiceLanguageAvailability{LanguageId = LanguageCache.Get(serviceLang)}
                }
            } }).AsQueryable());
            ServiceChannelRepoMock.Setup(g => g.All()).Returns((new List<ServiceChannelVersioned>{ new ServiceChannelVersioned
            {
                UnificRootId = channelId,
                LanguageAvailabilities = new List<ServiceChannelLanguageAvailability>
                {
                    new ServiceChannelLanguageAvailability{LanguageId = LanguageCache.Get(channelLang)}
                }
            } }).AsQueryable());
            
            // Act
            var result = _service.SaveServicesAndChannels(new List<V11VmOpenApiServiceServiceChannelAstiInBase> { relation });

            // Assert
            result.Should().NotBeNull();
            var list = Assert.IsType<List<string>>(result);
            list.Count.Should().Be(1);
            list.FirstOrDefault().Should().EndWith(message);
            translationManagerVModelMockSetup.Verify(x => x.Translate<V11VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(It.IsAny<V11VmOpenApiServiceServiceChannelAstiInBase>(), unitOfWorkMockSetup.Object), Times.Exactly(useTranslatorCount));
        }
    }
}
