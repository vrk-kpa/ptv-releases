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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.DataAccess.Translators.OpenApi.Finto;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Tests.TestHelpers;
using FluentAssertions;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiLanguageCodeListTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public OpenApiLanguageCodeListTranslatorTest()
        {
            translators = new List<object>
            {
                new OpenApiLanguageCodeListTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiLanguageNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
            };
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = CreateModel();
            var toTranslate = new List<Language>() { entity };
            var translations = RunTranslationEntityToModelTest<Language, VmOpenApiCodeListItem>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(entity, translation);
        }

        private void CheckTranslations(Language source, VmOpenApiCodeListItem target)
        {
            target.Code.Should().Be(source.Code);
            target.Names.First().Value.Should().Be(source.Names.First().Name);
        }

        private Language CreateModel()
        {
            return new Language()
            {
                Code = TestDataFactory.TEXT,
                Names = new List<LanguageName>()
                {
                    new LanguageName()
                    {
                        Name = TestDataFactory.TEXT,
                        Localization = TestDataFactory.LocalizationFi()
                    }
                }
            };
        }
    }
}