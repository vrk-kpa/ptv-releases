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
                RegisterTranslatorMock(new Mock<ITranslator<StreetName, VmAddressSimple>>(), unitOfWorkMock, address => new StreetName { Text = address.Street }),
                RegisterTranslatorMock(new Mock<ITranslator<PostOfficeBoxName, VmAddressSimple>>(), unitOfWorkMock, address => new PostOfficeBoxName { Text = address.PoBox }),

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
        /// <param name="addressType"></param>
        /// <param name="street"></param>
        /// <param name="pobox"></param>
        [Theory]
        [InlineData(StreetAddressTypeEnum.Street, AddressTypeEnum.Postal, "street", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        [InlineData(StreetAddressTypeEnum.Street, AddressTypeEnum.Visiting, "", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        [InlineData(StreetAddressTypeEnum.PostBox, AddressTypeEnum.Postal, "street", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        [InlineData(StreetAddressTypeEnum.PostBox, AddressTypeEnum.Visiting, "street", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
//        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Postal, "street", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
//        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Visiting, "street", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
//        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Postal, "", "pobox", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
//        [InlineData(StreetAddressTypeEnum.Both, AddressTypeEnum.Visiting, "", "", "11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        public void TranslateAddress(StreetAddressTypeEnum type, AddressTypeEnum addressType, string street, string pobox, string postalCodeId)
        {
            var model = CreateModel();
            model.AddressType = addressType;
            model.StreetType = type.ToString();
            model.Street = street;
            model.PoBox = pobox;
            model.PostalCode = string.IsNullOrEmpty(postalCodeId)
                ? null
                : new VmPostalCode {Id = postalCodeId.ParseToGuid() ?? Guid.Empty};

            var toTranslate = new List<VmAddressSimple>() { model };

            var translations = RunTranslationModelToEntityTest<VmAddressSimple, Address>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation, type);
        }


        private void CheckTranslation(VmAddressSimple source, Address target, StreetAddressTypeEnum addressType)
        {
            target.Should().NotBe(Guid.Empty);
            switch (addressType)
            {
                case StreetAddressTypeEnum.Street:

                    target.StreetNames.Count.Should().Be(1);
                    target.StreetNames.Select(x => x.Text).FirstOrDefault().Should().Be(source.Street);
                    target.PostOfficeBoxNames.Count.Should().Be(1);
                    target.PostOfficeBoxNames.Select(x => x.Text).FirstOrDefault().Should().BeEmpty();
                    break;
                case StreetAddressTypeEnum.PostBox:
                    target.StreetNames.Count.Should().Be(1);
                    target.StreetNames.Select(x => x.Text).FirstOrDefault().Should().BeEmpty();
                    target.PostOfficeBoxNames.Count.Should().Be(1);
                    target.PostOfficeBoxNames.Select(x => x.Text).FirstOrDefault().Should().Be(source.PoBox);
                    break;
                case StreetAddressTypeEnum.Both:
                    // not used now
                    target.StreetNames.Count.Should().Be(1);
                    target.PostOfficeBoxNames.Count.Should().Be(1);
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
