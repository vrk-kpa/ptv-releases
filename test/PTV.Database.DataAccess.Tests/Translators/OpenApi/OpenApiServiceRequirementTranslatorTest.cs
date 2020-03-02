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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Tests.TestHelpers;
using FluentAssertions;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceRequirementTranslatorTest : TranslatorTestBase
    {
        private const string REQUIREMENT_TEXT = "TEXT";
        private const string REQUIREMENT_MARKDOWN = REQUIREMENT_TEXT+"\n";
        private const string REQUIREMENT_JSON = "{\"entityMap\":{},\"blocks\":[{\"text\":\"" + REQUIREMENT_TEXT + "\",\"type\":\"unstyled\",\"key\":\"FUU4G\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";


        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OpenApiServiceRequirementTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;

            translators = new List<object>
            {
                new OpenApiServiceRequirementTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<Language, string>>(), unitOfWorkMock, language => TestDataFactory.LocalizationFi()),
            };
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = CreateModel();
            var toTranslate = new List<ServiceRequirement> { entity };
            var translations = RunTranslationEntityToModelTest<ServiceRequirement, VmOpenApiLanguageItem>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(translation);
        }

        private void CheckTranslations(VmOpenApiLanguageItem target)
        {
            target.Value.Should().NotBeNull();
        }

        private ServiceRequirement CreateModel()
        {
            return new ServiceRequirement
            {
                Requirement = REQUIREMENT_JSON,
            };
        }

        [Fact]
        private void TranslateVmToEntity()
        {
            var model = CreateViewModel();
            var toTranslate = new List<VmOpenApiLanguageItem> { model };
            var translations = RunTranslationModelToEntityTest<VmOpenApiLanguageItem, ServiceRequirement>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(model, translation);
        }

        private void CheckTranslations(VmOpenApiLanguageItem source, ServiceRequirement target)
        {
            target.Requirement.Should().NotBeNull();
            target.LocalizationId.Should().Be(LanguageCache.Get(source.Language));
        }

        private VmOpenApiLanguageItem CreateViewModel()
        {
            return new VmOpenApiLanguageItem
            {
                Value = REQUIREMENT_MARKDOWN,
                Language = TestDataFactory.LocalizationFi().Code
            };
        }
    }
}
