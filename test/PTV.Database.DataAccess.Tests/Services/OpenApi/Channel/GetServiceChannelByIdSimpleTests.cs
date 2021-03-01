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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
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
    public class GetServiceChannelByIdSimpleTests : ChannelServiceTestBase
    {
        private IUnitOfWorkWritable _unitOfWork;
        private IContextManager _contextManager;
        private IServiceUtilities _serviceUtilities;
        private Mock<IRepository<ServiceChannelVersioned>> _channelRepoMock;
        private ChannelService _service;

        public GetServiceChannelByIdSimpleTests()
        {
            _unitOfWork = unitOfWorkMockSetup.Object;
            _contextManager = new TestContextManager(_unitOfWork, _unitOfWork);
            _serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, _contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _channelRepoMock = new Mock<IRepository<ServiceChannelVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(_channelRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<ServiceChannelVersioned>>(),
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<ServiceChannelVersioned> channelServices, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
                {
                    return channelServices;
                });

            _service = new ChannelService(_contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, _serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache,
                VersioningManager, UserOrganizationChecker, LanguageOrderCache, null, null, null, null, null);

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RightVersionNotFound(bool getOnlyPublished)
        {
            // Arrange
            var rootId = Guid.NewGuid();
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, null, false)).Returns((Guid?)null);

            // Act
            var result = _service.GetServiceChannelByIdSimple(rootId, getOnlyPublished);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EntityNotFound(bool getOnlyPublished)
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var id = Guid.NewGuid();

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, PublishingStatus.Published, true)).Returns(id);
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, null, false)).Returns(id);

            // Act
            Action act = () => _service.GetServiceChannelByIdSimple(rootId, getOnlyPublished);

            // Assert
            act.Should().Throw<Exception>();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IRepository<ServiceChannelVersioned>>(), Times.Once);
        }

        [Fact]
        public void CanGetPublishedEntity()
        {
            // Arrange
            var entityList = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            var publishedChannel = entityList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var rootId = publishedChannel.UnificRootId;
            var id = publishedChannel.Id;

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, PublishingStatus.Published, true)).Returns(id);
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, null, false)).Returns(id);

            _channelRepoMock.Setup(c => c.All()).Returns(entityList.AsQueryable());

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiServiceChannel>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned item) =>
                {
                    return new VmOpenApiServiceChannel
                    {
                        Id = item.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(item.PublishingStatusId)
                    };
                });

            // Act
            var result = _service.GetServiceChannelByIdSimple(rootId, true);

            // Assert
            result.Should().NotBeNull();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IRepository<ServiceChannelVersioned>>(), Times.Once);
            var vmResult = Assert.IsType<VmOpenApiServiceChannel>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
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
            var rootId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var statusId = PublishingStatusCache.Get(status);
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(statusId, rootId, id);
            var entityList = new List<ServiceChannelVersioned> { entity };

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(_unitOfWork, rootId, null, false)).Returns(id);

            _channelRepoMock.Setup(c => c.All()).Returns(entityList.AsQueryable());

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiServiceChannel>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned item) =>
                {
                    return new VmOpenApiServiceChannel
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            // Act
            var result = _service.GetServiceChannelByIdSimple(rootId, false);

            // Assert
            result.Should().NotBeNull();
            unitOfWorkMockSetup.Verify(x => x.CreateRepository<IRepository<ServiceChannelVersioned>>(), Times.Once);
            var vmResult = Assert.IsType<VmOpenApiServiceChannel>(result);
            vmResult.PublishingStatus.Should().Be(status.ToString());
        }
    }
}
