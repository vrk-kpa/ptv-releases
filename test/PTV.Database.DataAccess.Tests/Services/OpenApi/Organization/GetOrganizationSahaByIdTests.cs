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
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationSahaByIdTests : OrganizationServiceTestBase
    {
        private DataAccess.Services.OrganizationService _service;

        public GetOrganizationSahaByIdTests()
        {
            SetupTypesCacheMock<PublishingStatusType>(typeof(PublishingStatus));
            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> orgs, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyFilters) =>
               {
                   return orgs;
               });

            translationManagerMockSetup.Setup(t => t.Translate<OrganizationVersioned, VmOpenApiOrganizationSaha>(It.IsAny<OrganizationVersioned>()))
                .Returns((OrganizationVersioned ov) =>
                {
                    if (ov == null) return null;

                    var vm = new VmOpenApiOrganizationSaha()
                    {
                        Id = ov.UnificRootId,
                        ParentOrganizationId = ov.ParentId,
                        Modified = ov.Modified,
                        PublishingStatus = PublishingStatusCache.GetByValue(ov.PublishingStatusId),

                    };
                    if (ov.UnificRoot.SahaOrganizationInformations?.FirstOrDefault() != null)
                    {
                        vm.SahaId = ov.UnificRoot.SahaOrganizationInformations.FirstOrDefault().SahaId.ToString();
                    }
                    return vm;
                });

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor, CacheManager.PostalCodeCache);
        }
        [Fact]
        public void RightVersionNotFound()
        {
            Assert.Throws<PtvAppException>(() => _service.GetOrganizationSahaById(Guid.NewGuid()));
        }

        [Fact]
        public void DeletedVersionFound()
        {
            // Arrange
            var organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var deletedOrganization = organizationList.Where(o => o.PublishingStatusId == DeletedId).FirstOrDefault();
            var rootId = deletedOrganization.UnificRootId;
            var id = deletedOrganization.Id;
            
            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());
            
            // Act
            var result = _service.GetOrganizationSahaById(rootId);

            // Assert
            result.Should().NotBeNull();
            result.PublishingStatus.Should().Be(PublishingStatus.Deleted.ToString());
        }

        [Fact]
        public void ApplyIncludesReturnsNull()
        {
            // Arrange
            var organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var deletedOrganization = organizationList.Where(o => o.PublishingStatusId == DeletedId).FirstOrDefault();
            var id = deletedOrganization.Id;
                        
            var unitOfWork = unitOfWorkMockSetup.Object;
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns(Guid.NewGuid());

            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());
            
            // Act
            Action act = () => _service.GetOrganizationSahaById(id);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void SahaIdCanBeReturned()
        {
            // Arrange
            var sahaId = Guid.NewGuid();
            var organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedOrganization = organizationList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedOrganization.UnificRoot.SahaOrganizationInformations.Add(new SahaOrganizationInformation { SahaId = sahaId });

            var unitOfWork = unitOfWorkMockSetup.Object;
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, publishedOrganization.UnificRootId, It.IsAny<PublishingStatus>(), true)).Returns(publishedOrganization.Id);

            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());
            
            // Act
            var result = _service.GetOrganizationSahaById(publishedOrganization.UnificRootId);

            // Assert
            result.Should().NotBeNull();
            result.SahaId.Should().Be(sahaId.ToString());
        }
    }
}
