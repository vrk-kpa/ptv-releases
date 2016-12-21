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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class LocationChannelMainTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;
        private ServiceChannel result;

        public LocationChannelMainTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var cacheManagerMock = SetupCacheManager();
            var duplicatedEntity = new ServiceChannel { CreatedBy = "ITranslator<ServiceChannel, IVmChannelDescription>>" };

            translators = new List<object>()
            {
                new LocationChannelMainStep1Translator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(cacheManagerMock.Object)),
                new LocationChannelMainStep2Translator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(cacheManagerMock.Object)),
                new LocationChannelMainStep3Translator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, IVmChannelDescription>>(), unitOfWorkMock,
                    desc =>
                    {
                        var x = TranslatedInstanceStorage.ProcessInstance(desc, duplicatedEntity);
                        x.OrganizationId = desc.OrganizationId ?? Guid.Empty;
                        x.ServiceChannelNames.Add(new ServiceChannelName { Name = desc.Name });
                        return x;
                    }),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLocationChannel, VmLocationChannelStep1>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLocationChannel, VmLocationChannelStep2>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLocationChannel, VmLocationChannelStep3>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, VmOpeningHoursStep>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, IPhoneNumberAndFax>>(), unitOfWorkMock,
                    phones =>
                    {
                        var x = TranslatedInstanceStorage.ProcessInstance(phones, duplicatedEntity);
                        x.Phones.Add(new ServiceChannelPhone { CreatedBy = phones.PhoneNumber.Number });
                        return x;
                    }),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, IEmail>>(), unitOfWorkMock,
                    email =>
                    {
                        var x = TranslatedInstanceStorage.ProcessInstance(email, duplicatedEntity);
                        x.Emails.Add(new ServiceChannelEmail { Email = new Email { Value = email.Email.Email} });
                        return x;
                    }),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelLanguage, VmListItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelWebPage, VmWebPage>>(), unitOfWorkMock),

            };
            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);

            RegisterDbSet(new List<ServiceChannel>(), unitOfWorkMockSetup);

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateLocationChannel()
        {
            var model = CreateModel();

            var toTranslate = new List<VmLocationChannel>() { model };
            var typesCacheMock = SetupTypesCacheMock<ServiceChannelType>();
            var mainTranslators = new List<object>
            {
                new LocationChannelMainTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object, new EntityDefinitionHelper()),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, VmLocationChannelStep1>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, VmLocationChannelStep2>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, VmLocationChannelStep3>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, VmOpeningHoursStep>>(), unitOfWorkMock)
            };


            var translations = RunTranslationModelToEntityTest<VmLocationChannel, ServiceChannel>(mainTranslators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("11D495DE-E1E7-4B57-BC86-20A87BA83324", 5)]
        [InlineData("", 0)]
        public void TranslateLocationChannelStep1(string organization, int selectedLanguages)
        {
            var model = CreateModel().Step1Form;
            model.OrganizationId = conversion.GetGuid(organization);
            model.Languages = itemListGenerator.Create<VmListItem>(selectedLanguages).Select(x=>x.Id).ToList();

            var toTranslate = new List<VmLocationChannelStep1>() { model };

            var translations = RunTranslationModelToEntityTest<VmLocationChannelStep1, ServiceChannel>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmLocationChannelStep1 step1, ServiceChannel target)
        {
            if (step1.OrganizationId.HasValue)
            {
                target.OrganizationId.Should().NotBeEmpty();
                target.OrganizationId.Should().Be(step1.OrganizationId.Value);
            }
//            var names = conversion.GetValidTexts(step1.Name);
            target.ServiceChannelNames.Count.Should().Be(1, "ServiceNames");
//            var descriptions = conversion.GetValidTexts(step1.ShortDescription, step1.Description);
//            target.ServiceChannelDescriptions.Count.Should().Be(descriptions.Count, "ServiceDescriptions");
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(0, "", "", "", "", ServiceChargeTypeEnum.Charged)]
        [InlineData(5,  "email", "5456", "456", "description", ServiceChargeTypeEnum.Charged)]
        [InlineData(0,  "email", "5456", "456", "description", ServiceChargeTypeEnum.Free)]
        [InlineData(10, "email", "5456", "456", "description", ServiceChargeTypeEnum.Other)]
        public void TranslateLocationChannelStep2(int count, string email, string phoneNumber, string fax, string chargeDescription, ServiceChargeTypeEnum chargeType)
        {
            var model = CreateModel().Step2Form;
            model.WebPages = itemListGenerator.Create<VmWebPage>(count);


            var toTranslate = new List<VmLocationChannelStep2>() { model };
            model.Email = new VmEmailData {Email = email};
            model.PhoneNumber = new VmPhone
            {
                Number = phoneNumber,
                ChargeTypeId = chargeType.ToString().GetGuid(),
                ChargeDescription = chargeDescription
            };
            model.Fax = new VmPhone {Number = fax};

            var translations = RunTranslationModelToEntityTest<VmLocationChannelStep2, ServiceChannel>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmLocationChannelStep2 step2, ServiceChannel target)
        {
            target.WebPages.Count.Should().Be(step2.WebPages.Count, "WebPages");
            target.Id.Should().NotBe(Guid.Empty);
            // checks that special translator was called
            target.Emails.Count.Should().Be(1);
            target.Emails.First().Email.Value.Should().Be(step2.Email.Email);
            target.Phones.Count.Should().Be(1);
            target.Phones.First().CreatedBy.Should().Be(step2.PhoneNumber.Number);
        }

        private void CheckTranslation(VmLocationChannel source, ServiceChannel target)
        {
            target.Id.Should().NotBe(Guid.Empty);
            target.TypeId.Should().Be(ServiceChannelTypeEnum.ServiceLocation.ToString().GetGuid());
            target.PublishingStatusId.Should().Be(source.PublishingStatus);
        }

        private VmLocationChannel CreateModel()
        {
            return new VmLocationChannel
            {
                PublishingStatus = PublishingStatus.Draft.ToString().GetGuid()
            };
        }
    }
}
