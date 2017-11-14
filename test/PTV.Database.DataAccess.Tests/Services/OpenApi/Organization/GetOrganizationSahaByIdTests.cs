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
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationSahaByIdTests : OrganizationServiceTestBase
    {
        [Fact]
        public void RightVersionNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Deleted, true)).Returns((Guid?)null);

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache);

            // Act
            var result = service.GetOrganizationSahaById(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DeletedVersionFound()
        {
            // Arrange
            var organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var deletedOrganization = organizationList.Where(o => o.PublishingStatusId == DeletedId).FirstOrDefault();
            var id = deletedOrganization.Id;

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns(new List<OrganizationVersioned> { deletedOrganization }.AsQueryable());
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            translationManagerMockSetup.Setup(t => t.Translate<OrganizationVersioned, VmOpenApiOrganizationSaha>(deletedOrganization))
                .Returns( new VmOpenApiOrganizationSaha());
            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Deleted, true)).Returns(Guid.NewGuid());

            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache);

            // Act
            var result = service.GetOrganizationSahaById(id);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void ApplyIncludesReturnsNull()
        {
            // Arrange
            var organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var deletedOrganization = organizationList.Where(o => o.PublishingStatusId == DeletedId).FirstOrDefault();
            var id = deletedOrganization.Id;
                        
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            translationManagerMockSetup.Setup(t => t.Translate<OrganizationVersioned, VmOpenApiOrganizationSaha>(deletedOrganization))
                .Returns(new VmOpenApiOrganizationSaha());
            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns(Guid.NewGuid());

            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache);

            // Act
            Action act = () => service.GetOrganizationSahaById(id);

            // Assert
            act.ShouldThrow<Exception>();
        }
    }
}
