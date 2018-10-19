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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
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
    public class GetServiceChannelsByIdListTests : ChannelServiceTestBase
    {
        private IUnitOfWorkWritable unitOfWork;
        private TestContextManager contextManager;
        private ServiceUtilities serviceUtilities;
        private IChannelService service;

        public GetServiceChannelsByIdListTests()
        {
            SetupTypesCacheMock<ServiceChannelType>();
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));

            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<ServiceChannelVersioned>>(),
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<ServiceChannelVersioned> channelServices, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
                {
                    return channelServices;
                });
            // Channel connections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceServiceChannel>>(),
               It.IsAny<Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceServiceChannel> items, Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>> func, bool applyFilters) =>
               {
                   return items;
               });
            // Related services
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceVersioned> items, Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });
            unitOfWork = unitOfWorkMockSetup.Object;
            contextManager = new TestContextManager(unitOfWork, unitOfWork);
            serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker, LanguageOrderCache, CloningManager);

        }

        [Fact]
        public void IdListIsNull()
        {
            // Act
            var result = service.GetServiceChannels(null, DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IdListIsEmpty()
        {
            // Act
            var result = service.GetServiceChannels(new List<Guid>(), DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NoPublishedChannelsFound()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();

            var list = new List<ServiceChannelVersioned>
            {
                EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId1),
                EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId2)
            };

            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            
            // Act
            var result = service.GetServiceChannels(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            ChannelRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<ServiceChannelLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetServiceChannels(new List<Guid> { publishedChannel.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            ChannelRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
        }

        [Fact]
        public void CanGetAllTypesOfChannels()
        {
            // Arrange
            var c1 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.EChannel);
            var c2 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.Phone);
            var c3 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.PrintableForm);
            var c4 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.ServiceLocation);
            var c5 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.WebPage);
            var list = new List<ServiceChannelVersioned>
            {
                c1, c2, c3, c4, c5
            };

            // repositories
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            
            ArrangeAllTranslationManager();

            // Act
            var result = service.GetServiceChannels(new List<Guid> { c1.UnificRootId, c2.UnificRootId, c3.UnificRootId, c4.UnificRootId, c5.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            ChannelRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
        }

        [Fact]
        public void CanGetRelatedServices()
        {
            // Arrange
            var c1 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.WebPage);
            c1.UnificRoot = new ServiceChannel { Id = c1.UnificRootId };
            var c2 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.EChannel);
            c2.UnificRoot = new ServiceChannel { Id = c2.UnificRootId };
            var connection = GetAndSetConnectionForChannel(c1, PublishedId, "Name");
            var list = new List<ServiceChannelVersioned>
            {
                c1, c2
            };

            // repositories
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.ToList().AsQueryable());
            
            ArrangeAllTranslationManager();

            // Act
            var result = service.GetServiceChannels(new List<Guid> { c1.UnificRootId, c2.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ChannelRepoMock.Verify(x => x.All(), Times.Once);
            var channel1 = result.Where(i => i.Id == c1.UnificRootId).FirstOrDefault();
            channel1.Should().NotBeNull();
            channel1.Services.Count.Should().Be(1);
            var channel2 = result.Where(i => i.Id == c2.UnificRootId).FirstOrDefault();
            channel2.Should().NotBeNull();
            channel2.Services.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServicesNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var statusId = PublishingStatusCache.Get(publishingStatus);
            var c1 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.WebPage);
            c1.UnificRoot = new ServiceChannel { Id = c1.UnificRootId };
            var c2 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.EChannel);
            c2.UnificRoot = new ServiceChannel { Id = c2.UnificRootId };
            var connection = GetAndSetConnectionForChannel(c1, statusId, "Name");
            var list = new List<ServiceChannelVersioned>
            {
                c1, c2
            };

            // repositories
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.ToList().AsQueryable());

            ArrangeAllTranslationManager();

            // Act
            var result = service.GetServiceChannels(new List<Guid> { c1.UnificRootId, c2.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            var channel1 = result.Where(i => i.Id == c1.UnificRootId).FirstOrDefault();
            channel1.Should().NotBeNull();
            channel1.Services.Should().BeNullOrEmpty();
            var channel2 = result.Where(i => i.Id == c2.UnificRootId).FirstOrDefault();
            channel2.Should().NotBeNull();
            channel2.Services.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedNamesReturned()
        {
            // Arrange
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            var c1 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.WebPage);
            c1.UnificRoot = new ServiceChannel { Id = c1.UnificRootId };

            var c2 = GetEntityAndArrangeForGetByList(ServiceChannelTypeEnum.EChannel);
            c2.UnificRoot = new ServiceChannel { Id = c2.UnificRootId };
            // Add only swedish language as published version
            var connection = GetAndSetConnectionForChannel(c1, PublishedId, svName, svId);
            var sv = connection.Service.Versions.First();
            sv.ServiceNames.Add(new ServiceName { Name = fiName, LocalizationId = fiId });

            var list = new List<ServiceChannelVersioned>
            {
                c1, c2
            };

            // repositories
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.ToList().AsQueryable());

            ArrangeAllTranslationManager();

            // Act
            var result = service.GetServiceChannels(new List<Guid> { c1.UnificRootId, c2.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            var channel1 = result.Where(i => i.Id == c1.UnificRootId).FirstOrDefault();
            channel1.Should().NotBeNull();
            channel1.Services.Should().NotBeNullOrEmpty();
            var resultConnection = channel1.Services.First();
            resultConnection.Should().NotBeNull();
            resultConnection.Service.Should().NotBeNull();
            resultConnection.Service.Name.Should().Be(svName);
            var channel2 = result.Where(i => i.Id == c2.UnificRootId).FirstOrDefault();
            channel2.Should().NotBeNull();
            channel2.Services.Should().BeNullOrEmpty();
        }
    }
}
