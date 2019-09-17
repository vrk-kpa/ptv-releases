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
using Newtonsoft.Json;
using PTV.Database.DataAccess.Translators.Localization;
using PTV.Database.DataAccess.Translators.Mass;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Domain.Model.Models.V2.Mass;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Localization
{
    public class LocalizationTextsTranslatorTest : TranslatorTestBase
    {
        private List<object> RegisterTranslators()
        {
            var translators = new List<object>
            {
                new LocalizationTextsTranslator(ResolveManager, TranslationPrimitives)
            };

            return translators;
        }

        [Theory]
        [MemberData(nameof(TranslateToEntityData))]
        public void TranslateToEntity(string language, int[] indexes, string[] languagesInDb)
        {
            var model = new VmLanguageMessages
            {
                LanguageCode = language,
                Texts = indexes
                    .Select(index => new {Key = $"key{index}", Text = $"text {index} {language}"})
                    .ToDictionary(x => x.Key, x => x.Text)
            };

            var databaseData = languagesInDb.Select(l => new Model.Models.Localization{ LanguageCode = l, Texts = "{}", CreatedBy = "db" }).ToList();
            RegisterDbSet(databaseData, unitOfWorkMockSetup);
            var translators = RegisterTranslators();

            var translation = RunTranslationModelToEntityTest<IVmLanguageMessages, Model.Models.Localization>(
                translators,
                model,
                unitOfWorkMockSetup.Object);

            if (languagesInDb.Contains(language))
            {
                databaseData.FirstOrDefault(x => x.LanguageCode == language).Should().Be(translation);
                translation.CreatedBy.Should().Be("db");
            }
            else
            {
                translation.CreatedBy.Should().BeNullOrEmpty();
            }
            Check(model, translation);
        }

        private void Check(IVmLanguageMessages source, Model.Models.Localization result)
        {
            result.Texts.Should().NotBeNull();
            var resultTexts = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Texts);
            resultTexts.Should().HaveCount(source.Texts.Count);
            resultTexts.Should().ContainKeys(source.Texts.Keys);
            resultTexts.Should().ContainValues(source.Texts.Values);
            result.LanguageCode.Should().Be(source.LanguageCode);
        }
        
        
        public static IEnumerable<object[]> TranslateToEntityData()
        {
            var allData = new List<object[]>
            {
                new object[] { "fi", new [] { 1, 2}, new string[0] },
                new object[] { "fi", new [] { 1, 2}, new [] { "sv" } },
                new object[] { "fi", new [] { 1, 2}, new [] { "fi" } },
                new object[] { "fi", new [] { 1, 2}, new [] { "fi", "sv" } },
            };

            return allData;
        }
        
        [Fact]
        public void TranslateToModel()
        {
            var texts = new Dictionary<string, string> { { "key1", "value1" }};
            var entity = new Model.Models.Localization
            {
                LanguageCode = "fi",
                Texts = JsonConvert.SerializeObject(texts)
            };

            var translators = RegisterTranslators();

            var translation = RunTranslationEntityToModelTest<Model.Models.Localization, IVmLanguageMessages>
            (
                translators,
                entity
            );

            Check(entity, translation, texts);
        }

        private void Check(Model.Models.Localization source, IVmLanguageMessages result,
            Dictionary<string, string> texts)
        {
            result.Texts.Should().NotBeNull();
            result.Texts.Should().HaveCount(texts.Count);
            result.Texts.Should().ContainKeys(texts.Keys);
            result.Texts.Should().ContainValues(texts.Values);
            result.LanguageCode.Should().Be(source.LanguageCode);
        }
    }
}