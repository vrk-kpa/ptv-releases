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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Addresses
{
    public class AddressTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public AddressTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            translators = new List<object>()
            {
                new AddressTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                RegisterTranslatorMock(new Mock<ITranslator<Coordinate, VmCoordinate>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<PostalCode, VmAddressSimple>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<Country, string>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<StreetName, VmAddressSimple>>(), unitOfWorkMock, address => new StreetName { Name = address.Street }),
                RegisterTranslatorMock(new Mock<ITranslator<PostOfficeBoxName, VmAddressSimple>>(), unitOfWorkMock, address => new PostOfficeBoxName { Name = address.PoBox.Values.First() }),
                RegisterTranslatorMock(new Mock<ITranslator<AddressPostOfficeBox, VmAddressSimple>>(), unitOfWorkMock, address => new AddressPostOfficeBox { PostOfficeBoxNames = new List<PostOfficeBoxName>{ new PostOfficeBoxName { Name = address.PoBox.Values.First() } } }),
//                RegisterTranslatorMock(new Mock<ITranslator<AddressStreet, VmAddressSimple>>(), unitOfWorkMock, address => new AddressStreet { StreetNames = new List<StreetName> { new StreetName { Name = address.Street } } } ),
//                RegisterTranslatorMock(new Mock<ITranslator<AddressForeign, VmAddressSimple>>(), unitOfWorkMock, address => new AddressForeign { ForeignTextNames = new List<AddressForeignTextName> { new AddressForeignTextName { Name = address.Street } } } ),
            };
//            RegisterDbSet(CreateTypeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMock);
//            RegisterDbSet(CreateTypeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMock);

            RegisterDbSet(new List<Address>(), unitOfWorkMockSetup);
//            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMock);

            unitOfWorkMock = unitOfWorkMockSetup.Object;
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        /// <param name="addressCharacter"></param>
        /// <param name="street"></param>
        /// <param name="pobox"></param>
        [Theory]
        [InlineData(AddressTypeEnum.Street, AddressCharacterEnum.Postal, "street", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        [InlineData(AddressTypeEnum.Street, AddressCharacterEnum.Visiting, "", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        [InlineData(AddressTypeEnum.PostOfficeBox, AddressCharacterEnum.Postal, "street", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        [InlineData(AddressTypeEnum.PostOfficeBox, AddressCharacterEnum.Visiting, "street", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        [InlineData(AddressTypeEnum.Foreign, AddressCharacterEnum.Postal, "street", "", "")]
        [InlineData(AddressTypeEnum.Foreign, AddressCharacterEnum.Visiting, "street", "", "")]
        //        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Postal, "street", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        //        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Visiting, "street", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        //        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Postal, "", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        //        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Visiting, "", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        public void TranslateAddress(AddressTypeEnum type, AddressCharacterEnum addressCharacter, string street, string pobox, string postalCodeId)
        {
            var model = CreateModel();
            model.AddressCharacter = addressCharacter;
            model.StreetType = type.ToString();
            //            model.Street = street;
            var poBox = new Dictionary<string, string>();
            poBox.Add("fi", pobox);
            model.PoBox = poBox;
            model.PostalCode = string.IsNullOrEmpty(postalCodeId)
                ? null
                : new VmPostalCode {Id = postalCodeId.ParseToGuid() ?? Guid.Empty};

            var toTranslate = new List<VmAddressSimple>() { model };

            var translations = RunTranslationModelToEntityTest<VmAddressSimple, Address>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation, type);
        }


        private void CheckTranslation(VmAddressSimple source, Address target, AddressTypeEnum addressType)
        {
            target.Should().NotBe(Guid.Empty);
            switch (addressType)
            {
                case AddressTypeEnum.Street:

                    target.AddressStreets.First().StreetNames.Count.Should().Be(1);
//                    target.AddressStreets.First().StreetNames.Select(x => x.Name).FirstOrDefault().Should().Be(source.Street);
                    break;
                case AddressTypeEnum.PostOfficeBox:
                    target.AddressPostOfficeBoxes.First().PostOfficeBoxNames.Count.Should().Be(1);
                    target.AddressPostOfficeBoxes.First().PostOfficeBoxNames.Select(x => x.Name).FirstOrDefault().Should().Be(source.PoBox.Values.First());
                    break;
                case AddressTypeEnum.Foreign:
                    target.AddressForeigns.First().ForeignTextNames.Count.Should().Be(1);
//                    target.AddressForeigns.First().ForeignTextNames.Select(x => x.Name).FirstOrDefault().Should().Be(source.Street);
                    break;
            }
            target.Country.Should().NotBeNull();
        }


        private VmAddressSimple CreateModel()
        {
            return new VmAddressSimple();
        }
    }
}
