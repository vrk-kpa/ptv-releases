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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Views;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Tasks
{
    public class GetTasksNumbersTests : TestBase
    {
        private TasksService taskService;
        // private readonly Mock<ITranslationEntity> translateToVmMock;
        private readonly Guid _channelId = "ChannelId".GetGuid();
        private readonly Guid _serviceId = "ServiceId".GetGuid();
        private readonly Guid _channelId2 = "ChannelId2".GetGuid();
        private readonly Guid _serviceId2 = "ServiceId2".GetGuid();
        private readonly Guid _serviceDraftFirstWarning = "OutdatedDraftService1".GetGuid();
        private readonly Guid _serviceDraftLastWarning = "OutdatedDraftService2".GetGuid();
        private readonly Guid _serviceDraftExpiration = "OutdatedDraftService3".GetGuid();
        private readonly Guid _servicePublishedFirstWarning = "OutdatedPublishedService1".GetGuid();
        private readonly Guid _servicePublishedLastWarning = "OutdatedPublishedService2".GetGuid();
        private readonly Guid _servicePublishedExpiration = "OutdatedPublishedService3".GetGuid();
        private readonly Guid _channelDraftFirstWarning = "OutdatedDraftServiceChannel1".GetGuid();
        private readonly Guid _channelDraftLastWarning = "OutdatedDraftServiceChannel2".GetGuid();
        private readonly Guid _channelDraftExpiration = "OutdatedDraftServiceChannel3".GetGuid();
        private readonly Guid _channelPublishedFirstWarning = "OutdatedPublishedServiceChannel1".GetGuid();
        private readonly Guid _channelPublishedLastWarning = "OutdatedPublishedServiceChannel2".GetGuid();
        private readonly Guid _channelPublishedExpiration = "OutdatedPublishedServiceChannel3".GetGuid();
        private readonly Guid _organization1Id = "Organization1Id".GetGuid();
        private readonly Guid _organization2Id = "Organization2Id".GetGuid();
        private readonly DateTime _utcNow = DateTime.UtcNow;
        private Mock<IServiceUtilities> utilitiesMock;
        private Mock<ICommonServiceInternal> commonService;
        private Mock<IPahaTokenAccessor> pahaTokenProcessor;
        private Mock<IVersioningManager> versioningManager;
        private Mock<IOrganizationServiceInternal> organizationService;
        private Mock<IBrokenLinkService> brokenLinkService;
        public GetTasksNumbersTests()
        {
            utilitiesMock = new Mock<IServiceUtilities>(MockBehavior.Strict);
            commonService = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            pahaTokenProcessor = new Mock<IPahaTokenAccessor>(MockBehavior.Strict);
            versioningManager = new Mock<IVersioningManager>(MockBehavior.Strict);
            organizationService = new Mock<IOrganizationServiceInternal>(MockBehavior.Strict);
            brokenLinkService = new Mock<IBrokenLinkService>(MockBehavior.Strict);
            SetMocsForTests();

            var expirationTimeCache = new ExpirationTimeCache(contextManagerMock.Object);

            var expirationService = new ExpirationService(
                null,
                null,
                CacheManager.PublishingStatusCache,
                null,
                versioningManager.Object,
                commonService.Object,
                pahaTokenProcessor.Object,
                CacheManager,
                expirationTimeCache
            );

            taskService = new TasksService(null, null, CacheManager.PublishingStatusCache, null,
                contextManagerMock.Object, null, null, commonService.Object, organizationService.Object,
                utilitiesMock.Object, versioningManager.Object, null, null,
                brokenLinkService.Object, expirationService, expirationTimeCache);
        }

        private List<TrackingTranslationOrder> _trackingTranslationOrder => new List<TrackingTranslationOrder>
        {
            new TrackingTranslationOrder
            {
                OrganizationId = _organization1Id,
                Created = _utcNow
            }
        };

        private IEnumerable<T> CreateFailedLanguageAvailabilities<T>()
            where T : ILanguageAvailability, new()
        {
            yield return new T
            {
                LanguageId = "fi".GetGuid(),
                LastFailedPublishAt = new DateTime(2017, 4, 4)
            };

            yield return new T
            {
                LanguageId = "sw".GetGuid(),
                LastFailedPublishAt = new DateTime(2017, 5, 5)
            };
        }

        private IEnumerable<ServiceLanguageAvailability> CreateServiceLanguageAvailabilities(Guid org1, Guid org2)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceLanguageAvailability>())
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.ServiceVersioned = new ServiceVersioned
                {
                    OrganizationId = usedOrganizationId
                };

                yield return languageAvailability;
            }
        }

        private IEnumerable<ServiceCollectionLanguageAvailability> CreateServiceCollectionLanguageAvailabilities(Guid org1, Guid org2)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceCollectionLanguageAvailability>())
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.ServiceCollectionVersioned = new ServiceCollectionVersioned
                {
                    OrganizationId = usedOrganizationId
                };

                yield return languageAvailability;
            }
        }

        private IEnumerable<ServiceChannelLanguageAvailability> CreateChannelLanguageAvailabilities(Guid org1, Guid org2)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<ServiceChannelLanguageAvailability>())
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.ServiceChannelVersioned = new ServiceChannelVersioned
                {
                    OrganizationId = usedOrganizationId
                };

                yield return languageAvailability;
            }
        }

        private IEnumerable<OrganizationLanguageAvailability> CreateOrganizationLanguageAvailabilities(Guid org1, Guid org2)
        {
            foreach (var languageAvailability in CreateFailedLanguageAvailabilities<OrganizationLanguageAvailability>())
            {
                var usedOrganizationId = languageAvailability.LastFailedPublishAt?.Day % 2 == 0 ? org1 : org2;
                languageAvailability.OrganizationVersioned = new OrganizationVersioned
                {
                    UnificRootId = usedOrganizationId
                };

                yield return languageAvailability;
            }
        }

        private List<ServiceVersioned> ServiceVersions => new List<ServiceVersioned>
        {
            new ServiceVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddDays(-10)
            },
            new ServiceVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddDays(-16)
            },
            new ServiceVersioned
            {
                UnificRootId = _serviceId,
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddMonths(-2)
            },
            new ServiceVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddDays(-60)
            },
            new ServiceVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddMonths(-5)
            },
            new ServiceVersioned
            {
                UnificRootId = _serviceId2,
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddMonths(-8)
            }
        };

        private List<ServiceChannelVersioned> ServiceChannelsVersions => new List<ServiceChannelVersioned>
        {
            new ServiceChannelVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddDays(-10)
            },
            new ServiceChannelVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddDays(-16)
            },
            new ServiceChannelVersioned
            {
                UnificRootId = _channelId,
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddMonths(-2)
            },
            new ServiceChannelVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddDays(-60)
            },
            new ServiceChannelVersioned
            {
                UnificRootId = Guid.NewGuid(),
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddMonths(-5)
            },
            new ServiceChannelVersioned
            {
                UnificRootId = _channelId2,
                PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published),
                OrganizationId = _organization1Id,
                Modified = _utcNow.AddMonths(-8)
            }
        };

        private List<Guid> _organizationsWithMissingLanguages => new List<Guid> {_organization1Id};

        // Postpone functionality has been disabled
        // private List<TasksFilter> _tasksFilter => new List<TasksFilter>
        // {
        //     new TasksFilter
        //     {
        //         EntityId = _serviceId,
        //         UserId = "testUser".GetGuid(),
        //         TypeId = _serviceDraftLastWarning
        //     },
        //     new TasksFilter
        //     {
        //         EntityId = _serviceId2,
        //         UserId = "testUser".GetGuid(),
        //         TypeId = _servicePublishedLastWarning
        //     },
        //     new TasksFilter
        //     {
        //         EntityId = _channelId,
        //         UserId = "testUser".GetGuid(),
        //         TypeId = _channelDraftLastWarning
        //     },
        //     new TasksFilter
        //     {
        //         EntityId = _channelId2,
        //         UserId = "testUser".GetGuid(),
        //         TypeId = _channelPublishedLastWarning
        //     }
        // };

        class HelpTaskConfiguration
        {
            public Guid Id
            {
                get;
                set;
            }

            public string Entity
            {
                get;
                set;
            }

            public string Code
            {
                get;
                set;
            }

            public Guid PublishingStatusId
            {
                get;
                set;
            }

            public DateTime Interval
            {
                get;
                set;
            }
        }

        private List<VTasksConfiguration> GetTaskConfigurationView()
        {
            var helpConfs = new List<HelpTaskConfiguration>
            {
                new HelpTaskConfiguration { Id = _servicePublishedLastWarning, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-10)},
                new HelpTaskConfiguration { Id = _servicePublishedExpiration, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-12)},
                new HelpTaskConfiguration { Id = _servicePublishedFirstWarning, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-9)},
                new HelpTaskConfiguration { Id = _serviceDraftExpiration, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddMonths(-3)},
                new HelpTaskConfiguration { Id = _serviceDraftLastWarning, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddDays(-45)},
                new HelpTaskConfiguration { Id = _serviceDraftFirstWarning, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow},
                new HelpTaskConfiguration { Id = _channelPublishedLastWarning, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-10)},
                new HelpTaskConfiguration { Id = _channelPublishedExpiration, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-12)},
                new HelpTaskConfiguration { Id = _channelPublishedFirstWarning, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-9)},
                new HelpTaskConfiguration { Id = _channelDraftExpiration, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddMonths(-3)},
                new HelpTaskConfiguration { Id = _channelDraftLastWarning, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddDays(-45)},
                new HelpTaskConfiguration { Id = _channelDraftFirstWarning, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow}
            };
            var result = new List<VTasksConfiguration>();

            foreach (var helpTaskConfiguration in helpConfs)
            {
                var t = new VTasksConfiguration();
                t.SetProperty(x => x.Id, helpTaskConfiguration.Id);
                t.SetProperty(x => x.Entity, helpTaskConfiguration.Entity);
                t.SetProperty(x => x.PublishingStatusId, helpTaskConfiguration.PublishingStatusId);
                t.SetProperty(x => x.Interval, helpTaskConfiguration.Interval);
                result.Add(t);
            }

            return result;
        }

        private void SetMocsForTests(UserRoleEnum userHighestRole = UserRoleEnum.Eeva)
        {
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(ServiceVersions.AsQueryable());
            RegisterRepository<IRepository<ServiceChannelVersioned>, ServiceChannelVersioned>(ServiceChannelsVersions.AsQueryable());
            RegisterRepository<ITasksFilterRepository, TasksFilter>(new List<TasksFilter>().AsQueryable());
            RegisterViewRepository<IVTasksConfigurationRepository, VTasksConfiguration>(GetTaskConfigurationView().AsQueryable());
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(
                new List<ServiceServiceChannel>().AsQueryable());
            RegisterRepository<ITrackingTranslationOrderRepository, TrackingTranslationOrder>(_trackingTranslationOrder
                .AsQueryable());

            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(
                CreateServiceLanguageAvailabilities(_organization1Id, _organization2Id).AsQueryable());
            RegisterRepository<IServiceCollectionLanguageAvailabilityRepository, ServiceCollectionLanguageAvailability>(
                CreateServiceCollectionLanguageAvailabilities(_organization1Id, _organization2Id).AsQueryable());
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(
                CreateChannelLanguageAvailabilities(_organization1Id, _organization2Id).AsQueryable());
            RegisterRepository<IOrganizationLanguageAvailabilityRepository, OrganizationLanguageAvailability>(
                CreateOrganizationLanguageAvailabilities(_organization1Id, _organization2Id).AsQueryable());
            RegisterRepository<IGeneralDescriptionLanguageAvailabilityRepository, GeneralDescriptionLanguageAvailability>
                ( CreateFailedLanguageAvailabilities<GeneralDescriptionLanguageAvailability>().AsQueryable());

            utilitiesMock.Setup(x => x.GetAllUserOrganizations()).Returns(() => new List<Guid> {_organization1Id});
            utilitiesMock.Setup(x => x.GetUserMainOrganization()).Returns(() => _organization1Id);
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(userHighestRole);
            commonService.Setup(x => x.ExtendPublishingStatusesByEquivalents(It.IsAny<List<Guid>>()));
            pahaTokenProcessor.Setup(x => x.UserName).Returns(() => "testUser");
            versioningManager
                .Setup(x => x.GetLastVersion<OrganizationVersioned>(It.IsAny<IUnitOfWork>(),
                    It.IsAny<Guid>())).Returns(() => new VersionInfo {EntityId = _organization2Id});
            organizationService.Setup(x => x.GetOrganizationMissingLanguages(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>()))
                .Returns(() => _organizationsWithMissingLanguages.AsQueryable());
            brokenLinkService.Setup(x =>
                    x.GetUnstableLinksCount(It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), It.IsAny<IUnitOfWork>()))
                .Returns(0);
        }

        [Fact]
        public void GetNumbersAllTypesShouldReturnAnyNumbers()
        {
            var result = taskService.GetTasksNumbers(new List<Guid> { _organization1Id, _organization2Id});

            /* sum should be following
             * outdated draft service should skip newer services, that should not be counted towards configuration
             * outdated published service should skip newer services, that should not be counted towards configuration
             * outdated draft service channel should skip newer channels, that should not be counted towards configuration
             * outdated published service channel should skip newer channels, that should not be counted towards configuration
             * orphan services: all given services
             * orphan service channels: all given service channels
             * all organizations with missing languages
             * all arrived translations
             */
            var taskConfig = GetTaskConfigurationView();
            var count = ServiceVersions.Count(x => x.PublishingStatusId ==
                                                   CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft) &&
                                                   x.Modified < taskConfig
                                                       .First(y => y.Id == _serviceDraftFirstWarning).Interval) +
                        ServiceVersions
                            .Count(x => x.PublishingStatusId ==
                                        CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                        x.Modified < taskConfig.First(y => y.Id == _servicePublishedFirstWarning)
                                            .Interval) +
                        ServiceChannelsVersions
                            .Count(x => x.PublishingStatusId ==
                                        CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft) &&
                                        x.Modified < taskConfig.First(y => y.Id == _channelDraftFirstWarning)
                                            .Interval) +
                        ServiceChannelsVersions
                            .Count(x => x.PublishingStatusId ==
                                        CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                        x.Modified < taskConfig
                                            .First(y => y.Id == _channelPublishedFirstWarning).Interval) +
                        ServiceVersions.Count +
                        ServiceChannelsVersions.Count +
                        _organizationsWithMissingLanguages.Count +
                        _trackingTranslationOrder.Count +
                        Enum.GetValues(typeof(EntityTypeEnum)).Length * 2;

            result.Should().NotBeNull();
            result.Sum(x => x.Count).Should().Be(count);
        }

        [Fact]
        public void GetNumbersOfAllOutdatedDraftServicesWithoutPostponedOnes()
        {
            // RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid> { _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            // var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceVersions.Count(x => x.PublishingStatusId ==
                                                   CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft) &&
                                                   x.Modified < taskConfig
                                                       .First(y => y.Id == _serviceDraftFirstWarning).Interval);
                                                   // !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftServices).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftServices).Count.Should().Be(count);
        }

        [Fact]
        public void GetNumbersOfAllOutdatedPublishedServicesWithoutPostponedOnes()
        {
            // RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid> { _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            // var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceVersions.Count(x => x.PublishingStatusId ==
                                                   CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                                   x.Modified < taskConfig
                                                       .First(y => y.Id == _servicePublishedFirstWarning).Interval);
                                                   // !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedServices).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedServices).Count.Should().Be(count);
        }

        [Fact]
        public void GetNumbersOfAllOutdatedDraftServiceChannelsWithoutPostponedOnes()
        {
            // RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid> { _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            // var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceChannelsVersions.Count(x => x.PublishingStatusId ==
                                                   CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft) &&
                                                   x.Modified < taskConfig
                                                       .First(y => y.Id == _channelDraftFirstWarning).Interval);
                                                   // !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftChannels).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftChannels).Count.Should().Be(count);
        }

        [Fact]
        public void GetNumbersOfAllOutdatedPublishedServiceChannelsWithoutPostponedOnes()
        {
            // RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid> { _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            // var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceChannelsVersions.Count(x => x.PublishingStatusId ==
                                                           CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                                           x.Modified < taskConfig
                                                               .First(y => y.Id == _channelPublishedFirstWarning).Interval);
                                                           // !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedChannels).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedChannels).Count.Should().Be(count);
        }

        [Fact]
        public void GetNumbersOfFailedTimedPublishEntities()
        {
            var expectedCount = Enum.GetValues(typeof(EntityTypeEnum)).Length * 2;
            var result = taskService.GetTasksNumbers(new List<Guid> {_organization1Id, _organization2Id});
            var failedTimedPublishCount = result.First(x => x.Id == TasksIdsEnum.TimedPublishFailed);

            Assert.Equal(expectedCount, failedTimedPublishCount.Count);
        }

        private List<ServiceServiceChannel> _connections => new List<ServiceServiceChannel>
        {
            new ServiceServiceChannel
            {
                ServiceId = _serviceId
            },
            new ServiceServiceChannel
            {
                ServiceId = _serviceId2
            },
            new ServiceServiceChannel
            {
                ServiceChannelId = _channelId
            },
            new ServiceServiceChannel
            {
                ServiceChannelId = _channelId2
            }
        };

        [Fact]
        public void GetNumbersOfServiceOrphans()
        {
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(
                _connections.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid> { _organization1Id, _organization2Id });
            var count = ServiceVersions.Count(x => !_connections.Select(y => y.ServiceId).Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.ServicesWithoutChannels).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.ServicesWithoutChannels).Count.Should().Be(count);
        }

        [Fact]
        public void GetNumbersOfServiceChannelsOrphans()
        {
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(
                _connections.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid> { _organization1Id, _organization2Id });
            var count = ServiceChannelsVersions.Count(x => !_connections.Select(y => y.ServiceChannelId).Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.ServicesWithoutChannels).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.ServicesWithoutChannels).Count.Should().Be(count);
        }
    }

}

public static class TestExtensions
{
    public static void SetProperty<TSource, TProperty>(
        this TSource source,
        Expression<Func<TSource, TProperty>> prop,
        TProperty value)
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)prop.Body).Member;
        propertyInfo.SetValue(source, value);
    }
}
