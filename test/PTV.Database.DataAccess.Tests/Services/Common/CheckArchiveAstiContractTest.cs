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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Common
{
    public class CheckArchiveAstiContractTest : TestBase
    {
        private readonly CommonService _commonService;
        private readonly Guid _channelId = "ChannelId".GetGuid();
        private readonly Guid _serviceId = "ServiceId".GetGuid();
        private readonly Guid _channelVersionedId = "ChannelVersionedId".GetGuid();
        private readonly Guid _serviceVersionedId = "ServiceVersionedId".GetGuid();
        private readonly Mock<IVersioningManager> _versioningManagerMock;
        private readonly Mock<IPahaTokenProcessor> _pahaTokenProcessorMock;
        public CheckArchiveAstiContractTest()
        {
            
            _versioningManagerMock = new Mock<IVersioningManager>(MockBehavior.Strict);
            SetupTypesCacheMock<ServiceChannelConnectionType>();
            SetupTypesCacheMock<PublishingStatusType>();
            
            _pahaTokenProcessorMock = new Mock<IPahaTokenProcessor>();
            
            _commonService = new CommonService(
                null,
                null,
                contextManagerMock.Object,
                CacheManager.TypesCache,
                null,
                null,
                null,
                null,
                _versioningManagerMock.Object,
                null,
                null,
                null,
                null,
                _pahaTokenProcessorMock.Object,
                null,
                null,
                null,
                null);
            
            _versioningManagerMock
                .Setup(x => x.GetUnificRootId<ServiceVersioned>(It.IsAny<IUnitOfWorkWritable>(), It.IsAny<Guid>()))
                .Returns(_serviceId);
            _versioningManagerMock
                .Setup(x => x.GetUnificRootId<ServiceChannelVersioned>(It.IsAny<IUnitOfWorkWritable>(), It.IsAny<Guid>()))
                .Returns(_channelId);

        }

        private List<ServiceServiceChannel> Connections => new List<ServiceServiceChannel>
        {
            new ServiceServiceChannel
            {
                ServiceChannelId = _channelId,
                ServiceId = _serviceId
            }
        };
        
        private List<ServiceServiceChannel> AstiConnections => new List<ServiceServiceChannel>
        {
            new ServiceServiceChannel
            {
                ServiceChannelId = _channelId,
                ServiceId = _serviceId,
                IsASTIConnection = true
            }
        };
        
        private List<ServiceChannelVersioned> ServiceChannelsVersions => new List<ServiceChannelVersioned>
        {
            new ServiceChannelVersioned
            {
                Id = _channelVersionedId,
                UnificRootId = _channelId,
            }
        };
        private List<ServiceVersioned> ServiceVersions => new List<ServiceVersioned>
        {
            new ServiceVersioned
            {
                Id = _serviceVersionedId,
                UnificRootId = _serviceId,
            }
        };

        private void RegisterRepositories(List<ServiceChannelVersioned> channels, List<ServiceVersioned> services)
        {
            RegisterRepository<IServiceVersionedRepository, ServiceVersioned>(services
                .AsQueryable());
            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(channels
                .AsQueryable());
        }
        
        [Theory]
        [InlineData(UserRoleEnum.Eeva)]
        [InlineData(UserRoleEnum.Pete)]
        [InlineData(UserRoleEnum.Shirley)]
        public void RemoveNotAstiServiceContractTest(UserRoleEnum role)
        {
            RegisterRepositories(ServiceChannelsVersions, ServiceVersions);
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(Connections.AsQueryable());
            
            _pahaTokenProcessorMock.Setup(i => i.UserRole).Returns(role);
            _commonService.CheckArchiveAstiContract<ServiceVersioned>(unitOfWorkMockSetup.Object, _serviceId);

            _pahaTokenProcessorMock.Verify(i => i.UserRole, Times.Once);
        }   
        
        [Theory]
        [InlineData(UserRoleEnum.Eeva, false)]
        [InlineData(UserRoleEnum.Pete, true)]
        [InlineData(UserRoleEnum.Shirley, true)]
        public void RemoveAstiServiceContractTest(UserRoleEnum role, bool throwEx)
        {
            RegisterRepositories(ServiceChannelsVersions, ServiceVersions);
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(AstiConnections.AsQueryable());

            _pahaTokenProcessorMock.Setup(i => i.UserRole).Returns(role);
            Action action = () => _commonService.CheckArchiveAstiContract<ServiceVersioned>(unitOfWorkMockSetup.Object, _serviceId);
            if (throwEx)
            {
                action.Should().ThrowExactly<PtvAppException>();
            }
            else
            {
                action();
            }
            _pahaTokenProcessorMock.Verify(i => i.UserRole, Times.Once);
        }
        
        [Theory]
        [InlineData(UserRoleEnum.Eeva)]
        [InlineData(UserRoleEnum.Pete)]
        [InlineData(UserRoleEnum.Shirley)]
        public void RemoveNotAstiServiceLocationContractTest(UserRoleEnum role)
        {
            RegisterRepositories(ServiceChannelsVersions, ServiceVersions);
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(Connections.AsQueryable());

            _pahaTokenProcessorMock.Setup(i => i.UserRole).Returns(role);
            _commonService.CheckArchiveAstiContract<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, _channelId);

            _pahaTokenProcessorMock.Verify(i => i.UserRole, Times.Once);
        }  
        
        [Theory]
        [InlineData(UserRoleEnum.Eeva, false)]
        [InlineData(UserRoleEnum.Pete, true)]
        [InlineData(UserRoleEnum.Shirley, true)]
        public void RemoveAstiServiceLocationContractTest(UserRoleEnum role, bool throwEx)
        {
            RegisterRepositories(ServiceChannelsVersions, ServiceVersions);
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(AstiConnections.AsQueryable());

            _pahaTokenProcessorMock.Setup(i => i.UserRole).Returns(role);
            Action action = () => _commonService.CheckArchiveAstiContract<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, _channelId);
            if (throwEx)
            {
                action.Should().ThrowExactly<PtvAppException>();
            }
            else
            {
                action();
            }
            _pahaTokenProcessorMock.Verify(i => i.UserRole, Times.Once);
        }
    }
}
