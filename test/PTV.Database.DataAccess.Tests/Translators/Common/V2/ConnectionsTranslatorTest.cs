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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.DataAccess.Translators.Common.V2;
using PTV.Database.DataAccess.Translators.Common.V2.Connections;
using PTV.Database.DataAccess.Translators.Services.V2;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Common.V2
{
    public class ConnectionsTranslatorTest : TranslatorTestBase
    {
        [Fact]
        public void TranslateChannelConnections_CorrectOrder()
        {
            var connectionEntities = CreateConnectionEntities();
            RegisterRepositories(connectionEntities);
            var translators = RegisterTranslators();
            var connectionsInput = CreateConnectionsInput(DomainEnum.Channels);

            var translation = RunTranslationModelToEntityTest<VmConnectionsInput, ServiceChannel>(translators, connectionsInput, unitOfWorkMockSetup.Object);
            var connections = translation.ServiceServiceChannels.ToList();

            Assert.Equal(3, connections.Count);

            var first = connections[0];
            var second = connections[1];
            var third = connections[2];

            // The order for services connected to the current channel is in correct order
            Assert.Equal(1, first.ServiceOrderNumber);
            Assert.Equal(2, second.ServiceOrderNumber);
            Assert.Equal(3, third.ServiceOrderNumber);

            // The order of channels for their respective services is unchanged
            Assert.Equal(4, first.ChannelOrderNumber);
            Assert.Equal(5, second.ChannelOrderNumber);
            Assert.Null(third.ChannelOrderNumber);
        }

        [Fact]
        public void TranslateServiceConnections_CorrectOrder()
        {
            var connectionEntities = CreateConnectionEntities();
            RegisterRepositories(connectionEntities);
            var translators = RegisterTranslators();
            var connectionsInput = CreateConnectionsInput(DomainEnum.Services);

            var translation = RunTranslationModelToEntityTest<VmConnectionsInput, Service>(translators, connectionsInput, unitOfWorkMockSetup.Object);
            var connections = translation.ServiceServiceChannels.ToList();

            Assert.Equal(3, connections.Count);

            var first = connections[0];
            var second = connections[1];
            var third = connections[2];

            // The order for channels connected to the current service is in correct order
            Assert.Equal(1, first.ChannelOrderNumber);
            Assert.Equal(2, second.ChannelOrderNumber);
            Assert.Equal(3, third.ChannelOrderNumber);

            // The order of services for their respective channels is unchanged
            Assert.Equal(7, first.ServiceOrderNumber);
            Assert.Equal(8, second.ServiceOrderNumber);
            Assert.Null(third.ServiceOrderNumber);
        }

        private void RegisterRepositories(List<ServiceServiceChannel> connectionEntities)
        {
            unitOfWorkMockSetup.Setup(x => x.GetSet<ServiceServiceChannel>())
                .Returns(new TestDbSet<ServiceServiceChannel>(connectionEntities));
            RegisterRepository<IRepository<ServiceServiceChannel>, ServiceServiceChannel>(connectionEntities.AsQueryable());
        }

        private List<ServiceServiceChannel> CreateConnectionEntities()
        {
            return new List<ServiceServiceChannel>
            {
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 4,
                    ServiceOrderNumber = 7,
                    ServiceId = GetEntityGuid(DomainEnum.Services, 0),
                    ServiceChannelId = GetEntityGuid(DomainEnum.Channels, 1),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 5,
                    ServiceOrderNumber = 8,
                    ServiceId = GetEntityGuid(DomainEnum.Services, 0),
                    ServiceChannelId = GetEntityGuid(DomainEnum.Channels, 2),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 4,
                    ServiceOrderNumber = 7,
                    ServiceId = GetEntityGuid(DomainEnum.Services, 1),
                    ServiceChannelId = GetEntityGuid(DomainEnum.Channels, 0),
                    IsASTIConnection = false
                },
                new ServiceServiceChannel
                {
                    ChannelOrderNumber = 5,
                    ServiceOrderNumber = 8,
                    ServiceId = GetEntityGuid(DomainEnum.Services, 2),
                    ServiceChannelId = GetEntityGuid(DomainEnum.Channels, 0),
                    IsASTIConnection = false
                },
            };
        }

        private VmConnectionsInput CreateConnectionsInput(DomainEnum mainEntityType)
        {
            var connectedEntityType = mainEntityType == DomainEnum.Channels ? DomainEnum.Services : DomainEnum.Channels;

            return new VmConnectionsInput
            {
                Id = GetEntityGuid(mainEntityType, 0),
                IsAsti = false,
                UnificRootId = GetEntityGuid(mainEntityType, 0),
                UseOrder = true,
                SelectedConnections = new List<VmConnectionInput>
                {
                    new VmConnectionInput
                    {
                        MainEntityId = GetEntityGuid(mainEntityType, 0),
                        MainEntityType = mainEntityType,
                        ChannelOrderNumber = 4,
                        ServiceOrderNumber = 7,
                        ConnectedEntityId = GetEntityGuid(connectedEntityType, 1),
                    },
                    new VmConnectionInput
                    {
                        MainEntityId = GetEntityGuid(mainEntityType, 0),
                        MainEntityType = mainEntityType,
                        ChannelOrderNumber = 5,
                        ServiceOrderNumber = 8,
                        ConnectedEntityId = GetEntityGuid(connectedEntityType, 2),
                    },
                    new VmConnectionInput
                    {
                        MainEntityId = GetEntityGuid(mainEntityType, 0),
                        MainEntityType = mainEntityType,
                        ChannelOrderNumber = 6,
                        ServiceOrderNumber = 9,
                        ConnectedEntityId = GetEntityGuid(connectedEntityType, 3),
                    },
                }
            };
        }

        private Guid GetEntityGuid(DomainEnum entityType, int entityOrder)
        {
            var prefix = (entityType == DomainEnum.Channels ? ChannelName : ServiceName);
            return (prefix + entityOrder).GetGuid();
        }

        private List<object> RegisterTranslators()
        {
            var translators = new List<object>
            {
                new ChannelConnectionsInputTranslator(ResolveManager, TranslationPrimitives),
                new ServiceConnectionsInputTranslator(ResolveManager, TranslationPrimitives),
                new ConnectionInputTranslator(ResolveManager, TranslationPrimitives, CacheManager)
            };

            return translators;
        }

        private const string ChannelName = "channel";
        private const string ServiceName = "service";
    }
}
