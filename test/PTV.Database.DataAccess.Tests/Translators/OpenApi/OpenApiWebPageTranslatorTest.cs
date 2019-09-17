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
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiWebPageTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OpenApiWebPageTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;

            translators = new List<object>
            {
                new OpenApiWebPageTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives)
            };
            string[] codes = {"fi", "sv", "en"}; 
            RegisterDbSet(codes.Select(x => new Language { Code = x }).ToList(), unitOfWorkMockSetup);
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = CreateModel();
            var toTranslate = new List<WebPage>() { entity };
            var translations = RunTranslationEntityToModelTest<WebPage, V9VmOpenApiWebPage>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(entity, translation);
        }

        private void CheckTranslations(WebPage source, V9VmOpenApiWebPage target)
        {
            target.Id.Should().Be(source.Id);
            target.Value.Should().Be(source.Name);
            target.Url.Should().Be(source.Url);
            //target.Language.Should().Be(source.Localization.Code);
        }

        private WebPage CreateModel()
        {
            return new WebPage()
            {
                Id = Guid.NewGuid(),
                Name = TestDataFactory.WEBPAGE_NAME,
                Description = TestDataFactory.TEXT,
                Url = TestDataFactory.URI,
                Localization = TestDataFactory.LocalizationFi()
            };
        }

        [Fact]
        public void TranslateVmToEntity()
        {
            var model = CreateViewModel();
            var toTranslate = new List<V9VmOpenApiWebPage>() { model };
            var translations = RunTranslationModelToEntityTest<V9VmOpenApiWebPage, WebPage>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(model, translation);
        }

        private V9VmOpenApiWebPage CreateViewModel()
        {
            return new V9VmOpenApiWebPage()
            {
                 //Id = Guid.NewGuid(),
                 Value = TestDataFactory.WEBPAGE_NAME,
                 //Description = TestDataFactory.TEXT,
                 Url = TestDataFactory.URI,
                 Language = "fi"
            };
        }

        private void CheckTranslations(V9VmOpenApiWebPage source, WebPage target)
        {
            //target.Id.Should().Be(source.Id.Value);
            target.Name.Should().Be(source.Value);
            target.Url.Should().Be(source.Url);
            target.Localization.Code.Should().Be(source.Language);
        }
    }
}
