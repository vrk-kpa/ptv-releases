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
using PTV.Database.DataAccess.Interfaces.Services;
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

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.GeneralDescription
{
    public class GetGeneralDescriptionsSimpleTests : GeneralDescriptionServiceTestBase
    {
        private IGeneralDescriptionService _service;

        public GetGeneralDescriptionsSimpleTests()
        {
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            // unitOfWork
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<StatutoryServiceGeneralDescriptionVersioned>>(),
                It.IsAny<Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<StatutoryServiceGeneralDescriptionVersioned> gds, Func<IQueryable<StatutoryServiceGeneralDescriptionVersioned>, IQueryable<StatutoryServiceGeneralDescriptionVersioned>> func, bool applyFilters) =>
                {
                    return gds;
                });

            translationManagerMockSetup.Setup(t => t.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<List<StatutoryServiceGeneralDescriptionVersioned>>()))
                .Returns((List<StatutoryServiceGeneralDescriptionVersioned> entities) =>
                {
                    var vmList = new List<VmOpenApiGeneralDescriptionVersionBase>();
                    entities.ForEach(entity =>
                    {
                        var item = new VmOpenApiGeneralDescriptionVersionBase()
                        {
                            Id = entity.UnificRootId,
                            PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                        };
                        if (entity.Names?.Count > 0)
                        {
                            if (item.Names == null)
                            {
                                item.Names = new List<VmOpenApiLocalizedListItem>();
                            }
                            entity.Names.ToList().ForEach(n =>
                            {
                                item.Names.Add(new VmOpenApiLocalizedListItem { Value = n.Name, Language = LanguageCache.GetByValue(n.LocalizationId) });
                            });
                        }
                        if (entity.Descriptions?.Count > 0)
                        {
                            if (item.Descriptions == null)
                            {
                                item.Descriptions = new List<VmOpenApiLocalizedListItem>();
                            }
                            entity.Descriptions.ToList().ForEach(n =>
                            {
                                item.Descriptions.Add(new VmOpenApiLocalizedListItem { Value = n.Description, Language = LanguageCache.GetByValue(n.LocalizationId) });
                            });
                        }
                        vmList.Add(item);
                    });
                    return vmList;
                });

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManager);
            _service = new GeneralDescriptionService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, TranslationService, PublishingStatusCache, UserOrganizationChecker, LanguageCache, TypeCache,
                VersioningManager, DataUtils, ValidationManagerMock, LanguageOrderCache, RestrictionFilterManager, PahaTokenProcessor, null);
        }

        [Fact]
        public void IdListIsNull()
        {
            // Act
            var result = _service.GetGeneralDescriptionsSimple(null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IdListIsEmpty()
        {
            // Act
            var result = _service.GetGeneralDescriptionsSimple(new List<Guid>());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void NoPublishedGDsFound()
        {
            // Arrange
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();

            var list = new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId1),
                EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Modified), rootId2)
            };

            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = _service.GetGeneralDescriptionsSimple(new List<Guid> { rootId1, rootId2 });

            // Assert
            result.Count.Should().Be(0);
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Never);
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetGeneralDescriptionEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<GeneralDescriptionLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = _service.GetGeneralDescriptionsSimple(new List<Guid> { publishedChannel.UnificRootId });

            // Assert
            result.Count.Should().Be(0);
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Never);
        }

        [Fact]
        public void OnlyPublishedPropertiesReturned()
        {
            // Arrange  
            var rootId1 = Guid.NewGuid();
            var rootId2 = Guid.NewGuid();
            var fiText = "Finnish text";
            var svText = "Swedish text";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            var nameTypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var descriptionTypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
            var gd1 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId1);
            var gd2 = EntityGenerator.CreateEntity<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published), rootId2);

            // Set the data for the lists
            // Add only swedish language as published version
            gd1.Names = new List<StatutoryServiceName> {
                new StatutoryServiceName { Name = fiText, LocalizationId = fiId, TypeId = nameTypeId },
                new StatutoryServiceName { Name = svText, LocalizationId = svId, TypeId = nameTypeId }
            };
            gd1.Descriptions = new List<StatutoryServiceDescription> {
                new StatutoryServiceDescription { Description = fiText, LocalizationId = fiId, TypeId = descriptionTypeId },
                new StatutoryServiceDescription { Description = svText, LocalizationId = svId, TypeId = descriptionTypeId }
            };
            gd1.LanguageAvailabilities.Add(new GeneralDescriptionLanguageAvailability { StatusId = PublishedId, LanguageId = svId });
            gd1.LanguageAvailabilities.Add(new GeneralDescriptionLanguageAvailability { StatusId = PublishingStatusCache.Get(PublishingStatus.Modified), LanguageId = fiId });

            var list = new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                gd1, gd2
            };

            // repositories
            GdRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            
            // Act
            var result = _service.GetGeneralDescriptionsSimple(new List<Guid> { rootId1, rootId2 });

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            GdRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()), Times.Once);
            var gdResult = result.Where(i => i.Id == rootId1).FirstOrDefault();
            gdResult.Should().NotBeNull();
            gdResult.Names.Count.Should().Be(1);
            var name = gdResult.Names.First();
            name.Value.Should().Be(svText);
            gdResult.Descriptions.Count.Should().Be(1);
            var description = gdResult.Descriptions.First();
            description.Value.Should().Be(svText);
        }
    }
}
