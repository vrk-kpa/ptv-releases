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
using PTV.Database.DataAccess.Tests.Translators.OpenApi;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi
{
    public class GetGeneralDescriptionsTests : ServiceTestBase
    {
        private ILogger<GeneralDescriptionService> _logger;
        
        public GetGeneralDescriptionsTests()
        {
            _logger = new Mock<ILogger<GeneralDescriptionService>>().Object;
        }

        [Theory]
        [InlineData(1, 1, 1, 1, 1)]
        [InlineData(2, 1, 1, 1, 0)]
        [InlineData(1, 1, 2, 2, 1)]
        [InlineData(2, 1, 2, 2, 1)]
        [InlineData(3, 1, 2, 2, 0)]
        [InlineData(1, 2, 2, 1, 2)]
        [InlineData(2, 2, 2, 1, 0)]
        public void NoDateDefined(int pageNumber, int pageSize, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            var list = EntityGenerator.GetGeneralDescriptionEntityList(count, PublishingStatusCache)
                .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId)).ToList();
            TestBase(null, list, pageNumber, pageSize, count, expectedPageCount, expectedItemCountOnPage);
        }

        [Fact]
        public void DateDefinedAsNow()
        {
            var date = DateTime.Now;
            var list = EntityGenerator.GetGeneralDescriptionEntityList(1, PublishingStatusCache)
                .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) && e.Modified > date).ToList();
            TestBase(DateTime.Now, list, 1, 1, 1, 0, 0);
        }

        [Fact]
        public void DateDefinedAsYearAgo()
        {
            var date = DateTime.Now.AddYears(-1);
            var list = EntityGenerator.GetGeneralDescriptionEntityList(1, PublishingStatusCache)
               .Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) && e.Modified > date).ToList();
            TestBase(date, list, 1, 1, 1, 1, 1);
        }

        private void TestBase(DateTime? date, List<StatutoryServiceGeneralDescriptionVersioned> entityList, int pageNumber, int pageSize, int count, int expectedPageCount, int expectedItemCountOnPage)
        {
            // unitOfWork
            var gdRepoMock = new Mock<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            gdRepoMock.Setup(g => g.All()).Returns(entityList.AsQueryable());

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<StatutoryServiceGeneralDescriptionVersioned>>()).Returns(gdRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<StatutoryServiceGeneralDescriptionVersioned>>(),
                It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>(),
                It.IsAny<bool>()
                )).Returns(entityList.AsQueryable());

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiItem>(It.IsAny<IQueryable<StatutoryServiceGeneralDescriptionVersioned>>()))
                .Returns(TestDataFactory.CreateVmItemList(count));
            var translationManagerMock = translationManagerMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilitiesMock = new ServiceUtilities(UserIdentificationMock, LockingManagerMock, contextManager, UserOrganizationServiceMock,
                VersioningManagerMock, UserInfoServiceMock, UserOrganizationCheckerMock);

            var gdService = new GeneralDescriptionService(contextManager, UserIdentificationMock, translationManagerMock, TranslationManagerVModelMock,
                _logger, serviceUtilitiesMock, CommonServiceMock, PublishingStatusCache, UserOrganizationCheckerMock, LanguageCache, TypeCache,
                VersioningManagerMock, DataUtils);

            // Act
            var result = gdService.V3GetGeneralDescriptions(date, pageNumber, pageSize);

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
