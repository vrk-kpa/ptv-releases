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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using Xunit;
using ChannelService = PTV.Database.DataAccess.Services.V2.ChannelService;

namespace PTV.Database.DataAccess.Tests.Services.Channel
{
    public class GetLocationChannelsByAddressTests:TestBase
    {
        private ChannelService channelService;
        private Mock<IServiceUtilities> serviceUtilitiesMock;
        private Mock<ICommonServiceInternal> commonServiceMock;
        private Mock<ITranslationEntity> translateToVmMock;
        private string myOrganization = "MyOrg";

        public GetLocationChannelsByAddressTests()
        {
            serviceUtilitiesMock = new Mock<IServiceUtilities>(MockBehavior.Strict);
            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            translateToVmMock = new Mock<ITranslationEntity>(MockBehavior.Strict);
            channelService = new ChannelService
            (
                contextManagerMock.Object, 
                null,
                translateToVmMock.Object,
                null,
                null,
                serviceUtilitiesMock.Object,
                commonServiceMock.Object,
                CacheManager,
                CacheManager.PublishingStatusCache,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
            SetupTypesCacheMock<PublishingStatusType>();
            SetupTypesCacheMock<NameType>();
            SetupTypesCacheMock<AddressCharacter>();
        }       

        private void TestSetup(List<ServiceChannelAddress> testData, VmServiceLocationChannelAddressSearch testInput, List<PostalCode> postalCodes)
        {
            SetupContextManager<object, IVmSearchBase>();
            serviceUtilitiesMock.Setup(x => x.GetUserMainOrganization()).Returns(myOrganization.GetGuid());
            commonServiceMock.Setup(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> guids) => new List<VmListItem>(guids.Select(x => new VmListItem { Id = x })));
            RegisterRepository<IServiceChannelAddressRepository, ServiceChannelAddress>(testData.AsQueryable());
            RegisterRepository<IOrganizationNameRepository, OrganizationName>(new List<OrganizationName>().AsQueryable());
            RegisterRepository<IServiceChannelNameRepository, ServiceChannelName>(new List<ServiceChannelName>().AsQueryable());
            RegisterRepository<IClsAddressStreetRepository, ClsAddressStreet>(testData
                .Select(x => x.Address.ClsAddressPoints.First().AddressStreet)
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .AsQueryable());
            RegisterRepository<IClsAddressPointRepository, ClsAddressPoint>(new List<ClsAddressPoint>().AsQueryable());
            RegisterRepository<IPostalCodeRepository, PostalCode>(postalCodes.AsQueryable());
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(new List<ServiceChannelLanguageAvailability>().AsQueryable());
            translateToVmMock.Setup(x =>
                    x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        It.IsAny<IEnumerable<ILanguageAvailability>>()))
                .Returns((IEnumerable<ILanguageAvailability> input) =>
                    input.Select(x => new VmLanguageAvailabilityInfo()).ToList());
            translateToVmMock.Setup(x =>
                    x.Translate<PostalCode, VmPostalCode>(
                        It.IsAny<PostalCode>()))
                .Returns((PostalCode input) => new VmPostalCode() { Code = input.Code });
            translateToVmMock.Setup(x =>
                    x.Translate<ClsAddressStreet, VmStreet>(It.IsAny<ClsAddressStreet>()))
                .Returns((ClsAddressStreet input) => new VmStreet { Id = input.Id });
        }

        [Theory]
        [InlineData(PublishingStatus.Published, 1)]
        [InlineData(PublishingStatus.Deleted, 0)]
        [InlineData(PublishingStatus.Draft, 1)]
        [InlineData(PublishingStatus.Modified, 1)]
        [InlineData(PublishingStatus.OldPublished, 0)]
        public void CheckByPublishingStatus(            
            PublishingStatus channelStatus,           
            int countResult)
        {
            //setup data
            var userOrgId = "OrganizationId".GetGuid();
            var testData = new List<ServiceChannelAddress>()
            {
                CreateServiceChannelAddress(
                    "street", "1", "00001", "fi", channelStatus, AddressCharacterEnum.Visiting)
            };
            var postalCodes = new List<PostalCode>() {new PostalCode() {Id = "00001".GetGuid(), Code = "00001"}};
            var testInput = new VmServiceLocationChannelAddressSearch()
            {
                Street = "street".GetGuid(),
                StreetNumber = "1",
                SortData = new List<VmSortParam>(),
                LanguageCode = "fi"
            };
            // setup test
           TestSetup(testData, testInput, postalCodes);
            
            // call service
            var result = channelService.GetLocationChannelsByAddress(testInput);
            
            // verify
            result.Should().NotBeNull();
            result.Count.Should<int>().Be(countResult);
            serviceUtilitiesMock.Verify(x => x.GetUserMainOrganization(), Times.Once);
            commonServiceMock.Verify(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            if (result.Count == 1)
            {
                translateToVmMock.Verify(
                    x => x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        It.IsAny<IEnumerable<ILanguageAvailability>>()), Times.Once);
                translateToVmMock.Verify(x => x.Translate<PostalCode, VmPostalCode>(It.IsAny<PostalCode>()),
                    Times.Once);
            }
        }
        
        [Theory]
        [InlineData("street", "1", 1)]
        [InlineData("street", "2", 0)]
        [InlineData("street1", "1", 0)]
        [InlineData("street1", "2", 0)]
        [InlineData("street", "", 0)]
        [InlineData("", "1", 0)]
        [InlineData("", "", 0)]
        public void CheckByStreet(            
            string streetName,
            string streetNumber,
            int countResult)
        {
            //setup data
            var userOrgId = "OrganizationId".GetGuid();
            var testData = new List<ServiceChannelAddress>()
            {
                CreateServiceChannelAddress(
                    "street", "1", "00001", "fi", PublishingStatus.Published, AddressCharacterEnum.Visiting)
            };
            var postalCodes = new List<PostalCode>() {new PostalCode() {Id = "00001".GetGuid(), Code = "00001"}};
            var testInput = new VmServiceLocationChannelAddressSearch()
            {
                Street = streetName.GetGuid(),
                StreetNumber = streetNumber,
                SortData = new List<VmSortParam>(),
                LanguageCode = "fi"
            };
            // setup test
            TestSetup(testData, testInput, postalCodes);
            
            // call service
            var result = channelService.GetLocationChannelsByAddress(testInput);
            
            // verify
            result.Should().NotBeNull();
            result.Count.Should<int>().Be(countResult);
            serviceUtilitiesMock.Verify(x => x.GetUserMainOrganization(), Times.Once);
            commonServiceMock.Verify(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            if (result.Count == 1)
            {
                translateToVmMock.Verify(
                    x => x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        It.IsAny<IEnumerable<ILanguageAvailability>>()), Times.Once);
                translateToVmMock.Verify(x => x.Translate<PostalCode, VmPostalCode>(It.IsAny<PostalCode>()),
                    Times.Once);
            }
        }
        
        [Theory]
        [InlineData("00001", 1)]  
        [InlineData("", 2)]  
        [InlineData("00003", 0)] 
        public void CheckByPostalCode(            
            string postalCode,
            int countResult)
        {
            //setup data
            var testData = new List<ServiceChannelAddress>()
            {
                CreateServiceChannelAddress(
                "street", "1", "00001", "fi", PublishingStatus.Published, AddressCharacterEnum.Visiting),
                CreateServiceChannelAddress(
                "street", "1", "00002", "fi", PublishingStatus.Published, AddressCharacterEnum.Visiting)

            };
            var postalCodes = new List<PostalCode>()
            {
                new PostalCode() {Id = "00001".GetGuid(), Code = "00001"},
                new PostalCode() {Id = "00002".GetGuid(), Code = "00002"}
            };
            var testInput = new VmServiceLocationChannelAddressSearch()
            {
                Street = "street".GetGuid(),
                StreetNumber = "1",
                SortData = new List<VmSortParam>(),
                LanguageCode = "fi",
                PostalCode = string.IsNullOrEmpty(postalCode) ? (Guid?)null : postalCode.GetGuid()
                
            };
            // setup test
            TestSetup(testData, testInput, postalCodes);
            
            // call service
            var result = channelService.GetLocationChannelsByAddress(testInput);
            
            // verify
            result.Should().NotBeNull();
            result.Count.Should<int>().Be(countResult);
            serviceUtilitiesMock.Verify(x => x.GetUserMainOrganization(), Times.Once);
            commonServiceMock.Verify(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            if (result.Count == 1)
            {
                translateToVmMock.Verify(
                    x => x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        It.IsAny<IEnumerable<ILanguageAvailability>>()), Times.Once);
                translateToVmMock.Verify(x => x.Translate<PostalCode, VmPostalCode>(It.IsAny<PostalCode>()),
                    Times.Once);
            }
        }
        
        [Theory]
        [InlineData(AddressCharacterEnum.Visiting, 1)]
        [InlineData(AddressCharacterEnum.Delivery, 0)]
        [InlineData(AddressCharacterEnum.Postal, 0)]
        public void CheckByAddressCharacter(            
            AddressCharacterEnum addressCharacter,           
            int countResult)
        {
            //setup data
            var userOrgId = "OrganizationId".GetGuid();
            var testData = new List<ServiceChannelAddress>()
            {
                CreateServiceChannelAddress(
                    "street", "1", "00001", "fi", PublishingStatus.Modified, addressCharacter)
            };
            var postalCodes = new List<PostalCode>() {new PostalCode() {Id = "00001".GetGuid(), Code = "00001"}};
            var testInput = new VmServiceLocationChannelAddressSearch()
            {
                Street = "street".GetGuid(),
                StreetNumber = "1",
                SortData = new List<VmSortParam>(),
                LanguageCode = "fi"
            };
            // setup test
           TestSetup(testData, testInput, postalCodes);
            
            // call service
            var result = channelService.GetLocationChannelsByAddress(testInput);
            
            // verify
            result.Should().NotBeNull();
            result.Count.Should<int>().Be(countResult);
            serviceUtilitiesMock.Verify(x => x.GetUserMainOrganization(), Times.Once);
            commonServiceMock.Verify(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            if (result.Count == 1)
            {
                translateToVmMock.Verify(
                    x => x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        It.IsAny<IEnumerable<ILanguageAvailability>>()), Times.Once);
                translateToVmMock.Verify(x => x.Translate<PostalCode, VmPostalCode>(It.IsAny<PostalCode>()),
                    Times.Once);
            }
        }
        
        [Fact]
        public void CheckResult()
        {
            //setup data
            var testData = new List<ServiceChannelAddress>()
            {
                CreateServiceChannelAddress(
                    "street", "1", "00001", "fi", PublishingStatus.Modified, AddressCharacterEnum.Visiting)
            };
            var postalCodes = new List<PostalCode>() {new PostalCode() {Id = "00001".GetGuid(), Code = "00001"}};
            var testInput = new VmServiceLocationChannelAddressSearch()
            {
                Street = "street".GetGuid(),
                StreetNumber = "1",
                SortData = new List<VmSortParam>(),
                LanguageCode = "fi",
                PostalCode = "00001".GetGuid()
            };
            // setup test
            TestSetup(testData, testInput, postalCodes);
            
            // call service
            var result = channelService.GetLocationChannelsByAddress(testInput);
            
            // verify
            result.Should().NotBeNull();
            result.Count.Should<int>().Be(1);
            serviceUtilitiesMock.Verify(x => x.GetUserMainOrganization(), Times.Once);
            commonServiceMock.Verify(x => x.GetOrganizations(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            if (result.Count == 1)
            {
                translateToVmMock.Verify(
                    x => x.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        It.IsAny<IEnumerable<ILanguageAvailability>>()), Times.Once);
                translateToVmMock.Verify(x => x.Translate<PostalCode, VmPostalCode>(It.IsAny<PostalCode>()),
                    Times.Once);
            }

            var resultOutput = result as VmServiceLocationChannelAddressSearchResult;
            resultOutput.Should().NotBeNull();
            resultOutput.SearchResult.Should().NotBeEmpty();
            resultOutput.SearchResult.First().PostalCode.Code.Should().Be("00001");
            resultOutput.SearchResult.First().OrganizationId.Should().Be(myOrganization.GetGuid());
        }

        private ServiceChannelAddress CreateServiceChannelAddress(
            string streetName, 
            string streetNumber, 
            string postalCode,
            string languageCode,
            PublishingStatus channelStatus,
            AddressCharacterEnum addressCharacter)
        {
            return new ServiceChannelAddress()
            {
                CharacterId = addressCharacter.ToString().GetGuid(),
                ServiceChannelVersioned = new ServiceChannelVersioned()
                {
                    OrganizationId = myOrganization.GetGuid(),
                    PublishingStatusId = channelStatus.ToString().GetGuid(),
                    ServiceChannelNames = new List<ServiceChannelName>()
                    {
                        new ServiceChannelName()
                        {
                            Name = "testChannel",
                            TypeId = NameTypeEnum.Name.ToString().GetGuid(),
                            LocalizationId = languageCode.GetGuid(),
                            Localization = new Language()
                            {
                                OrderNumber = 1,
                                Id = languageCode.GetGuid()
                            }
                        }
                    }
                },
                Address = CreateAddress(streetName, streetNumber, postalCode, languageCode) //CreateAddress("street1", "1", "000001", "fi")
            };
        }

        private Address CreateAddress(string streetName, string streetNumber, string postalCode, string languageCode)
        {
            var streetId = streetName.GetGuid();
            
            return new Address()
            {
                Id = streetId,
                ClsAddressPoints = new List<ClsAddressPoint>
                {
                    new ClsAddressPoint
                    {
                        Id = streetId,
                        PostalCode = new PostalCode { Code = postalCode },
                        PostalCodeId = postalCode.GetGuid(),
                        StreetNumber = streetNumber,
                        AddressStreetId = streetId,
                        AddressStreet = new ClsAddressStreet
                        {
                            Id = streetId,
                            StreetNames = new List<ClsAddressStreetName>
                            {
                                new ClsAddressStreetName
                                {
                                    ClsAddressStreetId = streetId,
                                    Name = streetName,
                                    LocalizationId = languageCode.GetGuid()
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}