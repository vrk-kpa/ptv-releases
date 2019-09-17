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

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationByIdListTests : OrganizationServiceTestBase
    {
        private IUnitOfWorkWritable unitOfWork;
        private TestContextManager contextManager;
        private ServiceUtilities serviceUtilities;
        private IOrganizationService service;

        public GetOrganizationByIdListTests()
        {
            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<OrganizationVersioned>>(),
                It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<OrganizationVersioned> orgs, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyFilters) =>
                {
                    return orgs;
                });

            unitOfWork = unitOfWorkMockSetup.Object;
            
            translationManagerMockSetup.Setup(t => t.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<List<OrganizationVersioned>>()))
                .Returns((List<OrganizationVersioned> entities) =>
                {
                    var vmList = new List<VmOpenApiOrganizationVersionBase>();
                    entities.ForEach(entity =>
                    {
                        var vm = new VmOpenApiOrganizationVersionBase()
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        };
                        if (entity.UnificRoot.Children?.Count > 0)
                        {
                            vm.SubOrganizations = new List<VmOpenApiItem>();
                            entity.UnificRoot.Children.ForEach(sub =>
                            {
                                var publishedLanguageIds = sub.LanguageAvailabilities.Where(l => l.StatusId == PublishedId).Select(l => l.LanguageId).ToList();
                                string name = null;
                                if (sub.OrganizationNames?.Count > 0)
                                {
                                    var nameItem = sub.OrganizationNames.Where(o => publishedLanguageIds.Contains(o.LocalizationId)).FirstOrDefault();
                                    if (nameItem != null) { name = nameItem.Name; }
                                }
                                vm.SubOrganizations.Add(new VmOpenApiItem { Id = sub.UnificRootId, Name = name });
                            });
                        }
                        vmList.Add(vm);
                    });
                    return vmList;
                });

            contextManager = new TestContextManager(unitOfWork, unitOfWork);
            serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManager);
            service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor, CacheManager.PostalCodeCache);

        }
        
        [Fact]
        public void IdListIsNull()
        {
            // Act
            var result = service.GetOrganizations(null, DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IdListIsEmpty()
        {
            // Act
            var result = service.GetOrganizations(new List<Guid>(), DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NoPublishedChannelsFound()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();

            var list = new List<OrganizationVersioned>
            {
                EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId1),
                EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId2)
            };

            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            OrganizationRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Never);
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<OrganizationLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { publishedChannel.UnificRootId }, DefaultVersion);

            // Assert
            result.Should().BeNull();
            OrganizationRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Never);
        }

        [Fact]
        public void CanGetItems()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var list = new List<OrganizationVersioned>
            {
                c1, c2
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
        }

        [Fact]
        public void CanGetRelatedSubOrganizations()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var subOrganization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId);
            subOrganization.ParentId = c1.UnificRootId;

            var list = new List<OrganizationVersioned>
            {
                c1, c2, subOrganization
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            
            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
            var org1 = result.Where(o => o.Id == rootId1).FirstOrDefault();
            org1.Should().NotBeNull();
            org1.SubOrganizations.Count.Should().Be(1);
            org1.SubOrganizations.First().Id.Should().Be(subOrganization.UnificRootId);
            var org2 = result.Where(o => o.Id == rootId2).FirstOrDefault();
            org2.Should().NotBeNull();
            org2.SubOrganizations.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedSubOrganizationsNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var subOrganization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(publishingStatus));
            subOrganization.ParentId = c1.UnificRootId;

            var list = new List<OrganizationVersioned>
            {
                c1, c2, subOrganization
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
            var org1 = result.Where(o => o.Id == rootId1).FirstOrDefault();
            org1.Should().NotBeNull();
            org1.SubOrganizations.Should().BeNullOrEmpty();
            var org2 = result.Where(o => o.Id == rootId2).FirstOrDefault();
            org2.Should().NotBeNull();
            org2.SubOrganizations.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetRelatedServicesWhereOrganizationAsResponsible()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var name = "Name";
            var sv = GetAndSetServiceForOrganization(c1, PublishedId, name, null, true, false, false);

            var list = new List<OrganizationVersioned>
            {
                c1, c2
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ServiceRepoMock.Setup(o => o.All()).Returns((new List<ServiceVersioned> { sv }).AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
            var org1 = result.Where(o => o.Id == rootId1).FirstOrDefault();
            org1.Should().NotBeNull();
            org1.Services.Should().NotBeNullOrEmpty();
            var resultService = org1.Services.First();
            resultService.RoleType.Should().Be("Responsible");
            resultService.Service.Id.Should().Be(sv.UnificRootId);
            resultService.Service.Name.Should().Be(name);
            var org2 = result.Where(o => o.Id == rootId2).FirstOrDefault();
            org2.Should().NotBeNull();
            org2.Services.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetRelatedServicesWhereOrganizationAsOtherResponsible()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var name = "Name";
            var sv = GetAndSetServiceForOrganization(c1, PublishedId, name, null, false, true, false);

            var list = new List<OrganizationVersioned>
            {
                c1, c2
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            OrganizationServiceRepoMock.Setup(o => o.All()).Returns(c1.UnificRoot.OrganizationServices.AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
            var org1 = result.Where(o => o.Id == rootId1).FirstOrDefault();
            org1.Should().NotBeNull();
            org1.Services.Should().NotBeNullOrEmpty();
            var resultService = org1.Services.First();
            resultService.RoleType.Should().Be("OtherResponsible");
            resultService.Service.Id.Should().Be(sv.UnificRootId);
            resultService.Service.Name.Should().Be(name);
            var org2 = result.Where(o => o.Id == rootId2).FirstOrDefault();
            org2.Should().NotBeNull();
            org2.Services.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetRelatedServicesWhereOrganizationAsProducer()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var name = "Name";
            var sv = GetAndSetServiceForOrganization(c1, PublishedId, name, null, false, false, true);

            var list = new List<OrganizationVersioned>
            {
                c1, c2
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ServiceProducerRepoMock.Setup(o => o.All()).Returns(c1.UnificRoot.ServiceProducerOrganizations.AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
            var org1 = result.Where(o => o.Id == rootId1).FirstOrDefault();
            org1.Should().NotBeNull();
            org1.Services.Should().NotBeNullOrEmpty();
            var resultService = org1.Services.First();
            resultService.RoleType.Should().Be("Producer");
            resultService.Service.Id.Should().Be(sv.UnificRootId);
            resultService.Service.Name.Should().Be(name);
            var org2 = result.Where(o => o.Id == rootId2).FirstOrDefault();
            org2.Should().NotBeNull();
            org2.Services.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServicesNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var sv = GetAndSetServiceForOrganization(c1, PublishingStatusCache.Get(publishingStatus), "Name");

            var list = new List<OrganizationVersioned>
            {
                c1, c2
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ServiceProducerRepoMock.Setup(o => o.All()).Returns(c1.UnificRoot.ServiceProducerOrganizations.AsQueryable());
            OrganizationServiceRepoMock.Setup(o => o.All()).Returns(c1.UnificRoot.OrganizationServices.AsQueryable());
            ServiceRepoMock.Setup(o => o.All()).Returns((new List<ServiceVersioned> { sv }).AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
            var org1 = result.Where(o => o.Id == rootId1).FirstOrDefault();
            org1.Should().NotBeNull();
            org1.Services.Should().BeNullOrEmpty();
            var org2 = result.Where(o => o.Id == rootId2).FirstOrDefault();
            org2.Should().NotBeNull();
            org2.Services.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedNamesReturned()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var c1 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId1);
            var c2 = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId, rootId2);
            
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            // Set sub organization 
            var subOrganization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId);
            subOrganization.ParentId = c1.UnificRootId;
            subOrganization.OrganizationNames.Add(new OrganizationName { Name = svName, LocalizationId = svId, OrganizationVersionedId = subOrganization.Id });
            subOrganization.OrganizationNames.Add(new OrganizationName { Name = fiName, LocalizationId = fiId, OrganizationVersionedId = subOrganization.Id });
            // Set only swedish as published version
            subOrganization.LanguageAvailabilities.Add(new OrganizationLanguageAvailability { LanguageId = svId, StatusId = PublishedId });

            // Set services
            // Add only swedish language as published version
            var sv = GetAndSetServiceForOrganization(c1, PublishedId, svName, svId);
            sv.ServiceNames.Add(new ServiceName { Name = fiName, LocalizationId = fiId });

            var list = new List<OrganizationVersioned>
            {
                c1, c2, subOrganization
            };

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            OrganizationNameRepoMock.Setup(g => g.All()).Returns(subOrganization.OrganizationNames.ToList().AsQueryable());
            ServiceProducerRepoMock.Setup(o => o.All()).Returns(c1.UnificRoot.ServiceProducerOrganizations.AsQueryable());
            OrganizationServiceRepoMock.Setup(o => o.All()).Returns(c1.UnificRoot.OrganizationServices.AsQueryable());
            ServiceRepoMock.Setup(o => o.All()).Returns((new List<ServiceVersioned> { sv }).AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizations(new List<Guid> { rootId1, rootId2 }, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            OrganizationRepoMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerMockSetup.Verify(x => x.TranslateAll<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<ICollection<OrganizationVersioned>>()), Times.Once);
            var org1 = result.Where(o => o.Id == rootId1).FirstOrDefault();
            org1.Should().NotBeNull();
            org1.SubOrganizations.Should().NotBeNullOrEmpty();
            var resultSubOrg = org1.SubOrganizations.First();
            resultSubOrg.Should().NotBeNull();
            resultSubOrg.Id.Should().Be(subOrganization.UnificRootId);
            resultSubOrg.Name.Should().Be(svName);
            org1.Services.Should().NotBeNullOrEmpty();
            org1.Services.ForEach(s => s.Service.Name.Should().Be(svName));
            var org2 = result.Where(o => o.Id == rootId2).FirstOrDefault();
            org2.Should().NotBeNull();
            org2.SubOrganizations.Should().BeNullOrEmpty();
            org2.Services.Should().BeNullOrEmpty();
        }
    }
}
