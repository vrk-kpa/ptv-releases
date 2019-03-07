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
    public class OpenApiServiceDescriptionTranslatorTest : TranslatorTestBase
    {
        private const string SERVICE_DESRIPTION = "ServiceDescription";
        private const string SERVICE_DESRIPTION_MARKDOWN = SERVICE_DESRIPTION + "\n";
        private const string SERVICE_DESRIPTION_JSON = "{\"entityMap\":{},\"blocks\":[{\"text\":\"" + SERVICE_DESRIPTION + "\",\"type\":\"unstyled\",\"key\":\"FUU4G\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";

        private readonly List<object> translators;

        public OpenApiServiceDescriptionTranslatorTest()
        {
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            translators = new List<object>
            {
                new OpenApiServiceDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives)
            };
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = CreateModel();
            var toTranslate = new List<ServiceDescription>() { entity };
            var translations = RunTranslationEntityToModelTest<ServiceDescription, VmOpenApiLocalizedListItem>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(entity, translation);
        }

        private void CheckTranslations(ServiceDescription source, VmOpenApiLocalizedListItem target)
        {
            target.Value.Should().NotBeNullOrEmpty();
            target.Type.Should().Be(CacheManager.TypesCache.GetByValue<DescriptionType>(source.TypeId));
            target.Language.Should().Be(CacheManager.LanguageCache.GetByValue(source.LocalizationId));
        }

        private ServiceDescription CreateModel()
        {
            return new ServiceDescription()
            {
                Description = SERVICE_DESRIPTION_JSON,
                TypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()),
                LocalizationId = CacheManager.LanguageCache.Get("sv")
            };
        }

        [Fact]
        public void TranslateVmToEntity()
        {
            var model = CreateViewModel();
            var toTranslate = new List<VmOpenApiLocalizedListItem>() { model };
            var translations = RunTranslationModelToEntityTest<VmOpenApiLocalizedListItem, ServiceDescription>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(model, translation);
        }

        private void CheckTranslations(VmOpenApiLocalizedListItem source, ServiceDescription target)
        {
            target.Description.Should().NotBeNullOrEmpty();
            target.TypeId.Should().Be(CacheManager.TypesCache.Get<DescriptionType>(source.Type));
            target.LocalizationId.Should().Be(CacheManager.LanguageCache.Get(source.Language));
        }

        private VmOpenApiLocalizedListItem CreateViewModel()
        {
            return new VmOpenApiLocalizedListItem()
            {
                Type = DescriptionTypeEnum.Description.ToString(),
                Language = "fi",
                Value = SERVICE_DESRIPTION_MARKDOWN,
            };
        }
    }
}
