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
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelBySourceTests : ChannelServiceTestBase
    {
        private IUnitOfWorkWritable _unitOfWork;
        private IContextManager _contextManager;
        private ServiceUtilities _serviceUtilities;
        private Mock<IExternalSourceRepository> _sourceRepoMock;

        public GetServiceChannelBySourceTests()
        {
            _unitOfWork = unitOfWorkMockSetup.Object;
            _contextManager = new TestContextManager(_unitOfWork, _unitOfWork);
            _serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, _contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            _sourceRepoMock = new Mock<IExternalSourceRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IExternalSourceRepository>()).Returns(_sourceRepoMock.Object);
        }

        [Fact]
        public void CannotGetUser()
        {
            // Arrange
            var sourceId = "sourceId";
            UserIdentificationMock.Setup(s => s.UserName).Returns((string)null);

            var service = new ChannelService(_contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, _serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            Action act = () => service.GetServiceChannelBySource(sourceId);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IExternalSourceRepository>(), Times.Never);
        }

        [Fact]
        public void SourceIdIsNull()
        {
            // Arrange
            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);            
            var service = new ChannelService(_contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, _serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            Action act = () => service.GetServiceChannelBySource(null);

            // Assert
            act.ShouldThrow<Exception>();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IExternalSourceRepository>(), Times.Never);
        }

        [Fact]
        public void CannotGetPTVId()
        {
            // Arrange
            var sourceId = "sourceId";
            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);
            
            var service = new ChannelService(_contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, _serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            Action act = () => service.GetServiceChannelBySource(sourceId);

            // Assert
            act.ShouldThrow<Exception>();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IExternalSourceRepository>(), Times.Once);
        }

        [Theory]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.OldPublished)]
        public void CanGetEntityWithAnyStatus(PublishingStatus status)
        {
            // Arrange
            var sourceId = "sourceId";
            var rootId = Guid.NewGuid();
            var id = Guid.NewGuid();

            var sourceList = new List<ExternalSource> {
                new ExternalSource { SourceId = sourceId, RelationId = USERNAME, ObjectType = typeof(ServiceChannel).Name, PTVId = rootId } };
            var statusId = PublishingStatusCache.Get(status);
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(statusId, rootId, id);
            var entityList = new List<ServiceChannelVersioned> { entity };

            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IExternalSourceRepository>()).Returns(_sourceRepoMock.Object);
            _sourceRepoMock.Setup(s => s.All()).Returns(sourceList.AsQueryable());
            var channelRepoMock = new Mock<IRepository<ServiceChannelVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(channelRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<ServiceChannelVersioned>>(),
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<ServiceChannelVersioned> channelServices, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
                {
                    return channelServices;
                });
            channelRepoMock.Setup(c => c.All()).Returns(entityList.AsQueryable());
            
            SetupTypesCacheMock<ServiceChannelType>();

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, null, false)).Returns(id);
            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiServiceChannel>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned item) =>
                {
                    return new VmOpenApiServiceChannel()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });
            var service = new ChannelService(_contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, _serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            var result = service.GetServiceChannelBySource(sourceId);

            // Assert
            result.Should().NotBeNull();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IExternalSourceRepository>(), Times.Once);
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IRepository<ServiceChannelVersioned>>(), Times.Once);
            var vmResult = Assert.IsType<VmOpenApiServiceChannel>(result);
            vmResult.PublishingStatus.Should().Be(status.ToString());
        }
    }
}
