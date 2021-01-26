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
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Views;
using Xunit;
using PTV.Framework.Exceptions;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class SaveServiceTests : ServiceServiceTestBase
    {
        private Guid _serviceId;
        private Guid _serviceVersionedId;
        private Guid _channelId;
        private Guid _channelVersionedId;

        private ServiceService _service;

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
                UnificRoot = new Model.Models.Service
                {
                    Id = _serviceId,
                    ServiceServiceChannels = new List<ServiceServiceChannel>()
                },
                Organization = new Model.Models.Organization()
            };

            var channelVersioned = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId, _channelId, _channelVersionedId);

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

            translationManagerVModelMockSetup.Setup(t => t.Translate<V11VmOpenApiServiceAndChannelRelationAstiInBase, Model.Models.Service>(It.IsAny<V11VmOpenApiServiceAndChannelRelationAstiInBase>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((V11VmOpenApiServiceAndChannelRelationAstiInBase x, IUnitOfWorkWritable y) =>
                {
                    var service = new Model.Models.Service();
                    if (x.ChannelRelations?.Count > 0)
                    {
                        service.ServiceServiceChannels = new List<ServiceServiceChannel>();
                        x.ChannelRelations.ForEach(c =>
                        {
                            var cv = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId, c.ChannelGuid, _channelVersionedId);
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
            ServiceChannelRepoMock.Setup(g => g.All())
                .Returns(new List<ServiceChannelVersioned> {channelVersioned}.AsQueryable());
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

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> items, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<Model.Models.ServiceCollectionService>>(),
               It.IsAny<Func<IQueryable<Model.Models.ServiceCollectionService>, IQueryable<Model.Models.ServiceCollectionService>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<Model.Models.ServiceCollectionService> items, Func<IQueryable<Model.Models.ServiceCollectionService>, IQueryable<Model.Models.ServiceCollectionService>> func, bool applyFilters) =>
               {
                   return items;
               });

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceChannelVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceChannelVersioned> items, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });

            // for GET
            ArrangeTranslateService();

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, UrlServiceMock.Object, ExpirationServiceMock.Object, null, null);

        }

        private void RegisterCongigurationRepo(List<VTasksConfiguration> list)
        {
            RegisterViewRepository<IVTasksConfigurationRepository, VTasksConfiguration>(list.AsQueryable());
        }

        [Fact]
        public void ModelIsNull()
        {
            // Act
            var result = _service.SaveService(null, DefaultVersion, false);

            // Assert
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Never());
        }

        [Fact]
        public void RightVersionNotFound()
        {
            // Act
            var result = _service.SaveService(new VmOpenApiServiceInVersionBase { Id = _serviceId }, DefaultVersion, false);

            // Assert
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Never());
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
                VersionId = _serviceVersionedId,
                GeneralDescriptionId = gdId.ToString()
            };
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            RegisterCongigurationRepo(new List<VTasksConfiguration>());

            gdServiceMock.Setup(g => g.GetGeneralDescriptionSimple(unitOfWorkMockSetup.Object, It.IsAny<Guid>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });
            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });

            // Act
            var result = _service.SaveService(vm, DefaultVersion, true);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.GeneralDescriptionId.Should().Be(gdId);
            vmResult.Type.Should().Be(gdType);
            vmResult.ServiceChargeType.Should().BeNull(); // service charge type is not gotten from GD (in GET) so should be null
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
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
                VersionId = _serviceVersionedId,
                GeneralDescriptionId = gdId.ToString()
            };
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            RegisterCongigurationRepo(new List<VTasksConfiguration>());

            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    ServiceChannels = new List<V6VmOpenApiServiceServiceChannel>
                    {
                        new V6VmOpenApiServiceServiceChannel{ServiceChannel = new VmOpenApiItem{Id = _channelId}, Description = descriptions}
                    }
                });

            // Act
            var result = _service.SaveService(vm, DefaultVersion, true);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.GeneralDescriptionId.Should().Be(gdId);
            vmResult.ServiceChannels.Should().NotBeNull();
            vmResult.ServiceChannels.Count.Should().Be(1);
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
        }

    }
}
