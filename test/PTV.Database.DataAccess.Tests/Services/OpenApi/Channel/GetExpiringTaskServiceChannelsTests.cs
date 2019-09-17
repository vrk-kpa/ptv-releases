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
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetExpiringTaskServiceChannelsTests : ChannelServiceTestBase
    {
        private int _pageNumber = 1;
        private int _pageSize = 1;
        private Guid _rootId = Guid.NewGuid();
        private List<Guid> _entityIdList;
        private ServiceChannelVersioned _entity;

        private Mock<IRepository<ServiceChannelVersioned>> _channelRepoMock;
        private Mock<IRepository<ServiceChannelLanguageAvailability>> _laRepoMock;
        private Mock<IRepository<ServiceChannelName>> _nameRepoMock;

        private ChannelService _service;

        public GetExpiringTaskServiceChannelsTests()
        {
            _entityIdList = new List<Guid> { _rootId };
            _entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId, _rootId);
            var entityList = new List<ServiceChannelVersioned> { _entity };

            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));

            _channelRepoMock = new Mock<IRepository<ServiceChannelVersioned>>();
            _channelRepoMock.Setup(x => x.All()).Returns(entityList.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(_channelRepoMock.Object);
            _laRepoMock = new Mock<IRepository<ServiceChannelLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelLanguageAvailability>>()).Returns(_laRepoMock.Object);
            _nameRepoMock = new Mock<IRepository<ServiceChannelName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelName>>()).Returns(_nameRepoMock.Object);

            translationManagerMockSetup.Setup(x => x.TranslateAll<ServiceChannelLanguageAvailability, VmOpenApiLanguageItem>(It.IsAny<IList<ServiceChannelLanguageAvailability>>()))
                .Returns((List<ServiceChannelLanguageAvailability> entities) =>
                {
                    if (entities?.Count > 0)
                    {
                        var vm = new List<VmOpenApiLanguageItem>();
                        entities.ForEach(e => vm.Add(new VmOpenApiLanguageItem()));
                        return vm;
                    }

                    return null;
                });

            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new ChannelService(contextManager, translationManagerMockSetup.Object,
                translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache,
                VersioningManager, UserOrganizationChecker, LanguageOrderCache, null);
        }

        [Fact]
        public void CanGetTasks()
        {
            // Act
            var result = _service.GetTaskServiceChannels(_pageNumber, _pageSize, _entityIdList, DateTime.Now, new List<Guid> { PublishedId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiExpiringTasks>(result);
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(1);
        }

        [Fact]
        public void CanGetNamesForTasks()
        {
            // Arrange
            var name = "Name";
            _nameRepoMock.Setup(x => x.All()).Returns(new List<ServiceChannelName>
                {
                    new ServiceChannelName
                    {
                        ServiceChannelVersionedId = _entity.Id,
                        Name = name,
                        TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                        LocalizationId = LanguageCache.Get("fi"),
                        Localization = new Language{ OrderNumber = 1}
                    }
                }.AsQueryable());

            // Act
            var result = _service.GetTaskServiceChannels(_pageNumber, _pageSize, _entityIdList, DateTime.Now, new List<Guid> { PublishedId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiExpiringTasks>(result);
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(1);
            var item = result.ItemList.First();
            item.Name.Should().Be(name);
        }

        [Fact]
        public void CanGetStatusesForTasks()
        {
            // Arrange
            _laRepoMock.Setup(x => x.All()).Returns(new List<ServiceChannelLanguageAvailability>
                {
                    new ServiceChannelLanguageAvailability
                    {
                        ServiceChannelVersionedId = _entity.Id,
                        LanguageId = LanguageCache.Get("fi"),
                        StatusId = PublishedId
                    }
                }.AsQueryable());

            // Act
            var result = _service.GetTaskServiceChannels(_pageNumber, _pageSize, _entityIdList, DateTime.Now, new List<Guid> { PublishedId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiExpiringTasks>(result);
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(1);
            var item = result.ItemList.First();
            item.Statuses.Should().NotBeNull();
            item.Statuses.Count().Should().Be(1);
        }
    }
}
