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
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.DataAccess.Translators.Organizations.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Domain.Model.Models.V2.Organization;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationMainTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly ItemListModelGenerator itemListGenerator;
        private readonly TestConversion conversion;

        public OrganizationMainTranslatorTest()
        {
            SetupTypesCacheMock<PhoneNumberType>(typeof(PhoneNumberTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<AddressCharacter>(typeof(AddressCharacterEnum));

            translators = new List<object>()
            {
                new OrganizationBaseTranslator(ResolveManager, TranslationPrimitives, CacheManager, new CommonTranslatorHelper(CacheManager), new EntityDefinitionHelper(CacheManager)),

                RegisterTranslatorMock<OrganizationVersioned, VmAreaInformation>(),
                RegisterTranslatorMock<OrganizationVersioned, VmOrganizationHeader>(),
                RegisterTranslatorMock<OrganizationAddress, VmAddressSimple>(),
                RegisterTranslatorMock<Address, VmAddressSimple>(null, add => new VmAddressSimple()),
                RegisterTranslatorMock<OrganizationDescription, VmDescription>(),
                RegisterTranslatorMock<OrganizationEmail, VmEmailData>(),
                RegisterTranslatorMock<Email, VmEmailData>(),
                RegisterTranslatorMock<OrganizationName, VmName>(),
                RegisterTranslatorMock<Business, VmBusiness>(model => new Business (), entity => new VmBusiness()),
                RegisterTranslatorMock<OrganizationWebPage, VmWebPage>(),
                RegisterTranslatorMock<WebPage, VmWebPage>(),
                RegisterTranslatorMock<OrganizationPhone, VmPhone>(),
                RegisterTranslatorMock<Phone, VmPhone>(),
                RegisterTranslatorMock<OrganizationDisplayNameType, VmDispalyNameType>(),
                RegisterTranslatorMock<IDescription, string>(text => new OrganizationDescription { Description = text}),
            };

            RegisterDbSet(new List<OrganizationVersioned>(), unitOfWorkMockSetup);
            RegisterEntityForVersionManager<OrganizationVersioned, Organization>();

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
        }
        
        [Theory]
        [InlineData("", "", "", "", "","", true, 0, 0, 0, 0, 0)]
        [InlineData("fi;sv;en", "name", "altName", "description", "", "", false, 5, 3, 4, 0, 4)]
        [InlineData("fi;sv;en", "name", "", "description", "", "", true, 5, 3, 4, 8, 0)]
        [InlineData("fi", "name", "altName", "description", "", "", false, 5, 3, 4, 8, 4)]
        [InlineData("", "name", "altName", "description", "parentId", "municipality", true, 0, 0, 0, 0, 0)]
        [InlineData("fi;sv;en", "name", "altName", "description", "parentId", "municipality", false, 8, 6, 9, 7, 2)]
        [InlineData(null, "name", "altName", "description", "parentId", "municipality", true, 5, 3, 4, 8, 4)]
        public void TranslateOrganizationStep1ToEntityTest(string list, string name, string alternateName, string description, string parentOrganizationId, string data,
            bool isMain, int selectedEmails, int selectedPhoneNumbers, int selectedWebPages, int postalAddresess, int visitingAddresses)
        {
            var model = new VmOrganizationBase
            {
                IsAlternateNameUsedAsDisplayName = itemListGenerator.CreateList(list, x => x)?.ToList(),
                Name = itemListGenerator.CreateList(list, lang => new {lang, name})
                    ?.ToDictionary(x => x.lang, x => x.name + x.lang),
                AlternateName = itemListGenerator.CreateList(list, lang => new {lang, alternateName})
                    ?.ToDictionary(x => x.lang, x => x.alternateName + x.lang),
                Description = itemListGenerator.CreateList(list, lang => new {lang, description})
                    ?.ToDictionary(x => x.lang, x => x.description + x.lang),
                ParentId = parentOrganizationId.GetGuid(),
                IsMainOrganization = isMain,
                OrganizationType = Guid.NewGuid(),
                Oid = data,
                Emails = itemListGenerator
                    .CreateList(list, l => itemListGenerator.Create(selectedEmails, i => new VmEmailData {Email = l}))
                    ?.ToDictionary(x => x.First().Email),
                PhoneNumbers = itemListGenerator
                    .CreateList(list, l => itemListGenerator.Create(selectedPhoneNumbers, i => new VmPhone {Number = l}))
                    ?.ToDictionary(x => x.First().Number),
                WebPages = itemListGenerator
                    .CreateList(list,
                        l => itemListGenerator.Create(selectedWebPages, i => new VmWebPage() {UrlAddress = l}))
                    ?.ToDictionary(x => x.First().UrlAddress),
                VisitingAddresses = itemListGenerator.Create(visitingAddresses,
                    c => new VmAddressSimple {StreetType = AddressTypeEnum.Street.ToString()}),
                PostalAddresses = itemListGenerator.Create(postalAddresess,
                    c => new VmAddressSimple {StreetType = AddressTypeEnum.Foreign.ToString()})
            };
            
            var translation = RunTranslationModelToEntityTest<VmOrganizationBase, OrganizationVersioned>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmOrganizationBase source, OrganizationVersioned target)
        {
            target.Id.Should().NotBe(Guid.Empty);

            target.OrganizationNames.Count.Should().Be((source.Name?.Count + source.AlternateName?.Count) ?? 0, "OrganizationNames");
            target.OrganizationDescriptions.Count.Should().Be(source.Description?.Count ?? 0, "OrganizationDescriptions");

            target.ParentId.Should().Be(source.IsMainOrganization ? null : source.ParentId);
            target.TypeId.Should().Be(source.OrganizationType);

            target.OrganizationEmails.Count.Should().Be(source.Emails?.Sum(x => x.Value.Count) ?? 0, "Emails");
            target.OrganizationPhones.Count.Should().Be(source.PhoneNumbers?.Sum(x => x.Value.Count) ?? 0, "Phone");
            target.OrganizationWebAddress.Count.Should().Be(source.WebPages?.Sum(x => x.Value.Count) ?? 0, "Web pages");
            target.OrganizationAddresses.Count.Should().Be(
                (source.PostalAddresses?.Count + source.VisitingAddresses?.Count) ?? 0, "Addresses");
        }
        
        /// <summary>
        /// test for OrganizationStep1Translator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateOrganizationToViewModel()
        {
            var localizationId = "fi".GetGuid();
            var entity = new OrganizationVersioned()
                {
                   ParentId = Guid.NewGuid(),
                   TypeId = Guid.NewGuid(),
                   Business = new Business(),
                   Municipality = new Municipality(),
                   OrganizationAddresses = new List<OrganizationAddress>()
                   {
                       new OrganizationAddress { CharacterId = AddressCharacterEnum.Postal.ToString().GetGuid(), Address = new Address {OrderNumber = 2} },
                       new OrganizationAddress { CharacterId = AddressCharacterEnum.Visiting.ToString().GetGuid(), Address = new Address {OrderNumber = 1} },
                   },
                   OrganizationDescriptions = new List<OrganizationDescription>()
                   {
                       new OrganizationDescription()
                       {
                           LocalizationId = localizationId,
                           Type = new DescriptionType() { Code = DescriptionTypeEnum.Description.ToString() }
                       }
                   },
                   OrganizationNames = new List<OrganizationName>()
                   {
                       new OrganizationName() { Type = new NameType() { Code = NameTypeEnum.Name.ToString() }, LocalizationId = localizationId },
                       new OrganizationName() { Type = new NameType() { Code = NameTypeEnum.AlternateName.ToString() }, LocalizationId = localizationId }
                   },
                   OrganizationDisplayNameTypes = new List<OrganizationDisplayNameType>()
                   {
                       new OrganizationDisplayNameType() { DisplayNameTypeId = NameTypeEnum.AlternateName.ToString().GetGuid(), LocalizationId = localizationId  }
                   },
                   OrganizationEmails = new List<OrganizationEmail>()
                   {
                       new OrganizationEmail
                       {
                           Email = new Email { LocalizationId = localizationId }
                       }
                   },
                   OrganizationPhones = new List<OrganizationPhone>()
                   {
                       new OrganizationPhone { Phone = new Phone { LocalizationId = localizationId } }
                   },
                   OrganizationWebAddress = new List<OrganizationWebPage>()
                   {
                       new OrganizationWebPage {WebPage = new WebPage { LocalizationId = localizationId }}
                   },
            
                
            };
            var translation = RunTranslationEntityToModelTest<OrganizationVersioned, VmOrganizationBase>(translators, entity);
            translation.OrganizationType.Should().Be(entity.TypeId);
            translation.Business.Should().NotBeNull();
            translation.VisitingAddresses.Should().HaveCount(1);
            translation.PostalAddresses.Should().HaveCount(1);
            translation.Emails.Should().HaveCount(1);
            translation.PhoneNumbers.Should().HaveCount(1);
            translation.WebPages.Should().HaveCount(1);
            translation.IsAlternateNameUsedAsDisplayName.Should().NotBeEmpty();
        }

    }
}
