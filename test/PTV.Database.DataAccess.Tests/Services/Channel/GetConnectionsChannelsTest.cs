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
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using Xunit;
using ChannelService = PTV.Database.DataAccess.Services.V2.ChannelService;

namespace PTV.Database.DataAccess.Tests.Services.Channel
{
    public class GetConnectionsChannelsTest : TestBase
    {
        [Fact]
        public void ConnectionsInCorrectOrder()
        {
            SetupContextManager<ServiceServiceChannel, VmConnectionsChannelSearchResult>();
            var channels = CreateChannels().ToList();
            var channelNames = CreateChannelNames().ToList();
            var connections = CreateConnections();

            SetupRepositories(channels, channelNames, connections);
            var translationManagerToVmMock = SetupTranslators();
            var utilitiesMock = SetupUtilities();
            var connectionsServiceMock = SetupConnectionsService(connections);
            var commonServiceMock = SetupCommonService();
            SetupLanguageCache();

            var channelService = new ChannelService(
                contextManagerMock.Object,
                translationManagerToVmMock.Object,
                null,
                utilitiesMock.Object,
                commonServiceMock.Object,
                CacheManagerMock.Object,
                PublishingStatusCache,
                null,
                null,
                null,
                null,
                connectionsServiceMock.Object,
                null,
                null,
                null,
                null,
                null,
                null);

            var search = new VmConnectionsChannelSearch
            {
                Fulltext = null,
                ChannelIds = new List<Guid>{ GetEntityGuid(DomainEnum.Channels, 1) }
            };

            var result = channelService.GetConnectionsChannels(search);
            var expectedFirstGuid = GetEntityGuid(DomainEnum.Services, 1).ToString()
                + GetEntityGuid(DomainEnum.Channels, 1).ToString();
            var expectedLastGuid = GetEntityGuid(DomainEnum.Services, 3).ToString()
                + GetEntityGuid(DomainEnum.Channels, 1).ToString();

            Assert.Equal(expectedFirstGuid, result.SearchResult[0].Connections[0].ConnectionId);
            Assert.Equal(expectedLastGuid, result.SearchResult[0].Connections[2].ConnectionId);
        }

        private void SetupLanguageCache()
        {
            var languageCacheMock = new Mock<ILanguageCache>();
            languageCacheMock.Setup(x => x.GetByValue(It.IsAny<Guid>())).Returns("fi");
            CacheManagerMock.Setup(x => x.LanguageCache).Returns(languageCacheMock.Object);
        }

        private static Mock<ICommonServiceInternal> SetupCommonService()
        {
            var commonServiceMock = new Mock<ICommonServiceInternal>();
            commonServiceMock
                .Setup(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()))
                .Returns(new List<VmListItem>());
            return commonServiceMock;
        }

        private static Mock<IConnectionsServiceInternal> SetupConnectionsService(List<ServiceServiceChannel> connections)
        {
            var connectionsDictionary =
                connections.GroupBy(x => x.ServiceChannelId).ToDictionary(x => x.Key, x => x.ToList());
            var connectionsServiceMock = new Mock<IConnectionsServiceInternal>();
            connectionsServiceMock
                .Setup(x => x.GetAllServiceChannelRelations(It.IsAny<IUnitOfWork>(), It.IsAny<List<Guid>>()))
                .Returns(connectionsDictionary);
            return connectionsServiceMock;
        }

        private static Mock<IServiceUtilities> SetupUtilities()
        {
            var utilitiesMock = new Mock<IServiceUtilities>();
            utilitiesMock.Setup(x => x.UserHighestRole()).Returns(UserRoleEnum.Eeva);
            return utilitiesMock;
        }

        private Mock<ITranslationEntity> SetupTranslators()
        {
            RegisterServiceMock<ILanguageCache>();
            RegisterServiceMock<IVersioningManager>();
            RegisterServiceMock<ITextManager>();
            RegisterServiceMock<ModelUtility>();
            RegisterServiceMock<ICacheManager>();

            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmConnectableService>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmConnectionBasicInformation>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmConnectionDigitalAuthorizationOutput>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmAstiDetails>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmContactDetails>>();
            RegisterServiceMock<ITranslator<ServiceServiceChannel, VmOpeningHours>>();

            var channelConnectionOutputTranslator =
                new ChannelConnectionOutputTranslator(ResolveManager, new TranslationPrimitives(ResolveManager));
            var translationManagerToVmMock = new Mock<ITranslationEntity>(MockBehavior.Strict);
            translationManagerToVmMock
                .Setup(x => x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                    It.IsAny<IEnumerable<ServiceChannelLanguageAvailability>>()))
                .Returns(new List<VmLanguageAvailabilityInfo>());
            translationManagerToVmMock
                .Setup(x =>
                    x.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                        It.IsAny<IEnumerable<ServiceServiceChannel>>()))
                .Returns<IEnumerable<ServiceServiceChannel>>(entities => entities
                    .Select(entity => channelConnectionOutputTranslator.TranslateEntityToVm(entity))
                    .ToList());
            return translationManagerToVmMock;
        }

        private void SetupRepositories(List<ServiceChannelVersioned> channels, List<ServiceChannelName> channelNames, List<ServiceServiceChannel> connections)
        {
            ConnectChannelNames(channels, channelNames);

            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(channels.AsQueryable());
            RegisterRepository<IServiceChannelNameRepository, ServiceChannelName>(channelNames.AsQueryable());
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(
                new List<ServiceChannelLanguageAvailability>().AsQueryable());
            RegisterRepository<IRepository<ServiceServiceChannel>, ServiceServiceChannel>(connections.AsQueryable());
        }

        private void ConnectChannelNames(List<ServiceChannelVersioned> channels, List<ServiceChannelName> channelNames)
        {
            foreach (var channel in channels)
            {
                var channelName = channelNames.FirstOrDefault(chn => chn.ServiceChannelVersionedId == channel.Id);
                channel.ServiceChannelNames = new List<ServiceChannelName> {channelName};
                channelName.ServiceChannelVersioned = channel;
            }
        }

        private List<ServiceServiceChannel> CreateConnections()
        {
            return new List<ServiceServiceChannel>
            {
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 4,
                    ServiceOrderNumber = 7,
                    ServiceId = GetEntityGuid(DomainEnum.Services, 1),
                    ServiceChannelId = GetEntityGuid(DomainEnum.Channels, 1),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 5,
                    ServiceOrderNumber = 8,
                    ServiceId = GetEntityGuid(DomainEnum.Services, 2),
                    ServiceChannelId = GetEntityGuid(DomainEnum.Channels, 1),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 6,
                    ServiceOrderNumber = 9,
                    ServiceId = GetEntityGuid(DomainEnum.Services, 3),
                    ServiceChannelId = GetEntityGuid(DomainEnum.Channels, 1),
                    IsASTIConnection = false
                }
            };
        }

        private IEnumerable<ServiceChannelName> CreateChannelNames()
        {
            for (var i = 1; i <= 3; i++)
            {
                var name = ChannelName + i;
                yield return new ServiceChannelName
                {
                    Name = name,
                    ServiceChannelVersionedId = name.GetGuid(),
                    Localization = new Language(),
                };
            }
        }

        private IEnumerable<ServiceChannelVersioned> CreateChannels()
        {
            for (var i = 1; i <= 3; i++)
            {
                var id = GetEntityGuid(DomainEnum.Channels, i);
                yield return new ServiceChannelVersioned
                {
                    Id = id,
                    UnificRootId = id,
                    PublishingStatusId = PublishingStatusCache.Get(PublishingStatus.Published)
                };
            }
        }

        private Guid GetEntityGuid(DomainEnum entityType, int index)
        {
            var prefix = entityType == DomainEnum.Services ? ServiceName : ChannelName;
            return (prefix + index).GetGuid();
        }

        private const string ServiceName = "service";
        private const string ChannelName = "channel";
    }
}
