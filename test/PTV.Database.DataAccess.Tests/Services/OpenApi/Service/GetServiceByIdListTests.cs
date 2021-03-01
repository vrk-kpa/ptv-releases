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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
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

            // Related service collections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<Model.Models.ServiceCollectionService>>(),
               It.IsAny<Func<IQueryable<Model.Models.ServiceCollectionService>, IQueryable<Model.Models.ServiceCollectionService>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<Model.Models.ServiceCollectionService> items, Func<IQueryable<Model.Models.ServiceCollectionService>, IQueryable<Model.Models.ServiceCollectionService>> func, bool applyFilters) =>
               {
                   return items;
               });

            ArrangeTranslateAllServices();

            unitOfWork = unitOfWorkMockSetup.Object;
            contextManager = new TestContextManager(unitOfWork, unitOfWork);
            serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManager);
            service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, null, null, null, null);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IdListIsNull(bool fillWithAllGdData)
        {
            // Act
            var result = service.GetServices(null, DefaultVersion, fillWithAllGdData, false);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IdListIsEmpty(bool fillWithAllGdData)
        {
            // Act
            var result = service.GetServices(new List<Guid>(), DefaultVersion, fillWithAllGdData, false);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void NoPublishedServicesFound(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

            // Assert
            result.Should().BeNull();
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void NoLanguageVersionsPublished(bool fillWithAllGdData)
        {
            // Arrange
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<ServiceLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetServices(new List<Guid> { publishedChannel.UnificRootId }, DefaultVersion, fillWithAllGdData, false);

            // Assert
            result.Should().BeNull();
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanGetServices(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Over100ServicesWouldBeReturned(bool fillWithAllGdData)
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

            Action act = () => service.GetServices(list.Select(i => i.UnificRootId).ToList(), DefaultVersion, fillWithAllGdData, false);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanGetMainOrganizations(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanGetOtherResponsibleOrganizations(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanGetProducerOrganizations(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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
        [InlineData(PublishingStatus.Deleted, true)]
        [InlineData(PublishingStatus.Deleted, false)]
        [InlineData(PublishingStatus.Draft, true)]
        [InlineData(PublishingStatus.Draft, false)]
        [InlineData(PublishingStatus.Modified, true)]
        [InlineData(PublishingStatus.Modified, false)]
        [InlineData(PublishingStatus.OldPublished, true)]
        [InlineData(PublishingStatus.OldPublished, false)]
        public void NotPublishedOrganizationsNotReturned(PublishingStatus publishingStatus, bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OnlyPublishedOrganizationNamesReturned(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanGetPublishedServiceChannels(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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
        [InlineData(PublishingStatus.Deleted, true)]
        [InlineData(PublishingStatus.Deleted, false)]
        [InlineData(PublishingStatus.Draft, true)]
        [InlineData(PublishingStatus.Draft, false)]
        [InlineData(PublishingStatus.Modified, true)]
        [InlineData(PublishingStatus.Modified, false)]
        [InlineData(PublishingStatus.OldPublished, true)]
        [InlineData(PublishingStatus.OldPublished, false)]
        public void NotPublishedServiceChannelsNotReturned(PublishingStatus publishingStatus, bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OnlyPublishedNamesReturned(bool fillWithAllGdData)
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
            var result = service.GetServices(new List<Guid> { rootId1, rootId2 }, DefaultVersion, fillWithAllGdData, false);

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
/* SOTE has been disabled (SFIPTV-1177)
        [Theory]
        [InlineData(GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct)]
        [InlineData(GeneralDescriptionTypeEnum.OtherPermissionGrantedSote)]
        public void GdDataIsAttached_DoNotFillWithAllGdData(GeneralDescriptionTypeEnum subType)
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var entity = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId);
            var list = new List<ServiceVersioned>
            {
                entity
            };
            var gdId = Guid.NewGuid();
            var gd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                Type = ServiceTypeEnum.PermissionAndObligation.ToString(),
                GeneralDescriptionType = subType.ToString(),
                TargetGroups = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> { new V7VmOpenApiFintoItemWithDescription { Id = Guid.NewGuid() } },
                OntologyTerms = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            entity.StatutoryServiceGeneralDescriptionId = gdId;

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            gdServiceMock.Setup(x => x.GetGeneralDescriptionsSimple(new List<Guid> { gdId })).Returns(new List<VmOpenApiGeneralDescriptionVersionBase> { gd });

            // Act
            var result = service.GetServices(new List<Guid> { rootId }, DefaultVersion, false);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            gdServiceMock.Verify(x => x.GetGeneralDescriptionsSimple(It.IsAny<List<Guid>>()), Times.Once);
            var vmService = result.Where(i => i.Id == rootId).FirstOrDefault();
            vmService.Should().NotBeNull();
            var vmResult = Assert.IsType<V10VmOpenApiService>(vmService);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Type.Should().Be(ServiceTypeEnum.PermissionAndObligation.ToString());
            vmResult.SubType.Should().Be(subType.ToString());
            vmResult.TargetGroups.Count.Should().Be(1);
            vmResult.ServiceClasses.Count.Should().Be(1);
            vmResult.OntologyTerms.Count.Should().Be(1);
        }
*/
        [Theory]
        [InlineData(GeneralDescriptionTypeEnum.AlandIsland)]
        [InlineData(GeneralDescriptionTypeEnum.BusinessSubregion)]
        [InlineData(GeneralDescriptionTypeEnum.Municipality)]
        public void GdSubTypeOtherThanSoteNotAttached_DoNotFillWithAllGdData(GeneralDescriptionTypeEnum subType)
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var entity = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId);
            var list = new List<ServiceVersioned>
            {
                entity
            };
            var gdId = Guid.NewGuid();
            var gd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                Type = ServiceTypeEnum.PermissionAndObligation.ToString(),
                GeneralDescriptionType = subType.ToString(),
                TargetGroups = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> { new V7VmOpenApiFintoItemWithDescription { Id = Guid.NewGuid() } },
                OntologyTerms = new List<V4VmOpenApiOntologyTerm> { new V4VmOpenApiOntologyTerm { Id = Guid.NewGuid() } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            entity.StatutoryServiceGeneralDescriptionId = gdId;

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            gdServiceMock.Setup(x => x.GetGeneralDescriptionsSimple(new List<Guid> { gdId })).Returns(new List<VmOpenApiGeneralDescriptionVersionBase> { gd });

            // Act
            var result = service.GetServices(new List<Guid> { rootId }, DefaultVersion, false, false);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            gdServiceMock.Verify(x => x.GetGeneralDescriptionsSimple(It.IsAny<List<Guid>>()), Times.Once);
            var vmService = result.Where(i => i.Id == rootId).FirstOrDefault();
            vmService.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(vmService);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Type.Should().Be(ServiceTypeEnum.PermissionAndObligation.ToString());
            vmResult.SubType.Should().BeNull();
            vmResult.TargetGroups.Count.Should().Be(1);
            vmResult.ServiceClasses.Count.Should().Be(1);
            vmResult.OntologyTerms.Count.Should().Be(1);
        }

        [Theory]
/* SOTE has been disabled (SFIPTV-1177)
        [InlineData(GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct)]
        [InlineData(GeneralDescriptionTypeEnum.OtherPermissionGrantedSote)]
*/
        [InlineData(GeneralDescriptionTypeEnum.AlandIsland)]
        [InlineData(GeneralDescriptionTypeEnum.BusinessSubregion)]
        [InlineData(GeneralDescriptionTypeEnum.Municipality)]
        public void GdDataIsAttached_FillWithAllGdData(GeneralDescriptionTypeEnum subType)
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var entity = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(PublishedId, rootId);
            var list = new List<ServiceVersioned>
            {
                entity
            };
            var gdId = Guid.NewGuid();
            var fi = "fi";
            var descriptionType = DescriptionTypeEnum.Description.ToString();
            var gdDescriptionValue = "Text";
            var gd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                Type = ServiceTypeEnum.PermissionAndObligation.ToString(),
                GeneralDescriptionType = subType.ToString(),
                ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                Descriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem {
                    Language = fi, Type = descriptionType, Value = gdDescriptionValue } },
                TargetGroups = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> { new V7VmOpenApiFintoItemWithDescription { Id = Guid.NewGuid() } },
                OntologyTerms = new List<V4VmOpenApiOntologyTerm> { new V4VmOpenApiOntologyTerm { Id = Guid.NewGuid() } },
                LifeEvents = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                IndustrialClasses = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Id = Guid.NewGuid() } },
                Legislation = new List<V4VmOpenApiLaw> { new V4VmOpenApiLaw { Id = Guid.NewGuid() } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            entity.StatutoryServiceGeneralDescriptionId = gdId;

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            gdServiceMock.Setup(x => x.GetPublishedGeneralDescriptionsWithDetails(new List<Guid> { gdId }, It.IsAny<int>(),It.IsAny<bool>())).Returns(new List<VmOpenApiGeneralDescriptionVersionBase> { gd });

            // Act
            var result = service.GetServices(new List<Guid> { rootId }, DefaultVersion, true, false);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            ServiceRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);
            gdServiceMock.Verify(x => x.GetPublishedGeneralDescriptionsWithDetails(It.IsAny<List<Guid>>(), It.IsAny<int>(),It.IsAny<bool>()), Times.Once);
            var vmService = result.Where(i => i.Id == rootId).FirstOrDefault();
            vmService.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(vmService);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Type.Should().Be(ServiceTypeEnum.PermissionAndObligation.ToString());
            vmResult.SubType.Should().Be(subType.ToString());
            vmResult.ServiceChargeType.Should().Be(ServiceChargeTypeEnum.Charged.ToString());
            var vmDescription = vmResult.ServiceDescriptions.FirstOrDefault(d => d.Type == "GD_" + descriptionType);
            vmDescription.Should().NotBeNull();
            vmDescription.Value.Should().Be(gdDescriptionValue);
            vmResult.TargetGroups.Count.Should().Be(1);
            vmResult.ServiceClasses.Count.Should().Be(1);
            vmResult.OntologyTerms.Count.Should().Be(1);
            vmResult.LifeEvents.Count.Should().Be(1);
            vmResult.IndustrialClasses.Count.Should().Be(1);
            vmResult.Requirements.Count.Should().Be(1);
            vmResult.Legislation.Count.Should().Be(1);
        }
    }
}
