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
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Translation
{
    public class GetTranslationItemsTests : ServiceTestBase
    {
        private Mock<IRepository<TranslationOrder>> translationRepoMock;

        private ITranslationService service;
        internal ILogger<TranslationService> Logger { get; private set; }


        public GetTranslationItemsTests()
        {
            Logger = new Mock<ILogger<TranslationService>>().Object;
            translationRepoMock = new Mock<IRepository<TranslationOrder>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<TranslationOrder>>()).Returns(translationRepoMock.Object);
            var serviceTranslationRepoMock = new Mock<IRepository<ServiceTranslationOrder>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceTranslationOrder>>()).Returns(serviceTranslationRepoMock.Object);
            var channelTranslationRepoMock = new Mock<IRepository<ServiceChannelTranslationOrder>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelTranslationOrder>>()).Returns(channelTranslationRepoMock.Object);
            var stateRepoMock = new Mock<IRepository<TranslationOrderState>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<TranslationOrderState>>())
                .Returns(stateRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            service = new TranslationService(contextManager, CacheManagerMock.Object.PublishingStatusCache, translationManagerMockSetup.Object, TranslationManagerVModel,
                CacheManagerMock.Object, VersioningManager, UserOrganizationChecker, serviceUtilities, CommonServiceMock.Object, Logger, ValidationManagerMock);
        }

        [Fact]
        public void NoEntitiesFound()
        {
            // Act
            var result = service.GetTranslationItems(null, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            result.Should().BeOfType<VmOpenApiTranslationItemsPage>();
            (result as VmOpenApiTranslationItemsPage).ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetEntities()
        {
            // Arrange
            var itemList = new List<TranslationOrder> { new TranslationOrder { OrderIdentifier = 1} };
            translationRepoMock.Setup(r => r.All()).Returns(itemList.AsQueryable());

            // Act
            var result = service.GetTranslationItems(null, null, 1, 10);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<VmOpenApiTranslationItemsPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void CanFilterOutItemsByDate()
        {
            // Arrange
            var now = DateTime.Now;
            var itemList = new List<TranslationOrder>
            {
                new TranslationOrder { OrderIdentifier = 1, Modified = now },
                new TranslationOrder { OrderIdentifier = 2, Modified = now.AddDays(-2) }, // modified two days ago
            };
            translationRepoMock.Setup(r => r.All()).Returns(itemList.AsQueryable());

            // Act
            // Get items that have been modified yesterday latest
            var result = service.GetTranslationItems(now.AddDays(-1), null, 1, 10);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<VmOpenApiTranslationItemsPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
            vmResult.ItemList.First().OrderId.Should().Be(1);
        }
    }
}
