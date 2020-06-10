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

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class GetTaskServicesTests : ServiceServiceTestBase
    {
        private int _pageNumber = 1;
        private int _pageSize = 1;
        private Guid _rootId = Guid.NewGuid();
        private List<Guid> _entityIdList;
        private ServiceVersioned _entity;

        private Mock<IRepository<ServiceVersioned>> _repoMock;
        private Mock<IRepository<ServiceLanguageAvailability>> _laRepoMock;
        private Mock<IRepository<ServiceName>> _nameRepoMock;

        private ServiceService _service;

        public GetTaskServicesTests()
        {
            _entityIdList = new List<Guid> { _rootId };
            _entity = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, _rootId);
            var entityList = new List<ServiceVersioned> { _entity };

            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));

            _repoMock = new Mock<IRepository<ServiceVersioned>>();
            _repoMock.Setup(x => x.All()).Returns(entityList.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(_repoMock.Object);
            _laRepoMock = new Mock<IRepository<ServiceLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceLanguageAvailability>>()).Returns(_laRepoMock.Object);
            _nameRepoMock = new Mock<IRepository<ServiceName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceName>>()).Returns(_nameRepoMock.Object);


            translationManagerMockSetup.Setup(x => x.TranslateAll<ServiceLanguageAvailability, VmOpenApiLanguageItem>(It.IsAny<IList<ServiceLanguageAvailability>>()))
                .Returns((List<ServiceLanguageAvailability> entities) =>
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
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, null, null);
        }

        [Fact]
        public void CanGetTasks()
        {
            // Act
            var result = _service.GetTaskServices(_pageNumber, _pageSize, _entityIdList, new List<Guid> { PublishedId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiTasks>(result);
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(1);
        }

        [Fact]
        public void CanGetNamesForTasks()
        {
            // Arrange
            var name = "Name";
            _nameRepoMock.Setup(x => x.All()).Returns(new List<ServiceName>
                {
                    new ServiceName
                    {
                        ServiceVersionedId = _entity.Id,
                        Name = name,
                        TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                        LocalizationId = LanguageCache.Get("fi"),
                        Localization = new Language{ OrderNumber = 1}
                    }
                }.AsQueryable());

            // Act
            var result = _service.GetTaskServices(_pageNumber, _pageSize, _entityIdList, new List<Guid> { PublishedId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiTasks>(result);
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(1);
            var item = result.ItemList.First();
            item.Name.Should().Be(name);
        }

        [Fact]
        public void CanGetStatusesForTasks()
        {
            // Arrange
            _laRepoMock.Setup(x => x.All()).Returns(new List<ServiceLanguageAvailability>
                {
                    new ServiceLanguageAvailability
                    {
                        ServiceVersionedId = _entity.Id,
                        LanguageId = LanguageCache.Get("fi"),
                        StatusId = PublishedId
                    }
                }.AsQueryable());

            // Act
            var result = _service.GetTaskServices(_pageNumber, _pageSize, _entityIdList, new List<Guid> { PublishedId });

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiTasks>(result);
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(1);
            var item = result.ItemList.First();
            item.Statuses.Should().NotBeNull();
            item.Statuses.Count().Should().Be(1);
        }
    }
}
