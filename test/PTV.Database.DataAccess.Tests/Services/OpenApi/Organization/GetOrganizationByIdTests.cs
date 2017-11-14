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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationByIdTests : OrganizationServiceTestBase
    {
        private List<OrganizationVersioned> _organizationList;
        private OrganizationVersioned _publishedOrganization;
        private Guid _publishedOrganizationId;
        private Guid _publishedOrganizationRootId;

        public GetOrganizationByIdTests()
        {
            _organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            _publishedOrganization = _organizationList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            _publishedOrganizationId = _publishedOrganization.Id;
            _publishedOrganizationRootId = _publishedOrganization.UnificRootId;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RightVersionNotFound(bool getOnlyPublished)
        {
            // Arrange
            var id = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, null, false)).Returns((Guid?)null);

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache);

            // Act
            var result = service.GetOrganizationById(id, DefaultVersion, getOnlyPublished);

            // Assert
            result.Should().BeNull();
            if (getOnlyPublished)
            {
                VersioningManagerMock.Verify(x => x.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true), Times.Once());
            }
            else
            {
                VersioningManagerMock.Verify(x => x.GetVersionId<OrganizationVersioned>(unitOfWork, id, null, false), Times.Once());
            }
        }

        [Fact]
        public void InterfaceVersion7CanBeFetched()
        {
            // Arrange           
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 7);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V7VmOpenApiOrganization>();
        }

        [Fact]
        public void InterfaceVersion6CanBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 6);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V6VmOpenApiOrganization>();
        }

        [Fact]
        public void InterfaceVersion5CanBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 5);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V5VmOpenApiOrganization>();
        }

        [Fact]
        public void InterfaceVersion4CanBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 4);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V4VmOpenApiOrganization>();
        }

        [Fact]
        public void InterfaceVersion3CannotBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            Action act = () => service.GetOrganizationById(_publishedOrganizationRootId, 3);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<OrganizationLanguageAvailability>());
            var publishedOrganization = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var service = Arrange(list);

            // Act
            var result = service.GetOrganizationById(publishedOrganization.UnificRootId, DefaultVersion);

            // Assert
            result.Should().BeNull();
            unitOfWorkMockSetup.Verify(x => x.ApplyIncludes(It.IsAny<IQueryable<OrganizationVersioned>>(),
                It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(), It.IsAny<bool>()), Times.Once());
        }

        private DataAccess.Services.OrganizationService Arrange(List<OrganizationVersioned> list = null)
        {
            var organizationList = list ?? _organizationList;
            var publishedOrganization = organizationList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var id = publishedOrganization.Id;
            var rootId = publishedOrganization.UnificRootId;

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> orgList, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyfilters) =>
               {
                   return orgList;
               });
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);
                        
            translationManagerMockSetup.Setup(t => t.Translate<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<OrganizationVersioned>()))
                .Returns((OrganizationVersioned org) =>
                {
                    if (org == null)
                    {
                        return null;
                    }                    
                    return new VmOpenApiOrganizationVersionBase { Id = org.UnificRootId, Modified = org.Modified, PublishingStatus = TypeCache.GetByValue<PublishingStatusType>(org.PublishingStatusId) };
                });
                
            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, rootId, PublishingStatus.Published, true)).Returns(id);

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());
            OrganizationNameRepoMock.Setup(m => m.All()).Returns(Enumerable.Empty<OrganizationName>().AsQueryable());
            var serviceNameRepoMock = new Mock<IServiceNameRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceNameRepository>()).Returns(serviceNameRepoMock.Object);
            var organizationServiceRepoMock = new Mock<IOrganizationServiceRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationServiceRepository>()).Returns(organizationServiceRepoMock.Object);
            var serviceRepoMock = new Mock<IServiceVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(serviceRepoMock.Object);
            var serviceProducerRepoMock = new Mock<IServiceProducerOrganizationRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceProducerOrganizationRepository>()).Returns(serviceProducerRepoMock.Object);

            return new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache);
        }
    }
}
