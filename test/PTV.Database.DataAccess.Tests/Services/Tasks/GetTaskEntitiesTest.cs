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

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Tasks;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Notifications;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Tasks
{
    public class GetTaskEntitiesTest : TranslatorTestBase
    {
        [Theory]
        [InlineData(UserRoleEnum.Eeva)]
        [InlineData(UserRoleEnum.Pete)]
        [InlineData(UserRoleEnum.Shirley)]
        public void GetFailedTimedPublishEntities(UserRoleEnum userRole)
        {
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(
                CreateServiceLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            RegisterRepository<IServiceCollectionLanguageAvailabilityRepository, ServiceCollectionLanguageAvailability>(
                CreateServiceCollectionLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(
                CreateChannelLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            RegisterRepository<IOrganizationLanguageAvailabilityRepository, OrganizationLanguageAvailability>(
                CreateOrganizationLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            if (userRole == UserRoleEnum.Eeva)
            {
                RegisterRepository<IGeneralDescriptionLanguageAvailabilityRepository,
                        GeneralDescriptionLanguageAvailability>
                    (CreateGeneralDescriptionLanguageAvailabilities().AsQueryable());
            }
            
            var translationManagerToVmMock = SetupTranslators();

            var utilitiesMock = new Mock<IServiceUtilities>();
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(userRole);
            
            var tasksService = new TasksService(
                translationManagerToVmMock.Object,
                null,
                null,
                null,
                contextManagerMock.Object,
                null,
                null,
                null,
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                CacheManager,
                null);

            var model = new VmTasksSearch
            {
                TaskType = TasksIdsEnum.TimedPublishFailed
            };

            var result = tasksService.GetTasksEntities(model, new List<Guid> { organization1Id, organization2Id } );
            var expectedCount = GetExpectedEntityTypesForRole(userRole).Count() * 2;
            
            Assert.Equal(expectedCount, result.Count);
        }
        [Fact]
        public void GetFailedTimedPublishEntitiesForGivenOrganizationIdsShouldEqual()
        {
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(
                CreateServiceLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            RegisterRepository<IServiceCollectionLanguageAvailabilityRepository, ServiceCollectionLanguageAvailability>(
                CreateServiceCollectionLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(
                CreateChannelLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            RegisterRepository<IOrganizationLanguageAvailabilityRepository, OrganizationLanguageAvailability>(
                CreateOrganizationLanguageAvailabilities(organization1Id, organization2Id).AsQueryable());
            RegisterRepository<IGeneralDescriptionLanguageAvailabilityRepository,
                        GeneralDescriptionLanguageAvailability>
                    (CreateGeneralDescriptionLanguageAvailabilities().AsQueryable());
            
            var translationManagerToVmMock = SetupTranslators();

            var utilitiesMock = new Mock<IServiceUtilities>();
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(UserRoleEnum.Eeva);
            
            var tasksService = new TasksService(
                translationManagerToVmMock.Object,
                null,
                null,
                null,
                contextManagerMock.Object,
                null,
                null,
                null,
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                CacheManager,
                null);

            var model = new VmTasksSearch
            {
                TaskType = TasksIdsEnum.TimedPublishFailed
            };

            var result = tasksService.GetTasksEntities(model, new List<Guid> { organization1Id } );
            var expectedCount = GetExpectedEntityTypesForRole(UserRoleEnum.Eeva).Count() +1; // General description is not limited for organization
            
            Assert.Equal(expectedCount, result.Count);
        }
        
        [Fact]
        public void GetFailedTimedPublishEntitiesForGivenOrganizationIdsShouldFilterOutEntitiesWithoutFailedPublishDate()
        {
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(
                CreateServiceLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IServiceCollectionLanguageAvailabilityRepository, ServiceCollectionLanguageAvailability>(
                CreateServiceCollectionLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(
                CreateChannelLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IOrganizationLanguageAvailabilityRepository, OrganizationLanguageAvailability>(
                CreateOrganizationLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IGeneralDescriptionLanguageAvailabilityRepository,
                        GeneralDescriptionLanguageAvailability>
                    (CreateGeneralDescriptionLanguageAvailabilities(true).AsQueryable());
            
            var translationManagerToVmMock = SetupTranslators();

            var utilitiesMock = new Mock<IServiceUtilities>();
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(UserRoleEnum.Eeva);
            
            var tasksService = new TasksService(
                translationManagerToVmMock.Object,
                null,
                null,
                null,
                contextManagerMock.Object,
                null,
                null,
                null,
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                CacheManager,
                null);

            var model = new VmTasksSearch
            {
                TaskType = TasksIdsEnum.TimedPublishFailed
            };

            var result = tasksService.GetTasksEntities(model, new List<Guid> { organization1Id, organization2Id } );
            var expectedCount = GetExpectedEntityTypesForRole(UserRoleEnum.Eeva).Count();
            
            Assert.Equal(expectedCount, result.Count);
        }
        
        [Fact]
        public void GetFailedTimedPublishEntitiesForGivenOrganizationIdsShouldFBeEmpty()
        {
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(
                CreateServiceLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IServiceCollectionLanguageAvailabilityRepository, ServiceCollectionLanguageAvailability>(
                CreateServiceCollectionLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(
                CreateChannelLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IOrganizationLanguageAvailabilityRepository, OrganizationLanguageAvailability>(
                CreateOrganizationLanguageAvailabilities(organization1Id, organization2Id, true).AsQueryable());
            RegisterRepository<IGeneralDescriptionLanguageAvailabilityRepository,
                        GeneralDescriptionLanguageAvailability>
                    (CreateGeneralDescriptionLanguageAvailabilities(true).AsQueryable());
            
            var translationManagerToVmMock = SetupTranslators();

            var utilitiesMock = new Mock<IServiceUtilities>();
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(UserRoleEnum.Eeva);
            
            var tasksService = new TasksService(
                translationManagerToVmMock.Object,
                null,
                null,
                null,
                contextManagerMock.Object,
                null,
                null,
                null,
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                CacheManager,
                null);

            var model = new VmTasksSearch
            {
                TaskType = TasksIdsEnum.TimedPublishFailed
            };

            var result = tasksService.GetTasksEntities(model, new List<Guid> { organization1Id } );

            result.Count.Should().Be(1); // one should be returned for GD, others should be filtered out, because no published failed date set for given organization
        }
        
        private Mock<ITranslationEntity> SetupTranslators()
        {
            RegisterServiceMock<ILanguageCache>();
            RegisterServiceMock<IVersioningManager>();
            RegisterServiceMock<ITextManager>();
            RegisterServiceMock<ModelUtility>();
            RegisterServiceMock<ICacheManager>();
            
            var translationManagerToVmMock = new Mock<ITranslationEntity>(MockBehavior.Strict);
            translationManagerToVmMock
                .Setup(x => x.TranslateAll<ServiceLanguageAvailability, VmTimedPublishFailedTask>(
                    It.IsAny<IEnumerable<ServiceLanguageAvailability>>()))
                .Returns<IEnumerable<ServiceLanguageAvailability>>(entities =>
                    new List<VmTimedPublishFailedTask>(
                        entities.Select(x => new VmTimedPublishFailedTask()
                        { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>()
                            { new VmLanguageAvailabilityInfo() }
                        }))
                );
            
            translationManagerToVmMock
                .Setup(x => x.TranslateAll<ServiceChannelLanguageAvailability, VmTimedPublishFailedTask>(
                    It.IsAny<IEnumerable<ServiceChannelLanguageAvailability>>()))
                .Returns<IEnumerable<ServiceChannelLanguageAvailability>>(entities =>
                    new List<VmTimedPublishFailedTask>(entities.Select(x => new VmTimedPublishFailedTask() { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>() { new VmLanguageAvailabilityInfo() }})));
            
            translationManagerToVmMock
                .Setup(x => x.TranslateAll<ServiceCollectionLanguageAvailability, VmTimedPublishFailedTask>(
                    It.IsAny<IEnumerable<ServiceCollectionLanguageAvailability>>()))
                .Returns<IEnumerable<ServiceCollectionLanguageAvailability>>(entities =>
                    new List<VmTimedPublishFailedTask>(entities.Select(x => new VmTimedPublishFailedTask() { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>() { new VmLanguageAvailabilityInfo()}})));
            
            translationManagerToVmMock
                .Setup(x => x.TranslateAll<OrganizationLanguageAvailability, VmTimedPublishFailedTask>(
                    It.IsAny<IEnumerable<OrganizationLanguageAvailability>>()))
                .Returns<IEnumerable<OrganizationLanguageAvailability>>(entities =>
                    new List<VmTimedPublishFailedTask>(entities.Select(x=> new VmTimedPublishFailedTask() { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>() { new VmLanguageAvailabilityInfo()}})));
            
            translationManagerToVmMock
                .Setup(x => x.TranslateAll<GeneralDescriptionLanguageAvailability, VmTimedPublishFailedTask>(
                    It.IsAny<IEnumerable<GeneralDescriptionLanguageAvailability>>()))
                .Returns<IEnumerable<GeneralDescriptionLanguageAvailability>>(entities =>
                    entities.Any() ? new List<VmTimedPublishFailedTask>(entities.Select(x => new VmTimedPublishFailedTask() { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>() { new VmLanguageAvailabilityInfo()}})) : new List<VmTimedPublishFailedTask>());

            return translationManagerToVmMock;
        }

        private IEnumerable<EntityTypeEnum> GetExpectedEntityTypesForRole(UserRoleEnum userRole)
        {
            var values = Enum.GetValues(typeof(EntityTypeEnum));

            foreach (var value in values)
            {
                var enumValue = (EntityTypeEnum) value;

                // Only Eeva can display failed timed publish GDs 
                if (userRole != UserRoleEnum.Eeva && enumValue == EntityTypeEnum.GeneralDescription)
                {
                    continue;
                }

                yield return enumValue;
            }
        }

        private IEnumerable<T> CreateFailedLanguageAvailabilities<T>(bool isPublishDateEmpty = false)
            where T : ILanguageAvailability, new()
        {
            yield return new T
            {
                LanguageId = "fi".GetGuid(),
                LastFailedPublishAt = isPublishDateEmpty ? (DateTime?)null : new DateTime(2017, 4, 4)
            };
            
            yield return new T
            {
                LanguageId = "sw".GetGuid(),
                LastFailedPublishAt = new DateTime(2017, 5, 5)
            };
        }

        private IEnumerable<ServiceLanguageAvailability> CreateServiceLanguageAvailabilities(Guid org1, Guid org2, bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceLanguageAvailability>(isPublishDateEmpty))
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                
                languageAvailability.ServiceVersioned = new ServiceVersioned
                {
                    OrganizationId = usedOrganizationId
                };
                languageAvailability.ServiceVersionedId = Guid.NewGuid();
                yield return languageAvailability;
            }
        }

        private IEnumerable<ServiceCollectionLanguageAvailability> CreateServiceCollectionLanguageAvailabilities(Guid org1, Guid org2, bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceCollectionLanguageAvailability>(isPublishDateEmpty))
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.ServiceCollectionVersioned = new ServiceCollectionVersioned
                {
                    OrganizationId = usedOrganizationId
                };
                languageAvailability.ServiceCollectionVersionedId = Guid.NewGuid();
                yield return languageAvailability;
            }
        }

        private IEnumerable<ServiceChannelLanguageAvailability> CreateChannelLanguageAvailabilities(Guid org1, Guid org2, bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceChannelLanguageAvailability>(isPublishDateEmpty))
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.ServiceChannelVersioned = new ServiceChannelVersioned
                {
                    OrganizationId = usedOrganizationId
                };
                languageAvailability.ServiceChannelVersioned.Type = new ServiceChannelType
                {
                    Code = ((ServiceChannelTypeEnum) (languageAvailability.LastFailedPublishAt?.Day % 5 ?? 0))
                        .ToString()
                };
                languageAvailability.ServiceChannelVersionedId = Guid.NewGuid();
                yield return languageAvailability;
            }
        }

        private IEnumerable<OrganizationLanguageAvailability> CreateOrganizationLanguageAvailabilities(Guid org1, Guid org2, bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<OrganizationLanguageAvailability>(isPublishDateEmpty))
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.OrganizationVersioned = new OrganizationVersioned
                {
                    UnificRootId = usedOrganizationId
                };
                languageAvailability.OrganizationVersionedId = Guid.NewGuid();
                yield return languageAvailability;
            }
        }

        private IEnumerable<GeneralDescriptionLanguageAvailability> CreateGeneralDescriptionLanguageAvailabilities(bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<GeneralDescriptionLanguageAvailability>(isPublishDateEmpty))
            {
                languageAvailability.StatutoryServiceGeneralDescriptionVersioned = new StatutoryServiceGeneralDescriptionVersioned();
                languageAvailability.StatutoryServiceGeneralDescriptionVersionedId = Guid.NewGuid();
                yield return languageAvailability;
            }
        }
        
        private readonly Guid organization1Id = "Organization1Id".GetGuid();
        private readonly Guid organization2Id = "Organization2Id".GetGuid();
    }
}