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
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationsHierarchyByIdTests : OrganizationServiceTestBase
    {
        private DataAccess.Services.OrganizationService _service;

        public GetOrganizationsHierarchyByIdTests()
        {
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> orgList, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyfilters) =>
               {
                   return orgList;
               });

            translationManagerMockSetup.Setup(t => t.Translate<OrganizationVersioned, VmOpenApiOrganizationHierarchy>(It.IsAny<OrganizationVersioned>()))
                .Returns((OrganizationVersioned org) =>
                {
                    if (org == null)
                    {
                        return null;
                    }
                    var orgWithParent = GetOrganizationWithParents(org);
                    if (orgWithParent == null) return null;

                    var vm = new VmOpenApiOrganizationHierarchy
                    {
                        Id = orgWithParent.Id,
                        OrganizationNames = orgWithParent.OrganizationNames,
                        Parent = orgWithParent.Parent
                    };

                    if (org.UnificRoot.Children?.Count > 0)
                    {
                        vm.SubOrganizations = GetSubOrganizations(org);
                    }
                    return vm;
                });


            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor);
        }

        [Fact]
        public void OrganizationNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, id, PublishingStatus.Published, true)).Returns((Guid?)null);

            //Act
            var result = _service.GetOrganizationsHierarchy(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NoParentsOrSubOrganizationsFound()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var versionId = Guid.NewGuid();
            var list = new List<OrganizationVersioned> { EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId, versionId) };
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, rootId, PublishingStatus.Published, true)).Returns(versionId);
            OrganizationRepoMock.Setup(o => o.All()).Returns(list.AsQueryable());
            //Act
            var result = _service.GetOrganizationsHierarchy(rootId);

            // Assert
            result.Should().NotBeNull();
            var model = Assert.IsAssignableFrom<VmOpenApiOrganizationHierarchy>(result);
            model.Parent.Should().BeNull();
            model.SubOrganizations.Should().NotBeNull();
            model.SubOrganizations.Count.Should().Be(0);
        }

        [Fact]
        public void ThreeLevelsOfParentsFound()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var versionId = Guid.NewGuid();
            var organization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId, versionId);
            var parent = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            organization.Parent = parent.UnificRoot;
            organization.ParentId = organization.Parent.Id;
            var grandParent = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            parent.Parent = grandParent.UnificRoot;
            parent.ParentId = grandParent.UnificRootId;
            var grandGrandParent = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            grandParent.Parent = grandGrandParent.UnificRoot;
            grandParent.ParentId = grandGrandParent.UnificRootId;
            var list = new List<OrganizationVersioned> { organization, parent, grandParent, grandGrandParent };

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, rootId, PublishingStatus.Published, true)).Returns(versionId);
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, parent.UnificRootId, PublishingStatus.Published, true)).Returns(parent.Id);
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, grandParent.UnificRootId, PublishingStatus.Published, true)).Returns(grandParent.Id);
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, grandGrandParent.UnificRootId, PublishingStatus.Published, true)).Returns(grandGrandParent.Id);

            OrganizationRepoMock.Setup(o => o.All()).Returns(list.AsQueryable());
            //Act
            var result = _service.GetOrganizationsHierarchy(rootId);

            // Assert
            result.Should().NotBeNull();
            var model = Assert.IsAssignableFrom<VmOpenApiOrganizationHierarchy>(result);
            model.Parent.Should().NotBeNull();
            model.Parent.Parent.Should().NotBeNull();
            model.Parent.Parent.Parent.Should().NotBeNull();
        }

        [Fact]
        public void ThreeLevelsOfSubOrganizationsFound()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var versionId = Guid.NewGuid();
            var organization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId, versionId);
            var child = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            organization.UnificRoot.Children.Add(child);
            var grandChild = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            child.UnificRoot.Children.Add(grandChild);
            var grandGrandChild = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            grandChild.UnificRoot.Children.Add(grandGrandChild);
            var list = new List<OrganizationVersioned> { organization, child, grandChild, grandGrandChild };

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, rootId, PublishingStatus.Published, true)).Returns(versionId);
            
            OrganizationRepoMock.Setup(o => o.All()).Returns(list.AsQueryable());
            //Act
            var result = _service.GetOrganizationsHierarchy(rootId);

            // Assert
            result.Should().NotBeNull();
            var model = Assert.IsAssignableFrom<VmOpenApiOrganizationHierarchy>(result);
            model.SubOrganizations.FirstOrDefault().Should().NotBeNull();
            model.SubOrganizations.FirstOrDefault().SubOrganizations.FirstOrDefault().Should().NotBeNull();
            model.SubOrganizations.FirstOrDefault().SubOrganizations.FirstOrDefault().SubOrganizations.FirstOrDefault().Should().NotBeNull();
        }

        private IList<VmOpenApiOrganizationSub> GetSubOrganizations(OrganizationVersioned org)
        {
            var list = new List<VmOpenApiOrganizationSub>();

            if (org == null || org.UnificRoot == null || org.UnificRoot.Children.Count == 0) return list;

            org.UnificRoot.Children.ForEach(child =>
            {
                var childModel = new VmOpenApiOrganizationSub
                {
                    Id = child.UnificRootId,
                    OrganizationNames = new List<VmOpenApiLocalizedListItem>()
                };
                if (child.OrganizationNames?.Count > 0)
                {
                    child.OrganizationNames.ForEach(n => childModel.OrganizationNames.Add(new VmOpenApiLocalizedListItem
                    {
                        Value = n.Name,
                        Type = TypeCache.GetByValue<NameType>(n.TypeId).GetOpenApiEnumValue<NameTypeEnum>(),
                        Language = LanguageCache.GetByValue(n.LocalizationId)
                    }));
                }
                if (child.UnificRoot != null && child.UnificRoot.Children?.Count > 0)
                {
                    childModel.SubOrganizations = GetSubOrganizations(child);
                }
                list.Add(childModel);
            });

            return list;
        }
        private VmOpenApiOrganizationParent GetOrganizationWithParents(OrganizationVersioned org)
        {
            if (org == null) return null;

            var model = new VmOpenApiOrganizationParent
            {
                Id = org.UnificRootId,
                OrganizationNames = new List<VmOpenApiLocalizedListItem>()
            };
            if (org.OrganizationNames?.Count > 0)
            {
                org.OrganizationNames.ForEach(n => model.OrganizationNames.Add(new VmOpenApiLocalizedListItem
                {
                    Value = n.Name,
                    Type = TypeCache.GetByValue<NameType>(n.TypeId).GetOpenApiEnumValue<NameTypeEnum>(),
                    Language = LanguageCache.GetByValue(n.LocalizationId)
                }));
            }
            if (org.Parent != null)
            {
                model.Parent = GetOrganizationWithParents(org.Parent.Versions.FirstOrDefault());
            }

            return model;
        }
    }
}
