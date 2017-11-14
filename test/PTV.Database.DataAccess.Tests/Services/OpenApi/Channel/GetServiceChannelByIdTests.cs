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
using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelByIdTests : ChannelServiceTestBase
    {
        private List<ServiceChannelVersioned> _channelList;
        private ServiceChannelVersioned _publishedChannel;
        private Guid _publishedChannelRootId;

        public GetServiceChannelByIdTests()
        {
            SetupTypesCacheMock<ServiceChannelType>();

            _channelList = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            _publishedChannel = _channelList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            _publishedChannelRootId = _publishedChannel.UnificRootId;
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

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, true)).Returns((Guid?)null);

            var service = new ChannelService(contextManager, UserIdentification, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, ServiceChannelLogic, serviceUtilities, CommonService, new VmListItemLogic(), DataUtils, new VmOwnerReferenceLogic(),
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker, UrlService);

            // Act
            var result = service.GetServiceChannelById(id, DefaultVersion, status);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ElectronicChannelCanBeFetched()
        {
            // Arrange           
            var service = Arrange(ServiceChannelTypeEnum.EChannel);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiElectronicChannel>();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
        }

        [Fact]
        public void PhoneChannelCanBeFetched()
        {
            // Arrange           
            var service = Arrange(ServiceChannelTypeEnum.Phone);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiPhoneChannel>();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
        }

        [Fact]
        public void PrintableFormChannelCanBeFetched()
        {
            // Arrange           
            var service = Arrange(ServiceChannelTypeEnum.PrintableForm);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiPrintableFormChannel>();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
        }

        [Fact]
        public void ServiceLocationChannelCanBeFetched()
        {
            // Arrange           
            var service = Arrange(ServiceChannelTypeEnum.ServiceLocation);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiServiceLocationChannel>();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
        }

        [Fact]
        public void WebPageChannelCanBeFetched()
        {
            // Arrange           
            var service = Arrange(ServiceChannelTypeEnum.WebPage);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiWebPageChannel>();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
        }

        [Fact]
        public void InterfaceVersion6CanBeFetched()
        {
            // Arrange            
            var service = Arrange(ServiceChannelTypeEnum.Phone);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, 6, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V6VmOpenApiPhoneChannel>();
        }

        [Fact]
        public void InterfaceVersion5CanBeFetched()
        {
            // Arrange            
            var service = Arrange(ServiceChannelTypeEnum.PrintableForm);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, 5, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V5VmOpenApiPrintableFormChannel>();
        }

        [Fact]
        public void InterfaceVersion4CanBeFetched()
        {
            // Arrange            
            var service = Arrange(ServiceChannelTypeEnum.ServiceLocation);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, 4, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V4VmOpenApiServiceLocationChannel>();
        }

        [Fact]
        public void InterfaceVersion3CannotBeFetched()
        {
            // Arrange            
            var service = Arrange(ServiceChannelTypeEnum.WebPage);

            // Act
            Action act = () => service.GetServiceChannelById(_publishedChannelRootId, 3, VersionStatusEnum.Published);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<ServiceChannelLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var service = Arrange(ServiceChannelTypeEnum.EChannel, list);

            // Act
            var result = service.GetServiceChannelById(publishedChannel.Id, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().BeNull();
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestServiceChannel(PublishingStatus publishingStatus)
        {
            // Arrange
            var channelType = ServiceChannelTypeEnum.WebPage;
            var item = _channelList.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            item.TypeId = TypeCache.Get<ServiceChannelType>(channelType.ToString());
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, false)).Returns(id);
            var service = Arrange(channelType);

            // Act
            var result = service.GetServiceChannelById(rootId, 7, VersionStatusEnum.Latest);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V7VmOpenApiWebPageChannel>(result);
            vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, false), Times.Once);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestActiveServiceChannel(PublishingStatus publishingStatus)
        {
            // Arrange
            var channelType = ServiceChannelTypeEnum.ServiceLocation;
            var item = _channelList.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            item.TypeId = TypeCache.Get<ServiceChannelType>(channelType.ToString());
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, true))
                .Returns(() =>
                {
                    if (publishingStatus == PublishingStatus.Deleted || publishingStatus == PublishingStatus.OldPublished) return null;

                    return id;
                });

            var service = Arrange(channelType);

            // Act
            var result = service.GetServiceChannelById(rootId, 7, VersionStatusEnum.LatestActive);

            // Assert
            // Method should only return draft, modified or published versions.
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, true), Times.Once);
            if (publishingStatus == PublishingStatus.Draft || publishingStatus == PublishingStatus.Modified || publishingStatus == PublishingStatus.Published)
            {
                result.Should().NotBeNull();
                var vmResult = Assert.IsType<V7VmOpenApiServiceLocationChannel>(result);
                vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            }
            else
            {
                result.Should().BeNull();
            }
        }

        private ChannelService Arrange(ServiceChannelTypeEnum channelType, List<ServiceChannelVersioned> list = null)
        {
            var channelList = list ?? _channelList;
            var publishedChannel = channelList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.TypeId = TypeCache.Get<ServiceChannelType>(channelType.ToString());
            var id = publishedChannel.Id;
            
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceChannelVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceChannelVersioned> channelServices, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
               {
                   return channelServices;
               }
                );
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            ArrangeTranslationManager(channelType);
            
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWork, _publishedChannelRootId, PublishingStatus.Published, true)).Returns(id);

            // repositories
            ChannelRepoMock.Setup(g => g.All()).Returns(channelList.AsQueryable());
            
            return new ChannelService(contextManager, UserIdentification, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, ServiceChannelLogic, serviceUtilities, CommonService, new VmListItemLogic(), DataUtils, new VmOwnerReferenceLogic(),
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker, UrlService);
        }
    }
}
