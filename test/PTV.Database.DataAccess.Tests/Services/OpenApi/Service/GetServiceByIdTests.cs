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
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class GetServiceByIdTests : ServiceServiceTestBase
    {
        private List<ServiceVersioned> _list;
        private ServiceVersioned _publishedItem;
        private Guid _publishedItemId;
        private Guid _publishedRootItemId;

        public GetServiceByIdTests()
        {
            _list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            _publishedItem = _list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            _publishedItemId = _publishedItem.Id;
            _publishedRootItemId = _publishedItem.UnificRootId;
        }

        [Theory]
        [InlineData(VersionStatusEnum.Published)]
        [InlineData(VersionStatusEnum.Latest)]
        [InlineData(VersionStatusEnum.LatestActive)]
        public void RightVersionNotFound(VersionStatusEnum status)
        {
            // Arrange
            var id = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, id, null, It.IsAny<bool>())).Returns((Guid?)null);

            var service = new ServiceService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.GetServiceById(id, DefaultVersion, status);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void InterfaceVersion7CanBeFetched()
        {
            // Arrange           
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, 7, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiService>();
        }

        [Fact]
        public void InterfaceVersion6CanBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, 6, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V6VmOpenApiService>();
        }

        [Fact]
        public void InterfaceVersion5CanBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, 5, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V5VmOpenApiService>();
        }

        [Fact]
        public void InterfaceVersion4CanBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, 4, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V4VmOpenApiService>();
        }

        [Fact]
        public void InterfaceVersion3CannotBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            Action act = () => service.GetServiceById(_publishedRootItemId, 3, VersionStatusEnum.Published);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<ServiceLanguageAvailability>());
            var publishedOrganization = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var service = Arrange(list);

            // Act
            var result = service.GetServiceById(publishedOrganization.UnificRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().BeNull();
            ServiceRepoMock.Verify(x => x.All(), Times.Once());
            unitOfWorkMockSetup.Verify(x => x.ApplyIncludes(It.IsAny<IQueryable<ServiceVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
               It.IsAny<bool>()), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Never);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestService(PublishingStatus publishingStatus)
        {
            // Arrange
            var item = _list.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, false)).Returns(id);
            var service = Arrange();

            // Act
            var result = service.GetServiceById(rootId, 7, VersionStatusEnum.Latest);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V7VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, false), Times.Once);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestActiveService(PublishingStatus publishingStatus)
        {
            // Arrange
            var item = _list.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, true))
                .Returns(() =>
                {
                    if (publishingStatus == PublishingStatus.Deleted || publishingStatus == PublishingStatus.OldPublished) return null;

                    return id;
                });
            var service = Arrange();

            // Act
            var result = service.GetServiceById(rootId, 7, VersionStatusEnum.LatestActive);

            // Assert
            // Method should only return draft, modified or published versions.
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, true), Times.Once);
            if (publishingStatus == PublishingStatus.Draft || publishingStatus == PublishingStatus.Modified || publishingStatus == PublishingStatus.Published)
            {
                result.Should().NotBeNull();
                var vmResult = Assert.IsType<V7VmOpenApiService>(result);
                vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            }
            else
            {
                result.Should().BeNull();
            }
        }

        private ServiceService Arrange(List<ServiceVersioned> list = null)
        {
            var serviceList = list ?? _list;
            var publishedItem = serviceList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var id = publishedItem.Id;
            var rootId = publishedItem.UnificRootId;

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceVersioned> services, Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> func, bool applyFilters) =>
               {
                   return services;
               });

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            translationManagerMockSetup.Setup(t => t.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()))
                .Returns((ServiceVersioned entity) =>
                {
                    if (entity == null) return null;

                    return new VmOpenApiServiceVersionBase() { Id = entity.UnificRootId, PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId) };
                });

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, rootId, PublishingStatus.Published, true)).Returns(id);

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(serviceList.AsQueryable());
            ServiceNameRepoMock.Setup(m => m.All()).Returns(Enumerable.Empty<ServiceName>().AsQueryable());
            var serviceNameRepoMock = new Mock<IServiceNameRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceNameRepository>()).Returns(serviceNameRepoMock.Object);

            return new ServiceService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);
        }
    }
}
