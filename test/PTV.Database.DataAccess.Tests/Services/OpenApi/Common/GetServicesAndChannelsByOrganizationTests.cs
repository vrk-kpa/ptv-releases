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
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Common
{
    public class GetServicesAndChannelsByOrganizationTests : CommonServiceTestBase
    {
        private Guid _organizationId;
        private CommonService _service;
        private Mock<IRepository<ServiceVersioned>> serviceRepoMock;
        private Mock<IRepository<ServiceChannelVersioned>> channelRepoMock;
        private Mock<IRepository<StatutoryServiceGeneralDescriptionVersioned>> gdRepoMock;

        public GetServicesAndChannelsByOrganizationTests()
        {
            _organizationId = Guid.NewGuid();

            // unitOfWork
            serviceRepoMock = new Mock<IRepository<ServiceVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(serviceRepoMock.Object);

            channelRepoMock = new Mock<IRepository<ServiceChannelVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(channelRepoMock.Object);

            gdRepoMock = new Mock<IRepository<StatutoryServiceGeneralDescriptionVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<StatutoryServiceGeneralDescriptionVersioned>>()).Returns(gdRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);
            _service = new CommonService(translationManagerMockSetup.Object, TranslationManagerVModel, contextManager, CacheManager.TypesCache, PublishingStatusCache,
                UserOrganizationChecker, DataServiceFetcherMock, serviceUtilities, VersioningManager, ValidationManagerMock, ApplicationConfigurationMock, OrganizationTreeDataCache, RestrictionFilterManager, PahaTokenProcessor, LanguageCache, null, null, null);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void NoEntitesFound(bool getSpecialTypes)
        {
            // Arrange
            // unitOfWork
            serviceRepoMock.Setup(o => o.All()).Returns(EntityGenerator.GetServiceEntityList(0, PublishingStatusCache).AsQueryable());

            channelRepoMock.Setup(o => o.All()).Returns(new List<ServiceChannelVersioned>().AsQueryable());
            
            // Act
            var result = _service.GetServicesAndChannelsByOrganization(Guid.NewGuid(), getSpecialTypes, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(0);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ServicesFound(bool getSpecialTypes)
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.OrganizationId = _organizationId;

            // unitOfWork
            serviceRepoMock.Setup(o => o.All()).Returns(services.AsQueryable());

            channelRepoMock.Setup(o => o.All()).Returns(new List<ServiceChannelVersioned>().AsQueryable());
            
            // Act
            var result = _service.GetServicesAndChannelsByOrganization(_organizationId, getSpecialTypes, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().NotBeNullOrEmpty();
            (result as VmOpenApiEntityGuidPage).ItemList.Count.Should().Be(1);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ChannelsFound(bool getSpecialTypes)
        {
            // Arrange
            var channels = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            var publishedChannel = channels.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.OrganizationId = _organizationId;

            // unitOfWork
            serviceRepoMock.Setup(o => o.All()).Returns(EntityGenerator.GetServiceEntityList(0, PublishingStatusCache).AsQueryable());

            channelRepoMock.Setup(o => o.All()).Returns(channels.AsQueryable());

            // Act
            var result = _service.GetServicesAndChannelsByOrganization(_organizationId, getSpecialTypes, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().NotBeNullOrEmpty();
            (result as VmOpenApiEntityGuidPage).ItemList.Count.Should().Be(1);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ServicesAndChannelsFound(bool getSpecialTypes)
        {
            // Arrange
            var services = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedService = services.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedService.OrganizationId = _organizationId;
            var channels = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            var publishedChannel = channels.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedChannel.OrganizationId = _organizationId;

            // unitOfWork
            serviceRepoMock.Setup(o => o.All()).Returns(services.AsQueryable());

            channelRepoMock.Setup(o => o.All()).Returns(channels.AsQueryable());
                        
            // Act
            var result = _service.GetServicesAndChannelsByOrganization(_organizationId, getSpecialTypes, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(2);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            (result as VmOpenApiEntityGuidPage).ItemList.Should().NotBeNullOrEmpty();
            (result as VmOpenApiEntityGuidPage).ItemList.Count.Should().Be(1);
        }

        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.ServiceLocation)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        public void CanReturnGenericTypesForChannel(ServiceChannelTypeEnum type)
        {
            // Arrange
            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));

            var publishedChannel = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId);
            publishedChannel.OrganizationId = _organizationId;
            publishedChannel.TypeId = TypeCache.Get<ServiceChannelType>(type.ToString());

            // unitOfWork
            channelRepoMock.Setup(o => o.All()).Returns(new List<ServiceChannelVersioned> { publishedChannel }.AsQueryable());

            // Act
            var result = _service.GetServicesAndChannelsByOrganization(_organizationId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            var vmResult = Assert.IsType<VmOpenApiEntityGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
            vmResult.ItemList.First().Type.Should().Be("ServiceChannel");
        }

        [Theory]
        [InlineData(ServiceTypeEnum.PermissionAndObligation)]
        [InlineData(ServiceTypeEnum.ProfessionalQualifications)]
        [InlineData(ServiceTypeEnum.Service)]
        public void CanReturnGenericTypesForService(ServiceTypeEnum type)
        {
            // Arrange
            SetupTypesCacheMock<ServiceType>(typeof(ServiceTypeEnum));

            var publishedService = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId);
            publishedService.OrganizationId = _organizationId;
            publishedService.TypeId = TypeCache.Get<ServiceType>(type.ToString());

            // unitOfWork
            serviceRepoMock.Setup(o => o.All()).Returns(new List<ServiceVersioned> { publishedService }.AsQueryable());

            // Act
            var result = _service.GetServicesAndChannelsByOrganization(_organizationId, false, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            var vmResult = Assert.IsType<VmOpenApiEntityGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
            vmResult.ItemList.First().Type.Should().Be("Service");
        }

        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.ServiceLocation)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        public void CanReturnSpecialTypesForChannel(ServiceChannelTypeEnum type)
        {
            // Arrange
            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));

            var publishedChannel = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId);
            publishedChannel.OrganizationId = _organizationId;
            publishedChannel.TypeId = TypeCache.Get<ServiceChannelType>(type.ToString());

            // unitOfWork
            channelRepoMock.Setup(o => o.All()).Returns(new List<ServiceChannelVersioned> { publishedChannel }.AsQueryable());

            // Act
            var result = _service.GetServicesAndChannelsByOrganization(_organizationId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            var vmResult = Assert.IsType<VmOpenApiEntityGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
            vmResult.ItemList.First().Type.Should().Be(type.ToString());
        }

        [Theory]
        [InlineData(ServiceTypeEnum.PermissionAndObligation)]
        [InlineData(ServiceTypeEnum.ProfessionalQualifications)]
        [InlineData(ServiceTypeEnum.Service)]
        public void CanReturnSpecialTypesForService(ServiceTypeEnum type)
        {
            // Arrange
            SetupTypesCacheMock<ServiceType>(typeof(ServiceTypeEnum));

            var publishedService = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId);
            publishedService.OrganizationId = _organizationId;
            publishedService.TypeId = TypeCache.Get<ServiceType>(type.ToString());

            // unitOfWork
            serviceRepoMock.Setup(o => o.All()).Returns(new List<ServiceVersioned> { publishedService }.AsQueryable());

            // Act
            var result = _service.GetServicesAndChannelsByOrganization(_organizationId, true, null, 1, 1);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            result.Should().BeOfType<VmOpenApiEntityGuidPage>();
            var vmResult = Assert.IsType<VmOpenApiEntityGuidPage>(result);
            vmResult.ItemList.Should().NotBeNullOrEmpty();
            vmResult.ItemList.Count.Should().Be(1);
            vmResult.ItemList.First().Type.Should().Be(type.ToString().GetOpenApiEnumValue<ServiceTypeEnum>());
        }
    }
}
