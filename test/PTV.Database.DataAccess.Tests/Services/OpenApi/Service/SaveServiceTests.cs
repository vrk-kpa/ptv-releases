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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.OpenApi;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class SaveServiceTests : ServiceServiceTestBase
    {
        private Guid _serviceId;
        private Guid _serviceVersionedId;
        private Guid _channelId;
        private Guid _channelVersionedId;

        public SaveServiceTests()
        {
            _serviceId = Guid.NewGuid();
            _serviceVersionedId = Guid.NewGuid();
            _channelId = Guid.NewGuid();
            _channelVersionedId = Guid.NewGuid();

            SetupTypesCacheMock<ServiceType>();
            SetupTypesCacheMock<ServiceChargeType>();

            var serviceVersioned = new ServiceVersioned
            {
                Id = _serviceVersionedId,
                UnificRootId = _serviceId,
                UnificRoot = new Model.Models.Service()
                {
                    Id = _serviceId,
                    ServiceServiceChannels = new List<ServiceServiceChannel>()
                },
                Organization = new Model.Models.Organization()
            };

            var channelVersioned = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId, _channelId, _channelVersionedId);
            channelVersioned.UnificRoot = new ServiceChannel { Id = _channelId, Versions = new List<ServiceChannelVersioned> { channelVersioned } };

            var connectionList = new List<ServiceServiceChannel>
            {
                new ServiceServiceChannel {
                    Service = serviceVersioned.UnificRoot,
                    ServiceId = serviceVersioned.UnificRootId,
                    ServiceChannel = channelVersioned.UnificRoot,
                    ServiceChannelId = channelVersioned.UnificRootId
                }
            };

            // for PUT/SAVE
            translationManagerVModelMockSetup.Setup(t => t.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<IVmOpenApiServiceInVersionBase>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((IVmOpenApiServiceInVersionBase x, IUnitOfWorkWritable y) =>
                {
                    if (!string.IsNullOrEmpty(x.GeneralDescriptionId))
                    {
                        serviceVersioned.StatutoryServiceGeneralDescriptionId = x.GeneralDescriptionId.ParseToGuid();
                    }
                    if (!string.IsNullOrEmpty(x.PublishingStatus))
                    {
                        serviceVersioned.PublishingStatusId = PublishingStatusCache.Get(x.PublishingStatus);
                    }
                    
                    if (x.ServiceNames?.Count > 0)
                    {
                        serviceVersioned.ServiceNames = new List<ServiceName>();
                        x.ServiceNames.ForEach(n => serviceVersioned.ServiceNames.Add(new ServiceName { Name = n.Value }));
                    }
                    if (!string.IsNullOrEmpty(x.Type))
                    {
                        serviceVersioned.TypeId = TypeCache.Get<ServiceType>(x.Type);
                    }
                    if (!string.IsNullOrEmpty(x.ServiceChargeType))
                    {
                        serviceVersioned.ChargeTypeId = TypeCache.Get<ServiceChargeType>(x.ServiceChargeType);
                    }

                    return serviceVersioned;
                });

            translationManagerVModelMockSetup.Setup(t => t.Translate<V7VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V7VmOpenApiServiceAndChannelRelationAstiInBase>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((V7VmOpenApiServiceAndChannelRelationAstiInBase x, IUnitOfWorkWritable y) =>
                {
                    var service = new Model.Models.Service();
                    if (x.ChannelRelations?.Count > 0)
                    {
                        service.ServiceServiceChannels = new List<ServiceServiceChannel>();
                        x.ChannelRelations.ForEach(c =>
                        {
                            var cv = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId, c.ChannelGuid, _channelVersionedId);
                            cv.UnificRoot = new ServiceChannel { Id = c.ChannelGuid, Versions = new List<ServiceChannelVersioned> { cv } };
                            var connection = new ServiceServiceChannel
                            {
                                ServiceChannelId = c.ChannelGuid,
                                ServiceChannel = cv.UnificRoot
                            };
                            if (c.Description?.Count > 0)
                            {
                                connection.ServiceServiceChannelDescriptions = new List<ServiceServiceChannelDescription>();
                                c.Description.ForEach(d =>
                                {
                                    var description = new ServiceServiceChannelDescription { Description = d.Value };
                                    connection.ServiceServiceChannelDescriptions.Add(description);
                                });
                                service.ServiceServiceChannels.Add(connection);
                            }
                        });
                    }                   
                        
                    return service;
                });

            ServiceRepoMock.Setup(g => g.All()).Returns((new List<ServiceVersioned> { serviceVersioned }).AsQueryable());
            ConnectionRepoMock.Setup(g => g.All()).Returns(connectionList.AsQueryable());
            
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
               It.IsAny<bool>()
               )).Returns(new List<ServiceVersioned> { serviceVersioned }.AsQueryable());


            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceServiceChannel>>(),
               It.IsAny<Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>>>(),
               It.IsAny<bool>()
               )).Returns(connectionList.AsQueryable());

            // for GET
            ArrangeTranslateService();
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns("user");

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.SaveService(null, false, DefaultVersion, false);

            // Assert
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Never());
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

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            Action act = () => service.SaveService(new VmOpenApiServiceInVersionBase(), false, DefaultVersion, false);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void RightVersionNotFound()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns("user");

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, _serviceId, null, false))
                .Returns((Guid?)null);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.SaveService(new VmOpenApiServiceInVersionBase { Id = _serviceId }, false, DefaultVersion, false);

            // Assert
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Never());
        }

        [Fact]
        public void ExternalSourceNotExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var userName = "userName";
            var entityName = typeof(Model.Models.Service).Name;
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId, RelationId = userName, ObjectType = entityName }
                }.AsQueryable());

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, _serviceId, null, false))
                .Returns((Guid?)null);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            Action act = () => service.SaveService(new VmOpenApiServiceInVersionBase(), false, DefaultVersion, false, sourceId);

            // Assert
            act.ShouldThrowExactly<Exception>(string.Format(CoreMessages.OpenApi.EntityNotFound, entityName, sourceId));
        }

        [Fact]
        public void ExternalSourceExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var userName = "userName";
            var entityName = typeof(Model.Models.Service).Name;
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId, RelationId = userName, ObjectType = entityName, PTVId = _serviceId }
                }.AsQueryable());

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, _serviceId, null, false))
                .Returns(_serviceVersionedId);
            
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.SaveService(new VmOpenApiServiceInVersionBase(), false, DefaultVersion, false, sourceId);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V8VmOpenApiService>(result);
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
        }

        [Fact]
        public void GdAttached_MapDataFormGD()
        {
            // Arrange
            var userName = "userName";
            var gdId = Guid.NewGuid();
            var gdType = ServiceTypeEnum.PermissionAndObligation.ToString();
            var gdChargeType = ServiceChargeTypeEnum.Free.ToString();
            var vm = new VmOpenApiServiceInVersionBase
            {
                Id = _serviceId,
                GeneralDescriptionId = gdId.ToString()
            };
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, _serviceId, null, false))
                .Returns(_serviceVersionedId);

            gdServiceMock.Setup(g => g.GetGeneralDescriptionSimple(unitOfWork, It.IsAny<Guid>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase()
                {
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });
            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase()
                {
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.SaveService(vm, false, DefaultVersion, true);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V8VmOpenApiService>(result);
            vmResult.GeneralDescriptionId.Should().Be(gdId);
            vmResult.Type.Should().Be(gdType);
            vmResult.ServiceChargeType.Should().BeNull(); // service charge type is not gotten from GD (in GET) so should be null
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
        }

        [Fact]
        public void GdAttached_AttachProposedChannels()
        {
            // Arrange
            var userName = "userName";
            var gdId = Guid.NewGuid();
            var descriptions = TestDataFactory.CreateLocalizedList("Description");
            var entityName = typeof(Model.Models.Service).Name;
            var vm = new VmOpenApiServiceInVersionBase
            {
                Id = _serviceId,
                GeneralDescriptionId = gdId.ToString()
            };
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, _serviceId, null, false))
                .Returns(_serviceVersionedId);

            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase()
                {
                    ServiceChannels = new List<V6VmOpenApiServiceServiceChannel>
                    {
                        new V6VmOpenApiServiceServiceChannel{ServiceChannel = new VmOpenApiItem{Id = _channelId}, Description = descriptions}
                    }
                });

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.SaveService(vm, false, DefaultVersion, true);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V8VmOpenApiService>(result);
            vmResult.GeneralDescriptionId.Should().Be(gdId);
            vmResult.ServiceChannels.Should().NotBeNull();
            vmResult.ServiceChannels.Count.Should().Be(1);
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
        }

    }
}
