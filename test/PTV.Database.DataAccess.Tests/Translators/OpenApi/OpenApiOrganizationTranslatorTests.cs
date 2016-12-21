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
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.DataAccess.Translators.OpenApi.Organizations;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.OpenApi.Channels;
using PTV.Database.DataAccess.Translators.OpenApi.V2;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiOrganizationTranslatorTests : TranslatorTestBase
    {
        private const string MUNICIPALITY = "Uusimaa";
        private const string DESCRIPTION_FI = "TestDescription_FI";
        private const string DESCRIPTION_SV = "TestDescription_SV";
        private const string EMAIL_FI = "some@one.fi";
        private const string EMAIL_SV = "some@one.sv";
        private const string PHONE_NUMBER = "0123456789";
        private const string PHONE_PREFIX = "3210";

        private readonly List<object> translators;
        private readonly ICacheManager cacheManager;

        private readonly Guid fiId;
        private readonly Guid svId;

        public OpenApiOrganizationTranslatorTests()
        {
            var languageCache = SetupLanguageCacheMock().Object;
            var typesCacheMock = SetupTypesCacheMock<PhoneNumberType>(typesEnum: typeof(PhoneNumberTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typesCacheMock, typeof(ServiceChargeTypeEnum));
            SetupTypesCacheMock<AddressType>(typesCacheMock, typeof(AddressTypeEnum));
            SetupTypesCacheMock<OrganizationType>(typesCacheMock, typeof(OrganizationTypeEnum));
            SetupTypesCacheMock<NameType>(typesCacheMock, typeof(NameTypeEnum));
            cacheManager = SetupCacheManager(languageCache, typesCacheMock.Object).Object;

            translators = new List<object>()
            {
                new OpenApiOrganizationTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new V2OpenApiOrganizationTranslator(ResolveManager, TranslationPrimitives),
                new OrganizationTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationNameTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationDescriptionTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChargeTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationWebPageTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new WebPageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationAddressTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new AddressTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetAddressTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new CountryCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationServiceTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new RoleTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ProvisionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationServiceWebPageTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiOrganizationInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives),
                new MunicipalityCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiBusinessTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationDescriptionInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiOrganizationWebPageInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiWebPageInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiAddressWithTypeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostalCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceChannelAttachmentTranslator(ResolveManager, TranslationPrimitives),
                new PhoneNumberTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationPhoneTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new EmailTranslator(ResolveManager, TranslationPrimitives),
                new OrganizationEmailDataTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationEmailTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationEmailEmailTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiEmailDescriptionTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiAddressDescriptionTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new OpenApiEmailTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new PhoneTranslator(ResolveManager, TranslationPrimitives, cacheManager.TypesCache),
                new OpenApiPhoneTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiOrganizationAddressWithCoordinatesTranslator(ResolveManager, TranslationPrimitives, cacheManager)
            };

            fiId = languageCache.Get(LanguageCode.fi.ToString());
            svId = languageCache.Get(LanguageCode.sv.ToString());
        }

        [Fact]
        public void TranslateOrganizationToVm()
        {
            var organization = CreateOrganization();
            var toTranslate = new List<Organization> { organization };
            var translations = RunTranslationEntityToModelTest<Organization, V2VmOpenApiOrganization>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(organization, translation);
        }

        private void CheckTranslations(Organization source, IV2VmOpenApiOrganization target)
        {
            target.OrganizationNames.Count.Should().Be(1);
            target.Municipality.Should().Be(source.Municipality.Name);
            target.OrganizationDescriptions.First().Value.Should().Be(source.OrganizationDescriptions.First().Description);
            target.OrganizationType.Should().Be(cacheManager.TypesCache.GetByValue<OrganizationType>(source.TypeId));
            var targetAddress = target.Addresses.First();
            var sourceAddress = source.OrganizationAddresses.First();
            targetAddress.StreetAddress.First().Value.Should().Be(sourceAddress.Address.StreetNames.First().Text);
            targetAddress.Type.Should().Be(cacheManager.TypesCache.GetByValue<AddressType>(sourceAddress.TypeId));
            targetAddress.Country.Should().Be(sourceAddress.Address.Country.Code);
            target.WebPages.First().Value.Should().Be(source.OrganizationWebAddress.First().WebPage.Name);
            target.Services.Count.Should().Be(2);
            target.DisplayNameType.Should().Be(cacheManager.TypesCache.GetByValue<NameType>(source.DisplayNameTypeId));

            // check phones
            target.PhoneNumbers.Count.Should().Be(source.OrganizationPhones.Count);
            AssertPhone(
                target.PhoneNumbers.Single(p => p.Language == LanguageCode.fi.ToString()),
                source.OrganizationPhones.Single(p => p.Phone.LocalizationId == fiId).Phone
                );
            AssertPhone(
                target.PhoneNumbers.Single(p => p.Language == LanguageCode.sv.ToString()),
                source.OrganizationPhones.Single(p => p.Phone.LocalizationId == svId).Phone
                );

            // check emails
            target.EmailAddresses.Count.Should().Be(source.OrganizationEmails.Count);
            AssertEmail(
                target.EmailAddresses.Single(e => e.Language == LanguageCode.fi.ToString()),
                source.OrganizationEmails.Single(e => e.Email.LocalizationId == fiId).Email
                );
            AssertEmail(
                target.EmailAddresses.Single(e => e.Language == LanguageCode.sv.ToString()),
                source.OrganizationEmails.Single(e => e.Email.LocalizationId == svId).Email
                );
        }

        [Fact]
        public void TranslateOrganizationVmToEntity()
        {
            var organization = CreateOrganizationVm();
            var toTranslate = new List<IV2VmOpenApiOrganizationInBase> { organization };
            var translations = RunTranslationModelToEntityTest<IV2VmOpenApiOrganizationInBase, Organization>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(organization, translation);
        }

        private void CheckTranslations(IV2VmOpenApiOrganizationInBase source, Organization target)
        {
            target.OrganizationNames.First().Name.Should().Be(TestDataFactory.TEXT);

            // organization emails
            source.EmailAddresses.Count.Should().Be(target.OrganizationEmails.Count);
            AssertEmail(
                source.EmailAddresses.Single(e => e.Language == LanguageCode.fi.ToString()),
                target.OrganizationEmails.Single(e => e.Email.LocalizationId == fiId).Email
                );
            AssertEmail(
                source.EmailAddresses.Single(e => e.Language == LanguageCode.sv.ToString()),
                target.OrganizationEmails.Single(e => e.Email.LocalizationId == svId).Email
                );

            // organization phones
            source.PhoneNumbers.Count.Should().Be(target.OrganizationPhones.Count);
            AssertPhone(
                source.PhoneNumbers.Single(p => p.Language == LanguageCode.fi.ToString()),
                target.OrganizationPhones.Single(p => p.Phone.LocalizationId == fiId).Phone
                );
            AssertPhone(
                source.PhoneNumbers.Single(p => p.Language == LanguageCode.sv.ToString()),
                target.OrganizationPhones.Single(p => p.Phone.LocalizationId == svId).Phone
                );
        }

        private void AssertEmail(IVmOpenApiEmail sourceEmail, Email targetEmail)
        {
            targetEmail.LocalizationId.Should().Be(cacheManager.LanguageCache.Get(sourceEmail.Language));
            targetEmail.Value.Should().Be(sourceEmail.Value);
            targetEmail.Description.Should().Be(sourceEmail.Description);
        }

        private void AssertPhone(IVmOpenApiPhone sourcePhone, Phone targetPhone)
        {
            targetPhone.LocalizationId.Should().Be(cacheManager.LanguageCache.Get(sourcePhone.Language));
            targetPhone.Number.Should().Be(sourcePhone.Number);
            targetPhone.PrefixNumber.Should().Be(sourcePhone.PrefixNumber);
            targetPhone.ServiceChargeTypeId.Should().Be(cacheManager.TypesCache.Get<ServiceChargeType>(sourcePhone.ServiceChargeType));
            targetPhone.ChargeDescription.Should().Be(sourcePhone.ChargeDescription);
            targetPhone.AdditionalInformation.Should().Be(sourcePhone.AdditionalInformation);
        }

        private Organization CreateOrganization()
        {
            return new Organization()
            {
                OrganizationNames = new List<OrganizationName>()
                {
                    new OrganizationName()
                    {
                        Name = "Organization1",
                        Type = new NameType { Code = OrganizationTypeEnum.RegionalOrganization.ToString() },
                        Localization = TestDataFactory.LocalizationFi()
                    }
                },
                DisplayNameTypeId = cacheManager.TypesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                OrganizationDescriptions = new List<OrganizationDescription>()
                {
                    new OrganizationDescription()
                    {
                        Description = DESCRIPTION_FI,
                        Localization = TestDataFactory.LocalizationFi(),
                        Type = new DescriptionType() { Code = DescriptionTypeEnum.Description.ToString() }
                    }
                },
                Municipality = new Municipality() { Name = MUNICIPALITY },
                TypeId = cacheManager.TypesCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString()),
                OrganizationAddresses = CreateAddresses(),
                OrganizationPhones = new List<OrganizationPhone>
                {
                    new OrganizationPhone
                    {
                        Phone = new Phone
                        {
                            TypeId = cacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                            Number =  PHONE_NUMBER,
                            PrefixNumber = PHONE_PREFIX,
                            ServiceChargeTypeId = cacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()),
                            ChargeDescription = DESCRIPTION_FI,
                            LocalizationId = fiId
                         }
                    },
                    new OrganizationPhone
                    {
                        Phone = new Phone
                        {
                            TypeId = cacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                            Number =  PHONE_NUMBER,
                            PrefixNumber = PHONE_PREFIX,
                            ServiceChargeTypeId = cacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()),
                            ChargeDescription = DESCRIPTION_SV,
                            LocalizationId = svId
                         }
                    }
                },
                OrganizationEmails = new List<OrganizationEmail>
                {
                    new OrganizationEmail
                    {
                        Email = new Email
                        {
                            LocalizationId = cacheManager.LanguageCache.Get(LanguageCode.fi.ToString()),
                            Value = EMAIL_FI,
                            Description = DESCRIPTION_FI
                        }
                    },
                    new OrganizationEmail
                    {
                        Email = new Email
                        {
                            LocalizationId = cacheManager.LanguageCache.Get(LanguageCode.sv.ToString()),
                            Value = EMAIL_FI,
                            Description = DESCRIPTION_SV
                        }
                    }
                },
                OrganizationWebAddress = new List<OrganizationWebPage>
                {
                    new OrganizationWebPage
                    {
                        WebPage = TestDataFactory.CreateWebPage()
                    }
                },
                OrganizationServices = CreateOrganizationServices(),
            };
        }

        private IList<OrganizationAddress> CreateAddresses()
        {
            return new List<OrganizationAddress>()
            {
                new OrganizationAddress()
                {
                    Address = TestDataFactory.CreateAddress(),
                    TypeId = cacheManager.TypesCache.Get<AddressType>(AddressTypeEnum.Postal.ToString())
                }
            };
        }

        private static IList<OrganizationService> CreateOrganizationServices()
        {
            return new List<OrganizationService>() {
                new OrganizationService()
                {
                    ServiceId = Guid.NewGuid(),
                    RoleType = new RoleType() {Code = RoleTypeEnum.Producer.ToString() },
                    WebPages = new List<OrganizationServiceWebPage>()
                    {
                        new OrganizationServiceWebPage()
                        {
                            WebPage = TestDataFactory.CreateWebPage(),
                            Type = new WebPageType() { Code = WebPageTypeEnum.HomePage.ToString() }
                        }
                    }
                },
                new OrganizationService() {  ServiceId = Guid.NewGuid() }
            };
        }

        private static IV2VmOpenApiOrganizationInBase CreateOrganizationVm()
        {

            return new V2VmOpenApiOrganizationInBase()
            {
                OrganizationType = OrganizationTypeEnum.Municipality.ToString(),
                Municipality = "620",
                BusinessCode = TestDataFactory.TEXT,
                OrganizationNames = TestDataFactory.CreateLocalizedList(NameTypeEnum.Name.ToString()),
                OrganizationDescriptions = TestDataFactory.CreateLanguageItemList(),
                EmailAddresses = new List<VmOpenApiEmail> {
                    new VmOpenApiEmail {Value = EMAIL_FI, Description = DESCRIPTION_FI, Language = LanguageCode.fi.ToString()},
                    new VmOpenApiEmail {Value = EMAIL_SV, Description = DESCRIPTION_SV, Language = LanguageCode.sv.ToString()}
                },
                PhoneNumbers = new List<VmOpenApiPhone>
                {
                    new VmOpenApiPhone {
                        ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                        Number = PHONE_NUMBER,
                        PrefixNumber = PHONE_PREFIX,
                        Language = LanguageCode.fi.ToString(),
                        AdditionalInformation = DESCRIPTION_FI
                    },
                    new VmOpenApiPhone {
                        ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                        Number = PHONE_NUMBER,
                        PrefixNumber = PHONE_PREFIX,
                        Language = LanguageCode.sv.ToString(),
                        AdditionalInformation = DESCRIPTION_SV
                    }
                },
                Addresses = new List<V2VmOpenApiAddressWithType>()
                {
                    TestDataFactory.CreateAddressVm()
                },
                WebPages = TestDataFactory.CreateWebPageListVm<VmOpenApiWebPageIn>(),
                PublishingStatus = PublishingStatus.Published.ToString(),
                DisplayNameType = NameTypeEnum.Name.ToString()
            };
        }
    }
}
