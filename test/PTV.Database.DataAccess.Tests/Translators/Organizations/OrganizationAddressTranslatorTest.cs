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
using Microsoft.Extensions.Caching.Memory;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;
using PTV.Database.DataAccess.Translators.Organizations;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationAddressTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;

        public OrganizationAddressTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>()
            {
                new OrganizationAddressTranslator(ResolveManager, TranslationPrimitives),
                new AddressTypeCodeTranslator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock(new Mock<ITranslator<Address, VmAddressSimple>>(), unitOfWorkMock),

            };
            RegisterDbSet(CreateCodeData<AddressType>(typeof(AddressTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(new List<OrganizationAddress>(), unitOfWorkMockSetup);

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for OrganizationAddressTranslator vm - > entity
        /// </summary>
        /// <param name="addressType">addtess type</param>
        [Theory]
        [InlineData(AddressTypeEnum.Visiting)]
        [InlineData(AddressTypeEnum.Postal)]
        public void TranslateOrganizationAddress(AddressTypeEnum addressType)
        {
            var model = CreateModel();
            model.AddressType = addressType;

            var toTranslate = new List<VmAddressSimple>() { model };

            var translations = RunTranslationModelToEntityTest<VmAddressSimple, OrganizationAddress>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmAddressSimple source, OrganizationAddress target)
        {
            target.Should().NotBe(Guid.Empty);
            target.Address.Should().NotBeNull();
            target.Type.Should().NotBeNull();
            target.Type.Code.Should().Be(source.AddressType.ToString());
        }


        private VmAddressSimple CreateModel()
        {
            return new VmAddressSimple();
        }
    }
}
