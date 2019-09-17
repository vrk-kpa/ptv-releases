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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Channels;
using Moq;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class PrintableFormChannelUrlTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public PrintableFormChannelUrlTranslatorTest()
        {
            translators = new List<object>()
            {
                new PrintableFormChannelUrlTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),

                RegisterTranslatorMock<Language, string>(),
                RegisterTranslatorMock<PrintableFormChannelUrlType, string>(),
                RegisterTranslatorMock<IOrderable, IVmOrderable>(model => new PrintableFormChannelUrl())

            };
            RegisterDbSet(new List<PrintableFormChannelUrl>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for PrintableFormChannelUrlTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslatePrintableFormChannelUrlToEntity()
        {
            var model = TestHelper.CreateVmWebPageUrlModel();
            var translation = RunTranslationModelToEntityTest<VmWebPage, PrintableFormChannelUrl>(translators, model, unitOfWorkMockSetup.Object);
            Assert.Equal(model.UrlAddress, translation.Url);
        }
    }
}
