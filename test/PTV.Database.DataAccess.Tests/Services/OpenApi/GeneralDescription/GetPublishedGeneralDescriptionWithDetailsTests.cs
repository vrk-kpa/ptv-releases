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
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using PTV.Database.DataAccess.Interfaces.DbContext;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.GeneralDescription
{
    public class GetPublishedGeneralDescriptionWithDetailsTests : GeneralDescriptionServiceTestBase
    {
        private IUnitOfWorkWritable _unitOfWork;
        private GeneralDescriptionService _service;

        public GetPublishedGeneralDescriptionWithDetailsTests()
        {
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<StatutoryServiceGeneralDescriptionVersioned>>(),
                It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<StatutoryServiceGeneralDescriptionVersioned> gds, Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>> func, bool applyFilters) =>
                {
                    return gds;
                });

            translationManagerMockSetup.Setup(t => t.Translate<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<StatutoryServiceGeneralDescriptionVersioned>()))
                .Returns((StatutoryServiceGeneralDescriptionVersioned entity) =>
                {
                    var vm = new VmOpenApiGeneralDescriptionVersionBase
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };

                    if (entity.Names?.Count > 0)
                    {
                        if (vm.Names == null)
                        {
                            vm.Names = new List<VmOpenApiLocalizedListItem>();
                        }
                        entity.Names.ToList().ForEach(n =>
                        {
                            vm.Names.Add(new VmOpenApiLocalizedListItem { Value = n.Name, Language = LanguageCache.GetByValue(n.LocalizationId) });
                        });
                    }
                    if (entity.Descriptions?.Count > 0)
                    {
                        if (vm.Descriptions == null)
                        {
                            vm.Descriptions = new List<VmOpenApiLocalizedListItem>();
                        }
                        entity.Descriptions.ToList().ForEach(n =>
                        {
                            vm.Descriptions.Add(new VmOpenApiLocalizedListItem { Value = n.Description, Language = LanguageCache.GetByValue(n.LocalizationId) });
                        });
                    }
                    return vm;
                });

            _unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(_unitOfWork, _unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new GeneralDescriptionService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, ValidationManagerMock, LanguageOrderCache, ResolveManager, PahaTokenProcessor, null, null, null);
        }

        [Fact]
        public void RightVersionNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(_unitOfWork, id, It.IsAny<PublishingStatus?>(), It.IsAny<bool>())).Returns((Guid?)null);

            // Act
            var result = _service.GetPublishedGeneralDescriptionWithDetails(_unitOfWork, id);

            // Assert
            result.Should().BeNull();
            VersioningManagerMock.Verify(x => x.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(_unitOfWork, id, PublishingStatus.Published, true), Times.Once());
        }

        [Fact]
        public void OnlyPublishedPropertiesReturned()
        {
            // Arrange
            var rootId = Guid.NewGuid();
            var versionId = Guid.NewGuid();
            var fiText = "Finnish text";
            var svText = "Swedish text";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            var nameTypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var descriptionTypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
            var gd = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId, versionId);

            // Set the data for the lists
            // Add only swedish language as published version
            gd.Names = new List<StatutoryServiceName> {
                new StatutoryServiceName { Name = fiText, LocalizationId = fiId, TypeId = nameTypeId },
                new StatutoryServiceName { Name = svText, LocalizationId = svId, TypeId = nameTypeId }
            };
            gd.Descriptions = new List<StatutoryServiceDescription> {
                new StatutoryServiceDescription { Description = fiText, LocalizationId = fiId, TypeId = descriptionTypeId },
                new StatutoryServiceDescription { Description = svText, LocalizationId = svId, TypeId = descriptionTypeId }
            };
            gd.LanguageAvailabilities.Add(new GeneralDescriptionLanguageAvailability { StatusId = PublishedId, LanguageId = svId });
            gd.LanguageAvailabilities.Add(new GeneralDescriptionLanguageAvailability { StatusId = PublishingStatusCache.Get(PublishingStatus.Modified), LanguageId = fiId });

            VersioningManagerMock.Setup(s => s.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(_unitOfWork, rootId, PublishingStatus.Published, true)).Returns(versionId);

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(new List<StatutoryServiceGeneralDescriptionVersioned> { gd }.AsQueryable());

            // Act
            var result = _service.GetPublishedGeneralDescriptionWithDetails(_unitOfWork, rootId);

            result.Should().NotBeNull();
            var vmResult = Assert.IsType<VmOpenApiGeneralDescriptionVersionBase>(result);
        }
    }
}
