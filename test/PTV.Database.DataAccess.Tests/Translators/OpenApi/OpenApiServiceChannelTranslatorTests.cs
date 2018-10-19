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
using PTV.Database.DataAccess.Translators.OpenApi.Channels;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Common;
using Xunit;
using OpenApiPhoneChannelTranslator = PTV.Database.DataAccess.Translators.OpenApi.Channels.OpenApiPhoneChannelTranslator;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;

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

        private readonly Language FI;
        private readonly Language SV;

        private readonly Guid PhoneTypePhoneId;

        public OpenApiServiceChannelTranslatorTests()
        {
            var typesCacheMock = SetupTypesCacheMock<PhoneNumberType>(typeof (PhoneNumberTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof (ServiceChargeTypeEnum));
            SetupTypesCacheMock<ServiceChannelType>(typeof (ServiceChannelTypeEnum));
            SetupTypesCacheMock<AddressCharacter>(typeof (AddressCharacterEnum));
            SetupTypesCacheMock<AddressType>(typeof(AddressTypeEnum));
            SetupTypesCacheMock<PrintableFormChannelUrlType>(typeof (PrintableFormChannelUrlTypeEnum));
            SetupTypesCacheMock<ServiceHourType>(typeof (ServiceHoursTypeEnum));
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            translators = new List<object>()
            {
                new OpenApiServiceChannelNameTranslators(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceChannelDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceHourTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceHoursAdditionalInformationTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceChannelWebPageTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiEChannelTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiEChannelUrlTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAttachmentTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAttachmentWithTypeTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceLocationChannelTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//LGE-SLCA                new OpenApiServiceLocationChannelAddressInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new AddressTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetAddressTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new CountryCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPhoneChannelTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiWebPageChannelTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiWebPageChannelUrlTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives),
                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceHourTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ExceptionHoursStatusTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new AttachmentTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPrintableFormChannelTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiPrintableFormChannelUrlTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAddressDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiEChannelMainInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiEChannelInTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChargeTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPhoneChannelInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceChannelLanguageTranslator(ResolveManager, TranslationPrimitives, LanguageCache),
                new OpenApiWebPageChannelMainInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiWebPageChannelInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPrintableFormChannelMainInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiPrintableFormChannelInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostalCodeTranslator(ResolveManager, TranslationPrimitives),
                new MunicipalityCodeTranslator(ResolveManager, TranslationPrimitives),
                new PrintableFormChannelUrlTypeEnumCodeTranslator(ResolveManager, TranslationPrimitives),
                new WebPageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceLocationChannelMainInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//LGE-SLCA                new OpenApiServiceLocationChannelInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiWebPageTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAddressInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiPhoneTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceChannelEmailTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceChannelAttachmentTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceChannelEmailTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new EmailTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelPhoneTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceChannelPhoneTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new PhoneTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
                new OpenApiPhoneWithTypeTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiDailyOpeningHourTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceChannelPhoneWithTypeTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//LGE-SLCA                new OpenApiServiceLocationChannelAddressTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiEmailTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAddressTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new OpenApiMunicipalityTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiMunicipalityNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiDialCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostalCodeNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiFormIdentifierTranslator(ResolveManager, TranslationPrimitives, LanguageCache),
//LGE-DA                new OpenApiFormReceiverTranslator(ResolveManager, TranslationPrimitives, LanguageCache),
                new OpenApiServiceChannelLanguageAvailabilityTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiPostOfficeBoxNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAddressTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new OpenApiEmailItemTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiAddressPostOfficeBoxTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressStreetTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiPostOfficeBoxTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiPostOfficeBoxInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiAddressDeliveryTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new OpenApiStreetTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiStreetWithCoordinatesTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new OpenApiAddressOtherTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new OpenApiServiceChannelServiceHourTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiEChannelUrlInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiWebPageChannelUrlInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceChannelAddressInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new V8OpenApiAddressInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiFormReceiverTranslator(ResolveManager, TranslationPrimitives, CacheManager.LanguageCache),
                new OpenApiSocialHealthCenterTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiDeliveryAddressInTranslator(ResolveManager, TranslationPrimitives, CacheManager)
            };

            // set languages
            var localizationFi = "fi";
            var localizationSv = "sv";
            var localizationIdFi = CacheManager.LanguageCache.Get(localizationFi);
            var localizationIdSv = CacheManager.LanguageCache.Get(localizationSv);
            FI = new Language {Id = localizationIdFi, Code = localizationFi};
            SV = new Language {Id = localizationIdSv, Code = localizationSv};

            PhoneTypePhoneId = CacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
        }

        [Fact]
        public void TranslateEChannelToVm()
        {
            var channel = CreateEChannel();
            var toTranslate = new List<ServiceChannelVersioned> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannelVersioned source, VmOpenApiElectronicChannelVersionBase target)
        {
            target.ServiceChannelType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId)).And.Be(ServiceChannelTypeEnum.EChannel.ToString());
            target.WebPages.Count.Should().Be(source.ElectronicChannels.First().LocalizedUrls.Count).And.Be(2);
            target.Attachments.First().Name.Should().Be(source.Attachments.First().Attachment.Name).And.Be(TestDataFactory.ATTACHMENT_NAME);
            CheckSupportContacts(source, target, true);
            CheckOpeningHours(source, target, true);
        }

        [Fact]
        public void TranslateServiceLocationChannelToVm()
        {
            var channel = CreateLocationServiceChannel();
            var toTranslate = new List<ServiceChannelVersioned> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannelVersioned source, VmOpenApiServiceLocationChannelVersionBase target)
        {
            target.ServiceChannelType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId));
            
            var targetAddress = target.Addresses.FirstOrDefault();
//LGE-SLCA            var sourceAddress = source.ServiceLocationChannels.First().Addresses.First();
//LGE-SLCA            targetAddress.Type.Should().Be(CacheManager.TypesCache.GetByValue<AddressCharacter>(sourceAddress.CharacterId));
//LGE-SLCA            targetAddress.Country.Should().Be(sourceAddress.Address.Country.Code);
            target.Languages.Count.Should().Be(2);
            CheckSupportContacts(source, target, true);
            CheckOpeningHours(source, target, true);
        }

        [Fact]
        public void TranslatePhoneChannelToVm()
        {
            var channel = CreatePhoneServiceChannel();
            var toTranslate = new List<ServiceChannelVersioned> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannelVersioned source, IVmOpenApiPhoneChannelVersionBase target)
        {
            CheckSupportContactsPhoneChannelToVm(source, target, true);
            CheckOpeningHours(source, target, true);
        }

        [Fact]
        public void TranslateWebpageChannelToVm()
        {
            var channel = CreateWebpageChannel();
            var toTranslate = new List<ServiceChannelVersioned> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation);
        }

        private void CheckTranslations(ServiceChannelVersioned source, VmOpenApiWebPageChannelVersionBase target)
        {
            target.ServiceChannelType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId)).And.Be(ServiceChannelTypeEnum.WebPage.ToString());
//             target.Attachments.First().Name.Should().Be(source.WebpageChannels.First().Attachments.First().Attachment.Name).And.Be(TestDataFactory.ATTACHMENT_NAME);
            target.WebPages.Count.Should().Be(source.WebpageChannels.First().LocalizedUrls.Count);
            CheckSupportContacts(source, target, true);
            CheckOpeningHours(source, target, true);
        }

        [Fact]
        public void TranslatePrintableFormChannelToVm()
        {
            var channel = CreatePrintableFormChannel();
            var toTranslate = new List<ServiceChannelVersioned> {channel};
            var translations = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(channel, translation, true);
        }

        private void CheckTranslations(ServiceChannelVersioned source, VmOpenApiPrintableFormChannelVersionBase target, bool openApiEnum = false)
        {
            target.ServiceChannelType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChannelType>(source.TypeId)).And.Be(ServiceChannelTypeEnum.PrintableForm.ToString());
            //target.Attachments.First().Type.Should().Be(source.PrintableFormChannels.First().Attachments.First().Attachment.Type.Code).And.Be(AttachmentTypeEnum.Attachment.ToString());
            target.ChannelUrls.First().Type.Should().Be(CacheManager.TypesCache.GetByValue<PrintableFormChannelUrlType>(source.PrintableFormChannels.First().ChannelUrls.First().TypeId)).And.Be(PrintableFormChannelUrlTypeEnum.DOC.ToString());
            //target.FormReceiver.Should().Be(source.PrintableFormChannels.First().FormReceivers.First().FormReceiver).And.Be(SERVICE_CHANNEL_TEXT);
            CheckSupportContacts(source, target, openApiEnum);
            CheckOpeningHours(source, target, openApiEnum);
        }

        [Fact]
        public void TranslateEChannelVmToEntity()
        {
            var vm = CreateEChannelVm();
            var toTranslate = new List<VmOpenApiElectronicChannelInVersionBase> {vm};
            var translations = RunTranslationModelToEntityTest<VmOpenApiElectronicChannelInVersionBase, ServiceChannelVersioned>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IVmOpenApiElectronicChannelInVersionBase source, ServiceChannelVersioned target)
        {
            target.ServiceChannelNames.Count.Should().Be(1);
            target.ServiceChannelServiceHours.First().ServiceHours.DailyOpeningTimes.First().DayFrom.Should().Be(0);

            // check attachement
            target.Attachments.Should().NotBeNull();
            target.Attachments.Count.Should().Be(source.Attachments.Count);

            var eChannel = target.ElectronicChannels.FirstOrDefault();
            Assert.NotNull(eChannel);
            eChannel.SignatureQuantity.Should().Be(2);
            eChannel.LocalizedUrls.Count.Should().Be(1);
            CheckSupportContacts(target, source);
            CheckOpeningHours(target, source);
        }

        [Fact]
        public void TranslatePhoneChannelVmToEntity()
        {
            var vm = CreatePhoneChannelVm();
            var toTranslate = new List<VmOpenApiPhoneChannelInVersionBase> {vm};
            var translations = RunTranslationModelToEntityTest<VmOpenApiPhoneChannelInVersionBase, ServiceChannelVersioned>(translators, toTranslate);
            var translation = translations.First();
            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IVmOpenApiPhoneChannelInVersionBase source, ServiceChannelVersioned target)
        {
            CheckSupportContactsPhoneChannelToEntity(source, target);
            CheckOpeningHours(target, source);
        }

        [Fact]
        public void TranslateWebPageChannelVmToEntity()
        {
            var vm = CreateWebPageChannelVm();
            var toTranslate = new List<VmOpenApiWebPageChannelInVersionBase> {vm};
            var translations = RunTranslationModelToEntityTest<VmOpenApiWebPageChannelInVersionBase, ServiceChannelVersioned>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IVmOpenApiWebPageChannelInVersionBase source, ServiceChannelVersioned target)
        {
            target.Languages.Count.Should().Be(1);
            var webPageChannel = target.WebpageChannels.FirstOrDefault();
            Assert.NotNull(webPageChannel);
            webPageChannel.LocalizedUrls.Count.Should().Be(source.WebPage.Count);
            CheckSupportContacts(target, source);
            CheckOpeningHours(target, source);
        }

        [Fact]
        public void TranslatePrintableFormChannelVmToEntity()
        {
            var vm = CreatePrintableFormChannelVm();
            var toTranslate = new List<VmOpenApiPrintableFormChannelInVersionBase>() {vm};
            var translations = RunTranslationModelToEntityTest<VmOpenApiPrintableFormChannelInVersionBase, ServiceChannelVersioned>(translators,toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        private void CheckTranslations(IVmOpenApiPrintableFormChannelInVersionBase source, ServiceChannelVersioned target)
        {
            var channel = target.PrintableFormChannels.FirstOrDefault();
            Assert.NotNull(channel);
            //channel.FormIdentifiers.First().FormIdentifier.Should().Be(TestDataFactory.TEXT);
            channel.ChannelUrls.Count.Should().Be(1);
            //channel.DeliveryAddressDescriptions.Count.Should().Be(1);
            //channel.Attachments.Count.Should().Be(1);

            CheckSupportContacts(target, source);
            CheckOpeningHours(target, source);
        }

        [Fact]
        public void TranslateServiceLocationChannelVmToEntity()
        {
            var vm = CreateServiceLocationChannelVm();
            var toTranslate = new List<VmOpenApiServiceLocationChannelInVersionBase> {vm};
            var translations = RunTranslationModelToEntityTest<VmOpenApiServiceLocationChannelInVersionBase, ServiceChannelVersioned>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vm, translation);
        }

        [Fact]
        public void TranslateServiceLocationChannelPhonesVmToEntity()
        {
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                PhoneNumbers = SupportPhoneList(),
                FaxNumbers = SupportPhoneSimpleList()
            };
            var toTranslate = new List<VmOpenApiServiceLocationChannelInVersionBase> { vm };
            unitOfWorkMockSetup.Setup(x => x.GetSet<ServiceChannelVersioned>()).Returns(new TestDbSet<ServiceChannelVersioned>(new List<ServiceChannelVersioned>()));
            unitOfWorkMockSetup.Setup(x => x.GetSet<ServiceChannelType>()).Returns(new TestDbSet<ServiceChannelType>(new List<ServiceChannelType> { new ServiceChannelType
            {
                Id = TypeCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString()),
                Code = ServiceChannelTypeEnum.ServiceLocation.ToString()
            } }));
            unitOfWorkMockSetup.Setup(x => x.GetSet<ServiceChannelPhone>()).Returns(new TestDbSet<ServiceChannelPhone>(new List<ServiceChannelPhone>()));
            unitOfWorkMockSetup.Setup(x => x.GetSet<Phone>()).Returns(new TestDbSet<Phone>(new List<Phone>()));
            //unitOfWorkMockSetup.Setup(x => x.GetSet<ServiceChannelDescription>()).Returns(new TestDbSet<ServiceChannelDescription>(new List<ServiceChannelDescription>()));
            var translations = RunTranslationModelToEntityTest<VmOpenApiServiceLocationChannelInVersionBase, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMockSetup.Object);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            translation.Phones.Count.Should().Be(vm.FaxNumbers.Count + vm.PhoneNumbers.Count);
        }
        private void CheckTranslations(IVmOpenApiServiceLocationChannelInVersionBase source, ServiceChannelVersioned target)
        {
            target.Languages.Count.Should().Be(source.Languages.Count).And.Be(1);
            //target.ServiceHours.Count.Should().Be(source.ServiceHours.Count).And.Be(1);
            target.WebPages.Count.Should().Be(source.WebPages.Count).And.Be(1);
//LGE-SLCA            var channel = target.ServiceLocationChannels.FirstOrDefault();
//LGE-SLCA            Assert.NotNull(channel);
//LGE-SLCA            channel.Addresses.Count.Should().Be(1);

            CheckOpeningHours(target, source);

            CheckSupportContactEmail(target, source, FI);
            CheckSupportContactEmail(target, source, SV);
        }

        private void CheckSupportContactsPhoneChannelToVm(ServiceChannelVersioned source, IVmOpenApiPhoneChannelVersionBase target, bool openApiEnum = false)
        {
            source.Phones.Count.Should().Be(target.PhoneNumbers.Count);
            source.Emails.Count.Should().Be(target.SupportEmails.Count);
            CheckSupportContactEmail(source, target, FI);
            CheckSupportContactEmail(source, target, SV);
            CheckPhoneChannelPhoneNumbers(source, target, FI, openApiEnum);
            CheckPhoneChannelPhoneNumbers(source, target, SV, openApiEnum);
        }

        private void CheckSupportContactsPhoneChannelToEntity(IVmOpenApiPhoneChannelInVersionBase target, ServiceChannelVersioned source)
        {
            source.Phones.Count.Should().Be(target.PhoneNumbers.Count);
            source.Emails.Count.Should().Be(target.SupportEmails.Count);
            CheckSupportContactEmail(source, target, FI);
            CheckSupportContactEmail(source, target, SV);
            CheckPhoneChannelInPhoneNumbers(source, target, FI);
            CheckPhoneChannelInPhoneNumbers(source, target, SV);
        }

        private void CheckSupportContacts(ServiceChannelVersioned source, IVmOpenApiServiceChannelBase target, bool openApiEnum = false)
        {
            var phones = source.Phones.Where(p => p.Phone.TypeId == PhoneTypePhoneId).ToList();
            phones.Count.Should().Be(target.SupportPhones.Count);
            CheckSupportContactsBase(source, target, openApiEnum);
        }

        private void CheckSupportContactsBase(ServiceChannelVersioned source, IVmOpenApiServiceChannelBase target, bool openApiEnum = false)
        {
            source.Emails.Count.Should().Be(target.SupportEmails.Count);
            CheckSupportContactEmail(source, target, FI);
            CheckSupportContactEmail(source, target, SV);
            CheckSupportContactPhone(source, target, FI, openApiEnum);
            CheckSupportContactPhone(source, target, SV, openApiEnum);
        }

        private static void CheckSupportContactEmail(ServiceChannelVersioned source, IVmOpenApiServiceChannelBase target, Language language)
        {
            var targetEmail = target.SupportEmails.First(e => e.Language != null && e.Language == language.Code);
            var sourceEmail = source.Emails.First(e => e.Email.LocalizationId == language.Id);
            targetEmail.Value.Should().Be(sourceEmail.Email.Value);
        }

        private void CheckSupportContactPhone(ServiceChannelVersioned source, IVmOpenApiServiceChannelBase target, Language language, bool openApiEnum = false)
        {
            var targetPhone = target.SupportPhones.SingleOrDefault(e => e.Language != null && e.Language == language.Code);
            if (targetPhone == null) return;

            var sourcePhone = source.Phones.SingleOrDefault(e => e.Phone.LocalizationId == language.Id).Phone;
            if (sourcePhone == null) return;

            CheckPhone(sourcePhone, targetPhone, openApiEnum);
        }

        private void CheckPhoneChannelPhoneNumbers(ServiceChannelVersioned source, IVmOpenApiPhoneChannelVersionBase target, Language language, bool openApiEnum = false)
        {
            var targetPhone = target.PhoneNumbers.SingleOrDefault(e => e.Language != null && e.Language == language.Code);
            if (targetPhone == null) return;

            var sourcePhone = source.Phones.SingleOrDefault(e => e.Phone.LocalizationId == language.Id).Phone;
            if (sourcePhone == null) return;

            CheckPhoneChannelPhone(sourcePhone, targetPhone, openApiEnum);
        }

        private void CheckPhoneChannelInPhoneNumbers(ServiceChannelVersioned source, IVmOpenApiPhoneChannelInVersionBase target, Language language)
        {
            var targetPhone = target.PhoneNumbers.SingleOrDefault(e => e.Language != null && e.Language == language.Code);
            if (targetPhone == null) return;

            var sourcePhone = source.Phones.SingleOrDefault(e => e.Phone.LocalizationId == language.Id).Phone;
            if (sourcePhone == null) return;

            CheckPhoneChannelPhone(sourcePhone, targetPhone);
        }

        private void CheckPhone(Phone sourcePhone, IV4VmOpenApiPhone targetPhone, bool openApiEnum = false)
        {
            targetPhone.Number.Should().Be(sourcePhone.Number);
            targetPhone.AdditionalInformation.Should().Be(sourcePhone.AdditionalInformation);
            targetPhone.ChargeDescription.Should().Be(sourcePhone.ChargeDescription);
            if (openApiEnum)
            {
                targetPhone.ServiceChargeType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChargeType>(sourcePhone.ChargeTypeId).GetOpenApiEnumValue<ServiceChargeTypeEnum>());
            }
            else
            {
                targetPhone.ServiceChargeType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChargeType>(sourcePhone.ChargeTypeId));
            }
        }

        private void CheckPhoneChannelPhone(Phone sourcePhone, V4VmOpenApiPhoneWithType targetPhone, bool openApiEnum = false)
        {
            targetPhone.Number.Should().Be(sourcePhone.Number);
            targetPhone.AdditionalInformation.Should().Be(sourcePhone.AdditionalInformation);
            targetPhone.ChargeDescription.Should().Be(sourcePhone.ChargeDescription);
            if (openApiEnum)
            {
                targetPhone.ServiceChargeType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChargeType>(sourcePhone.ChargeTypeId).GetOpenApiEnumValue<ServiceChargeTypeEnum>());
            }
            else
            {
                targetPhone.ServiceChargeType.Should().Be(CacheManager.TypesCache.GetByValue<ServiceChargeType>(sourcePhone.ChargeTypeId));
            }
        }

        private void CheckPhoneWithType(Phone sourcePhone, V4VmOpenApiPhoneWithType targetPhone)
        {
            CheckPhone(sourcePhone, targetPhone);
            targetPhone.Type.Should().Be(CacheManager.TypesCache.GetByValue<PhoneNumberType>(sourcePhone.TypeId));
        }

        private void CheckOpeningHours(ServiceChannelVersioned source, IVmOpenApiServiceChannelBase target, bool openApiEnum = false)
        {
            var sourceHours = source.ServiceChannelServiceHours.Select(i => i.ServiceHours).ToList().Single();
            var targetHours = target.ServiceHours.Single();

            if (openApiEnum)
            {
                sourceHours.ServiceHourTypeId.Should().Be(targetHours.ServiceHourType.GetEnumValueByOpenApiEnumValue<ServiceHoursTypeEnum>().GetGuid());
            }
            else
            {
                sourceHours.ServiceHourTypeId.Should().Be(targetHours.ServiceHourType.GetGuid());
            }

            targetHours.AdditionalInformation.Count.Should().Be(sourceHours.AdditionalInformations.Count);
            var sourceAdditionalInfo = sourceHours.AdditionalInformations.FirstOrDefault();
            var targetAdditionalInfo = targetHours.AdditionalInformation.FirstOrDefault();
            targetAdditionalInfo.Language.Should().Be(CacheManager.LanguageCache.GetByValue(sourceAdditionalInfo.LocalizationId));
            targetAdditionalInfo.Value.Should().Be(sourceAdditionalInfo.Text);

            targetHours.OpeningHour.Count.Should().Be(sourceHours.DailyOpeningTimes.Count);
            var sourceDailyOpeningTimes = sourceHours.DailyOpeningTimes.FirstOrDefault();
            var targetOpeningHour = targetHours.OpeningHour.FirstOrDefault();
            targetOpeningHour.DayFrom.Should().Be(((WeekDayEnum) sourceDailyOpeningTimes.DayFrom).ToString());
            targetOpeningHour.From.Should().Be(sourceDailyOpeningTimes.From.ToString());
            targetOpeningHour.To.Should().Be(sourceDailyOpeningTimes.To.ToString());
        }

        private ServiceChannelVersioned CreateServiceChannel()
        {
            return new ServiceChannelVersioned()
            {
                ServiceChannelNames = new List<ServiceChannelName>()
                {
                    new ServiceChannelName()
                    {
                        Name = SERVICE_CHANNEL_NAME,
                        Localization = FI,
                        TypeId = CacheManager.TypesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                        Type = new NameType() {Code = NameTypeEnum.Name.ToString()}
                    },
                    new ServiceChannelName()
                    {
                        Name = "TestName",
                        TypeId = CacheManager.TypesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())
                    }
                },
                ServiceChannelDescriptions = new List<ServiceChannelDescription>()
                {
                    new ServiceChannelDescription()
                    {
                        Description = SERVICE_CHANNEL_DESCRIPTION,
                        Localization = TestDataFactory.LocalizationSv(),
                        TypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()),
                    }
                },
                ServiceChannelServiceHours = new List<ServiceChannelServiceHours>()
                {
                    new ServiceChannelServiceHours
                    {
                        ServiceHours = new ServiceHours
                        {
                            IsClosed = false,
                            ServiceHourTypeId = ServiceHoursTypeEnum.Standard.ToString().GetGuid(),
                            DailyOpeningTimes = new List<DailyOpeningTime>
                            {
                                new DailyOpeningTime
                                {
                                    DayFrom = (int) WeekDayEnum.Monday,
                                    From = new TimeSpan(10, 00, 00),
                                    To = new TimeSpan(14, 00, 00)
                                }
                            },
                            AdditionalInformations = new List<ServiceHoursAdditionalInformation>()
                            {
                                new ServiceHoursAdditionalInformation()
                                {
                                    Text = SERVICE_CHANNEL_ADDITIONAL_INFO_TEXT,
                                    LocalizationId = FI.Id
                                }
                            }
                        }
                    }
                }
                ,
                Phones = new List<ServiceChannelPhone>
                {
                    new ServiceChannelPhone
                    {
                        Phone = new Phone
                        {
                            LocalizationId = FI.Id,
                            PrefixNumber = new DialCode { Code = PHONE_PREFIX_NUMBER_FI },
                            Number = PHONE_NUMBER_FI,
                            TypeId = CacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                            ChargeTypeId = CacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()),
                            AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI
                        }
                    },
                    new ServiceChannelPhone
                    {
                        Phone = new Phone
                        {
                            LocalizationId = SV.Id,
                            PrefixNumber = new DialCode { Code = PHONE_PREFIX_NUMBER_SV },
                            Number = PHONE_NUMBER_SV,
                            TypeId = CacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()),
                            ChargeTypeId = CacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()),
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

        private ServiceChannelVersioned CreateEChannel()
        {
            var eChannel = CreateServiceChannel();
            eChannel.TypeId = CacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString());
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

        private ServiceChannelVersioned CreateLocationServiceChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = CacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            channel.Languages = new List<ServiceChannelLanguage>()
            {
                new ServiceChannelLanguage() {Language = TestDataFactory.LocalizationFi()},
                new ServiceChannelLanguage() {Language = TestDataFactory.LocalizationSv()}
            };
            /*LGE-SLCAchannel.ServiceLocationChannels = new List<ServiceLocationChannel>()
            {
                new ServiceLocationChannel()
                {
                    Addresses = new List<ServiceLocationChannelAddress>()
                    {
                        new ServiceLocationChannelAddress()
                        {
                            Address = TestDataFactory.CreateAddress(CacheManager.TypesCache.Get<AddressType>(AddressTypeEnum.Street.ToString())),
                            CharacterId = CacheManager.TypesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString())
                        }
                    }
                }
            };*/

            return channel;
        }

        private ServiceChannelVersioned CreatePhoneServiceChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = CacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString());
            channel.Phones = new List<ServiceChannelPhone>
            {
                new ServiceChannelPhone
                {
                    Phone = new Phone
                    {
                        LocalizationId = FI.Id,
                        PrefixNumber = new DialCode { Code = PHONE_PREFIX_NUMBER_FI },
                        Number = PHONE_NUMBER_FI,
                        TypeId = CacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString()),
                        ChargeTypeId = CacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()),
                        AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI
                    }
                },
                new ServiceChannelPhone
                {
                    Phone = new Phone
                    {
                        LocalizationId = SV.Id,
                        PrefixNumber = new DialCode { Code = PHONE_PREFIX_NUMBER_SV },
                        Number = PHONE_NUMBER_SV,
                        TypeId = CacheManager.TypesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Sms.ToString()),
                        ChargeTypeId = CacheManager.TypesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()),
                        ChargeDescription = PHONE_DESCRIPTION_CHARGE_DESCRIPTION_SV,
                    }
                }
            };
            return channel;
        }

        private ServiceChannelVersioned CreateWebpageChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = CacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString());
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

        private ServiceChannelVersioned CreatePrintableFormChannel()
        {
            var channel = CreateServiceChannel();
            channel.TypeId = CacheManager.TypesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());
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
                            TypeId = CacheManager.TypesCache.Get<PrintableFormChannelUrlType>(PrintableFormChannelUrlTypeEnum.DOC.ToString())
                        }
                    },
//LGE-DA                    DeliveryAddress = TestDataFactory.CreateAddress(CacheManager.TypesCache.Get<AddressType>(AddressTypeEnum.Street.ToString())),
//                DeliveryAddressDescriptions = new List<PrintableFormChannelDeliveryAddressDescription>()
//                {
//                    new PrintableFormChannelDeliveryAddressDescription() { Description = SERVICE_CHANNEL_DESCRIPTION }
//                },
                    FormIdentifiers =  new List<PrintableFormChannelIdentifier>()
                    {
                        new PrintableFormChannelIdentifier()
                        {
                            FormIdentifier = SERVICE_CHANNEL_TEXT,
                            Localization = TestDataFactory.LocalizationFi(),
                         }
                    },
//LGE-DA                    FormReceivers =  new List<PrintableFormChannelReceiver>()
//LGE-DA                    {
//LGE-DA                        new PrintableFormChannelReceiver()
//LGE-DA                        {
//LGE-DA                            FormReceiver = SERVICE_CHANNEL_TEXT,
//LGE-DA                            Localization = TestDataFactory.LocalizationFi(),
//LGE-DA                         }
//LGE-DA                    }
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
            vm.ServiceChannelNamesWithType = TestDataFactory.CreateLocalizedList(NameTypeEnum.Name.ToString()).ToList();
            vm.ServiceChannelDescriptions = TestDataFactory.CreateLocalizedList(DescriptionTypeEnum.Description.ToString()).ToList();
            vm.PublishingStatus = PublishingStatus.Published.ToString();
        }

        private VmOpenApiElectronicChannelInVersionBase CreateEChannelVm()
        {
            var vm = new VmOpenApiElectronicChannelInVersionBase();
            SetVmValues(vm);
            vm.ServiceHours = ServiceHourList();
            vm.SupportPhones = SupportPhoneList();
            vm.SupportEmails = SupportEmailList();
            vm.SignatureQuantity = 2.ToString();
            vm.WebPage = TestDataFactory.CreateLanguageItemList().ToList();
            vm.Attachments = new List<VmOpenApiAttachment> {TestDataFactory.CreateAttachmentVm()};

            return vm;
        }

        private VmOpenApiPhoneChannelInVersionBase CreatePhoneChannelVm()
        {
            var vm = new VmOpenApiPhoneChannelInVersionBase();
            SetVmValues(vm);
            vm.Languages = new List<string>() { "fi" };
            vm.PhoneNumbers = PhoneChannelPhoneList();
            vm.SupportEmails = SupportEmailList();
            vm.ServiceHours = ServiceHourList();
            return vm;
        }

        private VmOpenApiWebPageChannelInVersionBase CreateWebPageChannelVm()
        {
            var vm = new VmOpenApiWebPageChannelInVersionBase();
            SetVmValues(vm);
            vm.WebPage = TestDataFactory.CreateLanguageItemList().ToList();
            vm.SupportPhones = SupportPhoneList();
            vm.SupportEmails = SupportEmailList();
            vm.Languages = new List<string> {"fi"};
            vm.ServiceHours = ServiceHourList();
            return vm;
        }

        private VmOpenApiPrintableFormChannelInVersionBase CreatePrintableFormChannelVm()
        {
            var vm = new VmOpenApiPrintableFormChannelInVersionBase();
            SetVmValues(vm);
            vm.FormIdentifier = TestDataFactory.CreateLanguageItemList().ToList();
            vm.DeliveryAddresses = TestDataFactory.CreateListOfAddressVm();
            vm.ChannelUrls = TestDataFactory.CreateLocalizedList(PrintableFormChannelUrlTypeEnum.DOC.ToString()).ToList();
            vm.SupportPhones = SupportPhoneList();
            vm.SupportEmails = SupportEmailList();
            vm.SupportPhones = new List<V4VmOpenApiPhone>();
            vm.Attachments = new List<VmOpenApiAttachment> {TestDataFactory.CreateAttachmentVm()};
            vm.ServiceHours = ServiceHourList();
            return vm;
        }

        private VmOpenApiServiceLocationChannelInVersionBase CreateServiceLocationChannelVm()
        {
            var vm = new VmOpenApiServiceLocationChannelInVersionBase();
            SetVmValues(vm);
            vm.Languages = new List<string>() {TestDataFactory.TEXT};
            vm.WebPages = new List<V9VmOpenApiWebPage>() {new V9VmOpenApiWebPage() {Url = TestDataFactory.URI}};
            vm.Addresses = new List<V9VmOpenApiAddressLocationIn> {TestDataFactory.CreateAddressVm<V9VmOpenApiAddressLocationIn>(AddressTypeEnum.PostOfficeBox)};
            vm.ServiceHours = ServiceHourList();
            vm.PhoneNumbers = SupportPhoneList();
            vm.FaxNumbers = SupportPhoneSimpleList();
            vm.SupportEmails = SupportEmailList();
            return vm;
        }

        private IList<V8VmOpenApiServiceHour> ServiceHourList()
        {
            return new List<V8VmOpenApiServiceHour>()
            {
                new V8VmOpenApiServiceHour()
                {
                    //Monday = true,
                    ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                    IsClosed = false,
                    OpeningHour = new List<V8VmOpenApiDailyOpeningTime>
                    {
                        new V8VmOpenApiDailyOpeningTime()
                        {
                            DayFrom = WeekDayEnum.Monday.ToString(),
                            From = "10:00:00",
                            To = "14:00:00"
                        }
                    },
                    AdditionalInformation = new List<VmOpenApiLanguageItem>
                    {
                        new VmOpenApiLanguageItem
                        {
                            Value = SERVICE_CHANNEL_ADDITIONAL_INFO_TEXT,
                            Language = FI.Code
                        }
                    }
                }
            };
        }

        private IList<V4VmOpenApiPhoneWithType> PhoneWithTypeList(PhoneNumberTypeEnum phoneNumberType = PhoneNumberTypeEnum.Phone)
        {
            return new List<V4VmOpenApiPhoneWithType>
            {
                new V4VmOpenApiPhoneWithType
                {
                    //PrefixNumber = PHONE_PREFIX_NUMBER_FI,
                    Number = PHONE_NUMBER_FI,
                    Language = FI.Code,
                    AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_FI,
                    ChargeDescription = PHONE_DESCRIPTION_CHARGE_DESCRIPTION_FI,
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                    Type = phoneNumberType.ToString()
                },
                new V4VmOpenApiPhoneWithType
                {
                    //PrefixNumber = PHONE_PREFIX_NUMBER_SV,
                    Number = PHONE_NUMBER_SV,
                    Language = SV.Code,
                    AdditionalInformation = PHONE_DESCRIPTION_ADDITIONAL_INFORMATION_SV,
                    ChargeDescription = PHONE_DESCRIPTION_CHARGE_DESCRIPTION_SV,
                    ServiceChargeType = ServiceChargeTypeEnum.Free.ToString(),
                    Type = phoneNumberType.ToString()
                }
            };
        }

        private IList<V4VmOpenApiPhone> SupportPhoneList()
        {
            return PhoneWithTypeList().Select(p => p as V4VmOpenApiPhone).ToList();
        }

        private IList<V4VmOpenApiPhoneWithType> PhoneChannelPhoneList()
        {
            return PhoneWithTypeList();
        }

        private IList<V4VmOpenApiPhoneSimple> SupportPhoneSimpleList()
        {
            return SupportPhoneList().Select(p => p as V4VmOpenApiPhoneSimple).ToList();
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
