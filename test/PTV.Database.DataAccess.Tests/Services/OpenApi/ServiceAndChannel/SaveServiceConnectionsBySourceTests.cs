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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
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
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);

            // Act
            var result = service.SaveServiceConnectionsBySource(null, null, DefaultVersion);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void CannotGetUser()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns((string)null);
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);

            // Act
            Action act = () => service.SaveServiceConnectionsBySource(null, new V7VmOpenApiServiceAndChannelRelationBySourceAsti(), DefaultVersion);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
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

            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);

            // Act
            Action act = () => service.SaveServiceConnectionsBySource(sourceId, new V7VmOpenApiServiceAndChannelRelationBySourceAsti(), DefaultVersion);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void ExternalSourceForChannelNotExists()
        {
            // Arrange
            var serviceSourceId = "sourceId";
            var channelSourceId = "sourceId2";
            var userName = "userName";
            var request = new V7VmOpenApiServiceAndChannelRelationBySourceAsti()
            {
                ChannelRelations = new List<V7VmOpenApiServiceServiceChannelBySourceAsti>
                {
                    new V7VmOpenApiServiceServiceChannelBySourceAsti
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

            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);
            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);

            // Act
            Action act = () => service.SaveServiceConnectionsBySource(serviceSourceId, request, DefaultVersion);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void CanModifyConnections()
        {
            // Arrange
            var serviceSourceId = "sourceId";
            var channelSourceId = "sourceId2";
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var userName = "userName";
            var request = new V7VmOpenApiServiceAndChannelRelationBySourceAsti()
            {
                ChannelRelations = new List<V7VmOpenApiServiceServiceChannelBySourceAsti>
                {
                    new V7VmOpenApiServiceServiceChannelBySourceAsti
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

            var unitOfWork = unitOfWorkMockSetup.Object;

            translationManagerVModelMockSetup.Setup(s => s.Translate<V7VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V7VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWork))
                .Returns(new Model.Models.Service()
                {
                    Id = serviceId,
                });

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            ChannelServiceMock.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel
                {
                    ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString(),
                    Security = new VmSecurityOwnOrganization { IsOwnOrganization = true }
                });

            var service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelServiceMock.Object, PublishingStatusCache, UserOrganizationChecker);

            // Act
            var result = service.SaveServiceConnectionsBySource(serviceSourceId, request, DefaultVersion);

            // Assert
            // We are not testing method GetServiceById so we expect result to be null.
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V7VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V7VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWork), Times.Once());
        }
    }
}
