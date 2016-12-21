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
using Microsoft.EntityFrameworkCore;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class OrganizationServiceWebPageTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OrganizationServiceWebPageTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var cacheManager = SetupCacheManager();
            translators = new List<object>
            {
                new OrganizationServiceWebPageTranslator(ResolveManager, TranslationPrimitives, cacheManager.Object),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives),
                new WebPageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationServiceWebPage, string>>(), unitOfWorkMock),

            };

            RegisterDbSet(CreateCodeData<WebPageType>(typeof(WebPageTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(Enum.GetNames(typeof(LanguageCode)).Select(x => new Language { Code = x }).ToList(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("Test")]
        [InlineData("")]
        public void TranslateToEntityTest(string name)
        {
            var model = new VmServiceProducerDetail {Link = name};
            var toTranslate = new List<VmServiceProducerDetail> { model };

            translators.Add(
                RegisterTranslatorMock(new Mock<ITranslator<WebPage, VmServiceProducerDetail>>(),
                    unitOfWorkMock, detail => new WebPage {Url = detail.Link})
                );
            var translations = RunTranslationModelToEntityTest<VmServiceProducerDetail, OrganizationServiceWebPage>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmServiceProducerDetail source, OrganizationServiceWebPage target)
        {
            target.Type.Should().NotBeNull("Type");
            target.Type.Code.Should().Be(WebPageTypeEnum.HomePage.ToString());
            target.WebPage.Should().NotBeNull("WebPage");
            target.WebPage.Url.Should().Be(source.Link);
        }

        private VmListItem CreateModel()
        {
            return new VmListItem()
            {
                Id = Guid.NewGuid()
            };
        }

    }
}
