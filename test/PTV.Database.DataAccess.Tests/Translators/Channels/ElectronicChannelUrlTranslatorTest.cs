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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Channels;
using System;
using Moq;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Common;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class ElectronicChannelUrlTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public ElectronicChannelUrlTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>
            {
                new ElectronicChannelUrlTranslator(ResolveManager, TranslationPrimitives),
                new WebPageUrlTranslator(ResolveManager, TranslationPrimitives)
//                RegisterTranslatorMock(new Mock<ITranslator<ElectronicChannelUrl, VmChannelAttachment>>(), unitOfWorkMock),
            };

            RegisterDbSet(new List<ElectronicChannelUrl>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<WebPage>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ElectronicChannelUrlTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateElectronicChannelUrlToEntity()
        {
            var model = TestHelper.CreateVmWebPageUrlModel();
            var toTranslate = new List<VmWebPage> { model };

            var translations = RunTranslationModelToEntityTest<VmWebPage, ElectronicChannelUrl>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(model.UrlAddress, translation.WebPage.Url);
        }
    }
}
