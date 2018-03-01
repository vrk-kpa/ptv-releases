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
using Microsoft.Extensions.Configuration;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.GeneralDescription
{
    public class GetGeneralDescriptionsByIdListTests : GeneralDescriptionServiceTestBase
    {
        private IUnitOfWorkWritable unitOfWork;
        private TestContextManager contextManager;
        private ServiceUtilities serviceUtilities;
        private IGeneralDescriptionService service;

        public GetGeneralDescriptionsByIdListTests()
        {
            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<StatutoryServiceGeneralDescriptionVersioned>>(),
                It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<StatutoryServiceGeneralDescriptionVersioned> gds, Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>> func, bool applyFilters) =>
                {
                    return gds;
                });
            // Channel connections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<GeneralDescriptionServiceChannel>>(),
               It.IsAny<Func<IQueryable<GeneralDescriptionServiceChannel>, IQueryable<GeneralDescriptionServiceChannel>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<GeneralDescriptionServiceChannel> items, Func<IQueryable<GeneralDescriptionServiceChannel>, IQueryable<GeneralDescriptionServiceChannel>> func, bool applyFilters) =>
               {
                   return items;
               });
            // Related channels
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceChannelVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceChannelVersioned> items, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });
            translationManagerMockSetup.Setup(t => t.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()))
                .Returns((List<StatutoryServiceGeneralDescriptionVersioned> entities) =>
                {
                    var vmList = new List<VmOpenApiGeneralDescriptionVersionBase>();
                    entities.ForEach(entity =>
                    {
                        vmList.Add(new VmOpenApiGeneralDescriptionVersionBase()
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        });
                    });
                    return vmList;
                });

            unitOfWork = unitOfWorkMockSetup.Object;
            contextManager = new TestContextManager(unitOfWork, unitOfWork);
            serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManager);
            service = new GeneralDescriptionService(contextManager, UserIdentification, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, DataUtils, ValidationManagerMock);
    }

        [Fact]
        public void IdListIsNull()
        {
            // Act
            var result = service.GetGeneralDescriptions(null, DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IdListIsEmpty()
        {
            // Act
            var result = service.GetGeneralDescriptions(new List<Guid>(), DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NoPublishedChannelsFound()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();

            var list = new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId1),
                EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId2)
            };

            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetGeneralDescriptions(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Never);
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetGeneralDescriptionEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<GeneralDescriptionLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetGeneralDescriptions(new List<Guid> { publishedChannel.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Never);
        }

        [Fact]
        public void CanGetGds()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId1);
            var c2 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId2);

            var list = new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                c1, c2
            };

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            
            // Act
            var result = service.GetGeneralDescriptions(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Once);
        }

        [Fact]
        public void CanGetRelatedChannels()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId1);
            c1.UnificRoot = new StatutoryServiceGeneralDescription { Id = rootId1 };
            var c2 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId2);
            c2.UnificRoot = new StatutoryServiceGeneralDescription { Id = rootId2 };

            // Set channel data
            var connection = GetAndSetConnectionForGD(c1, PublishedId, "Name");

            var list = new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                c1, c2
            };

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            GdConnectionRepoMock.Setup(g => g.All()).Returns((new List<GeneralDescriptionServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns((connection.ServiceChannel.Versions.ToList()).AsQueryable());
            
            // Act
            var result = service.GetGeneralDescriptions(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Once);
            var gd1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            gd1.Should().NotBeNull();
            gd1.ServiceChannels.Should().NotBeNullOrEmpty();
            gd1.ServiceChannels.Count.Should().Be(1);
            var gd2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            gd2.Should().NotBeNull();
            gd2.ServiceChannels.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServiceChannelsNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange  
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId1);
            c1.UnificRoot = new StatutoryServiceGeneralDescription { Id = rootId1 };
            var c2 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId2);
            c2.UnificRoot = new StatutoryServiceGeneralDescription { Id = rootId2 };

            // Set channel data
            var statusId = PublishingStatusCache.Get(publishingStatus);
            var connection = GetAndSetConnectionForGD(c1, statusId, "Name");

            var list = new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                c1, c2
            };

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            GdConnectionRepoMock.Setup(g => g.All()).Returns((new List<GeneralDescriptionServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns((connection.ServiceChannel.Versions.ToList()).AsQueryable());

            // Act
            var result = service.GetGeneralDescriptions(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Once);
            var gd1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            gd1.Should().NotBeNull();
            gd1.ServiceChannels.Should().BeNullOrEmpty();
            var gd2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            gd2.Should().NotBeNull();
            gd2.ServiceChannels.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedNamesReturned()
        {
            // Arrange  
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get(LanguageCode.fi.ToString());
            var svId = LanguageCache.Get(LanguageCode.sv.ToString());
            var c1 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId1);
            c1.UnificRoot = new StatutoryServiceGeneralDescription { Id = rootId1 };
            var c2 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId2);
            c2.UnificRoot = new StatutoryServiceGeneralDescription { Id = rootId2 };

            // Set channel data
            // Add only swedish language as published version
            var connection = GetAndSetConnectionForGD(c1, PublishedId, svName, svId);
            var channel = connection.ServiceChannel.Versions.First();
            channel.ServiceChannelNames.Add(new ServiceChannelName { Name = fiName, LocalizationId = fiId });

            var list = new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                c1, c2
            };

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            GdConnectionRepoMock.Setup(g => g.All()).Returns((new List<GeneralDescriptionServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns((connection.ServiceChannel.Versions.ToList()).AsQueryable());

            // Act
            var result = service.GetGeneralDescriptions(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Once);
            var gd1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            gd1.Should().NotBeNull();
            gd1.ServiceChannels.Should().NotBeNullOrEmpty();
            var resultConnection = gd1.ServiceChannels.First();
            resultConnection.Should().NotBeNull();
            resultConnection.ServiceChannel.Should().NotBeNull();
            resultConnection.ServiceChannel.Name.Should().Be(svName);
            var gd2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            gd2.Should().NotBeNull();
            gd2.ServiceChannels.Should().BeNullOrEmpty();
        }
    }
}
