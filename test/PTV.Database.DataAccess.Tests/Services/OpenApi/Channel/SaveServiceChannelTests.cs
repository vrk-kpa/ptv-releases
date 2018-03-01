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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class SaveServiceChannelTests : ChannelServiceTestBase
    {
        private Guid _channelId;
        private Guid _channelVersionedId;

        private ServiceChannelVersioned _channelVersioned;

        public SaveServiceChannelTests()
        {
            _channelId = Guid.NewGuid();
            _channelVersionedId = Guid.NewGuid();

            SetupTypesCacheMock<ServiceChannelType>();
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            _channelVersioned = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishedId, _channelId, _channelVersionedId);
            _channelVersioned.UnificRoot = new ServiceChannel { Id = _channelId, Versions = new List<ServiceChannelVersioned> { _channelVersioned } };
            
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

            var service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            var result = service.SaveServiceChannel((IVmOpenApiServiceChannelIn)null, false, DefaultVersion);

            // Assert
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWork), Times.Never());
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

            var service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            Action act = () => service.SaveServiceChannel(new VmOpenApiServiceChannelIn(), false, DefaultVersion);

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

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWork, _channelId, null, false))
                .Returns((Guid?)null);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            var result = service.SaveServiceChannel(new VmOpenApiServiceChannelIn { Id = _channelId }, false, DefaultVersion);

            // Assert
            result.Should().BeNull();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceInVersionBase, ServiceVersioned>(It.IsAny<VmOpenApiServiceInVersionBase>(), unitOfWork), Times.Never());
        }

        [Fact]
        public void ExternalSourceNotExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var entityName = typeof(ServiceChannel).Name;
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId, RelationId = USERNAME, ObjectType = entityName }
                }.AsQueryable());

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWork, _channelId, null, false))
                .Returns((Guid?)null);

            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            Action act = () => service.SaveServiceChannel(new VmOpenApiServiceChannelIn { SourceId = sourceId }, false, DefaultVersion);

            // Assert
            act.ShouldThrowExactly<Exception>(string.Format(CoreMessages.OpenApi.EntityNotFound, entityName, sourceId));
        }

        [Fact]
        public void ExternalSourceExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var entityName = typeof(ServiceChannel).Name;
            var unitOfWork = unitOfWorkMockSetup.Object;

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId, RelationId = USERNAME, ObjectType = entityName, PTVId = _channelId }
                }.AsQueryable());
        
            var service = Arrange(ServiceChannelTypeEnum.EChannel);

            // Act
            var result = service.SaveServiceChannel(new VmOpenApiServiceChannelIn { SourceId = sourceId }, false, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiElectronicChannel>();
        }

        [Fact]
        public void CanSaveElectronicChannel()
        {
            // Arrange            
            var service = Arrange(ServiceChannelTypeEnum.EChannel);
            var vm = new VmOpenApiElectronicChannelInVersionBase { Id = _channelId, PublishingStatus = PublishingStatus.Published.ToString() };

            // Act
            var result = service.SaveServiceChannel(vm, false, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiElectronicChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(_channelVersionedId, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanSavePhoneChannel()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.Phone);
            var vm = new VmOpenApiPhoneChannelInVersionBase { Id = _channelId, PublishingStatus = PublishingStatus.Published.ToString() };

            // Act
            var result = service.SaveServiceChannel(vm, false, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiPhoneChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(_channelVersionedId, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanSavePrintableFormChannel()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.PrintableForm);
            var vm = new VmOpenApiPrintableFormChannelInVersionBase { Id = _channelId, PublishingStatus = PublishingStatus.Published.ToString() };

            // Act
            var result = service.SaveServiceChannel(vm, false, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiPrintableFormChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(_channelVersionedId, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanSaveServiceLocationChannel()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.ServiceLocation);
            var vm = new VmOpenApiServiceLocationChannelInVersionBase { Id = _channelId, PublishingStatus = PublishingStatus.Published.ToString() };

            // Act
            var result = service.SaveServiceChannel(vm, false, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiServiceLocationChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(_channelVersionedId, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanSaveWebPageChannel()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.WebPage);
            var vm = new VmOpenApiWebPageChannelInVersionBase { Id = _channelId, PublishingStatus = PublishingStatus.Published.ToString() };

            // Act
            var result = service.SaveServiceChannel(vm, false, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiWebPageChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(_channelVersionedId, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        private ChannelService Arrange(ServiceChannelTypeEnum channelType)
        {
            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);
            
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, _channelId, null, false))
                .Returns(_channelVersionedId);

            // for PUT/SAVE
            translationManagerVModelMockSetup.Setup(t => t.Translate<VmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((VmOpenApiServiceChannelIn x, IUnitOfWorkWritable y) =>
                {

                    return _channelVersioned;
                });

            return ArrangeForGet(new List<ServiceChannelVersioned> { _channelVersioned }, channelType);
        }
    }
}
