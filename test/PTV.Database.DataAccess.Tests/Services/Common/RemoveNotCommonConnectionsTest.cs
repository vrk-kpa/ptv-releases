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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Common
{
    public class RemoveNotCommonConnectionsTest : TestBase
    {
        private readonly CommonService _commonService;
        private readonly Guid _channelId = "ChannelId".GetGuid();
        private readonly Guid _serviceId = "ServiceId".GetGuid();
        private readonly Guid _organization1Id = "Organization1Id".GetGuid();
        private readonly Guid _organization2Id = "Organization2Id".GetGuid();
        private readonly Mock<IServiceServiceChannelRepository> _connectionsRepo;

        public RemoveNotCommonConnectionsTest()
        {
            SetupTypesCacheMock<ServiceChannelConnectionType>();
            SetupTypesCacheMock<PublishingStatusType>();

            _connectionsRepo = RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(Connections
                .AsQueryable());

            _commonService = new CommonService(
                null,
                null,
                contextManagerMock.Object,
                CacheManager.TypesCache,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
        }

        private List<ServiceServiceChannel> Connections => new List<ServiceServiceChannel>
        {
            new ServiceServiceChannel
            {
                ServiceChannelId = _channelId,
                ServiceId = _serviceId
            }
        };
        private List<ServiceChannelVersioned> ServiceChannelsVersions => new List<ServiceChannelVersioned>
        {
            new ServiceChannelVersioned
            {
                Id = _channelId,
                ConnectionTypeId = ServiceChannelConnectionTypeEnum.NotCommon.ToString().GetGuid(),
                UnificRootId = _channelId,
                OrganizationId = _organization1Id
            }
        };
        private List<ServiceVersioned> ServiceVersions => new List<ServiceVersioned>
        {
            new ServiceVersioned
            {
                UnificRootId = _serviceId,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                OrganizationId = _organization2Id
            }
        };

        private void RegisterRepositories(List<ServiceChannelVersioned> channels, List<ServiceVersioned> services)
        {
            RegisterRepository<IServiceVersionedRepository, ServiceVersioned>(services
                .AsQueryable());
            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(channels
                .AsQueryable());
        }

        [Fact]
        public void NotCommonTypeIsRemovedForDifferentOrganizations()
        {
            RegisterRepositories(ServiceChannelsVersions, ServiceVersions);
            var versionedIds = new List<Guid> {_channelId};
            var unitOfWork = unitOfWorkMockSetup.Object;

            _connectionsRepo.Setup(
                r => r.Remove(It.IsAny<ServiceServiceChannel>()));

            _commonService.RemoveNotCommonConnections(versionedIds, unitOfWork);
            var removedConnection = Connections[0];

            _connectionsRepo.Verify(
                r => r.Remove(It.Is<ServiceServiceChannel>(ssc =>
                    ssc.ServiceId == removedConnection.ServiceId &&
                    ssc.ServiceChannelId == removedConnection.ServiceChannelId)), Times.Once);
        }

        [Fact]
        public void NotCommonTypeIsNotRemovedForSameOrganizations()
        {
            var editedServiceVersions = ServiceVersions;
            editedServiceVersions[0].OrganizationId = _organization1Id;
            RegisterRepositories(ServiceChannelsVersions, editedServiceVersions);

            var versionedIds = new List<Guid> {_channelId};
            var unitOfWork = unitOfWorkMockSetup.Object;

            _commonService.RemoveNotCommonConnections(versionedIds, unitOfWork);

            _connectionsRepo.Verify(r => r.Remove(It.IsAny<ServiceServiceChannel>()), Times.Never);
        }

        [Fact]
        public void CommonTypeIsNotRemovedFromDifferentOrganizations()
        {
            var editedChannelVersions = ServiceChannelsVersions;
            editedChannelVersions[0].ConnectionTypeId =
                ServiceChannelConnectionTypeEnum.CommonForAll.ToString().GetGuid();
            RegisterRepositories(editedChannelVersions, ServiceVersions);

            var versionedIds = new List<Guid> {_channelId};
            var unitOfWork = unitOfWorkMockSetup.Object;

            _commonService.RemoveNotCommonConnections(versionedIds, unitOfWork);

            _connectionsRepo.Verify(r => r.Remove(It.IsAny<ServiceServiceChannel>()), Times.Never);
        }
    }
}
