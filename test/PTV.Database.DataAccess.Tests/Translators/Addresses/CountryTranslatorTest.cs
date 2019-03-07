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
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Addresses
{
    public class CountryTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;

        public CountryTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>()
            {
                new CountryCodeTranslator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock(new Mock<ITranslator<Language, string>>(), unitOfWorkMock),

            };
//            RegisterDbSet(CreateTypeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMock);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);

            RegisterDbSet(CreateCodeData<Country>(typeof(CountryCode)), unitOfWorkMockSetup);
//            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMock);

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        /// <param name="addressType"></param>
        /// <param name="street"></param>
        /// <param name="postalCode"></param>
        /// <param name="postOffice"></param>
        /// <param name="pobox"></param>
        [Theory]
        [InlineData(CountryCode.FI)]
        public void TranslateStreetAddress(CountryCode model)
        {
            var toTranslate = new List<string>() { model.ToString() };

            var translations = RunTranslationModelToEntityTest<string, Country>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model.ToString(), translation);
        }

        [Fact]
        public void TranslateInvalidCountry()
        {
            var toTranslate = new List<string> { "" };

            Assert.Throws<DbEntityNotFoundException>(() => RunTranslationModelToEntityTest<string, Country>(translators, toTranslate, unitOfWorkMock));
        }


        private void CheckTranslation(string source, Country target)
        {
            target.Should().NotBe(Guid.Empty);
            target.Code.Should().Be(source);
        }


//        private VmAddressSimple CreateModel()
//        {
//            return new VmAddressSimple();
//        }
    }
}
