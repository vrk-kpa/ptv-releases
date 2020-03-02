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
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Framework;
using Xunit;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class GetServiceChannelByIdTests : ChannelServiceTestBase
    {
        private List<ServiceChannelVersioned> _channelList;
        private ServiceChannelVersioned _publishedChannel;
        private Guid _publishedChannelRootId;

        public GetServiceChannelByIdTests()
        {
            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));
            SetupTypesCacheMock<AreaInformationType>(typeof(AreaInformationTypeEnum));
            SetupTypesCacheMock<AreaType>(typeof(AreaTypeEnum));
            SetupTypesCacheMock<ServiceHourType>(typeof(ServiceHoursTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));
            SetupTypesCacheMock<PhoneNumberType>(typeof(PhoneNumberTypeEnum));

            _channelList = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            _publishedChannel = _channelList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            _publishedChannelRootId = _publishedChannel.UnificRootId;
        }

        [Theory]
        [InlineData(VersionStatusEnum.Published)]
        [InlineData(VersionStatusEnum.Latest)]
        [InlineData(VersionStatusEnum.LatestActive)]
        public void RightVersionNotFound(VersionStatusEnum status)
        {
            // Arrange
            var id = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, true)).Returns((Guid?)null);

            var service = new ChannelService(contextManager, translationManagerMockSetup.Object,
                TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache,
                VersioningManager, UserOrganizationChecker, LanguageOrderCache, null, null, null);

            // Act
            var result = service.GetServiceChannelById(id, DefaultVersion, status);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ElectronicChannelCanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.EChannel);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
            var vmResult = Assert.IsType<V11VmOpenApiElectronicChannel>(result);
            vmResult.ServiceChannelNames.FirstOrDefault(n => n.Type == NameTypeEnum.Name.GetOpenApiValue()).Should().NotBeNull();
            vmResult.ServiceChannelDescriptions.FirstOrDefault(n => n.Type == "Summary").Should().NotBeNull();
            vmResult.AreaType.Should().Be(AreaInformationTypeEnum.AreaType.GetOpenApiValue());
            vmResult.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "Other").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "DaysOfTheWeek").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "Exceptional").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "OverMidnight").Should().NotBeNull();
            vmResult.WebPages.Count.Should().Be(1);
        }

        [Fact]
        public void PhoneChannelCanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.Phone);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
            var vmResult = Assert.IsType<V11VmOpenApiPhoneChannel>(result);
            vmResult.ServiceChannelNames.FirstOrDefault(n => n.Type == NameTypeEnum.Name.GetOpenApiValue()).Should().NotBeNull();
            vmResult.ServiceChannelDescriptions.FirstOrDefault(n => n.Type == "Summary").Should().NotBeNull();
            vmResult.AreaType.Should().Be(AreaInformationTypeEnum.AreaType.GetOpenApiValue());
            vmResult.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
            vmResult.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            vmResult.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
            vmResult.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Other").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "DaysOfTheWeek").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "Exceptional").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "OverMidnight").Should().NotBeNull();
            vmResult.WebPages.Count.Should().Be(1);
        }

        [Fact]
        public void PrintableFormChannelCanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.PrintableForm);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
            var vmResult = Assert.IsType<V11VmOpenApiPrintableFormChannel>(result);
            vmResult.ServiceChannelNames.FirstOrDefault(n => n.Type == NameTypeEnum.Name.GetOpenApiValue()).Should().NotBeNull();
            vmResult.ServiceChannelDescriptions.FirstOrDefault(n => n.Type == "Summary").Should().NotBeNull();
            vmResult.AreaType.Should().Be(AreaInformationTypeEnum.AreaType.GetOpenApiValue());
            vmResult.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "Other").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "DaysOfTheWeek").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "Exceptional").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "OverMidnight").Should().NotBeNull();
            vmResult.WebPages.Count.Should().Be(1);
        }

        [Fact]
        public void ServiceLocationChannelCanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.ServiceLocation);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
            var vmResult = Assert.IsType<V11VmOpenApiServiceLocationChannel>(result);
            vmResult.ServiceChannelNames.FirstOrDefault(n => n.Type == NameTypeEnum.Name.GetOpenApiValue()).Should().NotBeNull();
            vmResult.ServiceChannelDescriptions.FirstOrDefault(n => n.Type == "Summary").Should().NotBeNull();
            vmResult.AreaType.Should().Be(AreaInformationTypeEnum.AreaType.GetOpenApiValue());
            vmResult.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
            vmResult.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            vmResult.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
            vmResult.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Other").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "DaysOfTheWeek").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "Exceptional").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "OverMidnight").Should().NotBeNull();
            vmResult.WebPages.Count.Should().Be(1);
        }

        [Fact]
        public void WebPageChannelCanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.WebPage);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once);
            var vmResult = Assert.IsType<V11VmOpenApiWebPageChannel>(result);
            vmResult.ServiceChannelNames.FirstOrDefault(n => n.Type == NameTypeEnum.Name.GetOpenApiValue()).Should().NotBeNull();
            vmResult.ServiceChannelDescriptions.FirstOrDefault(n => n.Type == "Summary").Should().NotBeNull();
            vmResult.AreaType.Should().Be(AreaInformationTypeEnum.AreaType.GetOpenApiValue());
            vmResult.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
            vmResult.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
            vmResult.SupportPhones.FirstOrDefault(p => p.ServiceChargeType == "Other").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "DaysOfTheWeek").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "Exceptional").Should().NotBeNull();
            vmResult.ServiceHours.FirstOrDefault(h => h.ServiceHourType == "OverMidnight").Should().NotBeNull();
            vmResult.WebPages.Count.Should().Be(1);
        }

        [Fact]
        public void InterfaceVersion11CanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.Phone);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, 11, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V11VmOpenApiPhoneChannel>();
        }

        [Fact]
        public void InterfaceVersion10CanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.WebPage);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, 10, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V10VmOpenApiWebPageChannel>();
        }

        [Fact]
        public void InterfaceVersion9CanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.ServiceLocation);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, 9, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V9VmOpenApiServiceLocationChannel>();
        }

        [Fact]
        public void InterfaceVersion8CanBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.EChannel);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, 8, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V8VmOpenApiElectronicChannel>();
        }

        [Fact]
        public void InterfaceVersion7CannotBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.Phone);

            // Act
            Action act = () => service.GetServiceChannelById(_publishedChannelRootId, 7, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion6CannotBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.Phone);

            // Act
            Action act = () => service.GetServiceChannelById(_publishedChannelRootId, 6, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion5CannotBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.PrintableForm);

            // Act
            Action act = () => service.GetServiceChannelById(_publishedChannelRootId, 5, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion4CannotBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.ServiceLocation);

            // Act
            Action act = () => service.GetServiceChannelById(_publishedChannelRootId, 4, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion3CannotBeFetched()
        {
            // Arrange
            var service = Arrange(ServiceChannelTypeEnum.WebPage);

            // Act
            Action act = () => service.GetServiceChannelById(_publishedChannelRootId, 3, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<ServiceChannelLanguageAvailability>());
            var publishedChannel = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var service = Arrange(ServiceChannelTypeEnum.EChannel, list);

            // Act
            var result = service.GetServiceChannelById(publishedChannel.UnificRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().BeNull();
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, publishedChannel.UnificRootId, PublishingStatus.Published, true), Times.Once);
            ChannelRepoMock.Verify(x => x.All(), Times.Once);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
            translationManagerMockSetup.Verify(x => x.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ICollection<ServiceChannelVersioned>>()), Times.Never);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestServiceChannel(PublishingStatus publishingStatus)
        {
            // Arrange
            var channelType = ServiceChannelTypeEnum.WebPage;
            var item = _channelList.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            item.TypeId = TypeCache.Get<ServiceChannelType>(channelType.ToString());
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, false)).Returns(id);
            var service = Arrange(channelType);

            // Act
            var result = service.GetServiceChannelById(rootId, DefaultVersion, VersionStatusEnum.Latest);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiWebPageChannel>(result);
            vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, false), Times.Once);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestActiveServiceChannel(PublishingStatus publishingStatus)
        {
            // Arrange
            var channelType = ServiceChannelTypeEnum.ServiceLocation;
            var item = _channelList.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            item.TypeId = TypeCache.Get<ServiceChannelType>(channelType.ToString());
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, true))
                .Returns(() =>
                {
                    if (publishingStatus == PublishingStatus.Deleted || publishingStatus == PublishingStatus.OldPublished) return null;

                    return id;
                });

            var service = Arrange(channelType);

            // Act
            var result = service.GetServiceChannelById(rootId, DefaultVersion, VersionStatusEnum.LatestActive);

            // Assert
            // Method should only return draft, modified or published versions.
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceChannelVersioned>(unitOfWorkMockSetup.Object, rootId, null, true), Times.Once);
            if (publishingStatus == PublishingStatus.Draft || publishingStatus == PublishingStatus.Modified || publishingStatus == PublishingStatus.Published)
            {
                result.Should().NotBeNull();
                var vmResult = Assert.IsType<V11VmOpenApiServiceLocationChannel>(result);
                vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            }
            else
            {
                result.Should().BeNull();
            }
        }

        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.ServiceLocation)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        public void CanGetRelatedServices(ServiceChannelTypeEnum channelType)
        {
            // Arrange
            var name = "Name";
            var connection = GetAndSetConnectionForChannel(_publishedChannel, PublishedId, name);
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.ToList().AsQueryable());
            var service = Arrange(channelType);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            result.Services.Should().NotBeNullOrEmpty();
            var resultConnection = result.Services.First();
            resultConnection.Should().NotBeNull();
            resultConnection.Service.Should().NotBeNull();
            resultConnection.Service.Name.Should().Be(name);
            resultConnection.ContactDetails.Should().NotBeNull();
            resultConnection.ContactDetails.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            resultConnection.ContactDetails.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
            resultConnection.ContactDetails.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Other").Should().NotBeNull();
        }

        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.ServiceLocation)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        public void CanGetRelatedOntologyServices(ServiceChannelTypeEnum channelType)
        {
            // Arrange
            var name = "Name";
            var connection = GetAndSetConnectionForChannel(_publishedChannel, PublishedId, name);
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.ToList().AsQueryable());
            GeneralDescriptionRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.First()
                .StatutoryServiceGeneralDescription.Versions.AsQueryable());
            var service = Arrange(channelType);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            result.OntologyTerms.Should().NotBeNullOrEmpty();
            result.OntologyTerms.Count.Should().Be(3);
            result.OntologyTerms.Select(x=>x.Id).Should().Contain("Onto1".GetGuid());
            result.OntologyTerms.Select(x=>x.Id).Should().Contain("Onto2".GetGuid());
            result.OntologyTerms.Select(x=>x.Id).Should().Contain("Onto3".GetGuid());
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServicesNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var statusId = PublishingStatusCache.Get(publishingStatus);
            var connection = GetAndSetConnectionForChannel(_publishedChannel, statusId, "Name");
            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.ToList().AsQueryable());
            var service = Arrange(ServiceChannelTypeEnum.ServiceLocation);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiServiceLocationChannel>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Services.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.ServiceLocation)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        public void OnlyPublishedNamesReturned(ServiceChannelTypeEnum channelType)
        {
            // Arrange
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            // Add only swedish language as published version
            var connection = GetAndSetConnectionForChannel(_publishedChannel, PublishedId, svName, svId);
            var sv = connection.Service.Versions.First();
            sv.ServiceNames.Add(new ServiceName { Name = fiName, LocalizationId = fiId });

            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceRepoMock.Setup(x => x.All()).Returns(connection.Service.Versions.ToList().AsQueryable());
            var service = Arrange(channelType);

            // Act
            var result = service.GetServiceChannelById(_publishedChannelRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            result.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            result.Services.Should().NotBeNullOrEmpty();
            var resultConnection = result.Services.First();
            resultConnection.Should().NotBeNull();
            resultConnection.Service.Should().NotBeNull();
            resultConnection.Service.Name.Should().Be(svName);
        }

        private ChannelService Arrange(ServiceChannelTypeEnum channelType, List<ServiceChannelVersioned> list = null)
        {
            return ArrangeForGet(list ?? _channelList, channelType);
        }
    }
}
