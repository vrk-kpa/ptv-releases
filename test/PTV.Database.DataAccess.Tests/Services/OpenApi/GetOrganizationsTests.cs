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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.OpenApi;
using PTV.Database.Model.Models;
using PTV.Domain.Logic.Address;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi
{
    public class GetOrganizationsTests : ServiceTestBase
    {
        private ILogger<DataAccess.Services.OrganizationService> _logger;

        private OrganizationLogic _organizationLogic;

        public GetOrganizationsTests()
        {
            _logger = new Mock<ILogger<DataAccess.Services.OrganizationService>>().Object;
            var mapServiceProvider = new MapServiceProvider(
                (new Mock<IHostingEnvironment>()).Object,
                new ApplicationConfiguration((new Mock<IConfigurationRoot>()).Object), 
                (new Mock<IOptions<ProxyServerSettings>>()).Object, 
                new Mock<ILogger<MapServiceProvider>>().Object);
            _organizationLogic = new OrganizationLogic(new ChannelAttachmentLogic(), new WebPageLogic(), new AddressLogic(mapServiceProvider));
        }

        [Theory]
        [InlineData(1, 1, true, 1, 1, 1)]
        [InlineData(1, 1, false, 1, 1, 1)]
        [InlineData(2, 1, true, 1, 1, 0)]
        [InlineData(2, 1, false, 1, 1, 0)]
        [InlineData(1, 1, true, 2, 2, 1)]
        [InlineData(1, 1, false, 2, 2, 1)]
        [InlineData(2, 1, true, 2, 2, 1)]
        [InlineData(2, 1, false, 2, 2, 1)]
        [InlineData(3, 1, true, 2, 2, 0)]
        [InlineData(3, 1, false, 2, 2, 0)]
        [InlineData(1, 2, true, 2, 1, 2)]
        [InlineData(1, 2, false, 2, 1, 2)]
        [InlineData(2, 2, true, 2, 1, 0)]
        [InlineData(2, 2, false, 2, 1, 0)]
        public void NoDateDefined(int pageNumber, int pageSize, bool archived, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetOrganizationEntityList(count, PublishingStatusCache)
                .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId)).ToList();
            TestBase(null, list, pageNumber, pageSize, archived, count, expectedPageCount, expectedItemCountOnPage);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DateDefinedAsNow(bool archived)
        {
            var date = DateTime.Now;
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache)
                .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) && e.Modified > date).ToList();
            TestBase(DateTime.Now, list, 1, 1, archived, 1, 0, 0);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DateDefinedAsYearAgo(bool archived)
        {
            var date = DateTime.Now.AddYears(-1);
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache)
               .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) && e.Modified > date).ToList();
            TestBase(date, list, 1, 1, archived, 1, 1, 1);
        }

        private void TestBase(DateTime? date, List<OrganizationVersioned> entityList, int pageNumber, int pageSize, bool archived, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            // unitOfWork
            var repoMock = new Mock<IOrganizationVersionedRepository>();
            repoMock.Setup(g => g.All()).Returns(entityList.AsQueryable());

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationVersioned>>()).Returns(repoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<OrganizationVersioned>>(),
                It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
                It.IsAny<bool>()
                )).Returns(entityList.AsQueryable());

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<OrganizationVersioned, VmOpenApiItem>(It.IsAny<IQueryable<OrganizationVersioned>>()))
                .Returns(TestDataFactory.CreateVmItemList(count));
            var translationManagerMock = translationManagerMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilitiesMock = new ServiceUtilities(UserIdentificationMock, LockingManagerMock, contextManager, UserOrganizationServiceMock,
                VersioningManagerMock, UserInfoServiceMock, UserOrganizationCheckerMock);

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModelMock, _logger, _organizationLogic,
                serviceUtilitiesMock, DataUtils, CommonServiceMock, (new Mock<IHttpContextAccessor>()).Object, AddressServiceMock, PublishingStatusCache, LanguageCache,
                VersioningManagerMock, UserOrganizationCheckerMock, CacheManager.TypesCache);

            // Act
            var result = service.GetOrganizations(date, pageNumber, pageSize, archived);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(expectedPageCount);
            if (expectedItemCountOnPage == 0)
            {
                result.ItemList.Should().BeNull();
            }
            else
            {
                result.ItemList.Count.Should().BeGreaterThan(0);
            }
        }

    }
}
