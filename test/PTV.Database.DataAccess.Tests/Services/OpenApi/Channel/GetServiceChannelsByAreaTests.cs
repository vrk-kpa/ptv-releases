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
using PTV.Domain.Model.Models.OpenApi.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelsByAreaTests : ChannelServiceTestBase
    {
        private Guid _municipalityId;
        private Guid _areaId;
        private ServiceChannelVersioned _publishedServiceChannel;
        private ChannelService _service;
        private Mock<IAreaMunicipalityRepository>  _areaMunicipalityRepoMock;
        //private Mock<IRepository<ServiceChannelVersioned>> _channelMock;

        public GetServiceChannelsByAreaTests()
        {
            SetupTypesCacheMock<AreaInformationType>();
            SetupTypesCacheMock<AreaType>();

            var channels = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            _publishedServiceChannel = channels.FirstOrDefault(o => o.PublishingStatusId == PublishedId);

            _municipalityId = Guid.NewGuid();
            _areaId = Guid.NewGuid();

            // unitOfWork
            var channelMock = new Mock<IRepository<ServiceChannelVersioned>>();
            channelMock.Setup(o => o.All()).Returns(channels.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(channelMock.Object);
            var laRepoMock = new Mock<IRepository<ServiceChannelLanguageAvailability>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelLanguageAvailability>>()).Returns(laRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<ServiceChannelName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelName>>()).Returns(nameRepoMock.Object);

            _areaMunicipalityRepoMock = new Mock<IAreaMunicipalityRepository>();
            _areaMunicipalityRepoMock.Setup(g => g.All()).Returns(EntityGenerator.GetAreaMunicipalityList(null, _municipalityId).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaMunicipalityRepository>()).Returns(_areaMunicipalityRepoMock.Object);
            var areaRepoMock = new Mock<IAreaRepository>();
            areaRepoMock.Setup(g => g.All()).Returns(new List<Area>().AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAreaRepository>()).Returns(areaRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            // channel service
            _service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache,
                VersioningManager, UserOrganizationChecker, LanguageOrderCache, null, null, null, null, null);
        }

        [Fact]
        public void NoEntitiesFound()
        {
            // Act
            var result = _service.GetServiceChannelsByArea(Guid.NewGuid(), true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServiceChannelsFound(bool includeWholeCountry)
        {
            // Arrange
            _publishedServiceChannel.Areas = new List<ServiceChannelArea> { new ServiceChannelArea { AreaId = _areaId } };

            // Act
            var result = _service.GetServiceChannelsByArea(_areaId, includeWholeCountry, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ServiceChannelsFound_WholeCountry_IncludeWholeCountry()
        {
            // Arrange
            _publishedServiceChannel.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());

            // Act
            var result = _service.GetServiceChannelsByArea(_areaId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ServiceChannelsFound_WholeCountry_NotIncludeWholeCountry()
        {
            // Arrange
            _publishedServiceChannel.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());

            // Act
            var result = _service.GetServiceChannelsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ServiceChannelsFound_WholeCountryExceptAland_IncludeWholeCountry_AreNotInAland()
        {
            // Arrange
            _publishedServiceChannel.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());

            // Act
            var result = _service.GetServiceChannelsByArea(_areaId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
        }

        [Fact]
        public void ServiceChannelsFound_WholeCountryExceptAland_IncludeWholeCountry_AreaInAland()
        {
            // Arrange
            _publishedServiceChannel.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var areaMunicipalities = EntityGenerator.GetAreaMunicipalityList(_areaId, _municipalityId);
            var alandMunicipality = new AreaMunicipality { AreaId = Guid.NewGuid(), MunicipalityId = _municipalityId };
            var alandProvince = new Area
            {
                Id = alandMunicipality.AreaId,
                AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString()),
                Code = "20",
                AreaMunicipalities = new List<AreaMunicipality> { alandMunicipality }
            };
            alandMunicipality.Area = alandProvince;
            areaMunicipalities.Add(alandMunicipality);
            _areaMunicipalityRepoMock.Setup(g => g.All()).Returns(areaMunicipalities.AsQueryable());

            // Act
            var result = _service.GetServiceChannelsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ServiceChannelsFound_WholeCountryExceptAland_NotIncludeWholeCountry_AreNotInAland()
        {
            // Arrange
            _publishedServiceChannel.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());

            // Act
            var result = _service.GetServiceChannelsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

        [Fact]
        public void ServiceChannelsFound_WholeCountryExceptAland_NotIncludeWholeCountry_AreaInAland()
        {
            // Arrange
            _publishedServiceChannel.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var areaMunicipalities = EntityGenerator.GetAreaMunicipalityList(_areaId, _municipalityId);
            var alandMunicipality = new AreaMunicipality { AreaId = Guid.NewGuid(), MunicipalityId = _municipalityId };
            var alandProvince = new Area
            {
                Id = alandMunicipality.AreaId,
                AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString()),
                Code = "20",
                AreaMunicipalities = new List<AreaMunicipality> { alandMunicipality }
            };
            alandMunicipality.Area = alandProvince;
            areaMunicipalities.Add(alandMunicipality);
            _areaMunicipalityRepoMock.Setup(g => g.All()).Returns(areaMunicipalities.AsQueryable());

            // Act
            var result = _service.GetServiceChannelsByArea(_areaId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            var vmResult = Assert.IsType<V3VmOpenApiGuidPage>(result);
            vmResult.ItemList.Should().BeNullOrEmpty();
        }

    }
}
