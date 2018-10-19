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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.ServiceAndChannel
{
    public class SaveServiceConnectionsTests : ServiceAndChannelTestBase
    {
        private ServiceAndChannelService _service;

        public SaveServiceConnectionsTests()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);
            _service = new ServiceAndChannelService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object,
                Logger, serviceUtilities, ServiceService, ChannelService, PublishingStatusCache, UserOrganizationChecker);
        }
        [Fact]
        public void ModelIsNull()
        {
            // Act
            var result = _service.SaveServiceConnections(null, DefaultVersion, false);

            // Assert
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V8VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V8VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object), Times.Never());
        }

        [Fact]
        public void ASTIUpdate_RegularConnectionsNotDeleted()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var request = new V9VmOpenApiServiceAndChannelRelationAstiInBase
            {
                //IsASTI = true,
                ServiceId = serviceId,
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelAstiInBase>
                {
                    new V9VmOpenApiServiceServiceChannelAstiInBase
                    {
                        ServiceGuid = serviceId,
                        ChannelGuid = channelId,
                    }
                }
            };

            translationManagerVModelMockSetup.Setup(s => s.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(request, unitOfWorkMockSetup.Object))
                .Returns(new Model.Models.Service()
                {
                    Id = serviceId,
                    ServiceServiceChannels = new List<ServiceServiceChannel>
                    {
                        new ServiceServiceChannel { IsASTIConnection = true, ServiceChannelId = channelId },
                        new ServiceServiceChannel { IsASTIConnection  = false, ServiceChannelId = Guid.NewGuid() }
                    }
                });

            // repositories
            ConnectionRepoMock.Setup(c => c.All()).Returns(new List<ServiceServiceChannel>().AsQueryable());
            DescriptionRepoMock.Setup(c => c.All()).Returns(new List<ServiceServiceChannelDescription>().AsQueryable());
                        
            // Act
            var result = _service.SaveServiceConnections(request, DefaultVersion, true);

            // Assert
            // We are not testing method GetServiceById so we expect result to be null.
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V9VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object), Times.Once());
            ConnectionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannel>()), Times.Never());
            DescriptionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannelDescription>()), Times.Never());
        }

        [Fact]
        public void ASTIUpdate_ASTIConnectionsNotInRequestDeleted()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var channelId2 = Guid.NewGuid();
            var request = new V9VmOpenApiServiceAndChannelRelationAstiInBase
            {
                //IsASTI = true,
                ServiceId = serviceId,
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelAstiInBase>
                {
                    new V9VmOpenApiServiceServiceChannelAstiInBase
                    {
                        ServiceGuid = serviceId,
                        ChannelGuid = channelId,
                    }
                }
            };

            // repositories
            var connectionList = new List<ServiceServiceChannel>()
            {
                new ServiceServiceChannel { ServiceId = serviceId, ServiceChannelId = channelId2 }
            };
            ConnectionRepoMock.Setup(c => c.All()).Returns(connectionList.AsQueryable());
            DescriptionRepoMock.Setup(c => c.All()).Returns(new List<ServiceServiceChannelDescription> { new ServiceServiceChannelDescription
            {
                ServiceId = serviceId, ServiceChannelId = channelId2
            } }.AsQueryable());
            
            translationManagerVModelMockSetup.Setup(s => s.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(request, unitOfWorkMockSetup.Object))
                .Returns(new Model.Models.Service()
                {
                    Id = serviceId,
                    ServiceServiceChannels = new List<ServiceServiceChannel>
                    {
                        new ServiceServiceChannel { IsASTIConnection = true, ServiceChannelId = channelId, ServiceId = serviceId },
                        new ServiceServiceChannel { IsASTIConnection = true, ServiceChannelId = channelId2, ServiceId = serviceId }, // additional ASTI connection that should be removed
                        new ServiceServiceChannel { IsASTIConnection  = false, ServiceChannelId = Guid.NewGuid() } // regular connection that should not be removed
                    }
                });

            // Act
            var result = _service.SaveServiceConnections(request, DefaultVersion, true);

            // Assert
            // We are not testing method GetServiceById so we expect result to be null.
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V9VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object), Times.Once());
            ConnectionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannel>()), Times.Once());
            DescriptionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannelDescription>()), Times.Once());
        }

        [Fact]
        public void RegularConnectionUpdate_ASTIConnectionsNotDeleted()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var request = new V9VmOpenApiServiceAndChannelRelationAstiInBase
            {
                //IsASTI = false,
                ServiceId = serviceId,
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelAstiInBase>
                {
                    new V9VmOpenApiServiceServiceChannelAstiInBase
                    {
                        ServiceGuid = serviceId,
                        ChannelGuid = channelId,
                    }
                }
            };

            translationManagerVModelMockSetup.Setup(s => s.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(request, unitOfWorkMockSetup.Object))
                .Returns(new Model.Models.Service()
                {
                    Id = serviceId,
                    ServiceServiceChannels = new List<ServiceServiceChannel>
                    {
                        new ServiceServiceChannel { IsASTIConnection = false, ServiceChannelId = channelId },
                        new ServiceServiceChannel { IsASTIConnection  = true, ServiceChannelId = Guid.NewGuid() }
                    }
                });

            // repositories
            ConnectionRepoMock.Setup(c => c.All()).Returns(new List<ServiceServiceChannel>().AsQueryable());
            DescriptionRepoMock.Setup(c => c.All()).Returns(new List<ServiceServiceChannelDescription>().AsQueryable());

            // Act
            var result = _service.SaveServiceConnections(request, DefaultVersion, false);

            // Assert
            // We are not testing method GetServiceById so we expect result to be null.
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V9VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object), Times.Once());
            ConnectionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannel>()), Times.Never());
            DescriptionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannelDescription>()), Times.Never());
        }

        [Fact]
        public void RegularConnectionUpdate_RegularConnectionsNotInRequestDeleted()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var channelId2 = Guid.NewGuid();
            var request = new V9VmOpenApiServiceAndChannelRelationAstiInBase
            {
                //IsASTI = false,
                ServiceId = serviceId,
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelAstiInBase>
                {
                    new V9VmOpenApiServiceServiceChannelAstiInBase
                    {
                        ServiceGuid = serviceId,
                        ChannelGuid = channelId,
                    }
                }
            };

            // repositories
            var connectionList = new List<ServiceServiceChannel>()
            {
                new ServiceServiceChannel { ServiceId = serviceId, ServiceChannelId = channelId2 }
            };
            ConnectionRepoMock.Setup(c => c.All()).Returns(connectionList.AsQueryable());
            DescriptionRepoMock.Setup(c => c.All()).Returns(new List<ServiceServiceChannelDescription> { new ServiceServiceChannelDescription
            {
                ServiceId = serviceId, ServiceChannelId = channelId2
            } }.AsQueryable());

            translationManagerVModelMockSetup.Setup(s => s.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(request, unitOfWorkMockSetup.Object))
                .Returns(new Model.Models.Service()
                {
                    Id = serviceId,
                    ServiceServiceChannels = new List<ServiceServiceChannel>
                    {
                        new ServiceServiceChannel { IsASTIConnection = false, ServiceChannelId = channelId, ServiceId = serviceId },
                        new ServiceServiceChannel { IsASTIConnection = false, ServiceChannelId = channelId2, ServiceId = serviceId }, // additional ASTI connection that should be removed
                        new ServiceServiceChannel { IsASTIConnection  = true, ServiceChannelId = Guid.NewGuid() } // ASTI connection that should not be removed
                    }
                });

            // Act
            var result = _service.SaveServiceConnections(request, DefaultVersion, false);

            // Assert
            // We are not testing method GetServiceById so we expect result to be null.
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V9VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object), Times.Once());
            ConnectionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannel>()), Times.Once());
            DescriptionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannelDescription>()), Times.Once());
        }

        [Fact]
        public void DeleteAllASTIConnections()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var channelId2 = Guid.NewGuid();
            var request = new V9VmOpenApiServiceAndChannelRelationAstiInBase
            {
                //IsASTI = true,
                ServiceId = serviceId,
                DeleteAllChannelRelations = true
            };

            // repositories
            var connectionList = new List<ServiceServiceChannel>()
            {
                new ServiceServiceChannel { ServiceId = serviceId, ServiceChannelId = channelId },
                new ServiceServiceChannel { ServiceId = serviceId, ServiceChannelId = channelId2 }
            };
            ConnectionRepoMock.Setup(c => c.All()).Returns(connectionList.AsQueryable());
            DescriptionRepoMock.Setup(c => c.All()).Returns(new List<ServiceServiceChannelDescription>
            {
                new ServiceServiceChannelDescription { ServiceId = serviceId, ServiceChannelId = channelId },
                new ServiceServiceChannelDescription { ServiceId = serviceId, ServiceChannelId = channelId2 },
            }.AsQueryable());

            translationManagerVModelMockSetup.Setup(s => s.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(request, unitOfWorkMockSetup.Object))
                .Returns(new Model.Models.Service()
                {
                    Id = serviceId,
                    ServiceServiceChannels = new List<ServiceServiceChannel>
                    {
                        new ServiceServiceChannel { IsASTIConnection = true, ServiceChannelId = channelId, ServiceId = serviceId },
                        new ServiceServiceChannel { IsASTIConnection = true, ServiceChannelId = channelId2, ServiceId = serviceId },
                        new ServiceServiceChannel { IsASTIConnection  = false, ServiceChannelId = Guid.NewGuid() } // regular connection that should not be removed
                    }
                });
 
            // Act
            var result = _service.SaveServiceConnections(request, DefaultVersion, true);

            // Assert
            // We are not testing method GetServiceById so we expect result to be null.
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(t => t.Translate<V9VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V9VmOpenApiServiceAndChannelRelationAstiInBase>(), unitOfWorkMockSetup.Object), Times.Once());
            ConnectionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannel>()), Times.Exactly(2));
            DescriptionRepoMock.Verify(x => x.Remove(It.IsAny<ServiceServiceChannelDescription>()), Times.Exactly(2));
        }
    }
}
