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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.OpenApi;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class AddServiceTests : ServiceServiceTestBase
    {
        private Guid _serviceId;
        private Guid _serviceVersionedId;
        private Guid _channelId;
        private Guid _channelVersionedId;

        public AddServiceTests()
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
            translationManagerVModelMockSetup.Setup(t => t.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<IVmOpenApiServiceInVersionBase>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((IVmOpenApiServiceInVersionBase x, IUnitOfWorkWritable y) =>
                {
                    if (!string.IsNullOrEmpty(x.StatutoryServiceGeneralDescriptionId))
                    {
                        serviceVersioned.StatutoryServiceGeneralDescriptionId = x.StatutoryServiceGeneralDescriptionId.ParseToGuid();
                    }                    
                    serviceVersioned.PublishingStatusId = PublishingStatusCache.Get(x.PublishingStatus);
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

            translationManagerVModelMockSetup.Setup(t => t.TranslateAll<VmOpenApiServiceServiceChannelInVersionBase, ServiceServiceChannel>(It.IsAny<List<V7VmOpenApiServiceServiceChannelAstiInBase>>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((List<V7VmOpenApiServiceServiceChannelAstiInBase> x, IUnitOfWorkWritable y) =>
                {
                    var connections = new List<ServiceServiceChannel>();
                    x.ForEach(c =>
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
                            connections.Add(connection);
                        }
                    });
                    return connections;
                });

            ServiceRepoMock.Setup(g => g.All()).Returns((new List<ServiceVersioned> { serviceVersioned }).AsQueryable());
            ConnectionRepoMock.Setup(g => g.All()).Returns(connectionList.AsQueryable());

            //ServiceChannelRepoMock.Setup(g => g.All()).Returns((new List<ServiceChannelVersioned> { channelVersioned }).AsQueryable());
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

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, _serviceId, PublishingStatus.Published, true)).Returns(_serviceVersionedId);

            translationManagerMockSetup.Setup(t => t.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()))
                .Returns((ServiceVersioned sv) =>
                {
                    var vm = new VmOpenApiServiceVersionBase()
                    {
                        Id = sv.UnificRootId,
                        StatutoryServiceGeneralDescriptionId = sv.StatutoryServiceGeneralDescriptionId,
                        PublishingStatus = PublishingStatusCache.GetByValue(sv.PublishingStatusId)
                    };
                    if (sv.ServiceNames?.Count > 0)
                    {
                        vm.ServiceNames = new List<VmOpenApiLocalizedListItem>();
                        sv.ServiceNames.ForEach(n => vm.ServiceNames.Add(new VmOpenApiLocalizedListItem { Value = n.Name }));
                    }
                    if (sv.UnificRoot?.ServiceServiceChannels?.Count > 0)
                    {
                        vm.ServiceChannels = new List<V7VmOpenApiServiceServiceChannel>();
                        sv.UnificRoot.ServiceServiceChannels.ForEach(c =>
                        {
                            var channel = new V7VmOpenApiServiceServiceChannel
                            {
                                ServiceChannel = new VmOpenApiItem { Id = c.ServiceChannelId }
                            };
                            vm.ServiceChannels.Add(channel);
                        });
                    }
                    
                    return vm;
                });
        }

        [Fact]
        public void Add_CannotGetUser()
        {
            // Arrange
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns((string)null);
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            Action act = () => service.AddService(new VmOpenApiServiceInVersionBase(), false, DefaultVersion, false);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void Add_ExternalSourceExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var userName = "userName";
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId, RelationId = userName, ObjectType = typeof(Model.Models.Service).Name }
                }.AsQueryable());
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            Action act = () => service.AddService(new VmOpenApiServiceInVersionBase() { SourceId = sourceId }, false, DefaultVersion, false);

            // Assert
            act.ShouldThrowExactly<ExternalSourceExistsException>(string.Format(CoreMessages.OpenApi.ExternalSourceExists, sourceId));
        }

        [Fact]
        public void Add_PublishedEntity()
        {
            // Arrange
            var sourceId = "sourceId";
            var userName = "userName";
            var gdId = Guid.NewGuid();
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedEntity = list.Where(i => i.PublishingStatusId == PublishedId).FirstOrDefault();
            var vm = new VmOpenApiServiceInVersionBase() { SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString() };
            
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId + "2", RelationId = userName, ObjectType = typeof(Model.Models.Service).Name }
                }.AsQueryable()); // does not return same source id
            gdServiceMock.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true)).Returns(new VmOpenApiGeneralDescriptionVersionBase());

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);
            
            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(_serviceVersionedId, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
           var result = service.AddService(vm, false, DefaultVersion, false);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V7VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(_serviceVersionedId, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void Add_DraftEntity()
        {
            // Arrange
            var userName = "userName";
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var draftEntity = list.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(PublishingStatus.Draft)).FirstOrDefault();
            var vm = new VmOpenApiServiceInVersionBase()
            {
                PublishingStatus = PublishingStatus.Draft.ToString()
            };
            
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);
            
            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(draftEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.AddService(vm, false, DefaultVersion, false);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V7VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Draft.ToString());
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(_serviceId, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()), Times.Never());
        }

        [Fact]
        public void Add_GDAttached_MapDataFormGD()
        {
            // Arrange
            var userName = "userName";
            var gdId = Guid.NewGuid();
            var gdNames = TestDataFactory.CreateLocalizedList("Name");
            var gdType = ServiceTypeEnum.PermissionAndObligation.ToString();
            var gdChargeType = ServiceChargeTypeEnum.Free.ToString();
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedEntity = list.Where(i => i.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedEntity.StatutoryServiceGeneralDescriptionId = gdId;
            var vm = new VmOpenApiServiceInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                StatutoryServiceGeneralDescriptionId = gdId.ToString(),
                Type = ServiceTypeEnum.Service.ToString(),
                ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString()
            };
            
            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);
            
            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            gdServiceMock.Setup(g => g.GetGeneralDescriptionSimple(unitOfWork, It.IsAny<Guid>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase()
                {
                    Names = gdNames,
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });
            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase()
                {
                    Names = gdNames,
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.AddService(vm, false, DefaultVersion, false);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V7VmOpenApiService>(result);
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()), Times.Never());
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true), Times.Exactly(1)); // Saving
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true), Times.Exactly(1)); // Geting
            vmResult.StatutoryServiceGeneralDescriptionId.Should().Be(gdId);
            vmResult.ServiceNames.Should().NotBeNull();
            vmResult.Type.Should().Be(gdType);
            vmResult.ServiceChargeType.Should().BeNull(); // service charge type is not gotten from GD (in GET) so should be null
        }

        [Fact]
        public void Add_GDAttached_AttachProposedChannels()
        {
            // Arrange
            var userName = "userName";
            var gdId = Guid.NewGuid();
            var descriptions = TestDataFactory.CreateLocalizedList("Description");
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            var publishedEntity = list.Where(i => i.PublishingStatusId == PublishedId).FirstOrDefault();
            publishedEntity.StatutoryServiceGeneralDescriptionId = gdId;
            var vm = new VmOpenApiServiceInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                StatutoryServiceGeneralDescriptionId = gdId.ToString(),
            };

            var unitOfWork = unitOfWorkMockSetup.Object;

            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker);

            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            gdServiceMock.Setup(g => g.GetGeneralDescriptionSimple(unitOfWork, It.IsAny<Guid>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase()
                {
                    ServiceChannels = new List<V6VmOpenApiServiceServiceChannel>
                    {
                        new V6VmOpenApiServiceServiceChannel{ServiceChannel = new VmOpenApiItem{Id = _channelId}, Description = descriptions}
                    }
                });
            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase()
                {
                    ServiceChannels = new List<V6VmOpenApiServiceServiceChannel>
                    {
                        new V6VmOpenApiServiceServiceChannel{ServiceChannel = new VmOpenApiItem{Id = _channelId}, Description = descriptions}
                    }
                });

            var service = new ServiceService(contextManager, translationManagerMockSetup.Object, translationManagerVModelMockSetup.Object, Logger, serviceUtilities,
                DataUtils, CommonService, new VmOwnerReferenceLogic(), CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker);

            // Act
            var result = service.AddService(vm, false, DefaultVersion, true);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V7VmOpenApiService>(result);
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()), Times.Never());
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true), Times.Exactly(1)); // Saving
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true), Times.Exactly(1)); // Geting
            vmResult.StatutoryServiceGeneralDescriptionId.Should().Be(gdId);
            vmResult.ServiceChannels.Should().NotBeNull();
            vmResult.ServiceChannels.Count.Should().Be(1);
        }
    }
}
