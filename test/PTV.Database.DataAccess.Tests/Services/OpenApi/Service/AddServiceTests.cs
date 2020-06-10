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
using PTV.Database.DataAccess.Interfaces;
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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Views;
using PTV.Framework.ServiceManager;
using Xunit;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class AddServiceTests : ServiceServiceTestBase
    {
        private Guid _serviceId;
        private Guid _serviceVersionedId;
        private Guid _channelId;
        private Guid _channelVersionedId;

        private ServiceService _service;

        public AddServiceTests()
        {
            _serviceId = Guid.NewGuid();
            _serviceVersionedId = Guid.NewGuid();
            _channelId = Guid.NewGuid();
            _channelVersionedId = Guid.NewGuid();

            SetupTypesCacheMock<ServiceType>();
            SetupTypesCacheMock<ServiceChargeType>();
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<PublishingStatusType>(typeof(PublishingStatus));

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

            translationManagerVModelMockSetup.Setup(t => t.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<IVmOpenApiServiceInVersionBase>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((IVmOpenApiServiceInVersionBase x, IUnitOfWorkWritable y) =>
                {
                    if (!string.IsNullOrEmpty(x.GeneralDescriptionId))
                    {
                        serviceVersioned.StatutoryServiceGeneralDescriptionId = x.GeneralDescriptionId.ParseToGuid();
                    }
                    serviceVersioned.PublishingStatusId = PublishingStatusCache.Get(x.PublishingStatus);
                    if (x.ServiceNames?.Count > 0)
                    {
                        serviceVersioned.ServiceNames = new List<ServiceName>();
                        x.ServiceNames.ForEach(n =>
                            { 
                                serviceVersioned.ServiceNames.Add(new ServiceName {Name = n.Value, TypeId = TypeCache.Get<NameType>(n.Type), ServiceVersionedId = serviceVersioned.Id});
                            }
                        );
                        ServiceNameRepoMock.Setup(o => o.All()).Returns(serviceVersioned.ServiceNames.AsQueryable());
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

            translationManagerVModelMockSetup.Setup(t => t.TranslateAll<V11VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(It.IsAny<List<V11VmOpenApiServiceServiceChannelAstiInBase>>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((List<V11VmOpenApiServiceServiceChannelAstiInBase> x, IUnitOfWorkWritable y) =>
                {
                    var connections = new List<ServiceServiceChannel>();
                    x.ForEach(c =>
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
                            connections.Add(connection);
                        }
                    });
                    return connections;
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

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, _serviceId, PublishingStatus.Published, true)).Returns(_serviceVersionedId);

            ArrangeTranslateService();

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);
            var urlService = new UrlService(TranslationManagerVModel);

            _service = new ServiceService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, urlService, ExpirationServiceMock.Object);
        }

        private void RegisterCongigurationRepo(List<VTasksConfiguration> list)
        {
            RegisterViewRepository<IVTasksConfigurationRepository, VTasksConfiguration>(list.AsQueryable());
        }

        [Fact]
        public void Add_CannotGetUser()
        {
            // Arrange
            UserIdentificationMock.Setup(s => s.UserName).Returns((string)null);

            // Act
            Action act = () => _service.AddService(new VmOpenApiServiceInVersionBase(), DefaultVersion, false);

            // Assert
            act.Should().ThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void Add_ExternalSourceExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var userName = "userName";
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>
                {
                    new ExternalSource { SourceId = sourceId, RelationId = userName, ObjectType = typeof(Model.Models.Service).Name }
                }.AsQueryable());

            // Act
            Action act = () => _service.AddService(new VmOpenApiServiceInVersionBase { SourceId = sourceId }, DefaultVersion, false);

            // Assert
            act.Should().ThrowExactly<ExternalSourceExistsException>(string.Format(CoreMessages.OpenApi.ExternalSourceExists, sourceId));
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
            var vm = new VmOpenApiServiceInVersionBase
            { SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString() };

            RegisterCongigurationRepo(new List<VTasksConfiguration>());
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);
            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>
                {
                    new ExternalSource { SourceId = sourceId + "2", RelationId = userName, ObjectType = typeof(Model.Models.Service).Name }
                }.AsQueryable()); // does not return same source id
            gdServiceMock.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>(), false)).Returns(new VmOpenApiGeneralDescriptionVersionBase());


            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(_serviceVersionedId, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());


            // Act
           var result = _service.AddService(vm, DefaultVersion, false);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
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
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Draft.ToString()
            };

            RegisterCongigurationRepo(new List<VTasksConfiguration>());
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(draftEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            // Act
            var result = _service.AddService(vm, DefaultVersion, false);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Draft.ToString());
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
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
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                Type = ServiceTypeEnum.Service.ToString(),
                ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString()
            };

            RegisterCongigurationRepo(new List<VTasksConfiguration>());
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            gdServiceMock.Setup(g => g.GetGeneralDescriptionSimple(unitOfWorkMockSetup.Object, It.IsAny<Guid>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Names = gdNames,
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });
            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Names = gdNames,
                    Type = gdType,
                    ServiceChargeType = gdChargeType
                });

            // Act
            var result = _service.AddService(vm, DefaultVersion, false);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()), Times.Never());
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false), Times.Exactly(1)); // Saving
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false), Times.Exactly(1)); // Geting
            vmResult.GeneralDescriptionId.Should().Be(gdId);
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
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
            };

            RegisterCongigurationRepo(new List<VTasksConfiguration>());
            UserIdentificationMock.Setup(s => s.UserName).Returns(userName);

            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            gdServiceMock.Setup(g => g.GetGeneralDescriptionSimple(unitOfWorkMockSetup.Object, It.IsAny<Guid>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    ServiceChannels = new List<V6VmOpenApiServiceServiceChannel>
                    {
                        new V6VmOpenApiServiceServiceChannel{ServiceChannel = new VmOpenApiItem{Id = _channelId}, Description = descriptions}
                    }
                });
            gdServiceMock.Setup(g => g.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    ServiceChannels = new List<V6VmOpenApiServiceServiceChannel>
                    {
                        new V6VmOpenApiServiceServiceChannel{ServiceChannel = new VmOpenApiItem{Id = _channelId}, Description = descriptions}
                    }
                });

            // Act
            var result = _service.AddService(vm, DefaultVersion, true);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceLanguageAvailability, bool>>>()), Times.Never());
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false), Times.Exactly(1)); // Saving
            gdServiceMock.Verify(x => x.GetGeneralDescriptionVersionBase(It.IsAny<Guid>(), 0, true, It.IsAny<bool>(), false), Times.Exactly(1)); // Geting
            vmResult.GeneralDescriptionId.Should().Be(gdId);
            vmResult.ServiceChannels.Should().NotBeNull();
            vmResult.ServiceChannels.Count.Should().Be(1);
        }
    }
}
