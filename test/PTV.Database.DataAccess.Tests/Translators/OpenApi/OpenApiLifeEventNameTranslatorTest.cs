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

using PTV.Database.DataAccess.Translators.OpenApi.Finto;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using FluentAssertions;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiLifeEventNameTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public OpenApiLifeEventNameTranslatorTest()
        {
            translators = new List<object>
            {
                new OpenApiLifeEventNameTranslator(ResolveManager, TranslationPrimitives, CacheManager)
            };
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = CreateModel();
            var toTranslate = new List<LifeEventName>() { entity };
            var translations = RunTranslationEntityToModelTest<LifeEventName, VmOpenApiLanguageItem>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(entity, translation);
        }

        private void CheckTranslations(LifeEventName source, IVmOpenApiLanguageItem target)
        {
            target.Value.Should().Be(source.Name);
            //target.Language.Should().Be(source.Localization.Code);
        }

        private LifeEventName CreateModel()
        {
            return new LifeEventName()
            {
                Name = TestDataFactory.LIFE_EVENT_NAME
            };
        }
    }
}