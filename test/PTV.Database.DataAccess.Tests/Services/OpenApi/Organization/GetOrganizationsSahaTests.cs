﻿/**
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
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationsSahaTests : OrganizationServiceTestBase
    {
        private List<OrganizationVersioned> organizationList;
        private DataAccess.Services.OrganizationService _service;
        private Mock<IRepository<OrganizationVersioned>> organizationMock;
        private Mock<IRepository<SahaOrganizationInformation>> sahaRepoMock;

        public GetOrganizationsSahaTests()
        {
            // unitOfWork
            organizationMock = new Mock<IRepository<OrganizationVersioned>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationVersioned>>()).Returns(organizationMock.Object);
            sahaRepoMock = new Mock<IRepository<SahaOrganizationInformation>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<SahaOrganizationInformation>>()).Returns(sahaRepoMock.Object);
            var nameRepoMock = new Mock<IRepository<OrganizationName>>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationName>>()).Returns(nameRepoMock.Object);

            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor, CacheManager.PostalCodeCache, null);

        }

        [Fact]
        public void NoDateDefined()
        {
            // Arrange
            var pageSize = 5;
            organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedAndDeletedList = organizationList.Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) ||
                e.PublishingStatusId == DeletedId || e.PublishingStatusId == OldPublishedId).ToList();
            organizationMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());

            // Act
            var result = _service.GetOrganizationsSaha(null, 1, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vm = Assert.IsType<VmOpenApiOrganizationSahaGuidPage>(result);
            vm.ItemList.Should().NotBeNullOrEmpty();
            vm.ItemList.Count.Should().Be(publishedAndDeletedList.Count);
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.Published.ToString()).FirstOrDefault().Should().NotBeNull();
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.OldPublished.ToString()).FirstOrDefault().Should().NotBeNull();
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.Deleted.ToString()).FirstOrDefault().Should().NotBeNull();
        }

        [Fact]
        public void OnlyTwoSubOrganizationLevelsReturned()
        {
            // Arrange
            // set an organization list where exists three levels of suborganizations. So we need at least 4 items in a list.
            organizationList = EntityGenerator.GetOrganizationEntityList(2, PublishingStatusCache); // 2 * 5 items
            var publishedAndDeletedList = organizationList.Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) ||
                e.PublishingStatusId == DeletedId || e.PublishingStatusId == OldPublishedId).ToList(); // includes 6 items = 2 * 3
            var mainOrganization = publishedAndDeletedList.FirstOrDefault();
            var nextItem = 1;
            var parentId = mainOrganization.UnificRootId;
            publishedAndDeletedList.ForEach(o =>
            {
                if (o.Id != mainOrganization.Id)
                {
                    o.ParentId = parentId;
                    switch (nextItem)
                    {
                        case 1:
                            // First level organization
                            o.Parent = new Model.Models.Organization { Versions = new List<OrganizationVersioned>
                            {
                                new OrganizationVersioned { PublishingStatusId = PublishedId }
                            } };
                            nextItem = 2;
                            break;
                        case 2:
                            // Second level organization
                            o.Parent = new Model.Models.Organization
                            { Versions = new List<OrganizationVersioned>
                            {
                                new OrganizationVersioned
                                {
                                    PublishingStatusId = PublishedId,
                                    ParentId = Guid.NewGuid(),
                                    Parent = new Model.Models.Organization
                                    {
                                        Versions = new List<OrganizationVersioned>
                                        {
                                            new OrganizationVersioned { PublishingStatusId = PublishedId }
                                        }
                                    }
                                }
                            } };
                            nextItem = 3;
                            break;
                        case 3:
                            // Third level organization
                            o.Parent = new Model.Models.Organization
                            {
                                Versions = new List<OrganizationVersioned>
                                {
                                    new OrganizationVersioned
                                    {
                                        PublishingStatusId = PublishedId,
                                        ParentId = Guid.NewGuid(),
                                        Parent = new Model.Models.Organization
                                        {
                                            Versions = new List<OrganizationVersioned>
                                            {
                                                new OrganizationVersioned
                                                {
                                                    PublishingStatusId = PublishedId,
                                                    ParentId = Guid.NewGuid(),
                                                    Parent = new Model.Models.Organization { Versions = new List<OrganizationVersioned>
                                                    {
                                                        new OrganizationVersioned { PublishingStatusId = PublishedId }
                                                    } }
                                                }
                                            },
                                        }
                                    }
                                }
                            };
                            nextItem = 3;
                            break;
                        default:
                            break;
                    }
                    parentId = o.Id;
                }
            });
            var validLevelsList = publishedAndDeletedList.Where(o => o.ParentId == null || // main level
                (o.Parent != null && o.Parent.Versions.Any(p => (p.PublishingStatusId == PublishedId || p.PublishingStatusId == DeletedId) && (p.ParentId == null ||// first level child
                p.Parent != null && p.Parent.Versions.Any(pp => (pp.PublishingStatusId == PublishedId || pp.PublishingStatusId == DeletedId) && pp.ParentId == null)))));

            organizationMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());

            // Act
            var result = _service.GetOrganizationsSaha(null, 1, 5);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vm = Assert.IsType<VmOpenApiOrganizationSahaGuidPage>(result);
            vm.ItemList.Should().NotBeNullOrEmpty();
            vm.ItemList.Count.Should().Be(validLevelsList.Count());
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.Published.ToString()).FirstOrDefault().Should().NotBeNull();
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.OldPublished.ToString()).FirstOrDefault().Should().NotBeNull();
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.Deleted.ToString()).FirstOrDefault().Should().NotBeNull();
        }

        [Fact]
        public void SahaIdCanBeReturned()
        {
            // Arrange
            organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            var publishedAndDeletedList = organizationList.Where(e => e.PublishingStatusId == PublishedId && e.LanguageAvailabilities.Any(l => l.StatusId == PublishedId) ||
                e.PublishingStatusId == DeletedId || e.PublishingStatusId == OldPublishedId).ToList();
            var mainOrganization = publishedAndDeletedList.FirstOrDefault();
            var sahaId = Guid.NewGuid();
            var mainOrgId = mainOrganization.UnificRootId;
            organizationMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());
            sahaRepoMock.Setup(s => s.All()).Returns((new List<SahaOrganizationInformation> { new SahaOrganizationInformation { OrganizationId = mainOrgId, SahaId = sahaId } }).AsQueryable());

            // Act
            var result = _service.GetOrganizationsSaha(null, 1, 5);

            // Assert
            result.Should().NotBeNull();
            result.PageCount.Should().Be(1);
            var vm = Assert.IsType<VmOpenApiOrganizationSahaGuidPage>(result);
            vm.ItemList.Should().NotBeNullOrEmpty();
            vm.ItemList.Count.Should().Be(publishedAndDeletedList.Count);
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.Published.ToString()).FirstOrDefault().Should().NotBeNull();
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.OldPublished.ToString()).FirstOrDefault().Should().NotBeNull();
            vm.ItemList.Where(i => i.PublishingStatus == PublishingStatus.Deleted.ToString()).FirstOrDefault().Should().NotBeNull();
            var vmSahaOrg = vm.ItemList.Where(i => i.Id == mainOrgId).FirstOrDefault();
            vmSahaOrg.Should().NotBeNull();
            vmSahaOrg.SahaId.Should().Be(sahaId.ToString());
        }
    }
}
