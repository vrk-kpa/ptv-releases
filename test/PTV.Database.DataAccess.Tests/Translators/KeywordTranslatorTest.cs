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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class KeywordTranslatorTest : TranslatorTestBase
    {
        private KeywordTranslator keywordTranslator;

        public KeywordTranslatorTest()
        {
            keywordTranslator = new KeywordTranslator(ResolveManager, TranslationPrimitives);
        }

        /// <summary>
        /// test for KeywordTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateKeywordToVm()
        {
            var keyword = CreateKeyword();
            var toTranslate = new List<Keyword> { keyword };
            var translations = RunTranslationEntityToModelTest<Keyword, VmKeywordItem>(new List<ITranslator> { keywordTranslator }, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(keyword.Name, translation.Name);
            Assert.Equal(keyword.Id, translation.Id);

        }

        /// <summary>
        /// test for KeywordTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateVmToKeyword()
        {
            var keyword = CreateVmKeyword();
            var toTranslate = new List<VmKeywordItem> { keyword };
            var translations = RunTranslationModelToEntityTest<VmKeywordItem, Keyword>(new List<ITranslator> { keywordTranslator }, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(keyword.Name, translation.Name);
            translation.Id.Should().NotBeEmpty();
        }

        private Keyword CreateKeyword()
        {
            return new Keyword
            {
                Name = "testKeyword",
                Id = Guid.NewGuid()
            };
        }
        private VmKeywordItem CreateVmKeyword()
        {
            return new VmKeywordItem
            {
                Name = "KeyWord"
            };
        }

    }
}
