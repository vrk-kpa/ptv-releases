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
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;

using Xunit;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class LocationChannelWebPageTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public LocationChannelWebPageTranslatorTest()
        {
            translators = new List<object>
            {
                new ServiceChannelWebPageTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new WebPageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock<WebPage, VmUrl>(),
                RegisterTranslatorMock<WebPage, VmWebPage>(),
                RegisterTranslatorMock<IOrderable, IVmOrderable>(model => new ServiceChannelWebPage()),

            };
            RegisterDbSet(CreateCodeData<WebPageType>(typeof(WebPageTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelWebPage>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for WebPage vm - > entity
        /// </summary>
        /// <param name="type"></param>
        [Theory]
        [InlineData("")]
        [InlineData("webPage")]
        public void TranslateToEntity(string type)
        {
            var model = CreateModel();
            model.TypeId = type.GetGuid();

            var toTranslate = new List<VmWebPage> { model };

            var translations = RunTranslationModelToEntityTest<VmWebPage, ServiceChannelWebPage>(translators, toTranslate, unitOfWorkMockSetup.Object);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmWebPage source, ServiceChannelWebPage target)
        {
            target.Should().NotBe(Guid.Empty);
            target.WebPage.Should().NotBeNull();
            target.TypeId.Should().Be(source.TypeId ?? Guid.Empty);
        }


        private VmWebPage CreateModel()
        {
            return new VmWebPage { UrlAddress = "some.url" };
        }
    }
}
