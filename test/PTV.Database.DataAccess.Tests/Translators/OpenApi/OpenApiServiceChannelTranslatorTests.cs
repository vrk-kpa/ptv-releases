/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *w
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
using PTV.Database.DataAccess.Translators.OpenApi.Channels;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Common;
using Xunit;
using OpenApiPhoneChannelTranslator = PTV.Database.DataAccess.Translators.OpenApi.Channels.OpenApiPhoneChannelTranslator;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceChannelTranslatorTests : TranslatorTestBase
    {
        private const string SERVICE_CHANNEL_NAME = "Service channel name";
        private const string SERVICE_CHANNEL_DESCRIPTION = "TestDescription";
        private const string SERVICE_CHANNEL_CLOSES_TEXT = "Closes text";
        private const string SERVICE_CHANNEL_ADDITIONAL_INFO_TEXT = "Additional info";
        private const string SERVICE_CHANNEL_PHONE_NUMBER = "0987654321";
        private const string SERVICE_CHANNEL_TEXT = "Text";
        private const string PHONE_DESCRIPTION_CHARGE_DESCRIPTION_FI = "PHONE_DESCRIPTION_CHARGE_DESCRIPTION_FI";
        private const string PHONE_DESCRIPTION_CHARGE_DESCRIPTION_SV = "PHONE_DESCRIPTION_CHARGE_DESCRIPTION_SV";
        private const string PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI = "PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI";
        private const string PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_SV = "PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_SV";
        private const string PHONE_PREFIX_NUMBER_FI = "0048";
        private const string PHONE_PREFIX_NUMBER_SV = "0046";
        private const string PHONE_NUMBER_FI = "123456789";
        private const string PHONE_NUMBER_SV = "987654321";
        private const string EMAIL_FI = "email@domain.fi";
        private const string EMAIL_SV = "email@domain.sv";
        private const string EMAIL_DESCRIPTION_FI = "EMAIL_DESCRIPTION_FI";
        private const string EMAIL_DESCRIPTION_SV = "EMAIL_DESCRIPTION_SV";


        private readonly string newGuid = Guid.NewGuid().ToString();
        private readonly List<object> translators;
        private readonly ICacheManager cacheManager;

        private readonly Language FI;
        private readonly Language SV;

        private readonly Guid PhoneTypePhoneId;

        public OpenApiServiceChannelTranslatorTests()
        {
            var languageCache = SetupLanguageCacheMock().Object;
            var typesCacheMock = SetupTypesCacheMock<PhoneNumberType>(typesEnum: typeof (PhoneNumberTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typesCacheMock, typeof (ServiceChargeTypeEnum));
            SetupTypesCacheMock<ServiceChannelType>(typesCacheMock, typeof (ServiceChannelTypeEnum));
            SetupTypesCacheMock<AddressType>(typesCacheMock, typeof (AddressTypeEnum));
            SetupTypesCacheMock<PrintableFormChannelUrlType>(typesCacheMock, typeof (PrintableFormChannelUrlTypeEnum));
            cacheManager = SetupCacheManager(languageCache, typesCacheMock.Object).Object;

            translators = new List<object>()
            {
                new OpenApiServiceChannelNameTranslators(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceChannelDescriptionTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceHourTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceHoursAdditionalInformationTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new OpenApiServiceChannelWebPageTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiEChannelTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiEChannelUrlTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new OpenApiAttachmentTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiAttachmentWithTypeTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceLocationChannelTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiAddressTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceLocationChannelAddressTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new AddressTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetAddressTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new CountryCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPhoneChannelTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiWebPageChannelTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiWebPageChannelUrlTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives),
                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceHourTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ExceptionHoursStatusTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new AttachmentTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPrintableFormChannelTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiPrintableFormChannelUrlTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiAddressDescriptionTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiEChannelMainInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiEChannelInTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChargeTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPhoneChannelInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceChannelLanguageTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new OpenApiWebPageChannelMainInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiWebPageChannelInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPrintableFormChannelMainInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiPrintableFormChannelInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostalCodeTranslator(ResolveManager, TranslationPrimitives),
                new MunicipalityCodeTranslator(ResolveManager, TranslationPrimitives),
                new PrintableFormChannelUrlTypeEnumCodeTranslator(ResolveManager, TranslationPrimitives),
                new WebPageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceLocationChannelMainInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceLocationChannelInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiWebPageTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceLocationChannelServiceAreaTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressWithTypeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPhoneTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new ServiceChannelEmailTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceChannelAttachmentTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceChannelEmailTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new EmailTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceChannelEmailSupportTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new ServiceChannelPhoneTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceChannelPhoneTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new PhoneTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
                new OpenApiPhoneWithTypeTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiDailyOpeningHourTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceChannelPhoneWithTypeTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceLocationChannelAddressWithCoordinatesTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiEmailTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiAddressWithCoordinatesTranslator(ResolveManager, TranslationPrimitives)
            };

            // set languages
            var localizationFi = LanguageCode.fi.ToString();
            var localizationSv = LanguageCode.sv.ToString();
            var localizationIdFi = cacheManager.LanguageCache.Get(localizationFi);
            var localizationIdSv = cacheManager.LanguageCache.Get(localizationSv);
            FI = new Language {Id = localizationIdFi, Code = localizationFi};
            SV = new Language {Id = localizationIdSv, Code = localizationSv};

            PhoneTypePhoneId = cacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
        }

        [Fact]
        public void TranslateEChannelToVm()
        {
            var channel = CreateEChannel();
            var toTranslate = new List<ServiceChannel> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannel, V2VmOpenApiElectronicChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannel source, V2VmOpenApiElectronicChannel target)
        {
            target.ServiceChannelType.Should().Be(cacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId)).And.Be(ServiceChannelTypeEnum.EChannel.ToString());
            target.Urls.Count.Should().Be(source.ElectronicChannels.First().LocalizedUrls.Count).And.Be(2);
            target.Attachments.First().Name.Should().Be(source.Attachments.First().Attachment.Name).And.Be(TestDataFactory.ATTACHMENT_NAME);
            target.WebPages.First().Value.Should().Be(source.WebPages.First().WebPage.Name).And.Be(TestDataFactory.WEBPAGE_NAME);
            CheckSupportContacts(source, target);
        }

        [Fact]
        public void TranslateServiceLocationChannelToVm()
        {
            var channel = CreateLocationServiceChannel();
            var toTranslate = new List<ServiceChannel> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannel, V2VmOpenApiServiceLocationChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannel source, V2VmOpenApiServiceLocationChannel target)
        {
            target.ServiceChannelType.Should().Be(cacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId));
            target.ServiceAreas.First().Should().Be(source.ServiceLocationChannels.First().ServiceAreas.First().Municipality.Name);

            var targetAddress = target.Addresses.First();
            var sourceAddress = source.ServiceLocationChannels.First().Addresses.First();
            targetAddress.Type.Should().Be(cacheManager.TypesCache.GetByValue<AddressType>(sourceAddress.TypeId));
            targetAddress.Country.Should().Be(sourceAddress.Address.Country.Code);
            target.Languages.Count.Should().Be(2);
            CheckSupportContacts(source, target);
        }

        [Fact]
        public void TranslatePhoneChannelToVm()
        {
            var channel = CreatePhoneServiceChannel();
            var toTranslate = new List<ServiceChannel> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannel, V2VmOpenApiPhoneChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannel source, IV2VmOpenApiPhoneChannel target)
        {
            CheckSupportContactsPhoneChannelToVm(source, target);
        }

        [Fact]
        public void TranslateWebpageChannelToVm()
        {
            var channel = CreateWebpageChannel();
            var toTranslate = new List<ServiceChannel> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannel, V2VmOpenApiWebPageChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannel source, V2VmOpenApiWebPageChannel target)
        {
            target.ServiceChannelType.Should().Be(cacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId)).And.Be(ServiceChannelTypeEnum.WebPage.ToString());
//             target.Attachments.First().Name.Should().Be(source.WebpageChannels.First().Attachments.First().Attachment.Name).And.Be(TestDataFactory.ATTACHMENT_NAME);
            target.Urls.Count.Should().Be(source.WebpageChannels.First().LocalizedUrls.Count);
            CheckSupportContacts(source, target);
        }

        [Fact]
        public void TranslatePrintableFormChannelToVm()
        {
            var channel = CreatePrintableFormChannel();
            var toTranslate = new List<ServiceChannel> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannel, V2VmOpenApiPrintableFormChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannel source, V2VmOpenApiPrintableFormChannel target)
        {
            target.ServiceChannelType.Should().Be(cacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId)).And.Be(ServiceChannelTypeEnum.PrintableForm.ToString());
            //target.Attachments.First().Type.Should().Be(source.PrintableFormChannels.First().Attachments.First().Attachment.Type.Code).And.Be(AttachmentTypeEnum.Attachment.ToString());
            target.ChannelUrls.First().Type.Should().Be(cacheManager.TypesCache.GetByValue<PrintableFormChannelUrlType>(source.PrintableFormChannels.First().ChannelUrls.First().TypeId)).And.Be(PrintableFormChannelUrlTypeEnum.DOC.ToString());
            target.DeliveryAddress.Country.Should().Be(source.PrintableFormChannels.First().DeliveryAddress.Country.Code).And.Be(LanguageCode.fi.ToString());
            target.FormReceiver.Should().Be(source.PrintableFormChannels.First().FormReceiver).And.Be(SERVICE_CHANNEL_TEXT);
            CheckSupportContacts(source, target);
        }

        [Fact]
        public void TranslateEChannelVmToEntity()
        {
            var vm = CreateEChannelVm();
            var toTranslate = new List<IV2VmOpenApiElectronicChannelInBase> {vm};
            var translations = RunTranslationModelToEntityTest<IV2VmOpenApiElectronicChannelInBase, ServiceChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IV2VmOpenApiElectronicChannelInBase source, ServiceChannel target)
        {
            target.ServiceChannelNames.Count.Should().Be(1);
            target.ServiceHours.First().DailyOpeningTimes.First().DayFrom.Should().Be(0);

            // check attachement
            target.Attachments.Should().NotBeNull();
            target.Attachments.Count.Should().Be(source.Attachments.Count);

            var eChannel = target.ElectronicChannels.FirstOrDefault();
            Assert.NotNull(eChannel);
            eChannel.SignatureQuantity.Should().Be(2);
            eChannel.LocalizedUrls.Count.Should().Be(1);
            CheckSupportContacts(target, source);
        }

        [Fact]
        public void TranslatePhoneChannelVmToEntity()
        {
            var vm = CreatePhoneChannelVm();
            var toTranslate = new List<IV2VmOpenApiPhoneChannelInBase> {vm};
            var translations = RunTranslationModelToEntityTest<IV2VmOpenApiPhoneChannelInBase, ServiceChannel>(translators, toTranslate);
            var translation = translations.First();
            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IV2VmOpenApiPhoneChannelInBase source, ServiceChannel target)
        {

#warning opening hours model change
            //target.ServiceHours.First().Monday.Should().Be(true);

            CheckSupportContactsPhoneChannelToEntity(source, target);
        }

        [Fact]
        public void TranslateWebPageChannelVmToEntity()
        {
            var vm = CreateWebPageChannelVm();
            var toTranslate = new List<IV2VmOpenApiWebPageChannelInBase> {vm};
            var translations = RunTranslationModelToEntityTest<IV2VmOpenApiWebPageChannelInBase, ServiceChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IVmOpenApiWebPageChannelInBase source, ServiceChannel target)
        {
            target.Languages.Count.Should().Be(1);
            var webPageChannel = target.WebpageChannels.FirstOrDefault();
            Assert.NotNull(webPageChannel);
            webPageChannel.LocalizedUrls.Count.Should().Be(source.Urls.Count);
            CheckSupportContacts(target, source);
        }

        [Fact]
        public void TranslatePrintableFormChannelVmToEntity()
        {
            var vm = CreatePrintableFormChannelVm();
            var toTranslate = new List<IV2VmOpenApiPrintableFormChannelInBase>() {vm};
            var translations = RunTranslationModelToEntityTest<IV2VmOpenApiPrintableFormChannelInBase, ServiceChannel>(translators,toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IV2VmOpenApiPrintableFormChannelInBase source, ServiceChannel target)
        {
            var channel = target.PrintableFormChannels.FirstOrDefault();
            Assert.NotNull(channel);
            channel.FormIdentifier.Should().Be(TestDataFactory.TEXT);
            channel.ChannelUrls.Count.Should().Be(1);
            //channel.DeliveryAddressDescriptions.Count.Should().Be(1);
            //channel.Attachments.Count.Should().Be(1);

            CheckSupportContacts(target, source);
        }

        [Fact]
        public void TranslateServiceLocationChannelVmToEntity()
        {
            var vm = CreateServiceLocationChannelVm();
            var toTranslate = new List<IV2VmOpenApiServiceLocationChannelInBase> {vm};
            var translations = RunTranslationModelToEntityTest<IV2VmOpenApiServiceLocationChannelInBase, ServiceChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IV2VmOpenApiServiceLocationChannelInBase source, ServiceChannel target)
        {
            target.Languages.Count.Should().Be(source.Languages.Count).And.Be(1);
            //target.ServiceHours.Count.Should().Be(source.ServiceHours.Count).And.Be(1);
            target.WebPages.Count.Should().Be(source.WebPages.Count).And.Be(1);
            var channel = target.ServiceLocationChannels.FirstOrDefault();
            Assert.NotNull(channel);
            channel.ServiceAreaRestricted.Should().Be(true);
            channel.ServiceAreas.Count.Should().Be(1);
            channel.Addresses.Count.Should().Be(1);

            ChecServiceLocationkSupportContacts(target, source);
        }

        private void CheckSupportContactsPhoneChannelToVm(ServiceChannel source, IV2VmOpenApiPhoneChannel target)
        {
            source.Phones.Count.Should().Be(target.PhoneNumbers.Count);
            source.Emails.Count.Should().Be(target.SupportEmails.Count);
            CheckSupportContactEmail(source, target, FI);
            CheckSupportContactEmail(source, target, SV);
            CheckPhoneChannelPhoneNumbers(source, target, FI);
            CheckPhoneChannelPhoneNumbers(source, target, SV);
        }

        private void CheckSupportContactsPhoneChannelToEntity(IV2VmOpenApiPhoneChannelInBase target, ServiceChannel source)
        {
            source.Phones.Count.Should().Be(target.PhoneNumbers.Count);
            source.Emails.Count.Should().Be(target.SupportEmails.Count);
            CheckSupportContactEmail(source, target, FI);
            CheckSupportContactEmail(source, target, SV);
            CheckPhoneChannelInPhoneNumbers(source, target, FI);
            CheckPhoneChannelInPhoneNumbers(source, target, SV);
        }

        private void CheckSupportContacts(ServiceChannel source, IVmOpenApiServiceChannelBase target)
        {
            var phones = source.Phones.Where(p => p.Phone.TypeId == PhoneTypePhoneId).ToList();
            phones.Count.Should().Be(target.SupportPhones.Count);
            CheckSupportContactsBase(source, target);
        }

        private void ChecServiceLocationkSupportContacts(ServiceChannel source, IV2VmOpenApiServiceLocationChannelInBase target)
        {
            source.Phones.Count.Should().Be(target.FaxNumbers.Count + target.PhoneNumbers.Count);
            CheckSupportContactsBase(source, target);
        }

        private void CheckSupportContactsBase(ServiceChannel source, IVmOpenApiServiceChannelBase target)
        {
            source.Emails.Count.Should().Be(target.SupportEmails.Count);
            CheckSupportContactEmail(source, target, FI);
            CheckSupportContactEmail(source, target, SV);
            CheckSupportContactPhone(source, target, FI);
            CheckSupportContactPhone(source, target, SV);
        }

        private static void CheckSupportContactEmail(ServiceChannel source, IVmOpenApiServiceChannelBase target, Language language)
        {
            var targetEmail = target.SupportEmails.First(e => e.Language != null && e.Language == language.Code);
            var sourceEmail = source.Emails.First(e => e.Email.LocalizationId == language.Id);
            targetEmail.Value.Should().Be(sourceEmail.Email.Value);
        }

        private void CheckSupportContactPhone(ServiceChannel source, IVmOpenApiServiceChannelBase target, Language language)
        {
            var targetPhone = target.SupportPhones.SingleOrDefault(e => e.Language != null && e.Language == language.Code);
            if (targetPhone == null) return;

            var sourcePhone = source.Phones.SingleOrDefault(e => e.Phone.LocalizationId == language.Id).Phone;
            if (sourcePhone == null) return;

            CheckPhone(sourcePhone, targetPhone);
        }

        private void CheckPhoneChannelPhoneNumbers(ServiceChannel source, IV2VmOpenApiPhoneChannel target, Language language)
        {
            var targetPhone = target.PhoneNumbers.SingleOrDefault(e => e.Language != null && e.Language == language.Code);
            if (targetPhone == null) return;

            var sourcePhone = source.Phones.SingleOrDefault(e => e.Phone.LocalizationId == language.Id).Phone;
            if (sourcePhone == null) return;

            CheckPhoneWithType(sourcePhone, targetPhone);
        }
        private void CheckPhoneChannelInPhoneNumbers(ServiceChannel source, IV2VmOpenApiPhoneChannelInBase target, Language language)
        {
            var targetPhone = target.PhoneNumbers.SingleOrDefault(e => e.Language != null && e.Language == language.Code);
            if (targetPhone == null) return;

            var sourcePhone = source.Phones.SingleOrDefault(e => e.Phone.LocalizationId == language.Id).Phone;
            if (sourcePhone == null) return;

            CheckPhoneWithType(sourcePhone, targetPhone);
        }

        private void CheckPhone(Phone sourcePhone, IVmOpenApiPhone targetPhone)
        {
            targetPhone.PrefixNumber.Should().Be(sourcePhone.PrefixNumber);
            targetPhone.Number.Should().Be(sourcePhone.Number);
            targetPhone.AdditionalInformation.Should().Be(sourcePhone.AdditionalInformation);
            targetPhone.ChargeDescription.Should().Be(sourcePhone.ChargeDescription);
            targetPhone.ServiceChargeType.Should().Be(cacheManager.TypesCache.GetByValue<ServiceChargeType>(sourcePhone.ServiceChargeTypeId));
        }

        private void CheckPhoneWithType(Phone sourcePhone, VmOpenApiPhoneWithType targetPhone)
        {
            CheckPhone(sourcePhone, targetPhone);
            targetPhone.Type.Should().Be(cacheManager.TypesCache.GetByValue<PhoneNumberType>(sourcePhone.TypeId));
        }

        private ServiceChannel CreateServiceChannel()
        {
            return new ServiceChannel()
            {
                ServiceChannelNames = new List<ServiceChannelName>()
                {
                    new ServiceChannelName()
                    {
                        Name = SERVICE_CHANNEL_NAME,
                        Localization = FI,
                        Type = new NameType() {Code = NameTypeEnum.Name.ToString()}
                    },
                    new ServiceChannelName()
                    {
                        Name = "TestName"
                    }
                },
                ServiceChannelDescriptions = new List<ServiceChannelDescription>()
                {
                    new ServiceChannelDescription()
                    {
                        Description = SERVICE_CHANNEL_DESCRIPTION,
                        Localization = TestDataFactory.LocalizationSv()
                    }
                },
                ServiceHours = new List<ServiceChannelServiceHours>()
                {
#warning oepning hours model change
                    new ServiceChannelServiceHours()
                    {
//                        Closes = SERVICE_CHANNEL_CLOSES_TEXT,
//                        ExceptionHoursType = new ExceptionHoursStatusType()
//                        {
//                            Code = ExceptionHoursStatus.Open.ToString()
//                        },
//                        Monday = true,
                        AdditionalInformations = new List<ServiceHoursAdditionalInformation>()
                        {
                            new ServiceHoursAdditionalInformation()
                            {
                                Text = SERVICE_CHANNEL_ADDITIONAL_INFO_TEXT,
                                Localization = TestDataFactory.LocalizationFi()
                            }
                        }
                    }
                },
                Phones = new List<ServiceChannelPhone>
                {
                    new ServiceChannelPhone
                    {
                        Phone = new Phone
                        {
                            LocalizationId = FI.Id,
                            PrefixNumber = PHONE_PREFIX_NUMBER_FI,
                            Number = PHONE_NUMBER_FI,
                            TypeId = cacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                            ServiceChargeTypeId = cacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()),
                            AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI
                        }
                    },
                    new ServiceChannelPhone
                    {
                        Phone = new Phone
                        {
                            LocalizationId = SV.Id,
                            PrefixNumber = PHONE_PREFIX_NUMBER_SV,
                            Number = PHONE_NUMBER_SV,
                            TypeId = cacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()),
                            ServiceChargeTypeId = cacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()),
                            ChargeDescription = PHONE_DESCRIPTION_CHARGE_DESCRIPTION_SV
                        }
                    }
                },
                Emails = new List<ServiceChannelEmail>
                {
                    new ServiceChannelEmail
                    {
                        Email = new Email
                        {
                            LocalizationId = FI.Id,
                            Value = EMAIL_FI,
                            Description = EMAIL_DESCRIPTION_FI
                        }
                    },
                    new ServiceChannelEmail
                    {
                        Email = new Email
                        {
                            LocalizationId = SV.Id,
                            Value = EMAIL_SV,
                            Description = EMAIL_DESCRIPTION_SV
                        }
                    }
                },
                WebPages = new List<ServiceChannelWebPage>()
                {
                    new ServiceChannelWebPage()
                    {
                        WebPage = TestDataFactory.CreateWebPage(),
                        Type = new WebPageType() {Code = WebPageTypeEnum.HomePage.ToString()}
                    }
                }
            };
        }

        private ServiceChannel CreateEChannel()
        {
            var eChannel = CreateServiceChannel();
            eChannel.TypeId = cacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString());
            eChannel.ElectronicChannels = new List<ElectronicChannel>()
            {
                new ElectronicChannel()
                {
                    RequiresAuthentication = true,
                    LocalizedUrls = new List<ElectronicChannelUrl>()
                    {
                        new ElectronicChannelUrl()
                        {
                            Url = "http://ptv.com",
                            Localization = TestDataFactory.LocalizationFi()
                        },
                        new ElectronicChannelUrl()
                        {
                            Url = "http://ptv.sv",
                            Localization = TestDataFactory.LocalizationSv()
                        }
                    }
                }
            };
            eChannel.Attachments = new List<ServiceChannelAttachment>()
            {
                new ServiceChannelAttachment() {Attachment = TestDataFactory.CreateAttachment()}
            };
            eChannel.WebPages = new List<ServiceChannelWebPage>()
            {
                new ServiceChannelWebPage() {WebPage = new WebPage() {Name = TestDataFactory.WEBPAGE_NAME}}
            };
            return eChannel;
        }

        private ServiceChannel CreateLocationServiceChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = cacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            channel.Languages = new List<ServiceChannelLanguage>()
            {
                new ServiceChannelLanguage() {Language = TestDataFactory.LocalizationFi()},
                new ServiceChannelLanguage() {Language = TestDataFactory.LocalizationSv()}
            };
            channel.ServiceLocationChannels = new List<ServiceLocationChannel>()
            {
                new ServiceLocationChannel()
                {
                    ServiceAreas = new List<ServiceLocationChannelServiceArea>()
                    {
                        new ServiceLocationChannelServiceArea() {Municipality = TestDataFactory.CreateMunicipality()}
                    },
                    Addresses = new List<ServiceLocationChannelAddress>()
                    {
                        new ServiceLocationChannelAddress()
                        {
                            Address = TestDataFactory.CreateAddress(),
                            TypeId = cacheManager.TypesCache.Get<AddressType>(AddressTypeEnum.Postal.ToString())
                        }
                    }
                }
            };

            return channel;
        }

        private ServiceChannel CreatePhoneServiceChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = cacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString());
            channel.Phones = new List<ServiceChannelPhone>
            {
                new ServiceChannelPhone
                {
                    Phone = new Phone
                    {
                        LocalizationId = FI.Id,
                        PrefixNumber = PHONE_PREFIX_NUMBER_FI,
                        Number = PHONE_NUMBER_FI,
                        TypeId = cacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                        ServiceChargeTypeId = cacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()),
                        AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI
                    }
                },
                new ServiceChannelPhone
                {
                    Phone = new Phone
                    {
                        LocalizationId = SV.Id,
                        PrefixNumber = PHONE_PREFIX_NUMBER_SV,
                        Number = PHONE_NUMBER_SV,
                        TypeId = cacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Sms.ToString()),
                        ServiceChargeTypeId = cacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()),
                        ChargeDescription = PHONE_DESCRIPTION_CHARGE_DESCRIPTION_SV,
                    }
                }
            };
            return channel;
        }

        private ServiceChannel CreateWebpageChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = cacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString());
            channel.WebpageChannels = new List<WebpageChannel>()
            {
                new WebpageChannel()
                {
                    LocalizedUrls = new List<WebpageChannelUrl>()
                    {
                        new WebpageChannelUrl() {Url = "Url1", Localization = TestDataFactory.LocalizationFi()},
                        new WebpageChannelUrl() {Url = "Url2", Localization = TestDataFactory.LocalizationSv()}
                    }
                }
            };
            channel.Attachments = new List<ServiceChannelAttachment>()
            {
                new ServiceChannelAttachment {Attachment = TestDataFactory.CreateAttachment()}
            };
            return channel;
        }

        private ServiceChannel CreatePrintableFormChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = cacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());
            channel.PrintableFormChannels = new List<PrintableFormChannel>()
            {
                new PrintableFormChannel()
                {
                    ChannelUrls = new List<PrintableFormChannelUrl>()
                    {
                        new PrintableFormChannelUrl()
                        {
                            Url = "Url1",
                            Localization = TestDataFactory.LocalizationFi(),
                            TypeId = cacheManager.TypesCache.Get<PrintableFormChannelUrlType>(PrintableFormChannelUrlTypeEnum.DOC.ToString())
                        }
                    },
                    DeliveryAddress = TestDataFactory.CreateAddress(),
//                DeliveryAddressDescriptions = new List<PrintableFormChannelDeliveryAddressDescription>()
//                {
//                    new PrintableFormChannelDeliveryAddressDescription() { Description = SERVICE_CHANNEL_DESCRIPTION }
//                },
                    FormIdentifier = SERVICE_CHANNEL_TEXT,
                    FormReceiver = SERVICE_CHANNEL_TEXT
                }
            };
            channel.Attachments = new List<ServiceChannelAttachment>()
            {
                new ServiceChannelAttachment {Attachment = TestDataFactory.CreateAttachment()}
            };
            return channel;
        }

        private void SetVmValues(IVmOpenApiServiceChannelIn vm)
        {
            vm.OrganizationId = newGuid;
            vm.ServiceChannelNames = TestDataFactory.CreateLanguageItemList();
            vm.ServiceChannelDescriptions = TestDataFactory.CreateLocalizedList(DescriptionTypeEnum.Description.ToString());
            vm.PublishingStatus = PublishingStatus.Published.ToString();
        }

        private V2VmOpenApiElectronicChannelIn CreateEChannelVm()
        {
            var vm = new V2VmOpenApiElectronicChannelIn();
            SetVmValues(vm);
            vm.ServiceHours = ServiceHourList();
            vm.SupportPhones = SupportPhoneList();
            vm.SupportEmails = SupportEmailList();
            vm.SignatureQuantity = 2.ToString();
            vm.Urls = TestDataFactory.CreateLanguageItemList();
            vm.Attachments = new List<VmOpenApiAttachment> {TestDataFactory.CreateAttachmentVm()};

            return vm;
        }

        private V2VmOpenApiPhoneChannelIn CreatePhoneChannelVm()
        {
            var vm = new V2VmOpenApiPhoneChannelIn();
            SetVmValues(vm);
            vm.Languages = new List<string>() { LanguageCode.fi.ToString() };
            vm.PhoneNumbers = PhoneWithTypeList();
            vm.SupportEmails = SupportEmailList();
            vm.ServiceHours = ServiceHourList();
            return vm;
        }

        private VmOpenApiWebPageChannelIn CreateWebPageChannelVm()
        {
            var vm = new VmOpenApiWebPageChannelIn();
            SetVmValues(vm);
            vm.Urls = TestDataFactory.CreateLanguageItemList();
            vm.SupportPhones = SupportPhoneList();
            vm.SupportEmails = SupportEmailList();
            vm.Languages = new List<string> {LanguageCode.fi.ToString()};
            return vm;
        }

        private V2VmOpenApiPrintableFormChannelIn CreatePrintableFormChannelVm()
        {
            var vm = new V2VmOpenApiPrintableFormChannelIn();
            SetVmValues(vm);
            vm.FormIdentifier = TestDataFactory.TEXT;
            vm.DeliveryAddress = TestDataFactory.CreateAddressVm();
            vm.ChannelUrls = TestDataFactory.CreateLocalizedList(PrintableFormChannelUrlTypeEnum.DOC.ToString());
            vm.SupportPhones = SupportPhoneList();
            vm.SupportEmails = SupportEmailList();
            vm.SupportPhones = new List<VmOpenApiPhone>();
            vm.Attachments = new List<VmOpenApiAttachment> {TestDataFactory.CreateAttachmentVm()};
            return vm;
        }

        private V2VmOpenApiServiceLocationChannelInBase CreateServiceLocationChannelVm()
        {
            var vm = new V2VmOpenApiServiceLocationChannelInBase();
            SetVmValues(vm);
            vm.ServiceAreaRestricted = true;
            vm.ServiceAreas = new List<string>() {TestDataFactory.TEXT};
            vm.Languages = new List<string>() {TestDataFactory.TEXT};
            vm.WebPages = new List<VmOpenApiWebPage>() {new VmOpenApiWebPage() {Url = TestDataFactory.URI}};
            vm.Addresses = new List<V2VmOpenApiAddressWithType> {TestDataFactory.CreateAddressVm()};
            vm.ServiceHours = ServiceHourList();
            vm.PhoneNumbers = SupportPhoneList();
            vm.FaxNumbers = SupportPhoneSimpleList();
            vm.SupportEmails = SupportEmailList();
            return vm;
        }

        private IReadOnlyList<V2VmOpenApiServiceHour> ServiceHourList()
        {
            return new List<V2VmOpenApiServiceHour>()
            {
                new V2VmOpenApiServiceHour()
                {
                    //Monday = true,
                    ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                    IsClosed = false,
                    OpeningHour = new List<V2VmOpenApiDailyOpeningTime>()
                    {
                        new V2VmOpenApiDailyOpeningTime()
                        {
                            DayFrom = WeekDayEnum.Monday.ToString(),
                            From = "10:00",
                            To = "14:00"
                        }
                    }
                }
            };
        }

        private IList<VmOpenApiPhoneWithType> PhoneWithTypeList(PhoneNumberTypeEnum phoneNumberType = PhoneNumberTypeEnum.Phone)
        {
            return new List<VmOpenApiPhoneWithType>
            {
                new VmOpenApiPhoneWithType
                {
                    PrefixNumber = PHONE_PREFIX_NUMBER_FI,
                    Number = PHONE_NUMBER_FI,
                    Language = FI.Code,
                    AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI,
                    ChargeDescription = PHONE_DESCRIPTION_CHARGE_DESCRIPTION_FI,
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                    Type = phoneNumberType.ToString()
                },
                new VmOpenApiPhoneWithType
                {
                    PrefixNumber = PHONE_PREFIX_NUMBER_SV,
                    Number = PHONE_NUMBER_SV,
                    Language = SV.Code,
                    AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_SV,
                    ChargeDescription = PHONE_DESCRIPTION_CHARGE_DESCRIPTION_SV,
                    ServiceChargeType = ServiceChargeTypeEnum.Free.ToString(),
                    Type = phoneNumberType.ToString()
                }
            };
        }

        private IList<VmOpenApiPhone> SupportPhoneList()
        {
            return PhoneWithTypeList().Select(p => p as VmOpenApiPhone).ToList();
        }

        private IList<VmOpenApiPhoneSimple> SupportPhoneSimpleList()
        {
            return SupportPhoneList().Select(p => p as VmOpenApiPhoneSimple).ToList();
        }

        private IList<VmOpenApiLanguageItem> SupportEmailList()
        {
            return new List<VmOpenApiLanguageItem>
            {
                new VmOpenApiLanguageItem {Value = EMAIL_FI, Language = FI.Code},
                new VmOpenApiLanguageItem {Value = EMAIL_SV, Language = SV.Code}
            };
        }
    }
}
