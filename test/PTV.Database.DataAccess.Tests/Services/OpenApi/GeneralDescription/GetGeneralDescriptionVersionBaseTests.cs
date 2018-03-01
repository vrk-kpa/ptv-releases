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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
namespace PTV.Database.DataAccess.Tests.Services.OpenApi.GeneralDescription
{
    public class GetGeneralDescriptionVersionBaseTests : GeneralDescriptionServiceTestBase
    {
        private List<StatutoryServiceGeneralDescriptionVersioned> _gdList;
        private StatutoryServiceGeneralDescriptionVersioned _publishedGd;
        private Guid _publishedGdId;
        private Guid _publishedGdRootId;

        public GetGeneralDescriptionVersionBaseTests()
        {
            _gdList = EntityGenerator.GetGeneralDescriptionEntityList(1, PublishingStatusCache);
            _publishedGd = _gdList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            _publishedGdId = _publishedGd.Id;
            _publishedGdRootId = _publishedGd.UnificRootId;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RightVersionNotFound(bool getOnlyPublished)
        {
            // Arrange
            var id = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, null, false)).Returns((Guid?)null);

            var gdService = new GeneralDescriptionService(contextManager, UserIdentification, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, DataUtils, ValidationManagerMock);

            // Act
            var result = gdService.GetGeneralDescriptionVersionBase(id, DefaultVersion, getOnlyPublished);

            // Assert
            result.Should().BeNull();
            if (getOnlyPublished)
            {
                VersioningManagerMock.Verify(x => x.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published, true), Times.Once());
            }
            else
            {
                VersioningManagerMock.Verify(x => x.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, null, false), Times.Once());
            }            
        }

        [Fact]
        public void InterfaceVersion7CanBeFetched()
        {
            // Arrange           
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 7);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiGeneralDescription>();
        }

        [Fact]
        public void InterfaceVersion6CanBeFetched()
        {
            // Arrange           
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 6);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V6VmOpenApiGeneralDescription>();
        }

        [Fact]
        public void InterfaceVersion5CanBeFetched()
        {
            // Arrange           
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 5);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V4VmOpenApiGeneralDescription>();
        }

        [Fact]
        public void InterfaceVersion4CanBeFetched()
        {
            // Arrange           
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 4);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V4VmOpenApiGeneralDescription>();
        }
        
        [Fact]
        public void ChannelsAttached()
        {
            // Arrange
            var connection = GetAndSetConnectionForGD(_publishedGd, PublishedId, "Name");

            GdRepoMock.Setup(g => g.All()).Returns(_gdList.AsQueryable());
            GdConnectionRepoMock.Setup(g => g.All()).Returns(_publishedGd.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns((connection.ServiceChannel.Versions.ToList()).AsQueryable());
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IGeneralDescriptionServiceChannelRepository>(), Times.Once());
            var vmResult = Assert.IsType<V7VmOpenApiGeneralDescription>(result);
            vmResult.ServiceChannels.Count.Should().Be(1);
            vmResult.ServiceChannels.First().ServiceChannel.Id.Should().Be(connection.ServiceChannelId);
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServiceChannelsNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange    
            var statusId = PublishingStatusCache.Get(publishingStatus);
            var connection = GetAndSetConnectionForGD(_publishedGd, statusId, "Name");
            GdRepoMock.Setup(g => g.All()).Returns(_gdList.AsQueryable());
            GdConnectionRepoMock.Setup(g => g.All()).Returns(_publishedGd.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns((connection.ServiceChannel.Versions.ToList()).AsQueryable());
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IGeneralDescriptionServiceChannelRepository>(), Times.Once());
            var vmResult = Assert.IsType<V7VmOpenApiGeneralDescription>(result);
            vmResult.ServiceChannels.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedNamesReturned()
        {
            // Arrange
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get(LanguageCode.fi.ToString());
            var svId = LanguageCache.Get(LanguageCode.sv.ToString());
            // Add only swedish language as published version
            var connection = GetAndSetConnectionForGD(_publishedGd, PublishedId, svName, svId);
            var channel = connection.ServiceChannel.Versions.First();
            channel.ServiceChannelNames.Add(new ServiceChannelName { Name = fiName, LocalizationId = fiId });

            GdRepoMock.Setup(g => g.All()).Returns(_gdList.AsQueryable());
            GdConnectionRepoMock.Setup(g => g.All()).Returns(_publishedGd.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns((connection.ServiceChannel.Versions.ToList()).AsQueryable());
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IGeneralDescriptionServiceChannelRepository>(), Times.Once());
            var vmResult = Assert.IsType<V7VmOpenApiGeneralDescription>(result);
            vmResult.ServiceChannels.Should().NotBeNullOrEmpty();
            var resultConnection = vmResult.ServiceChannels.First();
            resultConnection.Should().NotBeNull();
            resultConnection.ServiceChannel.Should().NotBeNull();
            resultConnection.ServiceChannel.Name.Should().Be(svName);
        }

        private GeneralDescriptionService Arrange()
        {
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<StatutoryServiceGeneralDescriptionVersioned>>(),
               It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<StatutoryServiceGeneralDescriptionVersioned> gds, Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>> func, bool applyFilters) =>//new List<StatutoryServiceGeneralDescriptionVersioned> { _publishedGd }.AsQueryable());
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
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            translationManagerMockSetup.Setup(t => t.Translate<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(_publishedGd))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase());

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, _publishedGdRootId, PublishingStatus.Published, true)).Returns(_publishedGdId);

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(_gdList.AsQueryable());
            var serviceChannelNameRepoMock = new Mock<IServiceChannelNameRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelNameRepository>()).Returns(serviceChannelNameRepoMock.Object);

            return new GeneralDescriptionService(contextManager, UserIdentification, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, DataUtils, ValidationManagerMock);
        }
    }
}
