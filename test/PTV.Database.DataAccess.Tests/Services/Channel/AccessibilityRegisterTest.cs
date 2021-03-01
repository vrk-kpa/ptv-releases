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
using Microsoft.Extensions.Options;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Channel
{
    /// <summary>
    /// Tests focused on accessibility register operations
    /// </summary>
    public class AccessibilityRegisterTest : TestBase
    {
        //private readonly ChannelService channelService;
        private readonly AccessibilityRegisterService accessibilityRegisterService;
        private readonly Mock<IPahaTokenAccessor> userIdentificationMock;

        private readonly AccessibilityRegisterSettings accessibilityRegisterSettings = new AccessibilityRegisterSettings
        {
            ChecksumSecret = "18c4d014-02f0-48cf-959a-1970d6a55352",
            SystemId = "ab6e2755-19a2-45b4-b5cd-484098b6c511",
            BaseUrl = "https://asiointi.hel.fi/kapaesteettomyys_testi/",
            CreateAccessibilityRegisterUrl = "app/{10}/ServicePoint/?systemId={0}&servicePointId={1}&user={2}&validUntil={3}&name={4}&streetAddress={5}&postOffice={6}&northing={7}&easting={8}&checksum={9}",
            UrlCreateLinkValidityInDays = 14
        };

        public AccessibilityRegisterTest()
        {
            var accessibilityRegisterSettingsMock = new Mock<IOptions<AccessibilityRegisterSettings>>();

            userIdentificationMock = new Mock<IPahaTokenAccessor>();
            accessibilityRegisterSettingsMock.Setup(s => s.Value).Returns(accessibilityRegisterSettings);

            SetupTypesCacheMock<CoordinateType>();

            accessibilityRegisterService = new AccessibilityRegisterService
                (
                null,
                null,
                CacheManager.PublishingStatusCache,
                null,
                accessibilityRegisterSettingsMock.Object,
                userIdentificationMock.Object,
                CacheManager,
                CloningManager,
                null,
                null,
                null,
                null
                );
        }

        [Theory]
        [InlineData("fi", "test@user.fi")]
        [InlineData("sv", "test@user.fi")]
        [InlineData("en", "test@user.fi")]
        public void CreateBaseAccessibilityRegister(
            string dataLanguage,
            string userName
            )
        {
            var channel = CreateServiceChannelVersioned(false,"fi", "sv", "en");
            var address = channel.Addresses.Single().Address;
            var addressPoint = address.ClsAddressPoints.Single();

            RegisterRepository<IServiceChannelVersionedRepository, ServiceChannelVersioned>(new List<ServiceChannelVersioned>{channel}.AsQueryable());
            RegisterRepository<IAddressRepository, Address>(new List<Address>{address}.AsQueryable());

            userIdentificationMock.Setup(ui => ui.UserName).Returns(userName);
            CloningManagerMock.Setup(cm => cm.CloneEntity(It.IsAny<Address>(), It.IsAny<IUnitOfWork>())).Returns(address);

            var taskRepoMock = new Mock<IAccessibilityRegisterRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IAccessibilityRegisterRepository>()).Returns(taskRepoMock.Object);

            var accessibilityRegister = accessibilityRegisterService.SetAccessibilityRegister(unitOfWorkMockSetup.Object, new VmAccessibilityRegisterSetIn
            {
                ServiceChannelVersionedId = channel.Id,
                AddressId = address.Id,
                LanguageCode = dataLanguage
            });

            Assert.NotNull(accessibilityRegister);

            var parsedUrl = ParseUrl(accessibilityRegister.AccessibilityRegister.Url);
            var defaultLanguageId = LanguageCache.Get(DomainConstants.DefaultLanguage);

            parsedUrl.SystemId.Should().Be(accessibilityRegisterSettings.SystemId);
            parsedUrl.ServicePointId.Should().Be(channel.UnificRootId.ToString());
            ValidateUrlDate(parsedUrl.ValidUntil, DateTime.Today.AddDays(accessibilityRegisterSettings.UrlCreateLinkValidityInDays));
            parsedUrl.Name.Should().Be(channel.ServiceChannelNames.Single(scn => scn.LocalizationId == defaultLanguageId).Name);
            parsedUrl.PostOffice.Should().Be(addressPoint.PostalCode.PostalCodeNames.Single(scn => scn.LocalizationId == defaultLanguageId).Name);
            parsedUrl.Northing.Should().Be(Convert.ToInt32(address.Coordinates.Single().Latitude).ToString());
            parsedUrl.Easting.Should().Be(Convert.ToInt32(address.Coordinates.Single().Longitude).ToString());
        }

        private static void ValidateUrlDate(string stringDate, DateTime date)
        {
            if (DateTime.TryParse(stringDate, out var parsedDate)) parsedDate.Should().Be(date);
            else throw new Exception("Date parsing error!");
        }

        private PostalCode CreatePostalCode(string postalCode, bool createDefault, params string[] languageCodes)
        {
            var postalCodeNames = new List<PostalCodeName>();
            languageCodes.ForEach(languageCode =>
            {
                var languageId = CacheManager.LanguageCache.Get(languageCode);
                if (PostalCodes_00270.ContainsKey(languageCode))
                    postalCodeNames.Add(new PostalCodeName {LocalizationId = languageId, Name = PostalCodes_00270[languageCode]});
                else if (createDefault)
                    postalCodeNames.Add(new PostalCodeName {LocalizationId = languageId, Name = $"PostalCodeName-{languageCode}"});
            });

            return new PostalCode {
                Id = Guid.NewGuid(),
                Code = postalCode,
                PostalCodeNames = postalCodeNames
            };
        }

        private List<ServiceChannelName> CreateServiceChannelNames(string defaultChannelName, params string[] languageCodes)
        {
            var serviceChannelNames = new List<ServiceChannelName>();
            languageCodes.ForEach(languageCode =>
            {
                serviceChannelNames.Add(new ServiceChannelName {LocalizationId = CacheManager.LanguageCache.Get(languageCode), Name = $"{defaultChannelName}-{languageCode}"});
            });

            return serviceChannelNames;
        }

        private List<ClsAddressStreetName> CreateStreetNames(bool createDefault, params string[] languageCodes)
        {
            var streetNames = new List<ClsAddressStreetName>();
            languageCodes.ForEach(languageCode =>
            {
                var languageId = CacheManager.LanguageCache.Get(languageCode);
                if (StreetNames_Vesikuja.ContainsKey(languageCode))
                    streetNames.Add(new ClsAddressStreetName {LocalizationId = languageId, Name = StreetNames_Vesikuja[languageCode]});
                else if (createDefault)
                    streetNames.Add(new ClsAddressStreetName {LocalizationId = languageId, Name = $"StreetName-{languageCode}"});
            });
            return streetNames;
        }

        private ServiceChannelVersioned CreateServiceChannelVersioned(bool createDefault, params string[] languageCodes)
        {
            var postalCode = CreatePostalCode("00270", createDefault, languageCodes);

            var addressPoint = new ClsAddressPoint
            {
                AddressStreet = new ClsAddressStreet
                {
                    Id = Guid.NewGuid(),
                    StreetNames = CreateStreetNames(createDefault, languageCodes)
                },

                PostalCodeId = postalCode.Id,
                PostalCode = postalCode,
                StreetNumber = "9"
            };

            var address = new Address
            {
                Id = Guid.NewGuid(),
                UniqueId = Guid.NewGuid(),
                ClsAddressPoints = new List<ClsAddressPoint>{ addressPoint },
                Coordinates = new List<AddressCoordinate>
                {
                    new AddressCoordinate
                    {
                        CoordinateState = CoordinateStates.Ok.ToString(),
                        Latitude = 6675172.647,
                        Longitude = 383235.93,
                        TypeId = CoordinateTypeEnum.Main.ToString().GetGuid(),
                    }
                }
            };

            addressPoint.Address = address;

            return new ServiceChannelVersioned
            {
                Id = Guid.NewGuid(),
                UnificRootId = Guid.NewGuid(),

                Addresses = new List<ServiceChannelAddress>
                {
                    new ServiceChannelAddress {AddressId = address.Id, Address = address}
                },

                ServiceChannelNames = CreateServiceChannelNames("ServiceChannelName", languageCodes)
            };
        }

        private static UrlParams ParseUrl(string url)
        {

            var uri = new Uri(url);
            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            return new UrlParams
            {
                SystemId = queryParams.ContainsKey("SystemId") ? (string) queryParams["SystemId"] : null,
                ServicePointId = queryParams.ContainsKey("ServicePointId") ? (string) queryParams["ServicePointId"] : null,
                UserName = queryParams.ContainsKey("UserName") ? (string) queryParams["UserName"] : null,
                ValidUntil = queryParams.ContainsKey("ValidUntil") ? (string) queryParams["ValidUntil"] : null,
                Name = queryParams.ContainsKey("Name") ? (string) queryParams["Name"] : null,
                StreetAddress = queryParams.ContainsKey("StreetAddress") ? (string) queryParams["StreetAddress"] : null,
                PostOffice = queryParams.ContainsKey("PostOffice") ? (string) queryParams["PostOffice"] : null,
                Northing = queryParams.ContainsKey("Northing") ? (string) queryParams["Northing"] : null,
                Easting = queryParams.ContainsKey("Easting") ? (string) queryParams["Easting"] : null,
                Checksum = queryParams.ContainsKey("Checksum") ? (string) queryParams["Checksum"] : null
            };
        }

        private readonly Dictionary<string, string> PostalCodes_00270 = new Dictionary<string, string>
        {
            {"fi", "HELSINKI"},
            {"sv", "HELSINGFORS"},
            {"en", "HELSINKI"},
        };

        private readonly Dictionary<string, string> StreetNames_Vesikuja = new Dictionary<string, string>
        {
            {"fi", "Vesikuja"},
            {"sv", "Vattengränden"},
        };

        private class UrlParams
        {
            public string SystemId { get; set; }
            public string ServicePointId { get; set; }
            public string UserName { get; set; }
            public string ValidUntil { get; set; }
            public string Name { get; set; }
            public string StreetAddress { get; set; }
            public string PostOffice { get; set; }
            public string Northing { get; set; }
            public string Easting { get; set; }
            public string Checksum { get; set; }
        }

    }

}
