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
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationMainTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;
        private readonly ItemListModelGenerator itemListGenerator;
        private readonly TestConversion conversion;

        public OrganizationMainTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            SetupTypesCacheMock<PhoneNumberType>(typeof(PhoneNumberTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));

            translators = new List<object>()
            {
//                new OrganizationMainTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock(new Mock<ITranslator<OrganizationAddress, VmAddressSimple>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationDescription, VmDescription>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationEmail, VmEmailData>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationName, VmName>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<Business, VmBusiness>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationWebPage, VmWebPage>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationPhone, VmPhone>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationDisplayNameType, VmDispalyNameType>>(), unitOfWorkMock),
            };

            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
            RegisterDbSet(new List<OrganizationVersioned>(), unitOfWorkMockSetup);
            RegisterEntityForVersionManager<OrganizationVersioned>();

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
        }

        [Theory]
        [InlineData("", "", "", "","", false, false, false, 0, 0, 0, 0, 0)]
        [InlineData("name", "altName", "description", "", "", true, false, true, 5, 3, 4, 0, 4)]
        [InlineData("name", "", "description", "", "", true, true, false, 5, 3, 4, 8, 0)]
        [InlineData("name", "altName", "description", "", "", true, true, true, 5, 3, 4, 8, 4)]
        [InlineData("name", "altName", "description", "11D495DE-E1E7-4B57-BC86-20A87BA83324", "C83C80FD-60CF-454B-938A-C2EC85AA5657",  true, true, true, 0, 0, 0, 0, 0)]
        [InlineData("name", "altName", "description", "11D495DE-E1E7-4B57-BC86-20A87BA83324", "C83C80FD-60CF-454B-938A-C2EC85AA5657",  true, true, true, 8, 6, 9, 7, 2)]
        [InlineData("name", "altName", "description", "11D495DE-E1E7-4B57-BC86-20A87BA83324", "C83C80FD-60CF-454B-938A-C2EC85AA5657",  true, true, true, 5, 3, 4, 8, 4)]
        public void TranslateOrganizationStep1ToEntityTest(string name, string alternateName, string description, string parentOrganizationId, string municipalityId,
            bool showContacts, bool ShowPostalAddress, bool ShowVisitingAddress,
            int selectedEmails, int selectedPhoneNumbers, int selectedWebPages, int postalAddresess, int visitingAddresses)
        {
            Assert.False(true, "redesign needed");
            //            var model = CreateModel();
            //            model.PublishingStatusId = PublishingStatus.Draft.ToString().GetGuid();
            //
            //            model.Step1Form.OrganizationName = name;
            //            model.Step1Form.OrganizationAlternateName = alternateName;
            //            model.Step1Form.Description = description;
            //            model.Step1Form.ParentId = conversion.GetGuid(parentOrganizationId);
            //            Guid? municipalityGuid = conversion.GetGuid(municipalityId);
            //            model.Step1Form.Municipality = municipalityGuid.IsAssigned() ? new VmListItem {Id = municipalityGuid ?? Guid.Empty } : null;
            //            model.Step1Form.OrganizationTypeId = Guid.NewGuid();
            //
            //            model.Step1Form.ShowContacts = showContacts;
            //            model.Step1Form.ShowPostalAddress = ShowPostalAddress;
            //            model.Step1Form.ShowVisitingAddress = ShowVisitingAddress;
            //
            //            model.Step1Form.Emails = itemListGenerator.Create<VmEmailData>(selectedEmails);
            //            model.Step1Form.PhoneNumbers = itemListGenerator.Create<VmPhone>(selectedEmails);
            //            model.Step1Form.WebPages = itemListGenerator.Create<VmWebPage>(selectedWebPages);
            //            model.Step1Form.VisitingAddresses = itemListGenerator.Create<VmAddressSimple>(visitingAddresses, c=> new VmAddressSimple { PostalCode = new VmPostalCode(), StreetType = AddressTypeEnum.Street.ToString()});
            //            model.Step1Form.PostalAddresses = itemListGenerator.Create<VmAddressSimple>(postalAddresess, c => new VmAddressSimple { StreetType = AddressTypeEnum.Foreign.ToString() });
            //
            //            var toTranslate = new List<VmOrganizationModel>() { model };
            //
            //            var translations = RunTranslationModelToEntityTest<VmOrganizationModel, OrganizationVersioned>(translators, toTranslate, unitOfWorkMock);
            //            var translation = translations.First();
            //
            //            CheckTranslation(model, model.Step1Form, translation);
        }

//        private void CheckTranslation(VmOrganizationModel source, VmOrganizationStep1 step1, OrganizationVersioned target)
//        {
//            target.Id.Should().NotBe(Guid.Empty);
//
//            var names = conversion.GetValidTextsSkipEmpty(step1.OrganizationName, step1.OrganizationAlternateName);
//            target.OrganizationNames.Count.Should().Be(Math.Max(names.Count, 1), "OrganizationNames");
//
//            var descriptions = conversion.GetValidTexts(step1.Description);
//            target.OrganizationDescriptions.Count.Should().Be(descriptions.Count, "OrganizationDescriptions");
//
//            if (step1.ParentId.HasValue)
//            {
//                target.ParentId.Should().NotBeEmpty();
//                target.ParentId.Should().Be(step1.ParentId.Value);
//            }
//
//            if (step1.Municipality?.Id != null)
//            {
//                target.MunicipalityId.Should().NotBeEmpty();
//                target.MunicipalityId.Should().Be(step1.Municipality?.Id);
//            }
//
//            if (step1.OrganizationTypeId.HasValue)
//            {
//                target.TypeId.Should().NotBeEmpty();
//                target.TypeId.Should().Be(step1.OrganizationTypeId.Value);
//            }
//
//            if (step1.ShowContacts)
//            {
//                target.OrganizationEmails.Count.Should().Be(step1.Emails.Count, "Emails");
//                target.OrganizationPhones.Count.Should().Be(step1.PhoneNumbers.Count, "Phone");
//                target.OrganizationWebAddress.Count.Should().Be(step1.WebPages.Count, "Web pages");
//
//                if (step1.ShowPostalAddress || step1.ShowVisitingAddress)
//                {
//                    target.OrganizationAddresses.Count.Should().Be(step1.PostalAddresses.Count + step1.VisitingAddresses.Count, "Adresses");
//                }
//            }
//        }

//        private VmOrganizationModel CreateModel()
//        {
//            return new VmOrganizationModel()
//            {
//                PublishingStatusId = Guid.NewGuid(),
//                Step1Form = new VmOrganizationStep1()
//            };
//        }
    }
}
