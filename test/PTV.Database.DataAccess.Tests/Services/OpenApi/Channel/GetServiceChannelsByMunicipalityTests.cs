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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelsByMunicipalityTests : ChannelServiceTestBase
    {
        private Guid _municipalityId;
        private ChannelService _service;

        public GetServiceChannelsByMunicipalityTests()
        {
            _municipalityId = Guid.NewGuid();

            // unitOfWork
            var areaMunicipalityRepoMock = new Mock<IAreaMunicipalityRepository>();
            areaMunicipalityRepoMock.Setup(g => g.All()).Returns(EntityGenerator.GetAreaMunicipalityList(null, _municipalityId).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaMunicipalityRepository>()).Returns(areaMunicipalityRepoMock.Object);
            var areaRepoMock = new Mock<IAreaRepository>();
            areaRepoMock.Setup(g => g.All()).Returns(new List<Area>().AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaRepository>()).Returns(areaRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            // channel service
            _service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker, LanguageOrderCache, CloningManager);

        }

        [Fact]
        public void NoEntitiesFound()
        {
            // Arrange
            // unitOfWork
            var areaRepoMock = new Mock<IAreaMunicipalityRepository>();
            areaRepoMock.Setup(g => g.All()).Returns(new List<AreaMunicipality>().AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaMunicipalityRepository>()).Returns(areaRepoMock.Object);

            var channelMock = new Mock<IServiceChannelVersionedRepository>();
            channelMock.Setup(o => o.All()).Returns(EntityGenerator.GetServiceChannelEntityList(0, PublishingStatusCache).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(channelMock.Object);
            
            // Act
            var result = _service.GetServiceChannelsByMunicipality(_municipalityId, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void EntitiesFound()
        {
            // Arrange
            var channels = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            var publishedChannel = channels.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.AreaMunicipalities = new List<ServiceChannelAreaMunicipality> { new ServiceChannelAreaMunicipality { MunicipalityId = _municipalityId } };
            
            // unitOfWork
            var areaRepoMock = new Mock<IAreaMunicipalityRepository>();
            areaRepoMock.Setup(g => g.All()).Returns(new List<AreaMunicipality>().AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaMunicipalityRepository>()).Returns(areaRepoMock.Object);

            var channelMock = new Mock<IServiceChannelVersionedRepository>();
            channelMock.Setup(o => o.All()).Returns(channels.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(channelMock.Object);

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceChannelVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceChannelVersioned> channelServices, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
               {
                   if (channelServices.ToList().Count > 0)
                   {
                       return new List<ServiceChannelVersioned> { publishedChannel }.AsQueryable();
                   }
                   return new List<ServiceChannelVersioned>().AsQueryable();
               }
                );
            
            // Translation manager
            translationManagerMockSetup.Setup(t => t.TranslateAll<ServiceChannelVersioned, VmOpenApiItem>(It.IsAny<IList<ServiceChannelVersioned>>()))
                .Returns(new List<VmOpenApiItem>() { new VmOpenApiItem() });
            
            // Act
            var result = _service.GetServiceChannelsByMunicipality(_municipalityId, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }
    }
}
