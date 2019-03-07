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
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.ServiceAndChannel
{
    public class SaveServiceConnectionsBySourceTests : ServiceAndChannelTestBase
    {
        private ServiceAndChannelService _service;

        public SaveServiceConnectionsBySourceTests()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);
            _service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object,
                translationManagerVModelMockSetup.Object, Logger, serviceUtilities, ServiceService, ChannelService,
                PublishingStatusCache, UserOrganizationChecker, VersioningManager);
        }

        [Fact]
        public void ModelIsNull()
        {
            // Act
            var result = _service.SaveServiceConnectionsBySource(null, null, DefaultVersion, false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void CannotGetUser()
        {
            // Arrange
            UserIdentificationMock.Setup(s => s.UserName).Returns((string)null);
            
            // Act
            Action act = () => _service.SaveServiceConnectionsBySource(null, new V9VmOpenApiServiceAndChannelRelationBySourceAsti(), DefaultVersion, false);

            // Assert
            act.Should().ThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void ExternalSourceForServiceNotExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var userName = "userName";

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = "someOtherSourceId", RelationId = userName, ObjectType = typeof(Model.Models.Service).Name }
                }.AsQueryable());
                        
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            
            // Act
            Action act = () => _service.SaveServiceConnectionsBySource(sourceId, new V9VmOpenApiServiceAndChannelRelationBySourceAsti(), DefaultVersion, false);

            // Assert
            act.Should().ThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void ExternalSourceForChannelNotExists()
        {
            // Arrange
            var serviceSourceId = "sourceId";
            var channelSourceId = "sourceId2";
            var userName = "userName";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceAsti()
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelBySourceAsti>
                {
                    new V9VmOpenApiServiceServiceChannelBySourceAsti
                    {
                        ServiceChannelSourceId = channelSourceId,
                    }
                }
            };

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = serviceSourceId, RelationId = userName, ObjectType = typeof(Model.Models.Service).Name },
                    new ExternalSource { SourceId = "otherSourceId", RelationId = userName, ObjectType = typeof(ServiceChannel).Name }
                }.AsQueryable());
            
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            
            // Act
            Action act = () => _service.SaveServiceConnectionsBySource(serviceSourceId, request, DefaultVersion, false);

            // Assert
            act.Should().ThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Draft)]
        public void CanModifyConnections(PublishingStatus status)
        {
            // Arrange
            var serviceSourceId = "sourceId";
            var channelSourceId = "sourceId2";
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var userName = "userName";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceAsti()
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelBySourceAsti>
                {
                    new V9VmOpenApiServiceServiceChannelBySourceAsti
                    {
                        ServiceChannelSourceId = channelSourceId,
                    }
                }
            };

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { PTVId = serviceId,  SourceId = serviceSourceId, RelationId = userName, ObjectType = typeof(Model.Models.Service).Name },
                    new ExternalSource { PTVId = channelId,  SourceId = channelSourceId, RelationId = userName, ObjectType = typeof(ServiceChannel).Name }
                }.AsQueryable());

            translationManagerVModelMockSetup.Setup(s => s.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V9VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object))
                .Returns(new Model.Models.Service()
                {
                    Id = serviceId,
                });

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            
            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns(status);

            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel
                {
                    ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString(),
                    Security = new VmSecurityOwnOrganization { IsOwnOrganization = true }
                });
            
            // Act
            var result = _service.SaveServiceConnectionsBySource(serviceSourceId, request, DefaultVersion, false);

            // Assert
            // We are not testing method GetServiceById so we expect result to be null.
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V9VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object), Times.Once());
        }
        
        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void ServiceNotPublished(PublishingStatus status)
        {
            // Arrange
            var serviceSourceId = "sourceId";
            var channelSourceId = "sourceId2";
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var userName = "userName";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceAsti()
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelBySourceAsti>
                {
                    new V9VmOpenApiServiceServiceChannelBySourceAsti
                    {
                        ServiceChannelSourceId = channelSourceId,
                    }
                }
            };

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { PTVId = serviceId,  SourceId = serviceSourceId, RelationId = userName, ObjectType = typeof(Model.Models.Service).Name },
                    new ExternalSource { PTVId = channelId,  SourceId = channelSourceId, RelationId = userName, ObjectType = typeof(ServiceChannel).Name }
                }.AsQueryable());
            
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns(status);

            // Act
            Action act = () => _service.SaveServiceConnectionsBySource(serviceSourceId, request, DefaultVersion, false);

            // Assert
            act.Should().ThrowExactly<Exception>(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", serviceSourceId));
            ServiceServiceMock.Verify(m => m.GetLatestVersionPublishingStatus(serviceId), Times.Once);
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void ServiceChannelNotPublished(PublishingStatus status)
        {
            // Arrange
            var serviceSourceId = "sourceId";
            var channelSourceId = "sourceId2";
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var userName = "userName";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceAsti()
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelBySourceAsti>
                {
                    new V9VmOpenApiServiceServiceChannelBySourceAsti
                    {
                        ServiceChannelSourceId = channelSourceId,
                    }
                }
            };

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { PTVId = serviceId,  SourceId = serviceSourceId, RelationId = userName, ObjectType = typeof(Model.Models.Service).Name },
                    new ExternalSource { PTVId = channelId,  SourceId = channelSourceId, RelationId = userName, ObjectType = typeof(ServiceChannel).Name }
                }.AsQueryable());
            
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            ServiceServiceMock.Setup(s => s.GetLatestVersionPublishingStatus(serviceId))
                .Returns(PublishingStatus.Published);
            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, false))
                .Returns(new VmOpenApiServiceChannel
                {
                    PublishingStatus = status.ToString(),
                    ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString(),
                    Security = new VmSecurityOwnOrganization { IsOwnOrganization = true }
                });
            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns((VmOpenApiServiceChannel)null);

            // Act
            Action act = () => _service.SaveServiceConnectionsBySource(serviceSourceId, request, DefaultVersion, false);

            // Assert
            act.Should().ThrowExactly<Exception>(string.Format(CoreMessages.OpenApi.EntityNotFound, "ServiceChannel", channelId));
            ServiceServiceMock.Verify(m => m.GetLatestVersionPublishingStatus(serviceId), Times.Once);
            ChannelServiceMock.Verify(m => m.GetServiceChannelByIdSimple(channelId, true), Times.Once);
        }
    }
}
