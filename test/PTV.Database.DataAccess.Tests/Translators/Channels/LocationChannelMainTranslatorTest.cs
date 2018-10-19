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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;
using FluentAssertions;
using Moq;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using ServiceChannelTranslationDefinitionHelper = PTV.Database.DataAccess.Translators.Channels.ServiceChannelTranslationDefinitionHelper;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class LocationChannelMainTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;
        private readonly TestConversion conversion;
        private readonly ItemListModelGenerator itemListGenerator;

        public LocationChannelMainTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var duplicatedEntity = new ServiceChannelVersioned() { CreatedBy = "ITranslator<ServiceChannel, IVmChannelDescription>>" };

            translators = new List<object>()
            {
//                new LocationChannelMainStep1Translator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(CacheManager), CacheManager),

//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IVmChannelDescription>>(), unitOfWorkMock,
//                    desc =>
//                    {
//                        var x = TranslatedInstanceStorage.ProcessInstance(desc, duplicatedEntity);
//                        duplicatedEntity.OrganizationId = desc.OrganizationId ?? Guid.Empty;
//                        duplicatedEntity.ServiceChannelNames.Add(new ServiceChannelName { Name = desc.Name });
//                        return duplicatedEntity;
//                    },
//                    setTargetAction: sc => duplicatedEntity = sc
//                    ),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceLocationChannel, VmLocationChannelStep1>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmOpeningHoursStep>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IPhoneNumberAndFax>>(), unitOfWorkMock,
//                    phones =>
//                    {
//                        var x = TranslatedInstanceStorage.ProcessInstance(phones, duplicatedEntity);
//                        duplicatedEntity.Phones.Add(new ServiceChannelPhone { CreatedBy = phones.PhoneNumber.Number });
//                        return duplicatedEntity;
//                    },
//                    setTargetAction: sc => duplicatedEntity = sc
//                    ),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IEmail>>(), unitOfWorkMock,
//                    email =>
//                    {
//                        var x = TranslatedInstanceStorage.ProcessInstance(email, duplicatedEntity);
//                        duplicatedEntity.Emails.Add(new ServiceChannelEmail { Email = new Email { Value = email.Email.Email} });
//                        return duplicatedEntity;
//                    },
//                    setTargetAction: sc => duplicatedEntity = sc
//                    ),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelLanguage, VmListItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelWebPage, VmWebPage>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelEmail, VmEmailData>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelPhone, VmPhone>>(), unitOfWorkMock)
            };
            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);

            RegisterDbSet(new List<ServiceChannel>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<Versioning>(), unitOfWorkMockSetup);

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
            Assert.False(true, "redesign needed");
//            var model = CreateModel();
//
//            var toTranslate = new List<VmLocationChannel>() { model };
//            var typesCacheMock = SetupTypesCacheMock<ServiceChannelType>();
//            var mainTranslators = new List<object>
//            {
//                new LocationChannelMainTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object, new PTV.Database.DataAccess.Translators.Channels.EntityDefinitionHelper()),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannel, VmLocationChannelStep1>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmLocationChannelStep1>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmOpeningHoursStep>>(), unitOfWorkMock)
//            };
//
//
//            var translations = RunTranslationModelToEntityTest<VmLocationChannel, ServiceChannelVersioned>(mainTranslators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//
//            Assert.Equal(toTranslate.Count, translations.Count);
//            CheckTranslation(model, translation);
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("11D495DE-E1E7-4B57-BC86-20A87BA83324", 5)]
        [InlineData("", 0)]
        public void TranslateLocationChannelStep1(string organization, int selectedLanguages)
        {
            Assert.False(true, "redesign needed");
//            var model = CreateModel().Step1Form;
//            model.OrganizationId = conversion.GetGuid(organization);
//            model.Languages = itemListGenerator.Create<VmListItem>(selectedLanguages).Select(x=>x.Id).ToList();
//
//            var toTranslate = new List<VmLocationChannelStep1>() { model };
//
//            var translations = RunTranslationModelToEntityTest<VmLocationChannelStep1, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//
//            Assert.Equal(toTranslate.Count, translations.Count);
//            CheckTranslation(model, translation);
        }

//        private void CheckTranslation(VmLocationChannelStep1 step1, ServiceChannelVersioned target)
//        {
//            if (step1.OrganizationId.HasValue)
//            {
//                target.OrganizationId.Should().NotBeEmpty();
//                target.OrganizationId.Should().Be(step1.OrganizationId.Value);
//            }
////            var names = conversion.GetValidTexts(step1.Name);
//            target.ServiceChannelNames.Count.Should().Be(1, "ServiceNames");
////            var descriptions = conversion.GetValidTexts(step1.ShortDescription, step1.Description);
////            target.ServiceChannelDescriptions.Count.Should().Be(descriptions.Count, "ServiceDescriptions");
//        }

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
            Assert.False(true, "redesign needed");
//            var model = CreateModel().Step1Form;
//            model.WebPages = itemListGenerator.Create<VmWebPage>(count);
//
//            var toTranslate = new List<VmLocationChannelStep1>() { model };
//            model.Emails = new List<VmEmailData>{ new VmEmailData {Email = email}};
//            model.PhoneNumbers = new List<VmPhone> { new VmPhone
//                {
//                    Number = phoneNumber,
//                    ChargeType = chargeType.ToString().GetGuid(),
//                    ChargeDescription = chargeDescription
//                }
//            };
//            model.FaxNumbers = new List<VmPhone> { new VmPhone
//                {
//                    Number = fax
//                }
//            };
//            var translations = RunTranslationModelToEntityTest<VmLocationChannelStep1, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//
//            Assert.Equal(toTranslate.Count, translations.Count);
//            CheckTranslation(model, translation);
        }


        //private void CheckTranslation(VmLocationChannelStep2 step2, ServiceChannel target)
        //{
        //    target.WebPages.Count.Should().Be(step2.WebPages.Count, "WebPages");
        //    target.Id.Should().NotBe(Guid.Empty);
        //    // checks that special translator was called
        //    target.Emails.Count.Should().Be(1);
        //    target.Emails.First().Email.Value.Should().Be(step2.Email.Email);
        //    target.Phones.Count.Should().Be(1);
        //    target.Phones.First().CreatedBy.Should().Be(step2.PhoneNumber.Number);
        //}

//        private void CheckTranslation(VmLocationChannel source, ServiceChannelVersioned target)
//        {
//            //target.Id.Should().NotBe(Guid.Empty);
//            target.TypeId.Should().Be(ServiceChannelTypeEnum.ServiceLocation.ToString().GetGuid());
//            //target.PublishingStatusId.Should().Be(source.PublishingStatusId.Value);
//        }

//        private VmLocationChannel CreateModel()
//        {
//            var poBox = new Dictionary<string, string>();
//            poBox.Add("fi", "pobox");
//            return new VmLocationChannel
//            {
//                PublishingStatusId = PublishingStatus.Draft.ToString().GetGuid(),
//                Step1Form =
//                    new VmLocationChannelStep1()
//                    {
//                        Description = "description",
//                        Language = "fi",
//                        Name = "name",
//                        VisitingAddresses =
//                            new List<VmAddressSimple>()
//                            {
//                                new VmAddressSimple()
//                                {
////                                    Street = "street",
//                                    StreetNumber = "753",
//                                    PoBox = poBox,
//                                    PostalCode = new VmPostalCode() {Code = "postal", PostOffice = "postoffice"}
//                                }
//                            },
//                        PhoneNumbers = new List<VmPhone>
//                        {
//                            new  VmPhone
//                            {
//                                Number = "123456",
//                                DialCode = new VmDialCode {Code = "789", CountryName = "Fi"}
//                            }
//                        },
//                        Emails = new List<VmEmailData>{new VmEmailData {Email = "email"}}
//                    }
//            };
//        }
    }
}
