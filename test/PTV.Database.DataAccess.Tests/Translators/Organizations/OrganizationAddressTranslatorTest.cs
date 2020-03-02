/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationAddressTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public OrganizationAddressTranslatorTest()
        {
            translators = new List<object>
            {
                new OrganizationAddressTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new AddressTypeCodeTranslator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock<Address, VmAddressSimple>(),
                RegisterTranslatorMock<IOrderable, IVmOrderable>(model => new OrganizationAddress()),

            };
            SetupTypesCacheMock<AddressCharacter>(typeof(AddressCharacterEnum));
            RegisterDbSet(new List<OrganizationAddress>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for OrganizationAddressTranslator vm - > entity
        /// </summary>
        /// <param name="addressCharacter">address type</param>
        [Theory]
        [InlineData(AddressCharacterEnum.Visiting)]
        [InlineData(AddressCharacterEnum.Postal)]
        public void TranslateOrganizationAddress(AddressCharacterEnum addressCharacter)
        {
            var model = CreateModel();
            model.AddressCharacter = addressCharacter;

            var translation = RunTranslationModelToEntityTest<VmAddressSimple, OrganizationAddress>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmAddressSimple source, OrganizationAddress target)
        {
            target.Should().NotBe(Guid.Empty);
            target.Address.Should().NotBeNull();
            target.CharacterId.Should().Be(source.AddressCharacter.ToString().GetGuid());
        }


        private VmAddressSimple CreateModel()
        {
            return new VmAddressSimple();
        }
    }
}
