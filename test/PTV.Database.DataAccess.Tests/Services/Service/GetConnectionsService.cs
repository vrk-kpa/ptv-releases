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
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Services.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Xunit;
using ServiceService = PTV.Database.DataAccess.Services.V2.ServiceService;

namespace PTV.Database.DataAccess.Tests.Services.Service
{
    public class GetConnectionsService : TestBase
    {
        [Fact]
        public void ConnectionsInCorrectOrder()
        {
            SetupContextManager<ServiceServiceChannel, VmConnectionsServiceSearchResult>();
            var services = CreateServices().ToList();
            var serviceNames = CreateServiceNames().ToList();
            var connections = CreateConnections();

            SetupRepositories(services, serviceNames);
            var utilitiesMock = SetupUtilities();
            var commonServiceMock = SetupCommonService();
            var searchServiceMock = SetupSearchService();
            SetupLanguageCache();
            var connectionsServiceMock = SetupConnectionsService(connections);
            var translationManagerToVmMock = SetupTranslators();
            var expirationServiceMock = new Mock<IExpirationService>();

            var serviceService = new ServiceService(
                contextManagerMock.Object,
                translationManagerToVmMock.Object,
                null,
                utilitiesMock.Object,
                commonServiceMock.Object,
                null,
                connectionsServiceMock.Object,
                null,
                null,
                CacheManager,
                PublishingStatusCache,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                searchServiceMock.Object,
                null,
                expirationServiceMock.Object,
                null);

            var search = new VmConnectionsServiceSearch
            {
                Fulltext = null,
                SelectedPublishingStatuses = new List<Guid> {PublishingStatusCache.Get(PublishingStatus.Published)}
            };

            var result = serviceService.GetConnectionsService(search);
            var resultConnections = result.SearchResult[0].Connections;

            var expectedOrderedGuids = new List<string>
            {
                (EChannelName + 1).GetGuid().ToString() + (ServiceName + 1).GetGuid().ToString(),
                (EChannelName + 2).GetGuid().ToString() + (ServiceName + 1).GetGuid().ToString(),
                (LocationName + 1).GetGuid().ToString() + (ServiceName + 1).GetGuid().ToString(),
                (LocationName + 2).GetGuid().ToString() + (ServiceName + 1).GetGuid().ToString(),
                (PhoneName + 1).GetGuid().ToString() + (ServiceName + 1).GetGuid().ToString(),
                (PhoneName + 2).GetGuid().ToString() + (ServiceName + 1).GetGuid().ToString()
            };

            for (var i = 0; i < resultConnections.Count; i++)
            {
                Assert.Equal(expectedOrderedGuids[i], resultConnections[i].ConnectionId);
            }
        }

        private Mock<ITranslationEntity> SetupTranslators()
        {
            RegisterServiceMock<ILanguageCache>();
            RegisterServiceMock<IVersioningManager>(mock => mock
                .Setup(x => x.ApplyPublishingStatusFilterFallback(It.IsAny<IEnumerable<ServiceChannelVersioned>>()))
                .Returns<IEnumerable<ServiceChannelVersioned>>(scv => scv.FirstOrDefault()));
            RegisterServiceMock<ITextManager>();
            RegisterServiceMock<ModelUtility>();
            RegisterServiceMock<ICacheManager>();

//            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmConnectableService>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmConnectionBasicInformation>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmConnectionDigitalAuthorizationOutput>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmAstiDetails>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmContactDetails>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmOpeningHours>>();

            var servieConnectionOutputTranslator = new ServiceConnectionOutputTranslator(
                ResolveManager, new TranslationPrimitives(ResolveManager), CacheManager);
            var translationManagerToVmMock = new Mock<ITranslationEntity>(MockBehavior.Strict);

            translationManagerToVmMock
                .Setup(x => x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                    It.IsAny<IEnumerable<ServiceLanguageAvailability>>()))
                .Returns(new List<VmLanguageAvailabilityInfo>());

            translationManagerToVmMock
                .Setup(x =>
                    x.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(
                        It.IsAny<ICollection<ServiceServiceChannel>>()))
                .Returns<IEnumerable<ServiceServiceChannel>>(entities => entities
                    .Select(entity => servieConnectionOutputTranslator.TranslateEntityToVm(entity))
                    .ToList());

            return translationManagerToVmMock;
        }

        private static Mock<IConnectionsServiceInternal> SetupConnectionsService(List<ServiceServiceChannel> connections)
        {
            var connectionsDictionary =
                connections.GroupBy(x => x.ServiceId).ToDictionary(x => x.Key, x => x.ToList());
            var connectionsServiceMock = new Mock<IConnectionsServiceInternal>();
            connectionsServiceMock
                .Setup(x => x.GetAllServiceRelations(It.IsAny<IUnitOfWork>(), It.IsAny<List<Guid>>()))
                .Returns(connectionsDictionary);
            return connectionsServiceMock;
        }

        private List<ServiceServiceChannel> CreateConnections()
        {
            var result = new List<ServiceServiceChannel>
            {
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 1,
                    ServiceOrderNumber = 8,
                    ServiceId = (ServiceName + 1).GetGuid(),
                    ServiceChannelId = (LocationName + 1).GetGuid(),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 2,
                    ServiceOrderNumber = 9,
                    ServiceId = (ServiceName + 1).GetGuid(),
                    ServiceChannelId = (PhoneName + 1).GetGuid(),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 3,
                    ServiceOrderNumber = 7,
                    ServiceId = (ServiceName + 1).GetGuid(),
                    ServiceChannelId = (EChannelName + 1).GetGuid(),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 4,
                    ServiceOrderNumber = 9,
                    ServiceId = (ServiceName + 1).GetGuid(),
                    ServiceChannelId = (PhoneName + 2).GetGuid(),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 5,
                    ServiceOrderNumber = 7,
                    ServiceId = (ServiceName + 1).GetGuid(),
                    ServiceChannelId = (EChannelName + 2).GetGuid(),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 6,
                    ServiceOrderNumber = 8,
                    ServiceId = (ServiceName + 1).GetGuid(),
                    ServiceChannelId = (LocationName + 2).GetGuid(),
                    IsASTIConnection = false
                },
            };

            AddChannelInfo(result);
            return result;
        }

        private void AddChannelInfo(List<ServiceServiceChannel> connections)
        {
            foreach (var connection in connections)
            {
                connection.ServiceChannel = new ServiceChannel
                {
                    Versions = new List<ServiceChannelVersioned>
                    {
                        new ServiceChannelVersioned
                        {
                            Id = connection.ServiceChannelId,
                            UnificRootId = connection.ServiceChannelId,
                            ServiceChannelNames = new List<ServiceChannelName>(),
                            Type = new ServiceChannelType
                            {
                                // This has nothing to do with the business logic, it is just a way to ensure the test
                                // case has some logic in it. Basically, it ensures
                                OrderNumber = connection.ServiceOrderNumber
                            }
                        }
                    }
                };
            }
        }

        private void SetupLanguageCache()
        {
            var languageCacheMock = new Mock<ILanguageCache>();
            languageCacheMock.Setup(x => x.GetByValue(It.IsAny<Guid>())).Returns("fi");
            CacheManagerMock.Setup(x => x.LanguageCache).Returns(languageCacheMock.Object);
        }

        private IEnumerable<ServiceName> CreateServiceNames()
        {
            for (var i = 1; i <= 3; i++)
            {
                var name = ServiceName + i;
                yield return new ServiceName
                {
                    Name = name,
                    ServiceVersionedId = name.GetGuid(),
                    Localization = new Language()
                };
            }
        }

        private static Mock<ICommonServiceInternal> SetupCommonService()
        {
            var commonServiceMock = new Mock<ICommonServiceInternal>();
            commonServiceMock
                .Setup(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()))
                .Returns(new List<VmListItem>());
            return commonServiceMock;
        }

        private static Mock<ISearchServiceInternal> SetupSearchService()
        {
            var searchServiceMock = new Mock<ISearchServiceInternal>();
            searchServiceMock
                .Setup(x => x.SearchEntities(It.IsAny<VmEntitySearch>()))
                .Returns(new VmSearchResult<IVmEntityListItem>
                {
                    SearchResult = new List<IVmEntityListItem>
                    {
                        new VmEntityListItem
                        {
                            UnificRootId = Guid.NewGuid()
                        }
                    }
                });
            return searchServiceMock;
        }

        private static Mock<IServiceUtilities> SetupUtilities()
        {
            var utilitiesMock = new Mock<IServiceUtilities>();
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(UserRoleEnum.Eeva);
            return utilitiesMock;
        }

        private void SetupRepositories(List<ServiceVersioned> services, List<ServiceName> serviceNames)
        {
            ConnectServiceNames(services, serviceNames);

            RegisterRepository<IServiceVersionedRepository, ServiceVersioned>(services.AsQueryable());
            RegisterRepository<IServiceNameRepository, ServiceName>(serviceNames.AsQueryable());
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(services.SelectMany(x=>x.LanguageAvailabilities).AsQueryable());
            RegisterRepository<IStatutoryServiceGeneralDescriptionVersionedRepository, StatutoryServiceGeneralDescriptionVersioned>(services.SelectMany(x=>x.StatutoryServiceGeneralDescription.Versions).AsQueryable());
        }

        private void ConnectServiceNames(List<ServiceVersioned> services, List<ServiceName> serviceNames)
        {
            foreach (var service in services)
            {
                var serviceName = serviceNames.FirstOrDefault(chn => chn.ServiceVersionedId == service.Id);
                service.ServiceNames = new List<ServiceName> {serviceName};
                serviceName.ServiceVersioned = service;
            }
        }

        private IEnumerable<ServiceVersioned> CreateServices()
        {
            for (var i = 1; i <= 3; i++)
            {
                var id = (ServiceName + i).GetGuid();
                yield return new ServiceVersioned
                {
                    Id = id,
                    UnificRootId = id,
                    PublishingStatusId = PublishingStatusCache.Get(PublishingStatus.Published),
                    StatutoryServiceGeneralDescription = new StatutoryServiceGeneralDescription
                    {
                        StatutoryServiceGeneralDescriptionServiceChannels = new List<GeneralDescriptionServiceChannel>()
                    }
                };
            }
        }

        private const string EChannelName = "echannel";
        private const string LocationName = "location";
        private const string PhoneName = "phone";
        private const string ServiceName = "service";
    }
}
