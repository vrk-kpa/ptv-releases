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
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class AddServiceChannelTests : ChannelServiceTestBase
    {
        private List<ServiceChannelVersioned> list;
        private ServiceChannelVersioned publishedEntity;
        private string sourceId = "sourceId";
        private VmOpenApiServiceLocationChannelInVersionBase vm;

        public AddServiceChannelTests()
        {
            SetupTypesCacheMock<ServiceChannelType>();
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            list = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            publishedEntity = list.Where(i => i.PublishingStatusId == PublishedId).FirstOrDefault();
            vm = new VmOpenApiServiceLocationChannelInVersionBase()
            {
                SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString()
            };
        }

        [Fact]
        public void Add_CannotGetUser()
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
            Action act = () => service.AddServiceChannel(new VmOpenApiServiceChannelIn(), false, DefaultVersion, null);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void Add_ExternalSourceExists()
        {
            // Arrange
            var sourceId = "sourceId";
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);
            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId, RelationId = USERNAME, ObjectType = typeof(Model.Models.ServiceChannel).Name }
                }.AsQueryable());
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, ApplicationConfigurationMock, ValidationManagerMock, CacheManagerMock.Object);

            var service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, DataUtils,
                AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker);

            // Act
            Action act = () => service.AddServiceChannel(new VmOpenApiServiceChannelIn() { SourceId = sourceId }, false, DefaultVersion, null);

            // Assert
            act.ShouldThrowExactly<ExternalSourceExistsException>(string.Format(CoreMessages.OpenApi.ExternalSourceExists, sourceId));
        }

        [Fact]
        public void Add_ElectronicChannel()
        {
            // Arrange
            var service = Arrange(vm, ServiceChannelTypeEnum.EChannel);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiElectronicChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void Add_PhoneChannel()
        {
            // Arrange
            var service = Arrange(vm, ServiceChannelTypeEnum.Phone);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiPhoneChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void Add_PrintableFormChannel()
        {
            // Arrange
            var service = Arrange(vm, ServiceChannelTypeEnum.PrintableForm);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiPrintableFormChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void Add_ServiceLocationChannel()
        {
            // Arrange
            var service = Arrange(vm, ServiceChannelTypeEnum.ServiceLocation);
            
            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiServiceLocationChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void Add_WebPageChannel()
        {
            // Arrange
            var service = Arrange(vm, ServiceChannelTypeEnum.WebPage);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiWebPageChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        private ChannelService Arrange(IVmOpenApiServiceChannelIn vm, ServiceChannelTypeEnum channelType)
        {
            var unitOfWork = unitOfWorkMockSetup.Object;
            
            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId + "2", RelationId = USERNAME, ObjectType = typeof(ServiceChannel).Name }
                }.AsQueryable()); // does not return same source id

            translationManagerVModelMockSetup.Setup(t => t.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(vm, unitOfWork))
                .Returns(publishedEntity);
            translationManagerVModelMockSetup.Setup(t => t.TranslateAll<VmOpenApiConnection, ServiceServiceChannel>(It.IsAny<List<VmOpenApiConnection>>(), unitOfWork))
                .Returns(new List<ServiceServiceChannel>());

            ArrangeTranslationManager(channelType);
            
            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            return ArrangeForGet(new List<ServiceChannelVersioned> { publishedEntity }, channelType);
        }
    }
}
