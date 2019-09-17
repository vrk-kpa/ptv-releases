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
using Microsoft.EntityFrameworkCore;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.GeneralDescription
{
    public class GetNewGeneralDescriptionsTests : GeneralDescriptionServiceTestBase
    {
        private int _pageNumber = 1;
        private int _pageSize = 1;
        private Guid _rootId = Guid.NewGuid();
        private List<Guid> _entityIdList;
        private StatutoryServiceGeneralDescriptionVersioned _entity;

        private Mock<IRepository<StatutoryServiceGeneralDescriptionVersioned>> _gdRepoMock;
        private Mock<IRepository<TrackingGeneralDescriptionVersioned>> _trackingRepoMock;
        private Mock<IRepository<GeneralDescriptionLanguageAvailability>> _laRepoMock;
        private Mock<IRepository<StatutoryServiceName>> _nameRepoMock;

        private GeneralDescriptionService _service;

        public GetNewGeneralDescriptionsTests()
        {
            _entityIdList = new List<Guid> { _rootId };
            _entity = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishedId, _rootId);
            var entityList = new List<StatutoryServiceGeneralDescriptionVersioned> { _entity };

            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));

            _gdRepoMock = new Mock<IRepository<StatutoryServiceGeneralDescriptionVersioned>>();
            _gdRepoMock.Setup(g => g.All()).Returns(entityList.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<StatutoryServiceGeneralDescriptionVersioned>>()).Returns(_gdRepoMock.Object);
            _trackingRepoMock = new Mock<IRepository<TrackingGeneralDescriptionVersioned>>();
            _trackingRepoMock.Setup(t => t.All()).Returns(new List<TrackingGeneralDescriptionVersioned> { new TrackingGeneralDescriptionVersioned
            {
                OperationType = EntityState.Added.ToString(),
                GenerealDescriptionId = _rootId,
                Created = DateTime.Now.AddDays(-1)
            } }.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<TrackingGeneralDescriptionVersioned>>()).Returns(_trackingRepoMock.Object);
            _laRepoMock = new Mock<IRepository<GeneralDescriptionLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<GeneralDescriptionLanguageAvailability>>()).Returns(_laRepoMock.Object);
            _nameRepoMock = new Mock<IRepository<StatutoryServiceName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<StatutoryServiceName>>()).Returns(_nameRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new GeneralDescriptionService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, DataUtils, ValidationManagerMock, LanguageOrderCache, RestrictionFilterManager, PahaTokenProcessor, null);
        }

        [Fact]
        public void CanGetNewGeneralDescriptions()
        {
            // Act
            var result = _service.GetNewGeneralDescriptions(_pageNumber, _pageSize);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(1);
        }

        [Fact]
        public void CanGetNamesForGeneralDescriptions()
        {
            // Arrange
            _laRepoMock.Setup(x => x.All()).Returns(new List<GeneralDescriptionLanguageAvailability>
                {
                    new GeneralDescriptionLanguageAvailability
                    {
                        StatutoryServiceGeneralDescriptionVersionedId = _entity.Id,
                        LanguageId = LanguageCache.Get("fi"),
                        StatusId = PublishedId
                    }
                }.AsQueryable());

            var name = "Name";
            _nameRepoMock.Setup(x => x.All()).Returns(new List<StatutoryServiceName>
                {
                    new StatutoryServiceName
                    {
                        StatutoryServiceGeneralDescriptionVersionedId = _entity.Id,
                        Name = name,
                        TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                        LocalizationId = LanguageCache.Get("fi"),
                        Localization = new Language{ OrderNumber = 1}
                    }
                }.AsQueryable());

            // Act
            var result = _service.GetNewGeneralDescriptions(_pageNumber, _pageSize);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNull();
            vmResult.ItemList.Count().Should().Be(1);
            var item = vmResult.ItemList.First();
            item.Name.Should().Be(name);
        }
    }
}
