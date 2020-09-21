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
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.DataAccess.Translators.OpenApi.Organizations;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.OpenApi.Channels;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Database.DataAccess.Translators.Finto;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiOrganizationTranslatorTests : TranslatorTestBase
    {
        private const string MUNICIPALITY = "Uusimaa";
        private const string DESCRIPTION_FI = "TestDescription_FI";
        private const string DESCRIPTION_SV = "TestDescription_SV";
        private const string DESCRIPTION_FI_JSON = "{\"entityMap\":{},\"blocks\":[{\"text\":\"" + DESCRIPTION_FI + "\",\"type\":\"unstyled\",\"key\":\"FUU4G\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";

        private const string EMAIL_FI = "some@one.fi";
        private const string EMAIL_SV = "some@one.sv";
        private const string PHONE_NUMBER = "0123456789";
        private const string PHONE_PREFIX = "3210";

        private readonly List<object> translators;

        private readonly Guid fiId;
        private readonly Guid svId;

        public OpenApiOrganizationTranslatorTests()
        {
            SetupTypesCacheMock<PhoneNumberType>(typeof(PhoneNumberTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));
            SetupTypesCacheMock<AddressCharacter>(typeof(AddressCharacterEnum));
            SetupTypesCacheMock<AddressType>(typeof(AddressTypeEnum));
            SetupTypesCacheMock<OrganizationType>(typeof(OrganizationTypeEnum));
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<ProvisionType>(typeof(ProvisionTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            translators = new List<object>
            {
                new OpenApiOrganizationTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OrganizationTypeStringTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiOrganizationDisplayNameTypeTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceChargeTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationWebPageTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
//                new WebPageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationAddressTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new AddressTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetNameTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new CountryCodeTranslator(ResolveManager, TranslationPrimitives),
                new DataAccess.Translators.OpenApi.Organizations.OpenApiOrganizationServiceTranslator(ResolveManager, TranslationPrimitives),
//                new ProvisionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives),
                new MunicipalityCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiBusinessTranslator(ResolveManager, TranslationPrimitives),
//                new OpenApiWebPageTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAddressInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiPostalCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceChannelAttachmentTranslator(ResolveManager, TranslationPrimitives),
//                new PhoneNumberTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationPhoneTranslator(ResolveManager, TranslationPrimitives),
                new EmailTranslator(ResolveManager, TranslationPrimitives),
                new OrganizationEmailDataTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationEmailTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationEmailEmailTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiEmailItemTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressDescriptionTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiEmailTranslator(ResolveManager, TranslationPrimitives),
                new PhoneTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new OpenApiPhoneTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiOrganizationAddressWithCoordinatesTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiMunicipalityTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiMunicipalityNameTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiDialCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostalCodeNameTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationLanguageAvailabilityTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAddressTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new OpenApiPostOfficeBoxNameTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressPostOfficeBoxTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressStreetTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiProducerOrganizationServiceTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceItemTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiStreetInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostOfficeBoxTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressForeignTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressForeignTextNameTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostOfficeBoxInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetWithCoordinatesTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
//                new OpenApiOrganizationWebPageTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OrderNumberTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetNumberTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new WebPageUrlTranslator(ResolveManager, TranslationPrimitives)
            };

            fiId = LanguageCache.Get("fi");
            svId = LanguageCache.Get("sv");
        }

        [Fact]
        public void TranslateOrganizationToVm()
        {
            var organization = CreateOrganization();
            var toTranslate = new List<OrganizationVersioned> { organization };
            var translations = RunTranslationEntityToModelTest<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(organization, translation, true);
        }

        private void CheckTranslations(OrganizationVersioned source, IVmOpenApiOrganizationVersionBase target, bool openApiEnumValue = false)
        {
            target.OrganizationNames.Count.Should().Be(1);
            target.Municipality.Name.FirstOrDefault().Value.Should().Be(source.Municipality.MunicipalityNames.FirstOrDefault().Name);
            target.OrganizationDescriptions.First().Value.Should().NotBeNull();
            target.OrganizationType.Should().Be(CacheManager.TypesCache.GetByValue<OrganizationType>(source.TypeId.Value));
            var targetAddress = target.Addresses.First();
            var sourceAddress = source.OrganizationAddresses.First();
            targetAddress.StreetAddress.Street.First().Value.Should().Be(
                sourceAddress.Address.ClsAddressPoints.First().AddressStreet.StreetNames.First().Name);
            targetAddress.Type.Should().Be(CacheManager.TypesCache.GetByValue<AddressCharacter>(sourceAddress.CharacterId));
            targetAddress.Country.Should().Be(sourceAddress.Address.Country.Code);
            target.WebPages.First().Value.Should().Be(source.OrganizationWebAddress.First().Name);
            target.Services.Count.Should().Be(3);
            target.DisplayNameType.First().Type.Should().Be(CacheManager.TypesCache.GetByValue<NameType>(source.OrganizationDisplayNameTypes.First().DisplayNameTypeId));

            // check phones
            target.PhoneNumbers.Count.Should().Be(source.OrganizationPhones.Count);
            AssertPhone(
                target.PhoneNumbers.Single(p => p.Language == "fi"),
                source.OrganizationPhones.Single(p => p.Phone.LocalizationId == fiId).Phone,
                openApiEnumValue
                );
            AssertPhone(
                target.PhoneNumbers.Single(p => p.Language == "sv"),
                source.OrganizationPhones.Single(p => p.Phone.LocalizationId == svId).Phone,
                openApiEnumValue
                );

            // check emails
            target.Emails.Count.Should().Be(source.OrganizationEmails.Count);
            AssertEmail(
                target.Emails.Single(e => e.Language == "fi"),
                source.OrganizationEmails.Single(e => e.Email.LocalizationId == fiId).Email
                );
            AssertEmail(
                target.Emails.Single(e => e.Language == "sv"),
                source.OrganizationEmails.Single(e => e.Email.LocalizationId == svId).Email
                );
        }

        [Fact]
        public void TranslateOrganizationVmToEntity()
        {
            var organization = CreateOrganizationVm();
            var toTranslate = new List<IVmOpenApiOrganizationInVersionBase> { organization };
            var translations = RunTranslationModelToEntityTest<IVmOpenApiOrganizationInVersionBase, OrganizationVersioned>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(organization, translation);
        }

        private void CheckTranslations(IVmOpenApiOrganizationInVersionBase source, OrganizationVersioned target)
        {
            target.OrganizationNames.First().Name.Should().Be(TestDataFactory.TEXT);

            // organization emails
            source.Emails.Count.Should().Be(target.OrganizationEmails.Count);
            AssertEmail(
                source.Emails.Single(e => e.Language == "fi"),
                target.OrganizationEmails.Single(e => e.Email.LocalizationId == fiId).Email
                );
            AssertEmail(
                source.Emails.Single(e => e.Language == "sv"),
                target.OrganizationEmails.Single(e => e.Email.LocalizationId == svId).Email
                );

            // organization phones
            source.PhoneNumbers.Count.Should().Be(target.OrganizationPhones.Count);
            AssertPhone(
                source.PhoneNumbers.Single(p => p.Language == "fi"),
                target.OrganizationPhones.Single(p => p.Phone.LocalizationId == fiId).Phone,
                true
                );
            AssertPhone(
                source.PhoneNumbers.Single(p => p.Language == "sv"),
                target.OrganizationPhones.Single(p => p.Phone.LocalizationId == svId).Phone,
                true
                );
        }

        private void AssertEmail(IVmOpenApiEmailBase sourceEmail, Email targetEmail)
        {
            targetEmail.LocalizationId.Should().Be(CacheManager.LanguageCache.Get(sourceEmail.Language));
            targetEmail.Value.Should().Be(sourceEmail.Value);
            targetEmail.Description.Should().Be(sourceEmail.Description);
        }

        private void AssertPhone(IV4VmOpenApiPhone sourcePhone, Phone targetPhone, bool openApiEnumValue = false)
        {
            targetPhone.LocalizationId.Should().Be(CacheManager.LanguageCache.Get(sourcePhone.Language));
            targetPhone.Number.Should().Be(sourcePhone.Number);
            if (openApiEnumValue)
            {
                targetPhone.ChargeTypeId.Should().Be(CacheManager.TypesCache.Get<ServiceChargeType>(sourcePhone.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>()));
            }
            else
            {
                targetPhone.ChargeTypeId.Should().Be(CacheManager.TypesCache.Get<ServiceChargeType>(sourcePhone.ServiceChargeType));
            }

            targetPhone.ChargeDescription.Should().Be(sourcePhone.ChargeDescription);
            targetPhone.AdditionalInformation.Should().Be(sourcePhone.AdditionalInformation);
        }

        private OrganizationVersioned CreateOrganization()
        {
            return new OrganizationVersioned
            {
                OrganizationNames = new List<OrganizationName>
                {
                    new OrganizationName
                    {
                        Name = "Organization1",
                        Type = new NameType { Code = OrganizationTypeEnum.RegionalOrganization.ToString() },
                        TypeId = CacheManager.TypesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                        Localization = TestDataFactory.LocalizationFi()
                    }
                },
                OrganizationDisplayNameTypes = new List<OrganizationDisplayNameType>
                {
                    new OrganizationDisplayNameType
                    {
                        DisplayNameTypeId = CacheManager.TypesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                        LocalizationId = fiId
                    }
                },
                OrganizationDescriptions = new List<OrganizationDescription>
                {
                    new OrganizationDescription
                    {
                        Description = DESCRIPTION_FI_JSON,
                        Localization = TestDataFactory.LocalizationFi(),
                        Type = new DescriptionType { Code = DescriptionTypeEnum.Description.ToString() },
                        TypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()),
                    }
                },
                Municipality = new Municipality { MunicipalityNames = new List<MunicipalityName> { new MunicipalityName { Name = MUNICIPALITY } } },
                TypeId = CacheManager.TypesCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString()),
                OrganizationAddresses = CreateAddresses(),
                OrganizationPhones = new List<OrganizationPhone>
                {
                    new OrganizationPhone
                    {
                        Phone = new Phone
                        {
                            TypeId = CacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                            Number =  PHONE_NUMBER,
                            PrefixNumber = new DialCode { Code = PHONE_PREFIX },
                            ChargeTypeId = CacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()),
                            ChargeDescription = DESCRIPTION_FI,
                            LocalizationId = fiId
                         }
                    },
                    new OrganizationPhone
                    {
                        Phone = new Phone
                        {
                            TypeId = CacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                            Number =  PHONE_NUMBER,
                            PrefixNumber = new DialCode { Code = PHONE_PREFIX },
                            ChargeTypeId = CacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()),
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
                            LocalizationId = CacheManager.LanguageCache.Get("fi"),
                            Value = EMAIL_FI,
                            Description = DESCRIPTION_FI
                        }
                    },
                    new OrganizationEmail
                    {
                        Email = new Email
                        {
                            LocalizationId = CacheManager.LanguageCache.Get("sv"),
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
                UnificRoot = new Organization { OrganizationServices = CreateOrganizationServices(), ServiceProducerOrganizations = CreateServiceProducerOrganization() }
            };
        }

        private IList<OrganizationAddress> CreateAddresses()
        {
            return new List<OrganizationAddress>
            {
                new OrganizationAddress
                {
                    Address = TestDataFactory.CreateAddress(CacheManager.TypesCache.Get<AddressType>(AddressTypeEnum.Street.ToString())),
                    CharacterId = CacheManager.TypesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())
                }
            };
        }

        private IList<OrganizationService> CreateOrganizationServices()
        {
            return new List<OrganizationService> {
                new OrganizationService { ServiceVersionedId = Guid.NewGuid() },
                new OrganizationService { ServiceVersionedId = Guid.NewGuid() }
            };
        }

        private IList<ServiceProducerOrganization> CreateServiceProducerOrganization()
        {
            var producer = new ServiceProducer
            {
                ProvisionTypeId = CacheManager.TypesCache.Get<ProvisionType>(ProvisionTypeEnum.Other.ToString()),
                AdditionalInformations = new List<ServiceProducerAdditionalInformation>
                {
                    new ServiceProducerAdditionalInformation
                    {
                        Text = DESCRIPTION_FI,
                        LocalizationId = CacheManager.LanguageCache.Get("sv")
                    }
                }
            };
            var producerOrganization = new ServiceProducerOrganization { ServiceProducer = producer };
            return new List<ServiceProducerOrganization> { producerOrganization };
        }

        private static IVmOpenApiOrganizationInVersionBase CreateOrganizationVm()
        {

            return new VmOpenApiOrganizationInVersionBase
            {
                OrganizationType = OrganizationTypeEnum.Municipality.GetOpenApiValue(),
                Municipality = "620",
                BusinessCode = TestDataFactory.TEXT,
                OrganizationNames = TestDataFactory.CreateLocalizedList(NameTypeEnum.Name.ToString()),
                OrganizationDescriptions = TestDataFactory.CreateLocalizedList("Description").ToList(),
                Emails = new List<V4VmOpenApiEmail> {
                    new V4VmOpenApiEmail {Value = EMAIL_FI, Description = DESCRIPTION_FI, Language = "fi"},
                    new V4VmOpenApiEmail {Value = EMAIL_SV, Description = DESCRIPTION_SV, Language = "sv"}
                },
                PhoneNumbers = new List<V4VmOpenApiPhone>
                {
                    new V4VmOpenApiPhone {
                        ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                        Number = PHONE_NUMBER,
                        PrefixNumber = PHONE_PREFIX,
                        Language = "fi",
                        AdditionalInformation = DESCRIPTION_FI
                    },
                    new V4VmOpenApiPhone {
                        ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                        Number = PHONE_NUMBER,
                        PrefixNumber = PHONE_PREFIX,
                        Language = "sv",
                        AdditionalInformation = DESCRIPTION_SV
                    }
                },
                Addresses = new List<V9VmOpenApiAddressIn>
                {
                    TestDataFactory.CreateAddressVm<V9VmOpenApiAddressIn>(AddressTypeEnum.Street),
                    TestDataFactory.CreateAddressVm<V9VmOpenApiAddressIn>(AddressTypeEnum.PostOfficeBox),
                    TestDataFactory.CreateAddressVm<V9VmOpenApiAddressIn>(AddressTypeEnum.Foreign)
                },
                WebPages = TestDataFactory.CreateWebPageListVm<V9VmOpenApiWebPage>(),
                PublishingStatus = PublishingStatus.Published.ToString(),
                DisplayNameType = new List<VmOpenApiNameTypeByLanguage>
                {
                    new VmOpenApiNameTypeByLanguage
                    {
                        Language = "fi",
                        Type = NameTypeEnum.Name.GetOpenApiValue()
                    }
                }
            };
        }
    }
}
