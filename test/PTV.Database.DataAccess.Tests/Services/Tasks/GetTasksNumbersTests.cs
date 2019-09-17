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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Moq;
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
        private readonly Guid _outdatedDraftServiceType1 = "OutdatedDraftService1".GetGuid();
        private readonly Guid _outdatedDraftServiceType2 = "OutdatedDraftService2".GetGuid();
        private readonly Guid _outdatedDraftServiceType3 = "OutdatedDraftService3".GetGuid();
        private readonly Guid _outdatedPublishedServiceType1 = "OutdatedPublishedService1".GetGuid();
        private readonly Guid _outdatedPublishedServiceType2 = "OutdatedPublishedService2".GetGuid();
        private readonly Guid _outdatedPublishedServiceType3 = "OutdatedPublishedService3".GetGuid();
        private readonly Guid _outdatedDraftServiceChannelType1 = "OutdatedDraftServiceChannel1".GetGuid();
        private readonly Guid _outdatedDraftServiceChannelType2 = "OutdatedDraftServiceChannel2".GetGuid();
        private readonly Guid _outdatedDraftServiceChannelType3 = "OutdatedDraftServiceChannel3".GetGuid();
        private readonly Guid _outdatedPublishedServiceChannelType1 = "OutdatedPublishedServiceChannel1".GetGuid();
        private readonly Guid _outdatedPublishedServiceChannelType2 = "OutdatedPublishedServiceChannel2".GetGuid();
        private readonly Guid _outdatedPublishedServiceChannelType3 = "OutdatedPublishedServiceChannel3".GetGuid();
        private readonly Guid _languageId1 = "LanguageId1".GetGuid();
        private readonly Guid _languageId2 = "LanguageId2".GetGuid();
        private readonly Guid _configurationId1 = "ConfigurationId1".GetGuid();
        private readonly Guid _configurationId2 = "ConfigurationId2".GetGuid();
        private readonly Guid _configurationId3 = "ConfigurationId3".GetGuid();
        private readonly Guid _configurationId4 = "ConfigurationId4".GetGuid();
        private readonly Guid _organization1Id = "Organization1Id".GetGuid();
        private readonly Guid _organization2Id = "Organization2Id".GetGuid();
        private readonly Guid _publishingStatusPublished = "PublishingStatusPublished".GetGuid();
        private readonly DateTime _utcNow = DateTime.UtcNow;
        private Mock<IServiceUtilities> utilitiesMock;
        private Mock<ICommonServiceInternal> commonService;
        private Mock<VTasksConfiguration> taskConfiguration;
        private Mock<IPahaTokenProcessor> pahaTokenProcessor;
        private Mock<IVersioningManager> versioningManager;
        private Mock<IOrganizationServiceInternal> organizationService;
        public GetTasksNumbersTests()
        {
            utilitiesMock = new Mock<IServiceUtilities>(MockBehavior.Strict);
            commonService = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            taskConfiguration = new Mock<VTasksConfiguration>(MockBehavior.Strict);
            pahaTokenProcessor = new Mock<IPahaTokenProcessor>(MockBehavior.Strict);
            versioningManager = new Mock<IVersioningManager>(MockBehavior.Strict);
            organizationService = new Mock<IOrganizationServiceInternal>(MockBehavior.Strict);
            taskService = new TasksService(null, null, CacheManager.PublishingStatusCache, null,
                contextManagerMock.Object, null, null, commonService.Object, organizationService.Object,
                utilitiesMock.Object, versioningManager.Object, null, pahaTokenProcessor.Object, null, CacheManager, null);
        }
        
        private List<TrackingTranslationOrder> _trackingTranslationOrder => new List<TrackingTranslationOrder>
        {
            new TrackingTranslationOrder()
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

        private List<TasksFilter> _tasksFilter => new List<TasksFilter>
        {
            new TasksFilter()
            {
                EntityId = _serviceId,
                UserId = "testUser".GetGuid(),
                TypeId = _outdatedDraftServiceType2
            },
            new TasksFilter()
            {
                EntityId = _serviceId2,
                UserId = "testUser".GetGuid(),
                TypeId = _outdatedPublishedServiceType2
            },
            new TasksFilter()
            {
                EntityId = _channelId,
                UserId = "testUser".GetGuid(),
                TypeId = _outdatedDraftServiceChannelType2
            },
            new TasksFilter()
            {
                EntityId = _channelId2,
                UserId = "testUser".GetGuid(),
                TypeId = _outdatedPublishedServiceChannelType2
            }
        };
        private List<VTasksConfiguration> TaskConfigurations => new List<VTasksConfiguration>
        {
            new VTasksConfiguration
            {
                
            }
        };

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
                new HelpTaskConfiguration() { Id = _outdatedPublishedServiceType2, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-6)},
                new HelpTaskConfiguration() { Id = _outdatedPublishedServiceType3, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-12)},
                new HelpTaskConfiguration() { Id = _outdatedPublishedServiceType1, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-3)},
                new HelpTaskConfiguration() { Id = _outdatedDraftServiceType3, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddMonths(-3)},
                new HelpTaskConfiguration() { Id = _outdatedDraftServiceType2, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddMonths(-1)},
                new HelpTaskConfiguration() { Id = _outdatedDraftServiceType1, Entity = "Service", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddDays(-15)},
                new HelpTaskConfiguration() { Id = _outdatedPublishedServiceChannelType2, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-6)},
                new HelpTaskConfiguration() { Id = _outdatedPublishedServiceChannelType3, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-12)},
                new HelpTaskConfiguration() { Id = _outdatedPublishedServiceChannelType1, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Published), Interval = _utcNow.AddMonths(-3)},
                new HelpTaskConfiguration() { Id = _outdatedDraftServiceChannelType3, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddMonths(-3)},
                new HelpTaskConfiguration() { Id = _outdatedDraftServiceChannelType2, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddMonths(-1)},
                new HelpTaskConfiguration() { Id = _outdatedDraftServiceChannelType1, Entity = "ServiceChannel", Code = "xxx", PublishingStatusId = CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft), Interval = _utcNow.AddDays(-15)}
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
                
            utilitiesMock.Setup(x => x.GetAllUserOrganizations()).Returns(() => new List<Guid>() {_organization1Id});
            utilitiesMock.Setup(x => x.GetUserMainOrganization()).Returns(() => _organization1Id);
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(userHighestRole);
            commonService.Setup(x => x.ExtendPublishingStatusesByEquivalents(It.IsAny<List<Guid>>()));
            pahaTokenProcessor.Setup(x => x.UserName).Returns(() => "testUser");
            versioningManager
                .Setup(x => x.GetLastVersion<OrganizationVersioned>(It.IsAny<IUnitOfWork>(),
                    It.IsAny<Guid>())).Returns(() => new VersionInfo() {EntityId = _organization2Id});
            organizationService.Setup(x => x.GetOrganizationMissingLanguages(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>()))
                .Returns(() => _organizationsWithMissingLanguages.AsQueryable());
        }
        
        [Fact]
        public void GetNumbersAllTypesShouldReturnAnyNumbers()
        {
            SetMocsForTests();
            var result = taskService.GetTasksNumbers(new List<Guid>() { _organization1Id, _organization2Id});
            
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
                                                       .First(y => y.Id == _outdatedDraftServiceType1).Interval) +
                        ServiceVersions
                            .Count(x => x.PublishingStatusId ==
                                        CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                        x.Modified < taskConfig.First(y => y.Id == _outdatedPublishedServiceType1)
                                            .Interval) +
                        ServiceChannelsVersions
                            .Count(x => x.PublishingStatusId ==
                                        CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft) &&
                                        x.Modified < taskConfig.First(y => y.Id == _outdatedDraftServiceChannelType1)
                                            .Interval) +
                        ServiceChannelsVersions
                            .Count(x => x.PublishingStatusId ==
                                        CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                        x.Modified < taskConfig
                                            .First(y => y.Id == _outdatedPublishedServiceChannelType1).Interval) +
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
            SetMocsForTests();
            RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid>(){ _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceVersions.Count(x => x.PublishingStatusId ==
                                                   CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft) &&
                                                   x.Modified < taskConfig
                                                       .First(y => y.Id == _outdatedDraftServiceType1).Interval &&
                                                   !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftServices).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftServices).Count.Should().Be(count);
        }
        
        [Fact]
        public void GetNumbersOfAllOutdatedPublishedServicesWithoutPostponedOnes()
        {
            SetMocsForTests();
            RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid>(){ _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceVersions.Count(x => x.PublishingStatusId ==
                                                   CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                                   x.Modified < taskConfig
                                                       .First(y => y.Id == _outdatedPublishedServiceType1).Interval &&
                                                   !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedServices).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedServices).Count.Should().Be(count);
        }
        
        [Fact]
        public void GetNumbersOfAllOutdatedDraftServiceChannelsWithoutPostponedOnes()
        {
            SetMocsForTests();
            RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid>() { _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceChannelsVersions.Count(x => x.PublishingStatusId ==
                                                   CacheManager.PublishingStatusCache.Get(PublishingStatus.Draft) &&
                                                   x.Modified < taskConfig
                                                       .First(y => y.Id == _outdatedDraftServiceChannelType1).Interval &&
                                                   !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftChannels).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedDraftChannels).Count.Should().Be(count);
        }
        
        [Fact]
        public void GetNumbersOfAllOutdatedPublishedServiceChannelsWithoutPostponedOnes()
        {
            SetMocsForTests();
            RegisterRepository<ITasksFilterRepository, TasksFilter>(_tasksFilter.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid>(){ _organization1Id, _organization2Id });
            var taskConfig = GetTaskConfigurationView();
            var tasksFilterIds = _tasksFilter.Select(x => x.EntityId);
            var count = ServiceChannelsVersions.Count(x => x.PublishingStatusId ==
                                                           CacheManager.PublishingStatusCache.Get(PublishingStatus.Published) &&
                                                           x.Modified < taskConfig
                                                               .First(y => y.Id == _outdatedPublishedServiceChannelType1).Interval &&
                                                           !tasksFilterIds.Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedChannels).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.OutdatedPublishedChannels).Count.Should().Be(count);
        }

        [Fact]
        public void GetNumbersOfFailedTimedPublishEntities()
        {
            SetMocsForTests();

            var expectedCount = Enum.GetValues(typeof(EntityTypeEnum)).Length * 2;
            var result = taskService.GetTasksNumbers(new List<Guid> {_organization1Id, _organization2Id});
            var failedTimedPublishCount = result.First(x => x.Id == TasksIdsEnum.TimedPublishFailed);
            
            Assert.Equal(expectedCount, failedTimedPublishCount.Count);
        }

        private List<ServiceServiceChannel> _connections => new List<ServiceServiceChannel>
        {
            new ServiceServiceChannel()
            {
                ServiceId = _serviceId
            },
            new ServiceServiceChannel()
            {
                ServiceId = _serviceId2
            },
            new ServiceServiceChannel()
            {
                ServiceChannelId = _channelId
            },
            new ServiceServiceChannel()
            {
                ServiceChannelId = _channelId2
            }
        };
        
        [Fact]
        public void GetNumbersOfServiceOrphans()
        {
            SetMocsForTests();
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(
                _connections.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid>(){ _organization1Id, _organization2Id });
            var count = ServiceVersions.Count(x => !_connections.Select(y => y.ServiceId).Contains(x.UnificRootId));
            result.Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.ServicesWithoutChannels).Should().NotBeNull();
            result.First(x => x.Id == TasksIdsEnum.ServicesWithoutChannels).Count.Should().Be(count);
        }
        
        [Fact]
        public void GetNumbersOfServiceChannelsOrphans()
        {
            SetMocsForTests();
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(
                _connections.AsQueryable());
            var result = taskService.GetTasksNumbers(new List<Guid>(){ _organization1Id, _organization2Id });
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
