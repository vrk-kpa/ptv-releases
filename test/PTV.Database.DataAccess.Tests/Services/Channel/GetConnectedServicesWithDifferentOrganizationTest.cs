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

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using PTV.Framework.Exceptions.DataAccess;
using Xunit;
using ChannelService = PTV.Database.DataAccess.Services.V2.ChannelService;

namespace PTV.Database.DataAccess.Tests.Services.Channel
{
    public class GetConnectedServicesWithDifferentOrganizationTest : TestBase
    {

        private readonly ChannelService channelService;
        private readonly Mock<IVersioningManager> versioningManagerMock;
        private readonly Mock<ITranslationEntity> translateToVmMock;
        private readonly Mock<ICommonServiceInternal> commonServiceMock;

        private const string ServiceChannelVersioned = "ServiceChannelVersioned";
        private const string ServiceWithTheSameOrganization = "ServiceWithTheSameOrganization";
        private const string ServiceWithDifferentOrganization = "ServiceWithDifferentOrganization";

        private readonly Guid ChannelOrganizationId = Guid.NewGuid();
        private readonly Guid ServiceOrganizationId = Guid.NewGuid();

        private const string LanguageCodeFi = "fi";

        public GetConnectedServicesWithDifferentOrganizationTest()
        {
            versioningManagerMock = new Mock<IVersioningManager>(MockBehavior.Strict);
            translateToVmMock = new Mock<ITranslationEntity>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);

            channelService = new ChannelService
            (
                contextManagerMock.Object,
                translateToVmMock.Object,
                null,
                null, //serviceUtilitiesMock.Object,
                commonServiceMock.Object,
                CacheManager,
                CacheManager.PublishingStatusCache,
                null,
                null,
                null,
                versioningManagerMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
        }

        private void BaseTestSetup(ServiceChannelVersioned channel)
        {
            SetupContextManager<object, IVmSearchBase>();
            SetupContextManager<object, ServiceChannelVersioned>();
            RegisterRepository<IRepository<ServiceChannelVersioned>, ServiceChannelVersioned>(new List<ServiceChannelVersioned> {channel}.AsQueryable());
        }

        [Fact]
        public void NotExistingChannelTest()
        {
            BaseTestSetup(CreateServiceChannelVersioned());
            Action action = () => channelService.GetConnectedServicesWithDifferentOrganization(new VmConnectionsInput {Id = Guid.NewGuid()});
            action.Should().ThrowExactly<EntityNotFoundException>();
        }

        [Fact]
        public void NoServiceForChannelTest()
        {
            BaseTestSetup(CreateServiceChannelVersioned());
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(new List<ServiceServiceChannel> {new ServiceServiceChannel {ServiceId = Guid.NewGuid(), ServiceChannelId = Guid.NewGuid()}}.AsQueryable());
            Assert.Null(channelService.GetConnectedServicesWithDifferentOrganization(new VmConnectionsInput {Id = GetVersionId(ServiceChannelVersioned)}));
        }

        [Fact]
        public void ChannelIdIsNullTest()
        {
            SetupContextManager<object, IVmSearchBase>();
            Assert.Null(channelService.GetConnectedServicesWithDifferentOrganization(new VmConnectionsInput{Id = Guid.Empty}));
        }

        [Fact]
        public void CheckResult()
        {
            var channel = CreateServiceChannelVersioned();
            var serviceWithTheSameOrganization = CreateServiceVersioned(ServiceWithTheSameOrganization, ChannelOrganizationId);
            var serviceWithDifferentOrganization = CreateServiceVersioned(ServiceWithDifferentOrganization, ServiceOrganizationId);

            BaseTestSetup(channel);

            var testData = new List<ServiceServiceChannel>
            {
                new ServiceServiceChannel {ServiceChannelId = channel.UnificRootId, ServiceId = serviceWithTheSameOrganization.UnificRootId},
                new ServiceServiceChannel {ServiceChannelId = channel.UnificRootId, ServiceId = serviceWithDifferentOrganization.UnificRootId}
            };

            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(testData.AsQueryable());
            RegisterRepository<IServiceVersionedRepository, ServiceVersioned>(new List<ServiceVersioned>{serviceWithTheSameOrganization, serviceWithDifferentOrganization}.AsQueryable());

            commonServiceMock.Setup(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> ids) => new List<VmListItem>(ids.Select(x => new VmListItem { Id = x })));

            versioningManagerMock.Setup(x => x.GetLastPublishedModifiedDraftVersion<ServiceVersioned>(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<Guid>()))
                .Returns((ITranslationUnitOfWork uow, Guid id) => new VersionInfo{EntityId = GetVersionedId(id, serviceWithTheSameOrganization, serviceWithDifferentOrganization)});

            RegisterRepository<IServiceNameRepository, ServiceName>(new List<ServiceName>().AsQueryable());
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(new List<ServiceLanguageAvailability>().AsQueryable());

            translateToVmMock.Setup(x => x.TranslateAll<ServiceVersioned, VmConnectableService>(It.IsAny<ICollection<ServiceVersioned>>()))
                .Returns((IEnumerable<ServiceVersioned> input) => input.Select(x => new VmConnectableService
                {
                    Id = x.Id,
                    UnificRootId = x.UnificRootId,
                    OrganizationId = x.OrganizationId,
                    Name = x.ServiceNames.ToDictionary(k => k.Localization.Code, v => v.Name),
                    LanguagesAvailabilities = x.LanguageAvailabilities.Select(l => new VmLanguageAvailabilityInfo {LanguageId = l.LanguageId}).ToList()
                }).ToList());

            var testInput = new VmConnectionsInput{Id = GetVersionId(ServiceChannelVersioned)};
            var result = channelService.GetConnectedServicesWithDifferentOrganization(testInput);

            // verify
            result.Should().NotBeNull();
            result.Count.Should<int>().Be(1);
            commonServiceMock.Verify(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            translateToVmMock.Verify(x => x.TranslateAll<ServiceVersioned, VmConnectableService>(It.IsAny<ICollection<ServiceVersioned>>()), Times.Once);

            var resultOutput = result as VmServiceChannelConnectedService;
            Assert.NotNull(resultOutput);
            resultOutput.SearchResult.Should().NotBeEmpty();
            resultOutput.SearchResult.First().OrganizationId.Should().Be(ServiceOrganizationId);
        }

        private ServiceChannelVersioned CreateServiceChannelVersioned()
        {
            return new ServiceChannelVersioned
            {
                Id = GetVersionId(ServiceChannelVersioned),
                UnificRootId = GetUnificRootId(ServiceChannelVersioned),
                OrganizationId = ChannelOrganizationId
            };
        }

        private ServiceVersioned CreateServiceVersioned(string serviceName, Guid organizationId)
        {
            return new ServiceVersioned
            {
                Id = GetVersionId(serviceName),
                UnificRootId = GetVersionId(serviceName),
                OrganizationId = organizationId,
                ServiceNames = new List<ServiceName>
                {
                    new ServiceName
                    {
                        Name = serviceName,
                        TypeId = NameTypeEnum.Name.ToString().GetGuid(),
                        LocalizationId = LanguageCodeFi.GetGuid(),
                        Localization = new Language { OrderNumber = 1, Id = LanguageCodeFi.GetGuid(), Code = LanguageCodeFi}
                    }
                },
                LanguageAvailabilities = new List<ServiceLanguageAvailability>
                {
                    new ServiceLanguageAvailability
                    {
                        Language = new Language { OrderNumber = 1, Id = LanguageCodeFi.GetGuid()},
                        LanguageId = LanguageCodeFi.GetGuid()
                    }
                }
            };
        }

        private static Guid GetVersionId(string s)
        {
            return (s + "VersionId").GetGuid();
        }

        private static Guid GetUnificRootId(string s)
        {
            return (s + "UnificRootId").GetGuid();
        }

        private static Guid GetVersionedId<TEntity>(Guid unificRootId, params TEntity[] entities) where TEntity: IEntityIdentifier, IVersionedVolume
        {
            var entity = entities.FirstOrDefault(s => s.UnificRootId == unificRootId);
            if (entity != null) return entity.Id;
            throw new KeyNotFoundException("Entity with the unific root id not found.");
        }
    }
}
