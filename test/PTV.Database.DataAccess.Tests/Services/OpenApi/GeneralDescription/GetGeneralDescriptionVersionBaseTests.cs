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
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Domain.Model.Models.OpenApi.V10;

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
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, null, false)).Returns((Guid?)null);

            var gdService = new GeneralDescriptionService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, DataUtils, ValidationManagerMock, LanguageOrderCache, RestrictionFilterManager, PahaTokenProcessor);

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
        public void InterfaceVersion10CanBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 10);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V10VmOpenApiGeneralDescription>();
        }

        [Fact]
        public void InterfaceVersion9CanBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 9);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V9VmOpenApiGeneralDescription>();
        }

        [Fact]
        public void InterfaceVersion8CanBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 8);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiGeneralDescription>();
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
        public void InterfaceVersion6CannotBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            Action act = () => service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 6);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion5CannotBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            Action act = () => service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 5);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion4CannotBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            Action act = () => service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 4);

            // Assert
            act.Should().Throw<Exception>();
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
            var vmResult = Assert.IsType<V10VmOpenApiGeneralDescription>(result);
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
            var vmResult = Assert.IsType<V10VmOpenApiGeneralDescription>(result);
            vmResult.ServiceChannels.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedNamesReturned()
        {
            // Arrange
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
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
            var vmResult = Assert.IsType<V10VmOpenApiGeneralDescription>(result);
            vmResult.ServiceChannels.Should().NotBeNullOrEmpty();
            var resultConnection = vmResult.ServiceChannels.First();
            resultConnection.Should().NotBeNull();
            resultConnection.ServiceChannel.Should().NotBeNull();
            resultConnection.ServiceChannel.Name.Should().Be(svName);
        }

        [Fact]
        public void RestrictedGDNotReturned()
        {
            // Arrange
            Guid restrictedGD = Guid.NewGuid();
            _publishedGd.GeneralDescriptionTypeId = restrictedGD;
            var service = Arrange();
            
//            restrictionFilterManagerMock.Setup(i => i.SetAccessForGuidTypes<GeneralDescriptionType>(It.IsAny<Guid>(), It.IsAny<IEnumerable<IVmRestrictableType>>())).Returns(new List<IVmRestrictableType>() { new VmEnumType() { Id = restrictedGD, Access = EVmRestrictionFilterType.Denied}});
            CommonServiceMock.Setup(i => i.GetRestrictedDescriptionTypes()).Returns(new List<Guid> {restrictedGD});
            
            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 0, true, true);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void UserHasRightsForRestrictedGD()
        {
            // Arrange
            Guid restrictedGD = Guid.NewGuid();
            _publishedGd.GeneralDescriptionTypeId = restrictedGD;
            var service = Arrange();
//            restrictionFilterManagerMock.Setup(i => i.SetAccessForGuidTypes<GeneralDescriptionType>(It.IsAny<Guid>(), It.IsAny<IEnumerable<IVmRestrictableType>>())).Returns(new List<IVmRestrictableType>() { new VmEnumType() { Id = restrictedGD, Access = EVmRestrictionFilterType.Allowed}});
            CommonServiceMock.Setup(i => i.GetRestrictedDescriptionTypes()).Returns(new List<Guid> {Guid.NewGuid()});

            // Act
            var result = service.GetGeneralDescriptionVersionBase(_publishedGdRootId, 0, true, true);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<VmOpenApiGeneralDescriptionVersionBase>();
        }

        private GeneralDescriptionService Arrange()
        {
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
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            translationManagerMockSetup.Setup(t => t.Translate<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(_publishedGd))
                .Returns((StatutoryServiceGeneralDescriptionVersioned entity) => 
                {
                    if (entity == null) return null;

                    var vm = new VmOpenApiGeneralDescriptionVersionBase
                    {
                        Id = entity.UnificRootId,
                        GeneralDescriptionTypeId = entity.GeneralDescriptionTypeId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                    return vm;
                });

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, _publishedGdRootId, PublishingStatus.Published, true)).Returns(_publishedGdId);

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(_gdList.AsQueryable());
            var serviceChannelNameRepoMock = new Mock<IServiceChannelNameRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelNameRepository>()).Returns(serviceChannelNameRepoMock.Object);

            return new GeneralDescriptionService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, DataUtils, ValidationManagerMock, LanguageOrderCache, RestrictionFilterManager, PahaTokenProcessor);
        }
    }
}
