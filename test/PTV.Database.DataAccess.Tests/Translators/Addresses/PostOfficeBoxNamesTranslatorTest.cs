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
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.DataAccess.Translators.Channels.V2.Common;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Addresses
{
    public class PostOfficeBoxNamesTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;

        public PostOfficeBoxNamesTranslatorTest()
        {
            translators = new List<object>()
            {
                new LocalizedPostOfficeBoxTranslator(ResolveManager, TranslationPrimitives),

//                RegisterTranslatorMock(new Mock<ITranslator<Language, string>>(), unitOfWorkMock),

            };
            RegisterDbSet(new List<PostOfficeBoxName>(), unitOfWorkMockSetup);

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for PostOfficeBoxNameTranslator vm - > entity
        /// </summary>
        /// <param name="pobox"></param>
        [Theory]
        [InlineData("poBox")]
        [InlineData("")]
        public void TranslateStreetAddress(string poBox)
        {
            var model = CreateModel(poBox, "sv");

            var translation = RunTranslationModelToEntityTest<VmLocalizedPostOfficeBox, PostOfficeBoxName>(translators, model, unitOfWorkMockSetup.Object);

            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmLocalizedPostOfficeBox source, PostOfficeBoxName target)
        {
            target.Should().NotBe(Guid.Empty);
            target.Name.Should().Be(source.PostOfficeBox);
            target.LocalizationId.Should().NotBe(Guid.Empty);
            target.LocalizationId.Should().Be(source.LocalizationId.IsAssigned() ? source.LocalizationId : DomainConstants.DefaultLanguage.GetGuid(), "Localization check");
        }


        private VmLocalizedPostOfficeBox CreateModel(string poBoxValue, string language)
        {
            var poBox = new Dictionary<string, string>();
            poBox.Add("fi", poBoxValue);
            return new VmLocalizedPostOfficeBox
            {
                LocalizationId = string.IsNullOrEmpty(language) ? Guid.Empty : language.GetGuid(),
                PostOfficeBox = poBoxValue
            };
        }
    }
}
