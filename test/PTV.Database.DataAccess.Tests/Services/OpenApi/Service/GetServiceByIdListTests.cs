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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class GetServiceByIdListTests : ServiceServiceTestBase
    {
        private IUnitOfWorkWritable unitOfWork;
        private TestContextManager contextManager;
        private ServiceUtilities serviceUtilities;
        private IServiceService service;

        public GetServiceByIdListTests()
        {
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));

            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<ServiceVersioned>>(),
                It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<ServiceVersioned> items, Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> func, bool applyFilters) =>
                {
                    return items;
                });

            // Related organizations
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> orgs, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyFilters) =>
               {
                   return orgs;
               });

            // Channel connections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceServiceChannel>>(),
               It.IsAny<Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceServiceChannel> items, Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>> func, bool applyFilters) =>
               {
                   return items;
               });

            // Related channels
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceChannelVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceChannelVersioned> items, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });

            ArrangeTranslateAllServices();

            unitOfWork = unitOfWorkMockSetup.Object;
            contextManager = new TestContextManager(unitOfWork, unitOfWork);
            serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManager);
            service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache);
        }

        [Fact]
        public void IdListIsNull()
        {
            // Act
            var result = service.GetServices(null, DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IdListIsEmpty()
        {
            // Act
            var result = service.GetServices(new List<Guid>(), DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NoPublishedServicesFound()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();

            var list = new List<ServiceVersioned>
            {
                EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId1),
                EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId2)
            };

            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Never);
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<ServiceLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { publishedChannel.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Never);
        }

        [Fact]
        public void CanGetServices()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
        }

        [Fact]
        public void Over100ServicesWouldBeReturned()
        {
            // Arrange
            var list = new List<ServiceVersioned>();
            for (var i = 0; i < 101; i++)
            {
                var item = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId);
                list.Add(item);
            }

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            Action act = () => service.GetServices(list.Select(i => i.UnificRootId).ToList(), DefaultVersion);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void CanGetMainOrganizations()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            // Set organization data
            var orgName = "Name";
            var publishedOrganization = GetAndSetOrganizationForService(c1, PublishedId, orgName, null, true, false, false);

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { publishedOrganization }).AsQueryable());
            
            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.Organizations.Should().NotBeNullOrEmpty();
            var resultMainOrg = service1.Organizations.First();
            resultMainOrg.Should().NotBeNull();
            resultMainOrg.RoleType.Should().Be("Responsible");
            resultMainOrg.Organization.Id.Should().Be(publishedOrganization.UnificRootId);
            resultMainOrg.Organization.Name.Should().Be(orgName);
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.Organizations.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetOtherResponsibleOrganizations()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            // Set organization data
            var orgName = "Name";
            var publishedOrganization = GetAndSetOrganizationForService(c1, PublishedId, orgName, null, false, true, false);

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { publishedOrganization }).AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.Organizations.Should().NotBeNullOrEmpty();
            var resultOrg = service1.Organizations.First();
            resultOrg.Should().NotBeNull();
            resultOrg.RoleType.Should().Be("OtherResponsible");
            resultOrg.Organization.Id.Should().Be(publishedOrganization.UnificRootId);
            resultOrg.Organization.Name.Should().Be(orgName);
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.Organizations.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetProducerOrganizations()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            // Set organization data
            var orgName = "Name";
            var publishedOrganization = GetAndSetOrganizationForService(c1, PublishedId, orgName, null, false, false, true);

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { publishedOrganization }).AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.Organizations.Should().NotBeNullOrEmpty();
            var resultProducer = service1.Organizations.First();
            resultProducer.Should().NotBeNull();
            resultProducer.RoleType.Should().Be("Producer");
            resultProducer.Organization.Id.Should().Be(publishedOrganization.UnificRootId);
            resultProducer.Organization.Name.Should().Be(orgName);
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.Organizations.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedOrganizationsNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var statusId = PublishingStatusCache.Get(publishingStatus);
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            // Set organization data
            var organization = GetAndSetOrganizationForService(c1, statusId, "Name");

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { organization }).AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.Organizations.Should().BeNullOrEmpty();
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.Organizations.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedOrganizationNamesReturned()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            // Set organization data
            var name1 = "Name1";
            var name2 = "Name2";
            var enId = LanguageCache.Get("en");
            // English version published
            var organization = GetAndSetOrganizationForService(c1, PublishedId, name1, enId);
            // Lets add Finnish name - but it is not published
            organization.OrganizationNames.Add(new OrganizationName { Name = name2 });

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { organization }).AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.Organizations.Should().NotBeNullOrEmpty();
            service1.Organizations.Count.Should().Be(5);
            service1.Organizations.ForEach(o => o.Organization.Name.Should().Be(name1));// English name should be set as organization name.
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.Organizations.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetPublishedServiceChannels()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            
            // Set channel data
            var connection = GetAndSetConnectionForService(c1, PublishedId, "Name");

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns(connection.ServiceChannel.Versions.ToList().AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.ServiceChannels.Should().NotBeNullOrEmpty();
            service1.ServiceChannels.Count.Should().Be(1);
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.ServiceChannels.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServiceChannelsNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var statusId = PublishingStatusCache.Get(publishingStatus);
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            
            // Set channel data
            var connection = GetAndSetConnectionForService(c1, statusId, "Name");

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns(connection.ServiceChannel.Versions.ToList().AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.ServiceChannels.Should().BeNullOrEmpty();
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.ServiceChannels.Should().BeNullOrEmpty();
        }
                
        public void OnlyPublishedNamesReturned()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            var c1 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId2);
            
            // Set channel data
            // Add only swedish language as published version
            var connection = GetAndSetConnectionForService(c1, PublishedId, svName, svId);
            var channel = connection.ServiceChannel.Versions.First();
            channel.ServiceChannelNames.Add(new ServiceChannelName { Name = fiName, LocalizationId = fiId });

            var list = new List<ServiceVersioned>
            {
                c1, c2
            };

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns(connection.ServiceChannel.Versions.ToList().AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            var service1 = result.Where(i => i.Id == rootId1).FirstOrDefault();
            service1.Should().NotBeNull();
            service1.ServiceChannels.Should().NotBeNullOrEmpty();
            var resultConnection = service1.ServiceChannels.First();
            resultConnection.Should().NotBeNull();
            resultConnection.ServiceChannel.Should().NotBeNull();
            resultConnection.ServiceChannel.Name.Should().Be(svName);
            var service2 = result.Where(i => i.Id == rootId2).FirstOrDefault();
            service2.Should().NotBeNull();
            service2.ServiceChannels.Should().BeNullOrEmpty();
        }
    }
}
