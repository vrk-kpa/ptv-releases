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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelsByOrganizationTests : ChannelServiceTestBase
    {
        private Guid _orgId;
        private IChannelService _service;

        public GetServiceChannelsByOrganizationTests()
        {
            _orgId = Guid.NewGuid();

            SetupTypesCacheMock<ServiceChannelType>();

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache,
                VersioningManager, UserOrganizationChecker, LanguageOrderCache, null, null, null, null, null);
        }

        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.ServiceLocation)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        [InlineData(null)]
        public void CanGetChannels(ServiceChannelTypeEnum? type)
        {
            // Arrange
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<ServiceChannelVersioned>>(),
                It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
                It.IsAny<bool>()
                )).Returns((IQueryable<ServiceChannelVersioned> channelServices, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
                {
                    return channelServices;
                });
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                   It.IsAny<IQueryable<ServiceServiceChannel>>(),
                   It.IsAny<Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>>>(),
                   It.IsAny<bool>()
                   )).Returns((IQueryable<ServiceServiceChannel> connections, Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>> func, bool applyFilters) =>
                   {
                       return connections;
                   });
            var entity = GetEntityAndArrangeForGetByList(type);
            entity.OrganizationId = _orgId;
            var list = new List<ServiceChannelVersioned> { entity };
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());
            ArrangeAllTranslationManager(type);

            // Act
            var result = _service.GetServiceChannelsByOrganization(_orgId, null, DefaultVersion, false, type);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            ChannelRepoMock.Verify(x => x.All(), Times.Once);
            if (type == ServiceChannelTypeEnum.EChannel)
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            else
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            if (type == ServiceChannelTypeEnum.Phone || !type.HasValue)
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            else
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            if (type == ServiceChannelTypeEnum.ServiceLocation)
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            else
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            if (type == ServiceChannelTypeEnum.PrintableForm)
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            else
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            if (type == ServiceChannelTypeEnum.WebPage)
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Once);
            else
                translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
        }

        [Fact]
        public void MoreThan100WouldBeReturned()
        {
            // Arrange
            var type = ServiceChannelTypeEnum.ServiceLocation;
            var list = new List<ServiceChannelVersioned>();
            for (var i = 0; i < 101; i++)
            {
                var item = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
                item.TypeId = TypeCache.Get<ServiceChannelType>(type.ToString());
                item.OrganizationId = _orgId;
                list.Add(item);
            }
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            Action act = () => _service.GetServiceChannelsByOrganization(_orgId, null, DefaultVersion, false);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void DateDefinedAsNow()
        {
            var date = DateTime.Now;
            var type = ServiceChannelTypeEnum.WebPage;
            var entity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            entity.TypeId = TypeCache.Get<ServiceChannelType>(type.ToString());
            entity.OrganizationId = _orgId;
            var list = new List<ServiceChannelVersioned> { entity };
            ChannelRepoMock.Setup(g => g.All()).Returns(list.AsQueryable());

            // Act
            var result = _service.GetServiceChannelsByOrganization(_orgId, date, DefaultVersion, false, type);

            // Assert
            result.Should().BeNull();
        }
    }
}
