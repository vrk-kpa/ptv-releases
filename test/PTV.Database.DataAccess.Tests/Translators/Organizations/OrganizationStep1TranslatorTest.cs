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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Caches;
using System;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationStep1TranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OrganizationStep1TranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var languageCacheMock = new Mock<ILanguageCache>();
            languageCacheMock.Setup(i => i.Filter(It.IsAny<Guid>(), It.IsAny<LanguageCode>())).Returns(true);
            languageCacheMock.Setup(i => i.Filter(It.IsAny<List<ILocalizable>>(), It.IsAny<LanguageCode>())).Returns((List<ILocalizable> i) => i.FirstOrDefault());
            var languageCache = languageCacheMock.Object;
            var cacheManagerMock = new Mock<ICacheManager>();
            cacheManagerMock.Setup(i => i.LanguageCache).Returns(languageCache);
            var cacheManager = cacheManagerMock.Object;
            translators = new List<object>()
            {
                new OrganizationStep1Translator(ResolveManager, TranslationPrimitives, cacheManager),

                RegisterTranslatorMock(new Mock<ITranslator<Municipality, VmMunicipality>>(), unitOfWorkMock, toViewModelFunc: x => new VmMunicipality()),
                RegisterTranslatorMock(new Mock<ITranslator<Business, VmBusiness>>(), unitOfWorkMock, toViewModelFunc: x => new VmBusiness()),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationEmail, VmEmailData>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationPhone, VmPhone>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationWebPage, VmWebPage>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<Address, VmAddressSimple>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<PublishingStatusType, VmPublishingStatus>>(), unitOfWorkMock),
            };
        }

        /// <summary>
        /// test for OrganizationStep1Translator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateOrganizationToViewModel()
        {
            var toTranslate = new List<Organization>
            {
                new Organization()
                {
                   DisplayNameType = new NameType() { Code = NameTypeEnum.AlternateName.ToString(), Id = Guid.NewGuid()},
                   ParentId = Guid.NewGuid(),
                   TypeId = Guid.NewGuid(),
                   Business = new Business(),
                   Municipality = new Municipality(),
                   OrganizationAddresses = new List<OrganizationAddress>()
                   {
                       new OrganizationAddress() { Type = new AddressType() {  Code = AddressTypeEnum.Postal.ToString() } },
                       new OrganizationAddress() { Type = new AddressType() {  Code = AddressTypeEnum.Visiting.ToString()} }
                   },
                   OrganizationDescriptions = new List<OrganizationDescription>()
                   {
                       new OrganizationDescription() { Type = new DescriptionType() { Code = DescriptionTypeEnum.Description.ToString() } }
                   },
                   OrganizationNames = new List<OrganizationName>()
                   {
                       new OrganizationName() { Type = new NameType() { Code = NameTypeEnum.Name.ToString() } },
                       new OrganizationName() { Type = new NameType() { Code = NameTypeEnum.AlternateName.ToString() } }
                   },
                   OrganizationEmails = new List<OrganizationEmail>()
                   {
                       new OrganizationEmail()
                   },
                   OrganizationPhones = new List<OrganizationPhone>()
                   {
                       new OrganizationPhone()
                   },
                   OrganizationWebAddress = new List<OrganizationWebPage>()
                   {
                       new OrganizationWebPage()
                   }

                }
            };
            var translations = RunTranslationEntityToModelTest<Organization, VmOrganizationStep1>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.First().ParentId, translation.ParentId);
            Assert.Equal(toTranslate.First().TypeId, translation.OrganizationTypeId);
            translation.Business.Should().NotBeNull();
            translation.Municipality.Should().NotBeNull();
            translation.VisitingAddresses.Should().HaveCount(1);
            translation.PostalAddresses.Should().HaveCount(1);
            translation.Emails.Should().HaveCount(1);
            translation.PhoneNumbers.Should().HaveCount(1);
            translation.WebPages.Should().HaveCount(1);
            translation.ShowContacts.Should().BeTrue();
            translation.IsAlternateNameUsedAsDisplayName.Should().BeTrue();
            translation.ShowPostalAddress.Should().BeTrue();
            translation.ShowVisitingAddress.Should().BeTrue();
        }
    }
}
