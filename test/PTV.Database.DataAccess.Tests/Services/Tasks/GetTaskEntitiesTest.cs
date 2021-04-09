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
            RegisterRepositories(organization1Id, organization2Id, false);

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
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                CacheManager);

            var model = new VmTasksSearch
            {
                TaskType = TasksIdsEnum.TimedPublishFailed
            };

            var result = tasksService.GetTasksEntities(model, new List<Guid> { organization1Id, organization2Id } );
            var expectedCount = GetExpectedEntityTypesForRole(userRole).Count() * 2;

            Assert.Equal(expectedCount, result.Count);
        }

        private void RegisterRepositories(Guid org1, Guid org2, bool isPublishDateEmpty)
        {
            var services = CreateServiceLanguageAvailabilities(org1, org2, isPublishDateEmpty).ToList();
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(services.AsQueryable());
            RegisterRepository<IServiceVersionedRepository, ServiceVersioned>(services.Select(x => x.ServiceVersioned).AsQueryable());

            var collections = CreateServiceCollectionLanguageAvailabilities(org1, org2, isPublishDateEmpty).ToList();
            RegisterRepository<IServiceCollectionLanguageAvailabilityRepository, ServiceCollectionLanguageAvailability>(collections.AsQueryable());
            RegisterRepository<IServiceCollectionVersionedRepository, ServiceCollectionVersioned>(collections.Select(x => x.ServiceCollectionVersioned).AsQueryable());

            var channels = CreateChannelLanguageAvailabilities(org1, org2, isPublishDateEmpty).ToList();
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(channels.AsQueryable());
            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(channels.Select(x => x.ServiceChannelVersioned).AsQueryable());

            var organizations = CreateOrganizationLanguageAvailabilities(org1, org2, isPublishDateEmpty).ToList();
            RegisterRepository<IOrganizationLanguageAvailabilityRepository, OrganizationLanguageAvailability>(organizations.AsQueryable());
            RegisterRepository<IOrganizationVersionedRepository, OrganizationVersioned>(organizations.Select(x => x.OrganizationVersioned).AsQueryable());

            var gds = CreateGeneralDescriptionLanguageAvailabilities(isPublishDateEmpty).ToList();
            RegisterRepository<IGeneralDescriptionLanguageAvailabilityRepository, GeneralDescriptionLanguageAvailability>(gds.AsQueryable());
            RegisterRepository<IStatutoryServiceGeneralDescriptionVersionedRepository,
                StatutoryServiceGeneralDescriptionVersioned>(gds
                .Select(x => x.StatutoryServiceGeneralDescriptionVersioned).AsQueryable());
        }

        [Fact]
        public void GetFailedTimedPublishEntitiesForGivenOrganizationIdsShouldEqual()
        {
            RegisterRepositories(organization1Id, organization2Id, false);

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
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                 CacheManager);

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
            RegisterRepositories(organization1Id, organization2Id, true);

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
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                 CacheManager);

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
            RegisterRepositories(organization1Id, organization2Id, true);

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
                null,
                utilitiesMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                 CacheManager);

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
                .Setup(x => x.TranslateAll<ServiceVersioned, VmTimedPublishFailedTask>(
                    It.IsAny<ICollection<ServiceVersioned>>()))
                .Returns<IEnumerable<ServiceVersioned>>(entities =>
                    new List<VmTimedPublishFailedTask>(
                        entities.Select(x => new VmTimedPublishFailedTask
                        { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo> { new VmLanguageAvailabilityInfo() }
                        }))
                );

            translationManagerToVmMock
                .Setup(x => x.TranslateAll<ServiceChannelVersioned, VmTimedPublishFailedTask>(
                    It.IsAny<ICollection<ServiceChannelVersioned>>()))
                .Returns<IEnumerable<ServiceChannelVersioned>>(entities =>
                    new List<VmTimedPublishFailedTask>(entities.Select(x => new VmTimedPublishFailedTask { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo> { new VmLanguageAvailabilityInfo() }})));

            translationManagerToVmMock
                .Setup(x => x.TranslateAll<ServiceCollectionVersioned, VmTimedPublishFailedTask>(
                    It.IsAny<ICollection<ServiceCollectionVersioned>>()))
                .Returns<IEnumerable<ServiceCollectionVersioned>>(entities =>
                    new List<VmTimedPublishFailedTask>(entities.Select(x => new VmTimedPublishFailedTask { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo> { new VmLanguageAvailabilityInfo()}})));

            translationManagerToVmMock
                .Setup(x => x.TranslateAll<OrganizationVersioned, VmTimedPublishFailedTask>(
                    It.IsAny<ICollection<OrganizationVersioned>>()))
                .Returns<IEnumerable<OrganizationVersioned>>(entities =>
                    new List<VmTimedPublishFailedTask>(entities.Select(x=> new VmTimedPublishFailedTask { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo> { new VmLanguageAvailabilityInfo()}})));

            translationManagerToVmMock
                .Setup(x => x.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmTimedPublishFailedTask>(
                    It.IsAny<ICollection<StatutoryServiceGeneralDescriptionVersioned>>()))
                .Returns<IEnumerable<StatutoryServiceGeneralDescriptionVersioned>>(entities =>
                    entities.Any() ? new List<VmTimedPublishFailedTask>(entities.Select(x => new VmTimedPublishFailedTask { LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo> { new VmLanguageAvailabilityInfo()}})) : new List<VmTimedPublishFailedTask>());

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
                var versionedId = Guid.NewGuid();
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;

                languageAvailability.ServiceVersioned = new ServiceVersioned
                {
                    OrganizationId = usedOrganizationId,
                    Id = versionedId,
                    LanguageAvailabilities = new List<ServiceLanguageAvailability> { languageAvailability }
                };
                languageAvailability.ServiceVersionedId = versionedId;
                yield return languageAvailability;
            }
        }

        private IEnumerable<ServiceCollectionLanguageAvailability> CreateServiceCollectionLanguageAvailabilities(Guid org1, Guid org2, bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceCollectionLanguageAvailability>(isPublishDateEmpty))
            {
                var versionedId = Guid.NewGuid();
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.ServiceCollectionVersioned = new ServiceCollectionVersioned
                {
                    OrganizationId = usedOrganizationId,
                    Id = versionedId,
                    LanguageAvailabilities = new List<ServiceCollectionLanguageAvailability> { languageAvailability }
                };
                languageAvailability.ServiceCollectionVersionedId = versionedId;
                yield return languageAvailability;
            }
        }

        private IEnumerable<ServiceChannelLanguageAvailability> CreateChannelLanguageAvailabilities(Guid org1, Guid org2, bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceChannelLanguageAvailability>(isPublishDateEmpty))
            {
                var versionedId = Guid.NewGuid();
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.ServiceChannelVersioned = new ServiceChannelVersioned
                {
                    OrganizationId = usedOrganizationId,
                    Id = versionedId,
                    LanguageAvailabilities = new List<ServiceChannelLanguageAvailability> { languageAvailability }
                };
                languageAvailability.ServiceChannelVersioned.Type = new ServiceChannelType
                {
                    Code = ((ServiceChannelTypeEnum) (languageAvailability.LastFailedPublishAt?.Day % 5 ?? 0))
                        .ToString()
                };
                languageAvailability.ServiceChannelVersionedId = versionedId;
                yield return languageAvailability;
            }
        }

        private IEnumerable<OrganizationLanguageAvailability> CreateOrganizationLanguageAvailabilities(Guid org1, Guid org2, bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<OrganizationLanguageAvailability>(isPublishDateEmpty))
            {
                var versionedId = Guid.NewGuid();
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.OrganizationVersioned = new OrganizationVersioned
                {
                    UnificRootId = usedOrganizationId,
                    Id = versionedId,
                    LanguageAvailabilities = new List<OrganizationLanguageAvailability> { languageAvailability }
                };
                languageAvailability.OrganizationVersionedId = versionedId;
                yield return languageAvailability;
            }
        }

        private IEnumerable<GeneralDescriptionLanguageAvailability> CreateGeneralDescriptionLanguageAvailabilities(bool isPublishDateEmpty = false)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<GeneralDescriptionLanguageAvailability>(isPublishDateEmpty))
            {
                var versionedId = Guid.NewGuid();
                languageAvailability.StatutoryServiceGeneralDescriptionVersioned = new StatutoryServiceGeneralDescriptionVersioned
                {
                    Id = versionedId,
                    LanguageAvailabilities = new List<GeneralDescriptionLanguageAvailability> { languageAvailability }
                };
                languageAvailability.StatutoryServiceGeneralDescriptionVersionedId = versionedId;
                yield return languageAvailability;
            }
        }

        private readonly Guid organization1Id = "Organization1Id".GetGuid();
        private readonly Guid organization2Id = "Organization2Id".GetGuid();
    }
}
